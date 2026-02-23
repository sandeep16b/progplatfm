using Abim.Enterprise.Core.Profile.Resource;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Resources;
using FluentAssertions;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;


namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "Portal User",
        IWant = "to be able to download my VOC Letter",
        SoThat = "I can use it"
        )]
    [TestFixture]
    public class GetVocLetterApiSpec
    {
        [TestCase]
        public void GetVocLetterReturnsOK()
        {
            new GetVocLetterReturnsOK().BDDfy();
        }

        [TestCase]
        public void GetVocLetterReturnsBadRequestWhenOtherAbimId()
        {
            new GetVocLetterReturnsBadRequestWhenOtherAbimId().BDDfy();
        }
    }

    public class GetVocLetterReturnsOK : GetVocLetterScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            ResetContainerToDependencyResolverOneOnInjectAdditionalDependencies = true;
        }
        protected override void PostSetup()
        {
            My<IProfileInterservice>()
                .Setup(o => o.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(new ProfileNestedResource() { Id = Guid.NewGuid()  }));

            My<ICredentialService>()
                .Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult((IEnumerable <App.Domain.Credential>)new List<App.Domain.Credential>()));

            My<IHelperService>()
                .Setup(o => o.GetVocLetterContent(It.IsAny<ProfileNestedResource>(), It.IsAny<IEnumerable<App.Domain.Credential>>()))
                .Returns(new App.Domain.VocPdfData());

            My<IHelperService>()
                .Setup(o => o.CreateVocLetter(It.IsAny<App.Domain.VocPdfData>()))
                .Returns(new MemoryStream());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/vocletter/" + UserKey.Key;
        }

        public async Task WhenICallGetVocLetter()
        {
            Result = await HttpServer.CreateRequest(Url)
               .AddHeader("Authorization", "Bearer " + Token)
               .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    public class GetVocLetterReturnsBadRequestWhenOtherAbimId : GetVocLetterScenario
    {
        PhysicianCertificationsPublicResource Resource { get; set; }

        IEnumerable<App.Domain.Credential> CredentialDomainObjects { get; set; }

        // override base method and set different UserKey value
        protected async override Task<int> GetToken()
        {
            // use different test user name who is not admin (DevAdmin, QAAdmin, SG-G-PortalAdmin-Dev, SG-G-PortalAdmin-QA)
            UserKey = new KeyValuePair<string, string>("288545", "1Password#");
            return await base.GetToken();
        }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            ResetContainerToDependencyResolverOneOnInjectAdditionalDependencies = true;
        }
        protected override void PostSetup()
        {

        }

        public void GivenIPassTheCorrectUrlWithIncorrectAbimId()
        {
            Url = "/api/v1.0/vocletter/345678";
        }

        public async Task WhenICallGetVocLetter()
        {
            Result = await HttpServer.CreateRequest(Url)
               .AddHeader("Authorization", "Bearer " + Token)
               .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetVocLetterScenario : CredentialControllerScenario
    {
        protected Mock<ILogger> Log { get; set; }
    }
}
