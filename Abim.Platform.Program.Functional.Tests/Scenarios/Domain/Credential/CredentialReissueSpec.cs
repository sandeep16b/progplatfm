using System;
using NUnit.Framework;
using Shouldly;
using TestStack.BDDfy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Scenarios.Domain.Credential
{
    [Story(
        AsA = "process working with a domain object credential",
        IWant = "to make use of the Reissue() method",
        SoThat = "so that I can properly reissue issuances"
        )]
    [TestFixture]
    public class CredentialReissueSpec
    {
        [Test]
        [WorkItem(183137)]
        public void Should_Carry_Over_Deselect_Values_When_Applicable()
        {
            new ShouldCarryOverDeselectValuesWhenApplicableScenario().BDDfy();
        }

        [Test]
        [WorkItem(183137)]
        public void Should_Not_Carry_Over_Deselect_Values_When_Cert_Not_Deselected()
        {
            new ShouldNotCarryOverDeselectValuesWhenCertNotDeselectedScenario().BDDfy();
        }

        [Test]
        [WorkItem(183137)]
        public void Should_Not_Carry_Over_Deselect_Values_When_Cert_Already_Processed_For_Deselection()
        {
            new ShouldNotCarryOverDeselectValuesWhenCertAlreadyProcessedForDeselectionScenario().BDDfy();
        }

        #region Scenarios

        #region Base Scenario
        private abstract class CredentialReissueScenarioBase
        {
            protected bool _issuanceHasBeenMarkedForDeselection;
            protected App.Domain.Credential _sut;
            protected Exception _exception;
            protected DateTime? _deselectDate; //Using the same date for all 3 values

            public CredentialReissueScenarioBase(bool markedForDeselection)
            {
                _issuanceHasBeenMarkedForDeselection = markedForDeselection;
            }

            protected void GivenIHaveACredential()
            {
                _sut = App.Domain.Credential.Create(
                    null,
                    Guid.NewGuid(),
                    CredentialType.General,
                    PathwayType.MOC,
                    null, null,
                    "Unit Test");

                var issuance = App.Domain.Issuance.Create("Unit Test");

                if (_issuanceHasBeenMarkedForDeselection)
                {
                    _deselectDate = DateTime.Now;

                    //In reality, we wouldn't have the same value for both
                    //properties, but in this scenario, it doesn't matter.
                    issuance.DeselectionSubmittedDate = _deselectDate;
                    issuance.DeselectionEffectiveDate = _deselectDate;
                    /*
                    We don't set DeselectionProcessedDate because if an issuance
                    has been marked for deselection, but not actually processed
                    for deselection, the DeselectionProcessedDate value will be null.
                    */
                }

                _sut.AddIssuance(issuance);
            }

            public void WhenICallReissue()
            {
                try
                {
                    _sut.Reissue(
                        App.Domain.Source.Create("ABIM", "ABIM", "Unit Test"),
                        DateTime.Now,
                        DateTime.Now,
                        "Unit Test");
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }

            public void ThenNoExceptionShouldHaveOccurred()
            {
                _exception.ShouldBeNull();
            }
        }
        #endregion Base Scenario

        #region Should Carry Over Deselect Values When Applicable Scenario
        private class ShouldCarryOverDeselectValuesWhenApplicableScenario : CredentialReissueScenarioBase
        {
            public ShouldCarryOverDeselectValuesWhenApplicableScenario() : base(true)
            {
            }

            /*
            Base class executes the following:
            GivenIHaveACredential() -- Existing issuance is marked for deselection in this case
            WhenICallReissue()
            ThenNoExceptionShouldHaveOccurred()
            */

            public void AndANewIssuanceShouldHaveBeenCreatedAndUpdatedWithTheDeselectValues()
            {
                var newestIssuance = _sut.NewestIssuance;
                newestIssuance.DeselectionSubmittedDate.ShouldBe(_deselectDate);
                newestIssuance.DeselectionEffectiveDate.ShouldBe(_deselectDate);
                newestIssuance.DeselectionProcessedDate.ShouldBeNull();
            }

            public void AndThePreviousIssueShouldNoLongerHaveTheDeselectValues()
            {
                var oldestIssuance = _sut.OldestIssuance;
                oldestIssuance.DeselectionSubmittedDate.ShouldBeNull();
                oldestIssuance.DeselectionEffectiveDate.ShouldBeNull();
                oldestIssuance.DeselectionProcessedDate.ShouldBeNull();
            }
        }
        #endregion Should Carry Over Deselect Values When Applicable Scenario

        #region Should Not Carry Over Deselect Values When Cert Not Deselected Scenario
        private class ShouldNotCarryOverDeselectValuesWhenCertNotDeselectedScenario : CredentialReissueScenarioBase
        {
            public ShouldNotCarryOverDeselectValuesWhenCertNotDeselectedScenario() : base(false)
            { }

            /*
            Base class executes the following:
            GivenIHaveACredential() -- Existing issuance is not marked for deselection in this case
            WhenICallReissue()
            ThenNoExceptionShouldHaveOccurred()
            */

            public void AndANewIssuanceShouldHaveBeenCreatedWithNullDeselectValues()
            {
                var newestIssuance = _sut.NewestIssuance;
                newestIssuance.DeselectionSubmittedDate.ShouldBeNull();
                newestIssuance.DeselectionEffectiveDate.ShouldBeNull();
                newestIssuance.DeselectionProcessedDate.ShouldBeNull();
            }
        }
        #endregion Should Not Carry Over Deselect Values When Cert Not Deselected Scenario

        #region Should Not Carry Over Deselect Values When Cert Already Processed For Deselection Scenario
        private class ShouldNotCarryOverDeselectValuesWhenCertAlreadyProcessedForDeselectionScenario : ShouldNotCarryOverDeselectValuesWhenCertNotDeselectedScenario
        {
            /*
            Base class executes the following:
            GivenIHaveACredential() -- Existing issuance is not marked for deselection in this case
            */

            public void AndGivenTheNewestIssuanceHasAlreadyBeenProcessedForDeselection()
            {
                _sut.NewestIssuance.DeselectionProcessedDate = DateTime.Now;
            }

            /*
            Base class executes the following:
            WhenICallReissue()
            ThenNoExceptionShouldHaveOccurred()
            AndANewIssuanceShouldHaveBeenCreatedWithNullDeselectValues()
            */
        }
        #endregion Should Not Carry Over Deselect Values When Cert Already Processed For Deselection Scenario

        #endregion Scenarios
    }
}
