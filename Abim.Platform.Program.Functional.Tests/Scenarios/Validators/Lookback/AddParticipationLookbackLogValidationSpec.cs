using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Lookback;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Lookback
{

    [Story(
        AsA = "process adding an participation lookbacklog entry",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class AddParticipationLookbackLogValidationSpec
    {
        [TestCase]
        public void ValidAddParticipationLookupLogCommandPassesValidation()
        {
            new ValidAddParticipationLookupLogCommandPassesValidation().BDDfy();
        }

        [TestCase]
        public void MissingCredentialIdInAddParticipationLookupLogCommandFailsValidation()
        {
            new NullCredentialIdInAddParticipationLookupLogCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void MissingUsernameInAddParticipationLookupLogCommandFailsValidation()
        {
            new NullUsernameInAddParticipationLookupLogCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    // <summary>
    /// Command Builder
    /// </summary>
    public static class AddParticipationLookupCommandBuilder
    {
        public static Random Random = new Random();
        public static AddParticipationLookbackLog BuildValid()
        {
            return CommandBuilder<AddParticipationLookbackLog>
                        .Valid()
                        .Build();
        }
    }

    #endregion

    #region Scenarios

    /// <summary>
    /// 
    /// </summary>
    public class NullUsernameInAddParticipationLookupLogCommandFailsValidation :
        LookbackLogValidatorScenario

    {
        protected AddParticipationLookbackLog Command { get; set; }
        protected AddParticipationLookbackLogCommandValidator Validator { get; set; }
        protected Exception ExceptionCaught { get; set; }

        protected override string ExpectedErrorMessage()
        {
            return "UserName is required";
        }

        public void GivenIPassACommandWithAnNullUserName()
        {
            Command = AddParticipationLookupCommandBuilder.BuildValid();
            Command.UserName = null;
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Command);
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

        protected override void PostSetup()
        {

        }

        protected override void PreSetup()
        {
            Validator = new AddParticipationLookbackLogCommandValidator();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class NullCredentialIdInAddParticipationLookupLogCommandFailsValidation :
        LookbackLogValidatorScenario

    {
        protected AddParticipationLookbackLog Command { get; set; }
        protected AddParticipationLookbackLogCommandValidator Validator { get; set; }
        protected Exception ExceptionCaught { get; set; }

        protected override string ExpectedErrorMessage()
        {
            return "CredentialId is required";
        }

        public void GivenIPassACommandWithAnNullUserName()
        {
            Command = AddParticipationLookupCommandBuilder.BuildValid();
            Command.CredentialId = Guid.Empty;
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Command);
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

        protected override void PostSetup()
        {

        }

        protected override void PreSetup()
        {
            Validator = new AddParticipationLookbackLogCommandValidator();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class ValidAddParticipationLookupLogCommandPassesValidation :
        LookbackLogValidatorScenario
    {
        protected AddParticipationLookbackLog Command { get; set; }
        protected AddParticipationLookbackLogCommandValidator Validator { get; set; }

        public void GivenIInputAValidCommand()
        {
            Command = AddParticipationLookupCommandBuilder.BuildValid();
        }

        protected override string ExpectedErrorMessage()
        {
            throw new NotImplementedException();
        }

        public void WhenICallValidate()
        {
            try
            {
                ValidationResult = Validator.Validate(Command);
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

        public void AndThenTheValidationShouldHavePassed()
        {
            ValidationResult.IsValid.Should().BeTrue();
            ValidationResult.Errors.Should().BeEmpty();
        }

        protected override void PostSetup()
        {

        }

        ///
        protected override void PreSetup()
        {
            Validator = new AddParticipationLookbackLogCommandValidator();
        }
    }

    #endregion
}
