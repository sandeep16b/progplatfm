using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using FluentAssertions;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using CredentialCategoryType = Abim.Platform.Program.Resources.CredentialCategoryType;


namespace Abim.Platform.Program.Tests.Scenarios.Domain.Credential
{
    [Story(
        AsA = "process creating a domain object credential",
        IWant = "to be ensured that my extension method is valid",
        SoThat = "so that I can safely use this method during processing"
        )]
    [TestFixture]
    public class CredentialExtensionMethods_ExpiredGF_Spec
    {
        [Test]
        public void ShouldReturnTrue_ExpiredGF()
        {
            new ShouldHaveCorrectResultScenario_ExpiredGF().BDDfy();
        }

        [Test]
        public void ShouldReturnFalse_ExpiredGF_MBM()
        {
            new ShouldHaveCorrectResultScenario_ExpiredGF_MBM().BDDfy();
        }

        [Test]
        public void ShouldReturnFalse_ExpiredGF_TL()
        {
            new ShouldHaveCorrectResultScenario_ExpiredGF_TL().BDDfy();
        }

        [Test]
        public void ShouldReturnFalse_NoIssuances()
        {
            new ShouldHaveCorrectResultScenario_NoIssuances().BDDfy();
        }

        [Test]
        public void ShouldReturnFalse_ActiveGF()
        {
            new ShouldHaveCorrectResultScenario_ActiveGF().BDDfy(); 
        }

        [Test]
        public void ShouldReturnFalse_Expired_MBM()
        {
            new ShouldHaveCorrectResultScenario_Expired_MBM().BDDfy(); 
        }

        [Test]
        public void ShouldReturnFalse_Expired_TL()
        {
            new ShouldHaveCorrectResultScenario_Expired_TL().BDDfy(); 
        }

        #region Scenarios

        #region Base Classes
        private abstract class CredentialExtensionMethodsScenario
        {
            protected App.Domain.Credential _sut;
            protected bool Result { get; set; }
            protected bool ExpectedResult { get; set; }
            protected Exception ExceptionCaught { get; set; }

            //++ Domain Data Builders ++++
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();

            protected void WhenICheckExtenstionMethod()
            {
                try
                {
                    Result = _sut.IsExpiredGrandfather;
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

            public void ThenResultShouldBeTrue()
            {
                Result.Should().Be(ExpectedResult);
            }
        }

        #endregion Base Classes

        #region private methods
        private class ShouldHaveCorrectResultScenario_ExpiredGF : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");
                // expired GF
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Expired,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                        .With(a => a.ExpiredDate = new DateTime(2018, 12, 31))
                        .Build());

                // set expected results
                ExpectedResult = true;

            }

        }

        private class ShouldHaveCorrectResultScenario_ActiveGF : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");
                // active GF
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                        .Build());

                // set expected results
                ExpectedResult = false;

            }

        }

        private class ShouldHaveCorrectResultScenario_NoIssuances : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");

                // set expected results
                ExpectedResult = false;

            }

        }

        private class ShouldHaveCorrectResultScenario_ExpiredGF_MBM : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");
                // expired GF
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Expired,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                        .With(a => a.ExpiredDate = new DateTime(2018, 12, 31))
                        .Build());

                // MBM issuance (invalidate expired GF)
                _sut.AddIssuance(IssuanceDataBuilder
                    .StartWithCredentialCategory(category: CredentialCategoryType.MustBeMaintained,
                                     issuanceDate: new DateTime(2019, 01, 01),
                                     issuanceStatus: IssuanceStatusType.Active,
                                     occurrenceType: OccurrenceType.Recertification,
                                     source: null)
                        .Build());

                // set expected results
                ExpectedResult = false;

            }

        }

        private class ShouldHaveCorrectResultScenario_ExpiredGF_TL : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");
                // expired GF
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Expired,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                        .With(a => a.ExpiredDate = new DateTime(2018, 12, 31))
                        .Build());

                // TL issuance (invalidate expired GF)
                _sut.AddIssuance(IssuanceDataBuilder
                    .StartWithCredentialCategory(category: CredentialCategoryType.TimeLimited,
                                     issuanceDate: new DateTime(2019, 01, 01),
                                     issuanceStatus: IssuanceStatusType.Active,
                                     occurrenceType: OccurrenceType.Recertification,
                                     source: null)
                        .Build());

                // set expected results
                ExpectedResult = false;

            }

        }

        private class ShouldHaveCorrectResultScenario_Expired_TL : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");

                // TL issuance (invalidate expired GF)
                _sut.AddIssuance(IssuanceDataBuilder
                    .StartWithCredentialCategory(category: CredentialCategoryType.TimeLimited,
                                     issuanceDate: new DateTime(2014, 01, 01),
                                     issuanceStatus: IssuanceStatusType.Expired,
                                     occurrenceType: OccurrenceType.Initial,
                                     source: null)
                        .Build());

                // set expected results
                ExpectedResult = false;

            }
        }

        private class ShouldHaveCorrectResultScenario_Expired_MBM : CredentialExtensionMethodsScenario
        {
            protected void GivenIHaveACredential()
            {

                _sut = App.Domain.Credential.Create(
                                null,
                                Guid.NewGuid(),
                                CredentialType.General,
                                PathwayType.MOC,
                                null, null,
                                "Unit Test");

                // TL issuance (invalidate expired GF)
                _sut.AddIssuance(IssuanceDataBuilder
                    .StartWithCredentialCategory(category: CredentialCategoryType.MustBeMaintained,
                                     issuanceDate: new DateTime(2015, 01, 01),
                                     issuanceStatus: IssuanceStatusType.Expired,
                                     occurrenceType: OccurrenceType.Initial,
                                     source: null)
                        .Build());

                // set expected results
                ExpectedResult = false;

            }
        }
        #endregion

        #endregion Scenarios
    }
}