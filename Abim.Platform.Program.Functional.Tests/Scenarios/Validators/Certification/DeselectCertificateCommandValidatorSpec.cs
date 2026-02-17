using System;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using TestStack.BDDfy;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process deselecting a certificate",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
    )]
    [TestFixture]
    public class DeselectCertificateCommandValidatorSpec
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
        public void FailsWhenExpiredDateIsMinDate()
        {
            new FailsValidationWhenExpiredDateIsMinDateScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsWhenUserInfoUsernameIsNull()
        {
            new FailsValidationWhenUsernameIsNullScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(187664)]
        public void FailsWhenUserInfoUsernameIsBlank()
        {
            new FailsValidationWhenUsernameIsBlankScenario().BDDfy();
        }


        #region Scenario Base Classes
        private abstract class DeselectCertificateCommandValidationScenarioBase
        {
            protected DeselectCertificateCommand command;
            protected DeselectCertificateCommandValidator sut;
            protected CommandBuilder<DeselectCertificateCommand> builder;
            protected ValidationResult validationResult;
            protected Exception exceptionCaught;

            public DeselectCertificateCommandValidationScenarioBase()
            {
                command = new DeselectCertificateCommand();
                sut = new DeselectCertificateCommandValidator();
                builder = new CommandBuilder<DeselectCertificateCommand>();
            }
        }

        private abstract class ValidationFailedScenario : DeselectCertificateCommandValidationScenarioBase
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
        private class PassesValidationWhenCommandIsValidScenario : DeselectCertificateCommandValidationScenarioBase
        {
            public PassesValidationWhenCommandIsValidScenario()
            { }

            public void GivenTheCommandIsValid()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.ExpiredDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.Username = "evanhalen")
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
                    .With(cmd => cmd.ExpiredDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.Username = "avanhalen")
                    .Build();
            }
        }
        #endregion Fails When CredentialId is Empty Guid

        #region Fails When ExpiredDate is Min Date
        private class FailsValidationWhenExpiredDateIsMinDateScenario : ValidationFailedScenario
        {
            public FailsValidationWhenExpiredDateIsMinDateScenario() : base("ExpiredDate is required.")
            { }

            public void GivenTheCommandHasTheMnDateValueForExpiredDate()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.ExpiredDate = DateTime.MinValue) //When I didn't specify a value here, I got a value which included some minutes and seconds for some reason
                    .With(cmd => cmd.Username = "dlroth")
                    .Build();
            }
        }
        #endregion Fails When SubmittedDate is Min Date

        #region Fails When Username is Null
        private class FailsValidationWhenUsernameIsNullScenario : ValidationFailedScenario
        {
            public FailsValidationWhenUsernameIsNullScenario() : base("Username is required.")
            { }

            public void GivenTheCommandHasANullUserInfoUsername()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.ExpiredDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.Username = null)
                    .Build();
            }
        }
        #endregion Fails When Username is Null

        #region Fails When Username is Blank
        private class FailsValidationWhenUsernameIsBlankScenario : ValidationFailedScenario
        {
            public FailsValidationWhenUsernameIsBlankScenario() : base("Username is required.")
            { }

            public void GivenTheCommandHasANullUserInfoUsername()
            {
                command = builder
                    .With(cmd => cmd.CredentialId = Guid.NewGuid())
                    .With(cmd => cmd.ExpiredDate = new DateTime(2020, 10, 8))
                    .With(cmd => cmd.Username = "")
                    .Build();
            }
        }
        #endregion Fails When Username is null

        #endregion Scenarios
    }
}
