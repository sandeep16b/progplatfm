using Abim.Platform.Program.Extensions.ExternalResponses;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Objects.Extensions;
using Abim.Platform.Program.WebApi.Responses;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using WebApi.OutputCache.V2;

namespace Abim.Platform.Program.WebApi
{
    /// <summary>
    /// Common methods used by all API Controllers
    /// </summary>
    public class ControllerBase : ApiController
    {
        #region Properties

        /// <summary>
        /// The base protected readonly logger
        /// </summary>
        protected readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// The log rhythm authentication logger
        /// </summary>
        protected static readonly ILogger LogRhythmAuthenticationLogger = LogManager.GetLogger("ABIM-LogRhythm-Authentication");

        #endregion

        #region Statics

        /// <summary>
        /// Gets the token.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static string GetToken(HttpRequestMessage request)
        {
            if(request == null) return null;
            if(request.Headers.Authorization == default(AuthenticationHeaderValue)) return null;
            string value = request.Headers.Authorization.ToString();
            return value;
        }

        #endregion

        #region Caching

        /// <summary>
        /// Invalidates the cache for the specified method on the specified controller.
        /// </summary>
        /// <param name="methodName">Name of the method.</param>
        /// <param name="controller">The controller.</param>
        protected void InvalidateCache(string methodName, Type controller)
        {
            try
            {
                var controllerFullName = controller.FullName;
                Logger.Debug("Start Invalidate Cache for {0} - {1}.", controllerFullName, methodName);
                var cache = Configuration.CacheOutputConfiguration().GetCacheOutputProvider(Request);
                cache.RemoveStartsWith(Configuration.CacheOutputConfiguration().MakeBaseCachekey(controllerFullName, methodName));
                Logger.Debug("End Invalidate Cache for {0} - {1}.", controllerFullName, methodName);
            }
            catch (Exception ex)
            {
                Logger.Error("Invalidate Cache failed with error: {0}.", ex.Message);
            }
        }

        /// <summary>
        /// Invalidates the cache.
        /// </summary>
        /// <param name="memberName">Name of the member.</param>
        protected virtual void InvalidateCache([CallerMemberName] string memberName = null)
        {
            if(memberName == null)
                return;

            var type = GetType();
            var member = type.GetMember(memberName).FirstOrDefault();
            if(member == null)
                return;

            var attr = member.CustomAttributes.FirstOrDefault(a => a.AttributeType == typeof(RouteAttribute));
            if(attr == null)
                return;

            var name = attr.NamedArguments.FirstOrDefault(a => a.MemberName == "Name").TypedValue.Value as string;
            if(string.IsNullOrEmpty(name))
                return;

            InvalidateCache(name, type);
        }

        #endregion Caching

        #region User Info

        /// <summary>
        /// Gets the user's profile info
        /// </summary>
        protected UserInfo UserProfile
        {
            get
            {
                var token = GetToken(Request);
                if(token == null) return null;
                return UserInfo.Create(Request);
            }
        }
        
        /// <summary>
        /// Get the GUID of the currently authenticated user based on their claim.
        /// </summary>
        /// <returns>The GUID of the current user or Guid.Empty if one can not be determined.</returns>
        protected Guid ProfileId 
        {
            get
            {
                return UserProfile.ProfileId;
            }
        }

        #endregion

        #region Enum Values

        /// <summary>
        /// Gets the values of a specific enum
        /// </summary>
        /// <param name="name">The name of the enum as provided by the route.</param>
        /// <param name="enumService">The enum service.</param>
        /// <returns>An array of enum values available for the given enum.</returns>
        public IHttpActionResult GetEnumValues(string name, IEnumService enumService, string selfLinkRouteName = null)
        {
            if(string.IsNullOrEmpty(name))
                return BadRequest(ErrorMessages.InvalidParameters("No enum name specified"));
            
            try
            {
                var e = enumService.GetEnumDefinitions().FirstOrDefault(i => i.Name.ToLower() == name.ToLower());
                if(e == null)
                {
                    //if none were found, try again with -s, in case they passed in e.g. "certificationType" instead of "certificationTypes"
                    e = enumService.GetEnumDefinitions().FirstOrDefault(i => i.Name.ToLower() == name.ToLower() + "s");
                    if(e == null) return NotFound();
                }
                
                Logger.Info("Start GetEnum for " + e.Type.Name);
                
                var executeMethod = typeof(ControllerBase)
                                   .GetMethod("Execute", BindingFlags.NonPublic | BindingFlags.Instance)
                                   .MakeGenericMethod(e.Type);
                
                return executeMethod.Invoke(this, new Object[]{ selfLinkRouteName }) as IHttpActionResult;
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }
        
        /// <summary>
        /// Gets the values of a specific enum, *IF* it's used in a certain model. Otherwise, it's considered a Bad Request
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="enumService">The enum service.</param>
        /// <returns></returns>
        public IHttpActionResult GetEnumValuesForModel<TModel>(string name, IEnumService enumService, bool includeNestedEnumTypes, string selfLinkRouteName = null)
        {
            var supportedTypes = enumService.GetEnumTypesFor(typeof(TModel), includeNestedEnumTypes);
            if(!supportedTypes.Any(t => t.Name.ToLower() == name.ToLower() || t.Name.ToLower() + "s" == name.ToLower()))
                return BadRequest(ErrorMessages.InvalidEnumType_ValidTypesAre(typeof(TModel), supportedTypes, includeNestedEnumTypes));
            return GetEnumValues(name, enumService, selfLinkRouteName);
        }

        /// <summary>
        /// Implements the shared query logic for the individual enum value requests.
        /// </summary>
        /// <returns>
        /// An IHttpActionResult object to be returned from the API method itself.
        /// </returns>
        private IHttpActionResult Execute<T>(string selfLinkRouteName) 
            where T : struct, IConvertible, IComparable, IFormattable
        {
            try
            {
                var resource = CreateEnumTypeResponseResource<T>();
                //var resource = EnumTypeResponseResource<T>.Create();
                if (selfLinkRouteName != null && Url != null)
                {
                    resource.Links.Add(new Link()
                    {
                        Name = "self",
                        Href = Url.Link(selfLinkRouteName, new { }),
                        Method = HttpVerbs.Get.ToString().ToUpper()
                    });
                }
                return Ok(resource);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        /// <summary>
        /// Creates a EnumTypeResponseResource
        /// </summary>
        public static EnumTypeResponseResource<TEnum> CreateEnumTypeResponseResource<TEnum>()
            where TEnum : struct, IConvertible, IComparable, IFormattable
        {
            var instance = new EnumTypeResponseResource<TEnum>();
            foreach (TEnum val in EnumAttributes.GetEnumValues<TEnum>(true))
            {
                var valueResponseResource = new EnumValueResponseResource<TEnum>(val);
                instance.Values.Add(valueResponseResource);
            }
            return instance;
        }

        #endregion

        #region Exception Handling

        /// <summary>
        /// HandleQueryException crops off stack traces, namespaces, and other sensitive information from the
        /// message returned to the end-user, while preserving them in the logs
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="query">The query.</param>
        /// <returns></returns>
        protected IHttpActionResult HandleQueryException(Exception ex, ComplexQueryBase query)
        {
            return HandleQueryException(ex, query.PageDefinition);
        }

        /// <summary>
        /// Handles the query exception.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <param name="paging">The paging.</param>
        /// <returns></returns>
        protected virtual IHttpActionResult HandleQueryException(Exception ex, PageDefinition paging)
        {
            HttpStatusCode statusCode;
            if(ex is HttpRequestException)
            {
                HandleExceptionLogging(ex, null , paging, isWarnLogging:true);
                statusCode = HttpStatusCode.RequestTimeout;
                return Content(statusCode, "Operation cancelled");
            }
            else
            {
                HandleExceptionLogging(ex, null, paging, isWarnLogging: false);
                statusCode = HttpStatusCode.InternalServerError;
                return Content(statusCode, ex.Message);
            }
        }

        /// <summary>
        /// Handles the query exception logging.
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="inputString"></param>
        /// <param name="inputObj"></param>
        /// <param name="isWarnLogging"></param>
        protected virtual void HandleExceptionLogging(Exception ex, string inputString, object inputObj = null , bool isWarnLogging = false )
        {
            StringBuilder message = new StringBuilder();

            message.Append($"RequestingUser: '{GetUserName()}' ,");

            if (!string.IsNullOrEmpty(inputString))
                message.Append($"{inputString}, ");

            if (inputObj!=null)
                message.Append($"Incoming Params: '{JsonConvert.SerializeObject(inputObj)}' ");

            if (isWarnLogging)
                Logger.Warn(ex, message.ToString());
            else
                Logger.Error(ex, message.ToString());

        }

        /// <summary>
        /// GetUserName from Claims or AbimId
        /// </summary>
        /// <param name="controller"></param>
        /// <returns></returns>
        public string GetUserName()
        {
            string userName = "";

            var abimId = UserProfile?.AbimId;
            var clientIdClaim = UserProfile?.Claims.FirstOrDefault(x => x.Type == "client_id");
            var preferredUsernameClaim = UserProfile?.Claims.FirstOrDefault(x => x.Type == "preferred_username");
            var nameIdentifierClaim = UserProfile?.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(abimId))
                userName = $"abimId:{abimId}";          
            else if (clientIdClaim != null)
                userName = $"clientId:{clientIdClaim.Value}";
            else if (preferredUsernameClaim != null)
                userName = preferredUsernameClaim.Value;
            else if (nameIdentifierClaim != null)
                userName = $"nameIdentifier:{nameIdentifierClaim.Value}";
            else
                userName = "cannot determine user Name from Claims";

            return userName;
        }

        /// <summary>
        /// The purpose of this method is to differentiate an explicitly-coded ModelState error, such as a failed [AntiXss] check, from
        /// an exception which occurred when trying to bind user input to the command which we may not care about. Therefore this will
        /// not be the same as ModelState.IsValid which would return false in both of those scenarios. This would only return false in
        /// the first.
        /// </summary>
        /// <returns></returns>
        protected bool ModelStateExplicitValidationError()
        {
            return ModelState.Values.Any(v => v.Errors.Any(e => !string.IsNullOrEmpty(e.ErrorMessage)));
        }

        /// <summary>
        /// Gets the model state error message.
        /// </summary>
        /// <returns></returns>
        protected string GetModelStateErrorMessage()
        {
            var message = ReadModelStateErrorMessage(ModelState);
            return message;
        }

        /// <summary>
        /// Reads a model state error message.
        /// </summary>
        /// <returns></returns>
        public static string ReadModelStateErrorMessage(ModelStateDictionary modelState)
        {
            var message = string.Join("; ", modelState.Values.Select(v => string.Join(", ", v.Errors.Select(e => ReadError(e)).ToArray())).ToArray());
            return message;
        }

        /// <summary>
        /// Reads the error.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public static string ReadError(ModelError error)
        {
            if(error == null) return "(A null validation error occurred)";   //this shouldn't be possible but it's still better than a NullReferenceException
            if(!string.IsNullOrEmpty(error.ErrorMessage))
                return error.ErrorMessage;
            if(error.Exception != null && !string.IsNullOrEmpty(error.Exception.Message))
                return error.Exception.Message;
            return "(A validation error occurred with no message)";
        }

        #endregion

        #region Paging

        /// <summary>
        /// Validates the PageDefinition, and creates a default one if it is null
        /// </summary>
        /// <param name="paging">The paging (ref param for initialization purposes if null)</param>
        /// <returns></returns>
        protected string ValidatePaging(ref PageDefinition paging)
        {
            if(paging == null)
            {
                paging = new PageDefinition();
                return null;
            }
            List<string> queryValidationErrors;
            if(!paging.Validate(out queryValidationErrors))
                return ErrorMessages.InvalidPaging(string.Join("; ", queryValidationErrors.ToArray()));
            else return null;
        }

        /// <summary>
        /// Validates the ComplexQueryBase. Does NOT create one by default if it is null, because it is expected
        /// that we will have different child classes of ComplexQueryBase which should be instantiated by default
        /// in the controller if need be
        /// </summary>
        /// <param name="query">The query (ref param for initialization purposes if null)</param>
        /// <returns></returns>
        protected string ValidateQuery(ComplexQueryBase query)
        {
            //we cannot instantiate a new one if it's null because there is no default ComplexQuery
            if(query == null) return "A query is required";
            
            List<string> queryValidationErrors;
            if(!query.Validate(out queryValidationErrors))
                return ErrorMessages.InvalidPaging(string.Join("; ", queryValidationErrors.ToArray()));
            else return null;
        }

        #endregion

        #region Post Data

        /// <summary>
        /// Validates the post body.
        /// </summary>
        /// <param name="postedData">The posted data.</param>
        /// <param name="errorMessages">The error messages.</param>
        /// <returns></returns>
        protected bool ValidateCommand(Object postedData, out List<string> errorMessages, bool validateIndividualFields = true)
        {
            errorMessages = new List<string>();
            
            if(postedData == null)
            {
                errorMessages.Add(ErrorMessages.CommandNotBound);
                return false;
            }
            if(!validateIndividualFields) return true;
            
            var json = RequestText();
            if(string.IsNullOrEmpty(json) || json.Trim().Length == 0)
            {
                errorMessages.Add("No request body");
                return false;
            }
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            
            List<string> passedInProperties = dictionary.Keys.ToList();
            List<string> acceptedProperties = postedData.GetType().GetConstructors()
                .OrderByDescending(c => c.GetParameters().Length).First().GetParameters().Select(p => p.Name).ToList();
            
            List<string> badPassedInProperties = passedInProperties.Where(p => !acceptedProperties.Any(a => a.ToLower() == p.ToLower())).ToList();
            if(!badPassedInProperties.Any()) return true;
            
            if(badPassedInProperties.Count == 1)
                errorMessages.Add(string.Format("Invalid post property: '{0}'", badPassedInProperties.First()));
            else
                errorMessages.Add(string.Format("Invalid post properties: '{0}'", string.Join(", ", badPassedInProperties.ToArray())));
            return false;
        }

        #endregion

        #region Util

        /// <summary>
        /// Gets the full Body of the Request, as a string. Requires that the Api is using the BodyStorageHandler
        /// </summary>
        /// <returns>The body text</returns>
        public string RequestText()
        {
            return Request.GetRequestBody();
        }

        #endregion
    }
}
