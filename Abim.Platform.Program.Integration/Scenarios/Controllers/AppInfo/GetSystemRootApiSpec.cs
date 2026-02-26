using Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base;
using Abim.Platform.Program.WebApi.Objects;
//using Abim.Platform.Program.Tests.Setup;
using FluentAssertions;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Configuration.Abstractions;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the AppInfo api",
        SoThat = "to get the system root links."
        )]
    [TestFixture]
    public class GetSystemRootApiSpec
    {
        [TestCase]
        [WorkItem(74032)]
        public void GetSystemRootReturnsCorrectData()
        {
            new GetSystemRootReturnsCorrectData().BDDfy();
        }

        [TestCase]
        [WorkItem(74032)]
        public void GetSystemRootAnonymousReturnsUnauthorized()
        {
            new GetSystemRootAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    #region Scenarios

    /// <summary>
    /// The get system root with token scenario
    /// </summary>
    public class GetSystemRootReturnsCorrectData :
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService.Object);
            Container.Inject(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/system/root";
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JObject.Parse(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldBeValidJson()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenTheResponseShouldHaveLinks()
        {
            Resource["links"].Should().NotBeNull();
        }

        public void AndThenTheResponseShouldHaveASelfLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("self"));
        }

        public void AndThenTheResponseShouldHaveSourceGetAllLinks()
        {
            //2 for Get and Post
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Count(l => l.ToString().Contains("Get all Sources")).Should().BeGreaterOrEqualTo(2);
        }

        public void AndThenTheResponseShouldHaveASourceOptionsLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
             Resource["links"].Any(l => l.ToString().Contains("Source Options"));
        }

        public void AndThenTheResponseShouldHaveCredentialGetAllLinks()
        {
            //2 for Get and Post
            Resource["links"].Count(l => l.ToString().Contains("Get all Credentials")).Should().BeGreaterOrEqualTo(2);
        }

        public void AndThenTheResponseShouldHaveACredentialOptionsLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Credential Options"));
        }

        public void AndThenTheResponseShouldHaveCertificationGetAllLinks()
        {
            //2 for Get and Post
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Count(l => l.ToString().Contains("Get all Certifications")).Should().BeGreaterOrEqualTo(2);
        }

        public void AndThenTheResponseShouldHaveCertificationGetForCurrentUserLinks()
        {
            //2 for Get and Post
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Count(l => l.ToString().Contains("Get Current User Certifications")).Should().BeGreaterOrEqualTo(2);
        }

        public void AndThenTheResponseShouldHaveACertificationOptionsLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Certification Options"));
        }
    }
    
    /// <summary>
    /// The get system root without token scenario
    /// </summary>
    public class GetSystemRootAnonymousReturnsUnauthorized : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService.Object);
            Container.Inject(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/system/root";
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
