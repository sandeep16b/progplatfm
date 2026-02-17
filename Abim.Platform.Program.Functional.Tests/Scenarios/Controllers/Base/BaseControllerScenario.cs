using Abim.Platform.Program.Tests.Setup.OtherBuilders;
using Microsoft.Owin;
using System;
using System.Net.Http;
using System.Security.Claims;

namespace Abim.Platform.Program.Tests.Scenarios.Controllers.Base
{
    public abstract class BaseControllerScenario
    {
        protected HttpRequestMessage _request;
        protected Exception _caughtException;

        protected virtual void Setup()
        {
            SetupConfig();
            SetupHttpRequest();
            //SetupAutoMapper();
        }

        protected virtual void SetupConfig()
        {
            System.Configuration.Abstractions.ConfigurationManager.Instance.AppSettings["ProgramHostUrl"] = "https://someprogramendpoint/";
        }

        /*
        protected virtual void SetupAutoMapper()
        {
            Mapper.Initialize(cfg =>
            {
                AddMappings(cfg);
            });
        }
        */

        protected virtual void TearDown()
        {
            if (_request != null)
                _request.Dispose();
        }

        /*
        /// <summary>
        /// Adds the mappings.
        /// </summary>
        /// <param name="cfg">The CFG.</param>
        protected void AddMappings(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<MediaTypeMapping>();
            cfg.AddProfile<ActivityCreditMapping>();
            cfg.AddProfile<ActivityMapping>();
            cfg.AddProfile<ProductCreditMapping>();
            cfg.AddProfile<ProductGroupMapping>();
            cfg.AddProfile<ProductMapping>();
            cfg.AddProfile<ProviderMapping>();
            cfg.AddProfile<SponsorMapping>();
            cfg.AddProfile<CreditTypeMapping>();
            cfg.AddProfile<DeliverySystemMapping>();
            cfg.AddProfile<ProductFormatMapping>();
        }
        */

        protected virtual void SetupHttpRequest()
        {
            var builder = new HttpRequestMessageBuilder();
            _request =
                builder
                    .WithAuthorizationHeader("my fake token")
                    .WithOwinContext(GetOwinContextMock())
                    .Build();
            _request.RequestUri = new Uri("https://someurl");
        }

        protected virtual IOwinContext GetOwinContextMock()
        {
            var builder = new IOwinContextMockBuilder();
            var claimsPrincipal = new ClaimsPrincipal();
            var claimsIdentity = new ClaimsIdentity();
            
            //Add Profile ID claim
            claimsIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, Guid.NewGuid().ToString()));

            //Add ABIM ID claim
            claimsIdentity.AddClaim(new Claim("http://schemas.abim.org/2016/identifier/abim", "123456"));
            claimsPrincipal.AddIdentity(claimsIdentity);

            var context = builder.WithClaimsPrincipal(claimsPrincipal).Build().Object;
            IOwinContext output = context;
            return output;
        }
    }

}
