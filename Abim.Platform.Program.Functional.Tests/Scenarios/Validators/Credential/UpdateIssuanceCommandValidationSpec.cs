using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
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
    public class UpdateIssuanceCommandValidationSpec
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
        public void Should_Fail_Validation_When_IssuanceDate_Is_NotSet()
        {
            new ShouldFailValidationWhenIssuanceDateIsNotSet().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_EffectiveDate_Is_NotSet()
        {
            new ShouldFailValidationWhenEffectiveDateIsNotSet().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_SourceId_Is_NotSet()
        {
            new ShouldFailValidationWhenSourceIdIsNotSet().BDDfy();
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
        #region Setup
        /// <summary>
        /// Command Builder
        /// </summary>
        public static class UpdateIssuanceCommandBuilder
        {
            public static UpdateIssuanceCommand BuildValid()
            {
                return CommandBuilder<UpdateIssuanceCommand>
                            .Valid()
                            .With(cmd => cmd.UserInfo = new UserInfo { Username = "someUsername" })
                            .Build();
            }
        }
        #endregion Setup

        #region Scenarios

        public abstract class UpdateIssuanceCommandValidationBaseScenario
        {
            protected UpdateIssuanceCommand _command;
            protected UpdateIssuanceCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new UpdateIssuanceCommandValidator();
            }
        }

        public class ShouldPassValidationWhenAllRequiredValuesArePresent
            : UpdateIssuanceCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = UpdateIssuanceCommandBuilder.BuildValid();
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
        public abstract class UpdateIssuanceCommandValidationFailureBaseScenario 
            : UpdateIssuanceCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = UpdateIssuanceCommandBuilder.BuildValid();
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
            : UpdateIssuanceCommandValidationFailureBaseScenario
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

        private class ShouldFailValidationWhenIssuanceDateIsNotSet
           : UpdateIssuanceCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "IssuanceDate is required";
            }

            protected override void SetInvalidProperty()
            {
                _command.IssuanceDate = DateTime.MinValue;
            }
        }

        private class ShouldFailValidationWhenEffectiveDateIsNotSet : UpdateIssuanceCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "EffectiveDate is required";
            }

            protected override void SetInvalidProperty()
            {
                _command.EffectiveDate = DateTime.MinValue;
            }
        }

        private class ShouldFailValidationWhenSourceIdIsNotSet : UpdateIssuanceCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "SourceId is required";
            }

            protected override void SetInvalidProperty()
            {
                _command.SourceId = Guid.Empty;
            }
        }
        private class ShouldFailValidationWhenUserInfoIsNull
            : UpdateIssuanceCommandValidationFailureBaseScenario
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

        private class ShouldFailValidationWhenUserNameIsNull
            : UpdateIssuanceCommandValidationFailureBaseScenario
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
