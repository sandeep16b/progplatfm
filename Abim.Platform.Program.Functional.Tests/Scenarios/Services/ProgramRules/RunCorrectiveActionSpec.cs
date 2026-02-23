using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Extensions.ExternalResponses;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using Abim.Platform.Program.Tests.Setup.Responses;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the RunCorrectiveAction function in Program Rules service",
      SoThat = "it can correctly process credential records"
      )]
    [TestFixture]
    public class RunCorrectiveActionSpec
    {
        [TestCase]
        [WorkItem(144911)]
        [WorkItem(230675)]
        public void ExamResultProcessing_PassMocExam_SuccessAndSetIsInCmp()
        {
            new ExamResultProcessing_PassMocExam_SuccessAndSetIsInCmp_Scenario().BDDfy();
        }

        [TestCase]
        [WorkItem(144911)]
        public void ExamResultProcessing_NoCreds_StopProcessing()
        {
            new ExamResultProcessing_NoCreds_StopProcessing().BDDfy();
        }

        [TestCase]
        [WorkItem(148899)]
        public void ExamResultProcessing_Set_ConsecutiveKCIPassRequired_When_Applicable()
        {
            new ExamResultProcessing_ConsecutiveKCIPassRequired_Set_To_True_When_Applicable().BDDfy();
        }

        [TestCase]
        [WorkItem(148899)]
        public void ExamResultProcessing_Do_Not_Set_ConsecutiveKCIPassRequired_When_It_Is_Already_True()
        {
            new ExamResultProcessing_ConsecutiveKCIPassRequired_Not_Set_When_ConsecutiveKCIPassRequired_Is_Already_True().BDDfy();
        }

        [TestCase]
        [WorkItem(148899)]
        public void ExamResultProcessing_Do_Not_Set_ConsecutiveKCIPassRequired_When_ExamDueDate_Is_After_LookbackDate()
        {
            new ExamResultProcessing_ConsecutiveKCIPassRequired_Not_Set_When_ExamDueDate_Is_After_LookbackDate().BDDfy();
        }

        [TestCase]
        [WorkItem(148899)]
        public void ExamResultProcessing_Do_Not_Set_ConsecutiveKCIPassRequired_When_Pending_Exam_Results_Exist()
        {
            new ExamResultProcessing_ConsecutiveKCIPassRequired_Not_Set_When_Pending_Exam_Results_Exist().BDDfy();
        }

        [TestCase]
        [WorkItem(160589)]
        public void ExamResultProcessing_Do_Not_Expire_Active_Issuances_When_Exam_Admin_Occurred_After_Last_Lookback()
        {
            new ExamResultProcessing_DoNotExpireActiveIssuances_ExamAdminAfterLastLookbackDate().BDDfy();
        }

        [TestCase]
        [WorkItem(160589)]
        public void ExamResultProcessing_Expire_Active_Issuances_When_Exam_Admin_Occurred_Before_Last_Lookback_And_Exam_Requirements_Not_Met()
        {
            new ExamResultProcessing_ExpireActiveIssuances_ExamAdminPriorToLastLookback_ExamRequirementsNotMet().BDDfy();
        }

        [TestCase]
        [WorkItem(160589)]
        public void ExamResultProcessing_Expire_Active_Issuances_When_Exam_Admin_Occurred_Before_Last_Lookback_And_Non_Exam_Requirements_Not_Met()
        {
            new ExamResultProcessing_ExpireActiveIssuances_ExamAdminPriorToLastLookback_NonExamRequirementsNotMet().BDDfy();
        }

        [TestCase]
        [WorkItem(160589)]
        public void ExamResultProcessing_Do_Not_Expire_Active_Issuances_When_Exam_Admin_Occurred_Before_Last_Lookback_But_All_Requirements_Were_Met()
        {
            new ExamResultProcessing_DoNotExpireActiveIssuances_ExamBeforeLastLookbackDate_ExamAndNonExamRequirementsMet().BDDfy();
        }

        [TestCase]
        [WorkItem(161292)]
        public void ExamResultProcessing_Do_Not_Perform_Corrective_Action_For_OnHold_CMPRegistration()
        {
            new CorrectiveActionOnHoldCMPRegistrationScenario().BDDfy();
        }

        // PSR 295380 -- Cert in Grace Period, not Eligible for Grace Period
        [TestCase]
        [WorkItem(175873)]
        public void CorrectiveAction_SetGracePeriodIn2019_Success()
        {
            new CorrectiveAction_SetGracePeriodIn2019().BDDfy(); //1
        }

        [TestCase]
        [WorkItem(183137)]
        public void CorrectiveAction_Should_Exclude_Deselected_Certs()
        {
            new CorrectiveActionShouldExcludeDeselectedCertsScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(175873)]
        public void CorrectiveAction_DontSetGracePeriodIn2021_Success()
        {
            new CorrectiveAction_DontSetGracePeriodIn2021().BDDfy();
        }

        [TestCase]
        [WorkItem(208254)]
        [WorkItem(216370)]
        public void CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2020_Success()
        {
            new CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2020().BDDfy();
        }

        [TestCase]
        [WorkItem(208254)]
        [WorkItem(216370)]
        public void CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2021_Success()
        {
            new CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2021().BDDfy();
        }

        [TestCase]
        [WorkItem(208254)]
        [WorkItem(216370)]
        public void CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2022ForCovidCreds_Success()
        {
            new CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2022ForCovidCreds().BDDfy();
        }

        [TestCase]
        [WorkItem(208254)]
        [WorkItem(216370)]
        public void CorrectiveAction_SetGracePeriodWhenExamDueDateIn2022ForNotCovidCreds_Success()
        {
            new CorrectiveAction_SetGracePeriodWhenExamDueDateIn2022ForNotCovidCreds().BDDfy();
        }

        [TestCase]
        [WorkItem(183137)]
        public void CorrectiveAction_Should_NOT_Exclude_Previously_Deselected_Certs_Which_Have_Been_Reselected()
        {
            new CorrectiveActionShouldIncludePreviouslyDeselectedButSinceReselectedCertsScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(210153)]
        public void CorrectiveAction_Should_Not_Process_Cosponsored_Credentials()
        {
            new CorrectiveActionShouldNotProcessCosponsoredCredentialsScenario().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class RunCorrectiveActionSpecScenario : ProgramRulesServiceScenario
    {
        protected Mock<ILogger> Log { get; set; }
        protected DateTime FirstIssuanceDate { get; set; }
        protected Guid CertificationId { get; set; }
        protected Credential Credential { get; set; }
        protected ProgramRulesService ProgramRulesService { get; set; }
        protected new Exception ExceptionCaught { get; set; }
        protected int IssuanceId { get; set; }
        protected bool Result { get; set; }
        protected ActivityFullCollectionResource ActivitiesFullCollectionResource { get; set; }
        protected DateTime EventDate { get; set; }
        protected DateTime ProcessingDate { get; set; }
        protected DateTime LookbackDate { get; set; }
        protected Tuple<DateTime?, DateTime?> LookBackEndDates { get; set; }
        protected LookBackDatesInfo currentLookBackDatesInfo { get; set; }
        protected LookBackDatesInfo expectedLookBackDatesInfo { get; set; }
        // builders
        protected IssuanceDataBuilder IssuanceDataBuilder { get; set; }
        protected SourceDataBuilder SourceDataBuilder { get; set; }
        protected CertificationDataBuilder CertificationDataBuilder { get; set; }
        protected CredentialDataBuilder CredentialDataBuilder { get; set; }
        protected RegistrationResourceBuilder RegistrationResourceBuilder { get; set; }

        protected DateTime Lookback2YearStartDate = new DateTime();
        protected DateTime Lookback2YearEndDate = new DateTime();
        protected DateTime Lookback5YearStartDate = new DateTime();
        protected DateTime Lookback5YearEndDate = new DateTime();

        protected RegistrationResource Registration { get; set; }

        protected readonly DateTime BASE_DATE = new DateTime(DateTime.Now.Year, 10, 1);

        /// <summary>
        /// Setup
        /// </summary>
        protected void Setup()
        {
            Log = new Mock<ILogger>();

            IssuanceDataBuilder = new IssuanceDataBuilder();
            SourceDataBuilder = new SourceDataBuilder();

            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        protected void AddToUserActivities(ActivityResource activityResource)
        {
            ActivitiesFullCollectionResource.Data.Add(activityResource);
        }

        #region Conditions

        // CompletedDate
        protected void Condition_InReciprocityProgram(DateTime completedDate)
        {
            ActivityResource activityResourceReciprocity = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ReciprocityAttest },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = completedDate,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = 0,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource() { CreditType = new CreditTypeResource() { Value = "Unknown" }, CreditDate = DateTime.Now }
                }
            };

            AddToUserActivities(activityResourceReciprocity);
        }

        // CancelledDate
        protected void Condition_InReciprocityProgram(DateTime completedDate, DateTime cancelledDate)
        {
            ActivityResource activityResourceReciprocity = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ReciprocityAttest },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Cancelled),
                CompletedDate = completedDate,
                CancellationReason = new EnumValueResponseResource<Product.Resources.Enums.CancellationReasonType>(Product.Resources.Enums.CancellationReasonType.Financial),
                CancelledDate = cancelledDate,
                TotalMOCPoints = 0,
                ActivityCredits = new List<ActivityCreditResource>(){
                    new ActivityCreditResource() { CreditType = new CreditTypeResource() { Value = "Unknown" }, CreditDate = DateTime.Now }
                }
            };

            AddToUserActivities(activityResourceReciprocity);
        }

        // MOC Points (activity record)       
        protected void Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(
                                        DateTime completedDate,
                                        DateTime creditDate,
                                        decimal totalMOCPoints,
                                        decimal medicalKnowledgePoints)
        {
            ActivityResource activity = new ActivityResource()
            {
                Product = new ProductResource() { Code = "Any" },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = completedDate,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = totalMOCPoints,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource()
                    {
                        CreditDate = creditDate,
                        CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.MedicalKnowledgePoints },
                       
                        Claimed=true,
                        CreditEarned = medicalKnowledgePoints
                    },

                     new ActivityCreditResource()
                    {
                        CreditDate = creditDate,
                        CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.PracticeAssessement },
                        
                        Claimed=true,
                        CreditEarned = totalMOCPoints-medicalKnowledgePoints
                    }

                }
            };

            AddToUserActivities(activity);
        }

        protected void Condition_RegistrationInterservice_GetUserRegistrations(Guid certificationId,
                                                                                DateTime examTestDate,
                                                                                ExamType examType,
                                                                                ExamResultType examResultType)
        {
            var registrationsMock = new RegistrationFullCollectionResource();

            registrationsMock.Data.Add(new RegistrationResource()
            {
                Result = examResultType.ToString(),
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(examType),
                MemberId = new Guid(),
                CertificationId = certificationId,
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = examTestDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));
        }


        #endregion
    }

    #region Scenarios 

    /// <summary>
    /// The ExamResultProcessing_PassMocExam_Success
    /// </summary>
    public class ExamResultProcessing_PassMocExam_SuccessAndSetIsInCmp_Scenario : RunCorrectiveActionSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(DateTime.Now.Year - 3, 8, 3);
            EventDate = new DateTime(DateTime.Now.Year, 9, 1);
            ProcessingDate = new DateTime(DateTime.Now.Year, 02, 02);
            LookbackDate = new DateTime(DateTime.Now.Year, 12, 31);

            //Not Expired Two and Five year look backs (not really real dates)
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(DateTime.Now.Year, 12, 31), new DateTime(DateTime.Now.Year, 12, 31));

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            certification.ExternalId = CertificationId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.IssuanceDate = FirstIssuanceDate)
                                .With(b => b.ExpirationDate = new DateTime(b.IssuanceDate.Year + 11, 12, 31)) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(a => a.Certification = certification)
                        .Build();

            // bug 230675 : Exam Result Processing Switches Pathway But Does Not Clear Is In CMP Flag
            // since it is MOC exam result it should set this flag to false
            Credential.IsInCMP = true; 

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                            creditDate: DateTime.Now,
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);

            Registration = (new RegistrationResourceBuilder())
                            .WithCertificationId(CertificationId)
                            .WithAdministrationYear(BASE_DATE.Year)
                            .WithAdministrationDate(BASE_DATE)
                            .WithExamResult(ExamResultType.Pass)
                            .WithExamType(ExamType.Moc)
                            .WithSeat(BASE_DATE.AddDays(-5))
                            .Build();
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                    .Returns(new UpdateCredentialOnExamResultCommandResult(CommandStatus.Accepted, null, null));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            var registrationsMock = new RegistrationFullCollectionResource();

            registrationsMock.Data.Add(new RegistrationResource()
            {
                Result = ExamResultType.Fail.ToString(),
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc),
                MemberId = new Guid(),
                CertificationId = new Guid(),
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = EventDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            // ++++++++++++ Log ++++++++++++
            ProgramRulesService.Log = Log.Object;
            LogTest.Watch(Log);

        }

        public async Task WhenICallRunCorrectiveActionAsync()
        {
            try
            {
               Result = await ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: new List<Credential>() { Credential },
                                memberId: new Guid(),
                                eventDate: EventDate,
                                processingDate: ProcessingDate,
                                triggeringEvent: TriggeringEvent.ExamResultMocKci,
                                registration: new RegistrationData(Registration));
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

        public void AndThenResultShouldBeTrue()
        {
            Result.Should().Be(true);
        }

        public void AndThenIsInCmpShouldBeSetToFalse()
        {
            // Bug 230675 : Exam Result Processing Switches Pathway But Does Not Clear Is In CMP Flag
            Credential.IsInCMP.Should().Be(false); 
        }

    }

    /// <summary>
    /// The ExamResultProcessing_NoCreds_StopProcessing
    /// </summary>
    public class ExamResultProcessing_NoCreds_StopProcessing : RunCorrectiveActionSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(DateTime.Now.Year - 3, 8, 3);
            EventDate = new DateTime(DateTime.Now.Year, 9, 1);
            ProcessingDate = new DateTime(DateTime.Now.Year, 02, 02);
            LookbackDate = new DateTime(DateTime.Now.Year, 12, 31);

            //Not Expired Two and Five year look backs (not really real dates)
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(DateTime.Now.Year, 12, 31), new DateTime(DateTime.Now.Year, 12, 31));

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            //certification.ExternalId = CertificationId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.IssuanceDate = FirstIssuanceDate)
                                .With(b => b.ExpirationDate = new DateTime(b.IssuanceDate.Year + 11, 12, 31)) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(a => a.Certification = certification)
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                            creditDate: DateTime.Now,
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);

            Registration = (new RegistrationResourceBuilder())
                            .WithCertificationId(CertificationId)
                            .WithAdministrationYear(BASE_DATE.Year)
                            .WithAdministrationDate(BASE_DATE)
                            .WithExamResult(ExamResultType.Pass)
                            .WithExamType(ExamType.Moc)
                            .WithSeat(BASE_DATE.AddDays(-5))
                            .Build();
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                    .Returns(new UpdateCredentialOnExamResultCommandResult(CommandStatus.Accepted, null, null));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            var registrationsMock = new RegistrationFullCollectionResource();

            registrationsMock.Data.Add(new RegistrationResource()
            {
                Result = ExamResultType.Fail.ToString(),
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc),
                MemberId = new Guid(),
                CertificationId = new Guid(),
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = EventDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            // ++++++++++++ Log ++++++++++++
            ProgramRulesService.Log = Log.Object;
            LogTest.Watch(Log);

        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: new List<Credential>() { Credential },
                                memberId: new Guid(),
                                eventDate: EventDate,
                                processingDate: ProcessingDate,
                                triggeringEvent: TriggeringEvent.ExamResultMocKci,
                                registration: new RegistrationData(Registration)).Result;
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
            Result.Should().Be(false);
        }

        public void ThenLogShouldContainWarn()
        {
            LogTest.Warns.Should().Contain(a=>a.StartsWith ("ExamResultMocKci : No Credential found for MemberId:"));
        }
    }

    public abstract class ConsecutiveKCIPassRequiredScenario : ProgramRulesServiceSimplifiedScenario
    {
        protected Credential _cred;
        protected RegistrationResource _reg;
        protected DateTime _eventDate;
        protected DateTime _processingDate;

        protected override void SetupCredentialServiceMock()
        {
            base.SetupCredentialServiceMock();

            _credSvcMock
                .Setup(x => x.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                .Returns(new UpdateCredentialOnExamResultCommandResult());

            _credSvcMock
                .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromObjectCommand>()))
                .Returns(new UpdateCredentialFromObjectCommandResult());

            _credSvcMock
                .Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(2008, 1, 1));

            _credSvcMock
                .Setup(x => x.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(new List<Credential>(0));
        }

        protected override void SetupProductInterserviceMock()
        {
            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new ActivityFullCollectionResource()));
        }

        protected override void SetupRegistrationInterserviceMock()
        {
            base.SetupRegistrationInterserviceMock();

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) }));

            _regInterSvcMock
                .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
        }

        protected virtual void SetupCredential()
        {
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "Unit Test");
            _cred =
                CredentialBuilder
                    .BuildWithoutRandoms(
                        source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);

            //_cred.Certification.Id = Guid.NewGuid();

            _cred.AddIssuance(
                IssuanceBuilder.BuildWithoutRandoms(
                    source,
                    IssuanceStatusType.Active,
                    DateTime.Now.AddYears(-3),
                    DurationType.Continuous,
                    MaintenanceRequirementType.Required,
                    MaintenanceStatusType.Maintained,
                    OccurrenceType.Initial));

            _cred.LookbackDate = new DateTime(2019, 12, 31);
            _cred.ExamDueDate = new DateTime(2019, 12, 31);
        }

        protected virtual void SetupRegistration()
        {
            var builder = new RegistrationResourceBuilder();
            _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithAdministrationDate(new DateTime(2019, 11, 30))
                    .Build();
        }

        protected void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            _eventDate = new DateTime(2019, 11, 30);
            _processingDate = new DateTime(2019, 12, 1);
        }

        protected async void WhenIRunCorrectiveAction()
        {
            try
            {
                await _sut.RunCorrectiveAction(
                    new List<Credential>(1) { _cred },
                    Guid.NewGuid(),
                    _eventDate,
                    _processingDate,
                    TriggeringEvent.ExamResultMocKci,
                    new RegistrationData(_reg));
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

    /// <summary>
    /// Test that ensures the credential's ConsecutiveKCIPassRequired property is set to true when applicable
    /// </summary>
    public class ExamResultProcessing_ConsecutiveKCIPassRequired_Set_To_True_When_Applicable : ConsecutiveKCIPassRequiredScenario
    {
        private void AndTheConsecutiveKCIPassRequiredPropertyShouldHaveBeenUpdatedViaAHandleMethod()
        {
            _credSvcMock.Verify(x => x.Handle(It.Is<UpdateCredentialFromObjectCommand>(cmd => cmd.SetConsecutiveKCIPassRequired == true)), Times.Once);
        }
    }

    /// <summary>
    /// Test that ensures the credential's ConsecutiveKCIPassRequired property is not set when it's already true
    /// </summary>
    public class ExamResultProcessing_ConsecutiveKCIPassRequired_Not_Set_When_ConsecutiveKCIPassRequired_Is_Already_True : ConsecutiveKCIPassRequiredScenario
    {
        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ConsecutiveKCIPassRequired = true;
        }

        private void AndTheConsecutiveKCIPassRequiredPropertyShouldNotHaveBeenUpdatedViaAHandleMethod()
        {
            _credSvcMock.Verify(x => x.Handle(It.Is<UpdateCredentialFromObjectCommand>(cmd => cmd.SetConsecutiveKCIPassRequired == false)), Times.Once);
        }
    }

    /// <summary>
    /// Test that ensures the credential's ConsecutiveKCIPassRequired property is not set when the exam due date is after the lookback date
    /// </summary>
    public class ExamResultProcessing_ConsecutiveKCIPassRequired_Not_Set_When_ExamDueDate_Is_After_LookbackDate : ConsecutiveKCIPassRequiredScenario
    {
        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2021, 12, 31);
        }

        private void AndTheConsecutiveKCIPassRequiredPropertyShouldNotHaveBeenUpdatedViaAHandleMethod()
        {
            _credSvcMock.Verify(x => x.Handle(It.Is<UpdateCredentialFromObjectCommand>(cmd => cmd.SetConsecutiveKCIPassRequired == false)), Times.Once);
        }
    }

    /// <summary>
    /// Test that ensures the credential's ConsecutiveKCIPassRequired property is not set when pending exam results exist
    /// </summary>
    public class ExamResultProcessing_ConsecutiveKCIPassRequired_Not_Set_When_Pending_Exam_Results_Exist : ConsecutiveKCIPassRequiredScenario
    {
        protected override void SetupRegistration()
        {
            var regBuilder = new RegistrationResourceBuilder();
            _reg =
                regBuilder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamType(ExamType.Kci)
                    .WithAdministrationYear(2019)
                    .WithResult("PENDING")
                    .WithExamResult(ExamResultType.Pending)
                    .Build();

            var regCollection = new UserRegistrationsAndCMPRegistrationsResource();
            regCollection.Registrations = new List<RegistrationResource>(1);
            regCollection.CMPRegistrations = new List<CMPRegistrationResource>(0);
            regCollection.Registrations = new List<RegistrationResource>(1);
            regCollection.Registrations.Add(_reg);

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(regCollection));
        }

        private void AndTheConsecutiveKCIPassRequiredPropertyShouldNotHaveBeenUpdatedViaAHandleMethod()
        {
            _credSvcMock.Verify(x => x.Handle(It.Is<UpdateCredentialFromObjectCommand>(cmd => cmd.SetConsecutiveKCIPassRequired == false)), Times.Once);
        }
    }

    public abstract class ExamResultProcessingShouldWeExpireActiveIssuancesScenario : ProgramRulesServiceSimplifiedScenario
    {
        protected Credential _cred;
        protected RegistrationResource _reg;
        protected DateTime _eventDate;
        protected DateTime _processingDate;

        protected override void SetupCredentialServiceMock()
        {
            base.SetupCredentialServiceMock();

            _credSvcMock
                .Setup(x => x.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                .Returns(new UpdateCredentialOnExamResultCommandResult());

            _credSvcMock
                .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromObjectCommand>()))
                .Returns(new UpdateCredentialFromObjectCommandResult());

            _credSvcMock
                .Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(2008, 1, 1));

            _credSvcMock
                .Setup(x => x.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(new List<Credential>(0));
        }

        protected override void SetupProductInterserviceMock()
        {
            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(new ActivityFullCollectionResource()));
        }

        protected override void SetupRegistrationInterserviceMock()
        {
            base.SetupRegistrationInterserviceMock();

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) }));

            _regInterSvcMock
                .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
        }

        protected virtual void SetupCredential()
        {
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "Unit Test");
            _cred =
                CredentialBuilder
                    .BuildWithoutRandoms(
                        source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);

            //_cred.Certification.Id = Guid.NewGuid();

            _cred.AddIssuance(
                IssuanceBuilder.BuildWithoutRandoms(
                    source,
                    IssuanceStatusType.Active,
                    DateTime.Now.AddYears(-3),
                    DurationType.Continuous,
                    MaintenanceRequirementType.Required,
                    MaintenanceStatusType.Maintained,
                    OccurrenceType.Initial));

            _cred.LookbackDate = new DateTime(2019, 12, 31);
            _cred.ExamDueDate = new DateTime(2019, 12, 31);
        }

        protected virtual void SetupRegistration()
        {
            var builder = new RegistrationResourceBuilder();
            _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithAdministrationDate(new DateTime(2019, 11, 30))
                    .WithAdministrationYear(2019)
                    .Build();
        }

        protected virtual void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            _eventDate = new DateTime(2019, 11, 30);
            _processingDate = new DateTime(2019, 12, 1);
        }

        protected async void WhenIRunCorrectiveAction()
        {
            try
            {
                await _sut.RunCorrectiveAction(
                    new List<Credential>(1) { _cred },
                    Guid.NewGuid(),
                    _eventDate,
                    _processingDate,
                    TriggeringEvent.ExamResultMocKci,
                    new RegistrationData(_reg));
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

    #region Expire Active Issuances During Exam Result Processing

    public abstract class ExamResultProcessingExpireActiveIssuancesScenario : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {
        protected void AndTheCredentialServiceShouldHaveBeenCalledWithACommandValueToExpireActiveIssuances()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialOnExamResultCommand>(command =>
                        command.ExpireActiveIssuances == true && command.CredentialId == _cred.ExternalId)),
                Times.Once);
        }
    }

    public class ExamResultProcessing_ExpireActiveIssuances_ExamAdminPriorToLastLookback_ExamRequirementsNotMet : ExamResultProcessingExpireActiveIssuancesScenario
    {
        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2019, 11, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }
    }

    public class ExamResultProcessing_ExpireActiveIssuances_ExamAdminPriorToLastLookback_NonExamRequirementsNotMet : ExamResultProcessingExpireActiveIssuancesScenario
    {
        protected override void SetupCredential()
        {
            //Setup credential so that they'll meet the Exam requirements (they will not have met the Non-Exam requirements)
            base.SetupCredential();
            _cred.AssessmentMet = true;
            _cred.AssessmentMetDate = new DateTime(2019, 11, 1);
        }
    }

    #endregion Expire Active Issuances During Exam Result Processing

    #region Do Not Expire Active Issuances During Exam Result Processing
    public abstract class ExamResultProcessingDoNotExpireActiveIssuancesScenario : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {
        protected void AndTheCredentialServiceShouldNotHaveBeenCalledWithACommandValueToExpireActiveIssuances()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialOnExamResultCommand>(command =>
                        command.ExpireActiveIssuances == true && command.CredentialId == _cred.ExternalId)),
                Times.Never);
        }
    }

    public class ExamResultProcessing_DoNotExpireActiveIssuances_ExamAdminAfterLastLookbackDate : ExamResultProcessingDoNotExpireActiveIssuancesScenario
    {
        protected override void SetupRegistration()
        {
            var builder = new RegistrationResourceBuilder();
            _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithAdministrationDate(new DateTime(2020, 11, 30))
                    .WithAdministrationYear(2020)
                    .Build();
        }
    }

    public class ExamResultProcessing_DoNotExpireActiveIssuances_ExamBeforeLastLookbackDate_ExamAndNonExamRequirementsMet : ExamResultProcessingDoNotExpireActiveIssuancesScenario
    {
        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2019, 11, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void SetupCredential()
        {
            //Setup credential so that they'll meet the Exam requirements
            base.SetupCredential();
            _cred.AssessmentMet = true;
            _cred.AssessmentMetDate = new DateTime(2019, 11, 1);
        }
    }


    #endregion Do Not Expire Active Issuances During Exam Result Processing

    public class CorrectiveActionOnHoldCMPRegistrationScenario : ProgramRulesServiceSimplifiedScenario
    {
        private Credential _cred;
        private CMPRegistrationResource _cmpReg;
        private DateTime _eventDate;
        private DateTime _processingDate;

        protected virtual void SetupCredential()
        {
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "Unit Test");
            _cred =
                CredentialBuilder
                    .BuildWithoutRandoms(
                        source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);

            _cred.AddIssuance(
                IssuanceBuilder.BuildWithoutRandoms(
                    source,
                    IssuanceStatusType.Active,
                    DateTime.Now.AddYears(-3),
                    DurationType.Continuous,
                    MaintenanceRequirementType.Required,
                    MaintenanceStatusType.Maintained,
                    OccurrenceType.Initial));

            _cred.LookbackDate = new DateTime(2019, 12, 31);
            _cred.ExamDueDate = new DateTime(2019, 12, 31);
        }

        protected virtual void SetupRegistration()
        {
            var examBuilder = new CMPExamSummaryResourceBuilder();
            var exam = examBuilder.Build();

            var builder = new CMPRegistrationResourceBuilder();
            _cmpReg = 
                builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithOnHold(true)
                    .WithCMPExam(exam)
                    .Build();
        }

        protected void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            _eventDate = new DateTime(2019, 11, 30);
            _processingDate = new DateTime(2019, 12, 1);
        }

        protected async void WhenIRunCorrectiveAction()
        {
            try
            {
                await _sut.RunCorrectiveAction(
                    new List<Credential>(1) { _cred },
                    Guid.NewGuid(),
                    _eventDate,
                    _processingDate,
                    TriggeringEvent.ExamResultMocKci,
                    new RegistrationData(_cmpReg));
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

        protected void AndNoFurtherProcessingShouldHaveTakenPlace()
        {
            //If either of these methods was called, it means Corrective Action kept going, and this test should fail!
            //_accessTokenSvcMock.Verify(mock => mock.GetAccessToken(), Times.Never);
            _prodInterSvcMock.Verify(mock => 
                mock.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Never);
        }
    }


    public class CorrectiveAction_SetGracePeriodIn2019 : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {
        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2019, 12, 31); // not in 2020
        }

        protected void SetupLocalRegistrationInterserviceMock()
        {
            var builder = new RegistrationResourceBuilder();
            RegistrationResource _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithAdministrationDate(new DateTime(2019, 11, 30))
                    .WithAdministrationYear(2019)
                    .Build();

            List<RegistrationResource> registrations = new List<RegistrationResource>() { _reg };

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = registrations }));
        }

        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2019, 1, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            SetupLocalRegistrationInterserviceMock();

            _eventDate = new DateTime(2019, 11, 30);
            _processingDate = new DateTime(2019, 12, 1);
        }

        protected void AndTheUpdateCredentialFromObjectCommand_Handler_ShoulBeCalledOnce()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialFromObjectCommand>(command =>
                    command.SetGracePeriod==true && // !!! setting grace period
                    command.ReistateCertificate==false )),
                Times.Once);
        }
    }

    public class CorrectiveAction_DontSetGracePeriodIn2021 : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {

        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2020, 12, 31); // 2020
        }

        protected void SetupLocalRegistrationInterserviceMock()
        {
            var builder = new RegistrationResourceBuilder();
            RegistrationResource _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithAdministrationDate(new DateTime(2020, 11, 30))
                    .WithAdministrationYear(2020)
                    .Build();

            List<RegistrationResource> registrations = new List<RegistrationResource>() { _reg };

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = registrations }));
        }

        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2019, 1, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            SetupLocalRegistrationInterserviceMock();

            _eventDate = new DateTime(2020, 11, 30);
            _processingDate = new DateTime(2020, 12, 1);
        }

        protected void AndTheUpdateCredentialFromObjectCommand_Handler_ShouldNeverBeCalled()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialFromObjectCommand>(command =>
                    command.SetGracePeriod == false && // !!! 
                    command.ReistateCertificate == false)),
                Times.Never);
        }
    }

    public class CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2020 : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {

        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2020, 12, 31);
            _cred.LookbackDate = new DateTime(2020, 12, 31);
        }

        protected void SetupLocalRegistrationInterserviceMock()
        {
            var builder = new RegistrationResourceBuilder();
            RegistrationResource _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithAdministrationDate(new DateTime(2020, 11, 30))
                    .WithAdministrationYear(2020)
                    .Build();

            List<RegistrationResource> registrations = new List<RegistrationResource>() { _reg };

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = registrations }));
        }

        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2020, 1, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            SetupLocalRegistrationInterserviceMock();

            _eventDate = new DateTime(2020, 11, 30);
            _processingDate = new DateTime(2020, 12, 1);
        }

        protected void AndTheUpdateCredentialFromObjectCommand_Handler_ShouldNeverBeCalled()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialFromObjectCommand>(command =>
                    command.SetGracePeriod == true 
                    )),
                Times.Never);
        }
    }

    public class CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2021 : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {

        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2021, 12, 31);
            _cred.LookbackDate = new DateTime(2021, 12, 31);
        }

        protected void SetupLocalRegistrationInterserviceMock()
        {
            var builder = new RegistrationResourceBuilder();
            RegistrationResource _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithAdministrationDate(new DateTime(2021, 11, 30))
                    .WithAdministrationYear(2021)
                    .Build();

            List<RegistrationResource> registrations = new List<RegistrationResource>() { _reg };

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = registrations }));
        }

        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2021, 1, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            SetupLocalRegistrationInterserviceMock();

            _eventDate = new DateTime(2021, 11, 30);
            _processingDate = new DateTime(2021, 12, 1);
        }

        protected void AndTheUpdateCredentialFromObjectCommand_Handler_ShouldNeverBeCalled()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialFromObjectCommand>(command =>
                    command.SetGracePeriod == true )),
                Times.Never);
        }
    }

    public class CorrectiveAction_DontSetGracePeriodWhenExamDueDateIn2022ForCovidCreds : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {

        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2022, 12, 31);
            _cred.LookbackDate = new DateTime(2022, 12, 31);
            _cred.Certification.Code = ProgramResourceConstants.CertificationCode.InfectiousDisease;
        }

        protected void SetupLocalRegistrationInterserviceMock()
        {
            var builder = new RegistrationResourceBuilder();
            RegistrationResource _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithAdministrationDate(new DateTime(2022, 11, 30))
                    .WithAdministrationYear(2022)
                    .Build();

            List<RegistrationResource> registrations = new List<RegistrationResource>() { _reg };

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = registrations }));
        }

        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2022, 1, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            SetupLocalRegistrationInterserviceMock();

            _eventDate = new DateTime(2022, 11, 30);
            _processingDate = new DateTime(2022, 12, 1);
        }

        protected void AndTheUpdateCredentialFromObjectCommand_Handler_ShouldNeverBeCalled()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialFromObjectCommand>(command =>
                    command.SetGracePeriod == true )),
                Times.Never); 
        }
    }

    public class CorrectiveAction_SetGracePeriodWhenExamDueDateIn2022ForNotCovidCreds : ExamResultProcessingShouldWeExpireActiveIssuancesScenario
    {

        protected override void SetupCredential()
        {
            base.SetupCredential();
            _cred.ExamDueDate = new DateTime(2022, 12, 31); 
            _cred.LookbackDate = new DateTime(2022, 12, 31);
            _cred.Certification.Code = ProgramResourceConstants.CertificationCode.GeriatricMedicine;
        }

        protected void SetupLocalRegistrationInterserviceMock()
        {
            var builder = new RegistrationResourceBuilder();
            RegistrationResource _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithAdministrationDate(new DateTime(2022, 11, 30))
                    .WithAdministrationYear(2022)
                    .Build();

            List<RegistrationResource> registrations = new List<RegistrationResource>() { _reg };

            _regInterSvcMock
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = registrations }));
        }

        protected override void SetupProductInterserviceMock()
        {
            //Give them an activity so that they'll have met their Non-Exam requirements (they will not have met the Exam requirements)
            var activities = new ActivityFullCollectionResource();
            activities.Data = new List<ActivityResource>(1);
            var builder = new ActivityResourceBuilder();
            activities.Data.Add(
                builder
                    .WithCompletedDate(new DateTime(2022, 1, 1))
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithTotalMOCPoints(100)
                    .Build());

            base.SetupProductInterserviceMock();
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void GivenIHaveValidParameters()
        {
            SetupCredential();
            SetupRegistration();
            SetupLocalRegistrationInterserviceMock();

            _eventDate = new DateTime(2022, 11, 30);
            _processingDate = new DateTime(2022, 12, 1);
        }

        protected void AndTheUpdateCredentialFromObjectCommand_Handler_ShouldBeenCalled()
        {
            _credSvcMock.Verify(mock =>
                mock.Handle(
                    It.Is<UpdateCredentialFromObjectCommand>(command =>
                    command.SetGracePeriod == true && // !!! 
                    command.ReistateCertificate == false)),
                Times.Once);
        }
    }

    public class CorrectiveActionShouldExcludeDeselectedCertsScenario : RunCorrectiveActionSpecScenario
    {
        private List<Credential> _credentials;
        private Guid _memberId = Guid.NewGuid();

        protected override void PreSetup()
        {
            //No pre setup needed for this scenario
        }


        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            ProgramRulesService.Log = Log.Object;
            LogTest.Watch(Log);
        }

        private Credential GetDeselectedCredential()
        {
            var credential = new Mock<Credential>();
            credential.SetupGet(cred => cred.DeselectionProcessed).Returns(true);
            credential.SetupGet(cred => cred.DeselectionElected).Returns(true);
            credential.SetupGet(cred => cred.HasIssuances).Returns(true);
            credential.SetupGet(cred => cred.NewestIssuance.IssuanceStatus).Returns(IssuanceStatusType.Active);
            return credential.Object;
        }

        public void GivenAllOfTheCredentialsAreActiveButDeselected()
        {
            /*
            In reality, any credentials processed for deselection should be 
            inactive. 
            */

            int numberOfCredentials = 3;
            _credentials = new List<Credential>(numberOfCredentials);

            for (int x = 0; x < numberOfCredentials; x++)
            {
                _credentials.Add(GetDeselectedCredential());
            }
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: _credentials,
                                memberId: _memberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate,
                                triggeringEvent: TriggeringEvent.Unkonwn,
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

        public void AndResultShouldBeTrue()
        {
            Result.Should().Be(true);
        }

        public void AndLogShouldContainInfoAboutNoSelectedCerts()
        {
            LogTest.Infos.Should().Contain(a => a.StartsWith($"MemberId: '{_memberId}' does not have any selected certs"));
        }
    }

    public class CorrectiveActionShouldIncludePreviouslyDeselectedButSinceReselectedCertsScenario : RunCorrectiveActionSpecScenario
    {
        private List<Credential> _credentials;
        private Guid _memberId = Guid.NewGuid();

        protected override void PreSetup()
        {
            ProcessingDate = new DateTime(DateTime.Now.Year, 02, 02);
        }


        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            ProgramRulesService.Log = Log.Object;
            LogTest.Watch(Log);
        }

        private Credential GetDeselectedCredential(bool laterReselected)
        {
            var credential = new Mock<Credential>();
            credential.SetupGet(cred => cred.DeselectionProcessed).Returns(true);
            credential.SetupGet(cred => cred.DeselectionElected).Returns(!laterReselected);
            credential.SetupGet(cred => cred.HasIssuances).Returns(true);
            credential.SetupGet(cred => cred.NewestIssuance.IssuanceStatus).Returns(IssuanceStatusType.Active);
            return credential.Object;
        }

        public void GivenAllOfTheCredentialsAreActiveButDeselected()
        {
            /*
            In reality, any credentials processed for deselection should be 
            inactive. 
            */

            int numberOfCredentials = 3;
            _credentials = new List<Credential>(numberOfCredentials);

            //First one will have been reselected
            _credentials.Add(GetDeselectedCredential(true));

            //Add remaining credentials
            for (int x = 1; x < numberOfCredentials; x++)
            {
                _credentials.Add(GetDeselectedCredential(false));
            }
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: _credentials,
                                memberId: _memberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate,
                                triggeringEvent: TriggeringEvent.Unkonwn,
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

        public void AndResultShouldBeTrue()
        {
            Result.Should().Be(true);
        }

        public void AndLogShouldNotContainInfoAboutNoSelectedCerts()
        {
            LogTest.Infos.Should().NotContain(a => a.StartsWith($"MemberId: '{_memberId}' does not have any selected certs"));
        }
    }

    public class CorrectiveActionShouldNotProcessCosponsoredCredentialsScenario : RunCorrectiveActionSpecScenario
    {
        private List<Credential> _credentials;
        private Guid _memberId = Guid.NewGuid();

        protected override void PreSetup()
        {
            ProcessingDate = new DateTime(DateTime.Now.Year, 02, 02);
        }

        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            ProgramRulesService.Log = Log.Object;
            LogTest.Watch(Log);
        }

        public void GivenAllOfTheCredentialsForThisUserAreCosponsored()
        {
            _credentials = new List<Credential>(3);
            
            for (byte x = 0; x < 3; x++)
            {
                var credential = new Mock<Credential>();
                credential.SetupGet(cred => cred.DeselectionProcessed).Returns(false);
                credential.SetupGet(cred => cred.DeselectionElected).Returns(false);
                credential.SetupGet(cred => cred.HasIssuances).Returns(true);
                credential.SetupGet(cred => cred.NewestIssuance.IssuanceStatus).Returns(IssuanceStatusType.Active);
                credential.SetupGet(cred => cred.IsCosponsored).Returns(true);
                _credentials.Add(credential.Object);
            }
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: _credentials,
                                memberId: _memberId,
                                eventDate: EventDate,
                                processingDate: ProcessingDate,
                                triggeringEvent: TriggeringEvent.Unkonwn,
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

        public void AndResultShouldBeTrue()
        {
            Result.Should().Be(true);
        }

        public void AndLogShouldContainInfoAboutCosponsoredCerts()
        {
            LogTest.Infos.Should().Contain(a => a.StartsWith($"MemberId: '{_memberId}' has only cosponsored credentials."));
        }
    }

    
    #endregion Scenarios
}
