using Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base;
using Abim.Platform.Program.WebApi.Objects;
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
        SoThat = "to get the system status."
        )]
    [TestFixture]
    public class GetSystemStatusApiSpec
    {
        [TestCase]
        [WorkItem(89181)]
        public void GetSystemStatusReturnsRunning()
        {
            new GetSystemStatusReturnsRunning().BDDfy();
        }
    }
    
    #region Scenarios

    /// <summary>
    /// The get system status scenario
    /// </summary>
    public class GetSystemStatusReturnsRunning : 
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
            Url = "/api/v1.0/app/status";
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

        public void AndThenTheResponseSaysRunning()
        {
            ResponseContent.ShouldBeEquivalentTo("\"\\\"" + ConfigurationManager.Instance.AppSettings["RunningStatusText"] + "\\\"\"");
        }
    }

    #endregion
}
