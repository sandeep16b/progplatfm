using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.App.Services.CommandValidators.Lookback;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Validators.LookbackDateLog
{
    public class AddLookbackDateLogValidationSpec
    {
        [Test]
        [WorkItem(137965)]
        public void Should_Pass_Validation_When_Command_Is_Valid()
        {
            new ShouldPassValidationWhenCommandIsValid().BDDfy();
        }
        
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_MemberIdIsEmpty()
        {
            new ShouldFailValidationWhenMemberIdIsEmpty().BDDfy();
        }
        
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_ChangedDate_Is_DateTimeMinVal()
        {
            new ShouldFailValidationWhenChangedDateIsDateTimeMinValue().BDDfy();
        }
        
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_LookbackDate_IsNull()
        {
            new ShouldFailValidationWhenLookbackDateIsNull().BDDfy();
        }
       
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_NewValue_Is_DateTimeMin()
        {
            new ShouldFailValidationWhenNewValueIsDateTimeMin().BDDfy();
        }
        
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_OldValue_Is_DateTimeMin()
        {
            new ShouldFailValidationWhenOldValueIsDateTimeMin().BDDfy();
        }
        
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_UserName_Is_Empty()
        {
            new ShouldFailValidationWhenUserNameIsEmpty().BDDfy();
        }
        
        [Test]
        [WorkItem(137965)]
        public void Should_Fail_Validation_When_NewValue_Or_OldValue_IsNull()
        {
            new ShouldPassValidationWhenNewValueOrOldValueIsNull().BDDfy();
        }

        private abstract class AddLookbackDateLogValidationScenarioBase
        {
            protected AddLookbackDateLogCommandValidator _sut;
            protected AddLookbackDateLogEntry _cmd;
            protected Exception _caughtException;
            protected ValidationResult _result;

            protected void Setup()
            {
                _sut = new AddLookbackDateLogCommandValidator();
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

        private class ShouldPassValidationWhenCommandIsValid : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveAValidCommand()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Now,
                    MemberGuid = Guid.NewGuid(),
                    LookbackDate = LookbackDateType.TwoYearStart,
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

        private class ShouldFailValidationWhenMemberIdIsEmpty : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithAnEmptyMemberId()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Now,
                    LookbackDate = LookbackDateType.TwoYearStart,
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
                    .Any(x => x.ErrorMessage.Contains("MemberGuid"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenChangedDateIsDateTimeMinValue : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithAnEmptyChangedDate()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    LookbackDate = LookbackDateType.TwoYearStart,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = "Unit Test",
                    MemberGuid = Guid.NewGuid()
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

        private class ShouldFailValidationWhenLookbackDateIsNull : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithAnEmptyLookbackDate()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Today,
                    NewValue = DateTime.Now.AddYears(1),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = "Unit Test",
                    MemberGuid = Guid.NewGuid()
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
                    .Any(x => x.ErrorMessage.Contains("LookbackDate"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenNewValueIsDateTimeMin : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithDateTimeMinAsNewValue()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Today,
                    LookbackDate = LookbackDateType.TwoYearStart,
                    NewValue = new DateTime(),
                    OldValue = DateTime.Now.AddYears(-1),
                    UserName = "Unit Test",
                    MemberGuid = Guid.NewGuid()
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
                    .Any(x => x.ErrorMessage.Contains("NewValue"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenOldValueIsDateTimeMin : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithDateTimeMinAsOldValue()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Today,
                    LookbackDate = LookbackDateType.TwoYearStart,
                    NewValue = DateTime.Now,
                    OldValue = new DateTime(),
                    UserName = "Unit Test",
                    MemberGuid = Guid.NewGuid()
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
                    .Any(x => x.ErrorMessage.Contains("OldValue"))
                    .Should().BeTrue();
            }
        }

        private class ShouldFailValidationWhenUserNameIsEmpty : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveACommandWithAnEmptyUserName()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Today,
                    LookbackDate = LookbackDateType.TwoYearStart,
                    NewValue = DateTime.Now,
                    OldValue = DateTime.Now,
                    UserName = " ",
                    MemberGuid = Guid.NewGuid()
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

        private class ShouldPassValidationWhenNewValueOrOldValueIsNull : AddLookbackDateLogValidationScenarioBase
        {
            private void GivenIHaveAValidCommand()
            {
                _cmd = new AddLookbackDateLogEntry
                {
                    ChangedDate = DateTime.Now,
                    MemberGuid = Guid.NewGuid(),
                    LookbackDate = LookbackDateType.TwoYearStart,
                    NewValue = null,
                    OldValue = null,
                    UserName = "Unit Test"
                };
            }

            private void AndResultShouldBeValid()
            {
                _result.Should().NotBeNull();
                _result.IsValid.Should().BeTrue();
            }
        }
    }
}
