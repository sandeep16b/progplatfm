using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential Api",
        SoThat = "to toggle the SelectedToMaintain flag"
        )]
    [TestFixture]
    public class UpdateSelectedToMaintainApiSpec
    {
        [TestCase]
        [WorkItem(75621)]
        public void UpdateSelectedToMaintainReturnsOK()
        {
            new UpdateSelectedToMaintainReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(75621)]
        public void UpdateSelectedToMaintainReturnsForbidden()
        {
            new UpdateSelectedToMaintainReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(75621)]
        public void UpdateSelectedToMaintainCommandNotBoundReturnsBadRequest()
        {
            new UpdateSelectedToMaintainCommandNotBoundReturnsBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(75621)]
        public void UpdateSelectedToMaintainCommandInvalidIdReturnsBadRequest()
        {
            new UpdateSelectedToMaintainCommandInvalidIdReturnsBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(75621)]
        public void UpdateSelectedToMaintainNonGuidIdReturnsProperMessage()
        {
            new UpdateSelectedToMaintainNonGuidIdReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(75621)]
        public void UpdateSelectedToMaintainAnonymousReturnsUnauthorized()
        {
            new UpdateSelectedToMaintainAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class UpdateSelectedToMaintainScenario : CredentialControllerScenario
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
    public class UpdateSelectedToMaintainReturnsOK : UpdateSelectedToMaintainScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        Guid memberId;

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            memberId = Guid.NewGuid();
            DomainObject.MemberId= memberId;
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UpdateSelectedToMaintainCommand>()))
                .Returns(Task.FromResult(new UpdateSelectedToMaintainCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new UpdateSelectedToMaintainCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                SelectedToMaintain = true
            };

            Url = $"/api/v1.0/Credential/{Guid.NewGuid()}/selectedToMaintain";
        }

        public async Task WhenICallUpdateSelectedToMaintain()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UpdateSelectedToMaintainCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
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

        public void AndThenIShouldGetBackTheRegistration()
        {
            Resource.MemberId.Should().Equals(memberId);
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "certification");
        }
    }

    public class UpdateSelectedToMaintainReturnsForbidden : UpdateSelectedToMaintainScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        Guid memberId;

        protected override void PreSetup()
        {
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            memberId = Guid.NewGuid();
            DomainObject.MemberId = memberId;
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UpdateSelectedToMaintainCommand>()))
                .Returns(Task.FromResult(new UpdateSelectedToMaintainCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new UpdateSelectedToMaintainCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                SelectedToMaintain = true
            };

            Url = $"/api/v1.0/Credential/{Guid.NewGuid()}/selectedToMaintain";
        }

        public async Task WhenICallUpdateSelectedToMaintain()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UpdateSelectedToMaintainCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
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
    public class UpdateSelectedToMaintainCommandNotBoundReturnsBadRequest : UpdateSelectedToMaintainScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UpdateSelectedToMaintainCommand>()))
                .Returns<App.Domain.Credential>(null);
        }

        public void GivenIGoToTheUrlWithAnUnboundCommand()
        {
            Body = null;
            Url = $"/api/v1.0/Credential/{Guid.NewGuid()}/selectedToMaintain";
        }

        public async Task WhenICallUpdateSelectedToMaintain()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UpdateSelectedToMaintainCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetANotFoundResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    /// <summary>
    /// The Invalid Id scenario GetCredentialInvalidIdReturnsNotFound
    /// </summary>
    public class UpdateSelectedToMaintainCommandInvalidIdReturnsBadRequest : UpdateSelectedToMaintainScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UpdateSelectedToMaintainCommand>()))
                .Returns(Task.FromResult(new UpdateSelectedToMaintainCommandResult(CommandStatus.Rejected, null, null)));
        }

        public void GivenIGoToTheUrlWithAnUnboundCommand()
        {
            Body = new UpdateSelectedToMaintainCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                SelectedToMaintain = true
            };

            Url = $"/api/v1.0/Credential/{Guid.NewGuid()}/selectedToMaintain";
        }

        public async Task WhenICallUpdateSelectedToMaintain()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UpdateSelectedToMaintainCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetANotFoundResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }

    /// <summary>
    /// The non-Guid Id scenario
    /// </summary>
    public class UpdateSelectedToMaintainNonGuidIdReturnsProperMessage : UpdateSelectedToMaintainScenario
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
                .Setup(o => o.Handle(It.IsAny<UpdateSelectedToMaintainCommand>()))
                .Returns(Task.FromResult(new UpdateSelectedToMaintainCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassAnInvalidUrl()
        {
            Body = new UpdateSelectedToMaintainCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                SelectedToMaintain = true
            };

            Url = $"/api/v1.0/Credential/{(new Random()).Next(0, int.MaxValue)}/selectedToMaintain";
        }

        public async Task WhenICallUpdateSelectedToMaintain()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UpdateSelectedToMaintainCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
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
    public class UpdateSelectedToMaintainAnonymousReturnsUnauthorized : UpdateSelectedToMaintainScenario
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
            Body = new UpdateSelectedToMaintainCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                SelectedToMaintain = true
            };

            Url = $"/api/v1.0/Credential/{Guid.NewGuid()}/selectedToMaintain";
        }

        public async Task WhenICallUpdateSelectedToMaintain()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UpdateSelectedToMaintainCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
