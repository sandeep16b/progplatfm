using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Hangfire;
using MassTransit;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base
{
    public abstract class ProgramRulesServiceSimplifiedScenario
    {
        protected ProgramRulesService _sut;
        protected Mock<ICertificationService> _certSvcMock;
        protected Mock<ICredentialService> _credSvcMock;
        protected Mock<ISourceService> _sourceSvcMock;
        protected Mock<IProductInterservice> _prodInterSvcMock;
        protected Mock<IRegistrationInterservice> _regInterSvcMock;
        protected Mock<IBusControl> _busControlMock;
        protected Mock<IBackgroundJobClient> _backgroundJobClientMock;
        protected Mock<IValidationFactory> _validationFactoryMock;
        protected Mock<IAccessTokenService> _accessTokenServiceMock;
        protected Mock<ICorrectiveActionResultService> _correctiveActionResultSvcMock;
        protected Mock<ILookBackDatesInfoService> _lookBackDatesInfoSvcMock;
        protected Mock<ILookbackLogService> _lookbackLogSvcMock;
        protected Exception _caughtException;

        protected virtual void SetupMocks()
        {
            SetupCertificationServiceMock();
            SetupCredentialServiceMock();
            SetupSourceServiceMock();
            SetupProductInterserviceMock();
            SetupRegistrationInterserviceMock();
            SetupBusControlMock();
            SetupBackgroundJobClientMock();
            SetupValidationFactoryMock();
            SetupAccessTokenServiceMock();
            SetupCorrectiveActionResultServiceMock();
            SetupLookBackDatesInfoServiceMock();
            SetupLookbackLogServiceMock();
        }

        protected virtual void SetupCertificationServiceMock()
        {
            _certSvcMock = new Mock<ICertificationService>(MockBehavior.Strict);
        }

        protected virtual void SetupCredentialServiceMock()
        {
            _credSvcMock = new Mock<ICredentialService>(MockBehavior.Strict);
        }

        protected virtual void SetupSourceServiceMock()
        {
            _sourceSvcMock = new Mock<ISourceService>(MockBehavior.Strict);
        }

        protected virtual void SetupProductInterserviceMock()
        {
            _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
        }

        protected virtual void SetupRegistrationInterserviceMock()
        {
            _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
        }

        protected virtual void SetupBusControlMock()
        {
            _busControlMock = new Mock<IBusControl>(MockBehavior.Strict);
        }

        protected virtual void SetupBackgroundJobClientMock()
        {
            _backgroundJobClientMock = new Mock<IBackgroundJobClient>(MockBehavior.Strict);
        }

        protected virtual void SetupValidationFactoryMock()
        {
            _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
        }

        protected virtual void SetupAccessTokenServiceMock()
        {
            _accessTokenServiceMock = new Mock<IAccessTokenService>();
            _accessTokenServiceMock
                .Setup(x => x.GetAccessToken())
                .Returns("--someToken--");
        }

        protected virtual void SetupCorrectiveActionResultServiceMock()
        {
            _correctiveActionResultSvcMock = new Mock<ICorrectiveActionResultService>(MockBehavior.Strict);

            _correctiveActionResultSvcMock
                .Setup(x => x.Add(It.IsAny<CorrectiveActionResult>()))
                .Returns(Task.FromResult(true));

            _correctiveActionResultSvcMock
                .Setup(x => x.Add(It.IsAny<IEnumerable<CorrectiveActionResult>>()))
                .Returns(Task.FromResult(true));
        }

        protected virtual void SetupLookBackDatesInfoServiceMock ()
        {
            _lookBackDatesInfoSvcMock= new Mock<ILookBackDatesInfoService>(MockBehavior.Strict);

            _lookBackDatesInfoSvcMock
                .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

            _lookBackDatesInfoSvcMock
                .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

            _lookBackDatesInfoSvcMock
                .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(true));
        }

        protected virtual void SetupLookbackLogServiceMock()
        {
            _lookbackLogSvcMock = new Mock<ILookbackLogService>(MockBehavior.Strict);

            _lookbackLogSvcMock
                .Setup(x => x.Handle(It.IsAny<AddCertificationLookbackLog>()))
                .Returns(new AddLookbackLogCommandResult());

            _lookbackLogSvcMock
                .Setup(x => x.Handle(It.IsAny<AddParticipationLookbackLog>()))
                .Returns(new AddLookbackLogCommandResult());
        }

        protected virtual void Setup()
        {
            SetupMocks();

            _sut = new ProgramRulesService(
                _certSvcMock.Object,
                _credSvcMock.Object,
                _sourceSvcMock.Object,
                _prodInterSvcMock.Object,
                _regInterSvcMock.Object,
                _busControlMock.Object,
                _backgroundJobClientMock.Object,
                _validationFactoryMock.Object,
                _accessTokenServiceMock.Object,
                _correctiveActionResultSvcMock.Object,
                _lookBackDatesInfoSvcMock.Object, 
                _lookbackLogSvcMock.Object);
        }
    }
}
