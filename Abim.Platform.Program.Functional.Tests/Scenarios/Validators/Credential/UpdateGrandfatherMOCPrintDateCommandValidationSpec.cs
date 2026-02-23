using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
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
    public class UpdateGrandfatherMOCPrintDateCommandValidationSpec
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
        public void Should_Fail_Validation_When_ModifiedBy_Is_Empty()
        {
            new ShouldFailValidationWhenModifiedByIsEmpty().BDDfy();
        }
        #region Setup
        /// <summary>
        /// Command Builder
        /// </summary>
        public static class UpdateGrandfatherMOCPrintDateCommandBuilder
        {
            public static UpdateGrandfatherMOCPrintDateCommand BuildValid()
            {
                return CommandBuilder<UpdateGrandfatherMOCPrintDateCommand>
                            .Valid()
                            .Build();
            }
        }
        #endregion Setup

        #region Scenarios

        public abstract class UpdateGrandfatherMOCPrintDateCommandValidationBaseScenario
        {
            protected UpdateGrandfatherMOCPrintDateCommand _command;
            protected UpdateGrandfatherMOCPrintDateCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new UpdateGrandfatherMOCPrintDateCommandValidator();
            }
        }

        public class ShouldPassValidationWhenAllRequiredValuesArePresent
            : UpdateGrandfatherMOCPrintDateCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = UpdateGrandfatherMOCPrintDateCommandBuilder.BuildValid();
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
        public abstract class UpdateGrandfatherMOCPrintDateCommandValidationFailureBaseScenario 
            : UpdateGrandfatherMOCPrintDateCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = UpdateGrandfatherMOCPrintDateCommandBuilder.BuildValid();
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
            : UpdateGrandfatherMOCPrintDateCommandValidationFailureBaseScenario
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

        private class ShouldFailValidationWhenModifiedByIsEmpty
            : UpdateGrandfatherMOCPrintDateCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "ModifiedBy is required";
            }

            protected override void SetInvalidProperty()
            {
                _command.ModifiedBy = string.Empty;
            }
        }

        #endregion Scenarios
    }
}
