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
        SoThat = "to get a list of the available values for MaintenanceRequirementTypes."
        )]
    [TestFixture]
    public class GetMaintenanceRequirementTypeValuesApiSpec
    {
        [TestCase]
        [WorkItem(78853)]
        public void GetMaintenanceRequirementTypeValuesReturnsOk()
        {
            new GetMaintenanceRequirementTypeValuesReturnsOk().BDDfy();
        }


        [TestCase]
        [WorkItem(78853)]
        public void GetMaintenanceRequirementTypeValuesReturnsForbidden()
        {
            new GetMaintenanceRequirementTypeValuesReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78853)]
        public void GetMaintenanceRequirementTypeValuesAnonymousReturnsUnauthorized()
        {
            new GetMaintenanceRequirementTypeValuesAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
    public abstract class GetMaintenanceRequirementTypesScenario : CredentialControllerScenario
    {
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetMaintenanceRequirementTypeValuesReturnsOk : 
        GetMaintenanceRequirementTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<MaintenanceRequirementType> Resource { get; set; }
       
        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/Issuance/enum/MaintenanceRequirementTypes";
        }

        public async Task WhenICallGetAllMaintenanceRequirementTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<MaintenanceRequirementType>>(ResponseContent);
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
            foreach(var enumValue in Enum.GetValues(typeof(MaintenanceRequirementType)))
            {
                Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(enumValue));
            }
        }
    }

    public class GetMaintenanceRequirementTypeValuesReturnsForbidden :
       GetMaintenanceRequirementTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<MaintenanceRequirementType> Resource { get; set; }

        protected override void PreSetup()
        {
            EnumService = new EnumService();
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/Issuance/enum/MaintenanceRequirementTypes";
        }

        public async Task WhenICallGetAllMaintenanceRequirementTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<MaintenanceRequirementType>>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }

    public class GetMaintenanceRequirementTypeValuesAnonymousReturnsUnauthorized
        : GetMaintenanceRequirementTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<MaintenanceRequirementType> Resource { get; set; }
       
        protected override void PreSetup()
        {
            EnumService = new EnumService();
        }

        protected override void PostSetup()
        {
            Container.Inject(EnumService);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credential/Issuance/enum/MaintenanceRequirementTypes";
        }

        public async Task WhenICallGetAllMaintenanceRequirementTypeValuesWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<MaintenanceRequirementType>>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
