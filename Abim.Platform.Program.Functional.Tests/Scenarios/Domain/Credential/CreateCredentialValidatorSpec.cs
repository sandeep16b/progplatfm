

using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
 
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.App.Domain;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using NLog;
using Abim.Platform.Program.Tests.Scenarios.Domain.Credential.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Testing.Scenarios.Validators.Credential
{
    [Story(
        AsA = "process creating a domain object credential",
        IWant = "to be ensured that my domain object is valid",
        SoThat = "so that I can safely use this domain object to save to DB"
        )]
    [TestFixture]
    public class CreateCredentialValidatorSpec
    {
        [TestCase]
        [WorkItem(90784)]
        public void ShouldPassValidationWhenDomainIsValid()
        {
            new CreateValidcredentialDomainObjectTestReturnValidValidationResult().BDDfy();
        }

        [TestCase]
        [WorkItem(90784)]
        public void ShouldFailValidationWhenDomainHasEmptyProperties()
        {
            new CreateInValidCredentialDomainObjectTestWithEmptyPropertiesReturnInValidValidationResult().BDDfy();
        }

        /// <summary>
        /// Base class for this file
        /// </summary>
        public abstract class CreateValidcredentialDomainTestsScenario : CredentialDomainScenario
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
        private class CreateValidcredentialDomainObjectTestReturnValidValidationResult: CreateValidcredentialDomainTestsScenario
        {
            App.Domain.Credential Credential { get; set; }
            App.Domain.Certification Certification { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            CredentialValidator Validator { get; set; }

            ValidationResult ValidationResult;

            //Exception ExceptionCaught { get; set; }

            Mock<ILogger> Log { get; set; }

            protected override void PreSetup()
            {
                Log = new Mock<ILogger>();
                Factory = new Mock<IValidationFactory> { DefaultValue = DefaultValue.Mock};
            }

            protected override void PostSetup()
            {
                Validator = new CredentialValidator(Factory.Object);

                My<IValidator<App.Domain.Credential>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Credential>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Credential>())
                    .Returns(My<IValidator<App.Domain.Credential>>().Object);
            }
            
            void GivenInputAValidDomain()
            {
                // create valid Source 
                //Source Source = App.Domain.Source.Create(name: RandomString.Build(),
                //                                         code: RandomString.Build(),
                //                                         createdBy: RandomString.Build());

                //// create valid Certification
                //Certification = App.Domain.Certification.Create(baseCertification: null,
                //                                            source: SourceBuilder.Build(),
                //                                            certType: EnumAttributes.RandomEntry<CertificationType>(),
                //                                            name: RandomString.BuildWithLength(255),
                //                                            code: RandomString.BuildWithLength(50),
                //                                            createdBy: RandomString.BuildWithLength(255));

                // create valid credential
                Credential = App.Domain.Credential.Create(certification: CertificationBuilder.Build(),
                                                            memberId: Guid.NewGuid(),
                                                            type: EnumAttributes.RandomEntry<CredentialType>(),
                                                            pathway: EnumAttributes.RandomEntry<PathwayType>(), 
                                                            onBehalfBoardCode: null, onBehalfBoardName: null, 
                                                            createdBy: RandomString.Build());
            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Credential);
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

        private class CreateInValidCredentialDomainObjectTestWithEmptyPropertiesReturnInValidValidationResult : CreateValidcredentialDomainTestsScenario
        {
            App.Domain.Credential Credential { get; set; }
            App.Domain.Certification Certification { get; set; }

            Mock<IValidationFactory> Factory { get; set; }

            CredentialValidator Validator { get; set; }

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
                Validator = new CredentialValidator(Factory.Object);

                My<IValidator<App.Domain.Credential>>()
                .Setup(o => o.Validate(It.IsAny<App.Domain.Credential>()))
                .Returns(new ValidationResult());

                My<IValidationFactory>()
                    .Setup(o => o.GetValidatorInstance<App.Domain.Credential>())
                    .Returns(My<IValidator<App.Domain.Credential>>().Object);
            }

            void GivenInputAInvalidValidDomain()
            {
                //// create valid Certification
                //Certification = App.Domain.Certification.Create(baseCertification: null,
                //                                            source: null,
                //                                            certType: EnumAttributes.RandomEntry<CertificationType>(),
                //                                            name: string.Empty,
                //                                            code: string.Empty,
                //                                            createdBy: string.Empty);

                // create valid credential
                Credential = App.Domain.Credential.Create(certification: null,
                                                            memberId: new Guid(),
                                                            type: EnumAttributes.RandomEntry<CredentialType>(),
                                                            pathway: EnumAttributes.RandomEntry<PathwayType>(),
                                                            onBehalfBoardCode: null, onBehalfBoardName: null, 
                                                            createdBy: string.Empty);
            }

            public void WhenICallValidate()
            {
                ValidationResult = Validator.Validate(Credential);
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
