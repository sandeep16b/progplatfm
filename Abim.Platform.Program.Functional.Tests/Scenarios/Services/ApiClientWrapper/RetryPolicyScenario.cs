using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Moq;
using System; 
using NLog;
using Abim.Platform.Program.Core.Identity;
using System.Net.Http;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ApiClientWrapper
{
    public abstract class RetryPolicyScenario : BaseServiceScenario
    {
        Mock<ILogger> Log { get; set; }
        public string BaseUrl { get; set; }
        public string EndpointUrl { get; set; }

        protected Mock<IHttpClientFactory> httpClientFactoryMock { get; set; }

        protected IMembershipClientService profileClientWrapperService { get; set; }
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
        }

        protected override void PostSetup()
        {
            var accessTokenSvcMock = new Mock<IAccessTokenService>();

            var urlBuilder = new System.Text.StringBuilder();

            httpClientFactoryMock = new Mock<IHttpClientFactory>();
            urlBuilder.Append(BaseUrl != null ? BaseUrl.TrimEnd('/') : "").Append(EndpointUrl);

            accessTokenSvcMock.Setup(mock => mock.GetAccessToken()).Returns("testToken");

            httpClientFactoryMock
                .Setup(mock => mock.CreateClient("HttpClientFactory"))
                .Returns(() =>
                {
                    var fakeResponseHandler = new FakeResponseHandler();
                    {
                        var fakeResponseMsg = new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
                        {
                            var json = new StringContent("{}");
                            {
                                var testClient = new HttpClient(fakeResponseHandler);
                                var uri = new Uri(urlBuilder.ToString());
                                fakeResponseMsg.Content = json;
                                fakeResponseHandler.AddFakeResponse(uri, fakeResponseMsg);
                                testClient.BaseAddress = uri;
                                return testClient;
                            }
                        }
                    }

                });

            profileClientWrapperService = new MembershipClientService(accessTokenSvcMock.Object, httpClientFactoryMock.Object, new ApiClientFactory(), "https://testapi", 3);

            LogTest.Watch(Log);
        }
    }
}
