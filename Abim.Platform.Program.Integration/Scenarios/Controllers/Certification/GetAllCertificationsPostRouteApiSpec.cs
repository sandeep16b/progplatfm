

using Abim.Platform.Program.Relational;
 
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Certification.Base;
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
using System.Net.Http.Formatting;
using System.Net.Http;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Certification
{
    /// <summary>
    /// Integration test class
    /// </summary>
    /// <remarks>
    ///  <seealso cref="http://stackoverflow.com/questions/7366495/use-for-workitemattribute">TFS Work Item Association</seealso>
    /// </remarks>
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Certification Api",
        SoThat = "to get all Certifications using the HTTP GET method"
        )]
    [TestFixture]
    public class GetAllCertificationsPostRouteApiSpec
    {
        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsViaPostReturnsOk()
        {
            new GetAllCertificationsViaPostReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsViaPostReturnsForbidden()
        {
            new GetAllCertificationsViaPostReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRoutePageDefinition()
        {
            new GetAllCertificationsPostRoutePageDefinition().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRoutePageDefinitionWithAscendingSorting()
        {
            new GetAllCertificationsPostRoutePageDefinitionWithAscendingSorting().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRoutePageDefinitionWithDescendingSorting()
        {
            new GetAllCertificationsPostRoutePageDefinitionWithDescendingSorting().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRouteZeroPageIndexReturnBadRequest()
        {
            new GetAllCertificationsPostRouteZeroPageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRouteNegativePageIndexReturnBadRequest()
        {
            new GetAllCertificationsPostRouteNegativePageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRouteExcessivePageIndexReturnBadRequest()
        {
            new GetAllCertificationsPostRouteExcessivePageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRouteExcessivePageSizeReturnBadRequest()
        {
            new GetAllCertificationsPostRouteExcessivePageSizeReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllCertificationsPostRouteAnonymousReturnsUnauthorized()
        {
            new GetAllCertificationsPostRouteAnonymousReturnsUnauthorized().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    public abstract class GetAllCertificationsPostRouteScenario
        : CertificationControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ICertificationRepository));
            list.Add(typeof(ICertificationService));
            list.Add(typeof(ICredentialService));
            list.Add(typeof(ISourceService));
            list.Add(typeof(IEnumService));
            return list;
        }
        protected App.Domain.Certification CreateCertification()
        {
            var source = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var domainObject = App.Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                RandomString.Build(), RandomString.Build(), RandomString.Build());
            return domainObject;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetAllCertificationsViaPostReturnsOk : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
       
        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
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

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetAllCertificationsViaPostReturnsForbidden :
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }

        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();            
            OverrideScope();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }

    }

    /// <summary>
    /// The basic paging scenario. Paging is done in the service but we need to ensure that the controller isn't adversely affecting it
    /// </summary>
    public class GetAllCertificationsPostRoutePageDefinition : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPageDefinitionBody()
        {
            Body = PageDefinitionBuilder
                    .WithPageIndex(pageIndex)
                    .WithPageSize(pageSize)
                    .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
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
            Resource.Data.First().Name.Should().NotBeNull();
            Resource.Data.First().Name.Should().Be((startNumber + ((pageIndex - 1) * pageSize)).ToString().PadLeft(5, '0'));
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
    public class GetAllCertificationsPostRoutePageDefinitionWithAscendingSorting : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.OrderBy(a => a.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPagedAndSortedQueryString()
        {
            Body = PageDefinitionBuilder
                    .WithPageIndex(pageIndex)
                    .WithPageSize(pageSize)
                    .WithSort("Name", "Ascending")
                    .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
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
            Resource.Data.First().Name.Should().NotBeNull();
            Resource.Data.First().Name.Should().Be((startNumber + ((pageIndex - 1) * pageSize)).ToString().PadLeft(5, '0'));
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
            CertificationSummaryResource previousItem = null;
            bool sorted = true;
            foreach(var item in Resource.Data)
            {
                if(previousItem != null && int.Parse(item.Name.TrimStart('0')) > int.Parse(previousItem.Name.TrimStart('0')))
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
    public class GetAllCertificationsPostRoutePageDefinitionWithDescendingSorting : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.OrderBy(a => a.Name).Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPagedAndSortedQueryString()
        {
            Body = PageDefinitionBuilder
                    .WithPageIndex(pageIndex)
                    .WithPageSize(pageSize)
                    .WithSort("Name", "Descending")
                    .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
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
            Resource.Data.First().Name.Should().NotBeNull();
            Resource.Data.First().Name.Should().Be((startNumber + ((pageIndex - 1) * pageSize)).ToString().PadLeft(5, '0'));
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
            CertificationSummaryResource previousItem = null;
            bool sorted = true;
            foreach(var item in Resource.Data)
            {
                if(previousItem != null && int.Parse(item.Name.TrimStart('0')) < int.Parse(previousItem.Name.TrimStart('0')))
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
    public class GetAllCertificationsPostRouteZeroPageIndexReturnBadRequest : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassAZeroPageIndex()
        {
            var legalPageSize = new Random().Next(1, 10);
            Body = PageDefinitionBuilder
                    .WithPageIndex(0)
                    .WithPageSize(legalPageSize)
                    .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllCertificationsPostRouteNegativePageIndexReturnBadRequest : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassANegativePageIndex()
        {
            var negativePageIndex = new Random().Next(int.MinValue, -1);
            var legalPageSize = new Random().Next(1, 10);
            Body = PageDefinitionBuilder
                    .WithPageIndex(negativePageIndex)
                    .WithPageSize(legalPageSize)
                    .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllCertificationsPostRouteExcessivePageIndexReturnBadRequest : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassAnExcessivePageIndex()
        {
            var legalPageSize = new Random().Next(1, 10);
            var excessivePageIndex = new Random().Next((int)(Math.Ceiling(totalNumber / (float)legalPageSize)) + 1, int.MaxValue);
            Body = PageDefinitionBuilder
                .WithPageIndex(excessivePageIndex)
                .WithPageSize(legalPageSize)
                .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
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
    public class GetAllCertificationsPostRouteExcessivePageSizeReturnBadRequest : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateCertification();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassAnExcessivePageSize()
        {
            var legalPageIndex = new Random().Next(1, totalNumber);
            var excessivePageSize = new Random().Next(PageDefinition.MaxPageSize + 1, int.MaxValue);
            Body = PageDefinitionBuilder
                    .WithPageIndex(legalPageIndex)
                    .WithPageSize(excessivePageSize)
                    .BuildObject();
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<CertificationCollectionResource>(ResponseContent);
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
    public class GetAllCertificationsPostRouteAnonymousReturnsUnauthorized : 
        GetAllCertificationsPostRouteScenario
    {
        CertificationCollectionResource Resource { get; set; }
        List<App.Domain.Certification> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Certification>();
            EmailBuilder = new EmailBuilder();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ICertificationService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }
        
        public void GivenIGoToTheUrlWithNoToken()
        {
            Url = "/api/v1.0/Certifications";
        }

        public async Task WhenICallGetAllCertifications()
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
