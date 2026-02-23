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
using Abim.Platform.Program.Tests.Setup.Responses;
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
    public class YearEndLookbackCertStatusSpec
    {
        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        public void Should_Update_Issuances_When_Requirements_Not_Met()
        {
            new ShouldUpdateIssuancesWhenRequirementsNotMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        public void Should_Update_Credential_To_Inactive_When_Requirements_Not_Met()
        {
            new ShouldUpdateCredentialToInactiveWhenRequirementsNotMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        public void Should_Keep_Issuance_As_Active_When_Exam_Assessment_Requirement_Met()
        {
            new ShouldKeepIssuanceAsActiveWhenExamAssessmentRequirementMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        public void Should_Expire_Issuance_When_Points_Req_Not_Met()
        {
            new ShouldExpireIssuanceWhenPointsReqNotMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        public void Should_Expire_Issuance_When_Attestation_Req_Not_Met()
        {
            new ShouldExpireIssuanceWhenAttestationReqNotMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        public void Should_Keep_Issuance_As_Active_When_Exam_GracePeriod_Requirement_Met()
        {
            new ShouldKeepIssuanceAsActiveWhenExamGracePeriodRequirementMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        public void Should_Keep_Issuance_As_Active_When_Exam_PendingResults_Requirement_Met()
        {
            new ShouldKeepIssuanceAsActiveWhenExamPendingResultsRequirementMet().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        public void Should_Update_Continuous_Issuance_As_Expired_But_Keep_Cred_As_Active_When_Lifetime_Issuance_Exists()
        {
            new ShouldUpdateContinuousIssuanceAsExpiredButKeepCredAsActiveWhenLifetimeIssuanceExists().BDDfy();
        }

        [Test]
        [WorkItem(135089)]
        [WorkItem(134116)]
        [WorkItem(144627)]
        public void Should_Keep_Issuance_As_Active_Even_WhenNotEnough_Points_At_LookbackDate_But_Have_NewIssuance_After()
        {
            new ShouldKeepIssuanceAsActiveEvenWhenNotEnoughPointsAtLookbackDateButHaveNewIssuanceAfter().BDDfy();
        }

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
        public void Should_Keep_Issuance_As_Active_Even_WhenNotEnough_Points_At_LookbackDate_In_2021 ()
        {
            new ShouldKeepIssuanceAsActiveEvenWhenNotEnoughPointsAtLookbackDateIn2021().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void Should_Keep_Issuance_As_Active_Even_WhenNotEnough_Points_At_LookbackDate_In_2022_Covid4()
        {
            new ShouldKeepIssuanceAsActiveEvenWhenNotEnoughPointsAtLookbackDateIn2022_Covid4().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void Should_Expire_Issuance_WhenNotEnough_Points_At_LookbackDate_In_2022_NotCovid4()
        {
            new ShouldExpireIssuanceWhenNotEnoughPointsAtLookbackDateIn2022_NotCovid4().BDDfy();
        }

        //===============================================================
        // pbi 179929 : Update Program Rule 12 Assessment Grace Period to Include LNG
        // The diplomate is enrolled in the Longitudinal Assessment  AND did not meet the annual LNG participation requirement 
        //      OR the diplomate received a result of FAIL on the summative assessment
        [Test]
        [WorkItem(179929)]
        public void Should_PlaceInGracePeriodWhenLkaParticipationIsNotMetFromCurrentlyMeetingParticipationFlag()
        {
            new ShouldAddGracePeriodWhenLkaParticipationIsNotMet_CurrentlyMeetingParticipationFlag().BDDfy();
        }

        [Test]
        [WorkItem(179929)]
        public void Should_PlaceInGracePeriodWhenLkaParticipationIsNotMetFromEnrollmentStatus()
        {
            new ShouldAddGracePeriodWhenLkaParticipationIsNotMet_EnrollmentStatus().BDDfy();
        }

        [Test]
        [WorkItem(179929)]
        public void Should_PlaceInGracePeriodWhenFailedSummativeAssessmentFromSummativeDecisionFlag()
        {
            new ShouldAddGracePeriodWhenFailedSummativeAssessment_SummativeDecisionFlag().BDDfy();
        }

        [Test]
        [WorkItem(179929)]
        public void Should_PlaceInGracePeriodWhenFailedSummativeAssessmentFromEnrollmentStatus()
        {
            new ShouldAddGracePeriodWhenFailedSummativeAssessment_EnrollmentStatus().BDDfy();
        }

        #region Scenarios
        private abstract class CertStatusSpec : ProgramRulesServiceSimplifiedScenario
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
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1,24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate: issuanceDate));
                //_creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
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

            protected void GivenThatIHaveParameters()
            {
                _memberId = Guid.NewGuid();
                _processingDate = new DateTime( _lookbackDate.Year + 1 , 2, 1);
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

        private class ShouldUpdateIssuancesWhenRequirementsNotMet : CertStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(2);
                for (int x = 0; x < 2; x++)
                {
                    _creds.Add(CredentialBuilder.Build(abimSource));
                    DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                    _creds[x].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                    _creds[x].ApplyChangesAfterCreatingCredential(true, new DateTime(_lookbackDate.Year, 1, 1), new DateTime(_lookbackDate.Year, 12, 31), null, null, null, null, false, null);
                }
            }

            private void AndTheIssuanceShouldBeUpdatedWithExpiredValues()
            {
                for (int x = 0; x < 2; x++)
                {
                    _creds[x].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Expired);
                    _creds[x].Issuances[0].ExpirationDate.Should().Be(_lookbackDate);
                    _creds[0].Issuances[0].ExpiredDate.Should().Be(_lookbackDate);
                    _creds[x].Issuances[0].MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                    _creds[x].Issuances[0].AuditData.ModifiedBy.Should().Be("CertStatus");
                    _creds[x].Issuances[0].AuditData.Modified.HasValue.Should().BeTrue();
                }
            }
        }

        private class ShouldUpdateCredentialToInactiveWhenRequirementsNotMet : CertStatusSpec
        {
            private void AndTheCredentialShouldBeUpdatedWithInactiveValues()
            {
                _creds[0].IsActive.Should().BeFalse();

                //NOPE -- will be overwritten by main update to cred
                //_creds[0].AuditData.ModifiedBy.Should().Be("CertStatus");
            }
        }

        private class ShouldKeepIssuanceAsActiveWhenExamAssessmentRequirementMet : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesForCreateCredential(
                    _lookbackDate, 
                    new DateTime(DateTime.Now.Year + 2, 12, 31), 
                    null, 
                    abimSource, 
                    MaintenanceStatusType.Maintained, 
                    DateTime.Now.AddYears(1), 
                    null, 
                    null, 
                    null, 
                    false, 
                    "Unit Test");

                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
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

            protected void AndTheIssuanceShouldRemainAsItWas()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be(_startingIssuanceModifiedBy);
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldExpireIssuanceWhenPointsReqNotMet : CertStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);

                _creds.Add(
                    CredentialBuilder.BuildWithoutRandoms(
                        abimSource, 
                        "SomeCertCode", 
                        "SomeCertName", 
                        CertificationType.Subspecialty, 
                        CredentialType.Subspecialty,
                        Resources.PathwayType.MOC));

                _creds[0].AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        abimSource, 
                        IssuanceStatusType.Active, 
                        DateTime.Now.AddYears(-11), 
                        DurationType.Timelimited, 
                        MaintenanceRequirementType.Required, 
                        MaintenanceStatusType.Maintained, 
                        OccurrenceType.Recertification));

                _creds[0].Issuances[0].ExpirationDate = DateTime.Now.AddYears(-3);

                /*
                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous));
                _creds[0].ApplyChangesForCreateCredential(
                    _lookbackDate,
                    new DateTime(DateTime.Now.Year + 2, 12, 31),
                    null,
                    abimSource,
                    MaintenanceStatusType.Maintained,
                    DateTime.Now.AddYears(1), 
                    null, 
                    null, 
                    null, 
                    false, 
                    "Unit Test");

                //Changes to override random values provided by builder which 
                //could cause test to fail intermittently
                _creds[0].Type = CredentialType.General;
                _creds[0].Pathway = PathwayType.MOC;
                */
            }

            protected void AndTheIssuanceShouldBeExpired()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Expired);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be("CertStatus");
                _creds[0].Issuances[0].ExpiredDate.Should().Be(_lookbackDate);
            }

            protected void AndTheCredentialShouldNotBeActive()
            {
                _creds[0].IsActive.Should().BeFalse();
            }
        }

        private class ShouldExpireIssuanceWhenAttestationReqNotMet : CertStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddYears(-(new Random()).Next(6, 20)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].Certification = CertificationBuilder.Build(abimSource, "ICARD");
                _creds[0].ApplyChangesForCreateCredential(
                    _lookbackDate,
                    new DateTime(DateTime.Now.Year + 2, 12, 31),
                    null,
                    abimSource,
                    MaintenanceStatusType.Maintained,
                    DateTime.Now.AddYears(1), 
                    null, 
                    null, 
                    null, 
                    false,
                    null);
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();
                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(1);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddYears(-2))
                        .WithTotalMOCPoints(200)
                        .WithProduct(new ProductResource { Code = "FPHM" }) //Not ICARD, which is what we're looking for
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));
            }

            protected void AndTheIssuanceShouldBeExpired()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Expired);
                _creds[0].Issuances[0].ExpirationDate.Should().Be(_lookbackDate);
                _creds[0].Issuances[0].ExpiredDate.Should().Be(_lookbackDate);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be("CertStatus");
            }

            protected void AndTheCredentialShouldNotBeActive()
            {
                _creds[0].IsActive.Should().BeFalse();
            }
        }

        private class ShouldKeepIssuanceAsActiveWhenExamGracePeriodRequirementMet : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, new DateTime(_lookbackDate.Year, 1, 1), new DateTime(_lookbackDate.Year + 1, 12, 31), null, null, null, null, false, null);

                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
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

            protected void AndTheIssuanceShouldRemainAsItWas()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                // can be modified by other process ...
                //_creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be(_startingIssuanceModifiedBy);
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldKeepIssuanceAsActiveWhenExamPendingResultsRequirementMet : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
                _creds[0].AssessmentMet = true;
                _creds[0].AssessmentMetDate = new DateTime(DateTime.Now.Year - 1, 1, 1);
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

            protected void AndTheIssuanceShouldRemainAsItWas()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                // can be modified by other process 
                //_creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be(_startingIssuanceModifiedBy);
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldUpdateContinuousIssuanceAsExpiredButKeepCredAsActiveWhenLifetimeIssuanceExists : CertStatusSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceStatusType.Maintained, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, new DateTime(_lookbackDate.Year,1,1) , new DateTime(_lookbackDate.Year, 12, 31), null, null, null, null, false, null);
                issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Lifetime, MaintenanceStatusType.NotMaintained, issuanceDate));
            }

            protected override void SetupCredentialServiceMock()
            {
                base.SetupCredentialServiceMock();
                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>())).Returns(new DateTime(2001, 1, 1));
            }

            private void AndTheContinuousIssuanceShouldBeUpdatedWithExpiredValues()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Expired);
                _creds[0].Issuances[0].ExpirationDate.Should().Be(_lookbackDate);
                _creds[0].Issuances[0].ExpiredDate.Should().Be(_lookbackDate);
                _creds[0].Issuances[0].MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be("CertStatus");
                _creds[0].Issuances[0].AuditData.Modified.HasValue.Should().BeTrue();
            }

            private void AndTheLifetimeIssuanceShouldStillBeActive()
            {
                _creds[0].Issuances[1].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].Issuances[1].ExpirationDate.Should().Be(null);
                _creds[0].Issuances[1].ExpiredDate.Should().Be(null);
                _creds[0].Issuances[1].MaintenanceStatus.Should().Be(MaintenanceStatusType.NotMaintained);
                _creds[0].Issuances[1].AuditData.ModifiedBy.Should().BeNull();
                _creds[0].Issuances[1].AuditData.Modified.Should().NotHaveValue();
            }

            private void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
                _creds[0].AuditData.ModifiedBy.Should().Be("YearEnd");
                _creds[0].AuditData.Modified.Should().HaveValue();
            }
        }

        private class ShouldKeepIssuanceAsActiveEvenWhenNotEnoughPointsAtLookbackDateButHaveNewIssuanceAfter : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);

                var cred = CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine",
                    CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);
                cred.IsActive = true;

                _creds.Add(cred);

                // first issuance
                DateTime issuanceDate = _lookbackDate.AddYears(-10); // make sure it is before _lookbackDate
                var issuance1 = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Expired, issuanceDate, DurationType.Timelimited,
                            MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial);
                _creds[0].AddIssuance(issuance1);

                // second issuance after look back date
                DateTime issuanceDateAfterLookBack = _lookbackDate.AddDays(2); // make sure it is after _lookbackDate
                var issuance2 = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, issuanceDateAfterLookBack, DurationType.Continuous,
                            MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial);
                _creds[0].AddIssuance(issuance2);


                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();

                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(2);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithProduct(product)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(80)
                        .Build());

                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithProduct(product)
                        .WithCompletedDate((_lookbackDate.AddDays(1)))
                        .WithTotalMOCPoints(20)
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected void AndTheRecentIssuanceShouldRemainAsItWas()
            {
                _creds[0].Issuances[1].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].Issuances[1].AuditData.ModifiedBy.Should().Be(_startingIssuanceModifiedBy);
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldKeepIssuanceAsActiveEvenWhenNotEnoughPointsAtLookbackDateIn2021 : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {

                _lookbackDate = new DateTime(2021, 12, 31); // !!!! It is exception in 2021 year

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);

                var cred = CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine",
                    CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);
                cred.IsActive = true;

                _creds.Add(cred);

                // first issuance
                DateTime issuanceDate = _lookbackDate.AddYears(-10); // make sure it is before _lookbackDate
                var issuance1 = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, issuanceDate, DurationType.Timelimited,
                            MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial);

                issuance1.ExpirationDate = _lookbackDate; // expires in 2021

                _creds[0].AddIssuance(issuance1);

                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();

                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(2);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithProduct(product)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(80) //!!! only 80 out of 100
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected void AndTheRecentIssuanceShouldRemainAsItWasBefore()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be(_startingIssuanceModifiedBy);
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }

            protected void AndTheCredentialShouldUpdatedLookBackDate()
            {
                _creds[0].LookbackDate.Should().Be(_lookbackDate);
                _creds[0].AuditData.ModifiedBy.Should().Be("YearEnd");
            }

        }

        private class ShouldKeepIssuanceAsActiveEvenWhenNotEnoughPointsAtLookbackDateIn2022_Covid4 : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {

                _lookbackDate = new DateTime(2022, 12, 31); // !!!! It is exception in 2022 year

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);

                var cred = CredentialBuilder.BuildWithoutRandoms(abimSource, ProgramResourceConstants.CertificationCode.InfectiousDisease , "Infectious Disease", // !!!
                    CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);
                cred.IsActive = true;

                _creds.Add(cred);

                // first issuance
                DateTime issuanceDate = _lookbackDate.AddYears(-10); // make sure it is before _lookbackDate
                var issuance1 = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, issuanceDate, DurationType.Timelimited,
                            MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial);

                issuance1.ExpirationDate = _lookbackDate; // expires in 2022

                _creds[0].AddIssuance(issuance1);

                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();

                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(2);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithProduct(product)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(80) //!!! only 80 out of 100
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected void AndTheRecentIssuanceShouldRemainAsItWasBefore()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be(_startingIssuanceModifiedBy);
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }

            protected void AndTheCredentialShouldUpdatedLookBackDate()
            {
                _creds[0].LookbackDate.Should().Be(_lookbackDate);
                _creds[0].AuditData.ModifiedBy.Should().Be("YearEnd");
            }

        }

        private class ShouldExpireIssuanceWhenNotEnoughPointsAtLookbackDateIn2022_NotCovid4 : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {

                _lookbackDate = new DateTime(2022, 12, 31); // !!!! It is exception in 2022 year

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);

                var cred = CredentialBuilder.BuildWithoutRandoms(abimSource, ProgramResourceConstants.CertificationCode.GeriatricMedicine, "Geriatric Medicine", // !!!
                    CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);
                cred.IsActive = true;

                _creds.Add(cred);

                // first issuance
                DateTime issuanceDate = _lookbackDate.AddYears(-10); // make sure it is before _lookbackDate
                var issuance1 = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, issuanceDate, DurationType.Timelimited,
                            MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial);

                issuance1.ExpirationDate = _lookbackDate; // expires in 2022

                _creds[0].AddIssuance(issuance1);

                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
            }

            protected override void SetupProductInterserviceMock()
            {
                var activites = new ActivityFullCollectionResource();

                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                activites.Data = new List<ActivityResource>(2);
                activites.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithProduct(product)
                        .WithCompletedDate(_lookbackDate.AddYears(-1))
                        .WithTotalMOCPoints(80) //!!! only 80 out of 100
                        .Build());

                base.SetupProductInterserviceMock();
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activites));

            }

            protected void AndTheRecentIssuanceShouldExpired()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Expired);
                _creds[0].Issuances[0].AuditData.ModifiedBy.Should().Be("CertStatus");
            }

            protected void AndTheCredentialShouldNotBeActive()
            {
                _creds[0].IsActive.Should().BeFalse();
            }

            protected void AndTheCredentialShouldUpdatedLookBackDate()
            {
                _creds[0].LookbackDate.Should().Be(_lookbackDate);
                _creds[0].AuditData.ModifiedBy.Should().Be("YearEnd");
            }

        }

        private class ShouldAddGracePeriodWhenLkaParticipationIsNotMet_CurrentlyMeetingParticipationFlag : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, null, null, new DateTime(_lookbackDate.Year, 12, 31), null, null, null, false, null);
                _creds[0].Issuances[0].MaintenanceStatus = MaintenanceStatusType.Maintained;
                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
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
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));

                // currently enrolled and failed to meet participation 
                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();
                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = _creds[0].Certification.ExternalId;
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = false;

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));
            }

            protected void AndTheIssuanceShouldHaveGracePeriod()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].GracePeriodStartDate.Should().Be(new DateTime(_lookbackDate.Year+1,1,1));
                _creds[0].GracePeriodEndDate.Should().Be(new DateTime(_lookbackDate.Year+1, 12, 31));
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldAddGracePeriodWhenLkaParticipationIsNotMet_EnrollmentStatus : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, null, null, new DateTime(_lookbackDate.Year, 12, 31), null, null, null, false, null);
                _creds[0].Issuances[0].MaintenanceStatus = MaintenanceStatusType.Maintained;
                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
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
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));

                // currently enrolled and failed to meet participation 
                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();
                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = _creds[0].Certification.ExternalId;
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = true; // just in case
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Suspended);
                lngSummaryResource.SuspensionReason = new RegistrationEnumValueResponseResource<SuspensionReasonType>(SuspensionReasonType.FailedToMeetParticipation);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));
            }

            protected void AndTheIssuanceShouldHaveGracePeriod()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].GracePeriodStartDate.Should().Be(new DateTime(_lookbackDate.Year + 1, 1, 1));
                _creds[0].GracePeriodEndDate.Should().Be(new DateTime(_lookbackDate.Year + 1, 12, 31));
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldAddGracePeriodWhenFailedSummativeAssessment_SummativeDecisionFlag : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, null, null, new DateTime(_lookbackDate.Year, 12, 31), null, null, null, false, null);
                _creds[0].Issuances[0].MaintenanceStatus = MaintenanceStatusType.Maintained;
                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
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
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));

                // currently enrolled and failed Summative Decision
                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();
                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = _creds[0].Certification.ExternalId;
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = false;
                lngSummaryResource.LongitudinalParticipations = new List<LongitudinalParticipationSummaryResource>();
                lngSummaryResource.LongitudinalParticipations.Add(new LongitudinalParticipationSummaryResource());
                lngSummaryResource.LongitudinalParticipations[0].SummativeDecision = new RegistrationEnumValueResponseResource<SummativeDecisionType>(SummativeDecisionType.Fail) ;

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));
            }

            protected void AndTheIssuanceShouldHaveGracePeriod()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].GracePeriodStartDate.Should().Be(new DateTime(_lookbackDate.Year + 1, 1, 1));
                _creds[0].GracePeriodEndDate.Should().Be(new DateTime(_lookbackDate.Year + 1, 12, 31));
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }

        private class ShouldAddGracePeriodWhenFailedSummativeAssessment_EnrollmentStatus : CertStatusSpec
        {
            private string _startingIssuanceModifiedBy;

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, null, null, new DateTime(_lookbackDate.Year, 12, 31), null, null, null, false, null);
                _creds[0].Issuances[0].MaintenanceStatus = MaintenanceStatusType.Maintained;
                _startingIssuanceModifiedBy = _creds[0].Issuances[0].AuditData.ModifiedBy;
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
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));

                // currently enrolled and failed Summative Decision
                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();
                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = _creds[0].Certification.ExternalId;
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = false;
                lngSummaryResource.LongitudinalParticipations = new List<LongitudinalParticipationSummaryResource>();
                lngSummaryResource.LongitudinalParticipations.Add(new LongitudinalParticipationSummaryResource());
                lngSummaryResource.LongitudinalParticipations[0].SummativeDecision = new RegistrationEnumValueResponseResource<SummativeDecisionType>(SummativeDecisionType.NoDecision);

                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Suspended);
                lngSummaryResource.SuspensionReason = new RegistrationEnumValueResponseResource<SuspensionReasonType>(SuspensionReasonType.FailedSummativeDecision);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));
            }

            protected void AndTheIssuanceShouldHaveGracePeriod()
            {
                _creds[0].Issuances[0].IssuanceStatus.Should().Be(IssuanceStatusType.Active);
                _creds[0].GracePeriodStartDate.Should().Be(new DateTime(_lookbackDate.Year + 1, 1, 1));
                _creds[0].GracePeriodEndDate.Should().Be(new DateTime(_lookbackDate.Year + 1, 12, 31));
            }

            protected void AndTheCredentialShouldStillBeActive()
            {
                _creds[0].IsActive.Should().BeTrue();
            }
        }
        #endregion Scenarios
    }
}
