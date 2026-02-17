using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
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
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    /// <summary>
    /// CertificationController
    /// </summary>
    [RoutePrefix(ProgramResourceConstants.Routes.Prefix.Certification)]
    public class CertificationController : ControllerBase
    {
        #region Static Members
        private static string[] _backDoorValues = ConfigurationManager.AppSettings["AdminGroup"].Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        #endregion

        #region Properties

        /// <summary>
        /// The certification service
        /// </summary>
        protected ICertificationService CertificationService { get; set; }
            
        /// <summary>
        /// The credential service
        /// </summary>
        protected ICredentialService CredentialService { get; set; }
            
        /// <summary>
        /// The enum service
        /// </summary>
        protected IEnumService EnumService { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CertificationController"/> class.
        /// </summary>
        /// <param name="certificationService">The certification service.</param>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="enumService">The enum service.</param>
        public CertificationController(ICertificationService certificationService, ICredentialService credentialService,
            IEnumService enumService)
        {
            CertificationService = certificationService;
            CredentialService = credentialService;
            EnumService = enumService;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the services.
        /// </summary>
        protected void DisposeServices(HttpRequestMessage message)
        {
            message.RegisterForDispose(CertificationService);
            message.RegisterForDispose(CredentialService);
        }

        #endregion

        #region Options Endpoints

        /// <summary>
        /// Get Certification Options
        /// </summary>
        /// <returns>A Certification options response object</returns>
        [HttpOptions]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "CertificationOptions", typeof(CertificationOptionsResponseResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.CertificationOptions, Name = ProgramResourceConstants.RouteNames.Certifications.CertificationOptions)]
        public IHttpActionResult CertificationOptions()
        {
            try
            {
                return Ok(new CertificationOptionsResponseResource(Url));
            }
            catch(Exception ex)
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
        /// Gets the values of a specific Certification enum
        /// </summary>
        /// <param name="name">The name of the enum as provided by the route.</param>
        /// <returns>An array of enum values available for the given enum.</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetEnum", typeof(object))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.CertificationEnumValues, Name = ProgramResourceConstants.RouteNames.Certifications.CertificationEnumValues)]
        public IHttpActionResult GetEnum(string name)
        {
            try
            {
                return GetEnumValuesForModel<Certification>(name, EnumService, EnumLinkSettings.IncludeNestedEnumTypesInLinks,
                    ProgramResourceConstants.RouteNames.Certifications.CertificationEnumValues);
            }
            finally
            {
                DisposeServices(Request);
            }
        }
        
        #endregion

        #region Retrieve Endpoints
        
        /// <summary>
        /// Get all Certifications
        /// </summary>
        /// <returns>A CertificationFullCollectionResource containing a collection of Certification objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertifications", typeof(CertificationFullCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCertifications, Name = ProgramResourceConstants.RouteNames.Certifications.GetCertifications)]
        public IHttpActionResult GetCertifications([FromUri] PageDefinition pageDefinition)
        {
            try
            {
                if(pageDefinition == null)
                {
                    //Get All
                    var certifications = CertificationService.Search();
                    var fullResource = Mapper.Map<IEnumerable<Certification>, CertificationFullCollectionResource>(certifications, o => o.Items["UrlHelper"] = Url);
                    return Ok(fullResource);
                }
                else
                {
                    //first ensure we have a valid PageDefinition
                    string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                    if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                        
                    //perform the Get and return the result
                    int totalCount;
                    var certifications = CertificationService.Search(pageDefinition, out totalCount);
                    if(pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                        return BadRequest(ErrorMessages.PageIndexTooHigh());
                    var resource = Mapper.Map<IEnumerable<Certification>, CertificationFullCollectionResource>(certifications, o => o.Items["UrlHelper"] = Url);
                    resource.SetPaging(pageDefinition, totalCount, Url);
                    return Ok(resource);
                }
            }
            catch(Exception ex)
            {
                return HandleQueryException(ex, pageDefinition);
            }
            finally
            {
                DisposeServices(Request);
            }
        }
            
        /// <summary>
        /// Get all Certifications
        /// </summary>
        /// <returns>A CertificationFullCollectionResource containing a collection of Certification objects</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertificationsPost", typeof(CertificationFullCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCertifications, Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsPost)]
        public IHttpActionResult GetCertificationsPost([FromBody] PageDefinition pageDefinition)
        {
            try
            {
                if(pageDefinition == null)
                {
                    //Get All
                    var certifications = CertificationService.Search();
                    var fullResource = Mapper.Map<IEnumerable<Certification>, CertificationFullCollectionResource>(certifications, o => o.Items["UrlHelper"] = Url);
                    return Ok(fullResource);
                }
                else
                {
                    //first ensure we have a valid PageDefinition
                    string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                    if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                        
                    //perform the Get and return the result
                    int totalCount;
                    var certifications = CertificationService.Search(pageDefinition, out totalCount);
                    if(pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                        return BadRequest(ErrorMessages.PageIndexTooHigh());
                    var resource = Mapper.Map<IEnumerable<Certification>, CertificationFullCollectionResource>(certifications,
                        o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsPost; });
                    resource.SetPaging(pageDefinition, totalCount, Url);
                    return Ok(resource);
                }
            }
            catch(Exception ex)
            {
                return HandleQueryException(ex, pageDefinition);
            }
            finally
            {
                DisposeServices(Request);
            }
        }
            
        /// <summary>
        /// Get all Certifications for a user
        /// </summary>
        /// <returns>A CertificationCollectionResource containing a collection of Certification objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertificationsForUser", typeof(CertificationCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCertificationsByMemberId, Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsByMemberId)]
        public IHttpActionResult GetCertificationsForUser(Guid memberId, [FromUri] PageDefinition pageDefinition)
        {
            try
            {
                //first ensure we have a valid PageDefinition
                string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                
                int totalCount;
                var certifications = CredentialService.SearchCertsByMemberId(memberId, pageDefinition, out totalCount); 
                var resource = Mapper.Map<IEnumerable<Certification>, CertificationCollectionResource>(certifications,
                   o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsByMemberId; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch(Exception ex)
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
        /// Get all Certifications for a user (Post)
        /// </summary>
        /// <returns>A CertificationCollectionResource containing a collection of Certification objects</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertificationsForUserPost", typeof(CertificationCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCertificationsByMemberIdPost, Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsByMemberIdPost)]
        public IHttpActionResult GetCertificationsForUserPost(Guid memberId, [FromBody] PageDefinition pageDefinition)
        {
            try
            {
                //first ensure we have a valid PageDefinition
                string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                
                int totalCount;
                var certifications = CredentialService.SearchCertsByMemberId(memberId, pageDefinition, out totalCount); 
                var resource = Mapper.Map<IEnumerable<Certification>, CertificationCollectionResource>(certifications,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsByMemberIdPost; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch(Exception ex)
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
        /// Get all Certifications for the current user
        /// </summary>
        /// <returns>A CertificationCollectionResource containing a collection of Certification objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [ImpersonateMemberId]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertificationsForCurrentUser", typeof(CertificationCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCurrentUserCertifications, Name = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertifications)]
        public IHttpActionResult GetCertificationsForCurrentUser([FromUri] PageDefinition pageDefinition, Guid? memberId = null)
        {
            if(UserProfile == null || UserProfile.ProfileId.Equals(Guid.Empty))
            {
                //this constitutes a 500 because if the ResourceAuthorization claims they were authorized to hit this, they should have had a ProfileId found
                Logger.Error("ProfileId not found, in GetCertificationsForCurrentUser");
                return Content(HttpStatusCode.InternalServerError, ErrorMessages.ProfileIdNotFound);
            }
            
            Guid targetProfileId = ProfileId;
            if(memberId.HasValue)
            {
                if(!_backDoorValues.Any())
                    return Content(HttpStatusCode.InternalServerError, "The ADFS claim type is unknown");
                if(!UserProfile.Claims.Any(c => c.Type.Contains("identity/claims/role") && _backDoorValues.Contains(c.Value)))
                    return Content(HttpStatusCode.Forbidden, ErrorMessages.ForbiddenResource());
                targetProfileId = memberId.Value;
            }
            
            try
            {
                //first ensure we have a valid PageDefinition
                string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                
                int totalCount;
                var certifications = CredentialService.SearchCertsByMemberId(targetProfileId, pageDefinition, out totalCount); 
                if(pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Certification>, CertificationCollectionResource>(certifications,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertifications; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch(Exception ex)
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
        /// Get all Certifications for the current user (Post)
        /// </summary>
        /// <returns>A CertificationCollectionResource containing a collection of Certification objects</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize]
        [ImpersonateMemberId]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertificationsForCurrentUserPost", typeof(CertificationCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCurrentUserCertificationsPost, Name = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertificationsPost)]
        public IHttpActionResult GetCertificationsForCurrentUserPost([FromBody] PageDefinition pageDefinition, Guid? memberId = null)
        {
            if(UserProfile == null || UserProfile.ProfileId.Equals(Guid.Empty))
            {
                //this constitutes a 500 because if the ResourceAuthorization claims they were authorized to hit this, they should have had a ProfileId found
                Logger.Error("ProfileId not found, in GetCertificationsForCurrentUserPost");
                return Content(HttpStatusCode.InternalServerError, ErrorMessages.ProfileIdNotFound);
            }
            
            Guid targetProfileId = ProfileId;
            if(memberId.HasValue)
            {
                if(!_backDoorValues.Any())
                    return Content(HttpStatusCode.InternalServerError, "The ADFS claim type is unknown");
                if(!UserProfile.Claims.Any(c => c.Type.Contains("identity/claims/role") && _backDoorValues.Contains(c.Value)))
                    return Content(HttpStatusCode.Forbidden, ErrorMessages.ForbiddenResource());
                targetProfileId = memberId.Value;
            }
            
            try
            {
                //first ensure we have a valid PageDefinition
                string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                
                int totalCount;
                var certifications = CredentialService.SearchCertsByMemberId(targetProfileId, pageDefinition, out totalCount).ToList(); 
                if(pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                    return BadRequest(ErrorMessages.PageIndexTooHigh());
                var resource = Mapper.Map<IEnumerable<Certification>, CertificationCollectionResource>(certifications,
                    o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertificationsPost; });
                resource.SetPaging(pageDefinition, totalCount, Url);
                return Ok(resource);
            }
            catch(Exception ex)
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
        /// Get a Certification by Id
        /// </summary>
        /// <param name="id">The id of the Certification (a Guid)</param>
        /// <returns>A Certification object</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetCertificationById", typeof(CertificationResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.GetCertificationById, Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationById)]
        public IHttpActionResult GetCertificationById(Guid id)
        {
            if(id.Equals(Guid.Empty))
                return BadRequest("Id is empty");
            
            try
            {
                var certification = CertificationService.Load(id);
                if(certification == null)
                {
                    var msg = ErrorMessages.NotFound("Certification", id);
                    return Content(HttpStatusCode.NotFound, msg);
                }
                var resource = Mapper.Map<Certification, CertificationResource>(certification, o => o.Items["UrlHelper"] = Url);
                return Ok(resource);
            }
            catch(Exception ex)
            {
                HandleExceptionLogging(ex, $"id: '{id}'");
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
        [Route(ProgramResourceConstants.Routes.Certifications.GetCertificationInvalidId, Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationInvalidId)]
        public IHttpActionResult GetWithInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }

        #endregion
        
        #region ETL processing endpoints

        #region Add Method
        /// <summary>
        /// Add a Certification 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPost]
        [ResourceAuthorize(Actions.Create)]
        [SwaggerResponse(HttpStatusCode.OK, "AddCertification", typeof(CertificationResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.AddCertification, Name = ProgramResourceConstants.RouteNames.Certifications.AddCertification)]
        public async Task<IHttpActionResult> AddCertification([FromBody] AddCertificationCommand command)
        {
            try
            {
                if (command == null) return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError()) return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = Helpers.SetUserNameInProfile(UserProfile, "backgroundClient");

                var commandResult = await CertificationService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Certification, CertificationResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));
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

        #region Update Method
        /// <summary>
        /// Update a Certification 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        [HttpPut]
        [ResourceAuthorize(Actions.Update)]
        [SwaggerResponse(HttpStatusCode.OK, "UpdateCertification", typeof(CertificationResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Certifications.UpdateCertification, Name = ProgramResourceConstants.RouteNames.Certifications.UpdateCertification)]
        public async Task<IHttpActionResult> UpdateCertification([FromBody] UpdateCertificationCommand command)
        {
            try
            {
                if (command == null) return BadRequest(ErrorMessages.CommandNotBound);

                if (ModelStateExplicitValidationError()) return BadRequest(GetModelStateErrorMessage());

                command.UserInfo = Helpers.SetUserNameInProfile(UserProfile, "backgroundClient");

                var commandResult = await CertificationService.Handle(command);

                if (!commandResult.Succeeded)
                    return BadRequest(commandResult.Message);

                return Ok(Mapper.Map<Certification, CertificationResource>(commandResult.Data, o => o.Items["UrlHelper"] = Url));

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

        #endregion

    }
}
