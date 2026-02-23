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
      IWant = "to be able to utilize the ReinstateCertificate functions in Program Rules service",
      SoThat = "it can correctly reinstate credential record"
      )]
    [TestFixture]
    public class RulesforReinstateCertificateSpec
    {
        [TestCase]
        [WorkItem(136522)]
        [WorkItem(197342)]
        public void ReinstateCertificate_Success_For_TimeLimited_Issuance()
        {
            new ReinstateCertificateSuccess(DurationType.Timelimited).BDDfy();
        }

        [TestCase]
        [WorkItem(197342)]
        public void ReinstateCertificate_Success_For_Continuous_Issuance()
        {
            new ReinstateCertificateSuccess(DurationType.Continuous).BDDfy();
        }

        [TestCase]
        [WorkItem(136522)]
        public void ReinstateCertificate_Skipped_Due_To_Expiration_Date()
        {
            new ReinstateCertificateSkipped().BDDfy();
        }

        [TestCase]
        [WorkItem(197342)]
        public void ReinstateCertificate_Skipped_Due_To_Lifetime_Issuance()
        {
            new ReinstateCertificateSkipped(ReinstateCertificateSkipped.SkipReason.IssuanceType).BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class RulesforReinstateCertificateSpecScenario : ProgramRulesServiceScenario
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
        protected Tuple<DateTime?,DateTime?> LookBackEndDates {get;set;}
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
    /// The ReinstateCertificateSuccess
    /// </summary>
    public class ReinstateCertificateSuccess : RulesforReinstateCertificateSpecScenario
    {
        private DurationType _duration;

        public ReinstateCertificateSuccess(DurationType duration)
        {
            _duration = duration;
        }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(DateTime.Now.Year - 3, 8, 3);
            EventDate = new DateTime(DateTime.Now.Year, 9, 1);
            ProcessingDate = new DateTime(DateTime.Now.Year+1, 02, 02);
            LookbackDate = new DateTime(DateTime.Now.Year, 12, 31);

            //Not Expired Two and Five year look backs (not really real dates)
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(DateTime.Now.Year, 12, 31), new DateTime(DateTime.Now.Year, 12, 31));

            currentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate:new DateTime(LookBackEndDates.Item1.Value.Year-1, 1,1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy:"");

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                         .With(a => a.Code = "ABIM")
                         .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            ////Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = _duration)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = LookbackDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f=>f.LookbackDate= LookbackDate) // !!!!! 
                        .With(a=>a.GracePeriodStartDate=new DateTime(LookbackDate.Year + 1,1,1)) // !!!!!
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
            My<ILookBackDatesInfoService>().Setup(a=>a.GetLookBackDatesInfo(It.IsAny<Guid>()))
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

            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                Result = ExamResultType.Fail.ToString(),
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc),
                MemberId = new Guid(),
                CertificationId = new Guid(),
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

        public void AndThenHandleUpdateCredentialFromObjectCommand_SHOULD_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.Is<UpdateCredentialFromObjectCommand>(
                    c => c.SetGracePeriod == false && 
                    c.ReistateCertificate==true)), Times.Once());
        }
    }

    /// <summary>
    /// The ReinstateCertificateSuccess
    /// </summary>
    public class ReinstateCertificateSkipped : RulesforReinstateCertificateSpecScenario
    {
        private SkipReason _skipReason;

        public enum SkipReason
        {
            ExpirationDate, 
            IssuanceType
        }

        public ReinstateCertificateSkipped(SkipReason skipReason = SkipReason.ExpirationDate)
        {
            _skipReason = skipReason;
        }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            //****  staging data values  ++++++++++++++++++++++++++++++++++++
            CertificationId = Guid.NewGuid();
            FirstIssuanceDate = new DateTime(DateTime.Now.Year - 3, 8, 3);
            EventDate = new DateTime(DateTime.Now.Year, 9, 1);
            ProcessingDate = new DateTime(DateTime.Now.Year + 1, 02, 02);
            LookbackDate = new DateTime(DateTime.Now.Year, 12, 31);

            DateTime issuanceExpirationDate;
            DurationType issuanceDuration;

            if (_skipReason == SkipReason.ExpirationDate)
            {
                issuanceExpirationDate = FirstIssuanceDate;
                issuanceDuration = DurationType.Timelimited;
            }
            else
            {
                issuanceExpirationDate = LookbackDate;
                issuanceDuration = DurationType.Lifetime;
            }

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
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Expired)
                                .With(b => b.Duration = issuanceDuration) // !!!!!
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.ExpirationDate = issuanceExpirationDate) // !!!!!
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(f => f.LookbackDate = LookbackDate) // !!!!! 
                        .With(a => a.GracePeriodStartDate = new DateTime(LookbackDate.Year + 1, 1, 1)) // !!!!!
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

            // ++++++++++++ RegistrationInterservice ++++++++++++
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();

            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                Result = ExamResultType.Fail.ToString(),
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc),
                MemberId = new Guid(),
                CertificationId = new Guid(),
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

        public void AndThenHandleUpdateCredentialFromObjectCommand_Should_Not_BeCalled()
        {
            My<ICredentialService>()
                .Verify(o => o.Handle(It.IsAny<UpdateCredentialFromObjectCommand>()), Times.Never());
        }
    }
    #endregion
}
