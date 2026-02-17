using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
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
using System.Collections.Generic;
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
        SoThat = "to UnEnroll given user in the CMP pathway"
        )]
    [TestFixture]
    public class UnEnrollInCMPApiSpec
    {
        [TestCase]
        [WorkItem(151637)]
        public void UnEnrollInCMPReturnsOK()
        {
            new UnEnrollInCMPReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(151637)]
        public void UnEnrollInCMPReturnsForbidden()
        {
            new UnEnrollInCMPReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(151637)]
        public void UnEnrollInCMPCommandNotBoundReturnsBadRequest()
        {
            new UnEnrollInCMPCommandNotBoundReturnsBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(151637)]
        public void UnEnrollInCMPCommandInvalidIdReturnsBadRequest()
        {
            new UnEnrollInCMPCommandInvalidIdReturnsBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(151637)]
        public void UnEnrollInCMPAnonymousReturnsUnauthorized()
        {
            new UnEnrollInCMPAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class UnEnrollInCMPScenario : CredentialControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(IEnumService));
            list.Add(typeof(ICredentialService));
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
    public class UnEnrollInCMPReturnsOK : UnEnrollInCMPScenario
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
            DomainObject.IsInCMP = false;
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UnEnrollInCMPCommand>()))
                .Returns(Task.FromResult(new UnEnrollInCMPCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new UnEnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = memberId,
                UnEnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4,5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/UnEnrollInCMP";
        }

        public async Task WhenICallUnEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UnEnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
                .PostAsync().ConfigureAwait(true);

            ResponseContent = await Result.Content.ReadAsStringAsync().ConfigureAwait(false);
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

        public void AndThenIShouldHaveCorrectMemberId()
        {
            Resource.MemberId.Should().Equals(memberId);
        }

        public void AndThenIShouldIsInCMPEqualsToTrue()
        {
            Resource.IsInCMP.Should().Equals(false);
        }

        public void AndThenIShouldPathwayEqualsToExpectedValue()
        {
            Resource.Pathway.Should().NotBeSameAs(PathwayType.OneYear);
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "certification");
        }
    }

    public class UnEnrollInCMPReturnsForbidden : UnEnrollInCMPScenario
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
            DomainObject.IsInCMP = false;
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UnEnrollInCMPCommand>()))
                .Returns(Task.FromResult(new UnEnrollInCMPCommandResult(CommandStatus.Accepted, null, DomainObject)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new UnEnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = memberId,
                UnEnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/UnEnrollInCMP";
        }

        public async Task WhenICallUnEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UnEnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
    public class UnEnrollInCMPCommandNotBoundReturnsBadRequest : UnEnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UnEnrollInCMPCommand>()))
                .Returns<App.Domain.Credential>(null);
        }

        public void GivenIGoToTheUrlWithAnUnboundCommand()
        {
            Body = null;
            Url = $"/api/v1.0/UnEnrollInCMP";
        }

        public async Task WhenICallUnEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UnEnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
    public class UnEnrollInCMPCommandInvalidIdReturnsBadRequest : UnEnrollInCMPScenario
    {
        CredentialResource Resource { get; set; }
        App.Domain.Credential DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICredentialService>()
                .Setup(o => o.Handle(It.IsAny<UnEnrollInCMPCommand>()))
                .Returns(Task.FromResult(new UnEnrollInCMPCommandResult(CommandStatus.Rejected, null, null)));
        }

        public void GivenIPassTheCorrectUrl()
        {
            Body = new UnEnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = Guid.NewGuid(),
                UnEnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/UnEnrollInCMP";
        }

        public async Task WhenICallUnEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UnEnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
    public class UnEnrollInCMPAnonymousReturnsUnauthorized : UnEnrollInCMPScenario
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
            Body = new UnEnrollInCMPCommand()
            {
                UserInfo = new UserInfo()
                {
                    AbimId = (new Random()).Next(1, int.MaxValue).ToString()
                },
                AbimId = Random.Next(1000, 100000).ToString(),
                MemberId = Guid.NewGuid(),
                UnEnrollmentDate = DateTime.Now,
                SubspecialtyCertCode = RandomString.BuildWithLength(4, 5),
                RequestingUserName = RandomString.Build()
            };

            Url = $"/api/v1.0/UnEnrollInCMP";
        }

        public async Task WhenICallUnEnrollInCMP()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(UnEnrollInCMPCommand), Body, new JsonMediaTypeFormatter()))
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
