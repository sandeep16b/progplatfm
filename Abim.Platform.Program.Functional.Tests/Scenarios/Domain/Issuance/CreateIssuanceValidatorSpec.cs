using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Domain.Issuance.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
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

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Issuance
{
    [Story(
        AsA = "process creating a domain object certification",
        IWant = "to be ensured that my domain object is valid",
        SoThat = "so that I can safely use this domain object to save to DB"
        )]
    [TestFixture]
    public class CreateIssuanceValidatorSpec
    {
        [TestCase]
        [WorkItem(90784)]
        public void ShouldPassValidationWhenDomainIsValid()
        {
            new CreateValidIssuanceDomainObjectTestReturnValidValidationResult().BDDfy();
        }

        [TestCase]
        [WorkItem(90784)]
        public void ShouldFailValidationWhenDomainHasEmptyProperties()
        {
            new CreateInValidIssuanceDomainObjectTestWithEmptyPropertiesReturnInValidValidationResult().BDDfy();
        }

        /// <summary>
        /// Base class for this file
        /// </summary>
        public abstract class CreateValidIssuanceDomainTestsScenario : IssuanceDomainScenario
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
        private class CreateValidIssuanceDomainObjectTestReturnValidValidationResult : CreateValidIssuanceDomainTestsScenario
        {
            App.Domain.Issuance Issuance { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            IssuanceValidator Validator { get; set; }

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
                Validator = new IssuanceValidator(Factory.Object);

                My<IValidator<App.Domain.Issuance>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Issuance>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Issuance>())
                    .Returns(My<IValidator<App.Domain.Issuance>>().Object);
            }

            void GivenInputAValidDomain()
            {
                // create valid Issuance
                Issuance = App.Domain.Issuance.Create(      source: SourceBuilder.Build(),
                                                            duration: EnumAttributes.RandomEntry<DurationType>(),
                                                            maintenanceRequirement: EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                                                            maintenanceStatus: EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                                                            occurrence: EnumAttributes.RandomEntry<OccurrenceType>(),
                                                            issuanceStatus: EnumAttributes.RandomEntry<IssuanceStatusType>(),
                                                            issuanceDate: DateTimeBuilder.Random().Build(),
                                                            effectiveDate: DateTimeBuilder.Random().Build(),
                                                            createdBy: RandomString.Build());

                Issuance.Credential = CredentialBuilder.Build();

            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Issuance);
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

        private class CreateInValidIssuanceDomainObjectTestWithEmptyPropertiesReturnInValidValidationResult : CreateValidIssuanceDomainTestsScenario
        {
            App.Domain.Issuance Issuance { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            IssuanceValidator Validator { get; set; }

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
                Validator = new IssuanceValidator(Factory.Object);

                My<IValidator<App.Domain.Issuance>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Issuance>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Issuance>())
                    .Returns(My<IValidator<App.Domain.Issuance>>().Object);
            }

            void GivenInputAInvalidValidDomain()
            {

                // create valid Issuance
                Issuance = App.Domain.Issuance.Create(source: null,
                                                            duration: EnumAttributes.RandomEntry<DurationType>(),
                                                            maintenanceRequirement: EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                                                            maintenanceStatus: EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                                                            occurrence: EnumAttributes.RandomEntry<OccurrenceType>(),
                                                            issuanceStatus: EnumAttributes.RandomEntry<IssuanceStatusType>(),
                                                            issuanceDate: DateTimeBuilder.Random().Build(),
                                                            effectiveDate: DateTimeBuilder.Random().Build(),
                                                            createdBy: RandomString.Build());
            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Issuance);
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
