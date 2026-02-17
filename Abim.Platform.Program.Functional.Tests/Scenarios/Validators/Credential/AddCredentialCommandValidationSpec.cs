using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using Abim.Platform.Program.Tests.Setup.CommandBuilders.Credential;
using Abim.Platform.Program.WebApi.Authentication;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Credential
{
    [Story(
        AsA = "process adding a credential",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
    )]
    [TestFixture]
    public class AddCredentialCommandValidationSpec
    {
        [TestCase]
        public void ValidAddCredentialCommandPassesValidation()
        {
            new PassesValidationWhenValid().BDDfy();
        }

        [TestCase]
        public void EmptyMemberIdInAddCredentialCommandFailsValidation()
        {
            new FailsValidationWhenMemberIdIsEmptyGuid().BDDfy();
        }

        [TestCase]
        public void EmptyCertificationIdInAddCredentialCommandFailsValidation()
        {
            new FailsValidationWhenCertificationIdIsNull().BDDfy();
        }

        [TestCase]
        public void NullUserInfoInAddCredentialCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoIsNull().BDDfy();
        }

        [TestCase]
        public void UserInfoWithNullNameInAddCredentialCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoUsernameIsNull().BDDfy();
        }

        [TestCase]
        public void UserInfoWithBlankNameInAddCredentialCommandFailsValidation()
        {
            new FailsValidationWhenUserInfoUsernameIsBlank().BDDfy();
        }


        #region Scenarios
        private class PassesValidationWhenValid : AddCredentialCommandScenarioBase
        {
            void GivenTheCommandIsValid()
            {
                command = builder
                    .WithCertificationId(Guid.NewGuid())
                    .WithMemberId(Guid.NewGuid())
                    .WithPathway(PathwayType.MOC)
                    .WithUserInfo(new UserInfo { Username = "username" })
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

        private class FailsValidationWhenMemberIdIsEmptyGuid : AddCredentialCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToEmptyMemberId()
            {
                command = builder
                    .WithCertificationId(Guid.NewGuid())
                    .WithPathway(PathwayType.KCI)
                    .WithUserInfo(new UserInfo { Username = "blah" })
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
                        "MemberId is required"));
            }
        }

        private class FailsValidationWhenCertificationIdIsNull : AddCredentialCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToEmptyCertificationId()
            {
                command = builder
                    .WithMemberId(Guid.NewGuid())
                    .WithPathway(PathwayType.KCI)
                    .WithUserInfo(new UserInfo { Username = "blah" })
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
                        "CertificationId is required"));
            }
        }

        private class FailsValidationWhenUserInfoIsNull : AddCredentialCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullUserInfo()
            {
                command = builder
                    .WithCertificationId(Guid.NewGuid())
                    .WithMemberId(Guid.NewGuid())
                    .WithPathway(PathwayType.KCI)
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

        private class FailsValidationWhenUserInfoUsernameIsNull : AddCredentialCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToNullUserInfoUsername()
            {
                command = builder
                    .WithCertificationId(Guid.NewGuid())
                    .WithMemberId(Guid.NewGuid())
                    .WithPathway(PathwayType.KCI)
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

        private class FailsValidationWhenUserInfoUsernameIsBlank : AddCredentialCommandScenarioBase
        {
            void GivenTheCommandIsInvalidDueToBlankUserInfoUsername()
            {
                command = builder
                    .WithCertificationId(Guid.NewGuid())
                    .WithMemberId(Guid.NewGuid())
                    .WithPathway(PathwayType.KCI)
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

        private abstract class AddCredentialCommandScenarioBase
        {
            protected AddCredentialCommand command;
            protected AddCredentialCommandValidator sut;
            protected AddCredentialCommandBuilder builder;
            protected ValidationResult validationResult;
            protected Exception exceptionCaught;

            public AddCredentialCommandScenarioBase()
            {
                command = new AddCredentialCommand();
                sut = new AddCredentialCommandValidator();
                builder = new AddCredentialCommandBuilder();
            }
        }
        #endregion Scenarios
    }
}
