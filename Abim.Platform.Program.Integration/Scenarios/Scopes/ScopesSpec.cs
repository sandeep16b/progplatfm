using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands; 
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Host.Config;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Api.Constants;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Testing.Setup; 
using FluentAssertions;
using Hangfire;
using MassTransit;
using Moq;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using Owin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Scopes
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call any Api with proper scopes",
        SoThat = "to get correct resource authorization result"
        )]
    [TestFixture]
    public class ScopesSpec
    {
        [TestCase]
       
        public void ProperViewScopesReturnsOK()
        {
            new ProperViewScopesReturnsOK().BDDfy();
        }

        [TestCase]
        public void ProperUpdateScopesReturnsOK()
        {
            new ProperUpdateScopesReturnsOK().BDDfy();
        }

        [TestCase]
        public void NoProperUpdateScopesReturnsForbidden()
        {
            new NoProperUpdateScopesReturnsForbidden().BDDfy();
        }

        [TestCase]
        public void LegacyWebApiForViewScopeReturnsOK()
        {
            new LegacyWebApiForViewScopeReturnsOK().BDDfy();
        }

        [TestCase]
        public void NoProperViewScopesReturnsForbidden()
        {
            new NoProperViewScopesReturnsForbidden().BDDfy();
        }

        [TestCase]
        public void NoProperViewScopesButAdminReturnsOk()
        {
            new NoProperViewScopesButAdminReturnsOk().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class BaseScopesScenario : BaseHttpServerScenario
    {

        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ICredentialService));
            list.Add(typeof(ICredentialRepository));
            list.Add(typeof(IBusControl));
            list.Add(typeof(IBackgroundJobClient));
            list.Add(typeof(IValidationFactory));
            list.Add(typeof(ISourceService));
            list.Add(typeof(ICredentialService));
            list.Add(typeof(IHelperService));
            list.Add(typeof(IMembershipClientService));
            return list;
        }

        protected override Action<IAppBuilder> UseStartup()
        {
            Action<IAppBuilder> action = (app) =>
            {
                Startup.Container = DependencyResolver.Container;
                Startup.UseIdentityClientConfig(app);
                Startup.UseResourceAuthorization(app);
                Startup.UseHttpConfig(app);
                Startup.UseMappings(); 
                Container.Inject(My<IAccessTokenService>().Object);
                Container.Inject(My<IEnumService>().Object); 
                Container.Inject(My<ILogger>().Object);
                Container.Inject(My<ICertificationService>().Object);
                Container.Inject(My<ISourceService>().Object);
                Container.Inject(My<IProgramRulesService>().Object);
                Container.Inject(My<IBackgroundJobClient>().Object); 
                Container.Inject(My<IMembershipClientService>().Object);
            };
            return action;
        }

        protected App.Domain.Credential ConstructDomainObject()
        {
            var source = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var certification = App.Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                RandomString.Build(), RandomString.Build(), RandomString.Build());
            var credential = App.Domain.Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            return credential;
        }
    }

    #region Scenarios

    public class ProperViewScopesReturnsOK : BaseScopesScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
            Scopes = "c.r";
            GrantType = GrantTypes.Client_credentials;
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallApi()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

    }

    public class ProperUpdateScopesReturnsOK : BaseScopesScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
            Scopes = "c.r c.w";
            GrantType = GrantTypes.Client_credentials;
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);

            My<ICredentialService>()
            .Setup(o => o.Handle(It.IsAny<AddCredentialCommand>()))
            .ReturnsAsync(new AddCredentialCommandResult() { Status = CommandStatus.Accepted, Data= DomainObject });
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new AddCredentialCommand()
            {
                MemberId= new Guid(),
                CertificationId= new Guid(),
                Pathway = PathwayType.MOC,
                Type = CredentialType.General,

                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                }
            };
            Url = $"/api/v1.0/credential";
        }

        public async Task WhenICallApi()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(AddCredentialCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();

            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

    }

    public class NoProperUpdateScopesReturnsForbidden : BaseScopesScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);

            My<ICredentialService>()
            .Setup(o => o.Handle(It.IsAny<AddCredentialCommand>()))
            .ReturnsAsync(new AddCredentialCommandResult() { Status = CommandStatus.Accepted, Data = DomainObject });
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new AddCredentialCommand()
            {
                MemberId = new Guid(),
                CertificationId = new Guid(),
                Pathway = PathwayType.MOC,
                Type = CredentialType.General,

                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                }
            };
            Url = $"/api/v1.0/credential";
        }

        public async Task WhenICallApi()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(AddCredentialCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();

            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

    }

    public class LegacyWebApiForViewScopeReturnsOK : BaseScopesScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
            Scopes = "c.r";
            GrantType = GrantTypes.Client_credentials;
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallApi()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

    }

    public class NoProperViewScopesReturnsForbidden : BaseScopesScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
            //   New Identity will return invalid scope if we pass the wrong scope, not Forbidden,
            //   and change GrandType to Password to create a token, which will return forbidden.
            OverrideScope("openid");
            GrantType = GrantTypes.Password;
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallApi()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

        public void AndThenMyResourceShouldOfTypeCredentialResource()
        {
            Resource.Should().BeOfType<CredentialResource>();
        }

    }

    public class NoProperViewScopesButAdminReturnsOk : BaseScopesScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
            OverrideScope("u.r"); 
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallApi()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

    }
    #endregion
}
