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
        SoThat = "it can handle the MarkCertificateForSelectCommand"
        )]
    [TestFixture]
    public class MarkCertificateForSelectCommandSpec
    {
        [TestCase]
        [WorkItem(187666)]
        public void MarkCertificateForSelectCommandHandleReturnsAcceptedOnSuccess()
        {
            new MarkCertificateForSelectCommandHandleSuccessfulScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187666)]
        public void MarkCertificateForSelectCommandHandleReturnsRejectedWhenCommandFailsValidation()
        {
            new MarkCertificateForSelectCommandHandleFailsValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187666)]
        public void MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsException()
        {
            new MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187666)]
        public void MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidation()
        {
            new MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187666)]
        public void MarkCertificateForSelectCommandHandleReturnsRejectedWhenExceptionOccurs()
        {
            new MarkCertificateForSelectCommandHandleReturnsRejectedWhenExceptionOccursScenario().BDDfy();
        }

        #region Scenario Base Classes
        private abstract class MarkCertificateForSelectCommandServiceScenario : CredentialServiceScenario
        {
            protected ICredentialService CredentialService { get; set; }
            protected MarkCertificateForSelectCommand Command { get; set; }
            protected MarkCertificateForSelectCommandResult CommandResult { get; set; }
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

                issuance.DeselectionEffectiveDate = new DateTime(DateTime.Now.Year,2,1);
                issuance.DeselectionSubmittedDate = DateTime.Now.AddYears(-1);

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
                    .Setup(o => o.GetValidatorInstance<MarkCertificateForSelectCommand>())
                    .Returns(My<IValidator<MarkCertificateForSelectCommand>>().Object);
                My<IValidator<MarkCertificateForSelectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificateForSelectCommand>()))
                    .Returns(new ValidationResult());
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (MarkCertificateForSelectCommandResult)(CredentialService.Handle(Command));
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

        private abstract class MarkCertificateForSelectCommandHandleReturnsRejectedScenario : MarkCertificateForSelectCommandServiceScenario
        {
            protected string resultMessageShouldContain;

            public MarkCertificateForSelectCommandHandleReturnsRejectedScenario(string rejectedResultMessageStringToFind)
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
        private class MarkCertificateForSelectCommandHandleSuccessfulScenario : MarkCertificateForSelectCommandServiceScenario
        {
            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<MarkCertificateForSelectCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo()
                            {
                                Username = "abim_user",
                                AbimId = "0123456"
                            })
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
                latestIssuance.DeselectionSubmittedDate.ShouldBeNull();
                latestIssuance.DeselectionEffectiveDate.ShouldBeNull(); 
                latestIssuance.AuditData.Modified.ShouldNotBeNull();
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
        private class MarkCertificateForSelectCommandHandleFailsValidationScenario : MarkCertificateForSelectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForSelectCommandHandleFailsValidationScenario() : base("Validation Failed")
            { }

            public void GivenIInputAnInvalidCommand()
            {
                var failures =
                    new List<ValidationFailure>(1);

                failures.Add(new ValidationFailure("some property", "some error"));

                var validationResult =
                    new ValidationResult(failures);

                My<IValidator<MarkCertificateForSelectCommand>>()
                    .Setup(o => o.Validate(It.IsAny<MarkCertificateForSelectCommand>()))
                    .Returns(validationResult);

                Command = CommandBuilder<MarkCertificateForSelectCommand>
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
        private class MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario : MarkCertificateForSelectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveThrowsExceptionScenario() : base("failed to update")
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
                    CommandBuilder<MarkCertificateForSelectCommand>
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
        private class MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario : MarkCertificateForSelectCommandHandleReturnsRejectedScenario
        {
            private ValidationResult badValidationResult;

            public MarkCertificateForSelectCommandHandleReturnsRejectedWhenDatabaseSaveFailsDomainValidationScenario() : base("Domain validation failed")
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
                    CommandBuilder<MarkCertificateForSelectCommand>
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
        private class MarkCertificateForSelectCommandHandleReturnsRejectedWhenExceptionOccursScenario : MarkCertificateForSelectCommandHandleReturnsRejectedScenario
        {
            public MarkCertificateForSelectCommandHandleReturnsRejectedWhenExceptionOccursScenario() : base("no issuances")
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
                    CommandBuilder<MarkCertificateForSelectCommand>
                        .Valid()
                        .Build();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }
        }
        #endregion Exception Scenario

        #endregion Scenarios
    }
}
