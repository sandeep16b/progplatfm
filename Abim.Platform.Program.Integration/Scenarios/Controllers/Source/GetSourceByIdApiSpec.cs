using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Source.Base;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Source
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Source Api",
        SoThat = "to get a single Source's details"
        )]
    [TestFixture]
    public class GetSourceByIdApiSpec
    {
        [TestCase]
        [WorkItem(74795)]
        public void GetSourceReturnsOK()
        {
            new GetSourceReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(74795)]
        public void GetSourceReturnsForbidden()
        {
            new GetSourceReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(74795)]
        public void GetSourceInvalidIdReturnsNotFound()
        {
            new GetSourceInvalidIdReturnsNotFound().BDDfy();
        }

        [TestCase]
        [WorkItem(74795)]
        public void GetSourceNonGuidIdReturnsProperMessage()
        {
            new GetSourceNonGuidIdReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(74795)]
        public void GetSourceAnonymousReturnsUnauthorized()
        {
            new GetSourceAnonymousReturnsUnauthorized().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetSourceByIdScenario : SourceControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(IEnumService));
            list.Add(typeof(ISourceService));
            return list;
        }

        protected App.Domain.Source ConstructDomainObject()
        {
            var model = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            return model;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetSourceReturnsOK : GetSourceByIdScenario
    {
        SourceResource Resource { get; set; }
        App.Domain.Source DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ISourceService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Source/" + Guid.NewGuid();
        }

        public async Task WhenICallGetSource()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }
    }

    public class GetSourceReturnsForbidden : GetSourceByIdScenario
    {
        SourceResource Resource { get; set; }
        App.Domain.Source DomainObject { get; set; }

        protected override void PreSetup()
        {
            OverrideScope();
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ISourceService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Source/" + Guid.NewGuid();
        }

        public async Task WhenICallGetSource()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The Invalid Id scenario
    /// </summary>
    public class GetSourceInvalidIdReturnsNotFound : GetSourceByIdScenario
    {
        SourceResource Resource { get; set; }
        App.Domain.Source DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ISourceService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns<App.Domain.Source>(null);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Source/" + Guid.NewGuid();
        }

        public async Task WhenICallGetSource()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetANotFoundResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    /// <summary>
    /// The non-Guid Id scenario
    /// </summary>
    public class GetSourceNonGuidIdReturnsProperMessage : GetSourceByIdScenario
    {
        SourceResource Resource { get; set; }
        App.Domain.Source DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ISourceService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassAnInvalidUrl()
        {
            Url = "/api/v1.0/Source/" + (new Random()).Next(0, int.MaxValue);
        }

        public async Task WhenICallGetSource()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceResource>(ResponseContent);
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
    public class GetSourceAnonymousReturnsUnauthorized : GetSourceByIdScenario
    {
        SourceResource Resource { get; set; }
        App.Domain.Source DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
        }
        
        public void GivenIGoToTheUrlWithNoToken()
        {
            Url = "/api/v1.0/Source/" + Guid.NewGuid();
        }

        public async Task WhenICallGetSource()
        {
            Result = await HttpServer.CreateRequest(Url).GetAsync();
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
