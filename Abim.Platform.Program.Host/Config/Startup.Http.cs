using Abim.Enterprise.Core.Swagger;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Api;
using Abim.Platform.Program.WebApi.Api.Configuration.Serialization;
using Abim.Platform.Program.WebApi.Api.Configuration.Swagger;
using Abim.Platform.Program.WebApi.Api.Handlers;
using Abim.Platform.Program.WebApi.Exceptions;
using Abim.Platform.Program.WebApi.Formatters;
using Abim.Platform.Program.WebApi.NLog;
using Microsoft.Owin.Extensions;
using Newtonsoft.Json.Converters;
using Owin;
using Swashbuckle.Application;
using System;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Dispatcher;
using System.Web.Http.ExceptionHandling;
using WebApiContrib.Formatting;
using WebApiThrottle;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// StartUp Class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// The use protocol buffers for profile interservice setting
        /// </summary>
        public const bool UseProtocolBuffersForProfileInterservice = true;

        /// <summary>
        /// The use protocol buffers for product interservice setting
        /// </summary>
        public const bool UseProtocolBuffersForProductInterservice = true;

        /// <summary>
        /// The use protocol buffers for program interservice setting
        /// </summary>
        public const bool UseProtocolBuffersForProgramInterservice = true;

        /// <summary>
        /// The use protocol buffers for registration interservice setting
        /// </summary>
        public const bool UseProtocolBuffersForRegistrationInterservice = true;
        
        /// <summary>
        /// UseWebApi method.
        /// </summary>
        /// <returns></returns>
        internal static void UseHttpConfig(IAppBuilder app)
        {
            if (new Feature_RestApi().FeatureEnabled)
            {
                Logger.Info("FEATURE :: WEB API --> ENABLED");
                Logger.Trace("UseWebApi Begin");
                
                var config = CreateHttpConfiguration();
                
                app.UseStageMarker(PipelineStage.MapHandler);
                app.UseWebApi(config);
            }
            else
            {
                Logger.Warn("FEATURE :: WEB API --> DISABLED");
            }
            
            Logger.Trace("UseWebApi Complete");
        }

        private static HttpConfiguration CreateHttpConfiguration()
        {
            var config = new HttpConfiguration();
            
            config.EnableCors(new EnableCorsAttribute(origins: "*", headers: "*", methods: "*"));
            config.IncludeErrorDetailPolicy = IncludeErrorDetailPolicy.Always;
            config.MapHttpAttributeRoutes();
            
            config.Routes.MapHttpRoute(
                name: "ResourceNotFound",
                routeTemplate: ProgramResourceConstants.Routes.Prefix.ApiVersion + "/{*uri}",
                defaults: new { controller = "AppInfo", action = "ResourceNotFound", uri = RouteParameter.Optional }
            );
            
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());
            GlobalExceptionHandler.AppName = ProgramResourceConstants.AppInfo.DisplayName;
            GlobalExceptionHandler.AppVersion = ProgramResourceConstants.ApiInfo.Version;
            ApiPlatform.PlatformName = ProgramResourceConstants.AppInfo.BasicName;
            config.Services.Add(typeof(IExceptionLogger), new NLogExceptionLogger());
            config.Services.Replace(typeof(System.Web.Http.Tracing.ITraceWriter), new NLogTraceWriter());
            config.Services.Replace(typeof(IHttpControllerActivator), new SmServiceActivator(DependencyResolver.Container));
            
            config.MessageHandlers.Add(new RequireHttpsHandler());
            config.MessageHandlers.Add(new PreflightRequestsHandler());
            //*** Register throttling handler *******************************************
            // ++++ for more information: https://github.com/stefanprodan/WebApiThrottle
            config.MessageHandlers.Add(new ThrottlingHandler()
            {
                Policy = ThrottlePolicy.FromStore(new PolicyConfigurationProvider()), // throttle policy defined as xml in App.config (throttlePolicy)
                //PolicyRepository = new PolicyMemoryCacheRepository(), // can be accessed in code like (new PolicyCacheRepository()).FirstOrDefault(ThrottleManager.GetPolicyKey())
                Repository = new MemoryCacheRepository() // uses the runtime memory cache for self-hosting WebApi with Owin. can use also Velocity, Redis or a NoSQL database
            });
            
            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());
            config.Formatters.Add(new XmlMediaTypeFormatter());
            config.Formatters.Add(new TextMediaTypeFormatter());
            config.Formatters.Add(new ProtoBufFormatter());
            config.Formatters.JsonFormatter.SerializerSettings.ContractResolver = new CamelCaseAbimContractResolver();
            config.Formatters.JsonFormatter.SerializerSettings.Converters.Add(new StringEnumConverter());
            config.Formatters.JsonFormatter.AddUriPathExtensionMapping("json", "application/json");
            config.Formatters.JsonFormatter.SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
            config.Formatters.JsonFormatter.AddQueryStringMapping("format", "json", "application/json");
            config.Formatters.JsonFormatter.AddRequestHeaderMapping("ReturnType", "json", StringComparison.InvariantCultureIgnoreCase, false, "application/json");
            config.Formatters.XmlFormatter.AddUriPathExtensionMapping("xml", "application/xml");
            config.Formatters.XmlFormatter.AddQueryStringMapping("format", "xml", "application/xml");
            config.Formatters.XmlFormatter.AddRequestHeaderMapping("ReturnType", "xml", StringComparison.InvariantCultureIgnoreCase, false, "application/xml");
            
            DefaultContentNegotiator negotiator = new DefaultContentNegotiator(excludeMatchOnTypeOnly: true);
            config.Services.Replace(typeof(IContentNegotiator), negotiator);
            
            config.EnsureInitialized();
            
            bool isSwaggerEnabled;
            Boolean.TryParse(ConfigurationManager.AppSettings["EnableSwagger"], out isSwaggerEnabled);
            if (isSwaggerEnabled)
            {
                config.EnableSwagger(c =>
                {
                    c.DescribeAllEnumsAsStrings();
                    c.SingleApiVersion(ProgramResourceConstants.ApiInfo.Version, ProgramResourceConstants.AppInfo.ProductName);
                    c.DocumentFilter<HideFromSwaggerFilter>();
                    
                    var authorityUri = new Uri(ConfigurationManager.AppSettings["Authority"]);
                    
                    c.OAuth2("oauth")
                        .AuthorizationUrl(authorityUri.ToString())
                        .TokenUrl(new Uri(authorityUri, "connect/token").ToString())
                        .Flow("implicit");
                }).EnableSwaggerUi(x => x.AddResourceOwnerFlowSupport());
            }
            
            Logger.Trace("UseHttpConfig: End HTTP Configuration.");
            return config;
        }
    }
}
