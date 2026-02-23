using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo.Base;
using FluentAssertions;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Configuration.Abstractions;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Platform.Program.Util;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.AppInfo
{
    [Story(
        AsA = "external application",
        IWant = "to be able to get a proper Resource Not Found error message",
        SoThat = "from any urls I hit which are not correct"
        )]
    [TestFixture]
    public class DefaultEndpointApiSpec
    {
        [TestCase]
        [WorkItem(89194)]
        public void IncorrectEndpointWithGetReturnsProperMessage()
        {
            new IncorrectEndpointWithGetReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(89194)]
        public void IncorrectEndpointWithPostReturnsProperMessage()
        {
            new IncorrectEndpointWithPostReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(89194)]
        public void IncorrectEndpointWithPutReturnsProperMessage()
        {
            new IncorrectEndpointWithPutReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(89194)]
        public void IncorrectEndpointWithOptionsReturnsProperMessage()
        {
            new IncorrectEndpointWithOptionsReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(89194)]
        public void IncorrectEndpointWithDeleteReturnsProperMessage()
        {
            new IncorrectEndpointWithDeleteReturnsProperMessage().BDDfy();
        }
    }

    #region Scenarios
    
    /// <summary>
    /// The Get scenario
    /// </summary>
    public class IncorrectEndpointWithGetReturnsProperMessage : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        new Random Random                       { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
            Random          = new Random();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIHitANonExistentUrl()
        {
            var length = Random.Next(3, 20);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var randomString = new StringBuilder();
            for(int i = 0; i < length; i++)
                randomString.Append(chars[Random.Next(chars.Length - 1)]);
            
            Url = "/api/v1.0/" + randomString;
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Get.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetA404()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void AndThenIGetTheProperErrorMessage()
        {
            ResponseContent.Should().Be("\"Resource not found\"");
        }
    }
    
    /// <summary>
    /// The Post scenario
    /// </summary>
    public class IncorrectEndpointWithPostReturnsProperMessage : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        new Random Random                       { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
            Random          = new Random();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIHitANonExistentUrl()
        {
            var length = Random.Next(3, 20);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var randomString = new StringBuilder();
            for(int i = 0; i < length; i++)
                randomString.Append(chars[Random.Next(chars.Length - 1)]);
            
            Url = "/api/v1.0/" + randomString;
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Post.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetA404()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void AndThenIGetTheProperErrorMessage()
        {
            ResponseContent.Should().Be("\"Resource not found\"");
        }
    }
    
    /// <summary>
    /// The Put scenario
    /// </summary>
    public class IncorrectEndpointWithPutReturnsProperMessage : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        new Random Random                       { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
            Random          = new Random();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIHitANonExistentUrl()
        {
            var length = Random.Next(3, 20);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var randomString = new StringBuilder();
            for(int i = 0; i < length; i++)
                randomString.Append(chars[Random.Next(chars.Length - 1)]);
            
            Url = "/api/v1.0/" + randomString;
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Put.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetA404()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void AndThenIGetTheProperErrorMessage()
        {
            ResponseContent.Should().Be("\"Resource not found\"");
        }
    }
    
    /// <summary>
    /// The Options scenario
    /// </summary>
    public class IncorrectEndpointWithOptionsReturnsProperMessage : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        new Random Random                       { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
            Random          = new Random();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIHitANonExistentUrl()
        {
            var length = Random.Next(3, 20);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var randomString = new StringBuilder();
            for(int i = 0; i < length; i++)
                randomString.Append(chars[Random.Next(chars.Length - 1)]);
            
            Url = "/api/v1.0/" + randomString;
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Options.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetA404()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void AndThenIGetTheProperErrorMessage()
        {
            ResponseContent.Should().Be("\"Resource not found\"");
        }
    }
    
    /// <summary>
    /// The Delete scenario
    /// </summary>
    public class IncorrectEndpointWithDeleteReturnsProperMessage : 
        AppInfoControllerScenario
    {
        Mock<IEnumService> EnumService      { get; set; }
        Mock<IBusControl> BusControl        { get; set; }
        JObject Resource                    { get; set; }
        new Random Random                       { get; set; }
        
        protected override void PreSetup()
        {
            EnumService     = new Mock<IEnumService>();
            BusControl      = new Mock<IBusControl>();
            Random          = new Random();
        }

        protected override void PostSetup()
        {
            Container.Inject<IEnumService>(EnumService.Object);
            Container.Inject<IBusControl>(BusControl.Object);
            Container.Inject<IConfigurationManager>(new ConfigurationManager());
        }

        public void GivenIHitANonExistentUrl()
        {
            var length = Random.Next(3, 20);
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789".ToCharArray();
            var randomString = new StringBuilder();
            for(int i = 0; i < length; i++)
                randomString.Append(chars[Random.Next(chars.Length - 1)]);
            
            Url = "/api/v1.0/" + randomString;
        }

        public async Task WhenICallTheEndpoint()
        {
            Result = await HttpServer.CreateRequest(Url)
                .SendAsync(HttpVerbs.Delete.ToString().ToUpper());
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetA404()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        public void AndThenIGetTheProperErrorMessage()
        {
            ResponseContent.Should().Be("\"Resource not found\"");
        }
    }

    #endregion
}
