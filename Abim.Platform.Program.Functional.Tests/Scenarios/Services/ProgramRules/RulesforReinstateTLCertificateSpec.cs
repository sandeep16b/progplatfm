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
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
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
      IWant = "to be able to utilize the CorrectiveActionReinstateTLCert functions in Program Rules service",
      SoThat = "it can correctly reinstate TL credential record"
      )]
    [TestFixture]
    public class RulesforReinstateTLCertificateSpec
    {
        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearOK_Reinstate()
        {
            new ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearOK_Reinstate().BDDfy();
        }

        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearNotOK_Skipped()
        {
            new ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearNotOK_Skipped().BDDfy();
        }

        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_InGracePeriod_Reinstate()
        {
            new ReinstateTLCertificate_InGracePeriod_Reinstate().BDDfy();
        }

        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_HasPendingExam_Reinstate()
        {
            new ReinstateTLCertificate_HasPendingExam_Reinstate().BDDfy();
        }

        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_DontMeetExamReq_5yearOK_Skipped()
        {
            new ReinstateTLCertificate_DontMeetExamReq_5yearOK_Skipped().BDDfy();
        }

        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_ICARD_AllMeet_Reinstate()
        {
            new ReinstateTLCertificate_ICARD_AllMeet_Reinstate().BDDfy();
        }

        [TestCase]
        [WorkItem(142211)]
        public void ReinstateTLCertificate_ICARD_NoAttestation_Skipped()
        {
            new ReinstateTLCertificate_ICARD_NoAttestation_Skipped().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class RulesforReinstateTLCertificateSpecScenario : ProgramRulesServiceScenario
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
        protected DateTime? LookbackDate { get; set; }
        protected DateTime ExpirationDate { get; set; }
        protected DateTime ExpiredDate { get; set; }
        protected bool AssessmentMet { get; set; }
        protected DateTime? GracePeriodStartDate { get; set; }
        protected DateTime? GracePeriodEndDate { get; set; }
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

        protected void Condition_Attestation(DateTime completedDate, string ProductCode)
        {
            ActivityResource activity = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductCode },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = completedDate,
                TotalMOCPoints = 0,
                ActivityCredits = null
            };

            AddToUserActivities(activity);
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
    /// The ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearOK_Reinstate
    /// </summary>
    public class ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearOK_Reinstate : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            //CertificationId = Guid.NewGuid();

            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = new DateTime(2018, 12, 31);
            ExpirationDate= new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = true;

            ////Not Expired Two and Five year look backs (not really real dates)
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

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = null) // !!!!!
                        .With(a => a.GracePeriodEndDate = null) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
                                                            medicalKnowledgePoints: 20);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EventDate,
                                                creditDate: EventDate,
                                                totalMOCPoints: 20,
                                                medicalKnowledgePoints: 20);

            LongitudinalEnrollments = new List<LongitudinalEnrollmentSummaryResource>();

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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                    .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();
            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            // ++++++++++++++++++++++++++++++
            // NoConsequence KCI Fail exam
            // ++++++++++++++++++++++++++++++
            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                NoConsequence = true, // !!!!!
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = EventDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource() { Data = LongitudinalEnrollments }));

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

        public void AndThenHandleReinstateTLCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()),Times.Once);
        }
    }

    /// <summary>
    /// The ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearNotOK_Skipped
    /// </summary>
    public class ReinstateTLCertificate_AssessmentMetByFailKciNoConsequence_5yearNotOK_Skipped : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            //CertificationId = Guid.NewGuid();

            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = new DateTime(2018, 12, 31);
            ExpirationDate = new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = true;

            ////Not Expired Two and Five year look backs (not really real dates)
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

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = null) // !!!!!
                        .With(a => a.GracePeriodEndDate = null) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                    .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();
            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            // ++++++++++++++++++++++++++++++
            // NoConsequence KCI Fail exam
            // ++++++++++++++++++++++++++++++
            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                NoConsequence = true, // !!!!!
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = EventDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
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

        public void AndThenHandleReinstateTLCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()), Times.Never);
        }
    }

    /// <summary>
    /// The ReinstateTLCertificate_InGracePeriod_Reinstate
    /// </summary>
    public class ReinstateTLCertificate_InGracePeriod_Reinstate : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = null; // not really real situation, but OK for unit testing
            ExpirationDate = new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = false;
            GracePeriodStartDate = new DateTime(2019, 1, 1);
            GracePeriodEndDate = new DateTime(2019, 12, 31);

            ////Not Expired Two and Five year look backs (not really real dates)
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

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = GracePeriodStartDate) // !!!!!
                        .With(a => a.GracePeriodEndDate = GracePeriodEndDate) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
                                                            medicalKnowledgePoints: 20);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EventDate,
                                                creditDate: EventDate,
                                                totalMOCPoints: 20,
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) }));

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

        public void AndThenHandleReinstateTLCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()), Times.Once);
        }
    }

    /// <summary>
    /// The ReinstateTLCertificate_HasPendingExam_Reinstate
    /// </summary>
    public class ReinstateTLCertificate_HasPendingExam_Reinstate : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = new DateTime(2018,12,31); // not really real situation, but OK for unit testing
            ExpirationDate = new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = false;
            GracePeriodStartDate = null;
            GracePeriodEndDate = null;

            ////Not Expired Two and Five year look backs (not really real dates)
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

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = GracePeriodStartDate) // !!!!!
                        .With(a => a.GracePeriodEndDate = GracePeriodEndDate) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
                                                            medicalKnowledgePoints: 20);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EventDate,
                                                creditDate: EventDate,
                                                totalMOCPoints: 20,
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();

            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            // ++++++++++++++++++++++++++++++
            // Pending KCI exam
            // ++++++++++++++++++++++++++++++
            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Pending) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                NoConsequence = false,
                Result = ExamResultType.Pending.ToString(),
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = LookbackDate.Value.AddDays(-60) } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
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

        public void AndThenHandleReinstateTLCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()), Times.Once);
        }
    }

    /// <summary>
    /// The ReinstateTLCertificate_DontMeetExamReq_5yearOK_Skipped
    /// </summary>
    public class ReinstateTLCertificate_DontMeetExamReq_5yearOK_Skipped : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = null; // not really real situation, but OK for unit testing
            ExpirationDate = new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = false;
            GracePeriodStartDate = null;
            GracePeriodEndDate = null;

            ////Not Expired Two and Five year look backs (not really real dates)
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

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = GracePeriodStartDate) // !!!!!
                        .With(a => a.GracePeriodEndDate = GracePeriodEndDate) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
                                                            medicalKnowledgePoints: 20);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EventDate,
                                                creditDate: EventDate,
                                                totalMOCPoints: 20,
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) } ));

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

        public void AndThenHandleReinstateTLCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()), Times.Never);
        }
    }

    /// <summary>
    /// The ReinstateTLCertificate_ICARD_AllMeet_Reinstate
    /// </summary>
    public class ReinstateTLCertificate_ICARD_AllMeet_Reinstate : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = null; // not really real situation, but OK for unit testing
            ExpirationDate = new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = true;
            GracePeriodStartDate = null;
            GracePeriodEndDate = null;

            ////Not Expired Two and Five year look backs (not really real dates)
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
            certification.Code = "ICARD";

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = GracePeriodStartDate) // !!!!!
                        .With(a => a.GracePeriodEndDate = GracePeriodEndDate) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
                                                            medicalKnowledgePoints: 20);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EventDate,
                                                creditDate: EventDate,
                                                totalMOCPoints: 20,
                                                medicalKnowledgePoints: 20);

            Condition_Attestation(EventDate.AddYears(-2), ProductResourceConstants.ProductCode.ICARDAttestMOC);

            LongitudinalEnrollments = new List<LongitudinalEnrollmentSummaryResource>();
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();
            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            // ++++++++++++++++++++++++++++++
            // NoConsequence KCI Fail exam
            // ++++++++++++++++++++++++++++++
            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                NoConsequence = true, // !!!!!
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = EventDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource() { Data = LongitudinalEnrollments }));

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

        public void AndThenHandleReinstateTLCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()), Times.Once);
        }
    }

    /// <summary>
    /// The ReinstateTLCertificate_ICARD_NoAttestation_Skipped
    /// </summary>
    public class ReinstateTLCertificate_ICARD_NoAttestation_Skipped : RulesforReinstateTLCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            FirstIssuanceDate = new DateTime(2008, 11, 15);
            EventDate = new DateTime(2019, 1, 13); //completed activity and now have 100 points
            ProcessingDate = new DateTime(2019, 02, 02);
            LookbackDate = null; // not really real situation, but OK for unit testing
            ExpirationDate = new DateTime(2018, 12, 31);
            ExpiredDate = new DateTime(2018, 12, 31);
            AssessmentMet = true;
            GracePeriodStartDate = null;
            GracePeriodEndDate = null;

            ////Not Expired Two and Five year look backs (not really real dates)
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
            certification.Code = "ICARD";

            CertificationId = certification.ExternalId;

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = ExpirationDate) // !!!!!
                                .With(b => b.ExpiredDate = ExpiredDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.AssessmentMet = AssessmentMet)
                        .With(a => a.AssessmentMetDate = FirstIssuanceDate)
                        .With(a => a.GracePeriodStartDate = GracePeriodStartDate) // !!!!!
                        .With(a => a.GracePeriodEndDate = GracePeriodEndDate) // !!!!!
                        .Build();

            Credential.AddIssuance(issuance);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: ProcessingDate.AddYears(-2),
                                                            creditDate: ProcessingDate.AddYears(-2),
                                                            totalMOCPoints: 80,
                                                            medicalKnowledgePoints: 20);

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: EventDate,
                                                creditDate: EventDate,
                                                totalMOCPoints: 20,
                                                medicalKnowledgePoints: 20);

            //Condition_Attestation(EventDate.AddYears(-2), ProductResourceConstants.ProductCode.ICARDAttestMOC);
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

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ReinstateTLCommand>()))
                .Returns(new ReinstateTLCommandResult(CommandStatus.Accepted, null, null));

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
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();
            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            // ++++++++++++++++++++++++++++++
            // NoConsequence KCI Fail exam
            // ++++++++++++++++++++++++++++++
            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                NoConsequence = true, // !!!!!
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = EventDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
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

        public void AndThenHandleReinstateTLCommand_SHOULD_NOT_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<ReinstateTLCommand>()), Times.Never);
        }
    }

    #endregion
}
