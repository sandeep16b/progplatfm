using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.OptionsResources;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.WebApi.Objects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential api",
        SoThat = "to get a list of the available options for Credentials."
        )]
    [TestFixture]
    public class GetCredentialApiOptionsApiSpec
    {
        [TestCase]
        [WorkItem(78409)]
        public void GetCredentialOptionsReturnsOk()
        {
            new GetCredentialOptionsReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(78409)]
        public void GetCredentialOptionsReturnsForbidden()
        {
            new GetCredentialOptionsReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78409)]
        public void GetCredentialOptionsAnonymousReturnsUnauthorized()
        {
            new GetCredentialOptionsAnonymousReturnsUnauthorized().BDDfy();
        }

        [TestCase]
        [WorkItem(95544)]
        public void GetCredentialIssuanceOptionsReturnsOk()
        {
            new GetCredentialIssuanceOptionsReturnsOk().BDDfy();
    }

        [TestCase]
        [WorkItem(95544)]
        public void GetCredentialIssuanceOptionsAnonymousReturnsUnauthorized()
        {
            new GetCredentialIssuanceOptionsAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
    public abstract class CredentialOptionsScenario : CredentialControllerScenario
    {
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetCredentialOptionsReturnsOk :
        CredentialOptionsScenario
    {
        IEnumService EnumService { get; set; }
        CredentialOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credentials";
        }

        public async Task WhenICallGetCredentialsOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceCollectionShouldHaveASelfLink()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenMyResourceCollectionShouldHaveAllTheEnumLinks()
        {
            var expectedEnumTypes = EnumService.GetEnumTypesFor(typeof(App.Domain.Credential), EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            foreach (var type in expectedEnumTypes)
            {
                Resource.Links.Should().Contain(link => link.Method.ToUpper() == "GET" && link.Name.ToLower().Contains(type.Name.ToLower()));
            }

            var allEnumLinks = Resource.Links.Where(link => link.Href.ToLower().Contains("/enum"));
            allEnumLinks.Count().ShouldBeEquivalentTo(expectedEnumTypes.Count);
        }

        public void AndThenMyResourceCollectionShouldHaveRootFunctionalityLinks()
        {
            Resource.Links.Should().Contain(link => link.Href.ToLower().Contains("/credentials") && link.Method == "GET");
            Resource.Links.Should().Contain(link => link.Href.ToLower().Contains("/credentials") && link.Method == "POST");
        }
    }

    public class GetCredentialOptionsReturnsForbidden :
    CredentialOptionsScenario
    {
        IEnumService EnumService { get; set; }
        CredentialOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
            OverrideScope();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credentials";
        }

        public async Task WhenICallGetCredentialsOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The Unauthorized scenario
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.CredentialOptionsScenario"/>
    public class GetCredentialIssuanceOptionsAnonymousReturnsUnauthorized
        : CredentialOptionsScenario
    {
        IEnumService EnumService { get; set; }
        IssuanceOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/credentials/issuances";
        }

        public async Task WhenICallGetCredentialIssuanceOptionsWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<IssuanceOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    /// <summary>
    /// The Unauthorized scenario
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.CredentialOptionsScenario"/>
    public class GetCredentialOptionsAnonymousReturnsUnauthorized
        : CredentialOptionsScenario
    {
        IEnumService EnumService { get; set; }
        CredentialOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/credentials/issuances";
        }

        public async Task WhenICallGetCredentialOptionsWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }
    public class GetCredentialIssuanceOptionsReturnsOk :
        CredentialOptionsScenario
    {
        IEnumService EnumService { get; set; }
        IssuanceOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/credentials/issuances";
        }

        public async Task WhenICallGetCredentialsOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<IssuanceOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
    }

        public void AndThenMyResourceCollectionShouldHaveASelfLink()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenMyResourceCollectionShouldHaveAllTheEnumLinks()
        {
            var expectedEnumTypes = EnumService.GetEnumTypesFor(typeof(App.Domain.Issuance), EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            foreach (var type in expectedEnumTypes)
            {
                Resource.Links.Should().Contain(link => link.Method.ToUpper() == "GET" && link.Name.ToLower().Contains(type.Name.ToLower()));
            }

            var allEnumLinks = Resource.Links.Where(link => link.Href.ToLower().Contains("/enum"));
            allEnumLinks.Count().ShouldBeEquivalentTo(expectedEnumTypes.Count);
        }
    }
    #endregion
}
