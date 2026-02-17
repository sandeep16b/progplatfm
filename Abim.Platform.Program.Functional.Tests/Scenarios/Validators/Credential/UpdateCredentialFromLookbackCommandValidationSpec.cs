

 
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using FluentAssertions;
using FluentValidation.Results;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Credential
{
    [Story(
        AsA = "process updating a credential",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class UpdateCredentialFromLookbackCommandValidationSpec
    {
        [Test]
        public void Should_Pass_Validation_When_All_Required_Values_Are_Present()
        {
            new ShouldPassValidationWhenAllRequiredValuesArePresent().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_Credential_Is_Null()
        {
            new ShouldFailValidationWhenCredentialIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_ModifiedBy_Is_Null()
        {
            new ShouldFailValidationWhenModifiedByIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_ModifiedBy_Is_Blank()
        {
            new ShouldFailValidationWhenModifiedByIsBlank().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_GracePeriodStart_Is_Null_And_GracePeriodEnd_Has_Value()
        {
            new ShouldFailValidationWhenGracePeriodStartIsNullAndGracePeriodEndHasValue().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_GracePeriodEnd_Is_Null_And_GracePeriodStart_Has_Value()
        {
            new ShouldFailValidationWhenGracePeriodEndIsNullAndGracePeriodStartHasValue().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_GracePeriodStart_Is_Greater_Than_GracePeriodEnd()
        {
            new ShouldFailValidationWhenGracePeriodStartIsGreaterThanGracePeriodEnd().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_IssuanceStatus_Is_Invalid_Value()
        {
            new ShouldFailValidationWhenIssuanceStatusIsInvalidValue().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_MaintenanceStatus_Is_Invalid_Value()
        {
            new ShouldFailValidationWhenMaintenanceStatusIsInvalidValue().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_LookbackDate_Is_Default_Value()
        {
            new ShouldFailValidationWhenLookbackDateIsDefaultValue().BDDfy();
        }

        #region Setup
        /// <summary>
        /// Command Builder
        /// </summary>
        public static class UpdateCredentialFromLookbackCommandBuilder
        {
            private static Random Random = new Random();
            private static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

            public static UpdateCredentialFromLookbackCommand BuildValid()
            {
                //var cred = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                var cred = CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, PathwayType.MOC);
                cred.AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        abimSource, 
                        IssuanceStatusType.Active, 
                        new DateTime(DateTime.Now.Year - 1, 3, 20), 
                        DurationType.Timelimited, 
                        MaintenanceRequirementType.NotRequired, 
                        MaintenanceStatusType.Maintained, 
                        OccurrenceType.Recertification));

                return CommandBuilder<UpdateCredentialFromLookbackCommand>
                            .Valid()
                            .With(cmd => cmd.Credential = cred)
                            .With(cmd => cmd.ModifiedBy = "YearEnd")
                            .With(cmd => cmd.Credential.GracePeriodStartDate = new DateTime(DateTime.Now.Year - 3, 1, 1))
                            .With(cmd => cmd.Credential.GracePeriodEndDate = new DateTime(DateTime.Now.Year - 2, 1, 1))
                            .With(cmd => cmd.Credential.NewestIssuance.IssuanceStatus = IssuanceStatusType.Active)
                            .With(cmd => cmd.Credential.NewestIssuance.MaintenanceStatus = MaintenanceStatusType.Maintained)
                            .With(cmd => cmd.Credential.LookbackDate = new DateTime(2018, 12, 31))
                            .Build();
            }
        }
        #endregion Setup

        #region Scenarios

        public abstract class UpdateCredentialFromLookbackCommandValidationBaseScenario
        {
            protected UpdateCredentialFromLookbackCommand _command;
            protected UpdateCredentialFromLookbackCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new UpdateCredentialFromLookbackCommandValidator();
            }
        }

        public class ShouldPassValidationWhenAllRequiredValuesArePresent
            : UpdateCredentialFromLookbackCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = UpdateCredentialFromLookbackCommandBuilder.BuildValid();
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
        public abstract class UpdateCredentialFromLookbackCommandValidationFailureBaseScenario 
            : UpdateCredentialFromLookbackCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = UpdateCredentialFromLookbackCommandBuilder.BuildValid();
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

        private class ShouldFailValidationWhenCredentialIsNull
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "Credential is required.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential = null;
            }
        }

        private class ShouldFailValidationWhenModifiedByIsNull
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "ModifiedBy is required.";
            }

            protected override void SetInvalidProperty()
            {
                _command.ModifiedBy = null;
            }
        }

        private class ShouldFailValidationWhenModifiedByIsBlank
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "ModifiedBy is required.";
            }

            protected override void SetInvalidProperty()
            {
                _command.ModifiedBy = "";
            }
        }

        private class ShouldFailValidationWhenGracePeriodStartIsNullAndGracePeriodEndHasValue 
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "GracePeriodStart is required when GracePeriodEnd is present.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential.GracePeriodStartDate = null;
            }
        }

        private class ShouldFailValidationWhenGracePeriodEndIsNullAndGracePeriodStartHasValue
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "GracePeriodEnd is required when GracePeriodStart is present.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential.GracePeriodEndDate = null;
            }
        }

        private class ShouldFailValidationWhenIssuanceStatusIsInvalidValue
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "IssuanceStatus is required.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential.NewestIssuance.IssuanceStatus = 0;
            }
        }

        private class ShouldFailValidationWhenMaintenanceStatusIsInvalidValue
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "MaintenanceStatus is required.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential.NewestIssuance.MaintenanceStatus = 0;
            }
        }

        private class ShouldFailValidationWhenLookbackDateIsDefaultValue
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "LookbackDate must be specified.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential.LookbackDate = default(DateTime);
            }
        }

        private class ShouldFailValidationWhenGracePeriodStartIsGreaterThanGracePeriodEnd
            : UpdateCredentialFromLookbackCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "GracePeriodStart must be less than GracePeriodEnd.";
            }

            protected override void SetInvalidProperty()
            {
                _command.Credential.GracePeriodStartDate = DateTime.Now.AddDays(5);
                _command.Credential.GracePeriodEndDate = DateTime.Now.AddDays(4);
            }
        }

        #endregion Scenarios
    }
}
