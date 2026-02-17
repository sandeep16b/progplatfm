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
        SoThat = "to get the system version."
        )]
    [TestFixture]
    public class GetSystemVersionApiSpec
    {
        [TestCase]
        [WorkItem(89190)]
        public void GetSystemVersionReturnsOk()
        {
            new GetSystemVersionReturnsOk().BDDfy();
        }
    }

    #region Scenarios

    /// <summary>
    /// The get system version scenario
    /// </summary>
    public class GetSystemVersionReturnsOk : 
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
            Container.Inject(EnumService.Object);
            Container.Inject(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/app/version";
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
