using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Host.OptionsResources;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.Base;
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

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Certification
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Certification api",
        SoThat = "to get a list of the available options for Certifications."
        )]
    [TestFixture]
    public class GetCertificationApiOptionsApiSpec
    {
        [TestCase]
        [WorkItem(78409)]
        public void GetCertificationOptionsReturnsOk()
        {
            new GetCertificationOptionsReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(78409)]
        public void GetCertificationOptionsReturnsForbidden()
        {
            new GetCertificationOptionsReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78409)]
        public void GetCertificationOptionsAnonymousReturnsUnauthorized()
        {
            new GetCertificationOptionsAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.Base.CertificationControllerScenario" />
    public abstract class CertificationOptionsScenario : CertificationControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ICertificationService));
            list.Add(typeof(ICertificationRepository));
            list.Add(typeof(IBusControl));
            list.Add(typeof(IBackgroundJobClient));
            list.Add(typeof(IValidationFactory));
            list.Add(typeof(ISourceService));
            list.Add(typeof(ICredentialService));
            return list;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetCertificationOptionsReturnsOk : 
        CertificationOptionsScenario
    {
        IEnumService EnumService { get; set; }
        CertificationOptionsResponseResource Resource { get; set; }
        
        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetCertificationsOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationOptionsResponseResource>(ResponseContent);
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
            var expectedEnumTypes = EnumService.GetEnumTypesFor(typeof(App.Domain.Certification), EnumLinkSettings.IncludeNestedEnumTypesInLinks);
            foreach(var type in expectedEnumTypes)
            {
                Resource.Links.Should().Contain(link => link.Method.ToUpper() == "GET" && link.Name.ToLower().Contains(type.Name.ToLower()));
            }
            
            var allEnumLinks = Resource.Links.Where(link => link.Href.ToLower().Contains("/enum"));
            allEnumLinks.Count().ShouldBeEquivalentTo(expectedEnumTypes.Count);
        }

        public void AndThenMyResourceCollectionShouldHaveRootFunctionalityLinks()
        {
            Resource.Links.Should().Contain(link => link.Href.ToLower().Contains("/certifications") && link.Method == "GET");
            Resource.Links.Should().Contain(link => link.Href.ToLower().Contains("/certifications") && link.Method == "POST");
        }
    }

    public class GetCertificationOptionsReturnsForbidden : CertificationOptionsScenario
    {
        IEnumService EnumService { get; set; }
        CertificationOptionsResponseResource Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
            OverrideScope();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetCertificationsOptions()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The Unauthorized scenario
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.CertificationOptionsScenario"/>
    public class GetCertificationOptionsAnonymousReturnsUnauthorized
        : CertificationOptionsScenario
    {
        IEnumService EnumService { get; set; }
        CertificationOptionsResponseResource Resource { get; set; }
       
        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetCertificationOptionsWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationOptionsResponseResource>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
