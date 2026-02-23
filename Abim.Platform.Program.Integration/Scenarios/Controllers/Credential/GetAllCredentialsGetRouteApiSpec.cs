using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Credential.Base;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Credential
{
    /// <summary>
    /// Integration test class
    /// </summary>
    /// <remarks>
    ///  <seealso cref="http://stackoverflow.com/questions/7366495/use-for-workitemattribute">TFS Work Item Association</seealso>
    /// </remarks>
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Credential Api",
        SoThat = "to get all Credentials using the HTTP GET method"
        )]
    [TestFixture]
    public class GetAllCredentialsGetRouteApiSpec
    {
        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRouteReturnsOK()
        {
            new GetAllCredentialsReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]

        public void GetAllCredentialsGetRouteReturnsForbidden()
        {
            new GetAllCredentialsReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRoutePageDefinition()
        {
            new GetAllCredentialsGetRoutePageDefinition().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRoutePageDefinitionWithAscendingSorting()
        {
            new GetAllCredentialsGetRoutePageDefinitionWithAscendingSorting().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRoutePageDefinitionWithDescendingSorting()
        {
            new GetAllCredentialsGetRoutePageDefinitionWithDescendingSorting().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRouteZeroPageIndexReturnBadRequest()
        {
            new GetAllCredentialsGetRouteZeroPageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRouteNegativePageIndexReturnBadRequest()
        {
            new GetAllCredentialsGetRouteNegativePageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRouteExcessivePageIndexReturnBadRequest()
        {
            new GetAllCredentialsGetRouteExcessivePageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllCredentialsGetRouteExcessivePageSizeReturnBadRequest()
        {
            new GetAllCredentialsGetRouteExcessivePageSizeReturnBadRequest().BDDfy();
        }

        [TestCase]
        
        public void GetAllCredentialsGetRouteAnonymousReturnsUnauthorized()
        {
            new GetAllCredentialsGetRouteAnonymousReturnsUnauthorized().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    public abstract class GetAllCredentialsGetRouteScenario
        : CredentialControllerScenario
    {
        protected App.Domain.Credential CreateCredential()
        {
            var source = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var certification = App.Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                RandomString.Build(), RandomString.Build(), RandomString.Build());
            var domainObject = App.Domain.Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());
            return domainObject;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetAllCredentialsReturnsOk : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
       
        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credentials";
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResourceCollectionShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenEachResourceShouldHaveLinks()
        {
            foreach(var item in Resource.Data)
            {
                item.Links.Should().NotBeNull();
                item.Links.Should().Contain(link => link.Name == "self");
            }
        }
    }


    public class GetAllCredentialsReturnsForbidden :
    GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }

        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            OverrideScope();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Credentials";
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The basic paging scenario. Paging is done in the service but we need to ensure that the controller isn't adversely affecting it
    /// </summary>
    public class GetAllCredentialsGetRoutePageDefinition : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPagedQueryString()
        {
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(pageIndex)
                               .WithPageSize(pageSize)
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResponseIsAResourceCollection()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceCollectionShouldHaveTheRightNumberOfItems()
        {
            Resource.Data.Count.Should().Be(pageSize);
        }

        public void AndThenMyItemsShouldStartAtTheRightIndex()
        {
            Resource.Data.First().CertificationName.Should().NotBeNull();
            Resource.Data.First().CertificationName.Should().Be((startNumber + ((pageIndex - 1) * pageSize)).ToString().PadLeft(5, '0'));
        }

        public void AndThenMyResourceCollectionShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenEachResourceShouldHaveLinks()
        {
            foreach(var item in Resource.Data)
            {
                item.Links.Should().NotBeNull();
                item.Links.Should().Contain(link => link.Name == "self");
            }
        }
    }

    /// <summary>
    /// The ascending sorting scenario. Sorting is done in the service but we need to ensure that the controller isn't adversely affecting it
    /// </summary>
    public class GetAllCredentialsGetRoutePageDefinitionWithAscendingSorting : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }

        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }

            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.OrderBy(a => a.Certification.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPagedAndSortedQueryString()
        {
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(pageIndex)
                               .WithPageSize(pageSize)
                               .WithSort("CertificationName", "Ascending")
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResponseIsAResourceCollection()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceCollectionShouldHaveTheRightNumberOfItems()
        {
            Resource.Data.Count.Should().Be(pageSize);
        }

        public void AndThenMyItemsShouldStartAtTheRightIndex()
        {
            Resource.Data.First().CertificationName.Should().NotBeNull();
            Resource.Data.First().CertificationName.Should().Be((startNumber + ((pageIndex - 1) * pageSize)).ToString().PadLeft(5, '0'));
        }

        public void AndThenMyResourceCollectionShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenEachResourceShouldHaveLinks()
        {
            foreach(var item in Resource.Data)
            {
                item.Links.Should().NotBeNull();
                item.Links.Should().Contain(link => link.Name == "self");
            }
        }

        public void AndThenMyResourceCollectionShouldBeSortedByTheSuppliedField()
        {
            CredentialSummaryResource previousItem = null;
            bool sorted = true;
            foreach(var item in Resource.Data)
            {
                if(previousItem != null && int.Parse(item.CertificationName.TrimStart('0')) > int.Parse(previousItem.CertificationName.TrimStart('0')))
                {
                    sorted = false;
                    break;
                }
            }
            sorted.Should().Be(true);
        }
    }

    /// <summary>
    /// The descending sorting scenario. Sorting is done in the service but we need to ensure that the controller isn't adversely affecting it
    /// </summary>
    public class GetAllCredentialsGetRoutePageDefinitionWithDescendingSorting : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }

            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.OrderBy(a => a.Certification.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPagedAndSortedQueryString()
        {
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(pageIndex)
                               .WithPageSize(pageSize)
                               .WithSort("CertificationName", "Descending")
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetAnOkResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        public void AndThenMyResponseIsAResourceCollection()
        {
            Resource.Should().NotBeNull();
        }

        public void AndThenMyResourceCollectionShouldHaveTheRightNumberOfItems()
        {
            Resource.Data.Count.Should().Be(pageSize);
        }

        public void AndThenMyItemsShouldStartAtTheRightIndex()
        {
            Resource.Data.First().CertificationName.Should().NotBeNull();
            Resource.Data.First().CertificationName.Should().Be((startNumber + ((pageIndex - 1) * pageSize)).ToString().PadLeft(5, '0'));
        }

        public void AndThenMyResourceCollectionShouldHaveLinks()
        {
            Resource.Links.Should().NotBeNull();
            Resource.Links.Should().Contain(link => link.Name == "self");
        }

        public void AndThenEachResourceShouldHaveLinks()
        {
            foreach(var item in Resource.Data)
            {
                item.Links.Should().NotBeNull();
                item.Links.Should().Contain(link => link.Name == "self");
            }
        }

        public void AndThenMyResourceCollectionShouldBeSortedByTheSuppliedField()
        {
            CredentialSummaryResource previousItem = null;
            bool sorted = true;
            foreach(var item in Resource.Data)
            {
                if(previousItem != null && int.Parse(item.CertificationName.TrimStart('0')) < int.Parse(previousItem.CertificationName.TrimStart('0')))
                {
                    sorted = false;
                    break;
                }
            }
            sorted.Should().Be(true);
        }
    }
    
    /// <summary>
    /// The zero page index scenario
    /// </summary>
    public class GetAllCredentialsGetRouteZeroPageIndexReturnBadRequest : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassAZeroPageIndex()
        {
            var legalPageSize = new Random().Next(1, 10);
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(0)
                               .WithPageSize(legalPageSize)
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllCredentialsGetRouteNegativePageIndexReturnBadRequest : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassANegativePageIndex()
        {
            var negativePageIndex = new Random().Next(int.MinValue, -1);
            var legalPageSize = new Random().Next(1, 10);
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(negativePageIndex)
                               .WithPageSize(legalPageSize)
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllCredentialsGetRouteExcessivePageIndexReturnBadRequest : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassAnExcessivePageIndex()
        {
            var legalPageSize = new Random().Next(1, 10);
            var excessivePageIndex = new Random().Next((int)(Math.Ceiling(totalNumber / (float)legalPageSize)) + 1, int.MaxValue);
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(excessivePageIndex)
                               .WithPageSize(legalPageSize)
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void AndThenTheErrorMessageShouldBeCorrect()
        {
            ResponseContent.Should().Contain("PageIndex too high");
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllCredentialsGetRouteExcessivePageSizeReturnBadRequest : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCredential();
                domainObject.Certification.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassAnExcessivePageSize()
        {
            var legalPageIndex = new Random().Next(1, totalNumber);
            var excessivePageSize = new Random().Next(PageDefinition.MaxPageSize + 1, int.MaxValue);
            var queryString = PageDefinitionBuilder
                               .WithPageIndex(legalPageIndex)
                               .WithPageSize(excessivePageSize)
                               .BuildComplexQueryQueryString("CredentialComplexQuery.PageDefinition");
            Url = string.Format("/api/v1.0/Credentials?{0}", queryString);
        }

        public async Task WhenICallGetAllCredentials()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .GetAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CredentialCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        public void AndThenTheErrorMessageShouldBeCorrect()
        {
            ResponseContent.Should().Contain("Invalid PageSize");
        }
    }
    
    /// <summary>
    /// The anonymous call scenario
    /// </summary>
    public class GetAllCredentialsGetRouteAnonymousReturnsUnauthorized : 
        GetAllCredentialsGetRouteScenario
    {
        CredentialCollectionResource Resource { get; set; }
        List<App.Domain.Credential> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Credential>();
            EmailBuilder = new EmailBuilder();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ICredentialService>()
                .Setup(o => o.Search(It.IsAny<ComplexQueryBase>(), out total))
                .Returns(MockList.AsQueryable());
        }
        
        public void GivenIGoToTheUrlWithNoToken()
        {
            Url = "/api/v1.0/Credentials";
        }

        public async Task WhenICallGetAllCredentials()
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
