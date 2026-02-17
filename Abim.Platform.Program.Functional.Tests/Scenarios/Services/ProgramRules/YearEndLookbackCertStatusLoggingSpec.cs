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
    public class YearEndLookbackCertStatusLoggingSpec
    {
        [Test]
        [WorkItem(137216)]
        public void Should_Log_When_Credential_Is_Not_Active_And_Five_Year_Req_Not_Met()
        {
            new ShouldLogWhenCredentialIsNotActiveAndFiveYearReqNotMet().BDDfy();
        }

        [Test]
        [WorkItem(137216)]
        public void Should_Log_When_Credential_Is_Not_Active_And_Assessment_Req_Not_Met()
        {
            new ShouldLogWhenCredentialIsNotActiveAndAsseementReqNotMet().BDDfy();
        }

        [Test]
        [WorkItem(137216)]
        public void Should_Log_When_Credential_Is_Not_Active_And_Attestation_Req_Not_Met()
        {
            new ShouldLogWhenCredentialIsNotActiveAndAttestationReqNotMet().BDDfy();
        }

        [Test]
        [WorkItem(137216)]
        public void Should_Log_When_Credential_Is_Active_But_Potentially_Going_Into_Grace_And_Assessment_Req_Not_Met()
        {
            new ShouldLogWhenCredentialIsActiveButPotentiallyGoingIntoGraceAndAssessmentReqIsMet().BDDfy();
        }

        #region Scenarios
        private abstract class CertStatusLoggingSpec : ProgramRulesServiceSimplifiedScenario
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
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 15, 2, 3), DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));
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
                //_lookbackDate is already set
                _processingDate = new DateTime(_lookbackDate.Year + 1, 2, 1);
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

        private class ShouldLogWhenCredentialIsNotActiveAndFiveYearReqNotMet : CertStatusLoggingSpec
        {
            private void AndItShouldHaveLoggedThatTheFiveYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddCertificationLookbackLog>(
                        cmd => 
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.FiveYear
                        && cmd.IsPendingAction == false
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "CertYearly"
                        )), Times.Once);
            }
        }

        private class ShouldLogWhenCredentialIsNotActiveAndAsseementReqNotMet : CertStatusLoggingSpec
        {
            private void AndItShouldHaveLoggedThatTheAssessmentReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddCertificationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Assessment
                        && cmd.IsPendingAction == false
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "CertYearly"
                        )), Times.Once);
            }
        }

        private class ShouldLogWhenCredentialIsNotActiveAndAttestationReqNotMet : CertStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                base.SetupCredentials();
                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.Name = "Interventional Cardiology";
            }

            private void AndItShouldHaveLoggedThatTheAssessmentReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddCertificationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Attestation
                        && cmd.IsPendingAction == false
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "CertYearly"
                        )), Times.Once);
            }
        }

        private class ShouldLogWhenCredentialIsActiveButPotentiallyGoingIntoGraceAndAssessmentReqIsMet
             : CertStatusLoggingSpec
        {
            /*
            This one gets a bit involved. I didn't think this scenario was possible, but when I
            reached out to Don he replied:

            "If the person was tl expiring, met all the other requirements other than the exam 
            and they have a pending result, the cert status should leave them active and then the 
            potential grace period would be true."

            So basically, someone with a TL cert could meet the non-exam requirements, and have 
            pending exam results which satisfies PBI 135089 (Meets Exam Requirements). Their 
            credential would still be active, and if their latest issuance meets all the criteria 
            they could potentially be going into the grace period.
            */

            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate;
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
                _creds[0].AssessmentMet = true;
                _creds[0].AssessmentMetDate = _lookbackDate.AddDays(-5);
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                //Give this diplomate some pending exam results
                var regResourceBuilder = new RegistrationResourceBuilder();
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();

                registrations.Registrations = new List<RegistrationResource>(1);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                registrations.Registrations.Add(
                    regResourceBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Kci)
                        .WithAdministrationYear(2016)
                        .WithExamResult(ExamResultType.Pending)
                        .WithResult("Pending")
                        .Build());

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            protected override void SetupProductInterserviceMock()
            {
                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(200)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheAssessmentReqWasNotMet()
            {
                //Yes, logging that the req was NOT met, due to them possibly going into
                //the grace period. That's what the acceptance criteria states.

                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddCertificationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Assessment
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "CertYearly"
                        )), Times.Once);
            }
        }

        #endregion
    }
}
