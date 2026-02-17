using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults.LookbackDateLog;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Tests.Scenarios.Services.LookbackDateLog.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using Moq;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.LookbackDateLog
{
    public class AddLookbackDateLogEntryCommandSpec
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



        /// <summary>
        /// AddLookbackDateLogEntryCommandScenario
        /// </summary>
        public abstract class AddLookbackDateLogEntryCommandScenario : LookbackDateLogScenario
        {
            protected ILookbackDateLogService Service { get; set; }
            protected AddLookbackDateLogEntry Command { get; set; }
            protected AddLookbackDateLogCommandResult CommandResult { get; set; }
            protected Exception ExceptionCaught { get; set; }
        }


        public class ReturnsAcceptedScenario : AddLookbackDateLogEntryCommandScenario
        {
            protected override void PostSetup()
            {
                Service = Container.GetInstance<LookbackDateLogService>();

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<AddLookbackDateLogEntry>())
                    .Returns(My<IValidator<AddLookbackDateLogEntry>>().Object);

                My<IValidator<AddLookbackDateLogEntry>>()
                    .Setup(o => o.Validate(It.IsAny<AddLookbackDateLogEntry>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<ILookbackDateLogRepository>()
               .Setup(o => o.Add(It.IsAny<App.Domain.LookbackDateLog>(), It.IsAny<string>()))
               .Returns(new AbimValidationResult() { Succeeded = true });
            }

            protected override void PreSetup()
            {

            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<AddLookbackDateLogEntry>
                            .Valid()
                            .With(cmd => cmd.UserName = "TestUser")
                            .Build();
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (AddLookbackDateLogCommandResult)(Service.Handle(Command));
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
        /// ThrowsDatabaseExecptionScenario
        /// </summary>
        public class ThrowsDatabaseExecptionScenario : AddLookbackDateLogEntryCommandScenario
        {
            protected override void PostSetup()
            {
                Service = Container.GetInstance<LookbackDateLogService>();

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<AddLookbackDateLogEntry>())
                    .Returns(My<IValidator<AddLookbackDateLogEntry>>().Object);

                My<IValidator<AddLookbackDateLogEntry>>()
                    .Setup(o => o.Validate(It.IsAny<AddLookbackDateLogEntry>()))
                    .Returns(new FluentValidation.Results.ValidationResult());

                My<ILookbackDateLogRepository>()
                    .Setup(o => o.Add(It.IsAny<App.Domain.LookbackDateLog>(), It.IsAny<string>()))
                    .Throws(new Exception(ExceptionText));
            }

            protected override void PreSetup()
            {

            }

            public void GivenIInputAValidCommand()
            {
                Command = CommandBuilder<AddLookbackDateLogEntry>
                            .Valid()
                            .With(cmd => cmd.UserName = "TestUser")
                            .Build();
            }

            public void WhenICallHandle()
            {
                try
                {
                    CommandResult = (AddLookbackDateLogCommandResult)(Service.Handle(Command));
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
    }
}
