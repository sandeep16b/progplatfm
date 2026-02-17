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
        AsA = "process adding a primary certification",
        IWant = "to be ensured that my command is valid",
        SoThat = "so that I can safely use the Handle() method"
        )]
    [TestFixture]
    public class AddPrimaryCertificationCommandValidationSpec
    {
        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void ValidAddPrimaryCertificationCommandPassesValidation()
        {
            new ValidAddPrimaryCertificationCommandPassesValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void NullNameInAddPrimaryCertificationCommandFailsValidation()
        {
            new NullNameInAddPrimaryCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void EmptyNameInAddPrimaryCertificationCommandFailsValidation()
        {
            new EmptyNameInAddPrimaryCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void NullCodeInAddPrimaryCertificationCommandFailsValidation()
        {
            new NullCodeInAddPrimaryCertificationCommandFailsValidation().BDDfy();
        }

        [TestCase]
        //[WorkItem(/*TODO: look up*/)]
        public void EmptyCodeInAddPrimaryCertificationCommandFailsValidation()
        {
            new EmptyCodeInAddPrimaryCertificationCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestAddPrimaryCertificationCommandBuilder
    {
        public static Random Random = new Random();
        public static AddPrimaryCertificationCommand BuildValid()
        {
            return CommandBuilder<AddPrimaryCertificationCommand>
                        .Valid()
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class AddPrimaryCertificationCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected AddPrimaryCertificationCommand Command     { get; set; }
        AddPrimaryCertificationCommandValidator Validator    { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        ValidationResult ValidationResult                    { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new AddPrimaryCertificationCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestAddPrimaryCertificationCommandBuilder.BuildValid();
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
    public class ValidAddPrimaryCertificationCommandPassesValidation
        : CertificationValidationScenario
    {
        AddPrimaryCertificationCommandValidator Validator    { get; set; }
        AddPrimaryCertificationCommand Command               { get; set; }
        new Exception ExceptionCaught                            { get; set; }
        ValidationResult ValidationResult                    { get; set; }
        
        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new AddPrimaryCertificationCommandValidator();
        }
        
        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestAddPrimaryCertificationCommandBuilder.BuildValid();
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
    /// The null Name scenario
    /// </summary>
    public class NullNameInAddPrimaryCertificationCommandFailsValidation
        : AddPrimaryCertificationCommandValidationFailureBaseScenario
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
    public class EmptyNameInAddPrimaryCertificationCommandFailsValidation
        : AddPrimaryCertificationCommandValidationFailureBaseScenario
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
    public class NullCodeInAddPrimaryCertificationCommandFailsValidation
        : AddPrimaryCertificationCommandValidationFailureBaseScenario
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
    public class EmptyCodeInAddPrimaryCertificationCommandFailsValidation
        : AddPrimaryCertificationCommandValidationFailureBaseScenario
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
