using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using FluentValidation.Results;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Domain.LookbackDateLog
{
    [Story(
       AsA = "process creating a domain object lookbackDateLog",
       IWant = "to be ensured that my domain object is valid",
       SoThat = "so that I can safely use this domain object to save to DB"
       )]
    [TestFixture]
    public class LookbackDateLogValidatorSpec
    {
        [Test]
        [WorkItem(139174)]
        public void HasEmptyPropertiesScenario()
        {
            new EmptyLookbackDateLogScenario().BDDfy();
        }

        [Test]
        [WorkItem(139174)]
        public void ValidLookbackScenario()
        {
            new ValidLookbackDateScenario().BDDfy();
        }
        
        [Test]
        [WorkItem(139174)]
        public void LookbackDateMissingAuditDataScenario()
        {
            new LookbackDateMissingAuditDataScenario().BDDfy();
        }
        
        [Test]
        [WorkItem(139174)]
        public void LookbackDateMissingLookbackDateScenario()
        {
            new LookbackDateMissingLookbackDateScenario().BDDfy();
        }

        [Test]
        [WorkItem(139174)]
        public void LookbackDateMissingChangedDateScenario()
        {
            new LookbackDateMissingChangedDateScenario().BDDfy();
        }

        [Test]
        [WorkItem(139174)]
        public void LookbackDateMissingMemberGuidScenario()
        {
            new LookbackDateMissingMemberGuidScenario().BDDfy();
        }
        
        [Test]
        [WorkItem(139174)]
        public void LookbackDateOldValueDateTimeMinScenario()
        {
            new LookbackDateOldValueDateTimeMinScenario().BDDfy();
        }
        [Test]
        [WorkItem(139174)]
        public void LookbackDateNewValueDateTimeMinScenario()
        {
            new LookbackDateNewValueDateTimeMinScenario().BDDfy();
        }
    }

    #region Scenerios
    /// <summary>
    /// CreateLookbackDateLogDomainTestsScenario
    /// </summary>
    public abstract class CreateLookbackDateLogDomainTestsScenario : BaseValidationScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(IValidationFactory));
            return types;
        }
    }
    /// <summary>
    /// ValidLookbackDateScenario
    /// </summary>
    public class ValidLookbackDateScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        //Exception ExceptionCaught { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                AuditData = AuditData.Create("test"),
                ChangedDate = DateTime.Now,
                LookbackDate = LookbackDateType.FiveYearEnd,
                MemberGuid = Guid.NewGuid(),
                OldValue = DateTime.Now,
                NewValue = DateTime.Now.AddYears(5)
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHavePassed()
        {
            ValidationResult.IsValid.Should().BeTrue();
            ValidationResult.Errors.Should().BeEmpty();
        }
    }
    /// <summary>
    /// EmptyLookbackDateLogScenario
    /// </summary>
    public class EmptyLookbackDateLogScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        Mock<IValidationFactory> Factory { get; set; }
        ValidationResult ValidationResult { get; set; }
        //Exception ExceptionCaught { get; set; }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void GivenAnEmptyDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog();
        }

        public void AndGivenIHaveAValidator()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFail()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }

        public void AndTheValidationShouldHaveFewErrors()
        {
            ValidationResult.Errors.Should().HaveCount(4);
        }
    }

    public class LookbackDateMissingAuditDataScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                ChangedDate = DateTime.Now,
                LookbackDate = LookbackDateType.FiveYearEnd,
                MemberGuid = Guid.NewGuid(),
                OldValue = DateTime.Now,
                NewValue = DateTime.Now.AddYears(5)
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }
        private void AndTheCorrectValidationErrorShouldBePresent()
        {
            ValidationResult
                .Errors
                .Any(x => x.ErrorMessage.Contains("AuditData"))
                .Should().BeTrue();
        }
    }

    public class LookbackDateMissingLookbackDateScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                AuditData = AuditData.Create("test"),
                ChangedDate = DateTime.Now,
                MemberGuid = Guid.NewGuid(),
                OldValue = DateTime.Now,
                NewValue = DateTime.Now.AddYears(5)
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }
        private void AndTheCorrectValidationErrorShouldBePresent()
        {
            ValidationResult
                .Errors
                .Any(x => x.ErrorMessage.Contains("LookbackDate"))
                .Should().BeTrue();
        }
    }

    public class LookbackDateMissingMemberGuidScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                AuditData = AuditData.Create("test"),
                ChangedDate = DateTime.Now,
                LookbackDate = LookbackDateType.FiveYearEnd,
                OldValue = DateTime.Now,
                NewValue = DateTime.Now.AddYears(5)
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }
        private void AndTheCorrectValidationErrorShouldBePresent()
        {
            ValidationResult
                .Errors
                .Any(x => x.ErrorMessage.Contains("MemberGuid"))
                .Should().BeTrue();
        }
    }

    public class LookbackDateMissingChangedDateScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                AuditData = AuditData.Create("test"),
                LookbackDate = LookbackDateType.FiveYearEnd,
                MemberGuid = Guid.NewGuid(),
                OldValue = DateTime.Now,
                NewValue = DateTime.Now.AddYears(5)
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }
        private void AndTheCorrectValidationErrorShouldBePresent()
        {
            ValidationResult
                .Errors
                .Any(x => x.ErrorMessage.Contains("ChangedDate"))
                .Should().BeTrue();
        }
    }


    public class LookbackDateOldValueDateTimeMinScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                AuditData = AuditData.Create("test"),
                ChangedDate = DateTime.Now,
                LookbackDate = LookbackDateType.FiveYearEnd,
                MemberGuid = Guid.NewGuid(),
                OldValue = new DateTime(),
                NewValue = DateTime.Now.AddYears(5)
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }
        private void AndTheCorrectValidationErrorShouldBePresent()
        {
            ValidationResult
                .Errors
                .Any(x => x.ErrorMessage.Contains("OldValue"))
                .Should().BeTrue();
        }
    }

    public class LookbackDateNewValueDateTimeMinScenario : CreateLookbackDateLogDomainTestsScenario
    {
        App.Domain.LookbackDateLog Entity { get; set; }
        LookbackDateLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        Mock<IValidationFactory> Factory { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new App.Domain.LookbackDateLog
            {
                AuditData = AuditData.Create("test"),
                ChangedDate = DateTime.Now,
                LookbackDate = LookbackDateType.FiveYearEnd,
                MemberGuid = Guid.NewGuid(),
                OldValue = DateTime.Now,
                NewValue = new DateTime()
            };
        }

        protected override void PostSetup()
        {
            Validator = new LookbackDateLogValidator(Factory.Object);
        }

        protected override void PreSetup()
        {
            Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
        }

        public void WhenICallValidate()
        {
            ValidationResult = Validator.Validate(Entity);
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndTheValidationShouldHaveFailed()
        {
            ValidationResult.IsValid.Should().BeFalse();
            ValidationResult.Errors.Should().NotBeEmpty();
        }
        private void AndTheCorrectValidationErrorShouldBePresent()
        {
            ValidationResult
                .Errors
                .Any(x => x.ErrorMessage.Contains("NewValue"))
                .Should().BeTrue();
        }
    }

    #endregion
}
