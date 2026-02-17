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
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Abim.Platform.Program.Tests.Setup.Responses;
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
      IWant = "to be able to utilize the Look Back Window Evaluation in Program Rules service",
      SoThat = "it can update Look Back Dates in the Profile platform"
      )]
    [TestFixture]
    public class RulesforLookBackWindowSpec
    {

        [TestCase]
        [WorkItem(134063)]
        public void NotExpiredTwoAndFiveYearLookWindows_SkippedProcessing()
        {
            new NotExpiredTwoAndFiveYearLookWindows_SkippedProcessing().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NotExpiredTwoButExpiredFiveLookWindows_SaveExpiredLookBackDates()
        {
            new NotExpiredTwoButExpiredFiveLookWindows_SaveExpiredLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NotExpiredFiveButExpiredTwoLookWindows_SaveExpiredLookBackDates()
        {
            new NotExpiredFiveButExpiredTwoLookWindows_SaveExpiredLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_CertifiedBefore2014_SaveFirtCycleLookBackDates()
        {
            new NullCurrentLookBackWindows_CertifiedBefore2014_SaveFirtCycleLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_Scenario1_NoPoints_SaveLookBackDates()
        {
            new NullCurrentLookBackWindows_Scenario1_NoPoints_SaveLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_Scenario2_HavePoints_SaveLookBackDates()
        {
            new NullCurrentLookBackWindows_Scenario2_HavePoints_SaveLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_Scenario3_CertAfter2014_WithPoints_SaveLookBackDates()
        {
            new NullCurrentLookBackWindows_Scenario3_CertAfter2014_WithPoints_SaveLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_Scenario4_CertAfter2014_NoPoints_SaveLookBackDates()
        {
            new NullCurrentLookBackWindows_Scenario4_CertAfter2014_NoPoints_SaveLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void OldCurrentLookBackWindows_Scenario5_CertAfter2014_NoPointsThanGetPoints_SaveLookBackDates()
        {
            new OldCurrentLookBackWindows_Scenario5_CertAfter2014_NoPointsThanGetPoints_SaveLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void OldCurrentLookBackWindows_Scenario6_CertAfter2014_NoPoints_SkippedSaveLookBackDates()
        {
            new OldCurrentLookBackWindows_Scenario6_CertAfter2014_NoPoints_SkippedSaveLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_CertifiedOn2015_SaveFirstCycleLookBackDates()
        {
            new CertifiedOn2015_SaveFirstCycleLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void NullCurrentLookBackWindows_CertifiedOn2014_100Points()
        {
            new CertifiedOn2014_100Points().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedOn2014_80Points()
        {
            new CertifiedOn2014_80Points().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedOn2014_NoPoints()
        {
            new CertifiedOn2014_NoPoints().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedOn2019_NoPoints()
        {
            new CertifiedOn2019_NoPoints().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedOn2019_100Points()
        {
            new CertifiedOn2019_100Points().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedBefore2014_NoPoints_Processing2019_SaveFirtCycleLookBackDates()
        {
            new CertifiedBefore2014_NoPoints_Processing2019_SaveFirtCycleLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedBefore2014_NoPoints_Processing2020_SaveFirtCycleLookBackDates()
        {
            new CertifiedBefore2014_NoPoints_Processing2020_SaveFirtCycleLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedBefore2014_NoPoints_Processing2021_SaveFirtCycleLookBackDates()
        {
            new CertifiedBefore2014_NoPoints_Processing2021_SaveFirtCycleLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedBefore2014_NoPoints_Processing2029_SaveFirtCycleLookBackDates()
        {
            new CertifiedBefore2014_NoPoints_Processing2029_SaveFirtCycleLookBackDates().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void CertifiedBefore2014_NoPoints_Processing2028_SaveFirtCycleLookBackDates()
        {
            new CertifiedBefore2014_NoPoints_Processing2028_SaveFirtCycleLookBackDates().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class LookBackWindowSpecScenario : ProgramRulesServiceScenario
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
        protected Tuple<DateTime?, DateTime?> LookBackEndDates { get; set; }
        protected LookBackDatesInfo currentLookBackDatesInfo { get; set; }
        protected LookBackDatesInfo expectedLookBackDatesInfo { get; set; }
        // builders
        protected IssuanceDataBuilder IssuanceDataBuilder { get; set; }
        protected SourceDataBuilder SourceDataBuilder { get; set; }
        protected CertificationDataBuilder CertificationDataBuilder { get; set; }
        protected CredentialDataBuilder CredentialDataBuilder { get; set; }

        protected DateTime Lookback2YearStartDate = new DateTime();
        protected DateTime Lookback2YearEndDate = new DateTime();
        protected DateTime Lookback5YearStartDate = new DateTime();
        protected DateTime Lookback5YearEndDate = new DateTime();

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
    /// The NotExpiredTwoAndFiveYearLookWindows_SkippedProcessing
    /// </summary>
    public class NotExpiredTwoAndFiveYearLookWindows_SkippedProcessing : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2010, 8, 3);
            EventDate = new DateTime(2020, 8, 1);
            ProcessingDate = new DateTime(2020, 8, 3);



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

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                            creditDate: DateTime.Now,
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);
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

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Never());
        }
        //--------------------------------------------------
        public void AndThenHandleUpdateLookBackDate_SHOULD_NOT_BeCalled()
        {
            My<ILookBackDatesInfoService>()
                .Verify(o => o.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The NotExpiredFiveButExpiredTwoLookWindows_SaveLookBackDates
    /// </summary>
    public class NotExpiredFiveButExpiredTwoLookWindows_SaveExpiredLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2010, 8, 3);
            EventDate = new DateTime(2020, 8, 1);
            ProcessingDate = new DateTime(2020, 8, 3);

            //Not Expired Five but Expired Two year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(ProcessingDate.Year - 1, 12, 31), new DateTime(ProcessingDate.Year + 1, 12, 31));

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(FirstIssuanceDate.Year + 10, 1, 1),
                                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 11, 12, 31),
                                lookback5YearStartDate: null,
                                lookback5YearEndDate: null,
                                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddDays(-5),
                                                            creditDate: ProcessingDate.AddDays(-5),
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThenBusPublishUpdateLookBackDatesInfoCommandsEventWithProperValue_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate
                  )), Times.Once());
        }

    }

    /// <summary>
    /// The NotExpiredFiveButExpiredTwoLookWindows_SaveLookBackDates
    /// </summary>
    public class NotExpiredTwoButExpiredFiveLookWindows_SaveExpiredLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2010, 8, 3);
            EventDate = new DateTime(2020, 8, 1);
            ProcessingDate = new DateTime(2020, 8, 3);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(ProcessingDate.Year + 1, 12, 31), new DateTime(ProcessingDate.Year - 1, 12, 31));
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: null,
                                lookback2YearEndDate: null,
                                lookback5YearStartDate: new DateTime(FirstIssuanceDate.Year + 9, 1, 1),
                                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 13, 12, 31),
                                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddDays(-5),
                                                            creditDate: ProcessingDate.AddDays(-5),
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class NullCurrentLookBackWindows_CertifiedBefore2014_SaveFirtCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2008, 8, 3);
            ProcessingDate = new DateTime(2020, 03, 25); ///2019 to-do

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2014, 1, 1),
                lookback2YearEndDate: new DateTime(2015, 12, 31),
                lookback5YearStartDate: new DateTime(2014, 1, 1),
                lookback5YearEndDate: new DateTime(2018, 12, 31),
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_Scenario2_SaveLookBackDates
    /// </summary>
    public class NullCurrentLookBackWindows_Scenario1_NoPoints_SaveLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++

            CertificationId = Guid.NewGuid();
            //---- FirstIssuanceDate before 2014
            FirstIssuanceDate = new DateTime(2015, 8, 3);

            //--- Event Date is 2018/12/31 ****
            EventDate = new DateTime(2018, 12, 31);
            ProcessingDate = new DateTime(2019, 01, 01);
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);

            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: FirstIssuanceDate,
                                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 2, 12, 31),
                                lookback5YearStartDate: FirstIssuanceDate,
                                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                                createdBy: "");

            //-------------------------------------------------------------------------------
            //DateTime EarnedPoints = new DateTime(2015, 09, 09); // met 2014-2015 && 2014-2018
            //---------------------------------------------------------------------------------

            // 2 year look back: 2014-2015 
            Lookback2YearStartDate = new DateTime(2014, 01, 01);
            Lookback2YearEndDate = new DateTime(2015, 12, 31);

            // 5 year look back: 2014-2018 met
            Lookback5YearStartDate = new DateTime(2014, 01, 01);
            Lookback5YearEndDate = new DateTime(2018, 12, 31);

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            //Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EarnedPoints,
            //                                                creditDate: EarnedPoints,
            //                                                totalMOCPoints: 100,
            //                                                medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class NullCurrentLookBackWindows_Scenario2_HavePoints_SaveLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++

            CertificationId = Guid.NewGuid();
            //---- FirstIssuanceDate before 2014
            FirstIssuanceDate = new DateTime(2010, 8, 3);

            //--- Event Date is 2018/12/31 ****
            EventDate = new DateTime(2018, 12, 31);
            ProcessingDate = new DateTime(2019, 01, 01);
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: null,
                                lookback2YearEndDate: null,
                                lookback5YearStartDate: null,
                                lookback5YearEndDate: null,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                        memberId: Guid.NewGuid(),
                                        // 2 year look back: 2014-2015 met, set to next one 2016-2017
                                        lookback2YearStartDate: new DateTime(2016, 01, 01),
                                        lookback2YearEndDate: new DateTime(2017, 12, 31),
                                        // 5 year look back: 2014-2018 met, set to next one 2019-2023
                                        lookback5YearStartDate: new DateTime(2019, 01, 01),
                                        lookback5YearEndDate: new DateTime(2023, 12, 31),
                                        createdBy: "");

            DateTime EarnedPoints = new DateTime(2015, 09, 09); // met 2014-2015 && 2014-2018

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EarnedPoints,
                                                            creditDate: EarnedPoints,
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(),It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class NullCurrentLookBackWindows_Scenario3_CertAfter2014_WithPoints_SaveLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++

            CertificationId = Guid.NewGuid();
            //---- FirstIssuanceDate after 2014
            FirstIssuanceDate = new DateTime(2015, 8, 3);

            //--- Event Date is 2018/12/31 ****
            EventDate = new DateTime(2018, 12, 31);
            ProcessingDate = new DateTime(2019, 01, 01);

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: null,
                                lookback2YearEndDate: null,
                                lookback5YearStartDate: null,
                                lookback5YearEndDate: null,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                // 2 year look back: met 1 cycle (first issuance 2015/08/03-2017/12/31, current 2018/01/01/-2019/12/31
                                lookback2YearStartDate: new DateTime(2018, 1, 1),
                                lookback2YearEndDate: new DateTime(2019, 12, 31),
                                // 5 year look back: still in first cycle since today date is 2019 2015/08/03-2020/12/31
                                lookback5YearStartDate: new DateTime(2015, 8, 3),
                                lookback5YearEndDate: new DateTime(2020, 12, 31),
                                createdBy: "");

            DateTime EarnedPoints = new DateTime(2015, 09, 09); // 

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EarnedPoints,
                                                            creditDate: EarnedPoints,
                                                            totalMOCPoints: 100,
                                                            medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class NullCurrentLookBackWindows_Scenario4_CertAfter2014_NoPoints_SaveLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++

            CertificationId = Guid.NewGuid();
            //---- FirstIssuanceDate after 2014
            FirstIssuanceDate = new DateTime(2015, 8, 3);

            //--- Event Date is 2018/12/31 ****
            EventDate = new DateTime(2018, 12, 31);
            ProcessingDate = new DateTime(2019, 01, 01);

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: null,
                                lookback2YearEndDate: null,
                                lookback5YearStartDate: null,
                                lookback5YearEndDate: null,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: FirstIssuanceDate,
                                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 2, 12, 31),
                                lookback5YearStartDate: FirstIssuanceDate,
                                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                                createdBy: "");

            DateTime EarnedPoints = new DateTime(2015, 09, 09); // 
            int Points = 0;

            // 2 year look back: current 2018/01/01/-2019/12/31
            Lookback2YearStartDate = new DateTime(2015, 8, 3);
            Lookback2YearEndDate = new DateTime(2017, 12, 31);

            // 5 year look back: still in first cycle since today date is 2019 2015/08/03-2020/12/31
            Lookback5YearStartDate = new DateTime(2015, 8, 3);
            Lookback5YearEndDate = new DateTime(2020, 12, 31);

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            if (Points > 0)
                Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EarnedPoints,
                                                                creditDate: EarnedPoints,
                                                                totalMOCPoints: 100,
                                                                medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }


    }

    /// <summary>
    /// The OldCurrentLookBackWindows_Scenario5_CertAfter2014_NoPointsThanGetPoints_SaveLookBackDates
    /// </summary>
    public class OldCurrentLookBackWindows_Scenario5_CertAfter2014_NoPointsThanGetPoints_SaveLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++

            CertificationId = Guid.NewGuid();
            //---- FirstIssuanceDate after 2014
            FirstIssuanceDate = new DateTime(2015, 8, 3);

            //--- Event Date is 2018/12/31 ****
            EventDate = new DateTime(2022, 09, 13);
            ProcessingDate = new DateTime(2022, 11, 01);
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(2017, 12, 31), new DateTime(2020, 12, 31));

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(2022, 1, 1),
                                lookback2YearEndDate: new DateTime(2023, 12, 31),
                                lookback5YearStartDate: new DateTime(2021, 01, 01),
                                lookback5YearEndDate: new DateTime(2025, 12, 31),
                                createdBy: "");

            DateTime EarnedPoints = new DateTime(2022, 09, 13);
            int Points = 100;

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            if (Points > 0)
                Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EarnedPoints,
                                                                creditDate: EarnedPoints,
                                                                totalMOCPoints: 100,
                                                                medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The OldCurrentLookBackWindows_Scenario6_CertAfter2014_NoPoints_SaveLookBackDates
    /// </summary>
    public class OldCurrentLookBackWindows_Scenario6_CertAfter2014_NoPoints_SkippedSaveLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++

            CertificationId = Guid.NewGuid();
            //---- FirstIssuanceDate after 2014
            FirstIssuanceDate = new DateTime(2015, 8, 3);

            //--- Event Date is 2018/12/31 ****
            ProcessingDate = new DateTime(2022, 11, 01);
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(2017, 12, 31), new DateTime(2020, 12, 31));

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");

            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: FirstIssuanceDate,
                                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 2, 12, 31),
                                lookback5YearStartDate: FirstIssuanceDate,
                                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                                createdBy: "");

            DateTime EarnedPoints = new DateTime(2022, 09, 13);
            int Points = 0;

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

            if (Points > 0)
                Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EarnedPoints,
                                                                creditDate: EarnedPoints,
                                                                totalMOCPoints: 100,
                                                                medicalKnowledgePoints: 20);
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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_Not_BeCalled()
        {
            My<ILookBackDatesInfoService>()
                .Verify(o => o.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()), Times.Never());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedOn2015_SaveFirstCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2015, 8, 3);
            ProcessingDate = new DateTime(2020, 03, 25);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: FirstIssuanceDate.AddDays(1),
                                                creditDate: new DateTime(2014, 8, 3),
                                                totalMOCPoints: 100,
                                                medicalKnowledgePoints: 20);

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2018, 1, 1),
                lookback2YearEndDate: new DateTime(2019, 12, 31),
                lookback5YearStartDate: FirstIssuanceDate,
                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                createdBy: "");

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");



            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);


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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedOn2014_100Points : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2014, 8, 3);
            ProcessingDate = new DateTime(2020, 03, 25);

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2017, 01, 01),
                lookback2YearEndDate: new DateTime(2018, 12, 31),
                lookback5YearStartDate: new DateTime(2020, 01, 01),
                lookback5YearEndDate: new DateTime(2024, 12, 31),
                createdBy: "");

            // points at first cycle
            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: FirstIssuanceDate.AddDays(1),
                                                creditDate: new DateTime(2014, 8, 3),
                                                totalMOCPoints: 100,
                                                medicalKnowledgePoints: 20);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedOn2014_80Points : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2014, 8, 3);
            ProcessingDate = new DateTime(2020, 03, 25);

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2017, 01, 01),
                lookback2YearEndDate: new DateTime(2018, 12, 31),
                lookback5YearStartDate: FirstIssuanceDate,
                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                createdBy: "");

            // points at first cycle
            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: FirstIssuanceDate.AddDays(1),
                                                creditDate: new DateTime(2014, 8, 3),
                                                totalMOCPoints: 80,
                                                medicalKnowledgePoints: 20);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedOn2014_NoPoints : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2014, 8, 3);
            ProcessingDate = new DateTime(2020, 03, 25);

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: FirstIssuanceDate,
                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 2, 12, 31),
                lookback5YearStartDate: FirstIssuanceDate,
                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                createdBy: "");

            // No points

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedOn2019_NoPoints : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2019, 8, 3);
            ProcessingDate = new DateTime(2019, 08, 03);

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: FirstIssuanceDate,
                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 2, 12, 31),
                lookback5YearStartDate: FirstIssuanceDate,
                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                createdBy: "");

            // No points

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedOn2019_100Points : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2019, 8, 3);
            ProcessingDate = new DateTime(2021, 08, 03);

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: FirstIssuanceDate,
                lookback2YearEndDate: new DateTime(FirstIssuanceDate.Year + 2, 12, 31), // still stay on first cycle because of processing date still in 2021
                lookback5YearStartDate: FirstIssuanceDate,
                lookback5YearEndDate: new DateTime(FirstIssuanceDate.Year + 5, 12, 31),
                createdBy: "");

            // points at first cycle
            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: FirstIssuanceDate.AddDays(1),
                                                creditDate: new DateTime(2014, 8, 3),
                                                totalMOCPoints: 100,
                                                medicalKnowledgePoints: 20);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedBefore2014_NoPoints_Processing2019_SaveFirtCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2008, 8, 3);
            ProcessingDate = new DateTime(2019, 03, 25);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2014, 1, 1),
                lookback2YearEndDate: new DateTime(2015, 12, 31),
                lookback5YearStartDate: new DateTime(2014, 1, 1),
                lookback5YearEndDate: new DateTime(2018, 12, 31),
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedBefore2014_NoPoints_Processing2020_SaveFirtCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2008, 8, 3);
            ProcessingDate = new DateTime(2020, 03, 25);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2014, 1, 1),
                lookback2YearEndDate: new DateTime(2015, 12, 31),
                lookback5YearStartDate: new DateTime(2014, 1, 1),
                lookback5YearEndDate: new DateTime(2018, 12, 31),
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedBefore2014_NoPoints_Processing2021_SaveFirtCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2008, 8, 3);
            ProcessingDate = new DateTime(2021, 03, 25);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2014, 1, 1),
                lookback2YearEndDate: new DateTime(2015, 12, 31),
                lookback5YearStartDate: new DateTime(2014, 1, 1),
                lookback5YearEndDate: new DateTime(2018, 12, 31),
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedBefore2014_NoPoints_Processing2029_SaveFirtCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2008, 8, 3);
            ProcessingDate = new DateTime(2029, 03, 25);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2014, 1, 1),
                lookback2YearEndDate: new DateTime(2015, 12, 31),
                lookback5YearStartDate: new DateTime(2014, 1, 1),
                lookback5YearEndDate: new DateTime(2018, 12, 31),
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }

    /// <summary>
    /// The NullCurrentLookBackWindows_SaveLookBackDates
    /// </summary>
    public class CertifiedBefore2014_NoPoints_Processing2028_SaveFirtCycleLookBackDates : LookBackWindowSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(2008, 8, 3);
            ProcessingDate = new DateTime(2028, 03, 25);

            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(null, null);
            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: null,
                lookback2YearEndDate: null,
                lookback5YearStartDate: null,
                lookback5YearEndDate: null,
                createdBy: "");

            // original dates for someone who was certified before 2014
            expectedLookBackDatesInfo = LookBackDatesInfo.Create(
                memberId: Guid.NewGuid(),
                lookback2YearStartDate: new DateTime(2014, 1, 1),
                lookback2YearEndDate: new DateTime(2015, 12, 31),
                lookback5YearStartDate: new DateTime(2014, 1, 1),
                lookback5YearEndDate: new DateTime(2018, 12, 31),
                createdBy: "");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .Build();

            Credential.AddIssuance(issuance);

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
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            //// ++++++++++++ LookBackDatesInfo Service ++++++++++++
            My<ILookBackDatesInfoService>().Setup(a => a.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(currentLookBackDatesInfo));

            My<ILookBackDatesInfoService>().Setup(a => a.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                .Returns(Task.FromResult(false));

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>()
                .Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new RegistrationFullCollectionResource()));

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

        public void AndThenGetFirstIssuanceDate_SHOULD_BeCalled()
        {
            My<ICredentialService>().Verify(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once());
        }

        //????????????????????????????????
        public void AndThen_HandleUpdateLookBackDatesInfoCommandCommand_SHOULD_BeCalled()
        {
            My<ILookBackDatesInfoService>()
              .Verify(o => o.Handle(It.Is<UpdateLookBackDatesInfoCommand>(
                  a => a.Lookback2YearStartDate == expectedLookBackDatesInfo.Lookback2YearStartDate
                  && a.Lookback2YearEndDate == expectedLookBackDatesInfo.Lookback2YearEndDate
                  && a.Lookback5YearStartDate == expectedLookBackDatesInfo.Lookback5YearStartDate
                  && a.Lookback5YearEndDate == expectedLookBackDatesInfo.Lookback5YearEndDate)), Times.Once());
        }

    }
    #endregion
}
