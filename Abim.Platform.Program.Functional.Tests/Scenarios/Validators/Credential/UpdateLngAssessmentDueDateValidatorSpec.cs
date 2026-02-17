using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using FluentAssertions;
using FluentValidation.Results;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using Abim.Platform.Program.WebApi.Authentication;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Credential
{
    [Story(
        AsA = "process updating a credential",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class UpdateLngAssessmentDueDateValidator
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
        public void Should_Fail_Validation_When_Year_Is_Before2022()
        {
            new ShouldFailValidationWhenYearIsBefore2022().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_Year_Is_After3000()
        {
            new ShouldFailValidationWhenYearIsAfter3000().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_BothValuesSet()
        {
            new ShouldFailValidationWhenBothValuesSet().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_BothValuesSet_True()
        {
            new ShouldFailValidationWhenBothValuesSetTrue().BDDfy();
        }

        #region Setup
        /// <summary>
        /// Command Builder
        /// </summary>
        public static class UpdateLngAssessmentDueDateCommandBuilder
        {
            public static UpdateLngAssessmentDueDateCommand BuildValid()
            {
                return CommandBuilder<UpdateLngAssessmentDueDateCommand>
                            .Valid()
                            .With(cmd => cmd.Year = 2022)
                            .With(cmd => cmd.MetParticipationStatus = true)
                            .With(cmd => cmd.PassSummativeDecision = null)
                            .Build();
            }
        }
        #endregion Setup

        #region Scenarios

        public abstract class UpdateLngAssessmentDueDateCommandValidationBaseScenario
        {
            protected UpdateLngAssessmentDueDateCommand _command;
            protected UpdateLngAssessmentDueDateCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new UpdateLngAssessmentDueDateCommandValidator();
            }
        }

        public class ShouldPassValidationWhenAllRequiredValuesArePresent
            : UpdateLngAssessmentDueDateCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = UpdateLngAssessmentDueDateCommandBuilder.BuildValid();
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
        public abstract class UpdateLngAssessmentDueDateCommandValidationFailureBaseScenario 
            : UpdateLngAssessmentDueDateCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = UpdateLngAssessmentDueDateCommandBuilder.BuildValid();
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
            : UpdateLngAssessmentDueDateCommandValidationFailureBaseScenario
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

        private class ShouldFailValidationWhenYearIsBefore2022 : UpdateLngAssessmentDueDateCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "Year value should be greater than 2021";
            }

            protected override void SetInvalidProperty()
            {
                _command.Year = 2021;
            }
        }

        private class ShouldFailValidationWhenYearIsAfter3000 : UpdateLngAssessmentDueDateCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "Year value should be less than 3000";
            }

            protected override void SetInvalidProperty()
            {
                _command.Year = 3000;
            }
        }

        private class ShouldFailValidationWhenBothValuesSet : UpdateLngAssessmentDueDateCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "MetParticipationStatus and PassSummativeDecision CANNOT be set for both, only one is allowed";
            }

            protected override void SetInvalidProperty()
            {
                _command.PassSummativeDecision = true;
                _command.MetParticipationStatus = false;
            }
        }

        private class ShouldFailValidationWhenBothValuesSetTrue : UpdateLngAssessmentDueDateCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "MetParticipationStatus and PassSummativeDecision CANNOT have 'true' value for both";
            }

            protected override void SetInvalidProperty()
            {
                _command.PassSummativeDecision = true;
                _command.MetParticipationStatus = true;
            }
        }
        #endregion Scenarios
    }
}
