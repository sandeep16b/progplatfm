using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Platform.Program.App.Data.ComplexQueries;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.Extensions;
using Abim.Platform.Program.Host.OptionsResources;
using Abim.Platform.Program.Host.Util;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Api.Attributes;
using Abim.Platform.Program.WebApi.Attributes;
using Abim.Platform.Program.WebApi.Filters;
using Abim.Platform.Program.WebApi.Objects;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    /// <summary>
    /// CredentialController
    /// </summary>
    [RoutePrefix(ProgramResourceConstants.Routes.Prefix.Credential)]
    public class CredentialController : ControllerBase
    {
        #region Static Members
        private static string[] _backDoorValues = ConfigurationManager.AppSettings["AdminGroup"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        #endregion

        #region Static Members
        /// <summary>
        /// 
        /// </summary>
        protected static string ProfileHostUrl = ConfigurationManager.AppSettings["ProfileHostUrl"];
        #endregion

        #region Properties

        /// <summary>
        /// The credential service
        /// </summary>
        private ICredentialService CredentialService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        protected IProfileInterservice ProfileInterService { get; set; }

        /// <summary>
        /// The enum service
        /// </summary>
        private IEnumService EnumService { get; set; }

        private IHelperService HelperService { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialController"/> class.
        /// </summary>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="enumService">The enum service.</param>
        ///  /// <param name="helperService">The helper service.</param>
        /// <param name="profileInterService">The enum service.</param>
        public CredentialController(ICredentialService credentialService,
                                    IEnumService enumService,
                                    IHelperService helperService,
                                    IProfileInterservice profileInterService)
        {
            CredentialService = credentialService;
            EnumService = enumService;
            HelperService = helperService;
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

        #region Options Endpoints

        /// <summary>
        /// Get Credential Options
        /// </summary>
        /// <returns>A Credential options response object</returns>
        [HttpOptions]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.CredentialOptions, Name = ProgramResourceConstants.RouteNames.Credentials.CredentialOptions)]
        public IHttpActionResult CredentialOptions()
        {
            try
            {
                return Ok(new CredentialOptionsResponseResource(Url));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Get Issuance Options
        /// </summary>
        /// <returns>An Issuance options response object</returns>
        [HttpOptions]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.CredentialIssuanceOptions, Name = ProgramResourceConstants.RouteNames.Credentials.IssuanceOptions)]
        public IHttpActionResult IssuanceOptions()
        {
            try
            {
                return Ok(new IssuanceOptionsResponseResource(Url));
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Gets the values of a specific Credential enum
        /// </summary>
        /// <param name="name">The name of the enum as provided by the route.</param>
        /// <returns>An array of enum values available for the given enum.</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.CredentialEnumValues, Name = ProgramResourceConstants.RouteNames.Credentials.CredentialEnumValues)]
        public IHttpActionResult GetCredentialEnum(string name)
        {
            try
            {
                return GetEnumValuesForModel<Credential>(name, EnumService, EnumLinkSettings.IncludeNestedEnumTypesInLinks, ProgramResourceConstants.RouteNames.Credentials.CredentialEnumValues);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Gets the values of a specific Issuance enum
        /// </summary>
        /// <param name="name">The name of the enum as provided by the route.</param>
        /// <returns>An array of enum values available for the given enum.</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.IssuanceEnumValues, Name = ProgramResourceConstants.RouteNames.Credentials.IssuanceEnumValues)]
        public IHttpActionResult GetIssuanceEnum(string name)
        {
            try
            {
                return GetEnumValuesForModel<Issuance>(name, EnumService, EnumLinkSettings.IncludeNestedEnumTypesInLinks,
               ProgramResourceConstants.RouteNames.Credentials.IssuanceEnumValues);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion

        #region Retrieve Endpoints

        /// <summary>
        /// Get all Credentials
        /// </summary>
        /// <remarks>
        /// Allows client to send complex query from query string through the use of a custom model binder.
        /// </remarks>
        /// <returns>A CredentialCollectionResource containing a collection of Credential objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize(Actions.ViewAllAdmin)]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCredentials, Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentials)]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCredentialsRouteExt, Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialsExt)]
        public IHttpActionResult GetCredentials([FromUri] CredentialComplexQuery credentialComplexQuery)
        {
            //first ensure we have a valid CredentialComplexQuery
            if (credentialComplexQuery == null) credentialComplexQuery = new CredentialComplexQuery();       //use the defaults
            else
            {
                string queryValidationErrorMsg = ValidateQuery(credentialComplexQuery);
                if (queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
            }

            try
            {
                var totalCount = 0;
                var credentials = CredentialService.Search(credentialComplexQuery, out totalCount);
                if (credentialComplexQuery.PageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Credential>, CredentialCollectionResource>(credentials, o => o.Items["UrlHelper"] = Url);
                resource.SetPaging(credentialComplexQuery.PageDefinition, totalCount, Url, true);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                return HandleQueryException(ex, credentialComplexQuery);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Get all Credentials
        /// </summary>
        /// <returns>A CredentialCollectionResource containing a collection of Credential objects</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.ViewAllAdmin)]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCredentials, Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialsPost)]
        public IHttpActionResult GetCredentialsPost([FromBody] CredentialComplexQuery credentialComplexQuery)
        {
            //first ensure we have a valid CredentialComplexQuery
            if (credentialComplexQuery == null) credentialComplexQuery = new CredentialComplexQuery();       //use the defaults
            else
            {
                string queryValidationErrorMsg = ValidateQuery(credentialComplexQuery);
                if (queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
            }

            try
            {
                var totalCount = 0;
                var credentials = CredentialService.Search(credentialComplexQuery, out totalCount);
                if (credentialComplexQuery.PageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Credential>, CredentialCollectionResource>(credentials,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Credentials.GetCredentialsPost; });
                resource.SetPaging(credentialComplexQuery.PageDefinition, totalCount, Url, true);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                return HandleQueryException(ex, credentialComplexQuery);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Get Current UserProfile Credentials
        /// </summary>
        /// <remarks>
        /// Allows client to send complex query from query string through the use of a custom model binder.
        /// </remarks>
        /// <returns>A CredentialCollectionResource containing a collection of Credential objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [ImpersonateMemberId]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCurrentUserCredentials, Name = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials)]
        public IHttpActionResult GetCurrentUserCredentials([FromUri] PageDefinition pageDefinition, Guid? memberId = null)
        {
            if (UserProfile == null || UserProfile.ProfileId.Equals(Guid.Empty))
            {
                //this constitutes a 500 because if the ResourceAuthorization claims they were authorized to hit this, they should have had a ProfileId found
                Logger.Error("ProfileId not found, in GetCurrentUserCredentials");
                return Content(HttpStatusCode.InternalServerError, ErrorMessages.ProfileIdNotFound);
            }

            Guid targetProfileId = ProfileId;
            if (memberId.HasValue)
            {
                if (!_backDoorValues.Any())
                    return Content(HttpStatusCode.InternalServerError, "The ADFS claim type is unknown");
                if (!UserProfile.Claims.Any(c => c.Type.Contains("identity/claims/role") && _backDoorValues.Contains(c.Value)))
                    return Content(HttpStatusCode.Forbidden, ErrorMessages.ForbiddenResource());
                targetProfileId = memberId.Value;
            }

            string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
            if (queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);

            try
            {
                int totalCount;
                var credentials = CredentialService.SearchByMemberId(targetProfileId, pageDefinition, out totalCount);
                if (pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Credential>, CredentialCollectionResource>(credentials,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                var memberIdValue = memberId.HasValue ? memberId.Value.ToString() : null;
                HandleExceptionLogging(ex, $"memberId: '{memberIdValue}'", pageDefinition);

                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Get Current UserProfile Credentials (Post)
        /// </summary>
        /// <remarks>
        /// Allows client to send complex query from query string through the use of a custom model binder.
        /// </remarks>
        /// <returns>A CredentialCollectionResource containing a collection of Credential objects</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize]
        [ImpersonateMemberId]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCurrentUserCredentialsPost, Name = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentialsPost)]
        public IHttpActionResult GetCurrentUserCredentialsPost([FromBody] PageDefinition pageDefinition, Guid? memberId = null)
        {
            if (UserProfile == null || UserProfile.ProfileId.Equals(Guid.Empty))
            {
                //this constitutes a 500 because if the ResourceAuthorization claims they were authorized to hit this, they should have had a ProfileId found
                Logger.Error("ProfileId not found, in GetCurrentUserCredentials");
                return Content(HttpStatusCode.InternalServerError, ErrorMessages.ProfileIdNotFound);
            }

            Guid targetProfileId = ProfileId;
            if (memberId.HasValue)
            {
                if (!_backDoorValues.Any())
                    return Content(HttpStatusCode.InternalServerError, "The ADFS claim type is unknown");
                if (!UserProfile.Claims.Any(c => c.Type.Contains("identity/claims/role") && _backDoorValues.Contains(c.Value)))
                    return Content(HttpStatusCode.Forbidden, ErrorMessages.ForbiddenResource());
                targetProfileId = memberId.Value;
            }

            string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
            if (queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);

            try
            {
                int totalCount;
                var credentials = CredentialService.SearchByMemberId(targetProfileId, pageDefinition, out totalCount);
                if (pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Credential>, CredentialCollectionResource>(credentials,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentialsPost; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                var memberIdValue = memberId.HasValue ? memberId.Value.ToString() : "";
                HandleExceptionLogging(ex, $"memberId: '{memberIdValue}'", pageDefinition);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Get Member Credentials by member Id (Admin endpoint)
        /// </summary>
        /// <remarks>
        /// Allows client to send complex query from query string through the use of a custom model binder.
        /// </remarks>
        /// <returns>A CredentialCollectionResource containing a collection of Credential objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize(Actions.ViewAllAdmin)]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCredentialsByMemberId, Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialByMemberId)]
        public IHttpActionResult GetUserCredentialsByMemberId(Guid memberId, [FromUri] PageDefinition pageDefinition)
        {
            string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
            if (queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);

            try
            {
                if (memberId == null || memberId.Equals(Guid.Empty))
                {
                    //this constitutes a 500 because if the ResourceAuthorization claims they were authorized to hit this, they should have had a ProfileId found
                    Logger.Error("Member Id not found, in GetUserCredentialsByMemberId", Request);
                    return Content(HttpStatusCode.InternalServerError, ErrorMessages.ProfileIdNotFound);
                }
                int totalCount;
                var credentials = CredentialService.SearchByMemberId(memberId, pageDefinition, out totalCount);
                if (pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Credential>, CredentialCollectionResource>(credentials,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"memberId: '{memberId}'", pageDefinition);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Get a Credential by Id
        /// </summary>
        /// <param name="id">The id of the Credential (a Guid)</param>
        /// <returns>A Credential object</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCredentialById, Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialById)]
        public IHttpActionResult GetCredentialById(Guid id)
        {
            if (id.Equals(Guid.Empty))
                return BadRequest("Id is empty");

            try
            {
                var credential = CredentialService.Load(id);
                if (credential == null)
                {
                    var msg = ErrorMessages.NotFound("Credential", id);
                    return Content(HttpStatusCode.NotFound, msg);
                }
                if (!credential.MemberId.Equals(ProfileId) && !UserProfile.IsAdmin)
                    return Content(HttpStatusCode.Forbidden, ErrorMessages.ForbiddenResource("Credential"));
                var resource = Mapper.Map<Credential, CredentialResource>(credential, o => o.Items["UrlHelper"] = Url);
                return Ok(resource);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"credentialId: '{id}'");
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Provides a user-friendly error message
        /// </summary>
        /// <param name="invalidId"></param>
        /// <returns></returns>
        [HttpGet]
        [HideFromSwagger]
        [Authorize]
        [Route(ProgramResourceConstants.Routes.Credentials.GetCredentialInvalidId, Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialInvalidId)]
        public IHttpActionResult GetWithInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }


        /// <summary>
        /// Gets the first abim issuance date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.GetFirstABIMIssuanceDate, Name = ProgramResourceConstants.RouteNames.Credentials.GetFirstABIMIssuanceDate)]
        public IHttpActionResult GetFirstABIMIssuanceDate(Guid memberId)
        {
            if (memberId.Equals(Guid.Empty))
                return BadRequest("memberId is empty");
            try
            {
                var result = CredentialService.GetFirstIssuanceDate(memberId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"memberId: '{memberId}'");
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Gets the latest lookback date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.GetLatestLookbackDate, Name = ProgramResourceConstants.RouteNames.Credentials.GetLatestLookbackDate)]
        public IHttpActionResult GetLatestLookbackDate(Guid memberId)
        {
            if (memberId.Equals(Guid.Empty))
                return BadRequest("memberId is empty");
            try
            {
                var result = CredentialService.GetLatestLookbackDate(memberId);

                return Ok(result);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"memberId: '{memberId}'");
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="abimId"></param>
        /// <returns>VOC Letter using the template</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.GetVocLetter, Name = ProgramResourceConstants.RouteNames.Credentials.GetVocLetter)]
        public async Task<IHttpActionResult> GetVocLetter(string abimId)
        {
            //The purpose of this code is apparently to prevent a non-admin from attempting to generate a VOC letter for anyone but 
            //themselves
            if (UserProfile.AbimId != null && (!UserProfile.AbimId.Equals(abimId) && !UserProfile.IsAdmin))
            {
                return Content(HttpStatusCode.Forbidden, ErrorMessages.ForbiddenResource("Profile"));
            }

            if (string.IsNullOrEmpty(abimId))
                return BadRequest("AbimId cannot be null or empty");
            try
            {
                //ar@7/14/22 : no change here since we are reusing existing incoming access token
                var profile = await ProfileInterService.GetProfileByABIMId(UserProfile.TokenWithoutBearer, ProfileHostUrl, abimId).ConfigureAwait(false);

                var allCredentials = await CredentialService.SearchByMemberIdAsync(profile.Id).ConfigureAwait(false);

                // filter only ABIM certificates (not ABIM issued cert (Issuance.SourceId=1)
                // Bug 165617 : (HF) Non-ABIM certs were being listed on the VOC page and the portal
                // PBI 219808 : (Jan) Suppress Cosponsored certs from My Profile page for hybrid physicians
                var ABIMcredentials = allCredentials.Where(c => c.Certification.Source.Code == "ABIM" && !c.IsCosponsored);

                var vocLetterContent = HelperService.GetVocLetterContent(profile, ABIMcredentials);

                if (vocLetterContent != null)
                {
                    using (var letterStream = HelperService.CreateVocLetter(vocLetterContent))
                    {
                        if (letterStream.Length < 0)
                        {
                            return BadRequest("VOC Letter could not be generated");
                        }
                        else
                        {
                            var result = new HttpResponseMessage(HttpStatusCode.OK)
                            {
                                Content = new ByteArrayContent(letterStream.ToArray())

                            };
                            result.Content.Headers.ContentDisposition =
                            new ContentDispositionHeaderValue("attachment")
                            {
                                FileName = "VOC.pdf"
                            };
                            result.Content.Headers.ContentType =
                                new MediaTypeHeaderValue("application/pdf");
                            return ResponseMessage(result);
                        }

                    }

                }
                else
                {
                    return BadRequest("VOC Letter content could not be generated");

                }

            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"abimId: '{abimId}'");
                return null;
            }
            finally
            {
                DisposeServices(Request);
            }
        }
        #endregion

        #region Edit Methods

        #region UpdateSelectedToMaintain
        /// <summary>
        /// updates the SelectedToMaintain flag
        /// </summary>
        /// <param name="credentialId">The credential Id.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize] //ar@10/19/2017 per task 101278
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.UpdateSelectedToMaintain, Name = ProgramResourceConstants.RouteNames.Credentials.UpdateSelectedToMaintain)]
        public async Task<IHttpActionResult> UpdateSelectedToMaintain(Guid credentialId, [FromBody] UpdateSelectedToMaintainCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);
                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.CredentialId = credentialId;
                command.UserInfo = UserProfile;

                var commandResult = await CredentialService.Handle(command);
                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"credentialId: '{credentialId}'", command);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Provides a user-friendly error message
        /// </summary>
        /// <param name="invalidId"></param>
        /// <returns></returns>
        [HttpPost]
        [HideFromSwagger]
        [Authorize]
        [Route(ProgramResourceConstants.Routes.Credentials.UpdateSelectedToMaintainInvalidId, Name = ProgramResourceConstants.RouteNames.Credentials.UpdateSelectedToMaintainInvalidId)]
        public IHttpActionResult UpdateSelectedToMaintainInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }

        #endregion

        #region UpdatePathway

        /// <summary>
        /// Updates the pathway value
        /// </summary>
        /// <param name="credentialId">The credential Id.</param>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        [HttpPost]
        [Authorize] //ah@11/5/2018 per PBI 136142
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.UpdatePathway, Name = ProgramResourceConstants.RouteNames.Credentials.UpdatePathway)]
        public async Task<IHttpActionResult> UpdatePathway(Guid credentialId, [FromBody] UpdatePathwayCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);
                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.CredentialId = credentialId;
                command.UserInfo = UserProfile;

                var commandResult = await CredentialService.Handle(command);
                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"credentialId: '{credentialId}'", command);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Provides a user-friendly error message
        /// </summary>
        /// <param name="invalidId"></param>
        /// <returns></returns>
        [HttpPost]
        [HideFromSwagger]
        [Authorize]
        [Route(ProgramResourceConstants.Routes.Credentials.UpdatePathwayInvalidId, Name = ProgramResourceConstants.RouteNames.Credentials.UpdatePathwayInvalidId)]
        public IHttpActionResult UpdatePathwayInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }

        #endregion UpdatePathway

        #region WithdrawCredential

        /// <summary>
        /// Withdraw credential by setting most recent issuance status to Suspend/Revoke/Surrender and set WithDrawnDate
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.WithdrawCredential, Name = ProgramResourceConstants.RouteNames.Credentials.WithdrawCredential)]
        public async Task<IHttpActionResult> WithdrawCredential(Guid credentialId, [FromBody] WithdrawCredentialCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);
                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.CredentialId = credentialId;
                command.UserInfo = UserProfile;

                var commandResult = await CredentialService.Handle(command);
                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"credentialId: '{credentialId}'", command);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Provides a user-friendly error message
        /// </summary>
        /// <param name="invalidId"></param>
        /// <returns></returns>
        [HttpPost]
        [HideFromSwagger]
        [Authorize]
        [Route(ProgramResourceConstants.Routes.Credentials.WithdrawCredentialInvalidId, Name = ProgramResourceConstants.RouteNames.Credentials.WithdrawCredentialInvalidId)]
        public IHttpActionResult WithdrawCredentialInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }

        #endregion

        #region ReinstateCredential

        /// <summary>
        /// Reinstate Credential by Setting Issuance Status to 'Expired' and run Corrective Action 
        /// </summary>
        /// <param name="credentialId"></param>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.ReinstateCredential, Name = ProgramResourceConstants.RouteNames.Credentials.ReinstateCredential)]
        public async Task<IHttpActionResult> ReinstateCredential(Guid credentialId, [FromBody] ReinstateCredentialCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.CredentialId = credentialId;
                command.UserInfo = UserProfile;

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, $"credentialId: '{credentialId}'", command);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Provides a user-friendly error message
        /// </summary>
        /// <param name="invalidId"></param>
        /// <returns></returns>
        [HttpPost]
        [HideFromSwagger]
        [Authorize]
        [Route(ProgramResourceConstants.Routes.Credentials.ReinstateCredentialInvalidId, Name = ProgramResourceConstants.RouteNames.Credentials.ReinstateCredentialInvalidId)]
        public IHttpActionResult ReinstateCredentialInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }

        #endregion

        #region Selection and Deselection
        /// <summary>
        /// Marks the specified credentials for selection or deselection
        /// </summary>
        /// <param name="command">A command containing the IDs of the Credentials to mark for selection or deselection</param>
        /// <returns>An HTTP response indicating success or failure</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.MarkForSelectionOrDeselection, Name = ProgramResourceConstants.RouteNames.Credentials.MarkForSelectionOrDeselection)]
        public IHttpActionResult MarkForSelectionOrDeselection([FromBody] MarkCertificatesForSelectOrDeselectCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = UserProfile;

                var commandResult = 
                    (MarkCertificatesForSelectOrDeselectCommandResult)CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null , command);

                HttpStatusCode httpStatusCode = 
                    (ex.Message.Contains("already") ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError);

                return Content(httpStatusCode, ex.Message);
            }
        }
        #endregion Selection and Deselection

        #endregion Edit Methods

        #region ETL processing endpoints

        #region Add Credential Method
        /// <summary>
        /// Add a Credential 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.Create)]
        [Route(ProgramResourceConstants.Routes.Credentials.AddCredential, Name = ProgramResourceConstants.RouteNames.Credentials.AddCredential)]
        public async Task<IHttpActionResult> AddCredential([FromBody] AddCredentialCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);
                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = Helpers.SetUserNameInProfile(UserProfile, "backgroundClient");

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException || ex is ArgumentException)
            {
                HandleExceptionLogging(ex, null, command);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null, command);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Update Credential Method
        /// <summary>
        /// Update a Credential 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.UpdateCredential, Name = ProgramResourceConstants.RouteNames.Credentials.UpdateCredential)]
        public async Task<IHttpActionResult> UpdateCredential([FromBody] UpdateCredentialCommand command)
        {
            try
            {
                if (command == null) return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError()) return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = Helpers.SetUserNameInProfile(UserProfile, "backgroundClient");

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));

            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException || ex is ArgumentException)
            {
                HandleExceptionLogging(ex, null, command);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null, command);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Add Issuance Method
        /// <summary>
        /// Add a Issuance 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.Create)]
        [Route(ProgramResourceConstants.Routes.Credentials.AddIssuance, Name = ProgramResourceConstants.RouteNames.Credentials.AddIssuance)]
        public async Task<IHttpActionResult> AddIssuance([FromBody] AddIssuanceCommand command)
        {
            try
            {
                if (command == null)
                    return BadRequest(ErrorMessages.CommandNotBound);
                if (ModelStateExplicitValidationError())
                    return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = Helpers.SetUserNameInProfile(UserProfile, "backgroundClient");

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Issuance, IssuanceResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException || ex is ArgumentException)
            {
                HandleExceptionLogging(ex, null, command);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null, command);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Update Issuance Method
        /// <summary>
        /// Update a Issuance 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.UpdateIssuance, Name = ProgramResourceConstants.RouteNames.Credentials.UpdateIssuance)]
        public async Task<IHttpActionResult> UpdateIssuance([FromBody] UpdateIssuanceCommand command)
        {
            try
            {
                if (command == null) return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError()) return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = Helpers.SetUserNameInProfile(UserProfile, "backgroundClient");

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Issuance, IssuanceResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));

            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException || ex is ArgumentException)
            {
                HandleExceptionLogging(ex, null, command);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null, command);
                return InternalServerError(ex);
            }
        }
        #endregion

        #region Get Total Count of NonAbim issuances

        /// <summary>
        /// Get a Total Count of NonAbim issuances
        /// </summary>
        /// <returns>count</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Credentials.GetNonAbimIssuancesCount, Name = ProgramResourceConstants.RouteNames.Credentials.GetNonAbimIssuancesCount)]
        public IHttpActionResult GetNonAbimIssuanceCount()
        {
            try
            {
                var count = CredentialService.GetNonAbimIssuanceCount();

                return Ok(count);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        #endregion

        #endregion

        #region ACC API (gateway consumer)

        /// <summary>
        /// EnrollInCMP
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [RequiresHttps, Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.EnrollInCMP, Name = ProgramResourceConstants.RouteNames.Credentials.EnrollInCMP)]
        public async Task<IHttpActionResult> EnrollInCMP([FromBody] EnrollInCMPCommand command)
        {
            try
            {
                if (command == null) return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError()) return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = UserProfile;

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                var link = Url.Link(ProgramResourceConstants.RouteNames.Credentials.GetCredentialById,
                             new Dictionary<string, object> { { "id", commandResult.Data.ExternalId } });

                return Created(link,  Mapper.Map<Credential, CredentialResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));

            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException || ex is ArgumentException)
            {
                HandleExceptionLogging(ex, null, command);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null, command);
                return InternalServerError(ex);
            }
        }

        /// <summary>
        /// UnEnrollInCMP
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [RequiresHttps, Authorize]
        [ResourceAuthorize(Actions.Update)]
        [Route(ProgramResourceConstants.Routes.Credentials.UnEnrollInCMP, Name = ProgramResourceConstants.RouteNames.Credentials.UnEnrollInCMP)]
        public async Task<IHttpActionResult> UnEnrollInCMP([FromBody] UnEnrollInCMPCommand command)
        {
            try
            {
                if (command == null) return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError()) return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = UserProfile;

                var commandResult = await CredentialService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                var resource = Mapper.Map<Credential, CredentialResource>(commandResult.Data,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials; });
                return Ok(resource);
            }
            catch (Exception ex) when (ex is InvalidOperationException || ex is ArgumentNullException || ex is ArgumentException)
            {
                HandleExceptionLogging(ex, null, command);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                HandleExceptionLogging(ex, null, command);
                return InternalServerError(ex);
            }
        }
        #endregion
    }
}
