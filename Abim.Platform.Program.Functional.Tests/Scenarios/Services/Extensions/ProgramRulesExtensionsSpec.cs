using System;
using System.Collections.Generic;
using System.Linq;
using Abim.Enterprise.Core.Resource.Program;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using NUnit.Framework;
using TestStack.BDDfy;
using FluentAssertions;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Extensions
{
    public class ProgramRulesExtensionsSpec
    {
        [TestCase]
        [WorkItem(148965)]
        public void EarnedNewSubspecialtyInitialCertEvents_Selects_The_Oldest_Issuance()
        {
            new EarnedNewSubspecialtyInitialCertEvents_Should_Select_The_Oldest_Issuance().BDDfy();
        }

        #region Scenarios 

        private class EarnedNewSubspecialtyInitialCertEvents_Should_Select_The_Oldest_Issuance
        {
            private List<Credential> _credentials;
            private Exception _caughtException;
            private List<DateTime> _results;

            private void GivenIHaveAListOfCredentials()
            {
                _credentials = new List<Credential>(3);

                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "Unit Test");

                var cred1 = 
                    CredentialBuilder
                        .BuildWithoutRandoms(
                            source, "IM", "Internal Medicine", 
                            CertificationType.Primary, CredentialType.General, PathwayType.MOC);

                cred1.AddIssuance(IssuanceBuilder.BuildActiveMaintained());

                var cred2 = 
                    CredentialBuilder
                        .BuildWithoutRandoms(
                            source, "JAZZ", "Smooth Jazz...ohhhh yeah...",
                            CertificationType.Primary, CredentialType.General, PathwayType.MOC);

                cred2.AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        source, IssuanceStatusType.Expired, new DateTime(2010, 1, 1), 
                        DurationType.Continuous, MaintenanceRequirementType.Required, 
                        MaintenanceStatusType.Maintained, OccurrenceType.Initial));

                cred2.AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        source, IssuanceStatusType.Active, new DateTime(2020, 1, 1),
                        DurationType.Continuous, MaintenanceRequirementType.Required,
                        MaintenanceStatusType.Maintained, OccurrenceType.Initial));

                var cred3 =
                    CredentialBuilder
                        .BuildWithoutRandoms(
                            source, "BION", "Bionics! (insert bionic sound effect here)",
                            CertificationType.Primary, CredentialType.General, PathwayType.MOC);

                cred3.AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        source, IssuanceStatusType.Expired, new DateTime(2012, 1, 1),
                        DurationType.Continuous, MaintenanceRequirementType.Required,
                        MaintenanceStatusType.Maintained, OccurrenceType.Initial));

                cred3.AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        source, IssuanceStatusType.Active, new DateTime(2022, 1, 1),
                        DurationType.Continuous, MaintenanceRequirementType.Required,
                        MaintenanceStatusType.Maintained, OccurrenceType.Initial));

                _credentials.Add(cred1);
                _credentials.Add(cred2);
                _credentials.Add(cred3);
            }

            private void WhenICallEarnedNewSubspecialtyInitialCertEvents()
            {
                try
                {
                    _results = 
                        _credentials
                            .EarnedNewSubspecialtyInitialCertEvents(new DateTime(2011, 1, 1))
                            .ToList();
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBeNull();
            }

            private void AndIShouldGetTheFirstIssuanceDateOfTheTheCredentialThatHadAnInitialIssuanceAfterTheEffectiveDate()
            {
                //Longest. Method name. Ever.

                _results.Count.ShouldBe(1);
                _results[0].ShouldBeEquivalentTo(new DateTime(2012, 1, 1));
            }
        }

        #endregion Scenarios
    }
}
