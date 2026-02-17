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
        AsA = "process adding a subspecialty certification",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class AddSubspecialtyCertificationCommandValidationSpec
    {
        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void ValidAddSubspecialtyCertificationCommandPassesValidation()
        {
            new ValidAddSubspecialtyCertificationCommandPassesValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void MissingBaseIdInAddSubspecialtyCertificationCommandFailsValidation()
        {
            new MissingBaseIdInAddSubspecialtyCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void NullNameInAddSubspecialtyCertificationCommandFailsValidation()
        {
            new NullNameInAddSubspecialtyCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void EmptyNameInAddSubspecialtyCertificationCommandFailsValidation()
        {
            new EmptyNameInAddSubspecialtyCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void NullCodeInAddSubspecialtyCertificationCommandFailsValidation()
        {
            new NullCodeInAddSubspecialtyCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void EmptyCodeInAddSubspecialtyCertificationCommandFailsValidation()
        {
            new EmptyCodeInAddSubspecialtyCertificationCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestAddSubspecialtyCertificationCommandBuilder
    {
        public static Random Random = new Random();
        public static AddSubspecialtyCertificationCommand BuildValid()
        {
            return CommandBuilder<AddSubspecialtyCertificationCommand>
                        .Valid()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class AddSubspecialtyCertificationCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected AddSubspecialtyCertificationCommand Command     { get; set; }
        AddSubspecialtyCertificationCommandValidator Validator    { get; set; }
        new Exception ExceptionCaught                                 { get; set; }
        ValidationResult ValidationResult                         { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new AddSubspecialtyCertificationCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestAddSubspecialtyCertificationCommandBuilder.BuildValid();
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
    public class ValidAddSubspecialtyCertificationCommandPassesValidation
        : CertificationValidationScenario
    {
        AddSubspecialtyCertificationCommandValidator Validator    { get; set; }
        AddSubspecialtyCertificationCommand Command               { get; set; }
        new Exception ExceptionCaught                                 { get; set; }
        ValidationResult ValidationResult                         { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new AddSubspecialtyCertificationCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestAddSubspecialtyCertificationCommandBuilder.BuildValid();
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
    public class MissingBaseIdInAddSubspecialtyCertificationCommandFailsValidation
        : AddSubspecialtyCertificationCommandValidationFailureBaseScenario
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
    public class NullNameInAddSubspecialtyCertificationCommandFailsValidation
        : AddSubspecialtyCertificationCommandValidationFailureBaseScenario
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
    public class EmptyNameInAddSubspecialtyCertificationCommandFailsValidation
        : AddSubspecialtyCertificationCommandValidationFailureBaseScenario
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
    public class NullCodeInAddSubspecialtyCertificationCommandFailsValidation
        : AddSubspecialtyCertificationCommandValidationFailureBaseScenario
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
    public class EmptyCodeInAddSubspecialtyCertificationCommandFailsValidation
        : AddSubspecialtyCertificationCommandValidationFailureBaseScenario
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
