using Abim.Platform.Program.Relational;
using Abim.Platform.Program.WebApi.Objects;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Integration.Scenarios.Controllers.Source.Base;
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
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Integration.Scenarios.Controllers.Source
{
    /// <summary>
    /// Integration test class
    /// </summary>
    /// <remarks>
    ///  <seealso cref="http://stackoverflow.com/questions/7366495/use-for-workitemattribute">TFS Work Item Association</seealso>
    /// </remarks>
    [Story(
        AsA = "external application",
        IWant = "to be able to call the Source Api",
        SoThat = "to get all Sources using the HTTP GET method"
        )]
    [TestFixture]
    public class GetAllSourcesPostRouteApiSpec
    {
        [TestCase]
        [WorkItem(74790)]
        public void GetAllSourcesViaPostReturnsOk()
        {
            new GetAllSourcesViaPostReturnsOk().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllSourcesViaPostReturnsForbidden()
        {
            new GetAllSourcesViaPostReturnsForbidden().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllSourcesPostRoutePageDefinition()
        {
            new GetAllSourcesPostRoutePageDefinition().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllSourcesPostRoutePageDefinitionWithAscendingSorting()
        {
            new GetAllSourcesPostRoutePageDefinitionWithAscendingSorting().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllSourcesPostRoutePageDefinitionWithDescendingSorting()
        {
            new GetAllSourcesPostRoutePageDefinitionWithDescendingSorting().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllSourcesPostRouteZeroPageIndexReturnBadRequest()
        {
            new GetAllSourcesPostRouteZeroPageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllSourcesPostRouteNegativePageIndexReturnBadRequest()
        {
            new GetAllSourcesPostRouteNegativePageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllSourcesPostRouteExcessivePageIndexReturnBadRequest()
        {
            new GetAllSourcesPostRouteExcessivePageIndexReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        
        public void GetAllSourcesPostRouteExcessivePageSizeReturnBadRequest()
        {
            new GetAllSourcesPostRouteExcessivePageSizeReturnBadRequest().BDDfy();
        }

        [TestCase]
        [WorkItem(74790)]
        public void GetAllSourcesPostRouteAnonymousReturnsUnauthorized()
        {
            new GetAllSourcesPostRouteAnonymousReturnsUnauthorized().BDDfy();
        }
    }
    
    /// <summary>
    /// Base class
    /// </summary>
    public abstract class GetAllSourcesPostRouteScenario
        : SourceControllerScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var list = base.AdditionalDependencies();
            list.Add(typeof(ISourceRepository));
            list.Add(typeof(ISourceService));
            list.Add(typeof(IEnumService));
            return list;
        }
        protected App.Domain.Source CreateSource()
        {
            var domainObject = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            return domainObject;
        }
    }

    #region Scenarios

    /// <summary>
    /// The basic Get All scenario
    /// </summary>
    public class GetAllSourcesViaPostReturnsOk : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
       
        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ISourceService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
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

    public class GetAllSourcesViaPostReturnsForbidden :
       GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }

        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            OverrideScope();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ISourceService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }

        public void GivenIPassTheCorrectUrl()
        {
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
        }

        public void ThenIGetForbiddenResponse()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        }
    }

    /// <summary>
    /// The basic paging scenario. Paging is done in the service but we need to ensure that the controller isn't adversely affecting it
    /// </summary>
    public class GetAllSourcesPostRoutePageDefinition : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ISourceService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.Skip((pageIndex - 1) * pageSize).Take(pageSize).AsQueryable());
        }

        public void GivenIPassAPageDefinitionBody()
        {
            Body = PageDefinitionBuilder
                    .WithPageIndex(pageIndex)
                    .WithPageSize(pageSize)
                    .BuildObject();
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
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
    public class GetAllSourcesPostRoutePageDefinitionWithAscendingSorting : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ISourceService>()
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
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
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
            SourceSummaryResource previousItem = null;
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
    public class GetAllSourcesPostRoutePageDefinitionWithDescendingSorting : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int pageSize;
        int pageIndex;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 1000);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {
            pageSize = new Random().Next(1, 10);
            pageIndex = new Random().Next(1, (int)(Math.Ceiling(totalNumber / (float)pageSize)));
            
            var total = MockList.Count;
            My<ISourceService>()
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
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
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
            SourceSummaryResource previousItem = null;
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
    public class GetAllSourcesPostRouteZeroPageIndexReturnBadRequest : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ISourceService>()
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
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllSourcesPostRouteNegativePageIndexReturnBadRequest : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ISourceService>()
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
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
        }

        public void ThenIGetABadRequest()
        {
            Result.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
    
    /// <summary>
    /// The negative page index scenario
    /// </summary>
    public class GetAllSourcesPostRouteExcessivePageIndexReturnBadRequest : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ISourceService>()
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
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
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
    public class GetAllSourcesPostRouteExcessivePageSizeReturnBadRequest : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        int totalNumber;
        int startNumber;
        
        protected override void PreSetup()
        {
            totalNumber = new Random().Next(100, 300);
            startNumber = new Random().Next(1000, 2000);
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
            for(var i = 0; i < totalNumber; i++)
            {
                var domainObject = CreateSource();
                domainObject.Name = (startNumber + i).ToString().PadLeft(5, '0');
                MockList.Add(domainObject);
            }
            OverrideAndInjectUnacceptableScope();
        }

        protected override void PostSetup()
        {            
            var total = MockList.Count;
            My<ISourceService>()
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
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
        {
            Result = await HttpServer.CreateRequest(Url)
                .AddHeader("Authorization", "Bearer " + Token)
                .AddHeader("Content-Type", "application/json; charset=utf-8")
                .And(req => req.Content = new ObjectContent(typeof(PageDefinition), Body, new JsonMediaTypeFormatter()))
                .PostAsync();
            ResponseContent = await Result.Content.ReadAsStringAsync();
            Resource = JsonConvert.DeserializeObject<SourceCollectionResource>(ResponseContent);
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
    public class GetAllSourcesPostRouteAnonymousReturnsUnauthorized : 
        GetAllSourcesPostRouteScenario
    {
        SourceCollectionResource Resource { get; set; }
        List<App.Domain.Source> MockList { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        protected override void PreSetup()
        {
            MockList = new List<App.Domain.Source>();
            EmailBuilder = new EmailBuilder();
        }

        protected override void PostSetup()
        {
            var total = 0;
            My<ISourceService>()
                .Setup(o => o.Search(It.IsAny<PageDefinition>(), out total))
                .Returns(MockList.AsQueryable());
        }
        
        public void GivenIGoToTheUrlWithNoToken()
        {
            Url = "/api/v1.0/Sources";
        }

        public async Task WhenICallGetAllSources()
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
