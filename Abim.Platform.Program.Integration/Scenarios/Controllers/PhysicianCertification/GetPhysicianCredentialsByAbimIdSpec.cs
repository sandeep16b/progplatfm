using Abim.Platform.Program.MembershipClient;
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

        [TestCase]
        [WorkItem(265918)]
        public void GetPhysicianCredentialsByAbimId_ReturnsIssuanceStatusWithModifier()
        {
            new GetPhysicianCredentialsByAbimId_ReturnsIssuanceStatusWithModifierSpec().BDDfy();
        }

        [TestCase]
        [WorkItem(265918)]
        public void GetPhysicianCredentialsByAbimId_DoesntReturnInActiveFPHM()
        {
            new GetPhysicianCredentialsByAbimId_DoesntReturnInActiveFPHMSpec().BDDfy();
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

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())
            }; 

            My<IMembershipClientService>()
               .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
               .Returns(Task.FromResult(profileResource)); 

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
             
            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
            };

            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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

            ProfileResource profileResource = new ProfileResource()
            {
                Id = Guid.NewGuid(),
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())
            };

            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())
            };

            My<IMembershipClientService>()
                 .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                 .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
            };

            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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

            CredentialDomainObjects = new List<App.Domain.Credential>() {   ConstructDomainObject( numberOfIssuances: 1, isCosponsored:true), 
                                                                            ConstructDomainObject(RandomString.Build(), numberOfIssuances: 0, isCosponsored:true),
                                                                            ConstructDomainObject(RandomString.Build(), numberOfIssuances: 1, isCosponsored:true) };

            ProfileResource profileResource = new ProfileResource()
            {
                Id = Guid.NewGuid(),
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "CoSponsored" },
                Aliases = new List<ProfileAliasResource>()
            };

            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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
            CredentialDomainObjects = new List<App.Domain.Credential>();

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
            };

            My<IMembershipClientService>()
                 .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                 .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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
            CredentialDomainObjects = new List<App.Domain.Credential>();

            ProfileResource profileSummaryShortResource = new ProfileResource()
            {
                Id = Guid.NewGuid(),
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
            };

            var ex = new ApiException("Not Found", 404, "", null, null); 

            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                 .Throws(ex);

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects);

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

        ProfileResource profileResource { get; set; }  

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
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
            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profileResource); 

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Credentials);

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

        ProfileResource profilResource { get; set; }  

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profilResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
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
            My<IMembershipClientService>()
               .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
               .ReturnsAsync(profilResource);

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Credentials);

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

        ProfileResource profileResource { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
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
            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                 .ReturnsAsync(profileResource);

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(Credentials);

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


    /// <summary>
    /// Returns IssuanceStatusWithModifier
    /// </summary>
    public class GetPhysicianCredentialsByAbimId_ReturnsIssuanceStatusWithModifierSpec : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> Credentials { get; set; }

        ProfileResource profilResource { get; set; }


        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profilResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = new List<ProfileAliasResource>()
            };

            //****  Credentials *****
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

            //Issuance Status: Active
            var credActive = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);

            credActive.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2010, 11, 02),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));

            //Issuance Status: InActive
            var credInActive = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
            credInActive.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Inactive,
                                                                    issuanceDate: new DateTime(2015, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));


            //Issuance Status: Surrendered
            var credSurrendered = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
            credSurrendered.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Surrendered,
                                                                    issuanceDate: new DateTime(2018, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));

            

            //Issuance Status: Expired

            var credExpired = CredentialBuilder.BuildWithoutRandoms(source, "NCARD", "Nuclear Cardiology", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
            credExpired.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Expired,
                                                                    issuanceDate: new DateTime(2015, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));


            //Issuance Status: Suspended
            var credSuspended = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
            credSuspended.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Suspended,
                                                                    issuanceDate: new DateTime(2018, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));


            //Issuance Status: Revoked
            var credRevoked = CredentialBuilder.BuildWithoutRandoms(source, "GERI", "Geriatric Medicine", CertificationType.Subspecialty, CredentialType.Subspecialty, PathwayType.MOC);
            credRevoked.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Revoked,
                                                                    issuanceDate: new DateTime(2018, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));
            //Issuance Status: Cancelled

            var credCancelled = CredentialBuilder.BuildWithoutRandoms(source, "HEMA", "Hematology", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
            credCancelled.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Cancelled,
                                                                    issuanceDate: new DateTime(2015, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));

            Credentials = new List<App.Domain.Credential>() { credActive, credInActive, credSurrendered, credExpired, credSuspended, credRevoked, credCancelled };

        }

        protected override void PostSetup()
        {
            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(profilResource));


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
         

        public void AndThenMyResourceShouldContainProperStatus()
        {
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Active).First().IssuanceStatusWithModifier.Should().Be("Certified");
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Inactive).First().IssuanceStatusWithModifier.Should().Be("Not Certified");
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Surrendered).First().IssuanceStatusWithModifier.Should().Be("Not Certified");
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Suspended).First().IssuanceStatusWithModifier.Should().Be("Not Certified, Suspended");
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Revoked).First().IssuanceStatusWithModifier.Should().Be("Not Certified, Revoked");
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Expired).First().IssuanceStatusWithModifier.Should().Be("Not Certified, Lapsed");
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Cancelled).First().IssuanceStatusWithModifier.Should().Be("Not Certified");

        }
    }

    /// <summary>
    /// Returns IssuanceStatusWithModifier
    /// </summary>
    public class GetPhysicianCredentialsByAbimId_DoesntReturnInActiveFPHMSpec : GetPhyCredentialByAbimIdScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> Credentials { get; set; }

        ProfileResource profileResource { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();

            profileResource = new ProfileResource()
            {
                AbimId = "123",

                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())

            };

            //****  Credentials *****
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

            //Issuance Status: Active
            var credActive = CredentialBuilder.BuildWithoutRandoms(source, "HOSP", "Focused Practice in Hospital Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);

            credActive.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Active,
                                                                    issuanceDate: new DateTime(2010, 11, 02),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));

            //Issuance Status: InActive
            var credInActive = CredentialBuilder.BuildWithoutRandoms(source, "HOSP", "Focused Practice in Hospital Medicine", CertificationType.FocusPractice, CredentialType.Subspecialty, PathwayType.MOC);
            credInActive.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source: source,
                                                                    issuanceStatus: IssuanceStatusType.Inactive,
                                                                    issuanceDate: new DateTime(2015, 11, 4),
                                                                    durationType: DurationType.Continuous,
                                                                    maintenanceRequirement: MaintenanceRequirementType.Required,
                                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                                    occurrenceType: OccurrenceType.Initial));


          
            Credentials = new List<App.Domain.Credential>() { credActive, credInActive };

        }

        protected override void PostSetup()
        {
            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(profileResource));

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


        public void AndThenMyResourceShouldContainActiveStatusForFPHM()
        {
            Resource.Certifications.Where(c => c.Status == IssuanceStatusType.Active).First().Name.Should().Be(CertificationName.IMwithFPHM);
            
        }

        public void AndThenMyResourceShouldNotContainInActiveStatusForFPHM()
        {
            Resource.Certifications.Where(c => c.Status != IssuanceStatusType.Active).FirstOrDefault().Should().BeNull();

        }
    }

    #endregion
}
