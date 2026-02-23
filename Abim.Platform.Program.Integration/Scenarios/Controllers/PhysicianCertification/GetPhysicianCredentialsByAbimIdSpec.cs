using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Integration.Scenarios.Controllers.PhysicianCertification.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
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
using ProfileSummaryShortResource = Abim.Enterprise.Core.Profile.Resource.ProfileSummaryShortResource;
using NameResource = Abim.Enterprise.Core.Profile.Resource.NameResource;
using static Abim.Platform.Program.Resources.ProgramResourceConstants;


namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the GetPhyCredentialsByAbimId Api",
        SoThat = "to get a Physician's Credentials basic information"
        )]
    [TestFixture]
    public class GetPhysicianCredentialsByAbimIdApiSpec
    {
        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByAbimIdReturnsOK()
        {
            new GetPhysicianCredentialsByAbimIdReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByAbimIdReturnsOnlyABIMCert()
        {
            new GetPhysicianCredentialsByAbimIdReturnsOnlyABIMCert().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByAbimIdReturnsNoABIMCert()
        {
            new GetPhysicianCredentialsByAbimIdReturnsNoABIMCert().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByAbimIdReturnsNoABIMCertIssuances()
        {
            new GetPhysicianCredentialsByAbimIdReturnsNoABIMCertIssuances().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByAbimId_NoCredentials_ReturnsOk()
        {
            new GetPhysicianCredentialsByAbimId_NoCredentials_ReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(210151)]
        public void GetPhysicianCredentialsByAbimId_DontReturnCoSponsoredCreds()
        {
            new GetPhysicianCredentialsByAbimIdDontReturnsCoSponsoredCreds().BDDfy();
        }

        [TestCase]
        [WorkItem(222785)]
        public void GetPhysicianCredentialsByAbimId_ReturnsNotFound_ForCoSponsoredDiplomate()
        {
            new GetPhysicianCredentialsByAbimIdReturnsNotFoundForCoSponsoredDiplomate().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void GetPhysicianCredentialsByAbimIdReturnsNotFound()
        {
            new GetPhysicianCredentialsByAbimIdReturnsNotFound().BDDfy();
        }

        //******  To test cases in PBI 170410 : VOC Page updates - ABIM.org ********
        [TestCase]
        [WorkItem(170410)]
        public void GetPhysicianCredentialsByAbimId_FPHMSelectedToBeMaintained_ReturnIsFocusPractiseTrue()
        {
            new GetPhysicianCredentialsByAbimId_FPHMSelectedToBeMaintained_ReturnIsFocusPractiseTrueSpec().BDDfy();
        }

        [TestCase]
        [WorkItem(170410)]
        public void GetPhysicianCredentialsByAbimId_FPHMHasNOTSelectedToBeMaintained_ReturnIsFocusPractiseFalse()
        {
            new GetPhysicianCredentialsByAbimId_FPHMHasNOTSelectedToBeMaintained_ReturnIsFocusPractiseFalseSpec().BDDfy();
        }

        [TestCase]
        [WorkItem(170410)]
        public void GetPhysicianCredentialsByAbimId_IMSelectedToBeMaintainedAndNoFPHMExists_ReturnIsFocusPractiseFalse()
        {
            new GetPhysicianCredentialsByAbimId_IMSelectedToBeMaintainedAndNoFPHMExists_ReturnIsFocusPractiseFalseSpec().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetPhyCredentialByAbimIdScenario : PhysicianCertificationControllerScenario
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

        protected App.Domain.Credential ConstructDomainObject(string SourceCode = "ABIM", int numberOfIssuances = 1, bool isCosponsored = false)
        {
            var source = App.Domain.Source.Create(RandomString.Build(), SourceCode, RandomString.Build());
            var certification = App.Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                name : ( SourceCode == "ABIM" ? "ABIM" + RandomString.Build() : RandomString.Build() ),
                code : RandomString.Build(),
                createdBy:  RandomString.Build());
            var credential = App.Domain.Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

            credential.IsCosponsored = isCosponsored;

            foreach (var i in Enumerable.Range(0, numberOfIssuances))
                credential.AddIssuance(App.Domain.Issuance.Create(RandomString.Build()));

            return credential;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByAbimIdReturnsOK : GetPhyCredentialByAbimIdScenario
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
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
    public class GetPhysicianCredentialsByAbimIdReturnsOnlyABIMCert : GetPhyCredentialByAbimIdScenario
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

            CredentialDomainObjects = new List<App.Domain.Credential>() { abimCredential, ConstructDomainObject(RandomString.Build()) , ConstructDomainObject(RandomString.Build()) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
            Resource.Certifications.Where(a=>a.Name.StartsWith("ABIM")).Should().Equals(1);
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
  
    }


    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByAbimIdReturnsNoABIMCert : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {

            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject(SourceCode: "ABIM", numberOfIssuances: 0) , ConstructDomainObject(RandomString.Build()) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
    public class GetPhysicianCredentialsByAbimIdReturnsNoABIMCertIssuances : GetPhyCredentialByAbimIdScenario
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
            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject(SourceCode : "ABIM", numberOfIssuances: 0) , ConstructDomainObject(RandomString.Build()), ConstructDomainObject(RandomString.Build()) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
    /// GetPhysicianCredentialsByAbimIdReturnsNoCoSponsoredCreds
    /// </summary>
    public class GetPhysicianCredentialsByAbimIdDontReturnsCoSponsoredCreds : GetPhyCredentialByAbimIdScenario
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
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
            Resource.Certifications.Where(a => a.Name.StartsWith("ABIM")).Should().Equals(0);
        }

        public void AndThenMyResourceLastNameShouldNotBeNull()
        {
            Resource.LastName.Should().NotBeNull();
        }
    }

    /// <summary>
    /// GetPhysicianCredentialsByAbimIdReturnsNotFoundForCoSponsoredDiplomate
    /// </summary>
    public class GetPhysicianCredentialsByAbimIdReturnsNotFoundForCoSponsoredDiplomate : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {

            CredentialDomainObjects = new List<App.Domain.Credential>() {   ConstructDomainObject("ABIM", numberOfIssuances: 1, isCosponsored:true), 
                                                                            ConstructDomainObject(RandomString.Build(), numberOfIssuances: 0, isCosponsored:true),
                                                                            ConstructDomainObject(RandomString.Build(), numberOfIssuances: 1, isCosponsored:true) };

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "CoSponsored" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetResultContentShouldBeNoUserExists()
        {
            ResponseContent.Should().Contain("No user exists with AbimId:'12345'.");
        }

    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetPhysicianCredentialsByAbimId_NoCredentials_ReturnsOk : GetPhyCredentialByAbimIdScenario
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
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
    public class GetPhysicianCredentialsByAbimIdReturnsNotFound : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            CredentialDomainObjects = new List<App.Domain.Credential>() {};

            ProfileSummaryShortResource profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            var ex = new Enterprise.Core.Profile.Interservice.Util.Extensions.UnsuccessfulStatusException("");
            ex.StatusCode = HttpStatusCode.NotFound;

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
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
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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
            ResponseContent.Should().Contain("No user exists with AbimId:");
        }

    }

    /// <summary>
    /// The FPHM is SelectedToBeMaintained and Return IsFocusPractise =True
    /// </summary>
    public class GetPhysicianCredentialsByAbimId_FPHMSelectedToBeMaintained_ReturnIsFocusPractiseTrueSpec : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> Credentials { get; set; }

        ProfileSummaryShortResource profileSummaryShortResource { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            //****  Credentials *****
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

            var credIM = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
            // IM SelectedToMaintain = false
            credIM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2010, 11, 02),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credIM.SelectedToMaintain = false;

            // FPHM SelectedToMaintain = true
            var credFPHM = CredentialBuilder.BuildWithoutRandoms(source, "HOSP", "Focused Practice in Hospital Medicine", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
            credFPHM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2015, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credFPHM.SelectedToMaintain = true;

            // GERI SelectedToMaintain = true
            var credGERI = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
            credGERI.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2018, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credGERI.SelectedToMaintain = true;

            Credentials = new List<App.Domain.Credential>() { credIM, credFPHM, credGERI };

        }

        protected override void PostSetup()
        {
            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(Credentials));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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

        public void AndThenMyResourceShouldContainThreeABIMCerts()
        {
            Resource.Certifications.Count().Should().Equals(3);
        }

        public void AndThenMyResourceShouldContainProperNames()
        {
            Resource.Certifications.Where(c => c.Name == CertificationName.IM).Any();
            Resource.Certifications.Where(c => c.Name == CertificationName.IMwithFPHM).Any();
            Resource.Certifications.Where(c => c.Name == "Geriatric Medicine").Any();
        }

        public void AndThenMyResourceIsFocusPracticeShouldBeTrue()
        {
            Resource.IsFocusPractice.Should().BeTrue();
        }

    }

    /// <summary>
    /// The FPHM is NOT SelectedToBeMaintained and Return IsFocusPractise = False
    /// </summary>
    public class GetPhysicianCredentialsByAbimId_FPHMHasNOTSelectedToBeMaintained_ReturnIsFocusPractiseFalseSpec : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> Credentials { get; set; }

        ProfileSummaryShortResource profileSummaryShortResource { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            //****  Credentials *****
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

            var credIM = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
            // IM SelectedToMaintain = true
            credIM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2010, 11, 02),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credIM.SelectedToMaintain = true;

            // FPHM SelectedToMaintain = false
            var credFPHM = CredentialBuilder.BuildWithoutRandoms(source, "HOSP", "Focused Practice in Hospital Medicine", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
            credFPHM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2015, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credFPHM.SelectedToMaintain = false;

            // GERI SelectedToMaintain = true
            var credGERI = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
            credGERI.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2018, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credGERI.SelectedToMaintain = true;

            Credentials = new List<App.Domain.Credential>() { credIM, credFPHM, credGERI };

        }

        protected override void PostSetup()
        {
            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(Credentials));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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

        public void AndThenMyResourceShouldContainThreeABIMCerts()
        {
            Resource.Certifications.Count().Should().Equals(3);
        }

        public void AndThenMyResourceShouldContainProperNames()
        {
            Resource.Certifications.Where(c=>c.Name==CertificationName.IM).Any();
            Resource.Certifications.Where(c => c.Name == CertificationName.IMwithFPHM).Any();
            Resource.Certifications.Where(c => c.Name == "Geriatric Medicine").Any();
        }

        public void AndThenMyResourceIsFocusPracticeShouldBeFalse()
        {
            Resource.IsFocusPractice.Should().BeFalse();
        }

    }

    /// <summary>
    /// The IM is SelectedToBeMaintained and NO FPHM exists Return IsFocusPractise = False
    /// </summary>
    public class GetPhysicianCredentialsByAbimId_IMSelectedToBeMaintainedAndNoFPHMExists_ReturnIsFocusPractiseFalseSpec : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> Credentials { get; set; }

        ProfileSummaryShortResource profileSummaryShortResource { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profileSummaryShortResource = new ProfileSummaryShortResource()
            {
                AbimId = "123",
                Name = new NameResource() { FirstName = "Alex", LastName = "Reznit" },
                NameAliases = new List<ProfileNameAliasSummaryResource>()
            };

            //****  Credentials *****
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

            var credIM = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
            // IM SelectedToMaintain = true
            credIM.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2010, 11, 02),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credIM.SelectedToMaintain = true;

            // GERI SelectedToMaintain = true
            var credGERI = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
            credGERI.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2018, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            credGERI.SelectedToMaintain = true;

            Credentials = new List<App.Domain.Credential>() { credIM, credGERI };

        }

        protected override void PostSetup()
        {
            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByAbimId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profileSummaryShortResource));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(Credentials));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/physicianCredentials/abimId/" + "12345";
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

        public void AndThenMyResourceShouldContainThreeABIMCerts()
        {
            Resource.Certifications.Count().Should().Equals(3);
        }

        public void AndThenMyResourceShouldContainProperNames()
        {
            Resource.Certifications.Where(c => c.Name == CertificationName.IM).Any();
            Resource.Certifications.Where(c => c.Name == "Geriatric Medicine").Any();
        }

        public void AndThenMyResourceIsFocusPracticeShouldBeFalse()
        {
            Resource.IsFocusPractice.Should().BeFalse();
        }

    }
    #endregion
}
