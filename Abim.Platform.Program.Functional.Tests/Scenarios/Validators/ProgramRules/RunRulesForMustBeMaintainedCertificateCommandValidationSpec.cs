using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.Tests.Scenarios.Validators.Certification.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Certification
{
    [Story(
        AsA = "background process running checks on expiring time-limited credentials",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class RunRulesForMustBeMaintainedCertificateCommandValidationSpec
    {
        [TestCase]
        [WorkItem(73194)]
        public void ValidRunRulesForMustBeMaintainedCertificateCommandPassesValidation()
        {
            new ValidRunRulesForMustBeMaintainedCertificateCommandPassesValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void MissingCredentialIdInRunRulesForMustBeMaintainedCertificateCommandFailsValidation()
        {
            new MissingEventDateInRunRulesForMustBeMaintainedCertificateCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void MissingIssuanceIdInRunRulesForMustBeMaintainedCertificateCommandFailsValidation()
        {
            new MissingIssuanceIdInRunRulesForMustBeMaintainedCertificateCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void MissingEventDateInRunRulesForMustBeMaintainedCertificateCommandFailsValidation()
        {
            new MissingEventDateInRunRulesForMustBeMaintainedCertificateCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void NullCreatedByInRunRulesForMustBeMaintainedCertificateCommandFailsValidation()
        {
            new NullCreatedByInRunRulesForMustBeMaintainedCertificateCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void EmptyCreatedByInRunRulesForMustBeMaintainedCertificateCommandFailsValidation()
        {
            new EmptyCreatedByInRunRulesForMustBeMaintainedCertificateCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestRunRulesForMustBeMaintainedCertificateCommandBuilder
    {
        public static Random Random = new Random();
        public static RunRulesForMustBeMaintainedCertificateCommand BuildValid()
        {
            return CommandBuilder<RunRulesForMustBeMaintainedCertificateCommand>
                        .Valid()
                        .WithNoZeroIntegers()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class RunRulesForMustBeMaintainedCertificateCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected RunRulesForMustBeMaintainedCertificateCommand Command     { get; set; }
        RunRulesForMustBeMaintainedCertificateCommandValidator Validator    { get; set; }
        new Exception ExceptionCaught                                           { get; set; }
        ValidationResult ValidationResult                                   { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new RunRulesForMustBeMaintainedCertificateCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestRunRulesForMustBeMaintainedCertificateCommandBuilder.BuildValid();
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
    public class ValidRunRulesForMustBeMaintainedCertificateCommandPassesValidation
        : CertificationValidationScenario
    {
        RunRulesForMustBeMaintainedCertificateCommandValidator Validator    { get; set; }
        RunRulesForMustBeMaintainedCertificateCommand Command               { get; set; }
        new Exception ExceptionCaught                                           { get; set; }
        ValidationResult ValidationResult                                   { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new RunRulesForMustBeMaintainedCertificateCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestRunRulesForMustBeMaintainedCertificateCommandBuilder.BuildValid();
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
    public class MissingCredentialIdInRunRulesForMustBeMaintainedCertificateCommandFailsValidation
        : RunRulesForMustBeMaintainedCertificateCommandValidationFailureBaseScenario
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
    /// The missing IssuanceId scenario
    /// </summary>
    public class MissingIssuanceIdInRunRulesForMustBeMaintainedCertificateCommandFailsValidation
        : RunRulesForMustBeMaintainedCertificateCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.IssuanceId = 0;
        }

        protected override string ExpectedErrorMessage()
        {
            return "IssuanceId is required";
        }
    }

    /// <summary>
    /// The missing EventDate scenario
    /// </summary>
    public class MissingEventDateInRunRulesForMustBeMaintainedCertificateCommandFailsValidation
        : RunRulesForMustBeMaintainedCertificateCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.EventDate = DateTime.MinValue;
        }

        protected override string ExpectedErrorMessage()
        {
            return "EventDate is required";
        }
    }

    /// <summary>
    /// The null CreatedBy scenario
    /// </summary>
    public class NullCreatedByInRunRulesForMustBeMaintainedCertificateCommandFailsValidation
        : RunRulesForMustBeMaintainedCertificateCommandValidationFailureBaseScenario
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
    public class EmptyCreatedByInRunRulesForMustBeMaintainedCertificateCommandFailsValidation
        : RunRulesForMustBeMaintainedCertificateCommandValidationFailureBaseScenario
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

    #endregion Scenarios
}
