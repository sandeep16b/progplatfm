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
    public class ExpireAndReissueCommandValidationSpec
    {
        [TestCase]
        [WorkItem(73194)]
        public void ValidExpireAndReissueCommandPassesValidation()
        {
            new ValidExpireAndReissueCommandPassesValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void MissingCredentialIdInExpireAndReissueCommandFailsValidation()
        {
            new MissingCredentialIdInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void TooOldIssuanceDateInExpireAndReissueCommandFailsValidation()
        {
            new TooOldIssuanceDateInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void TooFutureIssuanceDateInExpireAndReissueCommandFailsValidation()
        {
            new TooFutureIssuanceDateInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void TooOldScheduledUpdateInExpireAndReissueCommandFailsValidation()
        {
            new TooOldScheduledUpdateInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void NullCreatedByInExpireAndReissueCommandFailsValidation()
        {
            new NullCreatedByInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void EmptyCreatedByInExpireAndReissueCommandFailsValidation()
        {
            new EmptyCreatedByInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void DefaultMaintenanceStatusInExpireAndReissueCommandFailsValidation()
        {
            new DefaultMaintenanceStatusInExpireAndReissueCommandFailsValidation().BDDfy();
        }

        //[TestCase]
        //[WorkItem(73194)]
        //public void MissingExistingIssuanceIdInExpireAndReissueCommandFailsValidation()
        //{
        //    new MissingExistingIssuanceIdInExpireAndReissueCommandFailsValidation().BDDfy();
        //}
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestExpireAndReissueCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static ExpireAndReissueCommand BuildValid()
        {
            return CommandBuilder<ExpireAndReissueCommand>
                        .Valid()
                        .With(cmd => cmd.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                        .WithNoZeroIntegers()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class ExpireAndReissueCommandValidationFailureBaseScenario
        : CredentialValidationScenario
    {
        protected ExpireAndReissueCommand Command { get; set; }
        ExpireAndReissueCommandValidator Validator { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ExpireAndReissueCommandValidator();
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
            Command = UnitTestExpireAndReissueCommandBuilder.BuildValid();
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
    public class ValidExpireAndReissueCommandPassesValidation
        : CredentialValidationScenario
    {
        ExpireAndReissueCommandValidator Validator { get; set; }
        ExpireAndReissueCommand Command { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }
        new EmailBuilder EmailBuilder { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ExpireAndReissueCommandValidator();
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
            Command = UnitTestExpireAndReissueCommandBuilder.BuildValid();
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
    /// The missing CredentialId scenario
    /// </summary>
    public class MissingCredentialIdInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.CredentialId = Guid.Empty;
        }

        protected override string ExpectedErrorMessage()
        {
            return "CredentialId is required";
        }
    }

    /// <summary>
    /// The too old issuance date scenario
    /// </summary>
    public class TooOldIssuanceDateInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
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
    public class TooFutureIssuanceDateInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.IssuanceDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year + 2, DateTime.Now.Year + 20)).Build();
        }

        protected override string ExpectedErrorMessage()
        {
            return "IssuanceDate may not be more than a year in the future";
        }
    }

    /// <summary>
    /// The too old scheduled update scenario
    /// </summary>
    public class TooOldScheduledUpdateInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
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
    /// The null CreatedBy scenario
    /// </summary>
    public class NullCreatedByInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
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
    public class EmptyCreatedByInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
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
    public class DefaultMaintenanceStatusInExpireAndReissueCommandFailsValidation
        : ExpireAndReissueCommandValidationFailureBaseScenario
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
    ///// The missing ExistingIssuanceId scenario
    ///// </summary>
    //public class MissingExistingIssuanceIdInExpireAndReissueCommandFailsValidation
    //    : ExpireAndReissueCommandValidationFailureBaseScenario
    //{
    //    //protected override void SetInvalidProperty()
    //    //{
    //    //    Command.ExistingIssuanceId = 0;
    //    //}

    //    protected override string ExpectedErrorMessage()
    //    {
    //        return "ExistingIssuanceId is required";
    //    }
    //}

    #endregion Scenarios
}
