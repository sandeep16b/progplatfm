using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Domain = Abim.Platform.Program.App.Domain;



namespace Abim.Platform.Program.Integration.Scenarios.Controllers
{
    /// <summary>
    /// Integration test class
    /// </summary>
    /// <remarks>
    ///  <seealso cref="http://stackoverflow.com/questions/7366495/use-for-workitemattribute">TFS Work Item Association</seealso>
    /// </remarks>
    [Story(
        AsA = "Interservice",
        IWant = "to be able to call an Admin Api",
        SoThat = "through interservice calls using the background client"
        )]
    [TestFixture]
    public class BackgroundClientAdminApiSpec
    {
        [TestCase]
        public void GetCredentialReturnsOKOnAdminApiWithClientCredentials()
        {
            new GetCredentialReturnsOKOnAdminApiWithClientCredentials().BDDfy();
        }

    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetCredentialByIdScenario : CredentialControllerScenario
    {

        protected App.Domain.Credential ConstructDomainObject()
        {
            var source = Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var certification = Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                RandomString.Build(), RandomString.Build(), RandomString.Build());
            var credential = Domain.Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            return credential;
        }
    }

    public class GetCredentialReturnsOKOnAdminApiWithClientCredentials : GetCredentialByIdScenario
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
            Url = "/api/v1.0/credentials/member/" + Guid.NewGuid();
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

    }

}
