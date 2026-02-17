using Abim.Platform.Program.MembershipClient;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Moq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net;
using NLog;
using Abim.Platform.Program.Core.Identity;


namespace Abim.Platform.Program.Tests.Scenarios.Services.ApiClientWrapper
{
    public abstract class MembershipClientServiceScenario : BaseServiceScenario
    {
        protected IMembershipClientService profileMembershipClientWrapperService { get; set; }

        protected ProfileResource resource { get; set; }

        protected ICollection<CountryResource> countryResource { get; set; }

        protected ICollection<ProfileListResource> resources { get; set; }

        protected ICollection<RegionResource> regionResource { get; set; }

        protected ICollection<VocProfileResource> vocProfileResource { get; set; } 
        

        Mock<ILogger> Log { get; set; }

        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {

            using (var fakeResponseHandler = new FakeResponseHandler())
            {
                using (var fakeResponseMsg = new HttpResponseMessage(HttpStatusCode.OK))
                {
                    using (var json = new StringContent("{}"))
                    {

                        var uri = new Uri("https://testapi");
                        fakeResponseMsg.Content = json;
                        fakeResponseHandler.AddFakeResponse(uri, fakeResponseMsg);

                        var testClient = new HttpClient(fakeResponseHandler);
                        testClient.BaseAddress = uri;

                        var accessTokenSvcMock = new Mock<IAccessTokenService>();
                        accessTokenSvcMock.Setup(mock => mock.GetAccessToken()).Returns("testToken");

                        var httpClientFactoryMock = new Mock<IHttpClientFactory>();

                        httpClientFactoryMock
                            .Setup(mock => mock.CreateClient("HttpClientFactory"))
                            .Returns(testClient);

                        var profileClientMock = new Mock<IClient>();
                        profileClientMock
                            .Setup(mock => mock.GetProfileByAbimIdAsync(It.IsAny<string>()))
                            .ReturnsAsync(new ProfileResource());

                        profileClientMock
                            .Setup(mock => mock.GetProfilesByAbmsIdAsync(It.IsAny<string>()))
                            .ReturnsAsync((new ProfileResource()));

                        profileClientMock
                            .Setup(mock => mock.GetProfileByIdAsync(It.IsAny<Guid>()))
                            .ReturnsAsync((new ProfileResource()));

                        var mockResources = new List<ProfileListResource> { new ProfileListResource(), new ProfileListResource() };
                        profileClientMock
                            .Setup(mock => mock.GetProfilesAsync(It.IsAny<int>(), It.IsAny<int>()))
                            .ReturnsAsync((mockResources));

                        var mockVOCResources = new List<VocProfileResource> { new VocProfileResource(), new VocProfileResource() };
                        profileClientMock
                            .Setup(mock => mock.GetVocByNpiAsync(It.IsAny<string>()))
                            .ReturnsAsync(mockVOCResources);

                        profileClientMock
                            .Setup(mock => mock.NameAllAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime?>(), It.IsAny<bool>(), It.IsAny<int>(), It.IsAny<int>()))
                            .ReturnsAsync(mockVOCResources);

                        var mockRegionResource = new List<RegionResource> { new RegionResource(), new RegionResource() };
                        profileClientMock
                            .Setup(mock => mock.GetRegionsAsync(It.IsAny<string>()))
                            .ReturnsAsync(mockRegionResource);

                        var mockCountryResource = new List<CountryResource> { new CountryResource(), new CountryResource() };
                        profileClientMock
                            .Setup(mock => mock.GetCountriesAsync())
                            .ReturnsAsync(mockCountryResource);

                        var apiClientFactoryMock = new Mock<IApiClientFactory>();
                        apiClientFactoryMock.Setup(a => a.GetMembershipClient(It.IsAny<string>(), It.IsAny<HttpClient>()))
                                            .Returns(profileClientMock.Object);

                        profileMembershipClientWrapperService = new MembershipClientService(accessTokenSvcMock.Object, httpClientFactoryMock.Object, apiClientFactoryMock.Object, "https://testapi", 3);


                    }
                }
            }

            LogTest.Watch(Log);
        }

        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(IMembershipClientService));
            types.Add(typeof(IAccessTokenService));
            types.Add(typeof(ITokenClientWraper));
            types.Add(typeof(IHttpClientFactory));
            types.Add(typeof(IApiClientFactory));

            return types;
        }
    }
}
