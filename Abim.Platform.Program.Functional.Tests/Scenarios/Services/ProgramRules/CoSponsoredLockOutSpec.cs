using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Setup.ResourceDataBuilders;
using Abim.Platform.Program.Tests.Setup.Responses;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
  AsA = "controller, background job, or bus consumer",
  IWant = "to be able to utilize the Program Rules service",
  SoThat = "it can run the Cosponsored Lock Out process"
  )]
    [TestFixture]
    public class CoSponsoredLockOutSpec
    {

        [Test]
        [WorkItem(223712)]
        [WorkItem(288091)]
        public void Should_Properly_Update_Credential_LockOutDate_WhenNotInDueYear_LockOut_Process()
        {
            new ShouldProperlyUpdateCredentialLockOutDateWenNotInDueYearLockOutProcessScenario().BDDfy();
        }

        [Test]
        [WorkItem(223713)]
        [WorkItem(288091)]
        public void Should_Clear_LockOut_Period_WhenExpired_During_LockOut_Process()
        {
            new ShouldClearLockOutPeriodWhenExpiredScenario().BDDfy();
        }

        [Test]
        [WorkItem(223711)]
        public void Should_Set_LockOut_Period_WhenEnrolledInLka_AndFailedParticipation_During_LockOut_Process()
        {
            new ShouldSetLockOutPeriodWhenEnrolledInLkaAndFailedParticipationScenario().BDDfy();
        }

        [Test]
        [WorkItem(223711)]
        public void Should_Set_LockOut_Period_WhenEnrolledInLka_AndFailedSummativeDecision_During_LockOut_Process()
        {
            new ShouldSetLockOutPeriodWhenEnrolledInLkaAndFailedSummativeDecisionScenario().BDDfy();
        }

        [Test]
        [WorkItem(223711)]
        public void Should_NOT_Set_LockOut_Period_WhenNoMocExamResults_AndNoEnrollmentInLka_During_LockOut_Process()
        {
            new ShouldNotSetLockOutPeriodWhenNoMocExamResultsAndNoEnrollmentInLkaScenario().BDDfy();
        }

        [Test]
        [WorkItem(223711)]
        public void Should_NOT_Set_LockOut_Period_WhenNoMocExamResults_AndEnrolledInLkaButDifferentCert_During_LockOut_Process()
        {
            new ShouldNotSetLockOutPeriodWhenNoMocExamResultsAndEnrolledInLkaButDifferentCertScenario().BDDfy();
        }
        #region Scenarios

        private class ShouldProperlyUpdateCredentialLockOutDateWenNotInDueYearLockOutProcessScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _credentialId;
            private DateTime _lockOutDate;
            private Credential _credential;
            private int _lockOutYear = 2022;
            // builders
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();

            protected RegistrationResourceDataBuilder RegistrationResourceDataBuilder { get; set; } = new RegistrationResourceDataBuilder();

            protected override void SetupCredentialServiceMock()
            {
                // *********  creating domain data  *********
                var source = SourceDataBuilder
                             .With(a => a.Code = "ABIM")
                             .Build();

                Certification certification = (new CertificationDataBuilder(source))
                                                .Build();

                ////Active TL issuance NotMaintained
                Issuance issuance = IssuanceDataBuilder
                                    .With(a => a.Source = source)
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Continuous)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .Build();

                _credential = (new CredentialDataBuilder(certification))
                            .With(a=>a.IsCosponsored=true)
                            .With(a=>a.ExamDueDate= DateTime.UtcNow) // ExamDueDate is not in lookback year (2022)
                            .Build();

                _credential.AddIssuance(issuance);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_credential);
            }

            protected override void SetupRegistrationInterserviceMock()
            {

                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                // registration MOC 
                registrations.
                        Registrations.Add(RegistrationResourceDataBuilder
                                            .With(a => a.AdministrationYear = 2020)
                                            .With(a => a.CertificationId = _credential.Certification.ExternalId)
                                            .With(a => a.AdministrationDate = new DateTime(2020, 11, 02))
                                            .With(a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) })
                                            .With(a => a.Result = ExamResultType.Fail.ToString())
                                            .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc))
                                            .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = new DateTime(2020, 11, 02) } })
                                            .With(a => a.Id = Guid.NewGuid())
                                            .With(a => a.AdministrationId = Guid.NewGuid())
                                            .Build());

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));


                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();
                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = _credential.Certification.ExternalId; // same Certification
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = true;
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Suspended);
                lngSummaryResource.SuspensionReason = new RegistrationEnumValueResponseResource<SuspensionReasonType>(SuspensionReasonType.FailedToMeetParticipation);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveACredentialIdAndLockOutDate()
            {
                _credentialId = Guid.NewGuid();
                _lockOutDate = new DateTime(2022, 12, 31);
            }

            private async void WhenIRunTheCoSponosoredLockOutProcess()
            {
                await _sut.RunCoSponsoredLockOut(_credentialId, _lockOutDate, new DateTime(2023, 2, 1));
            }
            private void ThenVerifyUpdateCredentialFromLookBackCommand()
            {
                _credSvcMock.Verify(x =>
                        x.Handle(It.Is<UpdateCredentialFromLookbackCommand>(
                                cmd => cmd.Credential.LookbackDate.Value == _credential.LookbackDate &&
                                cmd.ModifiedBy == "CoSponsoredLockOut_" + _lockOutDate.ToString("yyyy-MM-dd") &&
                                cmd.Credential.GracePeriodStartDate == new DateTime(_lockOutYear + 1, 1, 1) &&
                                cmd.Credential.GracePeriodEndDate == new DateTime(_lockOutYear + 1, 12, 31))), Times.Once);
            }
        }

        private class ShouldClearLockOutPeriodWhenExpiredScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _credentialId;
            private DateTime _lockOutDate;
            private int _lockOutYear = 2022;
            private Credential _credential;
            // builders
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();

            protected RegistrationResourceDataBuilder RegistrationResourceDataBuilder { get; set; } = new RegistrationResourceDataBuilder();

            protected override void SetupCredentialServiceMock()
            {
                // *********  creating domain data  *********
                var source = SourceDataBuilder
                             .With(a => a.Code = "ABIM")
                             .Build();

                Certification certification = (new CertificationDataBuilder(source))
                                                .Build();

                ////Active TL issuance NotMaintained
                Issuance issuance = IssuanceDataBuilder
                                    .With(a => a.Source = source)
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Continuous)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .Build();

                _credential = (new CredentialDataBuilder(certification))
                            .With(a => a.IsCosponsored = true)
                            .With(a => a.ExamDueDate = DateTime.UtcNow) // ExamDueDate is not in lookback year (2022)
                            .Build();

                _credential.GracePeriodStartDate = new DateTime(_lockOutYear, 1, 1);
                _credential.GracePeriodEndDate = new DateTime(_lockOutYear, 12, 31);

                _credential.AddIssuance(issuance);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_credential);
            }

            protected override void SetupRegistrationInterserviceMock()
            {

                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                // registration MOC 
                registrations.
                        Registrations.Add(RegistrationResourceDataBuilder
                                            .With(a => a.AdministrationYear = 2020)
                                            .With(a => a.CertificationId = _credential.Certification.ExternalId)
                                            .With(a => a.AdministrationDate = new DateTime(2020, 11, 02))
                                            .With(a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) })
                                            .With(a => a.Result = ExamResultType.Fail.ToString())
                                            .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc))
                                            .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = new DateTime(2020, 11, 02) } })
                                            .With(a => a.Id = Guid.NewGuid())
                                            .With(a => a.AdministrationId = Guid.NewGuid())
                                            .Build());

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));


                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();
                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = Guid.NewGuid(); // different Certification
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = true;
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Suspended);
                lngSummaryResource.SuspensionReason = new RegistrationEnumValueResponseResource<SuspensionReasonType>(SuspensionReasonType.FailedToMeetParticipation);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveACredentialIdAndLockOutDate()
            {
                _credentialId = Guid.NewGuid();
                _lockOutDate = new DateTime(_lockOutYear, 12, 31);
            }

            private async void WhenIRunTheCoSponosoredLockOutProcess()
            {
                await _sut.RunCoSponsoredLockOut(_credentialId, _lockOutDate, new DateTime(2023, 2, 1));
            }

            private void ThenVerifyUpdateCredentialFromLookBackCommand()
            {

                _credSvcMock.Verify(x =>
                        x.Handle(It.Is<UpdateCredentialFromLookbackCommand>(
                                cmd => cmd.Credential.LookbackDate.Value == _credential.LookbackDate &&
                                cmd.ModifiedBy == "CoSponsoredLockOut_" + _lockOutDate.ToString("yyyy-MM-dd") &&
                                cmd.Credential.GracePeriodStartDate == null &&
                                cmd.Credential.GracePeriodEndDate == null)), Times.Once);
            }
        }

        private class ShouldSetLockOutPeriodWhenEnrolledInLkaAndFailedParticipationScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _credentialId;
            private DateTime _lockOutDate;
            private int _lockOutYear = 2022;
            private Credential _credential;
            // builders
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();

            protected override void SetupCredentialServiceMock()
            {
                // *********  creating domain data  *********
                var source = SourceDataBuilder
                             .With(a => a.Code = "ABIM")
                             .Build();

                Certification certification = (new CertificationDataBuilder(source))
                                                .Build();

                ////Active TL issuance NotMaintained
                Issuance issuance = IssuanceDataBuilder
                                    .With(a => a.Source = source)
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Continuous)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .Build();

                _credential = (new CredentialDataBuilder(certification))
                            .With(a => a.IsCosponsored = true)
                            .Build();

                _credential.GracePeriodStartDate = null;
                _credential.GracePeriodEndDate = null;
                _credential.ExamDueDate = new DateTime(_lockOutYear, 12, 31);

                _credential.AddIssuance(issuance);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_credential);
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
                lngSummaryResource.Assessment.CertificationId = _credential.Certification.ExternalId;
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = true; // just in case
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Suspended);
                lngSummaryResource.SuspensionReason = new RegistrationEnumValueResponseResource<SuspensionReasonType>(SuspensionReasonType.FailedToMeetParticipation);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveACredentialIdAndLockOutDate()
            {
                _credentialId = Guid.NewGuid();
                _lockOutDate = new DateTime(_lockOutYear, 12, 31);
            }

            private async void WhenIRunTheCoSponosoredLockOutProcess()
            {
                await _sut.RunCoSponsoredLockOut(_credentialId, _lockOutDate, new DateTime(_lockOutYear + 1, 2, 1));
            }

            private void ThenVerifyUpdateCredentialFromLookBackCommand()
            {
                _credSvcMock.Verify(x =>
                        x.Handle(It.Is<UpdateCredentialFromLookbackCommand>(
                                cmd => cmd.Credential.LookbackDate.Value == _credential.LookbackDate &&
                                cmd.ModifiedBy == "CoSponsoredLockOut_" + _lockOutDate.ToString("yyyy-MM-dd") &&
                                cmd.Credential.GracePeriodStartDate == new DateTime(_lockOutYear + 1, 1, 1) &&
                                cmd.Credential.GracePeriodEndDate == new DateTime(_lockOutYear + 1, 12, 31))), Times.Once);
            }
        }

        private class ShouldSetLockOutPeriodWhenEnrolledInLkaAndFailedSummativeDecisionScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _credentialId;
            private DateTime _lockOutDate;
            private int _lockOutYear = 2022;
            private Credential _credential;
            // builders
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();

            protected override void SetupCredentialServiceMock()
            {
                // *********  creating domain data  *********
                var source = SourceDataBuilder
                             .With(a => a.Code = "ABIM")
                             .Build();

                Certification certification = (new CertificationDataBuilder(source))
                                                .Build();

                ////Active TL issuance NotMaintained
                Issuance issuance = IssuanceDataBuilder
                                    .With(a => a.Source = source)
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Continuous)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .Build();

                _credential = (new CredentialDataBuilder(certification))
                            .With(a => a.IsCosponsored = true)
                            .Build();

                _credential.GracePeriodStartDate = null;
                _credential.GracePeriodEndDate = null;
                _credential.ExamDueDate = new DateTime(_lockOutYear, 12, 31);

                _credential.AddIssuance(issuance);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_credential);
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
                lngSummaryResource.Assessment.CertificationId = _credential.Certification.ExternalId;
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = true;
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Active);
                lngSummaryResource.LongitudinalParticipations = new List<LongitudinalParticipationSummaryResource>();
                lngSummaryResource.LongitudinalParticipations.Add(new LongitudinalParticipationSummaryResource());
                lngSummaryResource.LongitudinalParticipations[0].SummativeDecision = new RegistrationEnumValueResponseResource<SummativeDecisionType>(SummativeDecisionType.Fail);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveACredentialIdAndLockOutDate()
            {
                _credentialId = Guid.NewGuid();
                _lockOutDate = new DateTime(_lockOutYear, 12, 31);
            }

            private async void WhenIRunTheCoSponosoredLockOutProcess()
            {
                await _sut.RunCoSponsoredLockOut(_credentialId, _lockOutDate, new DateTime(_lockOutYear + 1, 2, 1));
            }

            private void ThenVerifyUpdateCredentialFromLookBackCommand()
            {
                _credSvcMock.Verify(x =>
                        x.Handle(It.Is<UpdateCredentialFromLookbackCommand>(
                                cmd => cmd.Credential.LookbackDate.Value == _credential.LookbackDate &&
                                cmd.ModifiedBy == "CoSponsoredLockOut_" + _lockOutDate.ToString("yyyy-MM-dd") &&
                                cmd.Credential.GracePeriodStartDate == new DateTime(_lockOutYear + 1, 1, 1) &&
                                cmd.Credential.GracePeriodEndDate == new DateTime(_lockOutYear + 1, 12, 31))), Times.Once);
            }
        }

        private class ShouldNotSetLockOutPeriodWhenNoMocExamResultsAndNoEnrollmentInLkaScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _credentialId;
            private DateTime _lockOutDate;
            private int _lockOutYear = 2022;
            private Credential _credential;
            // builders
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();

            protected override void SetupCredentialServiceMock()
            {
                // *********  creating domain data  *********
                var source = SourceDataBuilder
                             .With(a => a.Code = "ABIM")
                             .Build();

                Certification certification = (new CertificationDataBuilder(source))
                                                .Build();

                ////Active TL issuance NotMaintained
                Issuance issuance = IssuanceDataBuilder
                                    .With(a => a.Source = source)
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Continuous)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .Build();

                _credential = (new CredentialDataBuilder(certification))
                            .With(a => a.IsCosponsored = true)
                            .Build();

                _credential.GracePeriodStartDate = null;
                _credential.GracePeriodEndDate = null;
                _credential.ExamDueDate = new DateTime(_lockOutYear, 12, 31);

                _credential.AddIssuance(issuance);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_credential);
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

                LongitudinalEnrollmentCollectionResource lngCollection = new LongitudinalEnrollmentCollectionResource();

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveACredentialIdAndLockOutDate()
            {
                _credentialId = Guid.NewGuid();
                _lockOutDate = new DateTime(_lockOutYear, 12, 31);
            }

            private async void WhenIRunTheCoSponosoredLockOutProcess()
            {
                await _sut.RunCoSponsoredLockOut(_credentialId, _lockOutDate, new DateTime(_lockOutYear + 1, 2, 1));
            }

            private void ThenVerifyUpdateCredentialFromLookBackCommand()
            {
                _credSvcMock.Verify(x =>
                        x.Handle(It.Is<UpdateCredentialFromLookbackCommand>(
                                cmd => cmd.Credential.LookbackDate.Value == _credential.LookbackDate &&
                                cmd.ModifiedBy == "CoSponsoredLockOut_" + _lockOutDate.ToString("yyyy-MM-dd") &&
                                cmd.Credential.GracePeriodStartDate == null &&
                                cmd.Credential.GracePeriodEndDate == null)), Times.Once);
            }
        }

        private class ShouldNotSetLockOutPeriodWhenNoMocExamResultsAndEnrolledInLkaButDifferentCertScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _credentialId;
            private DateTime _lockOutDate;
            private int _lockOutYear = 2022;
            private Credential _credential;
            // builders
            protected IssuanceDataBuilder IssuanceDataBuilder { get; set; } = new IssuanceDataBuilder();
            protected SourceDataBuilder SourceDataBuilder { get; set; } = new SourceDataBuilder();
            protected CertificationDataBuilder CertificationDataBuilder { get; set; } = new CertificationDataBuilder();
            protected CredentialDataBuilder CredentialDataBuilder { get; set; } = new CredentialDataBuilder();

            protected override void SetupCredentialServiceMock()
            {
                // *********  creating domain data  *********
                var source = SourceDataBuilder
                             .With(a => a.Code = "ABIM")
                             .Build();

                Certification certification = (new CertificationDataBuilder(source))
                                                .Build();

                ////Active TL issuance NotMaintained
                Issuance issuance = IssuanceDataBuilder
                                    .With(a => a.Source = source)
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Continuous)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .Build();

                _credential = (new CredentialDataBuilder(certification))
                            .With(a => a.IsCosponsored = true)
                            .Build();

                _credential.GracePeriodStartDate = null;
                _credential.GracePeriodEndDate = null;
                _credential.ExamDueDate = new DateTime(_lockOutYear, 12, 31);

                _credential.AddIssuance(issuance);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_credential);
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
                lngSummaryResource.Assessment.CertificationId = Guid.NewGuid(); // different cert
                lngSummaryResource.IsActive = true;
                lngSummaryResource.CurrentlyMeetingParticipation = true; 
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Suspended);
                lngSummaryResource.SuspensionReason = new RegistrationEnumValueResponseResource<SuspensionReasonType>(SuspensionReasonType.FailedToMeetParticipation);

                lngCollection.Data.Add(lngSummaryResource);

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(lngCollection));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveACredentialIdAndLockOutDate()
            {
                _credentialId = Guid.NewGuid();
                _lockOutDate = new DateTime(_lockOutYear, 12, 31);
            }

            private async void WhenIRunTheCoSponosoredLockOutProcess()
            {
                await _sut.RunCoSponsoredLockOut(_credentialId, _lockOutDate, new DateTime(_lockOutYear + 1, 2, 1));
            }

            private void ThenVerifyUpdateCredentialFromLookBackCommand()
            {
                _credSvcMock.Verify(x =>
                        x.Handle(It.Is<UpdateCredentialFromLookbackCommand>(
                                cmd => cmd.Credential.LookbackDate.Value == _credential.LookbackDate &&
                                cmd.ModifiedBy == "CoSponsoredLockOut_" + _lockOutDate.ToString("yyyy-MM-dd") &&
                                cmd.Credential.GracePeriodStartDate == null &&
                                cmd.Credential.GracePeriodEndDate == null)), Times.Once);
            }
        }
        #endregion


    }
}
