using Abim.Platform.Program.MembershipClient;
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

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the SearchProfilesByNameAndDob Api",
        SoThat = "to get a Physician's Credentials basic information"
        )]
    [TestFixture]
    public class SearchProfilesByNameAndDobApiSpec
    {
        [TestCase]
        [WorkItem(140373)]
        [WorkItem(296934)]
        public void SearchProfilesByNameAndDob_MultipleRecords_ReturnsOK()
        {
            new SearchProfilesByNameAndDob_MultipleRecords_ReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void SearchProfilesByNameAndDob_MultipleRecordsCheckNameAlias_ReturnsOK()
        {
            new SearchProfilesByNameAndDob_MultipleRecordsCheckNameAlias_ReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void SearchProfilesByNameAndDob_OneRecord_ReturnsOK()
        {
            new SearchProfilesByNameAndDob_OneRecord_ReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(140373)]
        public void SearchProfilesByNameAndDobReturnsNotFound()
        {
            new SearchProfilesByNameAndDobReturnsNotFound().BDDfy();
        }

        [TestCase]
        [WorkItem(222785)]
        public void SearchProfilesByNameAndDob_ReturnsNotFound_ForCoSponsoredDiplomate()
        {
            new SearchProfilesByNameAndDob_NotFoundForCoSponsoredDiplomate().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class SearchProfilesByNameAndDobScenario : PhysicianCertificationControllerScenario
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
                name: (SourceCode == "ABIM" ? "ABIM" + RandomString.Build() : RandomString.Build()),
                code: RandomString.Build(),
                createdBy: RandomString.Build());
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
    public class SearchProfilesByNameAndDob_MultipleRecords_ReturnsOK : SearchProfilesByNameAndDobScenario
    {
        ProfileShortCollectionResourcePublic Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var profileShortCollection = new List<VocProfileResource>() {
                new VocProfileResource() {
                   PublicId =Guid.NewGuid(),
                   Id    = (new Random()).Next(1, int.MaxValue),
                   AbimId =  (new Random()).Next(1, int.MaxValue).ToString(),
                   NpiNumber =  (new Random()).Next(1, int.MaxValue).ToString(),
                   Credential  =  true,
                   Deceased   =  false,
                   FirstName    =  RandomString.BuildWithLength(255),
                   LastName     =  RandomString.BuildWithLength(255),
                   MiddleName   =  RandomString.BuildWithLength(255),
                   Suffix  =  VocProfileResourceSuffix.DC,
                   FirstNameSoundex = RandomString.BuildWithLength(10),
                   LastNameSoundex  = RandomString.BuildWithLength(10),
                   AliasId   = (new Random()).Next(1, int.MaxValue),
                   AliasFirstName   =  RandomString.BuildWithLength(255),
                   AliasMiddleName    =  RandomString.BuildWithLength(255),
                   AliasLastName     =  RandomString.BuildWithLength(255),
                   AliasSuffix      =   VocProfileResourceAliasSuffix.DC,
                   AliasFirstNameSoundex     = RandomString.BuildWithLength(10),
                   AliasLastNameSoundex      = RandomString.BuildWithLength(10),
                   ImageHref= RandomString.BuildWithLength(255)
                },
                new VocProfileResource() {
                   PublicId = Guid.NewGuid(),
                   Id    = (new Random()).Next(1, int.MaxValue),
                   AbimId =  (new Random()).Next(1, int.MaxValue).ToString(),
                   NpiNumber =  (new Random()).Next(1, int.MaxValue).ToString(),
                   Credential  =  true,
                   Deceased   =  false,
                   FirstName    =  RandomString.BuildWithLength(255),
                   LastName     =  RandomString.BuildWithLength(255),
                   MiddleName   =  RandomString.BuildWithLength(255),
                   Suffix  =  VocProfileResourceSuffix.Jr,
                   FirstNameSoundex = RandomString.BuildWithLength(10),
                   LastNameSoundex  = RandomString.BuildWithLength(10),
                   AliasId   = (new Random()).Next(1, int.MaxValue),
                   AliasFirstName   =  RandomString.BuildWithLength(255),
                   AliasMiddleName    =  RandomString.BuildWithLength(255),
                   AliasLastName     =  RandomString.BuildWithLength(255),
                   AliasSuffix      =   VocProfileResourceAliasSuffix.Jr,
                   AliasFirstNameSoundex     = RandomString.BuildWithLength(10),
                   AliasLastNameSoundex      = RandomString.BuildWithLength(10),
                   ImageHref= null
                }};  

            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject() };
         
            My<ICredentialService>()
                  .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                  .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            My<IMembershipClientService>()
                .Setup(o => o.GetNameAllAsync(
                           It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>())).ReturnsAsync(profileShortCollection); 

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/searchProfilesByNameAndDob?lastName=sam";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<ProfileShortCollectionResourcePublic>(ResponseContent);
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
            Resource.Data[0].LastName.Should().NotBeNull();
            Resource.Data[0].ImageHref.Should().NotBeNull();
            Resource.Data[1].ImageHref.Should().BeNull();
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class SearchProfilesByNameAndDob_MultipleRecordsCheckNameAlias_ReturnsOK : SearchProfilesByNameAndDobScenario
    {
        ProfileShortCollectionResourcePublic Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var profileShortCollection = new List<VocProfileResource>() {
                new VocProfileResource() {
                   PublicId =Guid.NewGuid(),
                   Id    = (new Random()).Next(1, int.MaxValue),
                   AbimId =   "123",
                   NpiNumber =   "1234",
                   Credential  =  true,
                   Deceased   =  false,
                   FirstName    =  RandomString.BuildWithLength(255),
                   LastName     =  RandomString.BuildWithLength(255),
                   MiddleName   =  RandomString.BuildWithLength(255),
                   Suffix  =  VocProfileResourceSuffix.DC,
                   FirstNameSoundex = RandomString.BuildWithLength(10),
                   LastNameSoundex  = RandomString.BuildWithLength(10),
                   AliasId   = (new Random()).Next(1, int.MaxValue),
                   AliasFirstName   = "SamAliase1",
                   AliasMiddleName    =  RandomString.BuildWithLength(255),
                   AliasLastName     =  "ReznitAliase1",
                   AliasSuffix      =   VocProfileResourceAliasSuffix.DC,
                   AliasFirstNameSoundex     = RandomString.BuildWithLength(10),
                   AliasLastNameSoundex      = RandomString.BuildWithLength(10),
                },
                new VocProfileResource() {
                   PublicId = Guid.NewGuid(),
                   Id    = (new Random()).Next(1, int.MaxValue),
                   AbimId =  "124",
                   NpiNumber =   "1245",
                   Credential  =  true,
                   Deceased   =  false,
                   FirstName    =  RandomString.BuildWithLength(255),
                   LastName     =  RandomString.BuildWithLength(255),
                   MiddleName   =  RandomString.BuildWithLength(255),
                   Suffix  =  VocProfileResourceSuffix.Jr,
                   FirstNameSoundex = RandomString.BuildWithLength(10),
                   LastNameSoundex  = RandomString.BuildWithLength(10),
                   AliasId   = (new Random()).Next(1, int.MaxValue),
                   AliasFirstName   = "SamAliase2",
                   AliasMiddleName    =  RandomString.BuildWithLength(255),
                   AliasLastName     =  "ReznitAliase2",
                   AliasSuffix      =   VocProfileResourceAliasSuffix.Jr,
                   AliasFirstNameSoundex     = RandomString.BuildWithLength(10),
                   AliasLastNameSoundex      = RandomString.BuildWithLength(10),
             }}; 
 

            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject() };

            My<IAccessTokenService>()
              .Setup(o => o.GetAccessToken())
              .Returns("--token--");

            My<ICredentialService>()
               .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult(CredentialDomainObjects));

            My<IMembershipClientService>()
                  .Setup(o => o.GetNameAllAsync(
                           It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>())).ReturnsAsync(profileShortCollection);

        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/searchProfilesByNameAndDob?lastName=sam";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<ProfileShortCollectionResourcePublic>(ResponseContent);
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
            Resource.Data[0].LastName.Should().NotBeNull();
        }

        public void AndThenMyResourceNameAliasesShouldNotBeNull()
        {
            Resource.Data[0].NameAliases.Should().NotBeNull();
            Resource.Data[0].NameAliases.Count.Should().Equals(2);
        }

        public void AndThenMyResourceNameAliasesShouldHaveProperValues()
        {
            Resource.Data[0].NameAliases[0].FirstName.Should().Equals("SamAliase");
            Resource.Data[0].NameAliases[0].LastName.Should().Equals("ReznitAliase1");
        }
    }

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class SearchProfilesByNameAndDob_OneRecord_ReturnsOK : SearchProfilesByNameAndDobScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var profileShortCollection = new List<VocProfileResource>() {
                new VocProfileResource() {
                   PublicId = Guid.NewGuid(),
                   Id    = (new Random()).Next(1, int.MaxValue),
                   AbimId =   "123",
                   NpiNumber =  "1234",
                   Credential  =  true,
                   Deceased   =  false,
                   FirstName    =  RandomString.BuildWithLength(255),
                   LastName     =  RandomString.BuildWithLength(255),
                   MiddleName   =  RandomString.BuildWithLength(255),
                   Suffix  =  VocProfileResourceSuffix.DC,
                   FirstNameSoundex = RandomString.BuildWithLength(10),
                   LastNameSoundex  = RandomString.BuildWithLength(10),
                   AliasId   = (new Random()).Next(1, int.MaxValue),
                   AliasFirstName   = "SamAliase1",
                   AliasMiddleName    =  RandomString.BuildWithLength(255),
                   AliasLastName     =  "ReznitAliase1",
                   AliasSuffix      =   VocProfileResourceAliasSuffix.DC,
                   AliasFirstNameSoundex     = RandomString.BuildWithLength(10),
                   AliasLastNameSoundex      = RandomString.BuildWithLength(10),
             }};

            var region = new List<RegionResource> { new RegionResource() {
                    Id = 1,
                    Code = "AL",
                    Name = "Alabama"
                } };

            CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject() };

            var profileResources = new List<VocProfileResource>() { 
                new VocProfileResource() {
                AbimId = "123",
                Id = 1256,
                FirstName = "Alex",
                LastName = "Reznit"
            }}; 

            My<ICredentialService>()
                 .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                 .Returns(Task.FromResult(CredentialDomainObjects));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");
             
            My<IMembershipClientService>()
                .Setup(o => o.GetVocByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profileResources);

            My<IMembershipClientService>()
             .Setup(o => o.GetNameAllAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<DateTime?>(),
                It.IsAny<bool>(),
                It.IsAny<int>(),
                It.IsAny<int>())).ReturnsAsync(profileShortCollection);

            My<IMembershipClientService>().Setup(_ => _.GetCountryRegionsAsync(It.IsAny<string>()))
                   .ReturnsAsync(region);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/searchProfilesByNameAndDob?lastName=sam";
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
    public class SearchProfilesByNameAndDobReturnsNotFound : SearchProfilesByNameAndDobScenario
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

            var ex = new ApiException("Not Found", 404, "", null, null); 

            My<ICredentialService>()
               .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new ProfileResource() { Id = Guid.NewGuid(), AbimId = "123" });

            My<IMembershipClientService>()
              .Setup(o => o.GetNameAllAsync(
                           It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<DateTime?>(),  
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>())).Throws(ex);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/searchProfilesByNameAndDob?lastName=notfound";
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
            ResponseContent.Should().Contain("No users exist with Last Name:");
        } 
    }

    public class SearchProfilesByNameAndDob_NotFoundForCoSponsoredDiplomate : SearchProfilesByNameAndDobScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        string AbimId = RandomString.BuildWithLength(10);

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var profileShortCollection = new List<VocProfileResource>() {
                new VocProfileResource() {
                   PublicId = Guid.NewGuid(),
                   Id    = (new Random()).Next(1, int.MaxValue),
                   AbimId =   AbimId,
                   NpiNumber =  "1234",
                   Credential  =  true,
                   Deceased   =  false,
                   FirstName    =  "Alex",
                   LastName     =  "Reznit",
                   MiddleName   =  RandomString.BuildWithLength(255),
                   Suffix  =  VocProfileResourceSuffix.DC,
                   FirstNameSoundex = RandomString.BuildWithLength(10),
                   LastNameSoundex  = RandomString.BuildWithLength(10),
                   AliasId   = (new Random()).Next(1, int.MaxValue),
                   AliasFirstName   = "SamAliase1",
                   AliasMiddleName    =  RandomString.BuildWithLength(255),
                   AliasLastName     =  "ReznitAliase1",
                   AliasSuffix      =   VocProfileResourceAliasSuffix.DC,
                   AliasFirstNameSoundex     = RandomString.BuildWithLength(10),
                   AliasLastNameSoundex      = RandomString.BuildWithLength(10),
             }};



            var profileResources = new List<VocProfileResource>();

            My<ICredentialService>()
               .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
               .Returns(Task.FromResult(CredentialDomainObjects));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            My<IMembershipClientService>()
            .Setup(o => o.GetVocByAbimIdAsync(It.IsAny<string>()))
            .ReturnsAsync(profileResources);

            My<IMembershipClientService>()
              .Setup(o => o.GetNameAllAsync(
                           It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>())).ReturnsAsync(profileShortCollection);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/searchProfilesByNameAndDob?lastName=sam";
        }

        public async Task WhenICallGetPhyCredential()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetResultContentShouldBeNoUserExists()
        {
            ResponseContent.Should().Contain($"No user exists with AbimId:'{AbimId}'.");
        }
    }
    #endregion
}
