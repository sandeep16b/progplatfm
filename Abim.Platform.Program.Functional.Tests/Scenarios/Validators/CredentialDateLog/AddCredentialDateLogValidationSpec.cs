using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.CredentialDateLog;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.CredentialDateLog
{
    public class AddCredentialDateLogValidationSpec
    {
        [Test]
        [WorkItem(136585)]
        public void Should_Pass_Validation_When_Command_Is_Valid()
        {
            new ShouldPassValidationWhenCommandIsValid().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Fail_Validation_When_CredentialId_Is_Empty()
        {
            new ShouldFailValidationWhenCredentialIdIsEmpty().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Fail_Validation_When_ChangedDate_Is_Min_DateTime()
        {
            new ShouldFailValidationWhenChangedDateIsMinDateTime().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Fail_Validation_When_Username_Is_Null()
        {
            new ShouldFailValidationWhenUsernameIsNull().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Fail_Validation_When_Username_Is_Empty_String()
        {
            new ShouldFailValidationWhenUsernameIsEmptyString().BDDfy();
        }

        #region Scenarios

        private abstract class AddCredentialDateLogValidationScenarioBase
        {
            protected AddCredentialDateLogValidator _sut;
            protected AddCredentialDateLog _cmd;
            protected Exception _caughtException;
            protected ValidationResult _result;

            protected void Setup()
            {
                _sut = new AddCredentialDateLogValidator();
            }

            protected void WhenICallValidate()
            {
                try
                {
                    _result = _sut.Validate(_cmd);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }
        }

        private class ShouldPassValidationWhenCommandIsValid : AddCredentialDateLogValidationScenarioBase
        {
            private void GivenIHaveAValidCommand()
            {
                _cmd = new AddCredentialDateLog
                {
                    ChangedDate = DateTime.Now,
                    CredentialId = Guid.NewGuid(),
                    DateType = App.Domain.CredentialDateType.ExamDueDate,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = "Unit Test"
                };
            }

            private void AndResultShouldBeValid()
            {
                _result.Should().NotBeNull();
                _result.IsValid.Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenCredentialIdIsEmpty : AddCredentialDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithAnEmptyCredentialId()
            {
                _cmd = new AddCredentialDateLog
                {
                    ChangedDate = DateTime.Now,
                    DateType = App.Domain.CredentialDateType.ExamDueDate,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = "Unit Test"
                };
            }

            private void AndResultShouldBeInValid()
            {
                _result.Should().NotBeNull();
                _result.IsValid.Should().BeFalse();
            }

            private void AndTheCorrectValidationErrorShouldBePresent()
            {
                _result
                    .Errors
                    .Any(x => x.ErrorMessage.Contains("CredentialId"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenChangedDateIsMinDateTime : AddCredentialDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithoutSpecifyingChangedDate()
            {
                _cmd = new AddCredentialDateLog
                {
                    CredentialId = Guid.NewGuid(), 
                    DateType = App.Domain.CredentialDateType.ExamDueDate,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = "Unit Test"
                };
            }

            private void AndResultShouldBeInValid()
            {
                _result.Should().NotBeNull();
                _result.IsValid.Should().BeFalse();
            }

            private void AndTheCorrectValidationErrorShouldBePresent()
            {
                _result
                    .Errors
                    .Any(x => x.ErrorMessage.Contains("ChangedDate"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenUsernameIsNull : AddCredentialDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithANullUserName()
            {
                _cmd = new AddCredentialDateLog
                {
                    ChangedDate = DateTime.Now, 
                    CredentialId = Guid.NewGuid(),
                    DateType = App.Domain.CredentialDateType.ExamDueDate,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = null
                };
            }

            private void AndResultShouldBeInValid()
            {
                _result.Should().NotBeNull();
                _result.IsValid.Should().BeFalse();
            }

            private void AndTheCorrectValidationErrorShouldBePresent()
            {
                _result
                    .Errors
                    .Any(x => x.ErrorMessage.Contains("UserName"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenUsernameIsEmptyString : AddCredentialDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithAnEmptyUserName()
            {
                _cmd = new AddCredentialDateLog
                {
                    ChangedDate = DateTime.Now,
                    CredentialId = Guid.NewGuid(),
                    DateType = App.Domain.CredentialDateType.ExamDueDate,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = ""
                };
            }

            private void AndResultShouldBeInValid()
            {
                _result.Should().NotBeNull();
                _result.IsValid.Should().BeFalse();
            }

            private void AndTheCorrectValidationErrorShouldBePresent()
            {
                _result
                    .Errors
                    .Any(x => x.ErrorMessage.Contains("UserName"))
                    .Should().BeTrue();
            }
        }

        #endregion Scenarios
    }
}
