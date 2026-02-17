using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using FluentValidation.Results;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Domain.Lookback
{
    [Story(
        AsA = "process creating a domain object lookback",
        IWant = "to be ensured that my domain object is valid",
        SoThat = "so that I can safely use this domain object to save to DB"
        )]
    [TestFixture]
    public class LookbackValidatorSpec
    {
        [Test]
        public void HasEmptyPropertiesScenario()
        {
            new EmptyLookbackScenario().BDDfy();
        }

        [Test]
        public void ValidLookbackScenario()
        {
            new ValidLookbackScenario().BDDfy();
        }
    }

    #region Scenerios

    

    public class ValidLookbackScenario : BaseValidationScenario
    {
        LookbackLog Entity                { get; set; }
        LookbackLogValidator Validator    { get; set; }
        ValidationResult ValidationResult { get; set; }
        //Exception ExceptionCaught         { get; set; }

        public void GivenAValidDomainObject()
        {
            Entity = new LookbackLog
            {
                Action          = LookbackActionType.FailurePoint,
                AuditData       = AuditData.Create("UserName"),
                ExternalId      = Guid.NewGuid(),
                IsPendingAction = true,
                LogDate         = DateTime.Now,
                Reason          = LookbackReasonType.TwoYear,
                Status          = LookbackStatusType.CredentialStatus,
                Credential      = App.Domain.Credential.Create(certification: CertificationBuilder.Build(),
                                        memberId: Guid.NewGuid(),
                                        type: EnumAttributes.RandomEntry<CredentialType>(),
                                        pathway: EnumAttributes.RandomEntry<PathwayType>(),
                                        onBehalfBoardCode: null, onBehalfBoardName: null, 
                                        createdBy: RandomString.Build())
            };
        }

        protected override void PostSetup()
        {
            
        }

        protected override void PreSetup()
        {

        }

        public void AndGivenIHaveAValidator()
        {
            Validator = new LookbackLogValidator();
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

    public class EmptyLookbackScenario : BaseValidationScenario
    {
        LookbackLog Entity { get; set; }
        LookbackLogValidator Validator { get; set; }
        ValidationResult ValidationResult { get; set; }
        //Exception ExceptionCaught { get; set; }

        protected override void PreSetup()
        {
                                                                                                                                                                                                      
        }

        protected override void PostSetup()                                                                     
        {
                                                                                                      
        }

        public void GivenAnEmptyDomainObject()
        {
            Entity = new LookbackLog();                                                                                            
        }

        public void AndGivenIHaveAValidator()
        {
            Validator = new LookbackLogValidator();
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
            ValidationResult.Errors.Should().HaveCount(3);
        }
    }

    #endregion
}
