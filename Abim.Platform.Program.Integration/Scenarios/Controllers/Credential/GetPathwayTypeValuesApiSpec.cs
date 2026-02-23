using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.WebApi.Objects;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Responses;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential api",
        SoThat = "to get a list of the available values for PathwayTypes."
        )]
    [TestFixture]
    public class GetPathwayTypeValuesApiSpec
    {
        [TestCase]
        [WorkItem(78851)]
        [WorkItem(189648)]
        public void GetPathwayTypeValuesReturnsOk()
        {
            new GetPathwayTypeValuesReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(78851)]
        public void GetPathwayTypeValuesReturnsForbidden()
        {
            new GetPathwayTypeValuesReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(78851)]
        public void GetPathwayTypeValuesAnonymousReturnsUnauthorized()
        {
            new GetPathwayTypeValuesAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base.CredentialControllerScenario" />
    public abstract class GetPathwayTypesScenario : CredentialControllerScenario
    {
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetPathwayTypeValuesReturnsOk : 
        GetPathwayTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<PathwayType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/enum/PathwayTypes";
        }

        public async Task WhenICallGetAllPathwayTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<PathwayType>>(ResponseContent);
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
            foreach(var enumValue in Enum.GetValues(typeof(PathwayType)))
            {
                Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(enumValue));
            }
        }
        public void AndThenMyResourceCollectionShouldHave_LNG_Value()
        {
            Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(PathwayType.LNG));
        }
    }

    public class GetPathwayTypeValuesReturnsForbidden :
        GetPathwayTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<PathwayType> Resource { get; set; }

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
            Url = "/api/v1.0/Credential/enum/PathwayTypes";
        }

        public async Task WhenICallGetAllPathwayTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<PathwayType>>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    public class GetPathwayTypeValuesAnonymousReturnsUnauthorized
        : GetPathwayTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<PathwayType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Credential/enum/PathwayTypes";
        }

        public async Task WhenICallGetAllPathwayTypeValuesWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<PathwayType>>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
