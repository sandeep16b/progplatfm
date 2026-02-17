using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NLog;
using NUnit.Framework;
using ServiceBus.Events;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    public class DeselectCertificateCommandHandlerSpec
    {
        [TestCase]
        [WorkItem(187664)]
        [WorkItem(322411)]
        public void DeselectCertificateCommandHandleReturnsAcceptedOnSuccess()
        {
            new DeselectCertificateCommandHandleSuccessfulScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void DeselectCertificateCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new DeselectCertificateCommandHandleFailsValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void DeselectCertificateCommandHandleReturnsRejectedWhenExceptionOccurs()
        {
            new DeselectCertificateCommandHandleReturnsRejectedWhenExceptionOccursScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(293547)]
        public void DeselectCertificateCommandHandleReturnsRejectedWhenCredentialHasAlreadyBeenDeselected()
        {
            new DeselectCertificateCommandHandleReturnsRejectedWhenCredentialAlreadyDeselectedScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void DeselectCertificateCommandHandlerShouldNotExpireNonActiveCredential()
        {
            new DeselectCertificateCommandHandleShouldNotUpdateNonActiveIssuancesScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void DeselectCertificateCommandHandleReturnsRejectedWhenLatestIssuanceNotMarkefForDeselect()
        {
            new DeselectCertificateCommandHandleReturnsRejectedWhenLatestIssuanceNotMarkefForDeselectScenario().BDDfy();
        }


        #region Scenario Base Classes
        private abstract class DeselectCertificateCommandServiceScenario : CredentialServiceScenario
        {
            protected ICredentialService CredentialService { get; set; }
            protected DeselectCertificateCommand Command { get; set; }
            protected DeselectCertificateCommandResult CommandResult { get; set; }
            protected Credential Credential { get; set; }
            protected Mock<ILogger> Log { get; set; }
            protected new EmailBuilder EmailBuilder { get; set; }
            protected new DateTimeBuilder DateTimeBuilder { get; set; }
            protected new Exception ExceptionCaught { get; set; }
            
            protected Mock<IBusControl> _busControlMock;

            protected override List<Type> AdditionalDependencies()
            {
                var types = base.AdditionalDependencies();
                types.Add(typeof(ICredentialRepository));
                types.Add(typeof(ISession));
                types.Add(typeof(IQueryFactory));
                types.Add(typeof(IValidationFactory));
                types.Add(typeof(ICertificationService));
                types.Add(typeof(ICertificationRepository));
                types.Add(typeof(ISourceService));
                types.Add(typeof(ISourceRepository));
                types.Add(typeof(IBackgroundJobClient));
                types.Add(typeof(IValidationFactory));
                types.Add(typeof(IValidator<MarkCertificateForDeselectCommand>));
                return types;
            }

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Log = new Mock<ILogger>();
                EmailBuilder = new EmailBuilder();
                DateTimeBuilder = new DateTimeBuilder();
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                My<IHelperService>()
                   .Setup(o => o.TriggeredCommunication(It.IsAny<Credential>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<ICredentialService>()))
                   .Returns(new Task(() => { }));

                SetupBusControlMock();

                CredentialService = Container.GetInstance<App.Services.Impl.CredentialService>();
                SetupCredential();
                SetupCredentialRespository();
                SetupValidation();

                ((App.Services.Impl.CredentialService)CredentialService).Log = Log.Object;
                LogTest.Watch(Log);
            }

            protected virtual void SetupCredential()
            {
                var source = SourceBuilder.BuildAbim();
                var issuance =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        Resources.IssuanceStatusType.Active,
                        new DateTime(2010, 1, 1),
                        Resources.DurationType.Continuous,
                        Resources.MaintenanceRequirementType.Required,
                        Resources.MaintenanceStatusType.Maintained,
                        Resources.OccurrenceType.Recertification);
                issuance.DeselectionSubmittedDate = DateTime.Now.AddDays(-3);
                Issuance[] issuances = { issuance };

                Credential =
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        Resources.CertificationType.Primary,
                        Resources.CredentialType.General,
                        Resources.PathwayType.MOC,
                        issuances);

                Credential.IsInCMP = true;
            }

            protected virtual void SetupBusControlMock()
            {

                _busControlMock = new Mock<IBusControl>(MockBehavior.Strict);

                _busControlMock
                    .Setup(x => x.Publish(It.IsAny<CMPUnEnrolled>(), It.IsAny<CancellationToken>()))
                     .Returns(Task.FromResult(false));

                Mocks.Add(typeof(IBusControl), _busControlMock);

                Container.Inject(typeof(IBusControl), _busControlMock.Object);
            }

            protected virtual void SetupCredentialRespository()
            {
                My<ICredentialRepository>()
                    .Setup(mock => mock.Load(It.IsAny<Guid>()))
                    .Returns(Credential);

                My<ICredentialRepository>()
                    .SetupGet(mock => mock.CommitEachCallInItsOwnTransaction)
                    .Returns(false);

                My<ICredentialRepository>()
                    .Setup(mock => mock.BeginTransaction());

                My<ICredentialRepository>()
                    .Setup(mock => mock.CommitTransaction());

                My<ICredentialRepository>()
                    .Setup(mock => mock.RollbackTransaction());

                My<ICredentialRepository>()
                    .Setup(mock => mock.Update(Credential, It.IsAny<string>()))
                    .Returns(new AbimValidationResult { Succeeded = true });
            }

            protected virtual void SetupValidation()
            {
                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<DeselectCertificateCommand>())
                    .Returns(My<IValidator<DeselectCertificateCommand>>().Object);
                My<IValidator<DeselectCertificateCommand>>()
                    .Setup(o => o.Validate(It.IsAny<DeselectCertificateCommand>()))
                    .Returns(new ValidationResult());
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (DeselectCertificateCommandResult)(CredentialService.Handle(Command));
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            public void AndThenTheCommandResultShouldNotBeNull()
            {
                CommandResult.Should().NotBeNull();
            }
        }

        private abstract class DeselectCertificateCommandHandleReturnsRejectedScenario : DeselectCertificateCommandServiceScenario
        {
            protected string resultMessageShouldContain;

            public DeselectCertificateCommandHandleReturnsRejectedScenario(string rejectedResultMessageStringToFind)
            {
                resultMessageShouldContain = rejectedResultMessageStringToFind;
            }

            public void AndThenTheCommandResultStatusShouldBeRejected()
            {
                CommandResult.Status.Should().Be(CommandStatus.Rejected);
            }

            public void AndThenTheCommandResultDotSucceededShouldBeFalse()
            {
                CommandResult.Succeeded.Should().BeFalse();
            }

            public void AndThenTheCommandResultMessageShouldMentionTheExpectedString()
            {
                CommandResult.Message.ToLower().Should().Contain(resultMessageShouldContain.ToLower());
            }
        }

        #endregion Scenario Base Classes

        #region Scenarios

        #region Successful Scenario
        private class DeselectCertificateCommandHandleSuccessfulScenario : DeselectCertificateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<DeselectCertificateCommand>
                            .Valid()
                            .With(cmd => cmd.Username = "groddenberry")
                            .With(cmd => cmd.ExpiredDate = new DateTime(2020, 9, 18))
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            public void AndThenTheCommandResultStatusShouldBeAccepted()
            {
                CommandResult.Status.Should().Be(CommandStatus.Accepted);
            }

            public void AndThenTheCommandResultDotSucceededShouldBeTrue()
            {
                CommandResult.Succeeded.Should().BeTrue();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }

            public void AndTheMostRecentIssuanceShouldHaveBeenMarkedForDeSelection()
            {
                var latestIssuance = Credential.Issuances.OrderByDescending(issuance => issuance.IssuanceDate).First();
                latestIssuance.DeselectionProcessedDate.ShouldNotBeNull();
                latestIssuance.ExpiredDate.ShouldBe(Command.ExpiredDate);
                latestIssuance.IssuanceStatus.ShouldBe(Resources.IssuanceStatusType.Expired);
                latestIssuance.MaintenanceStatus.ShouldBe(Resources.MaintenanceStatusType.NotMaintained);
                latestIssuance.AuditData.Modified.ShouldNotBeNull();
                latestIssuance.AuditData.ModifiedBy.ShouldBe(Command.Username);
            }

            public void AndTheCredentialShouldHaveAnIsActiveValueOfFalse()
            {
                Credential.IsActive.ShouldBeFalse();
            }

            public void AndThenThereShouldBeAStartTraceOrAtLeastADebugAtTheTop()
            {
                LogTest.Should().Match<LogTest>(log =>
                    log.Traces.Any(s => s.StartsWith("Started Handle for")) ||
                    log.Debugs.Any(s => s.Contains("Command Args for")));
            }

            public void AndThenThereShouldBeACommandDebugSomewhere()
            {
                LogTest.Debugs.Should().Contain(s => s.Contains("Command Args for"));
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

            public void AndThenTheTransactionShouldHaveBeenBegun()
            {
                My<ICredentialRepository>().Verify(mock => mock.BeginTransaction(), Times.Once);
            }

            public void AndThenTheTransactionShouldHaveBeenCommitted()
            {
                My<ICredentialRepository>().Verify(mock => mock.CommitTransaction(), Times.Once);
            }

            private void AndACMPUnEnrolledShouldBePublishedWithTrueVoluntaryValue()
            {
                _busControlMock
                .Verify(mock =>
                            mock.Publish(It.Is<CMPUnEnrolled>(e => e.Voluntary == true), It.IsAny<CancellationToken>()),
                                Times.Once);
            }

        }
        #endregion Successful Scenario

        #region Failed Validation Scenario
        private class DeselectCertificateCommandHandleFailsValidationScenario : DeselectCertificateCommandHandleReturnsRejectedScenario
        {
            public DeselectCertificateCommandHandleFailsValidationScenario() : base("Validation Failed")
            { }

            public void GivenIInputAnInvalidCommand()
            {
                var failures =
                    new List<ValidationFailure>(1);

                failures.Add(new ValidationFailure("some property", "some error"));

                var validationResult =
                    new ValidationResult(failures);

                My<IValidator<DeselectCertificateCommand>>()
                    .Setup(o => o.Validate(It.IsAny<DeselectCertificateCommand>()))
                    .Returns(validationResult);

                Command = CommandBuilder<DeselectCertificateCommand>
                    .Invalid()
                    .Build(); //In actuality, the validator is mocked to return an invalid response for this test
            }

            public void AndThenTheCommandResultValidationShouldShowFailure()
            {
                CommandResult.Validation.Succeeded.Should().BeFalse();
            }

            public void AndThenTheValidationMessageShouldHaveAValue()
            {
                CommandResult.ValidationMessage.ShouldNotBeNullOrEmpty();
            }
        }
        #endregion Failed Validation Scenario

        #region Database Save Failed Scenario
        private class DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario : DeselectCertificateCommandHandleReturnsRejectedScenario
        {
            private const string ERROR_MESSAGE = "Database go boom!";

            public DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario() : base(ERROR_MESSAGE)
            {
            }

            protected override void SetupCredentialRespository()
            {
                base.SetupCredentialRespository();
                My<ICredentialRepository>()
                    .Setup(mock => mock.Update(Credential, It.IsAny<string>()))
                    .Throws(new ApplicationException(ERROR_MESSAGE));
            }

            public void GivenIHaveAValidCommand()
            {
                Command =
                    CommandBuilder<DeselectCertificateCommand>
                        .Valid()
                        .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }
        }
        #endregion Database Save Failed Scenario

        #region Database Save Fails Domain Validation Scenario
        private class DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario : DeselectCertificateCommandHandleReturnsRejectedScenario
        {
            private ValidationResult badValidationResult;

            public DeselectCertificateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario() : base("Domain validation failed")
            { }

            protected override void SetupCredentialRespository()
            {
                base.SetupCredentialRespository();

                var failures = new List<ValidationFailure>(1);
                failures.Add(new ValidationFailure("some property", "some error"));

                badValidationResult = new ValidationResult(failures);

                My<ICredentialRepository>()
                    .Setup(o => o.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                    .Returns(badValidationResult.ToAbimValidationResult());
            }

            public void GivenIHaveAValidCommand()
            {
                Command =
                    CommandBuilder<DeselectCertificateCommand>
                        .Valid()
                        .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowFailure()
            {
                CommandResult.Validation.Succeeded.Should().BeFalse();
            }

            public void AndThenTheCommandResultMessageShouldIncludeTheObjectValidationMessage()
            {
                CommandResult.Message.Should().Contain(badValidationResult.ToAbimValidationResult().Message);
            }
        }
        #endregion Database Save Fails Domain Validation Scenario

        #region Exception Scenario
        private class DeselectCertificateCommandHandleReturnsRejectedWhenExceptionOccursScenario : DeselectCertificateCommandHandleReturnsRejectedScenario
        {
            public DeselectCertificateCommandHandleReturnsRejectedWhenExceptionOccursScenario() : base("no issuances")
            { }

            protected override void SetupCredential()
            {
                //The lack of issuances on this credential will cause an 
                //exception to be thrown by design.
                var source = SourceBuilder.BuildAbim();

                Credential =
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        Resources.CertificationType.Primary,
                        Resources.CredentialType.General,
                        Resources.PathwayType.MOC,
                        null);
            }

            public void GivenIHaveAValidCommand()
            {
                Command =
                    CommandBuilder<DeselectCertificateCommand>
                        .Valid()
                        .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }
        }
        #endregion Exception Scenario

        #region Already Deselected Scenario
        private class DeselectCertificateCommandHandleReturnsRejectedWhenCredentialAlreadyDeselectedScenario : DeselectCertificateCommandHandleReturnsRejectedScenario
        {
            Guid credentialId = Guid.NewGuid();
            public DeselectCertificateCommandHandleReturnsRejectedWhenCredentialAlreadyDeselectedScenario() : base("")
            { }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<DeselectCertificateCommand>
                             .Valid()
                             .With(cmd => cmd.Username = "bdickinson")
                             .With(cmd => cmd.ExpiredDate = new DateTime(2020, 9, 18))
                             .With(cmd => cmd.CredentialId = credentialId)
                             .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                var latestIssuance = Credential.Issuances.OrderByDescending(issuance => issuance.IssuanceDate).First();
                Credential.ExternalId = Guid.NewGuid();
                latestIssuance.DeselectionProcessedDate = new DateTime(2020, 11, 1);
                GivenIInputAValidCommand();
                resultMessageShouldContain = $"Credential {credentialId} has already been processed for deselection.";
            }
            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }

            public void AndThenThereShouldBeAWarningMessage()
            {
                LogTest.Should().Match<LogTest>(log =>
                    log.Warns.Any(s => s.StartsWith(resultMessageShouldContain)) &&
                    log.Errors.All(s => s.Equals("")));
            }
        }
        #endregion Already Deselected Scenario

        #region Shouldn't Update Non-Active Issuances Scenario
        private class DeselectCertificateCommandHandleShouldNotUpdateNonActiveIssuancesScenario : DeselectCertificateCommandServiceScenario
        {
            protected override void SetupCredential()
            {
                base.SetupCredential();
                Credential.Issuances[0].IssuanceStatus = Resources.IssuanceStatusType.Revoked;
            }

            public void GivenIHaveAValidCommand()
            {
                Command =
                    CommandBuilder<DeselectCertificateCommand>
                        .Valid()
                        .Build();
            }

            protected void AndTheIssuanceShouldNotHaveBeenExpired()
            {
                Credential.Issuances[0].IssuanceStatus.ShouldNotBe(Resources.IssuanceStatusType.Expired);
            }

            protected void AndTheIssuanceShouldHaveBeenUpdatedToShowThatDeselectionWasProcessed()
            {
                Credential.Issuances[0].DeselectionProcessedDate.ShouldNotBeNull();
            }
        }
        #endregion Shouldn't Update Non-Active Issuances Scenario

        #region Shouldn't Update When Latest Issuance Isn't Flagged For Deselection Scenario

        private class DeselectCertificateCommandHandleReturnsRejectedWhenLatestIssuanceNotMarkefForDeselectScenario : DeselectCertificateCommandHandleReturnsRejectedScenario
        {
            public DeselectCertificateCommandHandleReturnsRejectedWhenLatestIssuanceNotMarkefForDeselectScenario(): base("latest issuance")
            {
            }

            protected override void SetupCredential()
            {
                var source = SourceBuilder.BuildAbim();
                var issuance =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        Resources.IssuanceStatusType.Active,
                        new DateTime(2010, 1, 1),
                        Resources.DurationType.Continuous,
                        Resources.MaintenanceRequirementType.Required,
                        Resources.MaintenanceStatusType.Maintained,
                        Resources.OccurrenceType.Recertification);
                issuance.DeselectionSubmittedDate = null; //This should cause the exception we're testing for
                Issuance[] issuances = { issuance };

                Credential =
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        Resources.CertificationType.Primary,
                        Resources.CredentialType.General,
                        Resources.PathwayType.MOC,
                        issuances);
            }

            public void GivenIHaveAValidCommand()
            {
                Command =
                    CommandBuilder<DeselectCertificateCommand>
                        .Valid()
                        .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }
        }

        #endregion #region Shouldn't Update When Latest Issuance Isn't Flagged For Deselection Scenario

        #endregion Scenarios
    }
}
