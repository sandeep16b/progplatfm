using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Responses;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Certification
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Certification api",
        SoThat = "to get a list of the available values for CertificationTypes."
        )]
    [TestFixture]
    public class GetCertificationTypeValuesApiSpec
    {
        [TestCase]
        [WorkItem(86386)]
        public void GetCertificationTypeValuesReturnsOk()
        {
            new GetCertificationTypeValuesReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(86386)]
        public void GetCertificationTypeValuesReturnsForbidden()
        {
            new GetCertificationTypeValuesReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(86386)]
        public void GetCertificationTypeValuesAnonymousReturnsUnauthorized()
        {
            new GetCertificationTypeValuesAnonymousReturnsUnauthorized().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.Base.CertificationControllerScenario" />
    public abstract class GetCertificationTypesScenario : CertificationControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ICertificationService));
            list.Add(typeof(ICredentialService));
            //list.Add(typeof(ICertificationRepository));
            //list.Add(typeof(IBusControl));
            //list.Add(typeof(IValidationFactory));
            return list;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetCertificationTypeValuesReturnsOk : 
        GetCertificationTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<CertificationType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Certification/enum/CertificationTypes";
        }

        public async Task WhenICallGetAllCertificationTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<CertificationType>>(ResponseContent);
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
            foreach(var enumValue in Enum.GetValues(typeof(CertificationType)))
            {
                Resource.Values.Should().Contain(val => val.Name == EnumAttributes.ReadEnumName(enumValue));
            }
        }
    }

    public class GetCertificationTypeValuesReturnsForbidden :
    GetCertificationTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<CertificationType> Resource { get; set; }

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
            Url = "/api/v1.0/Certification/enum/CertificationTypes";
        }

        public async Task WhenICallGetAllCertificationTypeValues()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<CertificationType>>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    public class GetCertificationTypeValuesAnonymousReturnsUnauthorized
        : GetCertificationTypesScenario
    {
        IEnumService EnumService { get; set; }
        EnumTypeResponseResource<CertificationType> Resource { get; set; }
       
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
            Url = "/api/v1.0/Certification/enum/CertificationTypes";
        }

        public async Task WhenICallGetAllCertificationTypeValuesWithoutAToken()
        {
            Result = await HttpServer.CreateRequest(Url)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<EnumTypeResponseResource<CertificationType>>(ResponseContent);
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
