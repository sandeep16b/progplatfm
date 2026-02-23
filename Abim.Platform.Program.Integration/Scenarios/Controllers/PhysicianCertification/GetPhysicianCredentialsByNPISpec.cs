using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Integration.Scenarios.Controllers.PhysicianCertification.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using NameResource = Abim.Enterprise.Core.Profile.Resource.NameResource;
using ProfileSummaryShortResource = Abim.Enterprise.Core.Profile.Resource.ProfileSummaryShortResource;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the GetPhyCredentialsByNPI Api",
        SoThat = "to get a Physician's Credentials basic information"
        )]
    [TestFixture]
    public class GetPhysicianCredentialsByNPIApiSpec
    {
        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByNPIReturnsOK()
        {
            new GetPhysicianCredentialsByNPIReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByNPIReturnsOnlyABIMCert()
        {
            new GetPhysicianCredentialsByNPIReturnsOnlyABIMCert().BDDfy();
        }

        [TestCase]
        [WorkItem(210151)]
        public void GetPhysicianCredentialsByNPI_DontReturnCoSponsoredCreds()
        {
            new GetPhysicianCredentialsByNPIDontReturnCoSponsoredCreds().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByNPIReturnsNoABIMCert()
        {
            new GetPhysicianCredentialsByNPIReturnsNoABIMCert().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByNPIReturnsNoABIMCertIssuances()
        {
            new GetPhysicianCredentialsByNPIReturnsNoABIMCertIssuances().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByNPI_NoCredentials_ReturnsOk()
        {
            new GetPhysicianCredentialsByNPI_NoCredentials_ReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByNPIReturnsNotFound()
        {
            new GetPhysicianCredentialsByNPIReturnsNotFound().BDDfy();
        }

        [TestCase]
        [WorkItem(222785)]
        public void GetPhysicianCredentialsByNPI_ReturnsNotFound_ForCoSponsoredDiplomate()
        {
            new GetPhysicianCredentialsByNPIReturnsNotFoundForCoSponsoredDiplomate().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetPhyCredentialByNPIScenario : PhysicianCertificationControllerScenario
    {
        protected Mock<ILogger> Log { get; set; }
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(IEnumService));
            list.Add(typeof(ICredentialService));
            list.Add(typeof(IProfileInterservice));
            list.Add(typeof(IAccessTokenService));
            list.Add(typeof(ILogger));
            return list;
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

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPIReturnsOK : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject() };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<PhysicianCertificationsPublicResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPIReturnsOnlyABIMCert : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var abimCredential = ConstructDomainObject();

            CredentialDomainObjects = new List<App.Domain.Credential>() { abimCredential, ConstructDomainObject(RandomString.Build()), ConstructDomainObject(RandomString.Build()) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<PhysicianCertificationsPublicResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceShouldContainOnlyOneABIMCert()
        {
            Resource.Certifications.Count().Should().Equals(1);
        }

        public void AndThenMyResourceShouldContainABIMCertName()
        {
            Resource.Certifications.Where(a => a.Name.StartsWith("ABIM")).Should().Equals(1);
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPIDontReturnCoSponsoredCreds : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {

            CredentialDomainObjects = new List<App.Domain.Credential>() {   ConstructDomainObject(SourceCode: "ABIM", numberOfIssuances: 1, isCosponsored:false), // only this should be return
                                                                            ConstructDomainObject(SourceCode: "ABIM", numberOfIssuances: 1, isCosponsored:true),
                                                                            ConstructDomainObject(RandomString.Build(), numberOfIssuances: 1, isCosponsored:true) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<PhysicianCertificationsPublicResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceShouldContainOnlyOneABIMCert()
        {
            Resource.Certifications.Count().Should().Equals(1); // only one is not CoSponsored ABIM
        }

        public void AndThenMyResourceShouldContainABIMCertName()
        {
            Resource.Certifications.Where(a => a.Name.StartsWith("ABIM")).Should().Equals(1);
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPIReturnsNoABIMCert : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject(), ConstructDomainObject(RandomString.Build()) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<PhysicianCertificationsPublicResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceShouldContainOnlyOneABIMCert()
        {
            Resource.Certifications.Count().Should().Equals(0);
        }

        public void AndThenMyResourceShouldContainABIMCertName()
        {
            Resource.Certifications.Where(a => a.Name.StartsWith("ABIM")).Should().Equals(0);
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPIReturnsNoABIMCertIssuances : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            // has ABIM cert bu no issuances
            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject(SourceCode: "ABIM", numberOfIssuances: 0), ConstructDomainObject(RandomString.Build()), ConstructDomainObject(RandomString.Build()) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<PhysicianCertificationsPublicResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceShouldContainOnlyOneABIMCert()
        {
            Resource.Certifications.Count().Should().Equals(0);
        }

        public void AndThenMyResourceShouldContainABIMCertName()
        {
            Resource.Certifications.Where(a => a.Name.StartsWith("ABIM")).Should().Equals(0);
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPI_NoCredentials_ReturnsOk : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            CredentialDomainObjects = new List<App.Domain.Credential>() { };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<PhysicianCertificationsPublicResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByNPIReturnsNotFound : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            CredentialDomainObjects = new List<App.Domain.Credential>() { };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            var ex = new Enterprise.Core.Profile.Interservice.Util.Extensions.UnsuccessfulStatusException("");
            ex.StatusCode = HttpStatusCode.NotFound;

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .Throws(ex);

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void AndThenMyResponseContentShouldContainText()
        {
            ResponseContent.Should().Contain("No user exists with npi:");
        }

    }

    /// <summary>
    /// GetPhysicianCredentialsByNPIReturnsNotFoundForCoSponsoredDiplomate
    /// </summary>
    public class GetPhysicianCredentialsByNPIReturnsNotFoundForCoSponsoredDiplomate : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {

            CredentialDomainObjects = new List<App.Domain.Credential>() {   ConstructDomainObject(SourceCode: "ABIM", numberOfIssuances: 1, isCosponsored:true),
                                                                            ConstructDomainObject(SourceCode: "ABIM", numberOfIssuances: 0, isCosponsored:true),
                                                                            ConstructDomainObject(RandomString.Build(), numberOfIssuances: 1, isCosponsored:true) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/NPI/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetResultContentShouldBeNoUserExists()
        {
            ResponseContent.Should().Contain("No user exists with npi:'12345'.");
        }
    }
    #endregion
}
