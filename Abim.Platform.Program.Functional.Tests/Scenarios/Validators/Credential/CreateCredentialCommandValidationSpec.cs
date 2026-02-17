using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Validators.Certification.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process creating a credential",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class CreateCredentialCommandValidationSpec
    {
        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void ValidCreateCredentialCommandPassesValidation()
        {
            new ValidCreateCredentialCommandPassesValidation().BDDfy();
        }

        [TestCase]

        public void MissingCertificationIdInCreateCredentialCommandFailsValidation()
        {
            new MissingCertificationIdInCreateCredentialCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void MissingMemberIdInCreateCredentialCommandFailsValidation()
        {
            new MissingMemberIdInCreateCredentialCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void DefaultTypeInCreateCredentialCommandFailsValidation()
        {
            new DefaultTypeInCreateCredentialCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void DefaultPathwayTypeInCreateCredentialCommandFailsValidation()
        {
            new DefaultPathwayTypeInCreateCredentialCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestCreateCredentialCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static CreateCredentialCommand BuildValid()
        {
            return CommandBuilder<CreateCredentialCommand>
                        .Valid()
                        .With(cmd => cmd.GracePeriodStartDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.GracePeriodEndDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.ExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class CreateCredentialCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected CreateCredentialCommand Command     { get; set; }
        CreateCredentialCommandValidator Validator    { get; set; }
        new Exception ExceptionCaught                     { get; set; }
        ValidationResult ValidationResult             { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new CreateCredentialCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestCreateCredentialCommandBuilder.BuildValid();
            SetInvalidProperty();
        }

        protected abstract void SetInvalidProperty();

        protected abstract string ExpectedErrorMessage();

        public void WhenICallValidate()
        {
            try
            {
                ValidationResult = Validator.Validate(Command);
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }

        public void AndThenTheValidationErrorsShouldContainTheRequiredMessage()
        {
            ValidationResult.IsValid.Should().BeFalse();
            string expectedMessage = ExpectedErrorMessage().ToLower();
            ValidationResult.Errors.Should().Contain(msg => msg.ErrorMessage.ToLower().Contains(expectedMessage));
        }
    }

    #endregion

    #region Success

    /// <summary>
    /// The valid command scenario
    /// </summary>
    public class ValidCreateCredentialCommandPassesValidation
        : CertificationValidationScenario
    {
        CreateCredentialCommandValidator Validator    { get; set; }
        CreateCredentialCommand Command               { get; set; }
        new Exception ExceptionCaught                     { get; set; }
        ValidationResult ValidationResult             { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new CreateCredentialCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestCreateCredentialCommandBuilder.BuildValid();
        }

        public async Task WhenICallValidate()
        {
            try
            {
                ValidationResult = Validator.Validate(Command);
            }
            catch(Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheValidationShouldHavePassed()
        {
            ValidationResult.IsValid.Should().BeTrue();
            ValidationResult.Errors.Should().BeEmpty();
        }
    }

    #endregion

    #region Failure

    /// <summary>
    /// The missing CertificationId scenario
    /// </summary>
    public class MissingCertificationIdInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CertificationId = Guid.Empty;
        }

        protected override string ExpectedErrorMessage()
        {
            return "CertificationId is required";
        }
    }

    /// <summary>
    /// The missing MemberId scenario
    /// </summary>
    public class MissingMemberIdInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.MemberId = Guid.Empty;
        }

        protected override string ExpectedErrorMessage()
        {
            return "MemberId is required";
        }
    }

    /// <summary>
    /// The too old GracePeriodStartDate scenario
    /// </summary>
    public class TooOldGracePeriodStartDateInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.GracePeriodStartDate = DateTimeBuilder.Random().WithYear(Random.Next(1900, DateTime.Now.Year - 5)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "GracePeriodStartDate may not be more than 4 years ago";
        }
    }

    /// <summary>
    /// The too future GracePeriodStartDate scenario
    /// </summary>
    public class TooFutureGracePeriodStartDateInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.GracePeriodStartDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "GracePeriodStartDate may not be more than a year in the future";
        }
    }

    /// <summary>
    /// The too old GracePeriodEndDate scenario
    /// </summary>
    public class TooOldGracePeriodEndDateInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.GracePeriodEndDate = DateTimeBuilder.Random().WithYear(Random.Next(1900, DateTime.Now.Year - 5)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "GracePeriodEndDate may not be more than 4 years ago";
        }
    }

    /// <summary>
    /// The too future GracePeriodEndDate scenario
    /// </summary>
    public class TooFutureGracePeriodEndDateInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.GracePeriodEndDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "GracePeriodEndDate may not be more than a year in the future";
        }
    }

    /// <summary>
    /// The too old ExamDueDate scenario
    /// </summary>
    public class TooOldExamDueDateInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(1900, DateTime.Now.Year - 5)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "ExamDueDate may not be more than 4 years ago";
        }
    }

    /// <summary>
    /// The too future ExamDueDate scenario
    /// </summary>
    public class TooFutureExamDueDateInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "ExamDueDate may not be more than a year in the future";
        }
    }

    /// <summary>
    /// The default Type scenario
    /// </summary>
    public class DefaultTypeInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Type = default(CredentialType);
        }

        protected override string ExpectedErrorMessage()
        {
            return "Type is required";
        }
    }

    /// <summary>
    /// The default PathwayType scenario
    /// </summary>
    public class DefaultPathwayTypeInCreateCredentialCommandFailsValidation
        : CreateCredentialCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Pathway = default(PathwayType);
        }

        protected override string ExpectedErrorMessage()
        {
            return "Pathway is required";
        }
    }

    #endregion Scenarios
}
