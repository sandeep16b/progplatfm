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
    public class CreateCredentialIssuanceCommandValidationSpec
    {
        [TestCase]
        public void ValidCreateCredentialIssuanceCommandPassesValidation()
        {
            new ValidCreateCredentialIssuanceCommandPassesValidation().BDDfy();
        }

        [TestCase]
        public void MissingCertificationIdInCreateCredentialIssuanceCommandFailsValidation()
        {
            new MissingCertificationIdInCreateCredentialIssuanceCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void MissingMemberIdInCreateCredentialIssuanceCommandFailsValidation()
        {
            new MissingMemberIdInCreateCredentialIssuanceCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void DefaultTypeInCreateCredentialIssuanceCommandFailsValidation()
        {
            new DefaultTypeInCreateCredentialIssuanceCommandFailsValidation().BDDfy();
        }

        [TestCase]
        public void DefaultPathwayTypeInCreateCredentialIssuanceCommandFailsValidation()
        {
            new DefaultPathwayTypeInCreateCredentialIssuanceCommandFailsValidation().BDDfy();
        }
    }

    #region Setup

    /// <summary>
    /// Command Builder
    /// </summary>
    public static class UnitTestCreateCredentialIssuanceCommandBuilder
    {
        public static Random Random = new Random();
        public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
        public static CreateCredentialIssuanceCommand BuildValid()
        {
            return CommandBuilder<CreateCredentialIssuanceCommand>
                        .Valid()
                        .With(cmd => cmd.ExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).Build())
                        .With(cmd => cmd.ReAttestationDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.ScheduledUpdate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).Build())
                        .With(cmd => cmd.DisplayExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.KCIExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .With(cmd => cmd.MOCExamDueDate = DateTimeBuilder.Random().WithYear(Random.Next(DateTime.Now.Year - 3, DateTime.Now.Year)).BuildOrNull())
                        .Build();
        }
    }

    /// <summary>
    /// Base class for validation failure scenarios
    /// </summary>
    public abstract class CreateCredentialIssuanceCommandValidationFailureBaseScenario
        : CertificationValidationScenario
    {
        protected CreateCredentialIssuanceCommand Command { get; set; }
        CreateCredentialIssuanceCommandValidator Validator { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new CreateCredentialIssuanceCommandValidator();
        }

        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIPassAnInvalidCommand()
        {
            Command = UnitTestCreateCredentialIssuanceCommandBuilder.BuildValid();
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
    public class ValidCreateCredentialIssuanceCommandPassesValidation
        : CertificationValidationScenario
    {
        CreateCredentialIssuanceCommandValidator Validator { get; set; }
        CreateCredentialIssuanceCommand Command { get; set; }
        new Exception ExceptionCaught { get; set; }
        ValidationResult ValidationResult { get; set; }

        /// <summary>
        /// Setup and mocking
        /// </summary>
        protected override void PreSetup()
        {
            Validator = new CreateCredentialIssuanceCommandValidator();
        }

        /// <summary>
        /// Secondary setup
        /// </summary>
        protected override void PostSetup()
        {
        }

        public void GivenIInputAValidCommand()
        {
            Command = UnitTestCreateCredentialIssuanceCommandBuilder.BuildValid();
        }

        public async Task WhenICallValidate()
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
    public class MissingCertificationIdInCreateCredentialIssuanceCommandFailsValidation
        : CreateCredentialIssuanceCommandValidationFailureBaseScenario
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
    public class MissingMemberIdInCreateCredentialIssuanceCommandFailsValidation
        : CreateCredentialIssuanceCommandValidationFailureBaseScenario
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
    /// The default Type scenario
    /// </summary>
    public class DefaultTypeInCreateCredentialIssuanceCommandFailsValidation
        : CreateCredentialIssuanceCommandValidationFailureBaseScenario
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
    public class DefaultPathwayTypeInCreateCredentialIssuanceCommandFailsValidation
        : CreateCredentialIssuanceCommandValidationFailureBaseScenario
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
