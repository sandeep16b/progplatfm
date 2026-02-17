using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Enums;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using PathwayType = Abim.Platform.Program.Resources.PathwayType;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest
{
    [Story(
      AsA = "bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to update credentials on loading kci exam results"
      )]
    [TestFixture]
    public class UpdateCredentialOnLoadingKCIExamResultOnlySpecNew
    {

        // PBI 151802 : Update Assessment Due Date After 2nd KCI Pass ( Formerly Rule 30 KCI Assessment Requirement for Time Limited Certs)
        // Acceptance Criteria : "If a diplomate is in the grace period or whose certificate status is currently Not Certified, due to not meeting the assessment requirement , their assessment due date will not advance until they pass their second consecutive KCI.  
        //                      Note: current functionality is to advance the assessment due date after the first KCI pass."

        #region In Grace Period
        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass2ndKCIExamMeetAssessment_1stKCIPassIsOld_DontAdvancedDueDates()
        {
            new Pass1ndKCIExamMeetAssessment_1stKCIPassisOld_InGracePeriod_DontAdvancedDueDatesSpec().BDDfy();
        }

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass2ndKCIExamMeetAssessment_1stKCIFail_DontAdvancedDueDates()
        {
            new Pass1ndKCIExamMeetAssessment_1stKCIFail_InGracePeriod_DontAdvancedDueDatesSpec().BDDfy();
        }

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1ndKCIExamMeetAssessment_DontAdvancedDueDates()
        {
            new Pass1ndKCIExamMeetAssessment_InGracePeriod_DontAdvancedDueDatesSpec().BDDfy();
        }

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Fail1ndKCIExamMeetAssessment_DontAdvancedDueDates()
        {
            new Fail1ndKCIExamMeetAssessment_InGracePeriod_DontAdvancedDueDatesSpec().BDDfy();
        }

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Fail2ndKCIExamMeetAssessment_DontAdvancedDueDates()
        {
            new Fail2ndKCIExamMeetAssessment_InGracePeriod_DontAdvancedDueDatesSpec().BDDfy();
        }

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1ndKCIExamMeetAssessment1stKCIisOld_DontAdvancedDueDates()
        {
            new Pass1ndKCIExamMeetAssessment1stKCIisOld_InGracePeriod_DontAdvancedDueDatesSpec().BDDfy();
        }

        #endregion

        #region Expired and AssessmentMet is false

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1st_Faild2nd_Pass3rd_DontAdvancedDueDates()
        {
            new UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1st_Faild2nd_Pass3rd_DontAdvancedDueDatesSpec().BDDfy();
        }

        [TestCase]
        public void UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1st_Faild2nd_Ind3rd_CCC4rd_Pass5rd_DontAdvancedDueDates()
        {
           new UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1st_Faild2nd_Ind3rd_CCC4rd_Pass5rd_DontAdvancedDueDatesSpec().BDDfy();
        }
        #endregion

    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario : ProgramRulesServiceScenario
    {

        protected ExpectedTestResult ExpectedTestResult { get; set; }

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
            My<IRegistrationInterservice>().Setup(p => p.GetRegistrationById(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(Registration));

            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(RegistrationFullCollectionResource));

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(AllRegistrations));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            // ++++++++++++ Credential Service ++++++++++++
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                .Returns(new UpdateCredentialOnExamResultCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));

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

    /// <summary>
    /// The Pass KCI ExamMeet Assessment
    /// </summary>
    public class Pass1ndKCIExamMeetAssessment_1stKCIPassisOld_InGracePeriod_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2019, 03, 01);
            ProcessingDate = new DateTime(2019, 03, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=new DateTime(GracePeriodYear,01,01)),
                                                    (c => c.GracePeriodEndDate=new DateTime(GracePeriodYear,12,31)),
                                                    (d => d.Pathway=PathwayType.KCI)});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI pass in Grace period
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 03, 01),
                        seatDate: new DateTime(GracePeriodYear - 5, 03, 01), // !!!! too old
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            // 2 st KCI pass in Grace period
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 11, 01),
                        seatDate: new DateTime(GracePeriodYear, 11, 01),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** setting expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(GracePeriodYear, 12, 31),
                GracePeriodStartDate = new DateTime(GracePeriodYear, 01, 01),
                GracePeriodEndDate = new DateTime(GracePeriodYear, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.GracePeriodStartDate == ExpectedTestResult.GracePeriodStartDate
                                                         && y.GracePeriodEndDate == ExpectedTestResult.GracePeriodEndDate
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Never());
        }

        public void AndThenUpdateTLPCCredentialShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The Pass KCI ExamMeet Assessment
    /// </summary>
    public class Pass1ndKCIExamMeetAssessment_1stKCIFail_InGracePeriod_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2019, 03, 01);
            ProcessingDate = new DateTime(2019, 03, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=new DateTime(GracePeriodYear,01,01)),
                                                    (c => c.GracePeriodEndDate=new DateTime(GracePeriodYear,12,31)),
                                                    (d => d.Pathway=PathwayType.KCI)});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI pass in Grace period
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 03, 01),
                        seatDate: new DateTime(GracePeriodYear, 03, 01),
                        examResult: ExamResultType.Fail,
                        examType: ExamType.Kci);

            // 2 st KCI pass in Grace period
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 11, 01),
                        seatDate: new DateTime(GracePeriodYear, 11, 01),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(GracePeriodYear, 12, 31),
                GracePeriodStartDate = new DateTime(GracePeriodYear, 01, 01),
                GracePeriodEndDate = new DateTime(GracePeriodYear, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.GracePeriodStartDate == ExpectedTestResult.GracePeriodStartDate
                                                         && y.GracePeriodEndDate == ExpectedTestResult.GracePeriodEndDate
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Never());
        }

        public void AndThenUpdateTLPCCredentialShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The Pass KCI ExamMeet Assessment
    /// </summary>
    public class Pass1ndKCIExamMeetAssessment_InGracePeriod_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2019, 03, 01);
            ProcessingDate = new DateTime(2019, 03, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=new DateTime(GracePeriodYear,01,01)),
                                                    (c => c.GracePeriodEndDate=new DateTime(GracePeriodYear,12,31)),
                                                    (d => d.Pathway=PathwayType.MOC),
                                                    (d => d.ExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.KCIExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.MOCExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.DisplayExamDueDate = new DateTime(GracePeriodYear, 12, 31))});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI pass in Grace period
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 03, 01),
                        seatDate: new DateTime(GracePeriodYear, 03, 01),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(GracePeriodYear, 12, 31),
                GracePeriodStartDate = new DateTime(GracePeriodYear, 01, 01),
                GracePeriodEndDate = new DateTime(GracePeriodYear, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.GracePeriodStartDate == ExpectedTestResult.GracePeriodStartDate
                                                         && y.GracePeriodEndDate == ExpectedTestResult.GracePeriodEndDate
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Once());
        }

        public void AndThenUpdateTLPCCredentialShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The Pass KCI ExamMeet Assessment
    /// </summary>
    public class Pass1ndKCIExamMeetAssessment1stKCIisOld_InGracePeriod_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2019, 03, 01);
            ProcessingDate = new DateTime(2019, 03, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=new DateTime(GracePeriodYear,01,01)),
                                                    (c => c.GracePeriodEndDate=new DateTime(GracePeriodYear,12,31)),
                                                    (d => d.Pathway=PathwayType.MOC),
                                                    (d => d.ExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.KCIExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.MOCExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.DisplayExamDueDate = new DateTime(GracePeriodYear, 12, 31))});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI pass in Grace period
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 03, 01),
                        seatDate: new DateTime(GracePeriodYear, 03, 01),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(GracePeriodYear, 12, 31),
                GracePeriodStartDate = new DateTime(GracePeriodYear, 01, 01),
                GracePeriodEndDate = new DateTime(GracePeriodYear, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.GracePeriodStartDate == ExpectedTestResult.GracePeriodStartDate
                                                         && y.GracePeriodEndDate == ExpectedTestResult.GracePeriodEndDate
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Once());
        }

        public void AndThenUpdateTLPCCredentialShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The Pass 1st KCI ExamMeet Assessment ! Fail 2nd ! Pass 3rd 
    /// </summary>
    public class UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1st_Faild2nd_Pass3rd_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2020, 11, 02);
            ProcessingDate = new DateTime(2020, 12, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            //int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=null),
                                                    (c => c.GracePeriodEndDate=null),
                                                    (d => d.Pathway=PathwayType.MOC),
                                                    (d => d.ExamDueDate = new DateTime(2018, 12, 31)),
                                                    (d => d.KCIExamDueDate = new DateTime(2018, 12, 31)),
                                                    (d => d.MOCExamDueDate = new DateTime(2018, 12, 31)),
                                                    (d => d.DisplayExamDueDate = new DateTime(2018, 12, 31))});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI PASS 
            Set_Registration(credential: credential,
                                administrationDate: new DateTime(2020, 05, 02),
                                seatDate: new DateTime(2020, 03, 07),
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Kci);

            // 2 st KCI FAIL 
            Set_Registration(credential: credential,
                                administrationDate: new DateTime(2020, 08, 02),
                                seatDate: new DateTime(2020, 07, 15),
                                examResult: ExamResultType.Fail,
                                examType: ExamType.Kci);

            // 3 st KCI PASS
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(2020, 11, 02),
                        seatDate: new DateTime(2020, 11, 04),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(2018, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Once());
        }

    }

    /// <summary>
    /// The Pass 1st KCI ExamMeet Assessment ! Fail 2nd ! Ind 3rd ! CCC 4rd ! Pass 5rd 
    /// </summary>
    public class UpdateCredentialOnLoadingKCIExamResultOnlySpec_Pass1st_Faild2nd_Ind3rd_CCC4rd_Pass5rd_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2020, 11, 02);
            ProcessingDate = new DateTime(2020, 12, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            //int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=null),
                                                    (c => c.GracePeriodEndDate=null),
                                                    (d => d.Pathway=PathwayType.MOC),
                                                    (d => d.ExamDueDate = new DateTime(2018, 12, 31)),
                                                    (d => d.KCIExamDueDate = new DateTime(2018, 12, 31)),
                                                    (d => d.MOCExamDueDate = new DateTime(2018, 12, 31)),
                                                    (d => d.DisplayExamDueDate = new DateTime(2018, 12, 31))});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI PASS 
            Set_Registration(credential: credential,
                                administrationDate: new DateTime(2020, 05, 02),
                                seatDate: new DateTime(2020, 03, 07),
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Kci);

            // 2 st KCI FAIL 
            Set_Registration(credential: credential,
                                administrationDate: new DateTime(2020, 08, 02),
                                seatDate: new DateTime(2020, 07, 15),
                                examResult: ExamResultType.Fail,
                                examType: ExamType.Kci);


            // 3 st KCI Ind 
            Set_Registration(credential: credential,
                                administrationDate: new DateTime(2020, 08, 10),
                                seatDate: new DateTime(2020, 08, 15),
                                examResult: ExamResultType.Indeterminate,
                                examType: ExamType.Kci);

            // 4 st KCI CCC 
            Set_Registration(credential: credential,
                                administrationDate: new DateTime(2020, 08, 15),
                                seatDate: new DateTime(2020, 08, 20),
                                examResult: ExamResultType.Cancel,
                                examType: ExamType.Kci);

            // 5 st KCI PASS
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(2020, 11, 02),
                        seatDate: new DateTime(2020, 11, 04),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(2018, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Once());
        }

    }

    /// <summary>
    /// The Pass KCI ExamMeet Assessment
    /// </summary>
    public class Fail1ndKCIExamMeetAssessment_InGracePeriod_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2019, 03, 01);
            ProcessingDate = new DateTime(2019, 03, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=new DateTime(GracePeriodYear,01,01)),
                                                    (c => c.GracePeriodEndDate=new DateTime(GracePeriodYear,12,31)),
                                                    (d => d.Pathway=PathwayType.MOC),
                                                    (d => d.ExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.KCIExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.MOCExamDueDate = new DateTime(GracePeriodYear, 12, 31)),
                                                    (d => d.DisplayExamDueDate = new DateTime(GracePeriodYear, 12, 31))});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI FAIL in Grace period
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 03, 01),
                        seatDate: new DateTime(GracePeriodYear, 03, 01),
                        examResult: ExamResultType.Fail,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(GracePeriodYear, 12, 31),
                GracePeriodStartDate = new DateTime(GracePeriodYear, 01, 01),
                GracePeriodEndDate = new DateTime(GracePeriodYear, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(
                It.Is<UpdateCredentialOnExamResultCommand>(y => y.ExamDueDate == ExpectedTestResult.ExamDueDate.Value
                                                         && y.AssessmentMet == ExpectedTestResult.AssessmentMet
                                                         && y.GracePeriodStartDate == ExpectedTestResult.GracePeriodStartDate
                                                         && y.GracePeriodEndDate == ExpectedTestResult.GracePeriodEndDate
                                                         && y.Pathway == ExpectedTestResult.Pathway
                                                         && y.KCIExamDueDate == ExpectedTestResult.ExamDueDate)), Times.Once());
        }

        public void AndThenUpdateTLPCCredentialShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The Pass KCI ExamMeet Assessment
    /// </summary>
    public class Fail2ndKCIExamMeetAssessment_InGracePeriod_DontAdvancedDueDatesSpec : UpdateCredentialOnLoadingKCIExamResultOnlySpecScenario
    {
        /// <summary>
        /// PreSetup
        /// </summary>
        protected override void PreSetup()
        {
            //-----------------------
            Initialize();
            InitializeBuilders();
            InitializeDataProperties();

            // set Main data >>>>>
            EventDate = new DateTime(2019, 03, 01);
            ProcessingDate = new DateTime(2019, 03, 10);
            FirstIssuanceDate = new DateTime(2008, 11, 01);
            TriggeringEvent = TriggeringEvent.ExamResultMocKci;
            int GracePeriodYear = 2019;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Active,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: false,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: new List<Action<Credential>>() {
                                                    (a => a.LookbackDate = new DateTime(ProcessingDate.Year - 1, 12, 31)),
                                                    (b => b.GracePeriodStartDate=new DateTime(GracePeriodYear,01,01)),
                                                    (c => c.GracePeriodEndDate=new DateTime(GracePeriodYear,12,31)),
                                                    (d => d.Pathway=PathwayType.KCI)});

            // initial cert exam
            Set_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDate: FirstIssuanceDate,
                                examResult: ExamResultType.Pass,
                                examType: ExamType.Cert);

            // 1 st KCI pass in Grace period
            Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 03, 01),
                        seatDate: new DateTime(GracePeriodYear, 03, 01),
                        examResult: ExamResultType.Pass,
                        examType: ExamType.Kci);

            // 2 st KCI Fail in Grace period
            Registration = Set_Registration(credential: credential,
                        administrationDate: new DateTime(GracePeriodYear, 11, 01),
                        seatDate: new DateTime(GracePeriodYear, 11, 01),
                        examResult: ExamResultType.Fail,
                        examType: ExamType.Kci);

            Set_CurrentLookBackDatesInfo();

            //****** settingh expected results ++++++++++++++++++++++
            ExpectedTestResult = new ExpectedTestResult()
            {
                AssessmentMet = false,
                ExamDueDate = new DateTime(GracePeriodYear, 12, 31),
                GracePeriodStartDate = new DateTime(GracePeriodYear, 01, 01),
                GracePeriodEndDate = new DateTime(GracePeriodYear, 12, 31),
                Pathway = PathwayType.KCI
            };

        }

        public async Task WhenICallRunProcessesOnExamResultEvent()
        {
            try
            {
                await ProgramRulesService.RunProcessesOnExamResultEvent(It.IsAny<Guid>(), ProcessingDate, ExamRegistrationType.Registration);
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

        public void AndThenUpdateCredentialOnExamResultCommandWithExpectedValuesShouldBeCalledOnce()
        {
            // nothing to update 
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()), Times.Never());
        }

        public void AndThenUpdateTLPCCredentialShouldBeCalledOnce()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    #endregion
}
