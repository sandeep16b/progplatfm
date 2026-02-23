using Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base;
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
        SoThat = "to get the information on system version."
        )]
    [TestFixture]
    public class GetSystemVersionInfoApiSpec
    {
        [TestCase]
        [WorkItem(74034)]
        public void GetSystemVersionInfoReturnsCorrectData()
        {
            new GetSystemVersionInfoReturnsCorrectData().BDDfy();
        }
    }

    #region Scenarios

    /// <summary>
    /// The get system version info scenario
    /// </summary>
    public class GetSystemVersionInfoReturnsCorrectData : 
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
            Url = "/api/v1.0/system/version";
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
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

        public void AndTheResponseShouldContainVersionInfo()
        {
            Resource["versionInfo"].Should().NotBeNull();
            Resource["versionInfo"]["productVersion"].Should().NotBeNull();
        }

        public void AndThenTheResponseShouldHaveASelfLink()
        {
            Resource["links"].Should().NotBeNull();
            Resource["links"].Any(l => l.ToString().Contains("self"));
        }
    }

    #endregion
}
