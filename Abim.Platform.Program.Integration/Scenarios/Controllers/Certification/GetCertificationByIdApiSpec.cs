using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.Base;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Certification
{
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Certification Api",
        SoThat = "to get a single Certification's details"
        )]
    [TestFixture]
    public class GetCertificationByIdApiSpec 
    {
        [TestCase]
        [WorkItem(74793)]
        [WorkItem(327905)]
        public void GetCertificationReturnsOK()
        {
            new GetCertificationReturnsOK().BDDfy();
        }

        [TestCase]
        [WorkItem(327905)]
        public void GetCertificationReturnsCorrectFieldValues()
        {
            new GetCertificationReturnsCorrectFieldValues().BDDfy();
        }

        [TestCase]
        [WorkItem(327905)]
        public void GetCertificationReturnsOtherCorrectFieldValues()
        {
            new GetCertificationReturnsOtherCorrectFieldValues().BDDfy();
        }

        [TestCase]
        [WorkItem(74793)]
        public void GetCertificationReturnsForbidden()
        {
            new GetCertificationReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(74793)]
        public void GetCertificationInvalidIdReturnsNotFound()
        {
            new GetCertificationInvalidIdReturnsNotFound().BDDfy();
        }

        [TestCase]
        [WorkItem(74793)]
        public void GetCertificationNonGuidIdReturnsProperMessage()
        {
            new GetCertificationNonGuidIdReturnsProperMessage().BDDfy();
        }

        [TestCase]
        [WorkItem(74793)]
        public void GetCertificationAnonymousReturnsUnauthorized()
        {
            new GetCertificationAnonymousReturnsUnauthorized().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class for this file
    /// </summary>
    public abstract class GetCertificationByIdScenario : CertificationControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(IEnumService));
            list.Add(typeof(ICredentialService));
            list.Add(typeof(ICertificationService));
            return list;
        }

        protected App.Domain.Certification ConstructDomainObject()
        {
            var source = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var certification = App.Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                RandomString.Build(), RandomString.Build(), RandomString.Build());
            return certification;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get by Id scenario
    /// </summary>
    public class GetCertificationReturnsOK : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            DomainObject.IsCertificateRetired = true;
            DomainObject.CertificateRetiredDate = DateTime.Now.AddDays(-1);
            My<ICertificationService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certification/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenIsCertificateRetiredFlagShouldTrue()
        {
            Resource.IsCertificateRetired.ShouldBeTrue();
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "source");
            
            if(DomainObject.BaseCertification != null)
                Resource.Links.Should().Contain(link => link.Name == "base");
            else
                Resource.Links.Should().NotContain(link => link.Name == "base");
        }
    }

    public class GetCertificationReturnsCorrectFieldValues : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            DomainObject.IsCertificateRetired = true;
            DomainObject.CertificateRetiredDate = DateTime.Now.AddDays(1);
            My<ICertificationService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certification/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenIsCertificateRetiredFlagShouldFalse()
        {
            Resource.IsCertificateRetired.ShouldBeFalse();
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "source");

            if (DomainObject.BaseCertification != null)
                Resource.Links.Should().Contain(link => link.Name == "base");
            else
                Resource.Links.Should().NotContain(link => link.Name == "base");
        }
    }

    public class GetCertificationReturnsOtherCorrectFieldValues : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }

        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            DomainObject.IsCertificateRetired = true;
            // DomainObject.CertificateRetiredDate  is not set
            My<ICertificationService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certification/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceShouldNotBeNull()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenIsCertificateRetiredFlagShouldFalse()
        {
            Resource.IsCertificateRetired.ShouldBeFalse();
        }

        public void AndThenMyResourceShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
            Resource.Links.Should().Contain(link => link.Name == "source");

            if (DomainObject.BaseCertification != null)
                Resource.Links.Should().Contain(link => link.Name == "base");
            else
                Resource.Links.Should().NotContain(link => link.Name == "base");
        }
    }

    public class GetCertificationReturnsForbidden : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }

        protected override void PreSetup()
        {
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICertificationService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certification/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The Invalid Id scenario
    /// </summary>
    public class GetCertificationInvalidIdReturnsNotFound : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            My<ICertificationService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns<App.Domain.Certification>(null);
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certification/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
        }

        public void ThenIGetANotFoundResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }

    /// <summary>
    /// The non-Guid Id scenario
    /// </summary>
    public class GetCertificationNonGuidIdReturnsProperMessage : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
            DomainObject = ConstructDomainObject();
            My<ICertificationService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(DomainObject);
        }

        public void GivenIPassAnInvalidUrl()
        {
            Url = "/api/v1.0/Certification/" + (new Random()).Next(0, int.MaxValue);
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationResource>(ResponseContent);
        }

        public void ThenIGetAnErrorStatusCode()
        {
            Result.StatusCode.Should().NotBe(HttpStatusCode.OK);
        }

        public void AndThenIGetAProperErrorMessage()
        {
            ResponseContent.Should().Contain(ErrorMessages.InvalidGuidId);
        }
    }
    
    /// <summary>
    /// The anonymous call scenario
    /// </summary>
    public class GetCertificationAnonymousReturnsUnauthorized : GetCertificationByIdScenario
    {
        CertificationResource Resource { get; set; }
        App.Domain.Certification DomainObject { get; set; }
       
        protected override void PreSetup()
        {
        }

        protected override void PostSetup()
        {
        }
        
        public void GivenIGoToTheUrlWithNoToken()
        {
            Url = "/api/v1.0/Certification/" + Guid.NewGuid();
        }

        public async Task WhenICallGetCertification()
        {
            Result = await HttpServer.CreateRequest(Url).GetAsync();
        }

        public void ThenIGetUnauthorized()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }
    }

    #endregion
}
