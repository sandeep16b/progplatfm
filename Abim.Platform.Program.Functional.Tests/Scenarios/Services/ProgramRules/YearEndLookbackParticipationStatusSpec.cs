using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the Program Rules service",
        SoThat = "it can run the year end lookback process"
    )]
    [TestFixture]
    public class YearEndLookbackParticipationStatusSpec
    {
        //--- GF certs
        [Test]
        [WorkItem(134117)]
        [WorkItem(135236)]
        [WorkItem(135239)]
        public void GF_Should_Update_Issuances_When_Requirements_Not_Met()
        {
            new GF_ShouldUpdateIssuancesWhenRequirementsNotMet().BDDfy();
        }

        [Test]
        [WorkItem(134117)]
        [WorkItem(135236)]
        [WorkItem(135239)]
        public void GF_Should_NOT_Update_Issuances_When_Requirements_Met()
        {
            new GF_Shoul_NOT_UpdateIssuancesWhenRequirementsMet().BDDfy();
        }

        //--- TL certs ---
        [Test]
        [WorkItem(134117)]
        [WorkItem(135236)]
        [WorkItem(135239)]
        public void TL_Should_Update_Issuances_When_Requirements_Not_Met()
        {
            new TL_ShouldUpdateIssuancesWhenRequirementsNotMet().BDDfy();
        }

        [Test]
        [WorkItem(134117)]
        [WorkItem(135236)]
        [WorkItem(135239)]
        public void TL_Should_NOT_Update_Issuances_When_Requirements_Met()
        {
            new TL_Shoul_NOT_UpdateIssuancesWhenRequirementsMet().BDDfy();
        }

        //--- MBM certs ---
        [Test]
        [WorkItem(134117)]
        [WorkItem(135236)]
        [WorkItem(135239)]
        public void MBM_Should_Update_Issuances_When_Requirements_Not_Met()
        {
            new MBM_ShouldUpdateIssuancesWhenRequirementsNotMet().BDDfy();
        }

        [Test]
        [WorkItem(134117)]
        [WorkItem(135236)]
        [WorkItem(135239)]
        public void MBM_Should_NOT_Update_Issuances_When_Requirements_Met()
        {
            new MBM_Shoul_NOT_UpdateIssuancesWhenRequirementsMet().BDDfy();
        }

        //--- All certs ---

        /* 
        Pbi 216370 : Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
        For 2020, 2021, and 2022, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
        *** Diplomate does not meet an MOC assessment requirement that is due in 2020 or 2021 or 2022
        *** Diplomate does not meet an MOC attestation requirement that is due in 2020 or 2021 or 2022
        *** Diplomate does not meet the two or five year point requirement due in 2020 or 2021 or 2022
        ============================================================================================================================================================================================
        Pbi 208254 : (Release 2.35) Certification & Participation Status Changes for 2020 and 2021 MOC Requirements (Not COVID 4)
          For 2020 and 2021, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
          *** Diplomate does not meet an MOC assessment requirement that is due in 2020 or 2021
          *** Diplomate does not meet an MOC attestation requirement that is due in 2020 or 2021
          *** Diplomate does not meet the two or five year point requirement due in 2020 or 2021
        ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        */

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void Should_NOT_Update_Issuances_In_2021_YearOnly()
        {
            new Should_NOT_Update_Issuances_In_2021_YearOnlySpec().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void Should_NotUpdate_Issuances_In_2022_Covid4()
        {
            new Should_NOT_Update_Issuances_In_2022_Covid4Spec().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void Should_Update_Issuances_In_2022_NotCovid4()
        {
            new Should_Update_Issuances_In_2022_NotCovid4Spec().BDDfy();
        }

        #region Scenarios
        private abstract class ParticipationStatusSpec : ProgramRulesServiceSimplifiedScenario
        {
            protected List<Credential> _creds;
            protected Guid _memberId;
            // need to check if lookback is in 2020 then we need to go back to 2019 since we have exception rules in 2020 (run YELB in 2021)
            protected DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);
            protected DateTime _processingDate;

            protected virtual void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            protected override void SetupCredentialServiceMock()
            {
                SetupCredentials();

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);

                _credSvcMock.Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                //Should this be set like this? Maybe we don't want a reissue to occur 
                //if we're verifying certain scenarios
                _credSvcMock.Setup(x => x.Handle(It.IsAny<ReissueCommand>()))
                    .Returns(new ReissueCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
            }

            protected override void SetupCorrectiveActionResultServiceMock()
            {
                _correctiveActionResultSvcMock = new Mock<App.Services.ICorrectiveActionResultService>(MockBehavior.Strict);
                _correctiveActionResultSvcMock
                    .Setup(x => x.Add(It.IsAny<CorrectiveActionResult>())).Returns(Task.FromResult(true));
                _correctiveActionResultSvcMock
                    .Setup(x => x.Add(It.IsAny<IEnumerable<CorrectiveActionResult>>())).Returns(Task.FromResult(true));
            }

            protected override void SetupProductInterserviceMock()
            {
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(0);
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            protected  virtual void GivenThatIHaveParameters()
            {
                _memberId = Guid.NewGuid();
                // need to check if lookback is in 2020 then we need to go back to 2019 since we have exception rules in 2020 (run YELB in 2021)
                _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);
                _processingDate = DateTime.Now;
            }

            protected async void WhenICallTheRunLookbackMethod()
            {
                try
                {
                    await _sut.RunYearEndLookback(_memberId, _lookbackDate, _processingDate);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }
        }

        private class GF_ShouldUpdateIssuancesWhenRequirementsNotMet : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // GF issuance 
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));
                // TL issuance
                issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited,MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            private void AndThen_GF_IssuanceShouldBeUpdatedWithNotMaintainedValues()
            {
                var GFIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Lifetime && x.IssuanceStatus==IssuanceStatusType.Active).FirstOrDefault();

                GFIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                GFIssuance.AuditData.ModifiedBy.Should().Be("ClearMaint");
                GFIssuance.AuditData.Modified.HasValue.Should().BeTrue();
            }

            private void AndThen_TL_IssuanceShould_BeUpdatedWithNotMaintainedValues()
            {
                var TLIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Timelimited && x.IssuanceStatus == IssuanceStatusType.Active).FirstOrDefault();

                TLIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                TLIssuance.AuditData.ModifiedBy.Should().Be("ClearMaint");
                TLIssuance.AuditData.Modified.HasValue.Should().BeTrue();
            }
        }

        private class GF_Shoul_NOT_UpdateIssuancesWhenRequirementsMet : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // GF issuance 
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));
                // TL issuance
                issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, new DateTime(_lookbackDate.Year, 1, 1), new DateTime(_lookbackDate.Year + 1, 12, 31), null, null, null, null, false, null);
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(100)
                        .Build());

                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(DateTime.Now.AddMonths(-2))
                        .WithTotalMOCPoints(1)
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Moc)
                        .WithResult("Pending")
                        .Build());
                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            private void AndThen_GF_IssuanceShould_NOT_BeUpdated()
            {
                var GFIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Lifetime && x.IssuanceStatus == IssuanceStatusType.Active).FirstOrDefault();

                GFIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.Maintained); // stay maintained 
                GFIssuance.AuditData.ModifiedBy.Should().BeNull();
            }

            private void AndThen_TL_IssuanceShould_NOT_BeUpdated()
            {
                var TLIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Timelimited ).FirstOrDefault();

                TLIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.Maintained); // stay maintained 
                TLIssuance.AuditData.ModifiedBy.Should().BeNull();
            }
        }

        private class TL_ShouldUpdateIssuancesWhenRequirementsNotMet : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(
                    abimSource, 
                    "SomeCode", 
                    "SomeName", 
                    CertificationType.General, 
                    CredentialType.General,
                    Resources.PathwayType.MOC));

                // TL issuance
                _creds[0].AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        abimSource, 
                        IssuanceStatusType.Active, 
                        DateTime.Now.AddYears(-15), 
                        DurationType.Timelimited, 
                        MaintenanceRequirementType.NotRequired, 
                        MaintenanceStatusType.Maintained, 
                        OccurrenceType.Recertification));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            private void AndThen_TL_IssuanceShould_BeUpdatedWithNotMaintainedValues()
            {
                var TLIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Timelimited && x.IssuanceStatus == IssuanceStatusType.Active).FirstOrDefault();

                TLIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                TLIssuance.AuditData.ModifiedBy.Should().Be("ClearMaint");
                TLIssuance.AuditData.Modified.HasValue.Should().BeTrue();
            }
        }

        private class TL_Shoul_NOT_UpdateIssuancesWhenRequirementsMet : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // TL issuance
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(100)
                        .Build());

                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(DateTime.Now.AddMonths(-2))
                        .WithTotalMOCPoints(1)
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Moc)
                        .WithResult("Pending")
                        .Build());
                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            private void AndThen_TL_IssuanceShould_NOT_BeUpdated()
            {
                var TLIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Timelimited && x.IssuanceStatus == IssuanceStatusType.Active).FirstOrDefault();

                TLIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.Maintained); // stay maintained
                TLIssuance.AuditData.ModifiedBy.Should().BeNull();
            }
        }

        private class MBM_ShouldUpdateIssuancesWhenRequirementsNotMet : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));

                // MBM issuance
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            private void AndThen_TL_IssuanceShould_BeUpdatedWithNotMaintainedValues()
            {
                var MBMIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Continuous).FirstOrDefault();

                MBMIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                MBMIssuance.AuditData.ModifiedBy.Should().NotBeNull();
                MBMIssuance.AuditData.Modified.HasValue.Should().BeTrue();
            }
        }

        private class MBM_Shoul_NOT_UpdateIssuancesWhenRequirementsMet : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // MBM issuance
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(100)
                        .Build());

                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(DateTime.Now.AddMonths(-2))
                        .WithTotalMOCPoints(1)
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Moc)
                        .WithResult("Pending")
                        .Build());
                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            private void AndThen_TL_IssuanceShould_NOT_BeUpdated()
            {
                var MBMIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Continuous).FirstOrDefault();

                MBMIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.Maintained); // stay maintained
                MBMIssuance.AuditData.ModifiedBy.Should().BeNull();
            }
        }

        private class Should_NOT_Update_Issuances_In_2021_YearOnlySpec : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {

                _lookbackDate = new DateTime(2021, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // MBM issuance
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Moc)
                        .WithResult("Pending")
                        .Build());
                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            protected override void GivenThatIHaveParameters()
            {
                _memberId = Guid.NewGuid();
                _processingDate = DateTime.Now;
            }

            private void AndThen_MB_IssuanceShould_NOT_BeUpdated()
            {
                var MBMIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Continuous).FirstOrDefault();

                MBMIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.Maintained); // stay maintained
                MBMIssuance.AuditData.ModifiedBy.Should().BeNull(); // was not upated
            }
        }

        private class Should_NOT_Update_Issuances_In_2022_Covid4Spec : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {

                _lookbackDate = new DateTime(2022, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // MBM issuance
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);

                _creds[0].Certification.Code = ProgramResourceConstants.CertificationCode.InfectiousDisease; // Covid4
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Moc)
                        .WithResult("Pending")
                        .Build());
                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            protected override void GivenThatIHaveParameters()
            {
                _memberId = Guid.NewGuid();
                _processingDate = DateTime.Now;
            }

            private void AndThen_MB_IssuanceShould_NOT_BeUpdated()
            {
                var MBMIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Continuous).FirstOrDefault();

                MBMIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.Maintained); // stay maintained
                MBMIssuance.AuditData.ModifiedBy.Should().BeNull(); // was not upated
            }
        }

        private class Should_Update_Issuances_In_2022_NotCovid4Spec : ParticipationStatusSpec
        {
            protected override void SetupCredentials()
            {

                _lookbackDate = new DateTime(2022, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);

                _creds.Add(CredentialBuilder.Build(abimSource));
                // MBM issuance
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);

                _creds[0].Certification.Code = ProgramResourceConstants.CertificationCode.GeriatricMedicine;
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Moc)
                        .WithResult("Pending")
                        .Build());
                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            protected override void GivenThatIHaveParameters()
            {
                _memberId = Guid.NewGuid();
                _processingDate = DateTime.Now;
            }

            private void AndThen_MB_IssuanceShould_BeUpdated()
            {
                var MBMIssuance = _creds[0].Issuances.Where(x => x.Duration == DurationType.Continuous).FirstOrDefault();

                MBMIssuance.MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained); 
                MBMIssuance.AuditData.ModifiedBy.Should().Be("ClearMaint");
            }
        }
        #endregion Scenarios
    }
}
