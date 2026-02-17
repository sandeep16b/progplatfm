using Microsoft.Owin;
using Microsoft.Owin.Security;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;

namespace Abim.Platform.Program.Tests.Setup.OtherBuilders
{
    public class IOwinContextMockBuilder
    {
        private Mock<IOwinContext> _owinContextMock;

        public IOwinContextMockBuilder()
        {
            _owinContextMock = new Mock<IOwinContext>();
            _owinContextMock.SetupGet(x => x.Environment).Returns(new Dictionary<string, object>(0));
            _owinContextMock.SetupGet(x => x.Request).Returns(new OwinRequest());
            _owinContextMock.SetupGet(x => x.Response).Returns(new OwinResponse());
        }

        public IOwinContextMockBuilder WithClaimsPrincipal(ClaimsPrincipal principal)
        {
            var authMgr = new Mock<IAuthenticationManager>(MockBehavior.Strict);
            authMgr.Setup(x => x.User).Returns(principal);
            _owinContextMock.SetupGet(x => x.Authentication).Returns(authMgr.Object);
            return this;
        }

        public Mock<IOwinContext> Build()
        {
            return _owinContextMock;
        }
    }
}
