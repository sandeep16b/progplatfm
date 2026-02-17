using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using Abim.Platform.Program.Tests.Scenarios.Validators.Certification.Base;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process creating a credential",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class ReinstateCredentialCommandValidationSpec
    {
        [TestCase]
        public void ValidReinstateCredentialCommandPassesValidation()
        {
            new ValidReinstateCredentialCommandPassesValidation().BDDfy();
        }

        [TestCase]
        public void MissingCredentialIdInReinstateCredentialCommandFailsValidation()
        {
            new MissingCredentialIdInReinstateCredentialCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void MissingUserInfoInReinstateCredentialCommandFailsValidation()
        {
            new MissingUserInfoInReinstateCredentialCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void MissingUserNameInReinstateCredentialCommandFailsValidation()
        {
            new MissingUserInfoUserNameInReinstateCredentialCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestReinstateCredentialCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static ReinstateCredentialCommand BuildValid()
        {
            return CommandBuilder<ReinstateCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.UserInfo = new UserInfo { Username = "someUsername" })
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class ReinstateCredentialCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected ReinstateCredentialCommand Command { get; set; }
        ReinstateCredentialCommandValidator Validator { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ReinstateCredentialCommandValidator();
        }

        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestReinstateCredentialCommandBuilder.BuildValid();
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
            catch (Exception ex)
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
    public class ValidReinstateCredentialCommandPassesValidation
        : CertificationValidationScenario
    {
        ReinstateCredentialCommandValidator Validator { get; set; }
        ReinstateCredentialCommand Command { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ReinstateCredentialCommandValidator();
        }

        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestReinstateCredentialCommandBuilder.BuildValid();
        }

        public async Task WhenICallValidate()
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
    }

    #endregion

    #region Failure

    /// <summary>
    /// The missing CredentialId scenario
    /// </summary>
    public class MissingCredentialIdInReinstateCredentialCommandFailsValidation
        : ReinstateCredentialCommandValidationFailureBaseScenario
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
    /// The missing MemberId scenario
    /// </summary>
    public class MissingUserInfoInReinstateCredentialCommandFailsValidation
        : ReinstateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.UserInfo = null;
        }

        protected override string ExpectedErrorMessage()
        {
            return "UserInfo must not be null";
        }
    }

    /// <summary>
    /// The missing MemberId scenario
    /// </summary>
    public class MissingUserInfoUserNameInReinstateCredentialCommandFailsValidation
        : ReinstateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.UserInfo = new UserInfo();
        }

        protected override string ExpectedErrorMessage()
        {
            return "UserInfo's UserName cannot be empty";
        }
    }
    #endregion Scenarios
}
