using Microsoft.Owin;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace Abim.Platform.Program.Tests.Setup.OtherBuilders
{
    public class HttpRequestMessageBuilder
    {
        private HttpRequestMessage _requestMsg;

        public HttpRequestMessageBuilder()
        {
            _requestMsg = new HttpRequestMessage()
            {
                Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
            };
        }

        public HttpRequestMessageBuilder WithAuthorizationHeader(string token)
        {
            _requestMsg.Headers.Add("Authorization", "Bearer " + token);
            return this;
        }

        public HttpRequestMessageBuilder WithOwinContext(IOwinContext context)
        {
            _requestMsg.Properties.Add("MS_OwinContext", context);
            return this;
        }

        public HttpRequestMessage Build()
        {
            return _requestMsg;
        }
    }
}
