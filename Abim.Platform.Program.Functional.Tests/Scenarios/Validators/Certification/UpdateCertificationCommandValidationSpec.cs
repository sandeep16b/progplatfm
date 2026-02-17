using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Certification;
using Abim.Platform.Program.Tests.Setup.CommandBuilders.Certification;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process updating a certification",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
    )]
    [TestFixture]
    public class UpdateCertificationCommandValidationSpec
    {
        [TestCase]
        public void ValidUpdateCertificationCommandPassesValidation()
        {
            new PassesValidationWhenValid().BDDfy();
        }

        [TestCase]
        public void EmptyGuidSourceIdInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenSourceIdIsEmptyGuid().BDDfy();
        }

        [TestCase]
        public void NullCodeInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenCodeIsNull().BDDfy();
        }

        [TestCase]
        public void BlankCodeInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenCodeIsBlank().BDDfy();
        }

        [TestCase]
        public void NullNameInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenNameIsNull().BDDfy();
        }

        [TestCase]
        public void BlankNameInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenNameIsBlank().BDDfy();
        }

        [TestCase]
        public void NullUserInfoInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoIsNull().BDDfy();
        }

        [TestCase]
        public void UserInfoWithNullNameInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoUsernameIsNull().BDDfy();
        }

        [TestCase]
        public void UserInfoWithBlankNameInUpdateCertificationCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoUsernameIsBlank().BDDfy();
        }

        private class PassesValidationWhenValid : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsValid()
            {
                command = builder
                    .WithCode("blah")
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
                    .WithUserInfo(new UserInfo { Username = "someUsername" })
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

        private class FailsValidationWhenSourceIdIsEmptyGuid : UpdateCertificationCommandScenarioBase
        {
            /*
            I could've put all of the common functions for failure tests 
            into another base class to reduce duplicate code, but in this 
            case, it's more useful for it to be in one place to be more 
            easily readable.
            */
            void GivenTheCommandIsInvalidDueToEmptySourceId()
            {
                command = builder
                    .WithCode("blah")
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithType(CertificationType.FocusPractice)
                    .WithUserInfo(new UserInfo { Username = "someUsername" })
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

        private class FailsValidationWhenCodeIsNull : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullCode()
            {
                command = builder
                    .WithCode(null)
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
                    .WithUserInfo(new UserInfo { Username = "someUsername" })
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
                        "Code is required"));
            }
        }

        private class FailsValidationWhenCodeIsBlank : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToBlankCode()
            {
                command = builder
                    .WithCode("")
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
                    .WithUserInfo(new UserInfo { Username = "someUsername" })
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
                        "Code is required"));
            }
        }

        private class FailsValidationWhenNameIsNull : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullName()
            {
                command = builder
                    .WithCode("SomeCode")
                    .WithConsecutiveAttempt(1)
                    .WithName(null)
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
                    .WithUserInfo(new UserInfo { Username = "someUsername" })
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
                        "Name is required"));
            }
        }

        private class FailsValidationWhenNameIsBlank : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToBlankName()
            {
                command = builder
                    .WithCode("SomeCode")
                    .WithConsecutiveAttempt(1)
                    .WithName("")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
                    .WithUserInfo(new UserInfo { Username = "someUsername" })
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
                        "Name is required"));
            }
        }

        private class FailsValidationWhenUserInfoIsNull : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullUserInfo()
            {
                command = builder
                    .WithCode("SomeCode")
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
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

        private class FailsValidationWhenUserInfoUsernameIsNull : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullUserInfoUsername()
            {
                command = builder
                    .WithCode("SomeCode")
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
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

        private class FailsValidationWhenUserInfoUsernameIsBlank : UpdateCertificationCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToBlankUserInfoUsername()
            {
                command = builder
                    .WithCode("SomeCode")
                    .WithConsecutiveAttempt(1)
                    .WithName("Some Name")
                    .WithSourceId(Guid.NewGuid())
                    .WithType(CertificationType.FocusPractice)
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

        private abstract class UpdateCertificationCommandScenarioBase
        {
            protected UpdateCertificationCommand command;
            protected UpdateCertificationCommandValidator sut;
            protected UpdateCertificationCommandBuilder builder;
            protected ValidationResult validationResult;
            protected Exception exceptionCaught;

            public UpdateCertificationCommandScenarioBase()
            {
                command = new UpdateCertificationCommand();
                sut = new UpdateCertificationCommandValidator();
                builder = new UpdateCertificationCommandBuilder();
            }
        }

    }
}
