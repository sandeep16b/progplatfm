extern alias SharedOldServiceBus; 
using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Program.MembershipClient;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Exceptions;
using iTextSharp.text;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text.pdf;
using MassTransit;
using NLog;
using SharedOldServiceBus::Abim.Enterprise.Core.ServiceBus.Notification;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Abim.Platform.Program.App.Util.Constants;
using static Abim.Platform.Program.Resources.ProgramResourceConstants; 

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class HelperService
    /// </summary>
    public class HelperService : IHelperService
    {
        #region Properties

        /// <summary>
        /// 
        /// </summary>
        protected static string ProfileHostUrl = System.Configuration.ConfigurationManager.AppSettings["ProfileHostUrl"];

        /// <summary>
        /// 
        /// </summary>
        protected static string PrintLetterQueue = System.Configuration.ConfigurationManager.AppSettings["ExactTarget:PrintLetterQueue"] ?? "Not found";

        /// <summary>
        /// Enviroment
        /// </summary>
        protected static string Enviroment = System.Configuration.ConfigurationManager.AppSettings["Abim.Common.Env"] ?? "Not found";

        /// <summary>
        /// static Logger
        /// </summary>
        protected internal ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the bus.
        /// </summary>
        /// <value>
        /// The bus.
        /// </value>
        protected IBusControl Bus { get; set; }

        /// <summary>
        /// Gets or sets the access token
        /// </summary>
        protected IAccessTokenService AccessTokenService { get; set; }

        /// <summary>
        /// Profile Inter service
        /// </summary>
        protected IMembershipClientService MembershipClientService { get; set; }

        /// <summary>
        /// Registration Interservice
        /// </summary>
        protected IRegistrationInterservice RegistrationInterservice { get; set; }

        private string AccessToken => AccessTokenService.GetAccessToken();
        #endregion

        /// <summary>
        /// HelperService
        /// </summary>
        /// <param name="bus"></param>
        /// <param name="accessTokenService"></param>
        /// <param name="membershipClientService"></param>
        /// <param name="registrationInterservice"></param>
        public HelperService(IBusControl bus,
                             IAccessTokenService accessTokenService,
                             IMembershipClientService membershipClientService,
                             IRegistrationInterservice registrationInterservice)
        {
            Bus = bus;
            AccessTokenService = accessTokenService;
            MembershipClientService = membershipClientService;
            RegistrationInterservice = registrationInterservice;
        }

        #region Triggered Communication Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="communicationType"></param>
        /// <param name="memberId">Member id of the diplomate</param>
        /// <param name="credentialService">Credential Service instance</param>
        /// <returns></returns>
        public async Task TriggeredCommunication(Credential credential, string communicationType, Guid memberId, ICredentialService credentialService)
        {
            if (Util.Constants.TriggeredCommunication.EarnedMBMCertLetter.ToString().Equals(communicationType))
            {
                int totalCount;
                var pageDefinition = new PageDefinition
                {
                    PageIndex = 1,
                    PageSize = 100
                };

                var credentials = credentialService.SearchByMemberId(memberId, pageDefinition, out totalCount);
                var isCosponsoredOnly = totalCount > 0 && credentials.All(_ => _.IsCosponsored);

                // Prevent the Earned MBM Letter trigger Comm for Co-sponsored only candiates. PBI 212286
                if (!isCosponsoredOnly)
                {
                    await TriggeredCommunicationEarnedMBMLetter(credential, memberId);
                }
            }
            else
            {
                throw new ArgumentException($"HelperService.TriggeredCommunication was called with invalid argument for communicationType:'{communicationType}'");
            }
        }

        /// <summary>
        /// TriggeredCommunication_Reactivate_Certifications
        /// </summary>
        /// <param name="certNames"></param>
        /// <param name="memberId"></param>
        /// <returns></returns>
        public async Task TriggeredCommunication_Reactivate_Certifications(IList<string> certNames, Guid memberId)
        {
            //get user's profile
            var profile = await RetryHelper.RetryTask(() => MembershipClientService.GetProfileByMemberIdAsync(memberId), () => MembershipClientService.GetAccessToken()).ConfigureAwait(false);

            await Bus.Publish(new NotificationEvent()
            {
                TemplateExternalKey = TriggeredCommunicationTemplateExternalKey.Reactivate_Certification, // 43168
                EmailAddress = profile.EmailAddress,
                Parameters = new Dictionary<string, string> {
                    { "LastName", profile.Name.LastName},
                    { "CertificationNames", GetDelimitedCertNames(certNames, "<br />", true) },
                    { "CertificationNames_TV", GetDelimitedCertNames(certNames, ", ", false) },
                    { "EmailAddress", profile.EmailAddress},
                    { "SubscriberKey", profile.AbimId},
                    { "IID", profile.AbimId},
                    // Enviroment --
                    { "Env", Enviroment } 
                }
            });

            await Task.FromResult<object>(null);
        }

        #region Private methods
        /// <summary>
        /// TriggeredCommunicationEarnedMBMLetter
        /// </summary>
        /// <returns></returns>
        private async Task<Task> TriggeredCommunicationEarnedMBMLetter(Credential credential, Guid memberId)
        {
            try
            {
                //get user's profile
                var profile = await RetryHelper.RetryTask(() => MembershipClientService.GetProfileByMemberIdAsync(memberId), () => MembershipClientService.GetAccessToken()).ConfigureAwait(false);

                var address = profile.Addresses.FirstOrDefault(a => a.IsPrimary);

                if (address == null)
                    Log.Warn($"No primary address record is found for MemberId:'{credential.MemberId}', Notification wouldn't be send to Triggered Comunication");

                string state = "";
                string country; 

                if (address.CountryId?.ToUpper() == "US" || address.CountryId?.ToUpper() == "CA")
                {
                    //New API does not return the Region object as part of Address Objects and requering to get the Region
                    RegionResource matchingRegionByRegionId = await GetMatchingRegionByRegionId(address).ConfigureAwait(false);
                    state = matchingRegionByRegionId != null ? matchingRegionByRegionId.Code : "";
                }
             
                //New API does not return the Country object as part of Address Objects and requering to get the Country
                CountryResource matchingCountry = await GetCountryByCountryId(address).ConfigureAwait(false);
                country = matchingCountry != null ? matchingCountry.Name : ""; 

                Tuple<DateTime, DateTime> dates = CalculateIssueReturnDates(credential.NewestIssuance.IssuanceDate.Date);

                string Env = PrintLetterQueue.Contains("QA") ? "QA" : "PROD";

                await Bus.Publish(new NotificationEvent()
                {
                    TemplateExternalKey = TriggeredCommunicationTemplateExternalKey.EarnedMBMCertLetter, //"ts_earned_MBM_letter"
                    EmailAddress = PrintLetterQueue, //it is going to be send to printer 
                    Parameters = new Dictionary<string, string>
                    {
                        {"Specialty", credential.Certification.Name },
                        { "LastName", profile.Name.LastName },
                        { "FirstName", profile.Name.FirstName},
                        { "MiddleName", profile.Name.MiddleName??""},
                        { "AddressLine1", address.Address1},
                        { "AddressLine2", address.Address2??""},
                        { "AddressLine3", address.Address3??""},
                        { "City", address.City},
                        { "Zip", address.PostalCode},
                        { "IID", profile.AbimId},
                        { "InternalId", profile.AbimId},
                        { "CertificateIssueDate", credential.NewestIssuance.IssuanceDate.ToShortDateString()},
                        { "CertIssueByDate",  dates.Item1.ToShortDateString()},
                        { "FormReturnByDate", dates.Item2.ToShortDateString()},
                        //-- veries for US and other contries
                        { "State", state},
                        { "Country", country},
                        // Enviroment --
                        { "Env", Env } //"QA" or "PROD" // not sure how it was used in Triggered Com
                    }
                });

            }
            catch (Exception ex)
            {
                Log.Error(ex.InnerException.Message);
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// GetCountryByCountryId
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<CountryResource> GetCountryByCountryId(ProfileAddressResource address)
        {
            var countries = await RetryHelper.RetryTask(() => MembershipClientService.GetCountriesAsync(), () => MembershipClientService.GetAccessToken()).ConfigureAwait(false);
            var matchingCountry = countries?.FirstOrDefault(r => r.Code?.ToUpper() == address?.CountryId?.ToUpper());
            return matchingCountry;
        }

        /// <summary>
        /// GetMatchingRegionByRegionId
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public async Task<RegionResource> GetMatchingRegionByRegionId(ProfileAddressResource address)
        {
            var regions = await RetryHelper.RetryTask(() => MembershipClientService.GetCountryRegionsAsync(address.CountryId), () => MembershipClientService.GetAccessToken()).ConfigureAwait(false);
            var matchingRegionByRegionId = regions?.FirstOrDefault(r => r.Id == address?.RegionId);
            return matchingRegionByRegionId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static Tuple<DateTime, DateTime> CalculateIssueReturnDates(DateTime date)
        {
            DateTime CertIssueByDate = new DateTime();
            DateTime FormReturnByDate = new DateTime();

            // 1/1 -3/31
            if (date.Month >= 1 && date.Month <= 3)
            {
                FormReturnByDate = new DateTime(date.Year, 5, 1);
                CertIssueByDate = new DateTime(date.Year, 7, 15);
            }
            // 4/1 - 6/30
            else if (date.Month >= 4 && date.Month <= 6)
            {
                FormReturnByDate = new DateTime(date.Year, 8, 1);
                CertIssueByDate = new DateTime(date.Year, 10, 15);
            }
            // 7/1 - 9/15
            else if (date.Month >= 7 && date <= new DateTime(date.Year, 09, 15))
            {
                FormReturnByDate = new DateTime(date.Year, 10, 15);
                CertIssueByDate = new DateTime(date.Year, 12, 31);
            }
            // 9/16-12/31
            else if (date >= new DateTime(date.Year, 09, 16) && date.Month <= 12)
            {
                FormReturnByDate = new DateTime(date.Year + 1, 02, 15);
                CertIssueByDate = new DateTime(date.Year + 1, 04, 30);
            }

            return new Tuple<DateTime, DateTime>(CertIssueByDate, FormReturnByDate);
        }

        private string GetDelimitedCertNames(IList<string> certNames, string delimiter, bool addDelimiterToEnd)
        {
            return string.Join(delimiter, certNames) + (addDelimiterToEnd ? delimiter : "");
        }

        #endregion

        #endregion

        #region Getting Profile data 

        /// <summary>
        /// GetMemberIdByAbimId
        /// </summary>
        /// <param name="abimId"></param>
        /// <returns></returns>
        public async Task<Guid> GetMemberIdByAbimId(string abimId)
        {
            try
            { 
                var profile = await RetryHelper.RetryTask(() => MembershipClientService.GetProfileByAbimIdAsync(abimId),() => MembershipClientService.GetAccessToken()).ConfigureAwait(false);
                
                if (profile != null)
                    return profile.Id;
                else
                    return Guid.Empty;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return Guid.Empty;
            }
        }

        #endregion

        #region VOC Letter

        /// <summary>
        /// CreateVocLetter
        /// </summary>
        /// <param name="mergeData"></param>
        /// <returns></returns>
        public MemoryStream CreateVocLetter(VocPdfData mergeData)
        {
            try
            {
                IPdfMergeData data = new VocPdfMergeData(mergeData);
                string template = "VOCTemplate1.pdf";
                using (FileStream fs = new FileStream(template, FileMode.Open, FileAccess.Read))
                {
                    return FillPdf(fs, data);
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }

        }

        /// <summary>
        /// GetVocLetterContent
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="credentials"></param>
        /// <returns>Data needed to write into the PDF as an object</returns>
        public async Task<VocPdfData> GetVocLetterContent(ProfileResource profile, IEnumerable<Credential> credentials)
        {
            VocPdfData pdfData = new VocPdfData();
            string text = String.Empty;
            StringBuilder initialCerts = new StringBuilder();
            StringBuilder currentCertifications = new StringBuilder();
            StringBuilder certified = new StringBuilder();
            StringBuilder address = new StringBuilder();
            string cityStateZip = String.Empty;
            string isMaintainingMOC = "No";
            bool isAllCertsCertified = false;
            bool isActive = true;
            bool? isFocusPractice;

            try
            {
                var middleName = !String.IsNullOrWhiteSpace(profile.Name?.MiddleName) ? " " + profile.Name?.MiddleName : "";

                //Name
                string name = $"Dr. {profile.Name?.FirstName}{middleName} {profile.Name?.LastName}";

                //Address
                if (profile.Addresses != null)
                {
                    //Skip if Address Count is 0
                    if (profile.Addresses.Count() > 0)
                    {
                        var profileaddress = profile.Addresses.FirstOrDefault(r => r.IsPrimary);
                        if (profileaddress != null)
                        {
                            if (!String.IsNullOrEmpty(profileaddress.Address1))
                            {
                                address.AppendLine($"{name}");
                                address.AppendLine($"{profileaddress.Address1}");
                            }
                            if (!String.IsNullOrEmpty(profileaddress.Address2) && !(String.IsNullOrEmpty(address.ToString())))
                                address.AppendLine($"{profileaddress.Address2}");
                            if (!String.IsNullOrEmpty(profileaddress.Address3) && !(String.IsNullOrEmpty(address.ToString())))
                                address.AppendLine($"{profileaddress.Address3}");
                            if (!String.IsNullOrEmpty(profileaddress.City) && !(String.IsNullOrEmpty(address.ToString())))
                            {
                                cityStateZip = $"{profileaddress.City}";
                            }

                            string state = ""; 

                            if (profileaddress.CountryId?.ToUpper() == "US" || profileaddress.CountryId?.ToUpper() == "CA")
                            {
                                //New API does not return the Region object as part of Address Objects and requering to get the Region
                                RegionResource matchingRegionByRegionId = await GetMatchingRegionByRegionId(profileaddress).ConfigureAwait(false);
                                state = matchingRegionByRegionId != null ? matchingRegionByRegionId.Code : "";
                            }
 
                           cityStateZip += $", {state} {profileaddress.PostalCode}";

                            if (!String.IsNullOrEmpty(address.ToString()))
                                address.AppendLine($"{cityStateZip}");
                        }
                    }
                }

                //Intial Certs, Certifications,Certified
                IList<CertificationPublicResource> certs = credentials.Where(a => a.HasIssuances)
                                                                        .Select(
                                                                            cred => new CertificationPublicResource
                                                                            {
                                                                                Name = cred.Certification.Code != CertificationCode.FocusedPracticeHospitalMedicine ? cred.Certification.Name : CertificationName.IMwithFPHM,
                                                                                InitialIssuanceDate = cred.OldestIssuance.IssuanceDate,
                                                                                Status = cred.ProperIssuance.IssuanceStatus,
                                                                                MaintenanceStatus = cred.ProperIssuance.MaintenanceStatus
                                                                            }).OrderBy(a => a.InitialIssuanceDate).ToList();

                // check if dr. has FPHM and it is seleted to maintained and it is active
                isFocusPractice = credentials
                    .Any(a => a.HasIssuances
                              && a.Certification.Code == CertificationCode.FocusedPracticeHospitalMedicine
                              && a.NewestIssuance.IssuanceStatus == IssuanceStatusType.Active
                              && a.SelectedToMaintain);

                int certsCount = certs.Count();
                int inactiveCerts = certs.Count(c => c.Status == IssuanceStatusType.Inactive);
                int mocCerts = certs.Count(c => c.MaintenanceStatus == MaintenanceStatusType.Maintained);
                //if all certs are inactive thenn "Inactive"
                if (certsCount > 0 && inactiveCerts == certsCount)
                {
                    isActive = false;
                }
                else if (certsCount > 0)
                {
                    initialCerts.AppendLine($"INITIAL CERTIFICATION");
                    certs.ToList().ForEach(c => initialCerts.AppendLine($"{c.Name}: {c.InitialIssuanceDate.Year}"));

                    var certifiedCerts = certs.Where(c => c.Status == IssuanceStatusType.Active
                          // include FPHM only if selected to maintain
                          && ((c.Name == CertificationName.IMwithFPHM && isFocusPractice == true) ||
                                  // exclude IM if FPHM is selected to be maintained
                                  (c.Name == CertificationName.IM && isFocusPractice != true) ||
                                  // show all others if it is not IM or FPHM
                                  (c.Name != CertificationName.IMwithFPHM && c.Name != CertificationName.IM)));

                   

                    certifiedCerts.ToList().ForEach(c => currentCertifications.AppendLine($"{c.Name}: <b>{c.IssuanceStatusWithModifier}</b>"));
                    if (certifiedCerts.Any())
                        currentCertifications.AppendLine();
                    var notCertifiedCerts = certs.Where(c => c.Status != IssuanceStatusType.Active && c.Name != CertificationName.IMwithFPHM);
                    notCertifiedCerts.ToList().ForEach(c => currentCertifications.AppendLine($"{c.Name}: <b>{c.IssuanceStatusWithModifier}</b>"));

                    if (certifiedCerts.Count()>0 && notCertifiedCerts.Count()==0)
                    {
                        isAllCertsCertified = true;
                    }
                }

                //Maintaining MOC
                if (mocCerts >= 1)
                {
                    isMaintainingMOC = "Yes";
                }

                //Date
                pdfData.Date = DateTime.Now.ToString("MMMM dd, yyyy");
                //Name
                pdfData.Name = name;
                //Address
                pdfData.Address = address.ToString();
                //Intial Certifications
                pdfData.IntialCertifications = initialCerts.ToString();
                //Certifications
                pdfData.CurrentCertifications = currentCertifications.ToString();
                //Certified
                pdfData.Certified = certified.ToString();
                //Maintaining MOC
                pdfData.isMaintainingMOC = isMaintainingMOC;
                //pdfData.Text = String.Format(text, pdfData.Date);
                //pdfData.Text = text;
                pdfData.Text = DateTime.Now.ToString("MMMM dd, yyyy");
                pdfData.isActive = isActive;
                pdfData.isAllCertsCertified = isAllCertsCertified;
                pdfData.certsCount = certsCount;
                return pdfData;
            }

            catch (Exception ex)
            {
                Log.Error(ex);
                return null;
            }
        }

        #region Private methods
        private MemoryStream FillPdf(Stream templateStream, IPdfMergeData mergeItems)
        {
            MemoryStream outputStream = new MemoryStream();
            var pagesAll = new List<byte[]>();

            // Hold individual pages Here:
            byte[] pageBytes;

            // Read the form template for each item to be output:
            var templateReader = new PdfReader(templateStream);
            using (var tempStream = new MemoryStream())
            {
                PdfStamper stamper = new PdfStamper(templateReader, tempStream);
                stamper.FormFlattening = true;
                AcroFields fields = stamper.AcroFields;
                stamper.Writer.CloseStream = false;

                // Grab a reference to the Dictionary in the current merge item:
                var fieldVals = mergeItems.MergeFieldValues;

                // Walk the Dictionary keys, fnid teh matching AcroField, 
                // and set the value:
                foreach (string name in fieldVals.Keys)
                {
                    if ((name != "LetterBody")
                        && (name != "InitialCerts")
                        && (name != "MaintainingMOC")
                        && (name != "Certifications"))
                    {
                        fields.SetField(name, fieldVals[name]);
                    }
                }

                var pos = fields.GetFieldPositions("LetterBody");
                AddHTMLToContent(mergeItems.MergeFieldValues["LetterBody"], stamper.GetOverContent(pos[0].page), pos);


                // If we had not set the CloseStream property to false, 
                // this line would also kill our memory stream:
                stamper.Close();

                // Reset the stream position to the beginning before reading:
                tempStream.Position = 0;

                // Grab the byte array from the temp stream . . .
                pageBytes = tempStream.ToArray();

                // And add it to our array of all the pages:
                pagesAll.Add(pageBytes);
            }

            // Create a document container to assemble our pieces in:
            Document mainDocument = new Document(PageSize.A4);

            // Copy the contents of our document to our output stream:
            var pdfCopier = new PdfSmartCopy(mainDocument, outputStream);

            // Once again, don't close the stream when we close the document:
            pdfCopier.CloseStream = false;

            mainDocument.Open();
            foreach (var pageByteArray in pagesAll)
            {
                // Copy each page into the document:
                mainDocument.NewPage();
                pdfCopier.AddPage(pdfCopier.GetImportedPage(new PdfReader(pageByteArray), 1));
            }
            pdfCopier.Close();

            // Set stream position to the beginning before returning:
            outputStream.Position = 0;
            return outputStream;
        }

        private void AddHTMLToContent(String htmlText, PdfContentByte contentBtye, IList<AcroFields.FieldPosition> pos)
        {
            Paragraph par = new Paragraph();
            ColumnText c1 = new ColumnText(contentBtye);
            try
            {
                par.Font = FontFactory.GetFont(BaseFont.TIMES_ROMAN, 10);
                List<IElement> elements = HTMLWorker.ParseToList(new StringReader(htmlText), null);
                foreach (IElement element in elements)
                {
                    par.Add(element);
                }

                c1.AddElement(par);
                c1.SetSimpleColumn(pos[0].position.Left, pos[0].position.Bottom, pos[0].position.Right, pos[0].position.Top);
                c1.Go(); //very important!!!
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        #endregion

        #endregion

        #region RegistrationInterservice

        /// <summary>
        /// GetMostRecentExamTypeByCode
        /// </summary>
        /// <param name="MemberId"></param>
        /// <param name="CertificationId"></param>
        /// <returns></returns>
        public async Task<ExamType> GetMostRecentExamTypeByCode(Guid MemberId, Guid CertificationId)
        {

            try
            {

                var registrations = await RetryHelper.RetryTask(() => RegistrationInterservice.GetUserRegistrations(AccessToken, MemberId), () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);

                //The email from Natalie on 7/12 says we should only be using exams with a result of Fail, Incomplete, Indeterminate, Invalidated, Pass, or UnableToTest.
                var mostRecentExamType = registrations?.Data?.Where(a => a.CertificationId == CertificationId
                                    && a.ExamType.ToEnum() != ExamType.Cert
                                    && (a.ExamResult.Result.ToEnum() == ExamResultType.Fail ||
                                         a.ExamResult.Result.ToEnum() == ExamResultType.Incomplete ||
                                         a.ExamResult.Result.ToEnum() == ExamResultType.Indeterminate ||
                                         a.ExamResult.Result.ToEnum() == ExamResultType.Invalidated ||
                                         a.ExamResult.Result.ToEnum() == ExamResultType.Pass ||
                                          a.ExamResult.Result.ToEnum() == ExamResultType.UnableToTest
                                         ))?
                                         .OrderByDescending(c => c.AdministrationDate)
                                         .FirstOrDefault()?
                                         .ExamType
                                         .ToEnum();

                return (mostRecentExamType.HasValue ? mostRecentExamType.Value : ExamType.Moc);

            }
            catch (UnsuccessfulStatusException ex)
            {
                Log.Warn($"UnsuccessfulStatusException from GetUserRegistrations for MemberId:'{MemberId}' : '{ex}'");
                return ExamType.Moc;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                throw;
            }
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        #endregion
    }

}
