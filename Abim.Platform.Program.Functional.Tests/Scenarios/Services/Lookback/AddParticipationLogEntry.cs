using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Lookback
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the LookbackService",
        SoThat = "it can handle the AddParticipationLookbackLog Command"
        )]
    [TestFixture]
    public class AddParticipationLogEntrySpec
    {
        [TestCase]
        public void CommandHandleReturnsAcceptedOnSuccess()
        {
            new ReturnsAcceptedScenario().BDDfy();
        }

        [TestCase]
        public void ReturnsRejectedWhenDatabaseSaveFails()
        {
            new ThrowsDatabaseExecptionScenario().BDDfy();
        }

        [TestCase]
        public void ReturnsRejectedWhenCredentialNotFound()
        {
            new CannotFindCredentialScenario().BDDfy();
        }

        #region Scenarios

        /// <summary>
        /// AddCertificationLogEntryScenario
        /// </summary>
        /// <remarks>
        /// Base class for all AddParticipationLogEntry Commands scenarios
        /// </remarks>
        public abstract class AddParticipationLogEntryScenario : LookbackLogScenario
        {
            protected ILookbackLogService Service { get; set; }
            protected AddParticipationLookbackLog Command { get; set; }
            protected AddLookbackLogCommandResult CommandResult { get; set; }
            //protected Exception ExceptionCaught { get; set; }
        }

        /// <summary>
        /// ReturnsAcceptedScenario class
        /// </summary>
        /// <remarks>
        /// A Valid command is passed to the handle method of the service should return accepted.
        /// </remarks>
        public class ReturnsAcceptedScenario : AddParticipationLogEntryScenario
        {
            protected override void PostSetup()
            {
                var cert = Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build());
                var existing = Credential.Create(cert, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

                Service = Container.GetInstance<LookbackLogService>();

                My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<AddParticipationLookbackLog>())
                    .Returns(My<IValidator<AddParticipationLookbackLog>>().Object);

                My<IValidator<AddParticipationLookbackLog>>()
                    .Setup(o => o.Validate(It.IsAny<AddParticipationLookbackLog>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<ILookbackLogRepository>()
               .Setup(o => o.Add(It.IsAny<LookbackLog>(), It.IsAny<string>()))
               .Returns(new AbimValidationResult() { Succeeded = true });
            }

            protected override void PreSetup()
            {

            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<AddParticipationLookbackLog>
                            .Valid()
                            .With(cmd => cmd.UserName = "TestUser")
                            .Build();
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (AddLookbackLogCommandResult)(Service.Handle(Command));
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
        }

        /// <summary>
        /// ThrowsDatabaseExecptionScenario class
        /// </summary>
        /// <remarks>
        /// The handle method should return a rejction if the database throws any kind of error.
        /// </remarks>
        public class ThrowsDatabaseExecptionScenario : AddParticipationLogEntryScenario
        {
            protected override void PostSetup()
            {
                var cert = Certification.Create(null, Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build()), EnumAttributes.RandomEntry<CertificationType>(), RandomString.Build(), RandomString.Build(), RandomString.Build());
                var existing = Credential.Create(cert, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), EnumAttributes.RandomEntry<PathwayType>(), null, null, RandomString.Build());

                Service = Container.GetInstance<LookbackLogService>();

                My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<AddParticipationLookbackLog>())
                    .Returns(My<IValidator<AddParticipationLookbackLog>>().Object);

                My<IValidator<AddParticipationLookbackLog>>()
                    .Setup(o => o.Validate(It.IsAny<AddParticipationLookbackLog>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<ILookbackLogRepository>()
                    .Setup(o => o.Add(It.IsAny<LookbackLog>(), It.IsAny<string>()))
                    .Throws(new Exception(ExceptionText));
            }

            protected override void PreSetup()
            {

            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<AddParticipationLookbackLog>
                            .Valid()
                            .Build();
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (AddLookbackLogCommandResult)(Service.Handle(Command));
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

            public void AndThenTheCommandResultStatusShouldBeRejected()
            {
                CommandResult.Status.Should().Be(CommandStatus.Rejected);
            }

            public void AndThenTheCommandResultDotSucceededShouldBeFalse()
            {
                CommandResult.Succeeded.Should().BeFalse();
            }

            public void AndThenTheCommandResultValidationShouldShowSuccess()
            {
                CommandResult.Validation.Succeeded.Should().BeTrue();
            }

            public void AndThenTheCommandResultMessageShouldMentionTheDatabase()
            {
                CommandResult.Message.ToLower().Should().Contain("database");
            }
        }

        /// <summary>
        /// CannotFindCertificationScenario class
        /// </summary>
        /// <remarks>
        /// The handle method should return a rejection meesage if the credential id 
        /// does nto resolve to an existing credential
        /// </remarks>
        public class CannotFindCredentialScenario : AddParticipationLogEntryScenario
        {
            protected override void PostSetup()
            {
                Credential existing = null;
                Service = Container.GetInstance<LookbackLogService>();

                My<ICredentialService>()
                .Setup(o => o.Load(It.IsAny<Guid>()))
                .Returns(existing);

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<AddParticipationLookbackLog>())
                    .Returns(My<IValidator<AddParticipationLookbackLog>>().Object);

                My<IValidator<AddParticipationLookbackLog>>()
                    .Setup(o => o.Validate(It.IsAny<AddParticipationLookbackLog>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<ILookbackLogRepository>()
                    .Setup(o => o.Add(It.IsAny<LookbackLog>(), It.IsAny<string>()))
                    .Throws(new Exception(ExceptionText));
            }

            protected override void PreSetup()
            {

            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<AddParticipationLookbackLog>
                            .Valid()
                            .Build();
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (AddLookbackLogCommandResult)(Service.Handle(Command));
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

            public void AndThenTheCommandResultStatusShouldBeRejected()
            {
                CommandResult.Status.Should().Be(CommandStatus.Rejected);
            }

            public void AndThenTheCommandResultDotSucceededShouldBeFalse()
            {
                CommandResult.Succeeded.Should().BeFalse();
            }
        }

        #endregion
    }
}
