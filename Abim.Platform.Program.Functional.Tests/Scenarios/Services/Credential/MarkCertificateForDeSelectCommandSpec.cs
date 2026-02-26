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
using Abim.Platform.Program.WebApi.Authentication;
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
        SoThat = "it can handle the MarkCertificateForDeselectCommand"
        )]
    [TestFixture]
    public class MarkCertificateForDeselectCommandSpec
    {
        [TestCase]
        [WorkItem(187664)]
        public void MarkCertificateForDeselectCommandHandleReturnsAcceptedOnSuccess()
        {
            new MarkCertificateForDeselectCommandHandleSuccessfulScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void MarkCertificateForDeselectCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new MarkCertificateForDeselectCommandHandleFailsValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void MarkCertificateForDeselectCommandHandleReturnsRejectedWhenExceptionOccurs()
        {
            new MarkCertificateForDeselectCommandHandleReturnsRejectedWhenExceptionOccursScenario().BDDfy();
        } 

        [TestCase]
        [WorkItem(293547)]
        public void MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedAlreadyProcesseddOccurs()
        {
            new MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedAlreadyProcesseddOccursScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(293547)]
        public void MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedNotProcessedOc()
        {
            new MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedNotProcessedOccursScenario().BDDfy();
        } 

        #region Scenario Base Classes
        private abstract class MarkCertificateForDeselectCommandServiceScenario : CredentialServiceScenario
        {
            protected ICredentialService CredentialService { get; set; }
            protected MarkCertificateForDeselectCommand Command { get; set; }
            protected MarkCertificateForDeselectCommandResult CommandResult { get; set; }
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
                    .Setup(o => o.GetValidatorInstance<MarkCertificateForDeselectCommand>())
                    .Returns(My<IValidator<MarkCertificateForDeselectCommand>>().Object);
                My<IValidator<MarkCertificateForDeselectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificateForDeselectCommand>()))
                    .Returns(new ValidationResult());
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (MarkCertificateForDeselectCommandResult)(CredentialService.Handle(Command));
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

        private abstract class MarkCertificateForDeselectCommandHandleReturnsRejectedScenario : MarkCertificateForDeselectCommandServiceScenario
        {
            protected string resultMessageShouldContain;

            public MarkCertificateForDeselectCommandHandleReturnsRejectedScenario(string rejectedResultMessageStringToFind)
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
        private class MarkCertificateForDeselectCommandHandleSuccessfulScenario : MarkCertificateForDeselectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificateForDeselectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "bdickinson",
                                AbimId = "123456"
                            })
                            .With(cmd => cmd.SubmittedDate = new DateTime(2020, 9, 18))
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
                latestIssuance.DeselectionSubmittedDate.ShouldNotBeNull();
                latestIssuance.DeselectionEffectiveDate.ShouldNotBeNull(); //TODO: Refine
                latestIssuance.AuditData.Modified.ShouldNotBeNull();
                latestIssuance.DeselectionType.ShouldBe(Resources.DeselectionType.Self);
                latestIssuance.AuditData.ModifiedBy.ShouldBe(Command.UserInfo.Username);
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
        private class MarkCertificateForDeselectCommandHandleFailsValidationScenario : MarkCertificateForDeselectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForDeselectCommandHandleFailsValidationScenario() : base("Validation Failed")
            { }

            public void GivenIInputAnInvalidCommand()
            {
                var failures =
                    new List<ValidationFailure>(1);

                failures.Add(new ValidationFailure("some property", "some error"));

                var validationResult =
                    new ValidationResult(failures);

                My<IValidator<MarkCertificateForDeselectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificateForDeselectCommand>()))
                    .Returns(validationResult);

                Command = CommandBuilder<MarkCertificateForDeselectCommand>
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
        private class MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario : MarkCertificateForDeselectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario() : base("failed to update")
            { 
            }

            protected override void SetupCredentialRespository()
            {
                base.SetupCredentialRespository();
                My<ICredentialRepository>()
                    .Setup(mock => mock.Update(Credential, It.IsAny<string>()))
                    .Throws(new ApplicationException("Something bad happened!"));
            }

            public void GivenIHaveAValidCommand()
            {
                Command = 
                    CommandBuilder<MarkCertificateForDeselectCommand>
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
        private class MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario : MarkCertificateForDeselectCommandHandleReturnsRejectedScenario
        {
            private ValidationResult badValidationResult;

            public MarkCertificateForDeselectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario() : base("Domain validation failed")
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
                    CommandBuilder<MarkCertificateForDeselectCommand>
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
        private class MarkCertificateForDeselectCommandHandleReturnsRejectedWhenExceptionOccursScenario : MarkCertificateForDeselectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForDeselectCommandHandleReturnsRejectedWhenExceptionOccursScenario() : base("no issuances")
            {}

            protected override void SetupCredential()
            {
                //The lack of issuances for this credential will cause an 
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
                    CommandBuilder<MarkCertificateForDeselectCommand>
                        .Valid()
                        .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }
        }
        #endregion Exception Scenario

        #region Warning - Already Deselection Elected Scenario
        private class MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedAlreadyProcesseddOccursScenario : MarkCertificateForDeselectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedAlreadyProcesseddOccursScenario() : base("")
            {
            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificateForDeselectCommand>
                             .Valid()
                             .With(cmd => cmd.UserInfo = new UserInfo()
                             {
                                 Username = "bdickinson",
                                 AbimId = "123456"
                             })
                             .With(cmd => cmd.SubmittedDate = new DateTime(2020, 9, 18))
                             .With(cmd => cmd.CredentialId = Guid.NewGuid())
                             .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                var latestIssuance = Credential.Issuances.OrderByDescending(issuance => issuance.IssuanceDate).First();
                Credential.ExternalId = Guid.NewGuid();
                latestIssuance.DeselectionProcessedDate = new DateTime(2020, 11, 1);
                latestIssuance.DeselectionSubmittedDate = new DateTime(2020, 11, 1);
                GivenIInputAValidCommand();
                resultMessageShouldContain = $"Credential {Credential.ExternalId} was already marked for deselection, and processed on {latestIssuance.DeselectionProcessedDate}.";
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
        #endregion Warning - Already Deselection Elected Scenario 

        #region Warning - Deselection Elected Not Processed Scenario
        private class MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedNotProcessedOccursScenario : MarkCertificateForDeselectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForDeselectCommandHandleReturnsRejectedWhenWarningForDeselectionElectedNotProcessedOccursScenario() : base("")
            {
            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificateForDeselectCommand>
                             .Valid()
                             .With(cmd => cmd.UserInfo = new UserInfo()
                             {
                                 Username = "bdickinson",
                                 AbimId = "123456"
                             })
                             .With(cmd => cmd.SubmittedDate = new DateTime(2020, 9, 18))
                             .With(cmd => cmd.CredentialId = Guid.NewGuid())
                             .Build();
            }

            protected override void SetupCredential()
            {
                base.SetupCredential();
                var latestIssuance = Credential.Issuances.OrderByDescending(issuance => issuance.IssuanceDate).First();
                Credential.ExternalId = Guid.NewGuid();
                latestIssuance.DeselectionProcessedDate = null;
                latestIssuance.DeselectionSubmittedDate = new DateTime(2020, 11, 1);
                GivenIInputAValidCommand();
                resultMessageShouldContain = $"Credential {Credential.ExternalId} has already marked for deselection.";
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
        #endregion Warning - Deselection Elected Not Processed Scenario

        #endregion Scenarios
    }
}
