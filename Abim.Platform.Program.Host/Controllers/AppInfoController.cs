using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Response.AppInfo;
using GreenPipes;
using GreenPipes.Introspection;
using MassTransit;
using Newtonsoft.Json;
using Swashbuckle.Swagger.Annotations;
using System;
using System.Collections.Generic;
using System.Configuration.Abstractions;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Web.Http;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    /// <summary>
    /// Api Controller
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController"/>
    [RoutePrefix(ProgramResourceConstants.Routes.Prefix.ApiVersion)]
    public class AppInfoController : ControllerBase
    {
        #region Properties

        /// <summary>
        /// Gets or sets the bus control.
        /// </summary>
        /// <value>
        /// The bus control.
        /// </value>
        protected IBusControl BusControl { get; set; }

        /// <summary>
        /// Gets or sets the configuration manager.
        /// </summary>
        /// <value>
        /// The configuration manager.
        /// </value>
        private IConfigurationManager ConfigurationManager { get; set; }

        /// <summary>
        /// Gets or sets the enum service.
        /// </summary>
        /// <value>
        /// The enum service.
        /// </value>
        private IEnumService EnumService { get; set; }

        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="AppInfoController"/> class.
        /// </summary>
        /// <param name="enumService">The enum service.</param>
        /// <param name="busControl">The bus control.</param>
        /// <param name="configurationManager">The configuration manager.</param>
        public AppInfoController(IEnumService enumService, IBusControl busControl, IConfigurationManager configurationManager)
        {
            ConfigurationManager = configurationManager;
            EnumService = enumService;
            BusControl = busControl;
        }

        #endregion

        #region Endpoints

        /// <summary>
        /// Gets the service status
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "Status", typeof(string))]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [HttpGet, Route(ProgramResourceConstants.Routes.App.Status, Name = ProgramResourceConstants.RouteNames.App.Status)]
        public string Status()
        {
            string serialized = JsonConvert.SerializeObject(WebApi.Config.HostProperties.Status);
            return serialized;
        }
        
        /// <summary>
        /// GetSystemInfo method.
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetSystemInfo", typeof(SystemInfo))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.System.SystemInfo, Name = ProgramResourceConstants.RouteNames.System.SystemInfo)]
        public IHttpActionResult GetSystemInfo()
        {
            string owinUrl = (ConfigurationManager.AppSettings["OwinUrl"] ?? "").TrimEnd('/');
            return Ok(new SystemInfo
            {
                links = new[]
                {
                    new Link(){ Name = "self", Href = Url.Link(ProgramResourceConstants.RouteNames.System.SystemInfo, null), Method = HttpVerbs.Get.ToString().ToUpper() },
                    new Link(){ Name = "Metrics", Href = owinUrl + "/v2/json", Method = HttpVerbs.Get.ToString().ToUpper(), Enabled = new Feature_Metrics().FeatureEnabled },
                    new Link(){ Name = "Metrics UI", Href = owinUrl + "/metrics", Method = HttpVerbs.Get.ToString().ToUpper(), Enabled = new Feature_Metrics().FeatureEnabled },
                    new Link(){ Name = "Job UI", Href = owinUrl + "/jobs", Method = HttpVerbs.Get.ToString().ToUpper(), Enabled = new Feature_HangfireDashboard().FeatureEnabled },
                    new Link(){ Name = ProgramResourceConstants.RouteNames.System.BusInfo, Href = Url.Link(ProgramResourceConstants.RouteNames.System.BusInfo, null), Method = HttpVerbs.Get.ToString().ToUpper(), Enabled = new Feature_MassTransit().FeatureEnabled },
                    new Link(){ Name = ProgramResourceConstants.RouteNames.System.VersionInfo, Href = Url.Link(ProgramResourceConstants.RouteNames.System.VersionInfo, null), Method = HttpVerbs.Get.ToString().ToUpper() },
                    new Link(){ Name = ProgramResourceConstants.RouteNames.App.Version, Href = Url.Link(ProgramResourceConstants.RouteNames.App.Version, null), Method = HttpVerbs.Get.ToString().ToUpper() },
                    new Link(){ Name = ProgramResourceConstants.RouteNames.App.Uptime, Href = Url.Link(ProgramResourceConstants.RouteNames.App.Uptime, null), Method = HttpVerbs.Get.ToString().ToUpper() },
                    new Link(){ Name = ProgramResourceConstants.RouteNames.App.Status, Href = Url.Link(ProgramResourceConstants.RouteNames.App.Status, null), Method = HttpVerbs.Get.ToString().ToUpper() },
                    new Link(){ Name = ProgramResourceConstants.RouteNames.System.SystemRoot, Href = Url.Link(ProgramResourceConstants.RouteNames.System.SystemRoot, null), Method = HttpVerbs.Options.ToString().ToUpper() }
                }
            });
        }

        /// <summary>
        /// GetSystemInfoExt method.
        /// </summary>
        /// <returns></returns>
        [HttpOptions]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetSystemInfoExt", typeof(SystemInfo))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.System.SystemInfoExt, Name = ProgramResourceConstants.RouteNames.System.SystemInfoExt)]
        public IHttpActionResult GetSystemInfoExt()
        {
            return GetSystemInfo();
        }


        /// <summary>
        /// GetVersionInfo method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "GetVersionInfo", typeof(VersionInfo))]
        [Route(ProgramResourceConstants.Routes.System.VersionInfo, Name = ProgramResourceConstants.RouteNames.System.VersionInfo)]
        public IHttpActionResult GetVersionInfo()
        {
            var assembly = Assembly.GetExecutingAssembly().GetName().Version;
            var file = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).FileVersion;
            var product = FileVersionInfo.GetVersionInfo(Assembly.GetExecutingAssembly().Location).ProductVersion;
            
            return Ok(new VersionInfo
            {
                versionInfo = new VersionDetails { assemblyVersion = assembly, fileVersion = file, productVersion = product },
                links = new[]
                {
                    new Link()
                    {
                        Name = "self",
                        Href = Url.Link(ProgramResourceConstants.RouteNames.System.VersionInfo, null),
                        Method = HttpVerbs.Get.ToString().ToUpper(),
                    }
                }
            });
        }

        /// <summary>
        /// GetVersionInfoExt method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "GetVersionInfoExt", typeof(VersionInfo))]
        [Route(ProgramResourceConstants.Routes.System.VersionInfoExt, Name = ProgramResourceConstants.RouteNames.System.VersionInfoExt)]
        public IHttpActionResult GetVersionInfoExt()
        {
            return GetVersionInfo();
        }

        /// <summary>
        /// GetBusInfo method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetBusInfo", typeof(BusInfo))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.System.BusInfo, Name = ProgramResourceConstants.RouteNames.System.BusInfo)]
        public IHttpActionResult GetBusInfo()
        {
            ProbeResult probeData = BusControl.GetProbeResult();
            return Ok(new BusInfo
            {
                busInfo = probeData.ToString(),
                links = new[]
                {
                    new Link()
                    {
                        Name = "self",
                        Href = Url.Link(ProgramResourceConstants.RouteNames.System.VersionInfo, null),
                        Method = HttpVerbs.Get.ToString().ToUpper(),
                    }
                }
            });
        }

        /// <summary>
        /// GetBusInfoExt method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetBusInfoExt", typeof(BusInfo))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.System.BusInfoExt, Name = ProgramResourceConstants.RouteNames.System.BusInfoExt)]
        public IHttpActionResult GetBusInfoExt()
        {
            return GetBusInfo();
        }

        /// <summary>
        /// GetSystemRoot method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetSystemRoot", typeof(SystemRootInfo))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.System.SystemRoot, Name = ProgramResourceConstants.RouteNames.System.SystemRoot)]
        public IHttpActionResult GetSystemRoot()
        {
            var links = new List<Link>();
            
            //self link
            links.Add(new Link()
            {
                Name = "self",
                Href = Url.Link(ProgramResourceConstants.RouteNames.System.SystemRoot, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            
            //all base API links
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Sources.SourceOptions,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Sources.SourceOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Sources.GetSources,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Sources.GetSources, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Sources.GetSourcesPost,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Sources.GetSourcesPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.CredentialOptions,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Credentials.CredentialOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.IssuanceOptions,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Credentials.IssuanceOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentials,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Credentials.GetCredentials, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCredentialsPost,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Credentials.GetCredentialsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentials, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentialsPost,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Credentials.GetCurrentUserCredentialsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.CertificationOptions,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Certifications.CertificationOptions, null),
                Method = HttpVerbs.Options.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCertifications,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Certifications.GetCertifications, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCertificationsPost,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Certifications.GetCertificationsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertifications,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertifications, null),
                Method = HttpVerbs.Get.ToString().ToUpper()
            });
            links.Add(new Link()
            {
                Name = ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertificationsPost,
                Href = Url.Link(ProgramResourceConstants.RouteNames.Certifications.GetCurrentUserCertificationsPost, null),
                Method = HttpVerbs.Post.ToString().ToUpper()
            });
            
            //Warning: the following routes do not use constants, and may not be intended for exposure. Therefore they are not
            //... included in the links collection here: TimeLimitedCredentials, Schedule TimeLimitedCredentials, Trigger TimeLimitedCredentials Check Consumer,
            //... FPHMCredentials
            
            return Ok(new SystemRootInfo
            {
                links = links
            });
        }


        /// <summary>
        /// GetSystemRootExt method.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [SwaggerResponse(HttpStatusCode.OK, "GetSystemRootExt", typeof(SystemRootInfo))]
        [SwaggerResponse(HttpStatusCode.Unauthorized)]
        [SwaggerResponse(HttpStatusCode.Forbidden)]
        [SwaggerResponse(HttpStatusCode.InternalServerError)]
        [Route(ProgramResourceConstants.Routes.System.SystemRootExt, Name = ProgramResourceConstants.RouteNames.System.SystemRootExt)]
        public IHttpActionResult GetSystemRootExt()
        {
            return GetSystemRoot();
        }

        /// <summary>
        /// Version endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Version", typeof(string))]
        [Route(ProgramResourceConstants.Routes.App.Version, Name = ProgramResourceConstants.RouteNames.App.Version)]
        public string Version()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var productVersionAttributes = assembly.GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
            var version = productVersionAttributes.Length == 1 ?
                            ((AssemblyInformationalVersionAttribute)productVersionAttributes[0]).InformationalVersion :
                            assembly.GetName().Version.ToString();
            return JsonConvert.SerializeObject(version);
        }

        /// <summary>
        /// Uptime endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Uptime", typeof(string))]
        [Route(ProgramResourceConstants.Routes.App.Uptime, Name = ProgramResourceConstants.RouteNames.App.Uptime)]
        public string Uptime()
        {
            string serialized = JsonConvert.SerializeObject(DateTime.Now.Subtract(Process.GetCurrentProcess().StartTime));
            return serialized;
        }

        /// <summary>
        /// Servertime endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        [SwaggerResponse(HttpStatusCode.OK, "Servertime", typeof(string))]
        [Route(ProgramResourceConstants.Routes.App.Servertime, Name = ProgramResourceConstants.RouteNames.App.Servertime)]
        public string Servertime()
        {
            string serialized = JsonConvert.SerializeObject(DateTimeOffset.Now.ToString("o"));
            return serialized;
        }

        /// <summary>
        /// A friendly page not found message endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "GetResourceNotFound", typeof(string))]
        [HttpGet]
        public IHttpActionResult GetResourceNotFound(string uri)
        {
            return Content(HttpStatusCode.NotFound, "Resource not found");
        }

        /// <summary>
        /// A friendly page not found message endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "PostResourceNotFound", typeof(string))]
        [HttpPost]
        public IHttpActionResult PostResourceNotFound(string uri)
        {
            return Content(HttpStatusCode.NotFound, "Resource not found");
        }

        /// <summary>
        /// A friendly page not found message endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "PutResourceNotFound", typeof(string))]
        [HttpPut]
        public IHttpActionResult PutResourceNotFound(string uri)
        {
            return Content(HttpStatusCode.NotFound, "Resource not found");
        }

        /// <summary>
        /// A friendly page not found message endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "OptionsResourceNotFound", typeof(string))]
        [HttpOptions]
        public IHttpActionResult OptionsResourceNotFound(string uri)
        {
            return Content(HttpStatusCode.NotFound, "Resource not found");
        }

        /// <summary>
        /// A friendly page not found message endpoint
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [SwaggerResponse(HttpStatusCode.OK, "DeleteResourceNotFound", typeof(string))]
        [HttpDelete]
        public IHttpActionResult DeleteResourceNotFound(string uri)
        {
            return Content(HttpStatusCode.NotFound, "Resource not found");
        } 

        #endregion
    }
}
