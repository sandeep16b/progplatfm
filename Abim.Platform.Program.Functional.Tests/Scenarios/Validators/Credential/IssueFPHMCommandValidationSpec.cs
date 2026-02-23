using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Validators.Credential.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Credential
{
    [Story(
        AsA = "backend process",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely call Handle() on this command"
        )]
    [TestFixture]
    public class IssueFPHMCommandValidationSpec
    {
        [TestCase]
        [WorkItem(73615)]
        public void ValidIssueFPHMCommandPassesValidation()
        {
            new ValidIssueFPHMCommandPassesValidation().BDDfy();
        }

        //[TestCase]
        //[WorkItem(73615)]
        //public void MissingMemberIdInIssueFPHMCommandFailsValidation()
        //{
        //    new MissingMemberIdInIssueFPHMCommandFailsValidation().BDDfy();
        //}

        [TestCase]
        [WorkItem(73615)]
        public void TooOldIssuanceDateInIssueFPHMCommandFailsValidation()
        {
            new TooOldIssuanceDateInIssueFPHMCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void TooFutureIssuanceDateInIssueFPHMCommandFailsValidation()
        {
            new TooFutureIssuanceDateInIssueFPHMCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void TooOldScheduledUpdateInIssueFPHMCommandFailsValidation()
        {
            new TooOldScheduledUpdateInIssueFPHMCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void TooFutureScheduledUpdateInIssueFPHMCommandFailsValidation()
        {
            new TooFutureScheduledUpdateInIssueFPHMCommandFailsValidation().BDDfy();
        }

        //[TestCase]
        //[WorkItem(73615)]
        //public void TooOldPassExamDateInIssueFPHMCommandFailsValidation()
        //{
        //    new TooOldPassExamDateInIssueFPHMCommandFailsValidation().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73615)]
        //public void TooFuturePassExamDateInIssueFPHMCommandFailsValidation()
        //{
        //    new TooFuturePassExamDateInIssueFPHMCommandFailsValidation().BDDfy();
        //}

        [TestCase]
        [WorkItem(73615)]
        public void NullCreatedByInIssueFPHMCommandFailsValidation()
        {
            new NullCreatedByInIssueFPHMCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void EmptyCreatedByInIssueFPHMCommandFailsValidation()
        {
            new EmptyCreatedByInIssueFPHMCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void DefaultMaintenanceStatusInIssueFPHMCommandFailsValidation()
        {
            new DefaultMaintenanceStatusInIssueFPHMCommandFailsValidation().BDDfy();
        }

        //[TestCase]
        //[WorkItem(73615)]
        //public void DefaultAssessmentTypeInIssueFPHMCommandFailsValidation()
        //{
        //    new DefaultAssessmentTypeInIssueFPHMCommandFailsValidation().BDDfy();
        //}
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestIssueFPHMCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static IssueFPHMCommand BuildValid()
        {
            return CommandBuilder<IssueFPHMCommand>
                        .Valid()
                        .With(cmd => cmd.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        //.With(cmd => cmd.PassExamDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class IssueFPHMCommandValidationFailureBaseScenario
        : CredentialValidationScenario
    {
        protected IssueFPHMCommand Command { get; set; }
        IssueFPHMCommandValidator Validator { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new IssueFPHMCommandValidator();
            EmailBuilder = new EmailBuilder();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestIssueFPHMCommandBuilder.BuildValid();
            SetInvalidProperty();
        }

        protected abstract void SetInvalidProperty();

        protected abstract string ExpectedErrorMessage();

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
    public class ValidIssueFPHMCommandPassesValidation
        : CredentialValidationScenario
    {
        IssueFPHMCommandValidator Validator { get; set; }
        IssueFPHMCommand Command { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new IssueFPHMCommandValidator();
            EmailBuilder = new EmailBuilder();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestIssueFPHMCommandBuilder.BuildValid();
            Command.ProcessingDate = new DateTime(1940 + Random.Next(20, 50), Command.ProcessingDate.Month, Command.ProcessingDate.Day);
            Command.IssuanceDate = Command.ProcessingDate.AddMonths(Random.Next(1, 10));
            Command.ScheduledUpdate = Command.ProcessingDate.AddMonths(Random.Next(1, 10));
        }

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

        public void AndThenTheValidationShouldHavePassed()
        {
            ValidationResult.IsValid.Should().BeTrue();
            ValidationResult.Errors.Should().BeEmpty();
        }
    }

    #endregion

    #region Failure

    ///// <summary>
    ///// The missing MemberId scenario
    ///// </summary>
    //public class MissingMemberIdInIssueFPHMCommandFailsValidation
    //    : IssueFPHMCommandValidationFailureBaseScenario
    //{
    //    protected override void SetInvalidProperty()
    //    {
    //        Command.MemberId = Guid.Empty;
    //    }

    //    protected override string ExpectedErrorMessage()
    //    {
    //        return "MemberId is required";
    //    }
    //}

    /// <summary>
    /// The too old issuance date scenario
    /// </summary>
    public class TooOldIssuanceDateInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1900, 1935)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "IssuanceDate may not be prior to 1936";
        }
    }

    /// <summary>
    /// The too future issuance date scenario
    /// </summary>
    public class TooFutureIssuanceDateInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            var processingDate = DateTime.Now;
            Command.ProcessingDate = processingDate;
            Command.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(processingDate.Year + 2, processingDate.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "IssuanceDate may not be more than a year from ProcessingDate";
        }
    }

    /// <summary>
    /// The too old scheduled update scenario
    /// </summary>
    public class TooOldScheduledUpdateInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1900, 1935)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "ScheduledUpdate may not be prior to 1936";
        }
    }

    /// <summary>
    /// The too future scheduled update scenario
    /// </summary>
    public class TooFutureScheduledUpdateInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            var processingDate = DateTime.Now;
            Command.ProcessingDate = processingDate;
            Command.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(processingDate.Year + 2, processingDate.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "ScheduledUpdate may not be more than 13 months from ProcessingDate";
        }
    }

    /// <summary>
    /// The too old pass exam date scenario
    /// </summary>
    //public class TooOldPassExamDateInIssueFPHMCommandFailsValidation
    //    : IssueFPHMCommandValidationFailureBaseScenario
    //{
    //    protected override void SetInvalidProperty()
    //    {
    //        Command.PassExamDate = DateTimeBuilder.Random().WithYear(Random.Next(1900, 1935)).Build();
    //    }

    //    protected override string ExpectedErrorMessage()
    //    {
    //        return "PassExamDate may not be prior to 1936";
    //    }
    //}

    ///// <summary>
    ///// The too future pass exam date scenario
    ///// </summary>
    //public class TooFuturePassExamDateInIssueFPHMCommandFailsValidation
    //    : IssueFPHMCommandValidationFailureBaseScenario
    //{
    //    protected override void SetInvalidProperty()
    //    {
    //        Command.PassExamDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
    //    }

    //    protected override string ExpectedErrorMessage()
    //    {
    //        return "PassExamDate may not be more than a year in the future";
    //    }
    //}

    /// <summary>
    /// The null CreatedBy scenario
    /// </summary>
    public class NullCreatedByInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CreatedBy = null;
        }

        protected override string ExpectedErrorMessage()
        {
            return "CreatedBy is required";
        }
    }

    /// <summary>
    /// The empty CreatedBy scenario
    /// </summary>
    public class EmptyCreatedByInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CreatedBy = "";
        }

        protected override string ExpectedErrorMessage()
        {
            return "CreatedBy is required";
        }
    }

    /// <summary>
    /// The default MaintenanceStatus scenario
    /// </summary>
    public class DefaultMaintenanceStatusInIssueFPHMCommandFailsValidation
        : IssueFPHMCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.MaintenanceStatus = default(MaintenanceStatusType);
        }

        protected override string ExpectedErrorMessage()
        {
            return "MaintenanceStatus is required";
        }
    }

    ///// <summary>
    ///// The default ExamType scenario
    ///// </summary>
    //public class DefaultAssessmentTypeInIssueFPHMCommandFailsValidation
    //    : IssueFPHMCommandValidationFailureBaseScenario
    //{
    //    //protected override void SetInvalidProperty()
    //    //{
    //    //    Command.ExamType = default(ExamType);
    //    //}

    //    protected override string ExpectedErrorMessage()
    //    {
    //        return "ExamType is required";
    //    }
    //}

    #endregion Scenarios
}
