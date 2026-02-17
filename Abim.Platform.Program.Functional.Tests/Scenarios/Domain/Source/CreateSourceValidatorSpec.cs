using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Tests.Scenarios.Domain.Source.Base;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;


namespace Abim.Platform.Program.Testing.Scenarios.Validators.Source
{
    [Story(
        AsA = "process creating a domain object certification",
        IWant = "to be ensured that my domain object is valid",
        SoThat = "so that I can safely use this domain object to save to DB"
        )]
    [TestFixture]
    public class CreateSourceValidatorSpec
    {
        [TestCase]
        [WorkItem(90784)]
        public void ShouldPassValidationWhenDomainIsValid()
        {
            new CreateValidSourceDomainObjectTestReturnValidValidationResult().BDDfy();
        }

        [TestCase]
        [WorkItem(90784)]
        public void ShouldFailValidationWhenDomainHasEmptyProperties()
        {
            new CreateInValidSourceDomainObjectTestWithEmptyPropertiesReturnInValidValidationResult().BDDfy();
        }

        [TestCase]
        [WorkItem(90784)]
        public void ShouldFailValidationWhenDomainPropertiesExceededMaxLength()
        {
            new CreateInValidSourceDomainObjectTestWithPropertiesExceededMaxLengthReturnInValidValidationResult().BDDfy();
        }

        /// <summary>
        /// Base class for this file
        /// </summary>
        public abstract class CreateValidSourceDomainTestsScenario : SourceDomainScenario
        {
            protected override List<Type> AdditionalDependencies()
            {
                var types = base.AdditionalDependencies();
                types.Add(typeof(IBusControl));
                types.Add(typeof(IValidationFactory));
                return types;
            }
        }

        #region Scenarios
        private class CreateValidSourceDomainObjectTestReturnValidValidationResult : CreateValidSourceDomainTestsScenario
        {
            App.Domain.Source Source { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            SourceValidator Validator { get; set; }

            ValidationResult ValidationResult;

            //Exception ExceptionCaught { get; set; }

            Mock<ILogger> Log { get; set; }

            protected override void PreSetup()
            {
                Log = new Mock<ILogger>();
                Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
            }

            protected override void PostSetup()
            {
                Validator = new SourceValidator(Factory.Object);

                My<IValidator<App.Domain.Source>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Source>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Source>())
                    .Returns(My<IValidator<App.Domain.Source>>().Object);
            }

            void GivenInputAValidDomain()
            {
                // create valid Source
                Source = App.Domain.Source.Create(name: RandomString.BuildWithLength(255),
                                                code: RandomString.BuildWithLength(10),
                                                createdBy: RandomString.BuildWithLength(255));

            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Source);
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

        private class CreateInValidSourceDomainObjectTestWithEmptyPropertiesReturnInValidValidationResult : CreateValidSourceDomainTestsScenario
        {
            App.Domain.Source Source { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            SourceValidator Validator { get; set; }

            ValidationResult ValidationResult;

            //Exception ExceptionCaught { get; set; }

            Mock<ILogger> Log { get; set; }

            protected override void PreSetup()
            {
                Log = new Mock<ILogger>();
                Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
            }

            protected override void PostSetup()
            {
                Validator = new SourceValidator(Factory.Object);

                My<IValidator<App.Domain.Source>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Source>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Source>())
                    .Returns(My<IValidator<App.Domain.Source>>().Object);
            }

            void GivenInputAInvalidValidDomain()
            {

                // create invalid Source
                Source = App.Domain.Source.Create(name: string.Empty,
                                                code: string.Empty,
                                                createdBy: string.Empty);
            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Source);
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

        private class CreateInValidSourceDomainObjectTestWithPropertiesExceededMaxLengthReturnInValidValidationResult : CreateValidSourceDomainTestsScenario
        {
            App.Domain.Source Source { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            SourceValidator Validator { get; set; }

            ValidationResult ValidationResult;

            //Exception ExceptionCaught { get; set; }

            Mock<ILogger> Log { get; set; }

            protected override void PreSetup()
            {
                Log = new Mock<ILogger>();
                Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock };
            }

            protected override void PostSetup()
            {
                Validator = new SourceValidator(Factory.Object);

                My<IValidator<App.Domain.Source>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Source>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Source>())
                    .Returns(My<IValidator<App.Domain.Source>>().Object);
            }

            void GivenInputAInvalidValidDomain()
            {
                // create valid Source
                Source = App.Domain.Source.Create(name: RandomString.BuildWithLength(256),
                                                code: RandomString.BuildWithLength(11),
                                                createdBy: RandomString.BuildWithLength(256));
            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Source);
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
                ValidationResult.Errors.Should().HaveCount(2);
            }
        }
        #endregion Scenarios

    }

}
