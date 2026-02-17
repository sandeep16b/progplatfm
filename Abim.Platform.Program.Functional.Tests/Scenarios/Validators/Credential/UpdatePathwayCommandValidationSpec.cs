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
    public class UpdatePathwayCommandValidationSpec
    {
        [Test]
        public void Should_Pass_Validation_When_All_Required_Values_Are_Present()
        {
            new ShouldPassValidationWhenAllRequiredValuesArePresent().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_Credential_Id_Is_Missing()
        {
            new ShouldFailValidationWhenCredentialIdIsMissing().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Is_Null()
        {
            new ShouldFailValidationWhenUserInfoIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Username_Is_Blank()
        {
            new ShouldFailValidationWhenUserInfoUsernameIsBlank().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Username_Is_Null()
        {
            new ShouldFailValidationWhenUserInfoUsernameIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_Pathway_Is_Not_Specified()
        {
            new ShouldFailValidationWhenPathwayIsNotSpecified().BDDfy();
        }

        #region Setup
        private static class UpdatePathwayCommandBuilder
        {
            public static UpdatePathwayCommand BuildValid()
            {
                return CommandBuilder<UpdatePathwayCommand>
                    .Valid()
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "UnitTest" })
                    .Build();
            }
        }

        #endregion Setup

        #region Scenarios

        #region Base Classes
        private abstract class UpdatePathwayCommandValidationBaseScenario
        {
            protected UpdatePathwayCommand _command;
            protected UpdatePathwayCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new UpdatePathwayCommandValidator();
            }
        }

        private abstract class UpdatePathwayCommandValidationFailureBaseScenario
            : UpdatePathwayCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = UpdatePathwayCommandBuilder.BuildValid();
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
        #endregion Base Classes

        private class ShouldPassValidationWhenAllRequiredValuesArePresent
            : UpdatePathwayCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = UpdatePathwayCommandBuilder.BuildValid();
            }

            private void WhenICallValidate()
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

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheValidationShouldHaveSucceeded()
            {
                _validationResult.IsValid.Should().BeTrue();
                _validationResult.Errors.Should().BeEmpty();
            }
        }

        private class ShouldFailValidationWhenCredentialIdIsMissing
            : UpdatePathwayCommandValidationFailureBaseScenario
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

        private class ShouldFailValidationWhenUserInfoIsNull
            : UpdatePathwayCommandValidationFailureBaseScenario
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

        private class ShouldFailValidationWhenUserInfoUsernameIsBlank
            : UpdatePathwayCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "UserInfo's UserName cannot be empty";
            }

            protected override void SetInvalidProperty()
            {
                _command.UserInfo.Username = "";
            }
        }

        private class ShouldFailValidationWhenUserInfoUsernameIsNull
            : UpdatePathwayCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "UserInfo's UserName cannot be empty";
            }

            protected override void SetInvalidProperty()
            {
                _command.UserInfo.Username = null;
            }
        }

        private class ShouldFailValidationWhenPathwayIsNotSpecified
            : UpdatePathwayCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "Pathway must be specified";
            }

            protected override void SetInvalidProperty()
            {
                _command.Pathway = 0;
            }
        }

        #endregion Scenarios
    }
}
