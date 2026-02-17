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
    public class SetIssuanceToMaintainedCommandValidationSpec
    {
        [TestCase]
        [WorkItem(73199)]
        public void ValidSetIssuanceToMaintainedCommandPassesValidation()
        {
            new ValidSetIssuanceToMaintainedCommandPassesValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void MissingCredentialIdInSetIssuanceToMaintainedCommandFailsValidation()
        {
            new MissingCredentialIdInSetIssuanceToMaintainedCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void NullModifiedByInSetIssuanceToMaintainedCommandFailsValidation()
        {
            new NullModifiedByInSetIssuanceToMaintainedCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73199)]
        public void EmptyModifiedByInSetIssuanceToMaintainedCommandFailsValidation()
        {
            new EmptyModifiedByInSetIssuanceToMaintainedCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestSetIssuanceToMaintainedCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static SetIssuanceToMaintainedCommand BuildValid()
        {
            return CommandBuilder<SetIssuanceToMaintainedCommand>
                        .Valid()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class SetIssuanceToMaintainedCommandValidationFailureBaseScenario
        : CredentialValidationScenario
    {
        protected SetIssuanceToMaintainedCommand Command    { get; set; }
        SetIssuanceToMaintainedCommandValidator Validator   { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        ValidationResult ValidationResult                          { get; set; }
        new EmailBuilder EmailBuilder                                  { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new SetIssuanceToMaintainedCommandValidator();
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
            Command = UnitTestSetIssuanceToMaintainedCommandBuilder.BuildValid();
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
    public class ValidSetIssuanceToMaintainedCommandPassesValidation
        : CredentialValidationScenario
    {
        SetIssuanceToMaintainedCommandValidator Validator   { get; set; }
        SetIssuanceToMaintainedCommand Command              { get; set; }
        new Exception ExceptionCaught                                  { get; set; }
        ValidationResult ValidationResult                          { get; set; }
        new EmailBuilder EmailBuilder                                  { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new SetIssuanceToMaintainedCommandValidator();
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
            Command = UnitTestSetIssuanceToMaintainedCommandBuilder.BuildValid();
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
    public class MissingCredentialIdInSetIssuanceToMaintainedCommandFailsValidation
        : SetIssuanceToMaintainedCommandValidationFailureBaseScenario
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
    /// The null ModifiedBy scenario
    /// </summary>
    public class NullModifiedByInSetIssuanceToMaintainedCommandFailsValidation
        : SetIssuanceToMaintainedCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ModifiedBy = null;
        }

        protected override string ExpectedErrorMessage()
        {
            return "ModifiedBy is required";
        }
    }

    /// <summary>
    /// The empty ModifiedBy scenario
    /// </summary>
    public class EmptyModifiedByInSetIssuanceToMaintainedCommandFailsValidation
        : SetIssuanceToMaintainedCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ModifiedBy = "";
        }

        protected override string ExpectedErrorMessage()
        {
            return "ModifiedBy is required";
        }
    }

    #endregion Scenarios
}
