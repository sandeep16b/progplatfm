

 
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Issuance;
using Abim.Platform.Program.Tests.Setup.CommandBuilders.Credential;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Credential
{
    [Story(
        AsA = "process updating a certification",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
    )]
    [TestFixture]
    public class AddIssuanceCommandValidationSpec
    {
        [TestCase]
        public void ValidAddIssuanceCommandPassesValidation()
        {
            new PassesValidationWhenValid().BDDfy();
        }

        [TestCase]
        public void EmptyCredentialIdInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenCredentialIdIsEmptyGuid().BDDfy();
        }

        [TestCase]
        public void IssuanceDateWithMinValueInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenIssuanceDateIsDateTimeMinValue().BDDfy();
        }

        [TestCase]
        public void EffectiveDateWithMinValueInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenEffectiveDateIsDateTimeMinValue().BDDfy();
        }

        [TestCase]
        public void EmptySourceIdInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenSourceIdIsEmptyGuid().BDDfy();
        }

        [TestCase]
        public void NullUserInfoInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoIsNull().BDDfy();
        }

        [TestCase]
        public void NullUserInfoUsernameInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoUsernameIsNull().BDDfy();
        }

        [TestCase]
        public void BlankUserInfoUsernameInAddIssuanceCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoUsernameIsBlank().BDDfy();
        }

        #region Scenarios
        private class PassesValidationWhenValid : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsValid()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(new UserInfo { Username = "someUser" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHavePassed()
            {
                validationResult.IsValid.Should().BeTrue();
                validationResult.Errors.Should().BeEmpty();
            }
        }

        private class FailsValidationWhenCredentialIdIsEmptyGuid : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToEmptyCredentialId()
            {
                command = builder
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(new UserInfo { Username = "someUser" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "CredentialId is required"));
            }
        }

        private class FailsValidationWhenIssuanceDateIsDateTimeMinValue : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToIssuanceDateWithMinValue()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(new UserInfo { Username = "someUser" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "IssuanceDate is required"));
            }
        }

        private class FailsValidationWhenEffectiveDateIsDateTimeMinValue : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToEffectiveDateWithMinValue()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(new UserInfo { Username = "someUser" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "EffectiveDate is required"));
            }
        }

        private class FailsValidationWhenSourceIdIsEmptyGuid : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToEmptySourceId()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithUserInfo(new UserInfo { Username = "someUser" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "SourceId is required"));
            }
        }

        private class FailsValidationWhenUserInfoIsNull : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullUserInfo()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(null)
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "UserInfo must not be null"));
            }
        }

        private class FailsValidationWhenUserInfoUsernameIsNull : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullUserInfoUsername()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(new UserInfo { Username = null })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "UserInfo's UserName cannot be empty"));
            }
        }

        private class FailsValidationWhenUserInfoUsernameIsBlank : AddIssuanceCommandValidationScenarioBase
        {
            void GivenTheCommandIsInvalidDueToBlankUserInfoUsername()
            {
                command = builder
                    .WithCredential(Guid.NewGuid())
                    .WithDuration(DurationType.Continuous)
                    .WithEffectiveDate(DateTime.Now)
                    .WithExpirationDate(DateTime.Now.AddYears(1))
                    .WithIssuanceDate(DateTime.Now)
                    .WithIssuanceStatus(IssuanceStatusType.Active)
                    .WithMaintenanceRequirement(MaintenanceRequirementType.NotRequired)
                    .WithMaintenanceStatus(MaintenanceStatusType.Maintained)
                    .WithOccurrence(OccurrenceType.Initial)
                    .WithSourceId(Guid.NewGuid())
                    .WithUserInfo(new UserInfo { Username = "" })
                    .Build();
            }

            public void WhenICallValidate()
            {
                try
                {
                    validationResult = sut.Validate(command);
                }
                catch (Exception ex)
                {
                    exceptionCaught = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                exceptionCaught.Should().BeNull();
            }

            public void AndTheValidationShouldHaveFailed()
            {
                validationResult.IsValid.Should().BeFalse();
                validationResult.Errors.Should().NotBeEmpty();
            }

            public void AndTheValidationErrorsShouldContainTheRequiredMessage()
            {
                validationResult.Errors
                    .Should()
                    .Contain(msg => msg.ErrorMessage.Contains(
                        "UserInfo's UserName cannot be empty"));
            }
        }

        private abstract class AddIssuanceCommandValidationScenarioBase
        {
            protected AddIssuanceCommand command;
            protected AddIssuanceCommandValidator sut;
            protected AddIssuanceCommandBuilder builder;
            protected ValidationResult validationResult;
            protected Exception exceptionCaught;

            public AddIssuanceCommandValidationScenarioBase()
            {
                command = new AddIssuanceCommand();
                sut = new AddIssuanceCommandValidator();
                builder = new AddIssuanceCommandBuilder();
            }
        }
        #endregion Scenarios
    }
}
