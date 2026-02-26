using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential Api",
        SoThat = "to get a single Credential's details"
        )]
    [TestFixture]
    public class GetCredentialByIdApiSpec
    {
        [TestCase]
        [WorkItem(74773)]
        
        public void GetCredentialReturnsOK()
        {
            new GetCredentialReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(74773)]

        public void GetCredentialReturnsForbidden()
        {
            new GetCredentialReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(74773)]
        public void GetCredentialInvalidIdReturnsNotFound()
        {
            new GetCredentialInvalidIdReturnsNotFound().BDDfy();
        }

        [TestCase]
        [WorkItem(74773)]
        public void GetCredentialNonGuidIdReturnsProperMessage()
        {
            new GetCredentialNonGuidIdReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(74773)]
        public void GetCredentialAnonymousReturnsUnauthorized()
        {
            new GetCredentialAnonymousReturnsUnauthorized().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetCredentialByIdScenario : CredentialControllerScenario
    {

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

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetCredentialReturnsOK : GetCredentialByIdScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }
       
        protected override void PreSetup()
        { 
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

        public async Task WhenICallGetCredential()
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

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "certification");
        }
    }

    public class GetCredentialReturnsForbidden : GetCredentialByIdScenario
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
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCredential()
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

    }

    /// <summary>
    /// The Invalid Id scenario
    /// </summary>
    public class GetCredentialInvalidIdReturnsNotFound : GetCredentialByIdScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns<App.Domain.Credential>(null);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetANotFoundResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    /// <summary>
    /// The non-Guid Id scenario
    /// </summary>
    public class GetCredentialNonGuidIdReturnsProperMessage : GetCredentialByIdScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassAnInvalidUrl()
        {
            Url = "/api/v1.0/Credential/" + (new Random()).Next(0, int.MaxValue);
        }

        public async Task WhenICallGetCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetAnErrorStatusCode()
        {
            Result.StatusCode.Should().NotBe(HttpStatusCode.OK);
        }

        public void AndThenIGetAProperErrorMessage()
        {
            ResponseContent.Should().Contain(ErrorMessages.InvalidGuidId);
        }
    }
    
    /// <summary>
    /// The anonymous call scenario
    /// </summary>
    public class GetCredentialAnonymousReturnsUnauthorized : GetCredentialByIdScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
        }
        
        public void GivenIGoToTheUrlWithNoToken()
        {
            Url = "/api/v1.0/Credential/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCredential()
        {
            Result = await HttpServer.CreateRequest(Url).GetAsync();
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
