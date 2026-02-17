using System;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using TestStack.BDDfy;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abim.Platform.Program.WebApi.Authentication;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process to mark a certificate for eventual deselection",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
    )]
    [TestFixture]
    public class MarkCertificateForDeselectCommandValidationSpec
    {
        [TestCase]
        [WorkItem(187664)]
        public void PassesValidationWhenCommandIsValid()
        {
            new PassesValidationWhenCommandIsValidScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsValidationWhenCredentialIdIsEmptyGuid()
        {
            new FailsValidationWhenCredentialIdIsEmptyGuidScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsWhenSubmittedDateIsMinDate()
        {
            new FailsValidationWhenSubmittedDateIsMinDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsWhenUserInfoIsNull()
        {
            new FailsValidationWhenUserInfoIsNullScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsWhenUserInfoUsernameIsNull()
        {
            new FailsValidationWhenUserInfoUsernameIsNullScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsWhenUserInfoUsernameIsBlank()
        {
            new FailsValidationWhenUserInfoUsernameIsBlankScenario().BDDfy();
        }

        #region Scenario Base Classes
        private abstract class MarkCertificateForDeselectCommandValidationScenarioBase
        {
            protected MarkCertificateForDeselectCommand command;
            protected MarkCertificateForDeselectCommandValidator sut;
            protected CommandBuilder<MarkCertificateForDeselectCommand> builder;
            protected ValidationResult validationResult;
            protected Exception exceptionCaught;

            public MarkCertificateForDeselectCommandValidationScenarioBase()
            {
                command = new MarkCertificateForDeselectCommand();
                sut = new MarkCertificateForDeselectCommandValidator();
                builder = new CommandBuilder<MarkCertificateForDeselectCommand>();
            }
        }

        private abstract class ValidationFailedScenario : MarkCertificateForDeselectCommandValidationScenarioBase
        {
            protected string expectedValidationErrorMessage;

            public ValidationFailedScenario(string expectedValidationMessage)
            {
                expectedValidationErrorMessage = expectedValidationMessage;
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
                    .Contain(msg => 
                        msg.ErrorMessage.Contains(expectedValidationErrorMessage));
            }
        }
        #endregion Scenario Base Classes

        #region Scenarios

        #region Passes When Valid Scenario
        private class PassesValidationWhenCommandIsValidScenario : MarkCertificateForDeselectCommandValidationScenarioBase
        {
            public PassesValidationWhenCommandIsValidScenario()
            {}

            public void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.SubmittedDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
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
        #endregion Passes When Valid Scenario

        #region Fails When CredentialId is Empty Guid
        private class FailsValidationWhenCredentialIdIsEmptyGuidScenario : ValidationFailedScenario
        {
            public FailsValidationWhenCredentialIdIsEmptyGuidScenario() : base("CredentialId is required.")
            { }

            public void GivenTheCommandHasAnEmptyGuidForCredentialId() 
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.Empty)
                    .With(cmd => cmd.SubmittedDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Fails When CredentialId is Empty Guid

        #region Fails When SubmittedDate is Min Date
        private class FailsValidationWhenSubmittedDateIsMinDateScenario : ValidationFailedScenario
        {
            public FailsValidationWhenSubmittedDateIsMinDateScenario() : base("SubmittedDate is required.")
            { }

            public void GivenTheCommandHasTheMnDateValueForSubmittedDate()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.SubmittedDate = DateTime.MinValue) //When I didn't specify a value here, I got a value which included some minutes and seconds for some reason
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "evanhalen" })
                    .Build();
            }
        }
        #endregion Fails When SubmittedDate is Min Date

        #region Fails When UserInfo is null
        private class FailsValidationWhenUserInfoIsNullScenario : ValidationFailedScenario
        {
            public FailsValidationWhenUserInfoIsNullScenario() : base("UserInfo must not be null.")
            { }

            public void GivenTheCommandHasANullUserInfo()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.SubmittedDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.UserInfo = null)
                    .Build();
            }
        }
        #endregion Fails When UserInfo is null

        #region Fails When UserInfo.Username is null
        private class FailsValidationWhenUserInfoUsernameIsNullScenario : ValidationFailedScenario
        {
            public FailsValidationWhenUserInfoUsernameIsNullScenario() : base("UserInfo's UserName cannot be empty.")
            { }

            public void GivenTheCommandHasANullUserInfoUsername()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.SubmittedDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = null })
                    .Build();
            }
        }
        #endregion Fails When UserInfo.Username is null

        #region Fails When UserInfo.Username is blank
        private class FailsValidationWhenUserInfoUsernameIsBlankScenario : ValidationFailedScenario
        {
            public FailsValidationWhenUserInfoUsernameIsBlankScenario() : base("UserInfo's UserName cannot be empty.")
            { }

            public void GivenTheCommandHasANullUserInfoUsername()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.SubmittedDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "" })
                    .Build();
            }
        }
        #endregion Fails When UserInfo.Username is blank

        #endregion Scenarios
    }
}
