using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base;
using FluentAssertions;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System.Configuration.Abstractions;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the AppInfo api",
        SoThat = "to get the system uptime."
        )]
    [TestFixture]
    public class GetSystemUptimeApiSpec
    {
        [TestCase]
        [WorkItem(74182)]
        public void GetSystemUptimeReturnsOk()
        {
            new GetSystemUptimeReturnsOk().BDDfy();
        }
    }

    #region Scenarios

    /// <summary>
    /// The get system uptime scenario
    /// </summary>
    public class GetSystemUptimeReturnsOk : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        
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
            Url = "/api/v1.0/app/uptime";
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }
    }

    #endregion
}
