using Abim.Platform.Program.WebApi.Objects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Newtonsoft.Json;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Relational.Classes;
 
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Responses;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential api",
        SoThat = "to get a list of the available values for DurationTypes."
        )]
    [TestFixture]
    public class GetDurationTypeValuesApiSpec
    {
        [TestCase]
        [WorkItem(78852)]
        public void GetDurationTypeValuesReturnsOk()
        {
            new GetDurationTypeValuesReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(78852)]
        public void GetDurationTypeValuesReturnsForbidden()
        {
            new GetDurationTypeValuesReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78852)]
        public void GetDurationTypeValuesAnonymousReturnsUnauthorized()
        {
            new GetDurationTypeValuesAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
    public abstract class GetDurationTypesScenario : CredentialControllerScenario
    {
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetDurationTypeValuesReturnsOk : 
        GetDurationTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<DurationType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/Issuance/enum/DurationTypes";
        }

        public async Task WhenICallGetAllDurationTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<DurationType>>(ResponseContent);
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

        public void AndThenMyResourceCollectionShouldHaveAllTheValues()
        {
            foreach(var enumValue in Enum.GetValues(typeof(DurationType)))
            {
                Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(enumValue));
            }
        }
    }

    public class GetDurationTypeValuesReturnsForbidden :
       GetDurationTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<DurationType> Resource { get; set; }

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
            Url = "/api/v1.0/Credential/Issuance/enum/DurationTypes";
        }

        public async Task WhenICallGetAllDurationTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<DurationType>>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    public class GetDurationTypeValuesAnonymousReturnsUnauthorized
        : GetDurationTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<DurationType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/Issuance/enum/DurationTypes";
        }

        public async Task WhenICallGetAllDurationTypeValuesWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<DurationType>>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
