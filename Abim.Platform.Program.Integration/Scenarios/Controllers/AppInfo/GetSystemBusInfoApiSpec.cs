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
using Assert = NUnit.Framework.Assert;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the AppInfo api",
        SoThat = "to get the information on the bus."
        )]
    [TestFixture]
    public class GetSystemBusInfoApiSpec
    {
        [TestCase]
        [WorkItem(74033)]
        public void GetSystemBusInfoReturnsOk()
        {
            new GetSystemBusInfoReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(74033)]
        public void GetSystemBusInfoAnonymousReturnsUnauthorized()
        {
            new GetSystemBusInfoAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    #region Scenarios

    /// <summary>
    /// The get system bus info with token scenario
    /// </summary>
    public class GetSystemBusInfoReturnsOk : 
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
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/system/bus";
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

        public void AndTheResponseShouldContainBusInfo()
        {
            Resource["busInfo"].Should().NotBeNull();
        }

        public void AndThenTheResponseShouldHaveASelfLink()
        {
            // ReSharper disable once AssignNullToNotNullAttribute
            Resource["links"].Should().NotBeNull();
            // ReSharper disable once AssignNullToNotNullAttribute
            var exists = Resource["links"].Any(l => l.ToString().Contains("self"));
            Assert.IsTrue(exists);
        }
    }
    
    /// <summary>
    /// The get system bus info without token scenario
    /// </summary>
    public class GetSystemBusInfoAnonymousReturnsUnauthorized : 
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
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/system/bus";
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
