using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.Resources;
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
        AsA = "process expiring an issuance",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class ExpireIssuanceCommandValidationSpec
    {
        [TestCase]
        [WorkItem(73194)]
        public void ValidExpireIssuanceCommandPassesValidation()
        {
            new ValidExpireIssuanceCommandPassesValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void MissingCredentialIdInExpireIssuanceCommandFailsValidation()
        {
            new MissingCredentialIdInExpireIssuanceCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void MissingIssuanceIdInExpireIssuanceCommandFailsValidation()
        {
            new MissingIssuanceIdInExpireIssuanceCommandFailsValidation().BDDfy();
        }

        [TestCase]
        [WorkItem(73194)]
        public void DefaultStatusInExpireIssuanceCommandFailsValidation()
        {
            new DefaultStatusInExpireIssuanceCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestExpireIssuanceCommandBuilder
    {
        public static Random Random = new Random();
        public static ExpireIssuanceCommand BuildValid()
        {
            return CommandBuilder<ExpireIssuanceCommand>
                        .Valid()
                        .WithNoZeroIntegers()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class ExpireIssuanceCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected ExpireIssuanceCommand Command     { get; set; }
        ExpireIssuanceCommandValidator Validator    { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        ValidationResult ValidationResult           { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ExpireIssuanceCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestExpireIssuanceCommandBuilder.BuildValid();
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
    public class ValidExpireIssuanceCommandPassesValidation
        : CertificationValidationScenario
    {
        ExpireIssuanceCommandValidator Validator    { get; set; }
        ExpireIssuanceCommand Command               { get; set; }
        new Exception ExceptionCaught                   { get; set; }
        ValidationResult ValidationResult           { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new ExpireIssuanceCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestExpireIssuanceCommandBuilder.BuildValid();
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
    public class MissingCredentialIdInExpireIssuanceCommandFailsValidation
        : ExpireIssuanceCommandValidationFailureBaseScenario
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
    public class MissingIssuanceIdInExpireIssuanceCommandFailsValidation
        : ExpireIssuanceCommandValidationFailureBaseScenario
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
    /// The default Status scenario
    /// </summary>
    public class DefaultStatusInExpireIssuanceCommandFailsValidation
        : ExpireIssuanceCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Status = default(IssuanceStatusType);
        }

        protected override string ExpectedErrorMessage()
        {
            return "Status is required";
        }
    }

    #endregion Scenarios
}
