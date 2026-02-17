using Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base;
using Abim.Platform.Program.Util;
using Abim.Platform.Program.WebApi.Objects;
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
        SoThat = "to get the primary system options."
        )]
    [TestFixture]
    public class GetSystemOptionsApiSpec
    {
        [TestCase]
        [WorkItem(89189)]
        public void GetSystemOptionsReturnsCorrectData()
        {
            new GetSystemOptionsReturnsCorrectData().BDDfy();
        }

        [TestCase]
        [WorkItem(89189)]
        public void GetSystemOptionsAnonymousReturnsUnauthorized()
        {
            new GetSystemOptionsAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    #region Scenarios

    /// <summary>
    /// The get system options with token scenario
    /// </summary>
    public class GetSystemOptionsReturnsCorrectData : 
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
            Url = "/api/v1.0/system";
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
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

        public void AndThenTheResponseShouldHaveAMetricsLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Metrics"));
        }

        public void AndThenTheResponseShouldHaveAMetricsUILink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Metrics UI"));
        }

        public void AndThenTheResponseShouldHaveAJobUILink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Job UI"));
        }

        public void AndThenTheResponseShouldHaveABusInfoLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
                Resource["links"].Any(l => l.ToString().Contains("System Bus Info"));
        }

        public void AndThenTheResponseShouldHaveAVersionInfoLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("System Version Info"));
        }

        public void AndThenTheResponseShouldHaveAVersionLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Get App Version"));
        }

        public void AndThenTheResponseShouldHaveAnUptimeLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            // ReSharper disable once ReturnValueOfPureMethodIsNotUsed
            Resource["links"].Any(l => l.ToString().Contains("Get Uptime"));
        }
    }
    
    /// <summary>
    /// The get system options without token scenario
    /// </summary>
    public class GetSystemOptionsAnonymousReturnsUnauthorized : 
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
            Url = "/api/v1.0/system";
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
