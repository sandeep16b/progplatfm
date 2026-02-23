using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Objects;
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
//using Abim.Enterprise.Core.Interservice;
//using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
//using Abim.Enterprise.Core.Profile.Interservice.Interservices;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential Api",
        SoThat = "to enroll given user in the CMP pathway"
        )]
    [TestFixture]
    public class EnrollInCMPApiSpec
    {
        [TestCase]
        [WorkItem(147259)]
        public void EnrollInCMPReturnsOK()
        {
            new EnrollInCMPReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(147259)]
        public void EnrollInCMPReturnsForbidden()
        {
            new EnrollInCMPReturnsForbidden().BDDfy();
        }
        [TestCase]
        [WorkItem(147259)]
        public void EnrollInCMPCommandNotBoundReturnsBadRequest()
        {
            new EnrollInCMPCommandNotBoundReturnsBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(147259)]
        public void EnrollInCMPCommandInvalidIdReturnsBadRequest()
        {
            new EnrollInCMPCommandInvalidIdReturnsBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(147259)]
        public void EnrollInCMPAnonymousReturnsUnauthorized()
        {
            new EnrollInCMPAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class EnrollInCMPScenario : CredentialControllerScenario
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
    public class EnrollInCMPReturnsOK : EnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        IEnumService EnumService { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        Guid memberId;

        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);

            DomainObject = ConstructDomainObject();
            memberId = Guid.NewGuid();
            DomainObject.MemberId= memberId;
            DomainObject.IsInCMP = true;
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<EnrollInCMPCommand>()))
                .Returns(Task.FromResult(new EnrollInCMPCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new EnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = memberId,
                EnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/enrollInCMP";
        }

        public async Task WhenICallEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(EnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync().ConfigureAwait(true);

            ResponseContent = await Result.Content.ReadAsStringAsync().ConfigureAwait(false);
            Resource = JsonConvert.DeserializeObject<CredentialResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenIShouldHaveCorrectMemberId()
        {
            Resource.MemberId.Should().Equals(memberId);
        }

        public void AndThenIShouldIsInCMPEqualsToTrue()
        {
            Resource.IsInCMP.Should().Equals(true);
        }
        public void AndThenIShouldPathwayEqualsToExpectedValue()
        {
            Resource.Pathway.Should().Equals(PathwayType.OneYear);
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "certification");
        }
    }


    public class EnrollInCMPReturnsForbidden : EnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        IEnumService EnumService { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        Guid memberId;

        protected override void PreSetup()
        {            
            EnumService = new EnumService();
            OverrideScope();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);

            DomainObject = ConstructDomainObject();
            memberId = Guid.NewGuid();
            DomainObject.MemberId = memberId;
            DomainObject.IsInCMP = true;
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<EnrollInCMPCommand>()))
                .Returns(Task.FromResult(new EnrollInCMPCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new EnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = memberId,
                EnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/enrollInCMP";
        }

        public async Task WhenICallEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(EnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync().ConfigureAwait(true);

            ResponseContent = await Result.Content.ReadAsStringAsync().ConfigureAwait(false);
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
    public class EnrollInCMPCommandNotBoundReturnsBadRequest : EnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<EnrollInCMPCommand>()))
                .Returns<App.Domain.Credential>(null);
        }

        public void GivenIGoToTheUrlWithAnUnboundCommand()
        {
            Body = null;
            Url = $"/api/v1.0/enrollInCMP";
        }

        public async Task WhenICallEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(EnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
    public class EnrollInCMPCommandInvalidIdReturnsBadRequest : EnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }
        IEnumService EnumService { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);

            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<EnrollInCMPCommand>()))
                .Returns(Task.FromResult(new EnrollInCMPCommandResult(CommandStatus.Rejected, null, null)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new EnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = Guid.NewGuid(),
                EnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/enrollInCMP";
        }

        public async Task WhenICallEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(EnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
    /// The anonymous call scenario
    /// </summary>
    public class EnrollInCMPAnonymousReturnsUnauthorized : EnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new EnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = Guid.NewGuid(),
                EnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/enrollInCMP";
        }

        public async Task WhenICallEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(EnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
