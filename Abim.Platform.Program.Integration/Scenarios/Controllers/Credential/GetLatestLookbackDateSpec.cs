using System;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using TestStack.BDDfy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Newtonsoft.Json;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential Api",
        SoThat = "to get the latest LookbackDate for a member using the HTTP GET method"
        )]
    [TestFixture]
    public class GetLatestLookbackDateSpec
    {
        [TestCase]
        [WorkItem(141274)]
        public void Should_Get_Latest_LookbackDate_And_Return_OK()
        {
            new ShouldGetLatestLookbackDateAndReturnOK().BDDfy();
        }

        #region Scenarios

        /// <summary>
        /// Base class for this file
        /// </summary>
        /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
        private abstract class GetLatestLookbackDateScenario : CredentialControllerScenario
        {
        }

        private class ShouldGetLatestLookbackDateAndReturnOK : GetLatestLookbackDateScenario
        {
            private DateTime? _latestLookbackDate = DateTime.MinValue; //Initializing it so we can be sure it changes

            protected override void PostSetup()
            {
             
            }

            protected override void PreSetup()
            {
                Scopes = "c.r";
            }

            public void GivenIPassTheCorrectUrl()
            {
                Url = "/api/v1.0/latestLookbackDate/EB7A4651-6FCA-49F3-98B3-42D45B5C09EF";
            }

            public async Task WhenICallGetLatestLookbackDate()
            {
                Result = await HttpServer.CreateRequest(Url)
                    .AddHeader("Authorization", "Bearer " + Token)
                    .GetAsync();
                ResponseContent = await Result.Content.ReadAsStringAsync();
                _latestLookbackDate = JsonConvert.DeserializeObject<DateTime?>(ResponseContent);
            }

            public void ThenIGetAnOkResponse()
            {
                Result.StatusCode.Should().Be(HttpStatusCode.OK);
            }

            public void AndTheLatestLookbackDateShouldHaveBeenReturned()
            {
                _latestLookbackDate.Should().NotBe(DateTime.MinValue);
            }

        }

        #endregion Scenarios

    }
}
