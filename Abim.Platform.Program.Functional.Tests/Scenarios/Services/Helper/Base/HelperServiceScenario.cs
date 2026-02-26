using Abim.Platform.Program.MembershipClient;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Core.Identity;
using MassTransit;
using Moq;
using System; 

namespace Abim.Platform.Program.Tests.Scenarios.Services.Helper
{
    public abstract class HelperServiceScenario
    {
        protected HelperService _sut;
        protected Mock<IBusControl> _busControlMock;
        protected Mock<IAccessTokenService> _accessTokenServiceMock;
        protected Mock<IMembershipClientService> _membershipClientServiceMock;
        protected Mock<IRegistrationInterservice> _registrationInterserviceMock;
        protected Mock<ICredentialService> _credentialServiceMock;
        protected Exception _exception;
        protected Guid _profileGuid = Guid.NewGuid();

        protected virtual void Setup()
        {
            SetupBusControlMock();
            SetupAccessTokenServiceMock();
            SetupProfileMembershipMock();
            SetupRegistrationInterserviceMock();
            SetupCredentialServiceMock();
            _sut = new HelperService(
                    _busControlMock.Object, 
                    _accessTokenServiceMock.Object,
                    _membershipClientServiceMock.Object, 
                    _registrationInterserviceMock.Object);
        }

        protected virtual void SetupBusControlMock()
        {
            _busControlMock = new Mock<IBusControl>(MockBehavior.Strict);
        }

        protected virtual void SetupAccessTokenServiceMock()
        {
            _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
            _accessTokenServiceMock
                .Setup(x => x.GetAccessToken())
                .Returns("--token--");
        }

        protected virtual void SetupProfileMembershipMock()
        {
            _membershipClientServiceMock = new Mock<IMembershipClientService>(MockBehavior.Strict);

            var profile = new ProfileResource() { Id = _profileGuid };
            _membershipClientServiceMock.Setup(r => r.GetAccessToken()).Verifiable();
            _membershipClientServiceMock
                .Setup(x => x.GetProfileByAbimIdAsync(It.IsAny<string>()))
                .ReturnsAsync(profile);
        }

        protected virtual void SetupRegistrationInterserviceMock()
        {
            _registrationInterserviceMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
        }

        protected virtual void SetupCredentialServiceMock()
        {
            _credentialServiceMock = new Mock<ICredentialService>(MockBehavior.Strict);
        }
    }
}
