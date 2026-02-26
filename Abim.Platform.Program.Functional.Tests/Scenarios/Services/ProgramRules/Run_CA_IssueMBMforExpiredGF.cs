using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
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
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;


namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
     AsA = "controller, background job, or bus consumer",
     IWant = "to be able to utilize the RunCorrectiveAction function in Program Rules service",
     SoThat = "it can correctly process credential records"
     )]
    [TestFixture]
    public class Run_CA_IssueMBMforExpiredGF
    {
        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_100Points()
        {
            new Run_CA_IssueMBMforExpiredGF_100Points_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_Reciprocity()
        {
            new Run_CA_IssueMBMforExpiredGF_Reciprocity_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_NewSubspecialtyInitial()
        {
            new Run_CA_IssueMBMforExpiredGF_NewSubspecialtyInitial_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_RecentlyInitiallyCertified()
        {
            new Run_CA_IssueMBMforExpiredGF_RecentlyInitiallyCertified_MeetRules().BDDfy();
        }


        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_Attestation()
        {
            new Run_CA_IssueMBMforExpiredGF_Attestation_MeetRules().BDDfy();
        }


        // ---Don't MeetRules ----

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_Less100points()
        {
            new Run_CA_IssueMBMforExpiredGF_Less100Points_Dont_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_NO_Reciprocity()
        {
            new Run_CA_IssueMBMforExpiredGF_NO_Reciprocity_Dont_MeetRules().BDDfy();
        }
        //----
        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_NO_NewSubspecialtyInitial()
        {
            new Run_CA_IssueMBMforExpiredGF_NO_NewSubspecialtyInitial_Dont_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_NO_RecentlyInitiallyCertified()
        {
            new Run_CA_IssueMBMforExpiredGF_NO_RecentlyInitiallyCertified_Dont_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_NO_Attestation()
        {
            new Run_CA_IssueMBMforExpiredGF_NO_Attestation_Dont_MeetRules().BDDfy();
        }

        [TestCase]
        public void Run_CA_IssueMBMforExpiredGF_NO_Exam()
        {
            new Run_CA_IssueMBMforExpiredGF_NO_Exam_Dont_MeetRules().BDDfy();
        }

        /// <summary>
        /// Base class for this file
        /// </summary>
        /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
        public abstract class RunCAIssueMBMforExpiredGFScenario : ProgramRulesServiceScenario
        {
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

                // ++++++++++++ Credential Service ++++++++++++
                My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReissueCommand>()))
                    .Returns(new ReissueCommandResult(CommandStatus.Accepted, null, null));

                My<ICredentialService>().Setup(p => p.SearchByMemberId(It.IsAny<Guid>()))
                    .Returns(AllCredentials);

                // ***  AccessTokenServiceMock ---
                My<IAccessTokenService>()
                   .Setup(o => o.GetAccessToken())
                   .Returns("--token--");

                // ++++++++++++ Log ++++++++++++
                ((ProgramRulesService)ProgramRulesService).Log = Log.Object;
                LogTest.Watch(Log);

            }
        }

        #region Scenarios 

        public class Run_CA_IssueMBMforExpiredGF_100Points_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 100);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
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

                Set_CurrentLookBackDatesInfo();
            }

            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Once);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_Reciprocity_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;

                DateTime ActivityCompletedDate = new DateTime(2017, 01, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                //--- ReciprocityAttest
                Set_Reciprocity(ActivityCompletedDate);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
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

                Set_CurrentLookBackDatesInfo();
            }

           
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Once);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_NewSubspecialtyInitial_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
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

                Set_CurrentLookBackDatesInfo();
            }

            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Once);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_Attestation_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InterventionalCardiology, ///
                                                    issuanceStatus: IssuanceStatusType.Expired,
                                                    occurrenceType: OccurrenceType.Initial,
                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                    expirationDate: new DateTime(2018, 12, 31),
                                                    assessmentMet: true,
                                                    assessmentMetDate: null,
                                                    actions: null);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: true);

                Set_CurrentLookBackDatesInfo();
            }
          
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Once);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_RecentlyInitiallyCertified_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 11, 02); // --- fixed date !!!!
                FirstIssuanceDate = new DateTime(2014, 11, 01); // Recently initially certified (5 years window)
                TriggeringEvent = TriggeringEvent.ActivityCompleted;

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                    issuanceStatus: IssuanceStatusType.Expired,
                                                    occurrenceType: OccurrenceType.Initial,
                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                    expirationDate: new DateTime(2018, 12, 31),
                                                    assessmentMet: true,
                                                    assessmentMetDate: null,
                                                    actions: null);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: true);

                Set_CurrentLookBackDatesInfo();
            }

            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Once);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_Less100Points_Dont_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
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

                Set_CurrentLookBackDatesInfo();
            }

            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_NO_Reciprocity_Dont_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;
                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                Set_Reciprocity(ActivityCompletedDate.AddYears(-2));


                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
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

                Set_CurrentLookBackDatesInfo();
            }

           
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_NO_NewSubspecialtyInitial_Dont_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;
                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                // Earned NewSubspecialty Initial Cert ( too old)
                Set_NewSubspecialtyInitialCert(issuanceDate: ProcessingDate.AddYears(-5),
                                                category: CredentialCategoryType.MustBeMaintained,
                                                certificationCode: "GERI");

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
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

                Set_CurrentLookBackDatesInfo();
            }

          
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_NO_RecentlyInitiallyCertified_Dont_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;
                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                    issuanceStatus: IssuanceStatusType.Expired,
                                                    occurrenceType: OccurrenceType.Initial,
                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                    expirationDate: EventDate.AddMonths(-3),
                                                    assessmentMet: true,
                                                    assessmentMetDate: null,
                                                    actions: null);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                Set_CurrentLookBackDatesInfo();
            }

            
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_NO_Attestation_Dont_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;
                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InterventionalCardiology,
                                                    issuanceStatus: IssuanceStatusType.Expired,
                                                    occurrenceType: OccurrenceType.Initial,
                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                    expirationDate: EventDate.AddMonths(-3),
                                                    assessmentMet: true,
                                                    assessmentMetDate: null,
                                                    actions: null);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                Set_CurrentLookBackDatesInfo();
            }

           
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
            }

        }

        public class Run_CA_IssueMBMforExpiredGF_NO_Exam_Dont_MeetRules : RunCAIssueMBMforExpiredGFScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                Initialize();
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);
                TriggeringEvent = TriggeringEvent.ActivityCompleted;
                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                                    issuanceDate: FirstIssuanceDate,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                    issuanceStatus: IssuanceStatusType.Expired,
                                                    occurrenceType: OccurrenceType.Initial,
                                                    maintenanceStatus: MaintenanceStatusType.Maintained,
                                                    expirationDate: EventDate.AddMonths(-3),
                                                    assessmentMet: true,
                                                    assessmentMetDate: null,
                                                    actions: null);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    mocExamResult: ExamResultType.Fail);


                Set_CurrentLookBackDatesInfo();
            }

          
            public void WhenICallRunCorrectiveAction()
            {
                try
                {
                    Result = ProgramRulesService.RunCorrectiveAction(
                                    credentials: InputCredentials,
                                    memberId: new Guid(),
                                    eventDate: EventDate,
                                    processingDate: ProcessingDate,
                                    triggeringEvent: TriggeringEvent,
                                    registration: null).Result;
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

            public void AndThenHandleCommand_SHOULD_BeCalled()
            {
                My<ICredentialService>()
                    .Verify(o => o.Handle(It.IsAny<ReissueCommand>()), Times.Never);
            }

        }
        #endregion
    }
}
