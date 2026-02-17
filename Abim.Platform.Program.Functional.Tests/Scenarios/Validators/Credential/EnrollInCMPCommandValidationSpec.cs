using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup;
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
    public class EnrollInCMPCommandValidationSpec
    {
        [Test]
        public void Should_Pass_Validation_When_All_Required_Values_Are_Present()
        {
            new ShouldPassValidationWhenAllRequiredValuesArePresent().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_MemberId_Id_Is_Missing()
        {
            new ShouldFailValidationWhenMemberIdIsMissing().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Is_Null()
        {
            new ShouldFailValidationWhenUserInfoIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Username_Is_Blank()
        {
            new ShouldFailValidationWhenRequestingUserNameIsEmpty().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_UserInfo_Username_Is_Null()
        {
            new ShouldFailValidationWhenRequestingUserNameIsNull().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_EnrollmentEffectiveDate_Is_Too_Old()
        {
            new ShouldFailValidationWhenEnrollmentEffectiveDateIsTooOld().BDDfy();
        }

        [Test]
        public void Should_Fail_Validation_When_EnrollmentEffectiveDate_Is_Too_Future_Date()
        {
            new ShouldFailValidationWhenEnrollmentEffectiveDateIsTooFutureDate().BDDfy();
        }

        #region Setup
        private static class EnrollInCMPCommandBuilder
        {
            public static Random Random = new Random();
            public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
            public static EnrollInCMPCommand BuildValid()
            {
                return CommandBuilder<EnrollInCMPCommand>
                    .Valid()
                    .With(cmd => cmd.AbimId = Random.Next(1000, 100000).ToString())
                    .With(cmd => cmd.MemberId = Guid.NewGuid())
                    .With(cmd => cmd.SubspecialtyCertCode = RandomString.BuildWithLength(5))
                    .With(cmd => cmd.EnrollmentDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "UnitTest" })
                    .Build();
            }
        }

        #endregion Setup

        #region Scenarios

        #region Base Classes
        private abstract class EnrollInCMPCommandValidationBaseScenario
        {
            protected EnrollInCMPCommand _command;
            protected EnrollInCMPCommandValidator _validator;
            protected Exception _caughtException;
            protected ValidationResult _validationResult;

            protected virtual void Setup()
            {
                _validator = new EnrollInCMPCommandValidator();
            }
        }

        private abstract class EnrollInCMPCommandValidationFailureBaseScenario
            : EnrollInCMPCommandValidationBaseScenario
        {
            protected void GivenIHaveAnInvalidCommand()
            {
                _command = EnrollInCMPCommandBuilder.BuildValid();
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
        #endregion Base Classes

        private class ShouldPassValidationWhenAllRequiredValuesArePresent
            : EnrollInCMPCommandValidationBaseScenario
        {
            private void GivenIHaveAValidCommand()
            {
                _command = EnrollInCMPCommandBuilder.BuildValid();
            }

            private void WhenICallValidate()
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

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheValidationShouldHaveSucceeded()
            {
                _validationResult.IsValid.Should().BeTrue();
                _validationResult.Errors.Should().BeEmpty();
            }
        }

        private class ShouldFailValidationWhenMemberIdIsMissing
            : EnrollInCMPCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "MemberId or AbimId is required";
            }

            protected override void SetInvalidProperty()
            {
                _command.MemberId = Guid.Empty;
                _command.AbimId = "";
            }
        }

        private class ShouldFailValidationWhenUserInfoIsNull
            : EnrollInCMPCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "UserInfo must not be null";
            }

            protected override void SetInvalidProperty()
            {
                _command.UserInfo = null;
            }
        }

        private class ShouldFailValidationWhenRequestingUserNameIsEmpty
            : EnrollInCMPCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "'Requesting User Name' should not be empty.";
            }

            protected override void SetInvalidProperty()
            {
                _command.RequestingUserName = "";
            }
        }

        private class ShouldFailValidationWhenRequestingUserNameIsNull
            : EnrollInCMPCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "'Requesting User Name' should not be empty.";
            }

            protected override void SetInvalidProperty()
            {
                _command.RequestingUserName = null;
            }
        }


        private class ShouldFailValidationWhenEnrollmentEffectiveDateIsTooOld
          : EnrollInCMPCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "EnrollmentDate may not be prior to 1936";
            }

            protected override void SetInvalidProperty()
            {
                _command.EnrollmentDate = new DateTime(1936, 1, 1).AddDays(-(new Random()).Next(1, 10000));
            }
        }

        private class ShouldFailValidationWhenEnrollmentEffectiveDateIsTooFutureDate
  : EnrollInCMPCommandValidationFailureBaseScenario
        {
            protected override string ExpectedErrorMessage()
            {
                return "EnrollmentDate may not be more than 3 years in the future";
            }

            protected override void SetInvalidProperty()
            {
                _command.EnrollmentDate = DateTime.Now.AddYears((new Random()).Next(4, 1000));
            }
        }

        #endregion Scenarios
    }
}
