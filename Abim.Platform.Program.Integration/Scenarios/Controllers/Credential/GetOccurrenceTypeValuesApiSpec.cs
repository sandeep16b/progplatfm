using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Responses;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential api",
        SoThat = "to get a list of the available values for OccurrenceTypes."
        )]
    [TestFixture]
    public class GetOccurrenceTypeValuesApiSpec
    {
        [TestCase]
        [WorkItem(86387)]
        public void GetOccurrenceTypeValuesReturnsOk()
        {
            new GetOccurrenceTypeValuesReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(86387)]
        public void GetOccurrenceTypeValuesReturnsForbidden()
        {
            new GetOccurrenceTypeValuesReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(86387)]
        public void GetOccurrenceTypeValuesAnonymousReturnsUnauthorized()
        {
            new GetOccurrenceTypeValuesAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
    public abstract class GetOccurrenceTypesScenario : CredentialControllerScenario
    {
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetOccurrenceTypeValuesReturnsOk : 
        GetOccurrenceTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<OccurrenceType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/Issuance/enum/OccurrenceTypes";
        }

        public async Task WhenICallGetAllOccurrenceTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<OccurrenceType>>(ResponseContent);
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
            foreach(var enumValue in Enum.GetValues(typeof(OccurrenceType)))
            {
                Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(enumValue));
            }
        }
    }

    public class GetOccurrenceTypeValuesReturnsForbidden :
       GetOccurrenceTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<OccurrenceType> Resource { get; set; }

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
            Url = "/api/v1.0/Credential/Issuance/enum/OccurrenceTypes";
        }

        public async Task WhenICallGetAllOccurrenceTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<OccurrenceType>>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }

    public class GetOccurrenceTypeValuesAnonymousReturnsUnauthorized
        : GetOccurrenceTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<OccurrenceType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/Issuance/enum/OccurrenceTypes";
        }

        public async Task WhenICallGetAllOccurrenceTypeValuesWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<OccurrenceType>>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
