using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions.Registration;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services.Impl
{
    public partial class ProgramRulesService
    {

        #region Retrieval functions

        /// <summary>
        /// Get all the activiites of users in between start date and end date.
        /// </summary>
        /// <returns></returns>
        private async Task<List<ActivityResource>> GetUser10yearsActivities()
        {
            // we are adding two days and then Date would go back to 00:00, 
            // Example: 07/23/2020 11:50:13, add two days 07/25/2020 11:50:13 and then .Date would set back 07/25/2020 00:00:00. So. we would need complete next day, which is 7/24/2020
            DateTime endDate = ProcessingDate.AddDays(2).Date; 
            DateTime startDate = endDate.AddYears(-10);

            return await GetUserAcvities(startDate, endDate);
        }

        /// <summary>
        /// Get all the activiites of users in between start date and end date.
        /// </summary>
        /// <returns></returns>
        private async Task<List<ActivityResource>> GetUserAcvities(DateTime startDate, DateTime endDate)
        {

            // get all relevant last 11 year user activities up to current date
            var result = (await RetryHelper.RetryTask(() => ProductInterservice.GetUserActivities(AccessToken, MemberId,startDate,endDate), 
                                                          () => AccessTokenService.GetAccessToken()).ConfigureAwait(false)).Data;

            if (result.Count == 0)
            {
                var error = $"No user Activities found for MemberId:'{MemberId}' in range '{startDate.ToShortDateString()}-{endDate.ToShortDateString()}'";
                Logger.Info(error);
            }

            return result;
        }

        private async Task<UserRegistrationsAndCMPRegistrationsResource> GetAllRegistrationsAndCMPRegistrationsForUser()
        {
            return await RetryHelper.RetryTask(() => RegistrationInterservice.GetAllRegistrationsAndCMPRegistrationsForUser(AccessToken, MemberId),
                                               () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);
        }

        /// <summary>
        ///  Get registration record by registrationId (GUID)
        /// </summary>
        /// <returns></returns>
        private async Task<RegistrationResource> GetRegistrationById(Guid registrationId)
        {
            return await RetryHelper.RetryTask( () => RegistrationInterservice.GetRegistrationById(AccessToken, registrationId),
                                                () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);
        }

        /// <summary>
        /// Get CMP registration by registrationId (GUID)
        /// </summary>
        /// <param name="registrationId"></param>
        /// <returns></returns>
        private async Task<CMPRegistrationResource> GetCMPRegistrationById(Guid registrationId)
        {
            return await RetryHelper.RetryTask(() => RegistrationInterservice.GetCMPRegistrationById(AccessToken, registrationId),
                                    () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);
        }

        /// <summary>
        /// GetLongitudinalEnrollments
        /// </summary>
        /// <returns></returns>
        private async Task<LongitudinalEnrollmentCollectionResource> GetLongitudinalEnrollments()
        {
            return await RetryHelper.RetryTask(() => RegistrationInterservice.GetLongitudinalEnrollmentsByMemberId(AccessToken, MemberId),
                        () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);
        }

        #endregion

        #region Local CA functions
        /// <summary>
        /// Determine when the 10 year is due. Note that this isn't the stored exam due date, just a waorking data element that we can use as needed
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        public DateTime? Compute10YearExamDueDate(Credential credential)
        {
            if (Registrations.Count() == 0) return null;

            DateTime? ExamDueDate10Year = null;
            DateTime? recent10YearExamPassDate = Registrations.GetRecentExamPassDate(ExamType.Moc, credential.Certification.ExternalId);
            DateTime? recent2YearExamPassDate = Registrations.GetRecentExamPassDate(ExamType.Kci, credential.Certification.ExternalId);

            int specialTLAdjustmentYear = GetSpecialTLAdjustmentYear(credential);

            // odd ball people like board members who never took the initial cert exam and were just given a certificate. So we could use the date of certification
            if (!recent10YearExamPassDate.HasValue && credential.HasIssuances)
                recent10YearExamPassDate = credential.NewestIssuance.IssuanceDate;

            // if still no recent10YearExamPassDate and it is not initialFPHM then not much can done, just retun as null
            if (!recent10YearExamPassDate.HasValue && !credential.IsInitialFPHM)
                return null;

            int last10YearPassPlus10 = recent10YearExamPassDate.HasValue ? recent10YearExamPassDate.Value.Year + 10 : 0;
            int last2YearPassPlus2 = recent2YearExamPassDate.HasValue ? recent2YearExamPassDate.Value.Year + 2 : 0;

            switch (credential.CredentialCategory)
            {
                case CredentialCategoryType.GrandFather:
                    //If grandfather set 10 year due date to greatest(2023, last 10 year pass + 10)
                    ExamDueDate10Year = new DateTime(Math.Max(2023, last10YearPassPlus10), 12, 31);
                    break;
                case CredentialCategoryType.TimeLimited:
                    int certExpirationYear = credential.ExpirationDate?.Year ?? 0;
                    // If timelimited set 10 year due date to greatest(max(certificate_exp_date), year of last 10 year pass + 10, special time limited adjustment[if one exists])
                    int MaxYear = new[] { certExpirationYear, last10YearPassPlus10, specialTLAdjustmentYear }.Max();
                    ExamDueDate10Year = new DateTime(MaxYear, 12, 31);
                    break;
                case CredentialCategoryType.MustBeMaintained:
                    //If mbm set 10 year due date to greatest(year of last 10 year pass + 10, special time limited adjustment[if one exists])
                    ExamDueDate10Year = new DateTime(Math.Max( last10YearPassPlus10 , specialTLAdjustmentYear), 12, 31);
                    break;
                case CredentialCategoryType.InitialFPHM:
                    //If initialfphm, set 10 year due date to year greatest(last 10 year MOC pass of HM + 10, last 2 year kci pass of HM+2). | old one: If initialfphm, set 10 year due date to last 10 year pass of HM +10, 
                    //  if none exists, then set the next 10 year due date based on the IM certificate using the same rules
                    //     If IM grandfather set 10 year due date to greatest(2023, last IM 10 year pass + 10)
                    //     If IM timelimited set 10 year due date to greatest(max(IM certificate_exp_date), last IM 10 year pass + 10)
                    //     If IM mbm set 10 year due date to last 10 year pass +10
                    if (last10YearPassPlus10 > 0 || last2YearPassPlus2 > 0)
                        ExamDueDate10Year = new DateTime(Math.Max(last10YearPassPlus10, last2YearPassPlus2), 12, 31);
                    else
                    {
                        var IMcredential = CredentialService.GetCredentialByMemberAndCode(credential.MemberId, "IM");
                        ExamDueDate10Year = Compute10YearExamDueDate(IMcredential);
                    }

                    break;
                case CredentialCategoryType.Unknown:
                default:
                    Log.Error("CredentialCategoryType cannot be determined");
                    break;
            }

            return ExamDueDate10Year;
        }

        /// <summary>
        /// Set a few fields to negative concequences for a credential
        /// </summary>
        /// <param name="credential"></param>
        private void ExpireActiveIssuances(ref Credential credential)
        {
            
            //expire active issuance (which includes setting maintenance status to false)  if certificate if time-limited and past expiration date or must be maintained 
            if ((credential.IsTimelimited 
                && credential.ExpirationDate.HasValue 
                && ProcessingDate.Date >= credential.ExpirationDate.Value.Date)
                 || credential.IsMBM)
            {
                foreach( var issuance in  credential.Issuances.Where(a => a.IssuanceStatus == IssuanceStatusType.Active))
                {
                    issuance.IssuanceStatus = IssuanceStatusType.Expired;
                    issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
                    issuance.ExpirationDate = issuance.ExpirationDate?? ProcessingDate;
                }
                credential.IsActive = false;
            }

            if (credential.IsGrandfather)
            {
                credential.NewestIssuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            }
        }

        #region Project 1382:  Program Rule 16 - Forced to the Long Form Pathway Removed (pbi 129326)
        /*
        /// <summary>
        /// Forced credential to MOC and reset ExamFailCount and set new Exam Due Date
        /// </summary>
        /// <param name="credential"></param>
        private void Forced10YearPath (ref Credential credential)
        {
            credential.ForcedPathway = true;
            credential.Pathway = PathwayType.MOC;
            credential.ExamFailCount = 0;
            credential.ExamDueDate = new DateTime(credential.LookbackDate.Value.Year + 1, 12, 31);
        }
        */
        #endregion

        /// <summary>
        /// Pbi 108756: Backend process: revised (tl cert due date) loading of moc/kci exam results
        /// -----------------------------------------------------------------------------------------
        /// If there is/was a time-limited issuance that expires/expired 2007 or later
        ///     And there isn’t a subsequent time-limited cert issued
        ///     And there is a passed moc exam for that discipline with an administration date that occurred after the year of the issuance and before 1/1/14
        ///     Then the special timelimited adjustment is set to year of that issuances expirationdate+10
        ///     Otherwise the special timelimited adjustment is does not exist
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        private int GetSpecialTLAdjustmentYear(Credential credential)
        {

            int retunValue = 0;

            // Pbi 108756: Backend process: revised (tl cert due date) loading of moc/kci exam results
            //***************************************************************************************
            // Special time limited adjustment[if one exists])
            //***************************************************************************************
            // If there is/ was a time-limited issuance that expires / expired 2007 or later
            // And there isn’t a subsequent time-limited cert issued
            // And there is a passed moc exam for that discipline with an administration date that occurred after the year of the issuance and before 1 / 1 / 14

            // If there is/ was a time-limited issuance that expires / expired 2007 or later
            var isssuanceTlExpiresAfter2007 = credential.Issuances
                                               .Where(i => i.Duration == DurationType.Timelimited
                                                        && i.ExpirationDate.HasValue
                                                        && i.ExpirationDate.Value.Year >= 2007)
                                                .OrderByDescending(p => p.ExpirationDate)
                                                .FirstOrDefault();

            if (isssuanceTlExpiresAfter2007 != null)
            {
                // And there is a passed moc exam for that discipline with an administration date that occurred after the year of the issuance and before 1/1/14
                var mocPassExistAfterTlIssuance = Registrations
                                                        .Where(p => p.Result == ExamResultType.Pass.ToString())
                                                        .Where(e => e.CertificationId == credential.Certification.ExternalId)
                                                        .Where(r => r.ExamType != null && r.ExamType.ToEnum() == ExamType.Moc)
                                                        .Where(a => a.AdministrationDate.Year > isssuanceTlExpiresAfter2007.IssuanceDate.Year && a.AdministrationDate.Year < 2014)
                                                        .Any();

                if (mocPassExistAfterTlIssuance)
                {
                    // Then the special timelimited adjustment is set to year of that issuances expirationdate + 10
                    retunValue = isssuanceTlExpiresAfter2007.ExpirationDate.Value.Year + 10;
                }
            }

            return retunValue;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="eventDate"></param>
        /// <param name="includeBatchProcessingList"></param>
        /// <returns></returns>
        private IEnumerable<DateTime> ComputeEventDateList(DateTime eventDate, bool includeBatchProcessingList=false)
        {

            IList<DateTime> BatchProcessingList = new List<DateTime>();

            // find Exam test dates after eventDate
            var registraionEvents = Registrations.Where(r => r.ExamTestDate().Date > eventDate.Date)
                                                .Where(p => p.Result != ExamResultType.Pending.ToString()) // not intrested in pending results
                                                .Select(e => e.ExamTestDate().Date);

            // find New subspecialty issuances after eventDate
            var newSubspecialtyEvents = Credentials.EarnedNewSubspecialtyInitialCertEvents(eventDate.Date);

            // to find if it is 9/1 and if yes then set BatchProcessingEvent
            if (eventDate.Day == 1 && eventDate.Month == 9 && includeBatchProcessingList)
                BatchProcessingList.Add(eventDate);

            // find user activities after event date (for back-dated cituation)
            return UserActivities.Where(a => a.CompletedDate.HasValue && eventDate.Date <= a.CompletedDate.Value.Date)
                                    // add temp ActivityResource record which would be used in foreach()
                                    .DefaultIfEmpty(new ActivityResource() { CompletedDate = eventDate })
                                    .Select(e => e.CompletedDate.Value.Date)
                                    .Union(registraionEvents)
                                    .Union(newSubspecialtyEvents)
                                    .Union(BatchProcessingList)
                                    .OrderBy(o => o.Date);
        }

        /// <summary>
        /// Reinstate certificate (pbi 136522)
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="lookbackDate"></param>
        /// <returns></returns>
        private bool ReinstateCertificate (Credential credential, DateTime lookbackDate)
        {
            Log.Debug($"Entering ReinstateCertificate for credential {credential.Id}: Latest Issuance status is {credential.NewestIssuance?.IssuanceStatus}, duration is {credential.NewestIssuance?.Duration}, expiration date is {credential.NewestIssuance?.ExpirationDate?.Date}, credential grace period start is {credential.GracePeriodStartDate?.Date}, lookback date is {lookbackDate}.");
            if (credential.NewestIssuance?.IssuanceStatus == IssuanceStatusType.Expired
                && (credential.NewestIssuance?.Duration == DurationType.Timelimited || credential.NewestIssuance?.Duration == DurationType.Continuous) //Continuous added per Bug 197723
                && credential.NewestIssuance?.ExpirationDate?.Date == lookbackDate.Date
                && credential.GracePeriodStartDate?.Date == new DateTime(lookbackDate.Year+1,1,1) )
            {
                Log.Debug($"Reinstating credential {credential.Id}");
                credential.NewestIssuance.IssuanceStatus = IssuanceStatusType.Active;
                credential.NewestIssuance.ExpiredDate = null; //PBI : 142310 Reinstate certificate - set expired to null
                credential.NewestIssuance.SetModified("ReinstateFromGracePeriod");
                credential.NewestIssuance.HasChanged = true;

                if (credential.IsActive == false)
                {
                    credential.IsActive = true;
                    credential.SetModified("ReinstateFromGracePeriod");
                    credential.HasChanged = true;
                }

                //****  determine maintenacne status *****

                // Met 2 year requirement (pbi 135236)
                var meets2YearRequirements = MaintenanceStatus(credential, ProcessingDate, true, true);
                
                // Met all the non-exam requirements (pbi 135009)
                var meetsNonExamRequirements = NonExamRequirement(credential, lookbackDate, ProcessingDate,false);

                if (meetsNonExamRequirements && meets2YearRequirements.MeetStepRule)
                {
                    credential.NewestIssuance.MaintenanceStatus = MaintenanceStatusType.Maintained;
                    credential.NewestIssuance.SetModified("ReinstateFromGracePeriod");
                    credential.NewestIssuance.HasChanged = true;
                }

                return true;
            }
            Log.Debug($"Did not reinstating credential {credential.Id}");
            return false;
        }

        #endregion

    }
}
