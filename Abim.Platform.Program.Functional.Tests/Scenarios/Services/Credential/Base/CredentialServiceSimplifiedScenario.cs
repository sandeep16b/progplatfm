using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.ServiceBus.Program;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using MassTransit;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base
{
    public abstract class CredentialServiceSimplifiedScenario
    {
        protected ICredentialService _sut;
        protected Mock<ICredentialRepository> _credRepoMock;
        protected Mock<ICertificationService> _certSvcMock;
        protected Mock<ISourceService> _sourceSvcMock;
        protected Mock<IHelperService> _helperSvcMock;
        protected Mock<ICredentialService> _credSvcMock;
        protected Mock<IProfileInterservice> _profileInterServiceMock;
        protected Mock<IBusControl> _busControlMock;
        protected Mock<IBackgroundJobClient> _jobClientMock;
        protected Mock<IValidationFactory> _validationFactoryMock;
        protected Mock<UpdateCredentialFromLookbackCommandValidator> _updateCredentialFromLookbackCommandValidatorMock;
        protected Mock<UpdateCredentialFromObjectCommandValidator> _updateCredentialFromObjectCommandValidatorMock;

        protected Exception _caughtException;
        protected List<App.Domain.Credential> _credentials;
        protected ExamType mostRecentExamType = ExamType.Moc;

        protected virtual void SetupMocks()
        {
            SetupCredentialRepositoryMock();
            SetupCertificationServiceMock();
            SetupSourceServiceMock();
            SetupHelperServiceMock();
            SetupBusControlMock();
            SetupBackgroundJobClientMock();
            SetupValidationFactoryMock();
            SetupCredentialServiceMock();
        }

        protected virtual void SetupCredentialRepositoryMock()
        {
            _credentials = new List<App.Domain.Credential>(1);
            var credential = CredentialBuilder.Build();
            credential.AddIssuance(IssuanceBuilder.Build());
            _credentials.Add(credential);

            _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

            _credRepoMock
                .Setup(x => x.Load(It.IsAny<Guid>()))
                .Returns(credential);

            _credRepoMock
                 .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                 .Returns(credential);

            _credRepoMock
                .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                .Returns(new AbimValidationResult { Succeeded = true });

            _credRepoMock
                .SetupGet(x => x.CommitEachCallInItsOwnTransaction)
                .Returns(true);

            _credRepoMock
                .Setup(x => x.CommitTransaction());
        }

        protected virtual void SetupCertificationServiceMock()
        {
            _certSvcMock = new Mock<ICertificationService>(MockBehavior.Strict);
        }

        protected virtual void SetupCredentialServiceMock()
        {
            _credSvcMock = new Mock<ICredentialService>(MockBehavior.Strict);
        }

        protected virtual void SetupProfileInterServiceMock()
        {
            _profileInterServiceMock = new Mock<IProfileInterservice>(MockBehavior.Strict);
        }

        protected virtual void SetupSourceServiceMock()
        {
            _sourceSvcMock = new Mock<ISourceService>(MockBehavior.Strict);
        }

        protected virtual void SetupHelperServiceMock()
        {
            _helperSvcMock = new Mock<IHelperService>(MockBehavior.Strict);

            _helperSvcMock.Setup(x => x.GetMemberIdByAbimId(It.IsAny<string>()))
                .Returns(Task.FromResult(Guid.NewGuid()));

            _helperSvcMock.Setup(x => x.GetMostRecentExamTypeByCode(It.IsAny<Guid>(),It.IsAny<Guid>()))
                .Returns(Task.FromResult(mostRecentExamType));
        }

        protected virtual void SetupBusControlMock()
        {
            _busControlMock = new Mock<IBusControl>(MockBehavior.Strict);
            _busControlMock
                .Setup(x => x.Publish(It.IsAny<IssuanceChangedEvent>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(false));
        }

        protected virtual void SetupBackgroundJobClientMock()
        {
            _jobClientMock = new Mock<IBackgroundJobClient>(MockBehavior.Strict);
        }

        protected virtual void SetupValidationFactoryMock()
        {
            var validatorMock = new Mock<IValidator<UpdateCredentialFromLookbackCommand>>();
            validatorMock
                .Setup(x => x.Validate(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                .Returns(new ValidationResult());

            var validatorFromObjectMock = new Mock<IValidator<UpdateCredentialFromObjectCommand>>();
            validatorFromObjectMock
                .Setup(x => x.Validate(It.IsAny<UpdateCredentialFromObjectCommand>()))
                .Returns(new ValidationResult());

            _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
            _validationFactoryMock
                .Setup(x => x.GetValidatorInstance<UpdateCredentialFromLookbackCommand>())
                .Returns(validatorMock.Object);

            _validationFactoryMock
                .Setup(x => x.GetValidatorInstance<UpdateCredentialFromObjectCommand>())
                .Returns(validatorFromObjectMock.Object);
        }

        protected virtual void Setup()
        {
            SetupMocks();
            _sut = new App.Services.Impl.CredentialService(
                _credRepoMock.Object, 
                _certSvcMock.Object, 
                _sourceSvcMock.Object, 
                _helperSvcMock.Object, 
                _busControlMock.Object, 
                _jobClientMock.Object, 
                _validationFactoryMock.Object);
        }

        protected App.Domain.Credential ConstructDomainObject()
        {
            var source = App.Domain.Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var certification = App.Domain.Certification.Create(null, source, EnumAttributes.RandomEntry<CertificationType>(),
                RandomString.Build(), RandomString.Build(), RandomString.Build());
            var credential = App.Domain.Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(),
                EnumAttributes.RandomEntry<Resources.PathwayType>(), null, null, RandomString.Build());

            return credential;
        }
    }
}
