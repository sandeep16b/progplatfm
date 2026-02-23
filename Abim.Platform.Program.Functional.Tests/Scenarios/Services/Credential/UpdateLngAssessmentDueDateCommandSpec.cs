using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Relational.Queries;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
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
using Shouldly;
using System;
using System.Collections.Generic;
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
        SoThat = "it can handle the UpdateLngAssessmentDueDateCommand"
        )]
    [TestFixture]
    public class UpdateLngAssessmentDueDateCommandSpec
    {

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]

        public void UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_InSummativeYear_NoChangeToExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_InSummativeYear_NoChangeToExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_NotLapsedCert_NonCoSponsoredCert_NotInSummativeYear_AdvanceExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_NotLapsedCert_NonCoSponsoredCer_NotInSummativeYear_AdvanceExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_LapsedCert_NonCoSponsoredCert_NotInSummativeYear_NoChangeToExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_LapsedCert_NonCoSponsoredCert_NotInSummativeYear_NoChangeToExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_InDueYear_CoSponsoredCert_InSummativeYear_NoChangeToExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_InDueYear_CoSponsored_InSummativeYear_NoChangeToExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_NotInDueYear_CoSponsoredCert_NotInSummativeYear_NoChangeToExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_NotInDueYear_CoSponsoredCert_NotInSummativeYear_NoChangeToExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_FailMetParticipationStatus_NoChangeToExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_FailMetParticipationStatus_NoChangeToExamDueDateScenario().BDDfy();
        }


        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        [WorkItem(231779)]
        public void UpdateLngAssessmentDueDateCommandHandle_PassSummativeYearInDueYear_AdvanceExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_PassSummativeYear_AdvanceExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_FailSummativeYear_NoChangeToExamDueDate()
        {
            new UpdateLngAssessmentDueDateCommandHandle_FailSummativeYear_NoChangeToExamDueDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_ReturnsRejectedWhenCommandFailsValidation()
        {
            new UpdateLngAssessmentDueDateCommandHandleFailsValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_ReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new UpdateLngAssessmentDueDateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(181022)]
        [WorkItem(181023)]
        [WorkItem(181024)]
        public void UpdateLngAssessmentDueDateCommandHandle_ReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new UpdateLngAssessmentDueDateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario().BDDfy();
        }

        #region Scenario Base Classes
        private abstract class UpdateLngAssessmentDueDateCommandServiceScenario : CredentialServiceScenario
        {
            protected ICredentialService CredentialService { get; set; }
            protected UpdateLngAssessmentDueDateCommand Command { get; set; }
            protected UpdateLngAssessmentDueDateCommandResult CommandResult { get; set; }
            protected Credential Credential { get; set; }
            protected Mock<ILogger> Log { get; set; }
            protected new EmailBuilder EmailBuilder { get; set; }
            protected new DateTimeBuilder DateTimeBuilder { get; set; }
            protected new Exception ExceptionCaught { get; set; }

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
                types.Add(typeof(IBusControl));
                types.Add(typeof(IBackgroundJobClient));
                types.Add(typeof(IValidationFactory));
                types.Add(typeof(IValidator<UpdateLngAssessmentDueDateCommand>));
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

                CredentialService = Container.GetInstance<App.Services.Impl.CredentialService>();
                SetupCredential();
                SetupCredentialRespository();
                SetupValidation();

                ((App.Services.Impl.CredentialService)CredentialService).Log = Log.Object;
                LogTest.Watch(Log);

                My<IProgramRulesService>()
                    .Setup(mock => mock.RunCorrectiveActionForMember(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null))
                    .Returns(Task.FromResult(true));
            }

            protected virtual void SetupCredential()
            {
                var source = SourceBuilder.BuildAbim();
                var issuance =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        IssuanceStatusType.Active,
                        new DateTime(2010, 1, 1),
                        DurationType.Continuous,
                        MaintenanceRequirementType.Required,
                        MaintenanceStatusType.Maintained,
                        OccurrenceType.Recertification);

                Issuance[] issuances = { issuance };

                Credential =
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        CertificationType.Primary,
                        CredentialType.General,
                        PathwayType.LNG,
                        issuances);

                Credential.ExamDueDate = new DateTime(2022, 12, 31);
                Credential.AssessmentMet = false;

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
                    .Setup(o => o.GetValidatorInstance<UpdateLngAssessmentDueDateCommand>())
                    .Returns(My<IValidator<UpdateLngAssessmentDueDateCommand>>().Object);
                My<IValidator<UpdateLngAssessmentDueDateCommand>>()
                    .Setup(o => o.Validate(It.IsAny<UpdateLngAssessmentDueDateCommand>()))
                    .Returns(new ValidationResult());
            }

            public virtual void WhenICallHandle()
            {
                try
                {
                    CommandResult = CredentialService.Handle(Command).Result;
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

        }

        private abstract class UpdateLngAssessmentDueDateCommandHandleReturnsRejectedScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            protected string resultMessageShouldContain;

            public UpdateLngAssessmentDueDateCommandHandleReturnsRejectedScenario(string rejectedResultMessageStringToFind)
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
        private class UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_InSummativeYear_NoChangeToExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = true)
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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(new DateTime(Command.Year, 12, 31));
                CommandResult.Data.AssessmentMet.Should().BeFalse();
            }

            public void AndThenCorrectiveActionRun()
            {
                My<IProgramRulesService>()
                    .Verify(mock => mock.RunCorrectiveActionForMember(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Never);
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

        }

        private class UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_NotLapsedCert_NonCoSponsoredCer_NotInSummativeYear_AdvanceExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = false)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                Credential.IsCosponsored = false; // set CoSponsored flag
                Credential.Issuances[0].IssuanceStatus = IssuanceStatusType.Active;
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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(new DateTime(Command.Year + 1, 12, 31));
                CommandResult.Data.AssessmentMet.Should().BeTrue(); // The certificate is not lapsed and meets their annual longitudinal participation requirement and is NOT in summative decision year
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

        }


        private class UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_InDueYear_CoSponsored_InSummativeYear_NoChangeToExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = true)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                Credential.IsCosponsored = true; // set CoSponsored flag
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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(new DateTime(Command.Year, 12, 31));
                CommandResult.Data.AssessmentMet.Should().BeFalse(); // The certificate is not lapsed and meets their annual longitudinal participation requirement and is not in summative decision year
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

        }

        private class UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_NotInDueYear_CoSponsoredCert_NotInSummativeYear_NoChangeToExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2023)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = false)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                Credential.IsCosponsored = true; // set CoSponsored flag
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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(Credential.ExamDueDate.Value);
                CommandResult.Data.AssessmentMet.Should().BeFalse();
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }
        }

        private class UpdateLngAssessmentDueDateCommandHandle_MetParticipationStatus_LapsedCert_NonCoSponsoredCert_NotInSummativeYear_NoChangeToExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2023)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = false)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                Credential.IsCosponsored = false; // set CoSponsored flag
                Credential.Issuances[0].IssuanceStatus = IssuanceStatusType.Expired; // lapsed cert
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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(Credential.ExamDueDate.Value);
                CommandResult.Data.AssessmentMet.Should().BeFalse();
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }
        }

        private class UpdateLngAssessmentDueDateCommandHandle_PassSummativeYear_AdvanceExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = true)
                            .With(cmd => cmd.MetParticipationStatus = null)
                            .With(cmd => cmd.IsSummativeDecisionYear = true)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            protected override void SetupCredential()
            {
                var source = SourceBuilder.BuildAbim();
                var issuance =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        Resources.IssuanceStatusType.Expired, // !!!
                        new DateTime(2010, 1, 1), // Issuance Date 
                        Resources.DurationType.Timelimited,
                        Resources.MaintenanceRequirementType.NotRequired,
                        Resources.MaintenanceStatusType.NotMaintained,
                        Resources.OccurrenceType.Recertification);

                Issuance[] issuances = { issuance };

                Credential =
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        CertificationType.Primary,
                        CredentialType.General,
                        PathwayType.LNG,
                        issuances);

                Credential.ExamDueDate = new DateTime(2020, 12, 31);
                Credential.AssessmentMet = false;

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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(new DateTime(Command.Year + 1, 12, 31));
                CommandResult.Data.AssessmentMet.Should().BeTrue();
            }

            public void AndThenNewestIssuanceShouldBeSetToActive()
            {
                CommandResult.Data.NewestIssuance.IssuanceStatus.ShouldBe(IssuanceStatusType.Expired); // remain unchanged
                CommandResult.Data.NewestIssuance.IssuanceDate.ShouldBe(new DateTime(2010, 1, 1)); // same value
            }

            public void AndThenCorrectiveActionRun()
            {
                My<IProgramRulesService>()
                    .Verify(mock => mock.RunCorrectiveActionForMember(
                        It.IsAny<Guid>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<DateTime>(),
                        It.IsAny<TriggeringEvent>(),
                        null), Times.Once);
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

        }

        private class UpdateLngAssessmentDueDateCommandHandle_FailSummativeYear_NoChangeToExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = false)
                            .With(cmd => cmd.MetParticipationStatus = null)
                            .With(cmd => cmd.IsSummativeDecisionYear = true)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            protected override void SetupCredential()
            {
                var source = SourceBuilder.BuildAbim();
                var issuance =
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        IssuanceStatusType.Expired,
                        new DateTime(2010, 1, 1),
                        DurationType.Continuous,
                        MaintenanceRequirementType.Required,
                        MaintenanceStatusType.Maintained,
                        OccurrenceType.Recertification);

                Issuance[] issuances = { issuance };

                Credential =
                    CredentialBuilder.BuildWithoutRandoms(
                        source,
                        "IM",
                        "Internal Medicine",
                        CertificationType.Primary,
                        CredentialType.General,
                        PathwayType.LNG,
                        issuances);

                Credential.ExamDueDate = new DateTime(2020, 12, 31);
                Credential.AssessmentMet = false;

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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(Credential.ExamDueDate.Value); // no change
                CommandResult.Data.AssessmentMet.Should().BeFalse();
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

        }

        private class UpdateLngAssessmentDueDateCommandHandle_FailMetParticipationStatus_NoChangeToExamDueDateScenario : UpdateLngAssessmentDueDateCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = false)
                            .With(cmd => cmd.IsSummativeDecisionYear = false)
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

            public void AndThenAssessmentDueDateShouldBeSetToCorrectValues()
            {
                CommandResult.Data.ExamDueDate.Value.ShouldBe(new DateTime(Command.Year, 12, 31));
                CommandResult.Data.AssessmentMet.Should().BeFalse();
            }

            public void AndThenThereShouldBeAnEndTrace()
            {
                LogTest.Traces.Should().Contain(s => s.StartsWith("Returning from Handle for"));
            }

        }
        #endregion Successful Scenario

        #region Failed Validation Scenario
        private class UpdateLngAssessmentDueDateCommandHandleFailsValidationScenario : UpdateLngAssessmentDueDateCommandHandleReturnsRejectedScenario
        {
            public UpdateLngAssessmentDueDateCommandHandleFailsValidationScenario() : base("Validation Failed")
            { }

            public void GivenIInputAnInvalidCommand()
            {
                var failures =
                    new List<ValidationFailure>(1);

                failures.Add(new ValidationFailure("some property", "some error"));

                var validationResult =
                    new ValidationResult(failures);

                My<IValidator<UpdateLngAssessmentDueDateCommand>>()
                    .Setup(o => o.Validate(It.IsAny<UpdateLngAssessmentDueDateCommand>()))
                    .Returns(validationResult);

                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
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
        private class UpdateLngAssessmentDueDateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario : UpdateLngAssessmentDueDateCommandHandleReturnsRejectedScenario
        {
            public UpdateLngAssessmentDueDateCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario() : base("failed to update")
            { 
            }

            protected override void SetupCredentialRespository()
            {
                base.SetupCredentialRespository();
                My<ICredentialRepository>()
                    .Setup(mock => mock.Update(Credential, It.IsAny<string>()))
                    .Throws(new ApplicationException("Something bad happened!"));
            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = false)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
                            .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }
        }
        #endregion Database Save Failed Scenario

        #region Database Save Fails Domain Validation Scenario
        private class UpdateLngAssessmentDueDateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario : UpdateLngAssessmentDueDateCommandHandleReturnsRejectedScenario
        {
            private ValidationResult badValidationResult;

            public UpdateLngAssessmentDueDateCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario() : base("Domain validation failed")
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
                Command = CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.IsSummativeDecisionYear = false)
                            .With(cmd => cmd.CredentialId = Guid.NewGuid())
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

        #endregion Scenarios
    }
}
