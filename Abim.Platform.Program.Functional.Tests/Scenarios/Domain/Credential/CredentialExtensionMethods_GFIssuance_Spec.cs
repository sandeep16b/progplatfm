using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using FluentAssertions;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Domain.Credential
{
    [Story(
        AsA = "process creating a domain object credential",
        IWant = "to be ensured that my extension method is valid",
        SoThat = "so that I can safely use this method during processing"
        )]
    [TestFixture]
    public class CredentialExtensionMethods_GFIssuance_Spec
    {
        [Test]
        public void ShouldReturnCorrectResult_2ActiveGFIssuances_Spec()
        {
            new ShouldReturnCorrectResult_2ActiveGFIssuances().BDDfy();
        }

        [Test]
        public void ShouldReturnCorrectResult_1ActiveGFIssuances_Spec()
        {
            new ShouldReturnCorrectResult_1ActiveGFIssuances().BDDfy();
        }

        [Test]
        public void ShouldReturnCorrectResult_3ActiveGFIssuances_Spec()
        {
            new ShouldReturnCorrectResult_3ActiveGFIssuances().BDDfy();
        }

        [Test]
        public void ShouldReturnCorrectResult_2ActiveGFIssuances_1ActiveTL_Spec()
        {
            new ShouldReturnCorrectResult_2ActiveGFIssuances_1ActiveTL().BDDfy();
        }

        [Test]
        public void ShouldReturnCorrectResult_NoIssuances_Spec()
        {
            new ShouldReturnCorrectResult_NoIssuances().BDDfy();
        }

        [Test]
        public void ShouldReturnCorrectResult_NoGFIssuances_Spec()
        {
            new ShouldReturnCorrectResult_NoGFIssuances().BDDfy();
        }

        #region Scenarios

        #region Base Classes
        private abstract class CredentialExtensionMethodsScenario
        {
            protected App.Domain.Credential _sut;
            protected App.Domain.Issuance Result { get; set; }
            protected App.Domain.Issuance ExpectedReturnedIssuance { get; set; }
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
                    Result = _sut.GFIssuance;
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
                Result.Should().Be(ExpectedReturnedIssuance);
            }
        }

        #endregion Base Classes

        #region private methods
        private class ShouldReturnCorrectResult_2ActiveGFIssuances : CredentialExtensionMethodsScenario
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

                // set expected result
                ExpectedReturnedIssuance = IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(1999, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                                    .Build();

                _sut.AddIssuance(ExpectedReturnedIssuance);

                // additional active GF issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());
            }

        }

        private class ShouldReturnCorrectResult_1ActiveGFIssuances : CredentialExtensionMethodsScenario
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

                // set expected result
                ExpectedReturnedIssuance = IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(1999, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                                    .Build();

                _sut.AddIssuance(ExpectedReturnedIssuance);

            }

        }

        private class ShouldReturnCorrectResult_3ActiveGFIssuances : CredentialExtensionMethodsScenario
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

                // set expected result
                ExpectedReturnedIssuance = IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(1999, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                                    .Build();

                _sut.AddIssuance(ExpectedReturnedIssuance);

                // additional active GF issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());

                // additional active GF issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2001, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());

            }

        }

        private class ShouldReturnCorrectResult_2ActiveGFIssuances_1ActiveTL : CredentialExtensionMethodsScenario
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

                // set expected result
                ExpectedReturnedIssuance = IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(1999, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                                    .Build();

                _sut.AddIssuance(ExpectedReturnedIssuance);

                // additional active GF issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());

                // additional active TL issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.TimeLimited,
                                                     issuanceDate: new DateTime(2010, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());

            }

        }

        private class ShouldReturnCorrectResult_2ActiveGFIssuances_1MBM : CredentialExtensionMethodsScenario
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

                // set expected result
                ExpectedReturnedIssuance = IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(1999, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Initial,
                                                     source: null)
                                    .Build();

                _sut.AddIssuance(ExpectedReturnedIssuance);

                // additional active GF issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.GrandFather,
                                                     issuanceDate: new DateTime(2000, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Active,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());

                // additional active TL issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.MustBeMaintained,
                                                     issuanceDate: new DateTime(2010, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Expired,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());
            }
        }

        private class ShouldReturnCorrectResult_NoIssuances : CredentialExtensionMethodsScenario
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

                // set expected result
                ExpectedReturnedIssuance = null;

            }

        }

        private class ShouldReturnCorrectResult_NoGFIssuances : CredentialExtensionMethodsScenario
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

                // additional active TL issuance
                _sut.AddIssuance(IssuanceDataBuilder
                                    .StartWithCredentialCategory(category: CredentialCategoryType.MustBeMaintained,
                                                     issuanceDate: new DateTime(2010, 01, 01),
                                                     issuanceStatus: IssuanceStatusType.Expired,
                                                     occurrenceType: OccurrenceType.Recertification,
                                                     source: null)
                                    .Build());

            }
        }

        #endregion

        #endregion Scenarios
    }
}