using Abim.Platform.Program.Relational;
using Abim.Platform.Program.WebApi.Api.Constants;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Microsoft.Owin.Testing;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Testing.Setup
{
    /// <summary>
    /// A base class for Controller test classes
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Testing.Setup.BaseScenario" />
    public abstract class BaseHttpServerScenario : BaseScenario
    {
        #region Properties

        /// <summary>
        /// Gets or sets the HTTP server.
        /// </summary>
        /// <value>
        /// The HTTP server.
        /// </value>
        protected TestServer HttpServer { get; set; }

        /// <summary>
        /// Gets or sets the client.
        /// </summary>
        /// <value>
        /// The client.
        /// </value>
        protected HttpClient Client { get; set; }

        /// <summary>
        /// Gets or sets the client key.
        /// </summary>
        /// <value>
        /// The client key.
        /// </value>
        protected KeyValuePair<string, string> BackGroundClientKey { get; set; } = new KeyValuePair<string, string>(
                        ConfigurationManager.AppSettings["backgroundClientId"] ,
                        ConfigurationManager.AppSettings["backgroundClientSecret"]
                        );

        protected KeyValuePair<string, string> IdentityTestClientKey { get; set; } = new KeyValuePair<string, string>(
                ConfigurationManager.AppSettings["IdentityTestClientKey"],
                ConfigurationManager.AppSettings["IdentityTestClientSecret"]
                );

        /// <summary>
        /// Gets or sets the user key.
        /// </summary>
        protected KeyValuePair<string, string> UserKey { get; set; } = new KeyValuePair<string, string>(
                ConfigurationManager.AppSettings["IdentityTestUsername"] ,
                ConfigurationManager.AppSettings["IdentityTestPassword"]                 
                );

        protected string Scopes { get; set; } = "a.r a.w ata.r ata.w b.r b.w c.r c.w f.r f.w n.r n.w pd.r pd.w pf.r pf.w r.r r.w s.r s.w t.r t.w u.r u.w"; 

        protected string GrantType { get; set; } = GrantTypes.Client_credentials; 

        /// <summary>
        /// Gets or sets the token.
        /// </summary>
        /// <value>
        /// The token.
        /// </value>
        protected string Token { get; set; }

        /// <summary>
        /// Gets or sets the URL.
        /// </summary>
        /// <value>
        /// The URL.
        /// </value>
        protected string Url { get; set; }

        /// <summary>
        /// Gets or sets the body.
        /// </summary>
        /// <value>
        /// The body.
        /// </value>
        protected Object Body { get; set; }

        /// <summary>
        /// Gets or sets the result.
        /// </summary>
        /// <value>
        /// The result.
        /// </value>
        protected HttpResponseMessage Result { get; set; }

        /// <summary>
        /// Gets or sets the page definition builder.
        /// </summary>
        /// <value>
        /// The page definition builder.
        /// </value>
        protected PageDefinitionBuilder PageDefinitionBuilder { get; set; }

        /// <summary>
        /// Gets or sets the content of the response.
        /// </summary>
        /// <value>
        /// The content of the response.
        /// </value>
        protected string ResponseContent { get; set; }

        /// <summary>
        /// Gets or sets the content of the GetToken response.
        /// </summary>
        /// <value>
        /// The content of the token response.
        /// </value>
        protected string TokenResponseContent { get; set; }

        #endregion

        #region Fields

        #region Settings

        /// <summary>
        /// Somtimes, we may want to use DependencyResolver.Container instead of making a new one
        /// </summary>
        /// <value>
        /// <c>true</c> if [try using existing container]; otherwise, <c>false</c>.
        /// </value>
        protected bool TryUsingExistingContainer;

        /// <summary>
        /// Sometimes, UseStartup() calls Startup.UseIoc() which makes a new Container. In that case, we may want to throw away our old one and take that one
        /// </summary>
        protected bool ResetContainerToDependencyResolverOneOnInjectAdditionalDependencies;

        #endregion

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseHttpServerScenario"/> class.
        /// </summary>
        public BaseHttpServerScenario()
        {
            PageDefinitionBuilder = new PageDefinitionBuilder();
        }

        /// <summary>
        /// Anything before the main setup. Abstract.
        /// </summary>
        protected abstract void PreSetup();

        /// <summary>
        /// Anything after the main setup. Abstract. Typically for anything that requires the Container to be ready
        /// </summary>
        protected abstract void PostSetup();

        /// <summary>
        /// Sets up.
        /// </summary>
        [SetUp]
        public void SetUp()
        {
            //child class start setup
            PreSetup();
            
            CreateContainer(TryUsingExistingContainer);
                        
            GetHttpServer();
            var tokenTask = GetToken();
            tokenTask.Wait();
            if(ResetContainerToDependencyResolverOneOnInjectAdditionalDependencies)
                Container = DependencyResolver.Container;
            InjectAdditionalDependencies();
            
            //child class ending setup
            PostSetup();
        }

        /// <example>
        /// <code language="C#" title="Example Usage">
        /// <![CDATA[
        ///
        ///    Action<IAppBuilder> setup = (app) =>
        ///        Startup.UseIdentityClientConfig(app);
        ///        Startup.UseResourceAuthorization(app);
        ///        Startup.UseHttpConfig(app);
        ///        Startup.UseMappings(app);
        ///    return setup;
        ///
        /// ]]>
        /// </code>
        /// </example>
        /// <returns></returns>
        protected abstract Action<IAppBuilder> UseStartup();

        /// <summary>
        /// Gets the HTTP server.
        /// </summary>
        protected virtual void GetHttpServer()
        {
            HttpServer = TestServer.Create(app =>
            {
                DependencyResolver.Container = Container;
                var setup = UseStartup();
                setup(app);
            });

            HttpServer.BaseAddress = new Uri(ConfigurationManager.AppSettings["OwinUrl"]);
        }

        /// <summary>
        /// Gets a token.
        /// </summary>
        /// <returns></returns>
        protected async virtual Task<int> GetToken()
        {
            var authority = ConfigurationManager.AppSettings["authority"].TrimEnd('/');
            var request = new HttpRequestMessage(HttpMethod.Post, authority + "/connect/token")
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    {"scope"        , Scopes }, // "openid all_claims profile email webapi"
                    {"grant_type"   , GrantType },
                    {"client_id"    , GrantType == GrantTypes.Password ? IdentityTestClientKey.Key : BackGroundClientKey.Key },
                    {"client_secret", GrantType == GrantTypes.Password ? IdentityTestClientKey.Value : BackGroundClientKey.Value },
                    {"username"     , UserKey.Key},
                    {"password"     , UserKey.Value }
                })
            };
            
            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);
            TokenResponseContent = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            response.EnsureSuccessStatusCode();
            Token = JObject.Parse(await response.Content.ReadAsStringAsync()).Value<string>("access_token");
            return await Task.Run(() => { return 0; });
        }

        /// <summary>
        /// Teardown method
        /// </summary>
        [TearDown]
        public override void Teardown()
        {
            HttpServer.Dispose();
        }

        protected void OverrideAndInjectUnacceptableScope()
        {
            // wrong scope should return forbidden
            Scopes = "openid";
            GrantType = GrantTypes.Password;
        }

        /// <summary>
        /// Override Scope
        /// </summary>
        /// <param name="scopes"></param>
        protected void OverrideScope(string scopes = "c.r")
        {            
             Scopes = scopes;            
        }
       
   
    }
}
