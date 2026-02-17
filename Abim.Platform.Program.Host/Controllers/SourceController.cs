using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.Extensions;
using Abim.Platform.Program.Host.OptionsResources;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Api.Attributes;
using Abim.Platform.Program.WebApi.Attributes;
using Abim.Platform.Program.WebApi.Objects;
using AutoMapper;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Thinktecture.IdentityModel.WebApi;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    /// <summary>
    /// SourceController
    /// </summary>
    [RoutePrefix(ProgramResourceConstants.Routes.Prefix.Source)]
    public class SourceController : ControllerBase
    {
        #region Properties
            
        /// <summary>
        /// The source service
        /// </summary>
        protected ISourceService SourceService { get; set; }
            
        /// <summary>
        /// The enum service
        /// </summary>
        protected IEnumService EnumService { get; set; }
        
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="SourceController"/> class.
        /// </summary>
        /// <param name="sourceService">The source service.</param>
        /// <param name="enumService">The enum service.</param>
        public SourceController(ISourceService sourceService, IEnumService enumService)
        {
            SourceService = sourceService;
            EnumService = enumService;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the services.
        /// </summary>
        protected void DisposeServices(HttpRequestMessage message)
        {
            message.RegisterForDispose(SourceService);
        }

        #endregion

        #region Options Endpoints

        /// <summary>
        /// Get Source Options
        /// </summary>
        /// <returns>A Source options response object</returns>
        [HttpOptions]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "SourceOptions", typeof(SourceOptionsResponseResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Sources.SourceOptions, Name = ProgramResourceConstants.RouteNames.Sources.SourceOptions)]
        public IHttpActionResult SourceOptions()
        {
            try
            {
                return Ok(new SourceOptionsResponseResource(Url));
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
        /// Gets the values of a specific Source enum
        /// </summary>
        /// <param name="name">The name of the enum as provided by the route.</param>
        /// <returns>An array of enum values available for the given enum.</returns>
        [HttpGet] [HideFromSwagger]
        [Authorize]
        [ResourceAuthorize]
        [Route(ProgramResourceConstants.Routes.Sources.SourceEnumValues, Name = ProgramResourceConstants.RouteNames.Sources.SourceEnumValues)]
        public IHttpActionResult GetEnum(string name)
        {
            try
            {
                return GetEnumValuesForModel<Source>(name, EnumService, EnumLinkSettings.IncludeNestedEnumTypesInLinks,
                                            ProgramResourceConstants.RouteNames.Sources.SourceEnumValues);
            }
            finally
            {
                DisposeServices(Request);
            }
        }
            
        #endregion

        #region Retrieve Endpoints
            
        /// <summary>
        /// Get all Sources
        /// </summary>
        /// <returns>A SourceCollectionResource containing a collection of Source objects</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize(Actions.ViewAllAdmin)]
        [SwaggerResponse(HttpStatusCode.OK, "GetSources", typeof(SourceCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Sources.GetSources, Name = ProgramResourceConstants.RouteNames.Sources.GetSources)]
        public IHttpActionResult GetSources([FromUri] PageDefinition pageDefinition)
        {
            try
            {
                if (pageDefinition == null)
                {
                    //Get All
                    var allSources = SourceService.Search();
                    var fullResource = Mapper.Map<IEnumerable<Source>, SourceCollectionResource>(allSources, o => o.Items["UrlHelper"] = Url);
                    return Ok(fullResource);
                }
                else
                {
                    //first ensure we have a valid PageDefinition
                    string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                    if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                        
                    //perform the Get and return the result
                    int totalCount;
                    var sources = SourceService.Search(pageDefinition, out totalCount);
                    if (pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                        return BadRequest(ErrorMessages.PageIndexTooHigh());
                    var resource = Mapper.Map<IEnumerable<Source>, SourceCollectionResource>(sources, o => o.Items["UrlHelper"] = Url);
                    resource.SetPaging(pageDefinition, totalCount, Url);
                    return Ok(resource);
                }
            }
            catch (Exception ex)
            {
                return HandleQueryException(ex, pageDefinition);
            }
            finally
            {
                DisposeServices(Request);
            }
        }
            
        /// <summary>
        /// Get all Sources
        /// </summary>
        /// <returns>A SourceCollectionResource containing a collection of Source objects</returns>
        [HttpPost]
        [Authorize]
        [ResourceAuthorize(Actions.ViewAllAdmin)]
        [SwaggerResponse(HttpStatusCode.OK, "GetSourcesPost", typeof(SourceCollectionResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Sources.GetSources, Name = ProgramResourceConstants.RouteNames.Sources.GetSourcesPost)]
        public IHttpActionResult GetSourcesPost([FromBody] PageDefinition pageDefinition)
        {
            try
            {
                if (pageDefinition == null)
                {
                    //Get All
                    var allSources = SourceService.Search();
                    var fullResource = Mapper.Map<IEnumerable<Source>, SourceCollectionResource>(allSources, o => o.Items["UrlHelper"] = Url);
                    return Ok(fullResource);
                }
                else
                {
                    //first ensure we have a valid PageDefinition
                    string queryValidationErrorMsg = ValidatePaging(ref pageDefinition);
                    if(queryValidationErrorMsg != null) return BadRequest(queryValidationErrorMsg);
                        
                    //perform the Get and return the result
                    int totalCount;
                    var sources = SourceService.Search(pageDefinition, out totalCount);
                    if (pageDefinition.SkippedItems >= totalCount && totalCount > 0)
                        return BadRequest(ErrorMessages.PageIndexTooHigh());
                    var resource = Mapper.Map<IEnumerable<Source>, SourceCollectionResource>(sources,
                        o => { o.Items["UrlHelper"] = Url; o.Items["Action"] = ProgramResourceConstants.RouteNames.Sources.GetSourcesPost; });
                    resource.SetPaging(pageDefinition, totalCount, Url);
                    return Ok(resource);
                }
            }
            catch (Exception ex)
            {
                return HandleQueryException(ex, pageDefinition);
            }
            finally
            {
                DisposeServices(Request);
            }
        }
            
        /// <summary>
        /// Get a Source by Id
        /// </summary>
        /// <param name="id">The id of the Source (a Guid)</param>
        /// <returns>A Source object</returns>
        [HttpGet]
        [Authorize]
        [ResourceAuthorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetSourceById", typeof(SourceResource))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.Sources.GetSourceById, Name = ProgramResourceConstants.RouteNames.Sources.GetSourceById)]
        public IHttpActionResult GetSourceById(Guid id)
        {
            try
            {
                var source = SourceService.Load(id);
                if(source == null)
                {
                    var msg = ErrorMessages.NotFound("Source", id);
                    return Content(HttpStatusCode.NotFound, msg);
                }
                var resource = Mapper.Map<Source, SourceResource>(source, o => o.Items["UrlHelper"] = Url);
                return Ok(resource);
            }
            catch (Exception ex)
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
        [Route(ProgramResourceConstants.Routes.Sources.GetSourceInvalidId, Name = ProgramResourceConstants.RouteNames.Sources.GetSourceInvalidId)]
        public IHttpActionResult GetWithInvalidId(string invalidId)
        {
            return BadRequest(ErrorMessages.InvalidGuidId);
        }
        
        #endregion
    }
}
