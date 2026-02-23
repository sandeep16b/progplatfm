using Abim.Enterprise.Core.ServiceBus.Program;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NLog;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    ///<summary>
    ///Unit Test main class
    ///</summary>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the MarkCertificatesForSelectOrDeselectCommand"
        )]
    [TestFixture]
    public class MarkCertificatesForSelectOrDeselectCommandSpec
    {
        [TestCase]
        [WorkItem(187664)]
        [WorkItem(187666)]
        public void MarkCertificatesForSelectOrDeselectCommandHandleReturnsAcceptedOnSuccess()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandleSuccessfulScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        [WorkItem(187666)]
        public void MarkCertificatesForSelectOrDeselectCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandleFailsValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        [WorkItem(187666)]
        public void MarkCertificatesForSelectOrDeselectCommandHandleRollsBackTransactionOnError()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandleRollsbackTransactionOnErrorScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187663)]
        public void MarkCertificatesForSelectOrDeselectCommandHandleShouldPublishCertificateDeselectAndSelectEvents()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandleShouldPublishCertificateDeselectAndSelectEventsScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187666)]
        public void MarkCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveAction()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveActionScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(180989)]
        public void MarkCertificatesForSelectOrDeselectCommandHandleShouldTriggerNotification()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandleShouldTriggerNotificationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(180989)]
        public void MarkCertificatesForSelectOneOrDeselectCommandHandleShouldTriggerNotification()
        {
            new MarkCertificatesForSelectOneOrDeselectCommandHandleShouldTriggerNotificationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(212997)]
        public void MarkCertificatesForSelectOrDeselectCommandHandler_Should_NOT_TriggerNotification_WhenCredentialIsNotUpdated()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandler_Should_NOT_TriggerNotification_WhenCredentialIsNotUpdatedScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(212997)]
        public void MarkCertificatesForSelectOrDeselectCommandHandler_Should_TriggerNotification_OnlyForDeselectedCredental()
        {
            new MarkCertificatesForSelectOrDeselectCommandHandler_Should_TriggerNotification_OnlyForDeselectedCredentalScenario().BDDfy();
        }

        #region Scenario Base Classes
        private abstract class MarkCertificatesForSelectOrDeselectCommandServiceScenario : CredentialServiceScenario
        {
            protected ICredentialService CredentialService { get; set; }
            protected MarkCertificatesForSelectOrDeselectCommand Command { get; set; }
            protected MarkCertificatesForSelectOrDeselectCommandResult CommandResult { get; set; }
            protected List<Credential> Credentials { get; set; }
            protected Mock<ILogger> Log { get; set; }
            protected new EmailBuilder EmailBuilder { get; set; }
            protected new DateTimeBuilder DateTimeBuilder { get; set; }
            protected new Exception ExceptionCaught { get; set; }

            protected override List<Type> AdditionalDependencies()
            {
                var types = base.AdditionalDependencies();
                types.Add(typeof(IHelperService));
                types.Add(typeof(ICredentialRepository));
                types.Add(typeof(ISession));
                types.Add(typeof(IQueryFactory));
                types.Add(typeof(IValidationFactory));
                types.Add(typeof(ICertificationService));
                types.Add(typeof(ICertificationRepository));
                types.Add(typeof(ISourceService));
                types.Add(typeof(ISourceRepository));
                types.Add(typeof(IBusControl));
                types.Add(typeof(IBackgroundJobClient));
                types.Add(typeof(IValidationFactory));
                types.Add(typeof(IValidator<MarkCertificateForDeselectCommand>));
                types.Add(typeof(IValidator<MarkCertificateForSelectCommand>));
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
                CredentialService = Container.GetInstance<App.Services.Impl.CredentialService>();
                SetupCredentials();
                SetupCredentialRespository();
                SetupValidation();
                SetupMessageBus();
                SetupCorrectiveAction();

                ((App.Services.Impl.CredentialService)CredentialService).Log = Log.Object;
                LogTest.Watch(Log);
            }

            protected virtual void SetupCredentials()
            {
                Credentials = new List<Credential>(6);

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
                Issuance[] issuances = { issuance };

                Credentials.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        Resources.CertificationType.Primary,
                        Resources.CredentialType.General,
                        Resources.PathwayType.MOC,
                        issuances));

                var issuance2 =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        Resources.IssuanceStatusType.Active,
                        new DateTime(2012, 6, 1),
                        Resources.DurationType.Continuous,
                        Resources.MaintenanceRequirementType.Required,
                        Resources.MaintenanceStatusType.Maintained,
                        Resources.OccurrenceType.Recertification);
                Issuance[] issuances2 = { issuance2 };

                Credentials.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "HOSP",
                        "Focused Practice in Hospital Medicine",
                        Resources.CertificationType.FocusPractice,
                        Resources.CredentialType.General,
                        Resources.PathwayType.MOC,
                        issuances2));

                var issuance3 =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        Resources.IssuanceStatusType.Active,
                        new DateTime(2014, 9, 1),
                        Resources.DurationType.Continuous,
                        Resources.MaintenanceRequirementType.Required,
                        Resources.MaintenanceStatusType.Maintained,
                        Resources.OccurrenceType.Recertification);
                Issuance[] issuances3 = { issuance3 };

                Credentials.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "CARD",
                        "Cardiovascular Disease",
                        Resources.CertificationType.Subspecialty,
                        Resources.CredentialType.Subspecialty,
                        Resources.PathwayType.MOC,
                        issuances3));

                var issuance4 =
                   IssuanceBuilder.BuildWithoutRandoms(
                       source,
                       Resources.IssuanceStatusType.Active,
                       new DateTime(2014, 9, 1),
                       Resources.DurationType.Continuous,
                       Resources.MaintenanceRequirementType.Required,
                       Resources.MaintenanceStatusType.Maintained,
                       Resources.OccurrenceType.Recertification);
                Issuance[] issuances4 = { issuance4 };

                issuance4.DeselectionEffectiveDate = new DateTime(DateTime.Now.Year, 2, 1);
                issuance4.DeselectionSubmittedDate = DateTime.Now.AddYears(-1);

                Credentials.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "PULM",
                        "Pulmonary Disease",
                        Resources.CertificationType.Subspecialty,
                        Resources.CredentialType.Subspecialty,
                        Resources.PathwayType.MOC,
                        issuances4));

                var issuance5 =
                       IssuanceBuilder.BuildWithoutRandoms(
                           source,
                           Resources.IssuanceStatusType.Active,
                           new DateTime(2014, 9, 1),
                           Resources.DurationType.Continuous,
                           Resources.MaintenanceRequirementType.Required,
                           Resources.MaintenanceStatusType.Maintained,
                           Resources.OccurrenceType.Recertification);

                issuance5.DeselectionEffectiveDate = new DateTime(DateTime.Now.Year, 2, 1);
                issuance5.DeselectionSubmittedDate = DateTime.Now.AddYears(-1);

                Issuance[] issuances5 = { issuance5 };

                Credentials.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "HPM",
                        "Hospice and Palliative Medicine",
                        Resources.CertificationType.Subspecialty,
                        Resources.CredentialType.Subspecialty,
                        Resources.PathwayType.MOC,
                        issuances5));

                var issuance6 =
                   IssuanceBuilder.BuildWithoutRandoms(
                       source,
                       Resources.IssuanceStatusType.Active,
                       new DateTime(2014, 9, 1),
                       Resources.DurationType.Continuous,
                       Resources.MaintenanceRequirementType.Required,
                       Resources.MaintenanceStatusType.Maintained,
                       Resources.OccurrenceType.Recertification);

                issuance6.DeselectionEffectiveDate = new DateTime(DateTime.Now.Year, 2, 1);
                issuance6.DeselectionSubmittedDate = DateTime.Now.AddYears(-1);

                Issuance[] issuances6 = { issuance6 };

                Credentials.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "CRIT",
                        "Critical Care Medicine",
                        Resources.CertificationType.Subspecialty,
                        Resources.CredentialType.Subspecialty,
                        Resources.PathwayType.MOC,
                        issuances6));
            }

            protected virtual void SetupCredentialRespository()
            {
                My<ICredentialRepository>()
                    .SetupSequence(mock => mock.Load(It.IsAny<Guid>()))
                    .Returns(Credentials[0])
                    .Returns(Credentials[1])
                    .Returns(Credentials[2])
                    .Returns(Credentials[3])
                    .Returns(Credentials[4])
                    .Returns(Credentials[5]);

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
                    .Setup(mock => mock.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                    .Returns(new Relational.Validation.Impl.AbimValidationResult { Succeeded = true });

                My<ICredentialRepository>()
                    .Setup(mock => mock.SearchByMemberId(It.IsAny<Guid>()))
                    .Returns(Credentials);
            }

            protected virtual void SetupValidation()
            {
                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<MarkCertificatesForSelectOrDeselectCommand>())
                    .Returns(My<IValidator<MarkCertificatesForSelectOrDeselectCommand>>().Object);
                My<IValidator<MarkCertificatesForSelectOrDeselectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificatesForSelectOrDeselectCommand>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<MarkCertificateForDeselectCommand>())
                    .Returns(My<IValidator<MarkCertificateForDeselectCommand>>().Object);
                My<IValidator<MarkCertificateForDeselectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificateForDeselectCommand>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<MarkCertificateForSelectCommand>())
                    .Returns(My<IValidator<MarkCertificateForSelectCommand>>().Object);

                My<IValidator<MarkCertificateForSelectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificateForSelectCommand>()))
                    .Returns(new FluentValidation.Results.ValidationResult());
            }

            protected virtual void SetupMessageBus()
            {
                My<IBusControl>()
                    .Setup(mock => mock.Publish<CertificateDeselectedEvent>(
                        It.IsAny<CertificateDeselectedEvent>(), 
                        It.IsAny<System.Threading.CancellationToken>()))
                    .Returns(Task.FromResult(true));

                My<IBusControl>()
                    .Setup(mock => mock.Publish<CertificateSelectedEvent>(
                        It.IsAny<CertificateSelectedEvent>(),
                        It.IsAny<System.Threading.CancellationToken>()))
                    .Returns(Task.FromResult(true));
            }

            protected virtual void SetupCorrectiveAction()
            {
                My<IProgramRulesService>()
                    .Setup(mock => mock.RunCorrectiveActionForMember(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null))
                    .Returns(Task.FromResult(true));
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (MarkCertificatesForSelectOrDeselectCommandResult)(CredentialService.Handle(Command));
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

        private abstract class MarkCertificatesForSelectOrDeselectCommandHandleReturnsRejectedScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            protected string resultMessageShouldContain;

            public MarkCertificatesForSelectOrDeselectCommandHandleReturnsRejectedScenario(string rejectedResultMessageStringToFind)
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
        private class MarkCertificatesForSelectOrDeselectCommandHandleSuccessfulScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "bdickinson",
                                AbimId = "123456"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Skip(3).Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
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

            public void AndATransactionShouldHaveBeenBegun()
            {
                My<ICredentialRepository>().Verify(mock => mock.BeginTransaction(), Times.Once);
            }

            public void AndTheMostRecentIssuancesShouldHaveBeenMarkedForDeselection()
            {
                foreach (var credential in Credentials.Take(3))
                {
                    var latestIssuance = credential.Issuances.OrderByDescending(issuance => issuance.IssuanceDate).First();
                    latestIssuance.DeselectionSubmittedDate.ShouldNotBeNull();
                    latestIssuance.DeselectionEffectiveDate.ShouldNotBeNull(); //TODO: Refine
                    latestIssuance.AuditData.Modified.ShouldNotBeNull();
                    latestIssuance.AuditData.ModifiedBy.ShouldBe(Command.UserInfo.Username);
                }
            }

            public void AndTheMostRecentIssuancesShouldHaveBeenMarkedForSelection()
            {
                foreach (var credential in Credentials.Skip(3))
                {
                    var latestIssuance = credential.Issuances.OrderByDescending(issuance => issuance.IssuanceDate).First();
                    latestIssuance.DeselectionSubmittedDate.ShouldBeNull();
                    latestIssuance.DeselectionEffectiveDate.ShouldBeNull(); 
                    latestIssuance.AuditData.Modified.ShouldNotBeNull();
                    latestIssuance.AuditData.ModifiedBy.ShouldBe(Command.UserInfo.Username);
                }
            }

            public void AndTheTransactionShouldHaveBeenCommitted()
            {
                My<ICredentialRepository>().Verify(mock => mock.CommitTransaction(), Times.Once);
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

        }
        #endregion Successful Scenario

        #region Failed Validation Scenario
        private class MarkCertificatesForSelectOrDeselectCommandHandleFailsValidationScenario : MarkCertificatesForSelectOrDeselectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificatesForSelectOrDeselectCommandHandleFailsValidationScenario() : base("Validation Failed")
            { }

            public void GivenIInputAnInvalidCommand()
            {
                var failures =
                    new List<FluentValidation.Results.ValidationFailure>(1);

                failures.Add(new FluentValidation.Results.ValidationFailure("some property", "some error"));

                var validationResult =
                    new FluentValidation.Results.ValidationResult(failures);

                My<IValidator<MarkCertificatesForSelectOrDeselectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificatesForSelectOrDeselectCommand>()))
                    .Returns(validationResult);

                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                    .Invalid()
                    .Build();
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

        #region Rollback Transaction on Error Scenario
        private class MarkCertificatesForSelectOrDeselectCommandHandleRollsbackTransactionOnErrorScenario : MarkCertificatesForSelectOrDeselectCommandHandleReturnsRejectedScenario
        {
            protected override void SetupCredentialRespository()
            {
                base.SetupCredentialRespository();

                //Mock will let the first two calls return Succeeded, but the 
                //last one will not.
                My<ICredentialRepository>()
                    .SetupSequence(mock => mock.Update(It.IsAny<Credential>(), It.IsAny<string>()))
                    .Returns(new Relational.Validation.Impl.AbimValidationResult { Succeeded = true })
                    .Returns(new Relational.Validation.Impl.AbimValidationResult { Succeeded = true })
                    .Returns(new Relational.Validation.Impl.AbimValidationResult { Succeeded = false });
            }

            public MarkCertificatesForSelectOrDeselectCommandHandleRollsbackTransactionOnErrorScenario() : base("Failed to mark credentials for deselection")
            { }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "sharris",
                                AbimId = "654321"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Skip(3).Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndTheTransactionShouldHaveBeenRolledBack()
            {
                My<ICredentialRepository>().Verify(mock => mock.RollbackTransaction(), Times.Once);
            }

            public void AndTheResultShouldContainInformationAboutWhyTheOperationFailed()
            {
                CommandResult.Message.ShouldNotBeNull();
            }
        }
        #endregion Rollback Transaction on Error Scenario

        #region Publish CertificateDeselectedEvents Scenario
        private class MarkCertificatesForSelectOrDeselectCommandHandleShouldPublishCertificateDeselectAndSelectEventsScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "dmurray",
                                AbimId = "525252"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Skip(3).Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndACertificateDeselectedEventShouldBePublishedForEachCredential()
            {
                foreach (var credential in Credentials.Take(3))
                {
                    My<IBusControl>()
                        .Verify(mock =>
                            mock.Publish(It.Is<CertificateDeselectedEvent>(deselectedEvent => 
                                deselectedEvent.MemberId == credential.MemberId 
                                && deselectedEvent.CredentialGuid == credential.ExternalId
                                && deselectedEvent.CertificationGuid == credential.Certification.ExternalId), default(System.Threading.CancellationToken)), 
                            Times.Once);
                }
            }

            public void AndACertificateSelectedEventShouldBePublishedForEachCredential()
            {
                foreach (var credential in Credentials.Skip(3).Take(3))
                {
                    My<IBusControl>()
                        .Verify(mock =>
                            mock.Publish(It.Is<CertificateSelectedEvent>(selectedEvent =>
                                selectedEvent.MemberId == credential.MemberId
                                && selectedEvent.CredentialGuid == credential.ExternalId
                                && selectedEvent.CertificationGuid == credential.Certification.ExternalId
                                && selectedEvent.InitialCertDate == credential.OldestIssuance.IssuanceDate ), default(System.Threading.CancellationToken)),
                            Times.Once);
                }
            }
        }
        #endregion Publish CertificateDeselectedEvents Scenario

        #region Run CorrectiveAction Scenario
        private class MarkCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveActionScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "dmurray",
                                AbimId = "525252"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Skip(3).Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveAction()
            {
                My<IProgramRulesService>()
                    .Verify(mock =>
                        mock.RunCorrectiveActionForMember(It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Once);
            }
        }
        #endregion Publish CertificateDeselectedEvents Scenario

        #region Trigger Notification Service Scenario
        private class MarkCertificatesForSelectOrDeselectCommandHandleShouldTriggerNotificationScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "dmurray",
                                AbimId = "525252"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Skip(3).Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveAction()
            {
                My<IProgramRulesService>()
                    .Verify(mock =>
                        mock.RunCorrectiveActionForMember(It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Once);
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleTriggerNotification()
            {
                IList<string> certNames = new List<string>() {
                        Credentials[3].Certification.Name,
                        Credentials[4].Certification.Name,
                        Credentials[5].Certification.Name
                    };

                My<IHelperService>()
                    .Verify(mock =>
                        mock.TriggeredCommunication_Reactivate_Certifications(
                            It.Is<IList<string>>(l => l.SequenceEqual(certNames)),
                            It.IsAny<Guid>()), Times.Once);
            }

        }

        private class MarkCertificatesForSelectOneOrDeselectCommandHandleShouldTriggerNotificationScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "dmurray",
                                AbimId = "525252"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Skip(3).Take(1).Select(cred => cred.ExternalId).ToList())
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveAction()
            {
                My<IProgramRulesService>()
                    .Verify(mock =>
                        mock.RunCorrectiveActionForMember(It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Once);
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleTriggerNotification()
            {
                IList<string> certNames = new List<string>() {
                        Credentials[3].Certification.Name
                    };

                My<IHelperService>()
                    .Verify(mock =>
                        mock.TriggeredCommunication_Reactivate_Certifications(
                            It.Is<IList<string>>(l => l.SequenceEqual(certNames)),
                            It.IsAny<Guid>()), Times.Once);
            }

        }
        
        private class MarkCertificatesForSelectOrDeselectCommandHandler_Should_NOT_TriggerNotification_WhenCredentialIsNotUpdatedScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "dmurray",
                                AbimId = "525252"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = null)
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Take(3).Select(cred => cred.ExternalId).ToList()) // these creds already selected, no  updates is expected
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleShould_NOT_RunCorrectiveAction()
            {
                My<IProgramRulesService>()
                    .Verify(mock =>
                        mock.RunCorrectiveActionForMember(It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Never);
            }

            public void AndCertificatesForSelectOrDeselectCommandHandler_NOT_TriggerNotification()
            {
                My<IHelperService>()
                    .Verify(mock =>
                        mock.TriggeredCommunication_Reactivate_Certifications(
                            It.IsAny<List<string>>(),
                            It.IsAny<Guid>()), Times.Never);
            }

        }
        

        private class MarkCertificatesForSelectOrDeselectCommandHandler_Should_TriggerNotification_OnlyForDeselectedCredentalScenario : MarkCertificatesForSelectOrDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificatesForSelectOrDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "dmurray",
                                AbimId = "525252"
                            })
                            .With(cmd => cmd.CredentialIdsForDeselect = null)
                            .With(cmd => cmd.CredentialIdsForSelect = Credentials.Take(4).Select(cred => cred.ExternalId).ToList()) //  first 3 is selected and then 4 is deselected
                            .With(cmd => cmd.MemberId = Guid.NewGuid())
                            .Build();
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleShouldRunCorrectiveAction()
            {
                My<IProgramRulesService>()
                    .Verify(mock =>
                        mock.RunCorrectiveActionForMember(It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Once);
            }

            public void AndCertificatesForSelectOrDeselectCommandHandleTriggerNotification()
            {
                IList<string> certNames = new List<string>() {
                        Credentials[3].Certification.Name // only this one would be triggered in notification
                    };

                My<IHelperService>()
                    .Verify(mock =>
                        mock.TriggeredCommunication_Reactivate_Certifications(
                            It.Is<IList<string>>(l => l.SequenceEqual(certNames)),
                            It.IsAny<Guid>()), Times.Once);
            }

        }

        #endregion


        #endregion Scenarios
    }
}
