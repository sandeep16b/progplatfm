using Abim.Platform.Program.Resources;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using Shouldly;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Domain.Credential
{
    [Story(
        AsA = "process updating a domain object credential's pathway",
        IWant = "to be ensured that the expected changes are made",
        SoThat = "so that I can be sure the properties are as expected afterward"
        )]
    [TestFixture]
    public class CredentialSetPathwaySpec
    {
        [Test]
        [WorkItem(224230)]
        public void Should_Set_Pathway_To_OneYear()
        {
            new CredentialSetPathwayScenario(PathwayType.MOC, PathwayType.OneYear).BDDfy();
        }

        [Test]
        [WorkItem(224230)]
        public void Should_Set_Pathway_To_Something_Other_Than_OneYear()
        {
            new CredentialSetPathwayScenario(PathwayType.OneYear, PathwayType.LNG).BDDfy();
        }

        private class CredentialSetPathwayScenario
        {
            private PathwayType _startingPathway;
            private PathwayType _endingPathway;
            private App.Domain.Credential _sut;
            private Exception _exception;

            public CredentialSetPathwayScenario(PathwayType startingPathway, PathwayType endingPathway)
            {
                _startingPathway = startingPathway;
                _endingPathway = endingPathway;
            }

            public void GivenIHaveACredential()
            {
                _sut = App.Domain.Credential.Create(
                    null,
                    Guid.NewGuid(),
                    CredentialType.General,
                    _startingPathway,
                    null, null,
                    "Unit Test");

                if (_startingPathway == PathwayType.OneYear)
                    _sut.IsInCMP = true;
            }

            public void WhenISetThePathway()
            {
                try
                {
                    _sut.SetPathway(_endingPathway, "BOOM SHAKA LAKA LAKA!");
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }

            public void ThenNoExceptionsShouldHaveOccurred()
            {
                _exception.ShouldBeNull();
            }

            public void AndThePathwayShouldBeWhatWasRequested()
            {
                _sut.Pathway.ShouldBe(_endingPathway);
            }

            public void AndTheIsInCMPFlagShouldBeAsExpected()
            {
                //NOTE: Changing pathway to OneYear will not *set* IsInCMP -- that happens during CMP enrollment.
                //But changing pathway to something *other than* OneYear will *clear* IsInCMP.

                //We set IsInCMP to true if our test says starting pathway is OneYear, so in that case,
                //let's make sure the flag gets cleared
                if (_startingPathway == PathwayType.OneYear) 
                    _sut.IsInCMP.ShouldBeFalse();
            }
        }
    }
}
