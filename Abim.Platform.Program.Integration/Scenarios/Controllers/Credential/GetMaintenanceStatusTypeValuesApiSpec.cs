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
        SoThat = "to get a list of the available values for MaintenanceStatusTypes."
        )]
    [TestFixture]
    public class GetMaintenanceStatusTypeValuesApiSpec
    {
        [TestCase]
        [WorkItem(78854)]
        public void GetMaintenanceStatusTypeValuesReturnsOk()
        {
            new GetMaintenanceStatusTypeValuesReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(78854)]
        public void GetMaintenanceStatusTypeValuesReturnsForbidden()
        {
            new GetMaintenanceStatusTypeValuesReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78854)]
        public void GetMaintenanceStatusTypeValuesAnonymousReturnsUnauthorized()
        {
            new GetMaintenanceStatusTypeValuesAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
    public abstract class GetMaintenanceStatusTypesScenario : CredentialControllerScenario
    {
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetMaintenanceStatusTypeValuesReturnsOk : 
        GetMaintenanceStatusTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<MaintenanceStatusType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/Issuance/enum/MaintenanceStatusTypes";
        }

        public async Task WhenICallGetAllMaintenanceStatusTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<MaintenanceStatusType>>(ResponseContent);
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
            foreach(var enumValue in Enum.GetValues(typeof(MaintenanceStatusType)))
            {
                Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(enumValue));
            }
        }
    }

    public class GetMaintenanceStatusTypeValuesReturnsForbidden :
       GetMaintenanceStatusTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<MaintenanceStatusType> Resource { get; set; }

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
            Url = "/api/v1.0/Credential/Issuance/enum/MaintenanceStatusTypes";
        }

        public async Task WhenICallGetAllMaintenanceStatusTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<MaintenanceStatusType>>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    public class GetMaintenanceStatusTypeValuesAnonymousReturnsUnauthorized
        : GetMaintenanceStatusTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<MaintenanceStatusType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/Issuance/enum/MaintenanceStatusTypes";
        }

        public async Task WhenICallGetAllMaintenanceStatusTypeValuesWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<MaintenanceStatusType>>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
