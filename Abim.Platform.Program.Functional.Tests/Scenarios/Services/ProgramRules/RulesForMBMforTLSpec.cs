using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Abim.Platform.Program.Tests.Setup.Responses;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the IssueNewCredentialForTLPC_ functions in Program Rules service",
      SoThat = "it can correctly issue MBM issuance record if meet requirements"
      )]
    [TestFixture]
    public class RulesForMBMforTLSpec
    {
        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_100Points()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_100Points().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_Reciprocity()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_Reciprocity().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_NewSubspecialtyInitial()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_NewSubspecialtyInitial().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_Attestation()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_Attestation().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_RecentlyInitiallyCertified()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_RecentlyInitiallyCertified().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_Less100Points()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_Less100Points().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_OldReciprocity()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_OldReciprocity().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_OldNewSubspecialtyInitial()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_OldNewSubspecialtyInitial().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_NoRecentlyInitiallyCertified()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_NoRecentlyInitiallyCertified().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_NoAttestation()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_NoAttestation().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_AssessmentNotMet()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_AssessmentNotMet().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_FailExam()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_FailExam().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_PassMOC()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_PassMOC().BDDfy();
        }

        // --- test cases related to PBI 173158 - SR261851 - 196059 Cardio Cert Did Not Issue in 2019 ---
        // Two KCI exams withing the same Administration
        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_PassKCIAfterFailedKCI_IssueMBM()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_PassKCIAfterFailedKCI_IssueMBM().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_FailedKCI_DontIssueMBM()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_FailedKCI_DontIssueMBM().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_FailedKciAndFailedCmpLater_DontIssueMBM()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_FailedKciAndFailedCmpLater_DontIssueMBM().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_MeetParticipationLka_IssueMBM()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_MeetParticipationLka_IssueMBM().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_RunCorrectiveAction_NoNoconcequencesExams_IssueMBM()
        {
            new RulesForMBMforTLSpec_RunCorrectiveAction_NoNoconcequencesExams_IssueMBM().BDDfy();
        }

        //**** Handle commmand side ***
        [TestCase]
        public void RulesForMBMforTLSpec_HandleCommand_100Points()
        {
            new RulesForMBMforTLSpec_HandleCommand_100Points().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_HandleCommand_ActiveGF()
        {
            new RulesForMBMforTLSpec_HandleCommand_ActiveGF().BDDfy();
        }

        [TestCase]
        public void RulesForMBMforTLSpec_HandleCommand_ExpiredGF()
        {
            new RulesForMBMforTLSpec_HandleCommand_ExpiredGF().BDDfy();
        }
    }

    public abstract class RulesForMBMforTLSpecScenario : ProgramRulesServiceScenario
    {
        protected RunRulesForMustBeMaintainedCertificateCommand Command;


        protected override void PreSetup()
        {
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            //// ++++++++++++ Credential Service ++++++++++++
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                  .Returns(FirstIssuanceDate);

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivityFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(CurrentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(RegistrationFullCollectionResource));

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(AllRegistrations));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource() { Data = LongitudinalEnrollments }   ));

            // ++++++++++++ Credential Service ++++++++++++
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReissueCommand>()))
                .Returns(new ReissueCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(AllCredentials);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
                .Returns(InputCredential);

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            // ++++++++++++ Log ++++++++++++
            ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
            LogTest.Watch(Log);

        }
    }

    #region Corrective Action Scenarios

    public class RulesForMBMforTLSpec_RunCorrectiveAction_100Points : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31), 
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_Reciprocity : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;
            DateTime ActivityCompletedDate = new DateTime(2017, 01, 01);


            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m); //!!!!

            //--- ReciprocityAttest
            Set_Reciprocity(ActivityCompletedDate);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_NewSubspecialtyInitial : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            // Earned NewSubspecialty Initial Cert
            Set_NewSubspecialtyInitialCert(issuanceDate: new DateTime(2017, 11, 01),
                                            category: CredentialCategoryType.MustBeMaintained,
                                            certificationCode: "GERI");

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_Attestation : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            //--- Attestation
            UserActivities.Add(ActivityResourceDataBuilder
                                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                                ProductCode: ProductResourceConstants.ProductCode.ICARDAttestMOC)
                                            .Build());
            ActivityFullCollectionResource.Data.AddRange(UserActivities);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InterventionalCardiology,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_RecentlyInitiallyCertified : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 11, 02); // --- fixed date !!!!
            FirstIssuanceDate = new DateTime(2014, 11, 01); // Recently initially certified (5 years window)
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_Less100Points : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_OldReciprocity : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            // old Reciprocity
            DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);
            Set_Reciprocity(ActivityCompletedDate.AddYears(-2));

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_OldNewSubspecialtyInitial : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            // Earned NewSubspecialty Initial Cert ( too old)
            Set_NewSubspecialtyInitialCert(issuanceDate: ProcessingDate.AddYears(-5),
                                            category: CredentialCategoryType.MustBeMaintained,
                                            certificationCode: "GERI");

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_NoRecentlyInitiallyCertified : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 99.9m);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_NoAttestation : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InterventionalCardiology,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_AssessmentNotMet : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_FailExam : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate,
                                mocExamResult: ExamResultType.Fail);
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_PassMOC : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate,
                                withMOC: true);
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    // --- test cases related to PBI 173158 - SR261851 - 196059 Cardio Cert Did Not Issue in 2019 ---
    // Two KCI exams withing the same Administration
    public class RulesForMBMforTLSpec_RunCorrectiveAction_PassKCIAfterFailedKCI_IssueMBM : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate,
                                withMOC: true);

            // bad KCI with UTT result
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(2017, 11, 02),
                        seatDate: new DateTime(2017, 09, 02),
                        examResult: ExamResultType.UnableToTest,
                        examType: ExamType.Kci,
                        actions : new List<Action<RegistrationResource>>()
                        {
                           (a => a.NoConsequence = true)
                        });
            // following KCI with pass result
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(2017, 11, 02),
                        seatDate: new DateTime(2017, 10, 02),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci,
                        actions: new List<Action<RegistrationResource>>()
                        {
                           (a => a.NoConsequence = true)
                        });
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_FailedKCI_DontIssueMBM : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            // bad KCI with UTT result
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(2017, 11, 02),
                        seatDate: new DateTime(2017, 09, 02),
                        examResult: ExamResultType.Fail,
                        examType: ExamType.Kci,
                        actions: new List<Action<RegistrationResource>>()
                        {
                           (a => a.NoConsequence = true)
                        });
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_FailedKciAndFailedCmpLater_DontIssueMBM : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            // bad KCI with fail result
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(2017, 11, 02),
                        seatDate: new DateTime(2017, 09, 02),
                        examResult: ExamResultType.Fail,
                        examType: ExamType.Kci,
                        actions: new List<Action<RegistrationResource>>()
                        {
                           (a => a.NoConsequence = true)
                        });

            // following cmp with pass result
            Set_Sut_CmpRegistration(credential: credential,
                        testDate: new DateTime(2018, 09, 02),
                        examResult: ExamResultType.Fail); // !!!
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_MeetParticipationLka_IssueMBM : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            // bad KCI with UTT result
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(2017, 11, 02),
                        seatDate: new DateTime(2017, 09, 02),
                        examResult: ExamResultType.Fail,
                        examType: ExamType.Kci,
                        actions: new List<Action<RegistrationResource>>()
                        {
                           (a => a.NoConsequence = true)
                        });


            LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

            lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
            lngSummaryResource.Assessment.CertificationId = credential.Certification.ExternalId;
            lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Active); // !!!
            lngSummaryResource.IsActive = true;
            lngSummaryResource.LongitudinalParticipations = new List<LongitudinalParticipationSummaryResource>();
            lngSummaryResource.LongitudinalParticipations.Add(new LongitudinalParticipationSummaryResource()
            {
                Cycle = 1,
                PathwayYear = 1,
                Status = new RegistrationEnumValueResponseResource<ParticipationStatusType>(ParticipationStatusType.Met)
            });

            LongitudinalEnrollments.Add(lngSummaryResource);

        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BE_Called()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_RunCorrectiveAction_NoNoconcequencesExams_IssueMBM : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            // credential that would meet "MBMforTL" conditions
            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired, // The certificate status is Active or Expired(eg.not revoked / surrendered / suspended)
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2017, 12, 31),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate,
                                withMOC: true);
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentials: InputCredentials,
                                memberId: MemberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate).Result;
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
            Result.Should().Be(true);
        }

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    #endregion

    #region Handle Command Scenarios
    public class RulesForMBMforTLSpec_HandleCommand_100Points : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: true,
                                                assessmentMetDate: null,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = InputCredential.ExternalId,
                IssuanceId = InputIssuance.Id,
                EventDate = EventDate,
                ProcessingDate = ProcessingDate,
                CreatedBy = "Test" // per PBI
            };
        }

        public void WhenICallHandle()
        {
            try
            {
                ProgramRulesService.HandleJob(Command);
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

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }

    public class RulesForMBMforTLSpec_HandleCommand_ActiveGF : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: true,
                                                assessmentMetDate: null,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = InputCredential.ExternalId,
                IssuanceId = InputIssuance.Id,
                EventDate = EventDate,
                ProcessingDate = ProcessingDate,
                CreatedBy = "Test" // per PBI
            };
        }

        public void WhenICallHandle()
        {
            try
            {
                ProgramRulesService.HandleJob(Command);
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

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }

    }

    public class RulesForMBMforTLSpec_HandleCommand_ExpiredGF : RulesForMBMforTLSpecScenario
    {
        protected override void PreSetup()
        {
            base.PreSetup();

            // set Main data >>>>>
            EventDate = new DateTime(2018, 01, 13);
            ProcessingDate = new DateTime(2018, 12, 13);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: true,
                                                assessmentMetDate: null,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            //Set_CurrentLookBackDatesInfo();
        }

        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = InputCredential.ExternalId,
                IssuanceId = InputIssuance.Id,
                EventDate = EventDate,
                ProcessingDate = ProcessingDate,
                CreatedBy = "Test" // per PBI
            };
        }

        public void WhenICallHandle()
        {
            try
            {
                ProgramRulesService.HandleJob(Command);
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

        public void AndThenHandle_ExpireAndReissueCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }

    }
    #endregion

}
