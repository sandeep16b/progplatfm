using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Enterprise.Core.Profile.Resource.Constants;
using Abim.Platform.Program.App.Classes;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi;
using AutoMapper;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Routing;
using Abim.Enterprise.Core.Profile.Interservice.Util.Extensions;
using static Abim.Platform.Program.Resources.ProgramResourceConstants;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    [RoutePrefix(Routes.Prefix.PhysicianCertification)]
    public class PhysicianCredentialsController : ControllerBase
    {
        #region Static Members
        /// <summary>
        /// 
        /// </summary>
        protected static string ProfileHostUrl = System.Configuration.ConfigurationManager.AppSettings["ProfileHostUrl"];
        #endregion

        #region Properties

        /// <summary>
        /// The credential service
        /// </summary>
        protected ICredentialService CredentialService { get; set; }

        /// <summary>
        /// Profile Inter service
        /// </summary>
        protected IProfileInterservice ProfileInterService { get; set; }

        /// <summary>
        /// static Logger
        /// </summary>
        protected internal ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// 
        /// </summary>
        protected IAccessTokenService AccessTokenService { get; set; }

        /// <summary>
        /// AccessToken for interservices comuncation
        /// </summary>
        private string AccessToken => AccessTokenService.GetAccessToken();

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicianCredentialsController"/> class.
        /// </summary>
        /// <param name="credentialService"></param>
        /// <param name="accessTokenSingletonWraper"></param>
        /// <param name="profileInterService"></param>
        public PhysicianCredentialsController(      ICredentialService credentialService,
                                                    IAccessTokenService accessTokenSingletonWraper,
                                                    IProfileInterservice profileInterService)
        {
            CredentialService = credentialService;
            AccessTokenService = accessTokenSingletonWraper;
            ProfileInterService = profileInterService;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the services.
        /// </summary>
        protected void DisposeServices(HttpRequestMessage message)
        {
            message.RegisterForDispose(CredentialService);
        }

        #endregion

        #region Retrieve Endpoints

        /// <summary> 
        /// GetCredentialsByAbimId
        /// </summary>
        /// <param name="abimId"></param>
        /// <returns></returns> 169418
        [HttpGet, AllowAnonymous]
        [Route(Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId, Name = Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId)]
        public async Task<IHttpActionResult> GetPhysicianCredentialsByAbimId(string abimId)
        {
            if (string.IsNullOrEmpty(abimId))
                return BadRequest("AbimId cannot be null or empty");

            try
            {
                //get user's profile
                var profile = await RetryHelper.RetryTask(() => ProfileInterService.SearchProfilesByAbimId(AccessToken, ProfileHostUrl, abimId), () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);

                var allCredentials = await CredentialService.SearchByMemberIdAsync(profile.Id)
                                        .ConfigureAwait(false);

                // filter only ABIM certificates (not ABIM issued cert (Issuance.SourceId=1)
                // Bug 164024 : VOC pages should only display ABIM certificates
                var ABIMcredentials = allCredentials.Where(c => c.Certification.Source.Code == "ABIM" && c.IsCosponsored == false).ToList();

                // Bug 222785 : Cosponsored certs appearing on VOC page
                // PBi 210151 : Remove cosponsored "credentials" from the VoC page
                // The condition below only for co-sponsored diplomates. If regular diplomate does not have any creds then it should work as before
                if (!ABIMcredentials.Any() && allCredentials.Any())
                    return Content(HttpStatusCode.NotFound, $"No user exists with AbimId:'{abimId}'.");

                return Ok(PhysicianCertificationsMapping(profile, ABIMcredentials, Url));

            }
            catch (UnsuccessfulStatusException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return Content(HttpStatusCode.NotFound, $"No user exists with AbimId:'{abimId}'.");
                else
                {
                    HandleExceptionLogging(ex, $"abimId: '{abimId}'");
                    return BadRequest(ex.InnerException?.Message);
                }
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"abimId: '{abimId}'");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// GetCredentialsByNPI
        /// </summary>
        /// <param name="npi"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route(Routes.PhysicianCredentials.GetPhysicianCredentialsByNPI, Name = RouteNames.PhysicianCredentials.GetPhysicianCredentialsByNPI)]
        public async Task<IHttpActionResult> GetPhysicianCredentialsByNPI(string npi)
        {
            if (string.IsNullOrEmpty(npi))
                return BadRequest("AbimId cannot be null or empty");

            try
            {
                //get user's profile
                var profile = await RetryHelper.RetryTask(() => ProfileInterService.SearchProfilesByNPI(AccessToken, ProfileHostUrl, npi), () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);

                var allCredentials = await CredentialService.SearchByMemberIdAsync(profile.Id)
                                        .ConfigureAwait(false);
                // filter only ABIM certificates (not ABIM issued cert (Issuance.SourceId=1)
                // Bug 164024 : VOC pages should only display ABIM certificates
                var ABIMcredentials = allCredentials.Where(c => c.Certification.Source.Code == "ABIM" && c.IsCosponsored == false).ToList();

                // Bug 222785 : Cosponsored certs appearing on VOC page
                // PBi 210151 : Remove cosponsored "credentials" from the VoC page
                // The condition below only for co-sponsored diplomates. If regular diplomate does not have any creds then it should work as before
                if (!ABIMcredentials.Any() && allCredentials.Any())
                    return Content(HttpStatusCode.NotFound, $"No user exists with npi:'{npi}'.");

                return Ok(PhysicianCertificationsMapping(profile, ABIMcredentials, Url));
            }
            catch (UnsuccessfulStatusException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return Content(HttpStatusCode.NotFound, $"No user exists with npi:'{npi}'.");
                else
                {
                    HandleExceptionLogging(ex, $"npi: '{npi}'");
                    return BadRequest(ex.InnerException?.Message);
                }
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"npi: '{npi}'");
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="firstName"></param>
        /// <param name="dob"></param>
        /// <param name="soundEx"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <returns></returns>
        [HttpGet, AllowAnonymous]
        [Route(Routes.PhysicianCredentials.SearchProfilesByNameAndDob, Name = Routes.PhysicianCredentials.SearchProfilesByNameAndDob)]
        public async Task<IHttpActionResult> SearchProfilesByNameAndDob(string lastName, string firstName = null, DateTime? dob = null, bool soundEx = false, int pageSize = 0, int pageIndex = 0)
        {
            if (string.IsNullOrEmpty(lastName))
                return BadRequest("LastName cannot be null or empty.");

            try
            {
                //search user profiles
                var result = await RetryHelper.RetryTask(() => ProfileInterService.SearchProfilesByNameAndDob(AccessToken, ProfileHostUrl, lastName, firstName, dob, soundEx, pageSize, pageIndex), () => AccessTokenService.GetAccessToken()).ConfigureAwait(false);

                if (result.TotalCount == 1)
                    return await GetPhysicianCredentialsByAbimId(result.Data[0].AbimId).ConfigureAwait(false);
                else
                {
                    //  we are getting Profile resource file but we need to change SELF link
                    var resource = Mapper.Map<ProfileShortCollectionResource, ProfileShortCollectionResourcePublic>(result,
                                   o => { o.Items["UrlHelper"] = Url; });

                    return Ok(resource);
                }

            }
            catch (UnsuccessfulStatusException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var resMessage = $"No users exist with Last Name:'{lastName}'";
                    if (!string.IsNullOrEmpty(firstName))
                        resMessage += $" and FirsName:'{firstName}'";
                    if (dob != null)
                        resMessage += $" and DOB:'{dob?.ToShortDateString()}'";
                    resMessage += ".";
                    return Content(HttpStatusCode.NotFound, resMessage);
                }
                else
                {
                    var dobValue = dob.HasValue ? dob.Value.ToString() : "";
                    HandleExceptionLogging(ex, $"lastName: '{lastName}', firstName: '{firstName}', dob: '{dobValue}', soundEx : '{soundEx}', pageSize: '{pageSize}', pageIndex: '{pageIndex}'");
                    return BadRequest(ex.InnerException?.Message);
                }
            }
            catch (Exception ex)
            {
                var dobValue = dob.HasValue ? dob.Value.ToString() : "";
                HandleExceptionLogging(ex, $"lastName: '{lastName}', firstName: '{firstName}', dob: '{dobValue}', soundEx : '{soundEx}', pageSize: '{pageSize}', pageIndex: '{pageIndex}'");
                return BadRequest(ex.Message);
            }
        }

        #endregion

        /// <summary>
        /// PhysicianCertificationsMapping
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="credentials"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static PhysicianCertificationsPublicResource PhysicianCertificationsMapping(ProfileSummaryShortResource profile,
                                                                            IEnumerable<Credential> credentials,
                                                                            UrlHelper Url)
        {
            
            // for some reason profile return Link type under Abim.Platform.Identity.NugetCore (base is ResourceBase)
            // so we better off return Abim.Platform.Program.WebApi.Link
            List<Link> links = new List<Link>() {
                new Link("self",
                            HttpVerbs.Get,
                            Url.Link(Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId,
                            new Dictionary<string, object> { { "abimId", profile.AbimId } }))
            };

            var image = profile.Links.Where(a => a.Name == ProfileResourceConstants.RouteNames.GetUserImage).FirstOrDefault();

            if (image != null)
            {
                links.Add(new Link("image",
                            HttpVerbs.Get,
                            image.Href));
            }

            return new PhysicianCertificationsPublicResource
            {
                AbimId = profile.AbimId,
                LastName = profile.Name.LastName,
                FirstName = profile.Name.FirstName,
                MiddleName = profile.Name.MiddleName ?? "",
                MaidenName = profile.Name.MaidenName,
                Suffix = profile.Name.Suffix,
                Salutation = profile.Name.Salutation,

                NameAliases = profile.NameAliases.Select(nm => new NameAliasPublicResource
                {
                    LastName = nm.Name.LastName,
                    FirstName = nm.Name.FirstName,
                    MiddleName = nm.Name.MiddleName,
                    MaidenName = nm.Name.MaidenName,
                    Salutation = nm.Name.Salutation,
                    Suffix = nm.Name.Suffix
                }).ToList(),

                IsActive = profile.IsActive,

                ParticipatingInMOC = credentials
                                        .Where(a => a.HasIssuances && a.NewestIssuance.MaintenanceStatus == MaintenanceStatusType.Maintained)
                                        .Count() > 1,
                // if FPHM exist and it is selected to be maintained
                IsFocusPractice = credentials
                                    .Where(a => a.HasIssuances && 
                                                a.Certification.Code == CertificationCode.FocusedPracticeHospitalMedicine && 
                                                a.NewestIssuance.IssuanceStatus == IssuanceStatusType.Active &&
                                                a.SelectedToMaintain)
                                    .Any(),

                // the same logic in HelperService.GetVocLetterContent (unit tests in GetVocLetterContentCommandSpec )
                Certifications = credentials
                                        .Where(a => a.HasIssuances)
                                        .Select(
                                            cred => new CertificationPublicResource
                                            {
                                                Name = cred.Certification.Code != CertificationCode.FocusedPracticeHospitalMedicine ? cred.Certification.Name : CertificationName.IMwithFPHM,
                                                InitialIssuanceDate = cred.OldestIssuance.IssuanceDate,
                                                Status = cred.ProperIssuance.IssuanceStatus,
                                                MaintenanceStatus = cred.ProperIssuance.MaintenanceStatus,
                                            }).OrderBy(a => a.InitialIssuanceDate).ToList(),
                Links = links
            };
        }

    }
}
