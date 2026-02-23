using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Credential
{
    [Story(
        AsA = "process updating a credential",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class WithdrawCredentialCommandValidationSpec
    {
        [Test]
        public void Should_Pass_Validation_When_All_Required_Values_Are_Present()
        {
            new ShouldPassValidationWhenAllRequiredValuesArePresent().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_CredentialId_Is_Null()
        {
            new ShouldFailValidationWhenCredentialIdIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Is_Null()
        {
            new ShouldFailValidationWhenUserInfoIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserName_Is_Null()
        {
            new ShouldFailValidationWhenUserNameIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_WithdrawnStatus_Is_Wrong()
        {
            new ShouldFailValidationWhenWithdrawnStatusIsWrong().BDDfy();
        }
        #region Setup
        /// <summary>
        /// Command Builder
        /// </summary>
        public static class WithdrawCredentialCommandBuilder
        {
            public static WithdrawCredentialCommand BuildValid()
            {
                return CommandBuilder<WithdrawCredentialCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo() { Username = "some name" })
                            .With(cmd=>cmd.WithdrawnStatus= IssuanceStatusType.Revoked)
                            .Build();
            }
        }
        #endregion Setup

        #region Scenarios

        public abstract class WithdrawCredentialCommandValidationBaseScenario
        {
            protected WithdrawCredentialCommand _command;
            protected WithdrawCredentialCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new WithdrawCredentialCommandValidator();
            }
        }

        public class ShouldPassValidationWhenAllRequiredValuesArePresent
            : WithdrawCredentialCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = WithdrawCredentialCommandBuilder.BuildValid();
            }

            protected void WhenICallValidate()
            {
                try
                {
                    _validationResult = _validator.Validate(_command);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.Should().BeNull();
            }

            protected void AndTheValidationShouldHaveSucceeded()
            {
                _validationResult.IsValid.Should().BeTrue();
                _validationResult.Errors.Should().BeEmpty();
            }
        }

        /// <summary>
        /// Base class for validation failure scenarios
        /// </summary>
        public abstract class WithdrawCredentialCommandValidationFailureBaseScenario 
            : WithdrawCredentialCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = WithdrawCredentialCommandBuilder.BuildValid();
                SetInvalidProperty();
            }

            protected abstract void SetInvalidProperty();

            protected abstract string ExpectedErrorMessage();

            protected void WhenICallValidate()
            {
                try
                {
                    _validationResult = _validator.Validate(_command);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.Should().BeNull();
            }

            protected void AndThenTheValidationShouldHaveFailed()
            {
                _validationResult.IsValid.Should().BeFalse();
                _validationResult.Errors.Should().NotBeEmpty();
            }

            protected void AndThenTheValidationErrorsShouldContainTheRequiredMessage()
            {
                _validationResult.IsValid.Should().BeFalse();
                string expectedMessage = ExpectedErrorMessage().ToLower();
                _validationResult.Errors.Should().Contain(msg => msg.ErrorMessage.ToLower().Contains(expectedMessage));
            }
        }

        private class ShouldFailValidationWhenCredentialIdIsNull
            : WithdrawCredentialCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "CredentialId is required";
            }

            protected override void SetInvalidProperty()
            {
                _command.CredentialId = Guid.Empty;
            }
        }


        private class ShouldFailValidationWhenWithdrawnStatusIsWrong
            : WithdrawCredentialCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "Withdrawn Status must be one of these values: Revoked, Surrendered or Suspended";
            }

            protected override void SetInvalidProperty()
            {
                _command.WithdrawnStatus = IssuanceStatusType.Active;
            }
        }


        private class ShouldFailValidationWhenUserInfoIsNull : WithdrawCredentialCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "UserInfo must not be null";
            }

            protected override void SetInvalidProperty()
            {
                _command.UserInfo = null;
            }
        }

        private class ShouldFailValidationWhenUserNameIsNull : WithdrawCredentialCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "UserInfo's UserName cannot be empty";
            }

            protected override void SetInvalidProperty()
            {
                _command.UserInfo = new UserInfo();
            }
        }

        #endregion Scenarios
    }
}
