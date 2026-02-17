using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.Tests.Scenarios.Validators.Credential.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Credential
{
    [Story(
        AsA = "backend process",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely call Handle() on this command"
        )]
    [TestFixture]
    public class ReissueCommandValidationSpec
    {
        [TestCase]
        [WorkItem(73199)]
        public void ValidReissueCommandPassesValidation()
        {
            new ValidReissueCommandPassesValidation().BDDfy();
        } 

        [TestCase]
        [WorkItem(73199)]
        public void MissingCredentialIdInReissueCommandFailsValidation()
        {
            new MissingCredentialIdInReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void TooOldIssuanceDateInReissueCommandFailsValidation()
        {
            new TooOldIssuanceDateInReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void TooFutureIssuanceDateInReissueCommandFailsValidation()
        {
            new TooFutureIssuanceDateInReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void TooOldScheduledUpdateInReissueCommandFailsValidation()
        {
            new TooOldScheduledUpdateInReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void TooFutureScheduledUpdateInReissueCommandFailsValidation()
        {
            new TooFutureScheduledUpdateInReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void NullCreatedByInReissueCommandFailsValidation()
        {
            new NullCreatedByInReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void EmptyCreatedByInReissueCommandFailsValidation()
        {
            new EmptyCreatedByInReissueCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestReissueCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static ReissueCommand BuildValid()
        {
            return CommandBuilder<ReissueCommand>
                        .Valid()
                        .With(cmd => cmd.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ProcessingDate = cmd.IssuanceDate.AddYears(-1))
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class ReissueCommandValidationFailureBaseScenario
        : CredentialValidationScenario
    {
        protected ReissueCommand Command { get; set; }
        ReissueCommandValidator Validator { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ReissueCommandValidator();
            EmailBuilder = new EmailBuilder();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestReissueCommandBuilder.BuildValid();
            SetInvalidProperty();
        }

        protected abstract void SetInvalidProperty();

        protected abstract string ExpectedErrorMessage();

        public void WhenICallValidate()
        {
            try
            {
                ValidationResult = Validator.Validate(Command);
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }

        public void AndThenTheValidationErrorsShouldContainTheRequiredMessage()
        {
            ValidationResult.IsValid.Should().BeFalse();
            string expectedMessage = ExpectedErrorMessage().ToLower();
            ValidationResult.Errors.Should().Contain(msg => msg.ErrorMessage.ToLower().Contains(expectedMessage));
        }
    }

    #endregion

    #region Success

    /// <summary>
    /// The valid command scenario
    /// </summary>
    public class ValidReissueCommandPassesValidation
        : CredentialValidationScenario
    {
        ReissueCommandValidator Validator { get; set; }
        ReissueCommand Command { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ReissueCommandValidator();
            EmailBuilder = new EmailBuilder();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestReissueCommandBuilder.BuildValid();
            Command.IssuanceDate = Command.ProcessingDate.AddMonths(Random.Next(1, 10));
            Command.ScheduledUpdate = Command.ProcessingDate.AddMonths(Random.Next(1, 10));
        }

        public void WhenICallValidate()
        {
            try
            {
                ValidationResult = Validator.Validate(Command);
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheValidationShouldHavePassed()
        {
            ValidationResult.IsValid.Should().BeTrue();
            ValidationResult.Errors.Should().BeEmpty();
        }
    }

    #endregion

    #region Failure

    /// <summary>
    /// The missing CredentialId scenario
    /// </summary>
    public class MissingCredentialIdInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CredentialId = Guid.Empty;
        }

        protected override string ExpectedErrorMessage()
        {
            return "CredentialId is required";
        }
    }

    /// <summary>
    /// The too old issuance date scenario
    /// </summary>
    public class TooOldIssuanceDateInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1900, 1935)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "IssuanceDate may not be prior to 1936";
        }
    }

    /// <summary>
    /// The too future issuance date scenario
    /// </summary>
    public class TooFutureIssuanceDateInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "IssuanceDate may not be more than a year from ProcessingDate:'"+Command.ProcessingDate.ToShortDateString()+"'";
        }
    }

    /// <summary>
    /// The too old scheduled update scenario
    /// </summary>
    public class TooOldScheduledUpdateInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1900, 1935)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "ScheduledUpdate may not be prior to 1936";
        }
    }

    /// <summary>
    /// The too future scheduled update scenario
    /// </summary>
    public class TooFutureScheduledUpdateInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "ScheduledUpdate may not be more than 13 months from ProcessingDate:'" + Command.ProcessingDate.ToShortDateString() + "'";
        }
    }

    /// <summary>
    /// The null CreatedBy scenario
    /// </summary>
    public class NullCreatedByInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CreatedBy = null;
        }

        protected override string ExpectedErrorMessage()
        {
            return "CreatedBy is required";
        }
    }

    /// <summary>
    /// The empyu CreatedBy scenario
    /// </summary>
    public class EmptyCreatedByInReissueCommandFailsValidation
        : ReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CreatedBy = "";
        }

        protected override string ExpectedErrorMessage()
        {
            return "CreatedBy is required";
        }
    }

    #endregion Scenarios
}
