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
using ProfileShortCollectionResource = Abim.Enterprise.Core.Profile.Resource.ProfileShortCollectionResource;
using ProfileSummaryShortResource = Abim.Enterprise.Core.Profile.Resource.ProfileSummaryShortResource;

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
            var profileShortCollection = new ProfileShortCollectionResource()
            {
                CurrentPage = 1,
                PageSize = 2,
                TotalCount = 2,
                TotalPages = 1,
                Data = new List<ProfileSummaryShortResource>() {
                    new ProfileSummaryShortResource() { AbimId="123", Id=new Guid(),
                        Name = new NameResource() { FirstName="Sam", LastName="Reznit"} },
                    new ProfileSummaryShortResource() { AbimId="124", Id=new Guid(),
                        Name = new NameResource() { FirstName="Sam", LastName="Smith"} }
                }
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNameAndDob(It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(), //lastname
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>()))
                .Returns(Task.FromResult(profileShortCollection));

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
            var profileShortCollection = new ProfileShortCollectionResource()
            {
                CurrentPage = 1,
                PageSize = 2,
                TotalCount = 2,
                TotalPages = 1,
                Data = new List<ProfileSummaryShortResource>() {
                    new ProfileSummaryShortResource() { AbimId="123", Id=new Guid(),
                        Name = new NameResource() { FirstName="Sam", LastName="Reznit"} ,
                        NameAliases = new List<ProfileNameAliasSummaryResource> () { new ProfileNameAliasSummaryResource()
                        {
                            Name = (new NameResource() { FirstName = "SamAliase1", LastName = "ReznitAliase1", MiddleName="middle" }), UserAccountsId=12345, UserAccountsNameAliasId = 789654
                        },
                        new ProfileNameAliasSummaryResource()
                        {
                            Name = (new NameResource() { FirstName = "SamAliase", LastName = "ReznitAliase", MiddleName="middle" }), UserAccountsId=12345, UserAccountsNameAliasId = 789654 }
                        } },
                    new ProfileSummaryShortResource() { AbimId="124", Id=new Guid(),
                        Name = new NameResource() { FirstName="Sam", LastName="Smith"},
                        NameAliases = new List<ProfileNameAliasSummaryResource> () { new ProfileNameAliasSummaryResource()
                        {
                            Name = (new NameResource() { FirstName = "SamAliase", LastName = "ReznitAliase", MiddleName="middle" }), UserAccountsId=12345, UserAccountsNameAliasId = 789654
                        }
                    }
                    }
                }
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNameAndDob(It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(), //lastname
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>()))
                .Returns(Task.FromResult(profileShortCollection));

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
            var profileShortCollection = new ProfileShortCollectionResource()
            {
                CurrentPage = 1,
                PageSize = 1,
                TotalCount = 1,
                TotalPages = 1,
                Data = new List<ProfileSummaryShortResource>() {
                    new ProfileSummaryShortResource() { AbimId="123", Id=new Guid(),
                        Name = new NameResource() { FirstName="Sam", LastName="Reznit"} }
                }
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNameAndDob(It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(), //lastname
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>()))
                .Returns(Task.FromResult(profileShortCollection));

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
            CredentialDomainObjects = new List<App.Domain.Credential>() { };

            Enterprise.Core.Profile.Interservice.Util.Extensions.UnsuccessfulStatusException ex = new Enterprise.Core.Profile.Interservice.Util.Extensions.UnsuccessfulStatusException();
            ex.StatusCode = HttpStatusCode.NotFound;

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNameAndDob(It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(), //lastname
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>()))
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

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var profileShortCollection = new ProfileShortCollectionResource()
            {
                CurrentPage = 1,
                PageSize = 1,
                TotalCount = 1,
                TotalPages = 1,
                Data = new List<ProfileSummaryShortResource>() {
                    new ProfileSummaryShortResource() { AbimId="123", Id=new Guid(),
                        Name = new NameResource() { FirstName="Sam", LastName="Reznit"} }
                }
            };

            My<IProfileInterservice>()
                .Setup(o => o.SearchProfilesByNameAndDob(It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(),
                            It.IsAny<string>(), //lastname
                            It.IsAny<DateTime?>(),
                            It.IsAny<bool>(),
                            It.IsAny<int>(),
                            It.IsAny<int>()))
                .Returns(Task.FromResult(profileShortCollection));

            //CredentialDomainObjects = new List<App.Domain.Credential>() { ConstructDomainObject() };
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
            ResponseContent.Should().Contain("No user exists with AbimId:'123'.");
        }
    }
    #endregion
}
