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
        SoThat = "it can run the year end lookback process and log appropriately"
    )]
    [TestFixture]
    public class YearEndLookbackParticipationStatusLoggingSpec
    {
        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAnActive_MbmCredential_ThatMeets_TheTwoYearRequirement()
        {
            new ShouldNotLogForAnActiveMbmCredentialThatMeetsTheTwoYearRequirement().BDDfy();
        }

        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAnActive_MbmCredential_ThatDoesNotMeet_TheTwoYearRequirement()
        {
            new ShouldLogForAnActiveMbmCredentialThatDoesNotMeetTheTwoYearRequirement().BDDfy();
        }

        [Test]
        [WorkItem(137824)]
        public void Should_Log_WhenCredentialIsNotActive_AndIsMbm()
        {
            new ShouldLogWhenCredentialIsNotActiveAndIsMbm().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_Log_WhenCredentialIsTimeLimited_AndIsNotActive()
        {
            new ShouldLogWhenCredentialIsTimeLimitedAndIsNotActive().BDDfy();
        }

        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAnActiveTimeLimitedCredential_ThatDoesNotMeet_TheTwoYearRequirement()
        {
            new ShouldLogForAnActiveTimeLimitedCredentialThatDoesNotMeetTheTwoYearRequirement().BDDfy();
        }

        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAnActiveTimeLimitedCredential_ThatDoesNotMeet_TheTwoYearRequirement()
        {
            new ShouldNotLogForAnActiveTimeLimitedCredentialThatDoesMeetTheTwoYearRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAnActiveTimeLimitedCredential_ThatDoesNotMeet_TheFiveYearRequirement()
        {
            new ShouldLogForAnActiveTimeLimitedCredentialThatDoesNotMeetTheFiveYearRequirement().BDDfy();
        }

        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAnActiveTimeLimitedCredential_ThatDoesMeet_TheFiveYearRequirement()
        {
            new ShouldNotLogForAnActiveTimeLimitedCredentialThatDoesMeetTheFiveYearRequirement().BDDfy();
        }

        //Commenting out for YELB.  Test premised on invalid logic, will need to re-add
        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAnActiveTimeLimitedCredential_ThatDoesNotMeet_TheAttestationRequirement()
        {
            new ShouldLogForAnActiveTimeLimitedCredentialThatDoesNotMeetTheAttestationRequirement().BDDfy();
        }

        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAnActiveTimeLimitedCredential_ThatDoesMeet_TheAttestationRequirement()
        {
            new ShouldNotLogForAnActiveTimeLimitedCredentialThatDoesMeetTheAttestationRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAGrandfatherCredential_ThatDoesNotMeet_TheTwoYearRequirement()
        {
            new ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheTwoYearRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAGrandfatherCredential_ThatDoesMeet_TheTwoYearRequirement()
        {
            new ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheTwoYearRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAGrandfatherCredential_ThatDoesNotMeet_TheFiveYearRequirement()
        {
            new ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheFiveYearRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAGrandfatherCredential_ThatDoesMeet_TheFiveYearRequirement()
        {
            new ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheFiveYearRequirement().BDDfy();
        }

        //Commenting out for YELB.  Test premised on invalid logic, will need to re-add
        //[Test]
        //[WorkItem(137824)]
        //public void Should_Log_ForAGrandfatherCredential_ThatDoesNotMeet_TheAttestationRequirement()
        //{
        //    new ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheAttestationRequirement().BDDfy();
        //}

        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAGrandfatherCredential_ThatDoesMeet_TheAttestationRequirement()
        {
            new ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheAttestationRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_Log_ForAGrandfatherCredential_ThatDoesNotMeet_TheAssessmentRequirement()
        {
            new ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheAssessmentRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(137824)]
        public void Should_NotLog_ForAGrandfatherCredential_ThatDoesMeet_TheAssessmentRequirement()
        {
            new ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheAssessmentRequirement().BDDfy();
        }


        private abstract class ParticipationStatusLoggingSpec : ProgramRulesServiceSimplifiedScenario
        {
            protected List<Credential> _creds;
            protected Guid _memberId;
            // need to check if lookback is in 2020 then we need to go back to 2019 since we have exception rules in 2020 (run YELB in 2021)
            protected DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);
            protected DateTime _processingDate;
            protected DateTime FirstIssuanceDate;

            protected virtual void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine",
                    CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active,
                    new DateTime(DateTime.Now.Year - 15, 2, 3), DurationType.Continuous,
                    MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained,
                    OccurrenceType.Recertification));
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2),
                    null, null, null, null, false, null);
            }

            protected override void SetupCredentialServiceMock()
            {
                SetupCredentials();

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);

                _credSvcMock.Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());
                
                _credSvcMock.Setup(x => x.Handle(It.IsAny<ReissueCommand>()))
                    .Returns(new ReissueCommandResult());
                FirstIssuanceDate = new DateTime(_lookbackDate.Year - 5, 2, 21);
                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(FirstIssuanceDate);
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
                _correctiveActionResultSvcMock =
                    new Mock<App.Services.ICorrectiveActionResultService>(MockBehavior.Strict);
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
                    .Setup(
                        x =>
                            x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
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
                _processingDate = new DateTime(_lookbackDate.Year + 1, 2, 2);
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

        private class ShouldNotLogForAnActiveMbmCredentialThatMeetsTheTwoYearRequirement
            : ParticipationStatusLoggingSpec
        {
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

            private void AndItShouldNotHaveLoggedAParticipationStatusFailure()
            {
                _lookbackLogSvcMock.Verify(o => o.Handle(It.IsAny<AddParticipationLookbackLog>()), Times.Never());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private class ShouldLogForAnActiveMbmCredentialThatDoesNotMeetTheTwoYearRequirement
           : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                FirstIssuanceDate = new DateTime(DateTime.Now.Year - 5, 2, 21);
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
                var product = new ProductResource();
                product.Code = "erwre2342342";//can't do Reciprocity


                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);

                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddYears(-3))
                        .WithTotalMOCPoints(100)
                        .WithProduct(product)
                        .Build());

                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheTwoYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.TwoYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }
        
        private class ShouldLogWhenCredentialIsNotActiveAndIsMbm : ParticipationStatusLoggingSpec
        {
            private void AndItShouldHaveLoggedThatTheCertificationReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Certification
                        && cmd.IsPendingAction == false
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }

        private class ShouldLogWhenCredentialIsTimeLimitedAndIsNotActive : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Expired, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));
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
            private void AndItShouldHaveLoggedThatTheCertificationReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Certification
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }


        private class ShouldLogForAnActiveTimeLimitedCredentialThatDoesNotMeetTheTwoYearRequirement
           : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                //FirstIssuanceDate = new DateTime(DateTime.Now.Year - 5, 2, 21);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification));
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate;
                _creds[0].ApplyChangesAfterCreatingCredential(true, new DateTime(_lookbackDate.Year, 1, 1), new DateTime(_lookbackDate.Year + 1, 12, 31), null, null, null, null, false, null);
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
                        .WithAdministrationYear(2019)
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);

                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddYears(-3))
                        .WithTotalMOCPoints(100)
                        .WithProduct(product)
                        .Build());

                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheTwoYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.TwoYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }


        private class ShouldNotLogForAnActiveTimeLimitedCredentialThatDoesMeetTheTwoYearRequirement
         : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification));
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(100)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheTwoYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.TwoYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }

        private class ShouldLogForAnActiveTimeLimitedCredentialThatDoesNotMeetTheFiveYearRequirement
        : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                FirstIssuanceDate = new DateTime(_lookbackDate.AddYears(-6).Year, 2, 21);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, FirstIssuanceDate, DurationType.Timelimited,MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained,OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2); 
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheFiveYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.FiveYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }
        
        private class ShouldNotLogForAnActiveTimeLimitedCredentialThatDoesMeetTheFiveYearRequirement: ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification));
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(100)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheFiveYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.FiveYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }

        private class ShouldLogForAnActiveTimeLimitedCredentialThatDoesNotMeetTheAttestationRequirement: ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(2013, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheAttestationReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Attestation
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }
        
        private class ShouldNotLogForAnActiveTimeLimitedCredentialThatDoesMeetTheAttestationRequirement : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(2013, 2, 3), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
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
                var product = new ProductResource();
                product.Code = "ICARDAttestMOC";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheAttestationReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Attestation
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }
        
        private class ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheTwoYearRequirement: ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification));
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheTwoYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.TwoYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }

        private class ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheTwoYearRequirement: ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification));
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(100)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheTwoYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.TwoYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }

        private class ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheFiveYearRequirement: ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                FirstIssuanceDate = new DateTime(_lookbackDate.AddYears(-6).Year, 2, 21);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, FirstIssuanceDate, DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheFiveYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.FiveYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }
        
        private class ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheFiveYearRequirement: ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification));
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(100)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheFiveYearReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.FiveYear
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }

        private class ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheAttestationRequirement : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
                _creds[0].AssessmentMet = true;
                _creds[0].AssessmentMetDate = _lookbackDate.AddDays(-5);
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                //Give this diplomate some pending exam results
                var regResourceBuilder = new RegistrationResourceBuilder();
                var registrations = new RegistrationFullCollectionResource();
                registrations.Data = new List<RegistrationResource>(1);
                registrations.Data.Add(
                    regResourceBuilder
                        .WithCertificationId(_creds[0].Certification.ExternalId)
                        .WithExamType(ExamType.Kci)
                        .WithAdministrationYear(2016)
                        .WithExamResult(ExamResultType.Pending)
                        .WithResult("Pending")
                        .Build());

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));
            }

            protected override void SetupProductInterserviceMock()
            {
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheAttestationReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Attestation
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }

        private class ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheAttestationRequirement : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
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
                var product = new ProductResource();
                product.Code = "ICARDAttestMOC";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheAttestationReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Attestation
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }

        private class ShouldLogForAGrandfatherCredentialThatDoesNotMeetTheAssessmentRequirement : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
                _creds[0].AssessmentMet = false;
                _creds[0].AssessmentMetDate = _lookbackDate.AddYears(-5);
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldHaveLoggedThatTheAssessmentReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Assessment
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Once);
            }
        }
        
        private class ShouldNotLogForAGrandfatherCredentialThatDoesMeetTheAssessmentRequirement : ParticipationStatusLoggingSpec
        {
            protected override void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC));
                var issuance = IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 5, 2, 3), DurationType.Lifetime, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.NotMaintained, OccurrenceType.Recertification);
                _creds[0].AddIssuance(issuance);
                _creds[0].Issuances[0].ExpirationDate = _lookbackDate.AddYears(2);
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
                _creds[0].AssessmentMet = true;
                _creds[0].AssessmentMetDate = _lookbackDate.AddYears(-5);
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
                var product = new ProductResource();
                product.Code = "erwre2342342";

                var builder = new ActivityResourceBuilder();
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(1);
                activities.Data.Add(
                    builder
                        .WithActivityResult(ActivityResultType.Pass)
                        .WithCompletedDate(_lookbackDate.AddDays(-5))
                        .WithTotalMOCPoints(0)
                        .WithProduct(product)
                        .Build());

                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            private void AndItShouldNotHaveLoggedThatTheAssessmentReqWasNotMet()
            {
                _lookbackLogSvcMock.Verify(x => x.Handle(
                    It.Is<AddParticipationLookbackLog>(
                        cmd =>
                        cmd.CredentialId == _creds[0].ExternalId
                        && cmd.Action == LookbackActionType.FailurePoint
                        && cmd.Reason == LookbackReasonType.Assessment
                        && cmd.IsPendingAction == true
                        && cmd.LookbackLogDate == _lookbackDate
                        && cmd.UserName == "ParticipationYearly"
                        )), Times.Never);
            }
        }
    }
}
