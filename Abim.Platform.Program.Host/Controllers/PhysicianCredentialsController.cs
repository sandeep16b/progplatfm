using Abim.Platform.Program.MembershipClient;
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
using static Abim.Platform.Program.Resources.ProgramResourceConstants;
using Abim.Platform.Product.Utils.Exceptions; 
using Swashbuckle.Swagger.Annotations;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    /// <summary>
    /// PhysicianCredentialsController
    /// </summary> 
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
        /// Membership Client Service   
        /// </summary>
        protected IMembershipClientService MembershipClientService { get; set; }
         
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
        /// <param name="membershipClientService"></param>
        public PhysicianCredentialsController(      ICredentialService credentialService,
                                                    IAccessTokenService accessTokenSingletonWraper,
                                                    IMembershipClientService membershipClientService)
        {
            CredentialService = credentialService;
            AccessTokenService = accessTokenSingletonWraper;
            MembershipClientService = membershipClientService;
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
        [SwaggerResponse(HttpStatusCode.OK, "GetPhysicianCredentialsByAbimId", typeof(PhysicianCertificationsPublicResource))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId, Name = Routes.PhysicianCredentials.GetPhysicianCredentialsByAbimId)]
        public async Task<IHttpActionResult> GetPhysicianCredentialsByAbimId(string abimId)
        {
            if (string.IsNullOrEmpty(abimId))
                return BadRequest("AbimId cannot be null or empty");

            try
            {
                //get user's profile
                var vocprofiles = await MembershipClientService.GetVocByAbimIdAsync(abimId).ConfigureAwait(false);

                if (vocprofiles == null || vocprofiles.Count == 0 )
                    return Content(HttpStatusCode.NotFound, $"No user exists with AbimId:'{abimId}'.");

                var vocprofilesFirst = vocprofiles.FirstOrDefault();

                var allCredentials = await CredentialService.SearchByMemberIdAsync(vocprofilesFirst.PublicId)
                                         .ConfigureAwait(false); 
           
                var ABIMcredentials = allCredentials.Where(c => c.Certification.Source.Code == "ABIM" && c.IsCosponsored == false).ToList();
 
                if (!ABIMcredentials.Any() && allCredentials.Any())
                    return Content(HttpStatusCode.NotFound, $"No user exists with AbimId:'{abimId}'.");
            
                return Ok(PhysicianCertificationsMappingForVocProfileResource(vocprofilesFirst, ABIMcredentials, Url));

            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404)
                    return Content(HttpStatusCode.NotFound, $"No user exists with AbimId:'{abimId}'.");
                else
                {
                    HandleExceptionLogging(ex, $"abimId: '{abimId}'");
                    return BadRequest(ex.InnerException?.Message);
                }
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
            finally
            {
                DisposeServices(Request);
            }
        }


        // *** below endpoint is not used by VOC page, in case it is used by Someone we leave it here ****
        /// <summary> 
        /// GetCredentialsByGuid
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>  
        [HttpGet, AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "GetPhysicianCredentialsById", typeof(PhysicianCertificationsPublicResource))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(Routes.PhysicianCredentials.GetPhysicianCredentialsById, Name = Routes.PhysicianCredentials.GetPhysicianCredentialsById)]
        public async Task<IHttpActionResult> GetPhysicianCredentialsById(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest("Id cannot be null or empty");

            try
            {
                //get user's profile
                var profile = await  MembershipClientService.GetProfileByMemberIdAsync(id).ConfigureAwait(false);

                if (profile == null) throw new Exception($"profile is not found for Guid id {id}");

                var allCredentials = await CredentialService.SearchByMemberIdAsync(profile.Id)
                                        .ConfigureAwait(false);
                var ABIMcredentials = allCredentials.Where(c => c.Certification.Source.Code == "ABIM" && c.IsCosponsored == false).ToList();

                if (!ABIMcredentials.Any() && allCredentials.Any())
                    return Content(HttpStatusCode.NotFound, $"No user exists with Id:'{id}'.");

                return Ok(PhysicianCertificationsMappingForProfileResource(profile, ABIMcredentials, Url));

            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404)
                    return Content(HttpStatusCode.NotFound, $"No user exists with Guid:'{id}'.");
                else
                {
                    HandleExceptionLogging(ex, $"Guid: '{id}'");
                    return BadRequest(ex.InnerException?.Message);
                }
            }
            catch (UnsuccessfulStatusException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                    return Content(HttpStatusCode.NotFound, $"No user exists with Guid:'{id}'.");
                else
                {
                    HandleExceptionLogging(ex, $"Guid: '{id}'");
                    return BadRequest(ex.InnerException?.Message);
                }
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"Guid: '{id}'");
                return BadRequest(ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }


        /// <summary>
        /// GetPhysicianCredentialsByNPI
        /// </summary>
        /// <param name="npi"></param>
        /// <returns></returns> 
        [HttpGet, AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "GetPhysicianCredentialsByNPI", typeof(PhysicianCertificationsPublicResource))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(Routes.PhysicianCredentials.GetPhysicianCredentialsByNPI, Name = RouteNames.PhysicianCredentials.GetPhysicianCredentialsByNPI)]
        public async Task<IHttpActionResult> GetPhysicianCredentialsByNPI(string npi)
        {
            if (string.IsNullOrEmpty(npi))
                return BadRequest("npi cannot be null or empty"); 

            try
            {
                //get user's profile
                var vocprofile = await MembershipClientService.SearchProfilesByNPI(npi).ConfigureAwait(false);

                var profilefirst = vocprofile.FirstOrDefault();

                if (profilefirst == null)
                    return Content(HttpStatusCode.NotFound, $"No user exists with npi:'{npi}'.");


                var allCredentials = await CredentialService.SearchByMemberIdAsync(profilefirst.PublicId)
                                        .ConfigureAwait(false);
                // filter only ABIM certificates (not ABIM issued cert (Issuance.SourceId=1)
                // Bug 164024 : VOC pages should only display ABIM certificates
                var ABIMcredentials = allCredentials.Where(c => c.Certification.Source.Code == "ABIM" && c.IsCosponsored == false).ToList();

                // Bug 222785 : Cosponsored certs appearing on VOC page
                // PBi 210151 : Remove cosponsored "credentials" from the VoC page
                // The condition below only for co-sponsored diplomates. If regular diplomate does not have any creds then it should work as before
                if (!ABIMcredentials.Any() && allCredentials.Any())
                    return Content(HttpStatusCode.NotFound, $"No user exists with npi:'{npi}'.");

                return Ok(PhysicianCertificationsMappingForVocProfileResource(profilefirst, ABIMcredentials, Url));
            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404)
                    return Content(HttpStatusCode.NotFound, $"No user exists with npi:'{npi}'.");
                else
                {
                    HandleExceptionLogging(ex, $"npi: '{npi}'");
                    return BadRequest(ex.InnerException?.Message);
                }
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
            finally
            {
                DisposeServices(Request);
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
        [SwaggerResponse(HttpStatusCode.OK, "SearchProfilesByNameAndDob", typeof(object))]
        [SwaggerResponse(HttpStatusCode.BadRequest)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(Routes.PhysicianCredentials.SearchProfilesByNameAndDob, Name = Routes.PhysicianCredentials.SearchProfilesByNameAndDob)]
        public async Task<IHttpActionResult> SearchProfilesByNameAndDob(string lastName, string firstName = null, DateTime? dob = null, bool soundEx = false, int pageSize = 50, int pageIndex = 1)
        {
            if (string.IsNullOrEmpty(lastName))
                return BadRequest("LastName cannot be null or empty.");

            try
            {
                //search user profiles
                var result = await MembershipClientService.GetNameAllAsync(lastName, firstName, dob, soundEx, pageSize, pageIndex).ConfigureAwait(false);

                if (result?.Count == 1)
                {
                    return await GetPhysicianCredentialsByAbimId(result.FirstOrDefault().AbimId).ConfigureAwait(false);
                }
                else
                {
                    //  we are getting Profile resource file but we need to change SELF link
                    var resource = Mapper.Map<IEnumerable<VocProfileResource>, ProfileShortCollectionResourcePublic>(result,
                                   o => { o.Items["UrlHelper"] = Url; });
                    resource.PageSize = pageSize;
                    resource.CurrentPage = pageIndex;
                    resource.TotalCount = result.Count;
                    resource.TotalPages = resource.PageSize != 0 ? (int)(Math.Ceiling((float)resource.TotalCount / resource.PageSize)) : 1;
                    return Ok(resource);
                }

            }
            catch (ApiException ex)
            {
                if (ex.StatusCode == 404)
                {
                    var resMessage = $"No users exist with Last Name:'{lastName}'";
                    if (!string.IsNullOrEmpty(firstName))
                        resMessage += $" and FirsName:'{firstName}'";
                    if (dob != null)
                        resMessage += $" and DOB:'{dob.Value.ToShortDateString()}'";
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
            catch (UnsuccessfulStatusException ex)
            {
                if (ex.StatusCode == HttpStatusCode.NotFound)
                {
                    var resMessage = $"No users exist with Last Name:'{lastName}'";
                    if (!string.IsNullOrEmpty(firstName))
                        resMessage += $" and FirsName:'{firstName}'";
                    if (dob != null)
                        resMessage += $" and DOB:'{dob.Value.ToShortDateString()}'";
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
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion

        /// <summary>
        /// PhysicianCertificationsMappingForProfileResource
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="credentials"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static PhysicianCertificationsPublicResource PhysicianCertificationsMappingForProfileResource(ProfileResource profile,
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

            //According to Kevin, Image is no longer needed.  
            //var image = profile.Links.Where(a => a.Name == ProfileResourceConstants.RouteNames.GetUserImage).FirstOrDefault(); 
            // if (image != null)
            // {
            //     links.Add(new Link("image",
            //                 HttpVerbs.Get,
            //                 image.Href));
            // }

            return new PhysicianCertificationsPublicResource
            {
                AbimId = profile.AbimId,
                LastName = profile.Name.LastName,
                FirstName = profile.Name.FirstName,
                MiddleName = profile.Name.MiddleName ?? "",
                MaidenName = profile.Name.MaidenName ?? "",
                Suffix = profile.Name.Suffix.ToString(),
                Salutation = profile.Name.Salutation.ToString(),

                NameAliases = profile.Aliases.Select(nm => new NameAliasPublicResource
                {
                    LastName = nm.LastName,
                    FirstName = nm.FirstName,
                    MiddleName = nm.MiddleName ?? "",
                    MaidenName = nm.MaidenName ?? "",
                    Salutation = nm.Salutation.ToString(),
                    Suffix = nm.Suffix.ToString()
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


        /// <summary>
        /// PhysicianCertificationsMappingForVocProfileResource
        /// </summary>
        /// <param name="profile"></param>
        /// <param name="credentials"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static PhysicianCertificationsPublicResource PhysicianCertificationsMappingForVocProfileResource(VocProfileResource profile,
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

            return new PhysicianCertificationsPublicResource
            {
                AbimId = profile.AbimId,
                LastName = profile.LastName,
                FirstName = profile.FirstName,
                MiddleName = profile.MiddleName ?? "",
                MaidenName = "", //  No Maiden Name in the VocProfile View
                Suffix = profile.Suffix.ToString(),
                Salutation = "",  //  No Maiden Name in the VocProfile View

                NameAliases = new List<NameAliasPublicResource>() { new NameAliasPublicResource()
                {
                    LastName = profile.LastName,
                    FirstName = profile.FirstName,
                    MiddleName = profile.MiddleName ?? "",
                    MaidenName = "", //   No Maiden Name in the VocProfile View
                    Salutation = "", //   No Maiden Name in the VocProfile View
                    Suffix = profile.Suffix.ToString()
                } },

                ImageHref = profile.ImageHref, // Task 295167 : Code Fix to Program Platform to update VocProfileResource class

                IsActive = profile.IsActive,  //   profile.profile is missing from New Profile API,

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

