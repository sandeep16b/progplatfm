using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.OptionsResources;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Source.Base;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Objects;
using FluentAssertions;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Source
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Source api",
        SoThat = "to get a list of the available options for Sources."
        )]
    [TestFixture]
    public class GetSourceApiOptionsApiSpec
    {
        [TestCase]
        [WorkItem(78409)]
        public void GetSourceOptionsReturnsOk()
        {
            new GetSourceOptionsReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(78409)]
        public void GetSourceOptionsReturnsForbidden()
        {
            new GetSourceOptionsReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78409)]
        public void GetSourceOptionsAnonymousReturnsUnauthorized()
        {
            new GetSourceOptionsAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Source.Base.SourceControllerScenario" />
    public abstract class SourceOptionsScenario : SourceControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ISourceService));
            list.Add(typeof(ISourceRepository));
            list.Add(typeof(IBusControl));
            list.Add(typeof(IBackgroundJobClient));
            list.Add(typeof(IValidationFactory));
            return list;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetSourceOptionsReturnsOk : 
        SourceOptionsScenario
    {
        IEnumService EnumService { get; set; }
        SourceOptionsResponseResource Resource { get; set; }
        
        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetSourcesOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceCollectionShouldHaveASelfLink()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenMyResourceCollectionShouldHaveAllTheEnumLinks()
        {
            var expectedEnumTypes = EnumService.GetEnumTypesFor(typeof(App.Domain.Source), EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            foreach(var type in expectedEnumTypes)
            {
                Resource.Links.Should().Contain(link => link.Method.ToUpper() == "GET" && link.Name.ToLower().Contains(type.Name.ToLower()));
            }
            
            var allEnumLinks = Resource.Links.Where(link => link.Href.ToLower().Contains("/enum"));
            allEnumLinks.Count().ShouldBeEquivalentTo(expectedEnumTypes.Count);
        }

        public void AndThenMyResourceCollectionShouldHaveRootFunctionalityLinks()
        {
            Resource.Links.Should().Contain(link => link.Href.ToLower().Contains("/sources") && link.Method == "GET");
            Resource.Links.Should().Contain(link => link.Href.ToLower().Contains("/sources") && link.Method == "POST");
        }
    }

    public class GetSourceOptionsReturnsForbidden :
        SourceOptionsScenario
    {
        IEnumService EnumService { get; set; }
        SourceOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetSourcesOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetAnForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The Unauthorized scenario
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Source.SourceOptionsScenario"/>
    public class GetSourceOptionsAnonymousReturnsUnauthorized
        : SourceOptionsScenario
    {
        IEnumService EnumService { get; set; }
        SourceOptionsResponseResource Resource { get; set; }
       
        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetSourceOptionsWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
