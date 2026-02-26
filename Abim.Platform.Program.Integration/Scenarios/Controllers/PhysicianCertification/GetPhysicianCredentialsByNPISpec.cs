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
    public class GetPhysicianCredentialsByNPIReturnsOK : GetPhyCredentialByNPIScenario
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

            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())
            };

            My<IMembershipClientService>()
            .Setup(o => o.GetProfileByMemberIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(profileResource);
            
            My<IMembershipClientService>()
            .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
            .ReturnsAsync(profileSummaryShortResource); 

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
    public class GetPhysicianCredentialsByNPIReturnsOnlyABIMCert : GetPhyCredentialByNPIScenario
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

            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } }; 
     
            var GUID = Guid.NewGuid();

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(CredentialDomainObjects); 

            My<IMembershipClientService>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
                .ReturnsAsync(profileSummaryShortResource); 

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
    public class GetPhysicianCredentialsByNPIDontReturnCoSponsoredCreds : GetPhyCredentialByNPIScenario
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
            
            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())
            };   

            My<IMembershipClientService>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
                .ReturnsAsync(profileSummaryShortResource);

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
    public class GetPhysicianCredentialsByNPIReturnsNoABIMCert : GetPhyCredentialByNPIScenario
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
       
            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };  

            My<IMembershipClientService>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
                .ReturnsAsync(profileSummaryShortResource);

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
    public class GetPhysicianCredentialsByNPIReturnsNoABIMCertIssuances : GetPhyCredentialByNPIScenario
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
           
            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };

            ProfileResource profileResource = new ProfileResource()
            {
                AbimId = "123",
                Name = new ProfileNameResource() { FirstName = "Alex", LastName = "Reznit" },
                Aliases = (new List<ProfileAliasResource>())
            }; 

            //My<IProfileApiClientWraperService>()
            //      .Setup(o => o.ProfileGETAsync(It.IsAny<Guid>())).ReturnsAsync(profileResource);

            My<IMembershipClientService>()
                 .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
                 .ReturnsAsync(profileSummaryShortResource);

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
    public class GetPhysicianCredentialsByNPI_NoCredentials_ReturnsOk : GetPhyCredentialByNPIScenario
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

            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };

            My<IMembershipClientService>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
                .ReturnsAsync(profileSummaryShortResource);

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
    public class GetPhysicianCredentialsByNPIReturnsNotFound : GetPhyCredentialByNPIScenario
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

            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };

            var ex = new ApiException("Not Found", 404, "", null, null); 

            My<IMembershipClientService>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
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
    public class GetPhysicianCredentialsByNPIReturnsNotFoundForCoSponsoredDiplomate : GetPhyCredentialByNPIScenario
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

            List<VocProfileResource> profileSummaryShortResource = new List<VocProfileResource>() { new VocProfileResource()
            {
                AbimId = "123",
                FirstName = "Alex",
                LastName = "Reznit",
                MiddleName = "Mike",
                AliasFirstName = "",
                AliasMiddleName = "",
                AliasLastName = "",
            } };

            My<IMembershipClientService>()
                .Setup(o => o.SearchProfilesByNPI(It.IsAny<string>()))
                .ReturnsAsync(profileSummaryShortResource);

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
