using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Core.Identity;
using MassTransit;
using Moq;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Helper
{
    public abstract class HelperServiceScenario
    {
        protected HelperService _sut;
        protected Mock<IBusControl> _busControlMock;
        protected Mock<IAccessTokenService> _accessTokenServiceMock;
        protected Mock<IProfileInterservice> _profileInterserviceMock;
        protected Mock<IRegistrationInterservice> _registrationInterserviceMock;
        protected Mock<ICredentialService> _credentialServiceMock;
        protected Exception _exception;
        protected Guid _profileGuid = Guid.NewGuid();

        protected virtual void Setup()
        {
            SetupBusControlMock();
            SetupAccessTokenServiceMock();
            SetupProfileInterserviceMock();
            SetupRegistrationInterserviceMock();
            SetupCredentialServiceMock();
            _sut = new HelperService(
                    _busControlMock.Object, 
                    _accessTokenServiceMock.Object, 
                    _profileInterserviceMock.Object, 
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

        protected virtual void SetupProfileInterserviceMock()
        {
            _profileInterserviceMock = new Mock<IProfileInterservice>(MockBehavior.Strict);

            var profile = new ProfileNestedResource();
            profile.Id = _profileGuid;

            _profileInterserviceMock
                .Setup(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns(Task.FromResult(profile));
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
