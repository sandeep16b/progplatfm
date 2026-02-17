using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators;
using Abim.Platform.Program.Tests.Scenarios.Validators.Certification.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Certification
{
    [Story(
        AsA = "process adding an added qualification certification",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class AddAddedQualificationCertificationCommandValidationSpec
    {
        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void ValidAddAddedQualificationCertificationCommandPassesValidation()
        {
            new ValidAddAddedQualificationCertificationCommandPassesValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void MissingBaseIdInAddAddedQualificationCertificationCommandFailsValidation()
        {
            new MissingBaseIdInAddAddedQualificationCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void NullNameInAddAddedQualificationCertificationCommandFailsValidation()
        {
            new NullNameInAddAddedQualificationCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void EmptyNameInAddAddedQualificationCertificationCommandFailsValidation()
        {
            new EmptyNameInAddAddedQualificationCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void NullCodeInAddAddedQualificationCertificationCommandFailsValidation()
        {
            new NullCodeInAddAddedQualificationCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void EmptyCodeInAddAddedQualificationCertificationCommandFailsValidation()
        {
            new EmptyCodeInAddAddedQualificationCertificationCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestAddAddedQualificationCertificationCommandBuilder
    {
        public static Random Random = new Random();
        public static AddAddedQualificationCertificationCommand BuildValid()
        {
            return CommandBuilder<AddAddedQualificationCertificationCommand>
                        .Valid()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class AddAddedQualificationCertificationCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected AddAddedQualificationCertificationCommand Command     { get; set; }
        AddAddedQualificationCertificationCommandValidator Validator    { get; set; }
        new Exception ExceptionCaught                                       { get; set; }
        ValidationResult ValidationResult                               { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new AddAddedQualificationCertificationCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestAddAddedQualificationCertificationCommandBuilder.BuildValid();
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
            catch (Exception ex)
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
    public class ValidAddAddedQualificationCertificationCommandPassesValidation
        : CertificationValidationScenario
    {
        AddAddedQualificationCertificationCommandValidator Validator    { get; set; }
        AddAddedQualificationCertificationCommand Command               { get; set; }
        new Exception ExceptionCaught                                       { get; set; }
        ValidationResult ValidationResult                               { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new AddAddedQualificationCertificationCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestAddAddedQualificationCertificationCommandBuilder.BuildValid();
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

    /// <summary>
    /// The missing BaseId scenario
    /// </summary>
    public class MissingBaseIdInAddAddedQualificationCertificationCommandFailsValidation
        : AddAddedQualificationCertificationCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.BaseId = Guid.Empty;
        }

        protected override string ExpectedErrorMessage()
        {
            return "BaseId is required";
        }
    }

    /// <summary>
    /// The null Name scenario
    /// </summary>
    public class NullNameInAddAddedQualificationCertificationCommandFailsValidation
        : AddAddedQualificationCertificationCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Name = null;
        }

        protected override string ExpectedErrorMessage()
        {
            return "Name is required";
        }
    }

    /// <summary>
    /// The empty Name scenario
    /// </summary>
    public class EmptyNameInAddAddedQualificationCertificationCommandFailsValidation
        : AddAddedQualificationCertificationCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Name = "";
        }

        protected override string ExpectedErrorMessage()
        {
            return "Name is required";
        }
    }

    /// <summary>
    /// The null Code scenario
    /// </summary>
    public class NullCodeInAddAddedQualificationCertificationCommandFailsValidation
        : AddAddedQualificationCertificationCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Code = null;
        }

        protected override string ExpectedErrorMessage()
        {
            return "Code is required";
        }
    }

    /// <summary>
    /// The empty Code scenario
    /// </summary>
    public class EmptyCodeInAddAddedQualificationCertificationCommandFailsValidation
        : AddAddedQualificationCertificationCommandValidationFailureBaseScenario
    {
        protected override void SetInvalidProperty()
        {
            Command.Code = "";
        }

        protected override string ExpectedErrorMessage()
        {
            return "Code is required";
        }
    }

    #endregion Scenarios
}
