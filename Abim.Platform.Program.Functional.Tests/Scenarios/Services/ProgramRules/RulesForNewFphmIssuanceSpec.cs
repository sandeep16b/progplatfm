using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Extensions.ExternalResponses;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
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
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to run Print Grand Father certificate"
      )]
    [TestFixture]
    public class RulesForNewFphmIssuanceSpec
    {
        [TestCase]
        [WorkItem(131139)]
        public void Create_Fphm_Issuance_KciExam_WithinTwoYears_LastKciWith_NoConsequence()
        {
            new Do_Not_Create_Fphm_Issuance_KciExam_WithinTwoYears_Failed_With_No_Consequence().BDDfy();
        }

        [TestCase]
        [WorkItem(131139)]
        public void Create_Fphm_Issuance_KciExam_WithinTwoYears_LastKciWithConsequence()
        {
            new Create_Fphm_Issuance_KciExam_WithinTwoYears_Failed_With_Consequence().BDDfy();
        }

        
        [TestCase]
        [WorkItem(135744)]
        public void ShouldMeet_FphmFiveYearLookbackRequirement_With100MOCPoints()
        {
            new ShouldMeetFphmFiveYearLookbackRequirementWith100MOCPoints().BDDfy();
        }
        
        [TestCase]
        [WorkItem(135744)]
        public void ShouldMeet_FphmFiveYearLookbackRequirement_WithLessThan100MOCPoints_InReciprocity()
        {
            new ShouldMeetFphmFiveYearLookbackRequirementWithLessThan100MOCPointsInReciprocity().BDDfy();
        }
        
        [TestCase]
        [WorkItem(135744)]
        public void ShouldMeet_FphmFiveYearLookbackRequirement_WithLessThan100MOCPoints_NotInReciprocity_InSubscpecialty()
        {
            new ShouldMeetFphmFiveYearLookbackRequirementWithLessThan100MOCPointsNotInReciprocityInSubscpecialty().BDDfy();
        }
        
    }

    public abstract class RulesForNewFphmIssuanceSpecScenario : ProgramRulesServiceScenario
    {
        protected Guid CredentialId { get; set; }

        protected Guid CertificationId { get; set; }

        protected int IssuanceId { get; set; }
        protected ProgramRulesService ProgramRulesService { get; set; }
        protected App.Services.Impl.CertificationService CertificationService { get; set; }
        protected new Exception ExceptionCaught { get; set; }

        protected bool Result { get; set; }
        protected ActivityFullCollectionResource ActivitiesFullCollectionResource { get; set; }

        protected List<Credential> CredentialCollection { get; set; }

        protected RunRulesForMustBeMaintainedCertificateCommand Command { get; set; }

        protected int Year = DateTime.Now.Year;

        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            types.Add(typeof(ICorrectiveActionResultService));
            types.Add(typeof(ICertificationRepository));
            return types;
        }

        protected Credential CreateGFCredentialWithIssuance(DateTime issuanceDate, Guid certificationId)
        {
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            source.Code = "ABIM";

            var certification = Certification.Create(null, source, CertificationType.Primary, RandomString.Build(), RandomString.Build(), RandomString.Build());
            certification.Code = "IM";
            certification.ExternalId = certificationId;

            var credential = Credential.Create(certification, Guid.NewGuid(), CredentialType.General, EnumAttributes.RandomEntry<Resources.PathwayType>(),
                null, null, RandomString.Build());
            //create GF cert
            var issuance = Issuance.Create(source,
                                    DurationType.Lifetime,
                                    MaintenanceRequirementType.NotRequired,
                                    EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                                    OccurrenceType.Initial,
                                    IssuanceStatusType.Active,
                                    issuanceDate,
                                    RandomString.Build());

            IssuanceId = issuance.Id;

            credential.AddIssuance(issuance);
            return credential;
        }

        protected Credential CreateFPHMCredentialWithNoIssuance(DateTime issuanceDate, Guid certificationId)
        {
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            source.Code = "ABIM";

            var certification = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(), RandomString.Build(), RandomString.Build());
            certification.Code = "FPHM";
            certification.ExternalId = certificationId;

            var credential = Credential.Create(certification, Guid.NewGuid(), CredentialType.General, EnumAttributes.RandomEntry<Resources.PathwayType>(),
                null, null, RandomString.Build());

            credential.AssessmentMet = true;
            credential.AssessmentMetDate = DateTime.Today.AddMonths(-24);

            return credential;
        }

        protected Credential CreateFPHMCredentialWithIssuance(DateTime issuanceDate, Guid certificationId)
        {
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            source.Code = "ABIM";

            var certification = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(), RandomString.Build(), RandomString.Build());
            certification.Code = "FPHM";
            certification.ExternalId = certificationId;

            var credential = Credential.Create(certification, Guid.NewGuid(), CredentialType.General, EnumAttributes.RandomEntry<Resources.PathwayType>(),
                null, null, RandomString.Build());

            credential.AssessmentMet = true;
            credential.AssessmentMetDate = DateTime.Today.AddMonths(-24);

            var issuance = Issuance.Create(source,
                                   DurationType.Lifetime,
                                   MaintenanceRequirementType.NotRequired,
                                   EnumAttributes.RandomEntry<MaintenanceStatusType>(),
                                   OccurrenceType.Initial,
                                   IssuanceStatusType.Active,
                                   issuanceDate,
                                   RandomString.Build());

            IssuanceId = issuance.Id;

            credential.AddIssuance(issuance);

            return credential;
        }


        protected void AddToUserActivities(ActivityResource activityResource)
        {
            ActivitiesFullCollectionResource.Data.Add(activityResource);
        }

        #region Conditions
        
        protected void Condition_Completed_Fphm_Initial_Attestation(DateTime completedDate)
        {
            ActivityResource activityResourceReciprocity = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.FPHMAttestInitial },
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
        protected void Condition_RegistrationInterservice_GetUserRegistrations_OnePass_OneFailedLatest(bool noConsequence)
        {
            var output = new UserRegistrationsAndCMPRegistrationsResource();
            output.Registrations = new List<RegistrationResource>(2);
            output.CMPRegistrations = new List<CMPRegistrationResource>(0);

            //Exam passed greater than 10 years ago
            output.Registrations.Add(new RegistrationResource()
            {
                Result = ExamResultType.Pass.ToString(),
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Pass) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = new DateTime(Year - 12, 01, 11) } }
            });

            //Kci exam within the last 2 years
            output.Registrations.Add(new RegistrationResource()
            {
                Result = ExamResultType.Fail.ToString(),
                ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Fail) },
                ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci),
                MemberId = new Guid(),
                CertificationId = CertificationId,
                NoConsequence = noConsequence,
                AdministrationDate = new DateTime(Year - 1, 01, 11),
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = new DateTime(Year - 1, 01, 11) } }
            });
            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(output));
        }
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

        #endregion
    }

    public class Do_Not_Create_Fphm_Issuance_KciExam_WithinTwoYears_Failed_With_No_Consequence : RulesForNewFphmIssuanceSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = Guid.NewGuid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            CertificationService = Container.GetInstance<App.Services.Impl.CertificationService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(credential);

            var cred2 = CreateFPHMCredentialWithNoIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(cred2);


            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_Completed_Fphm_Initial_Attestation(completedDate: DateTime.Now);

            Condition_RegistrationInterservice_GetUserRegistrations_OnePass_OneFailedLatest(noConsequence:true);

            Condition_InReciprocityProgram(DateTime.Now);
            
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var fphmCert = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(),
                RandomString.Build(), RandomString.Build());

            fphmCert.ExternalId = CertificationId; 

            My<ICertificationService>().Setup(c => c.GetByCode(It.IsAny<string>()))
                .Returns(fphmCert);


            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))  
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
               .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: CredentialCollection,
                                memberId: new Guid(),
                                eventDate: new DateTime(DateTime.Now.Year, 9, 1),
                                processingDate: DateTime.Now).Result;
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never);
        }
    }



    public class Create_Fphm_Issuance_KciExam_WithinTwoYears_Failed_With_Consequence : RulesForNewFphmIssuanceSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = Guid.NewGuid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            CertificationService = Container.GetInstance<App.Services.Impl.CertificationService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(credential);

            var cred2 = CreateFPHMCredentialWithNoIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(cred2);


            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_Completed_Fphm_Initial_Attestation(completedDate: DateTime.Now);
            
            Condition_RegistrationInterservice_GetUserRegistrations_OnePass_OneFailedLatest(noConsequence: false);

            Condition_InReciprocityProgram(DateTime.Now);

            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var fphmCert = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(),
                RandomString.Build(), RandomString.Build());

            fphmCert.ExternalId = CertificationId;

            My<ICertificationService>().Setup(c => c.GetByCode(It.IsAny<string>()))
                .Returns(fphmCert);


            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
               .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
        }

        public void WhenICallRunCorrectiveAction()
        {
            try
            {
                Result = ProgramRulesService.RunCorrectiveAction(
                                credentialsIn: CredentialCollection,
                                memberId: new Guid(),
                                eventDate: new DateTime(DateTime.Now.Year, 9, 1),
                                processingDate: DateTime.Now).Result;
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

        public void AndThenIssueFPHMCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Once);
        }
    }



    public class ShouldMeetFphmFiveYearLookbackRequirementWith100MOCPoints : RulesForNewFphmIssuanceSpecScenario
    {
        private Credential fphmCredential;
        private IStep fiveYearLookbackStep;

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = Guid.NewGuid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            CertificationService = Container.GetInstance<App.Services.Impl.CertificationService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(credential);

            fphmCredential = CreateFPHMCredentialWithNoIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(fphmCredential);


            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));


            var activity = new ActivityResource();
            activity.TotalMOCPoints = 100;
            activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
            activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
            activity.Product = new ProductResource() { Code = RandomString.Build() };

            ActivitiesFullCollectionResource.Data.Add(activity);


            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_Completed_Fphm_Initial_Attestation(completedDate: DateTime.Now);

            Condition_RegistrationInterservice_GetUserRegistrations_OnePass_OneFailedLatest(noConsequence: true);

            Condition_InReciprocityProgram(DateTime.Now);

            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var fphmCert = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(),
                RandomString.Build(), RandomString.Build());

            fphmCert.ExternalId = CertificationId;

            My<ICertificationService>().Setup(c => c.GetByCode(It.IsAny<string>()))
                .Returns(fphmCert);


            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
               .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallFiveYearsLookBackFPHM()
        {
            try
            {
                var processingDate = new DateTime(DateTime.Now.Year, 12, 31);
                ProgramRulesService.ProcessingDate = processingDate;
                fiveYearLookbackStep = ProgramRulesService.FiveYearsLookBackFPHM(fphmCredential, processingDate);
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

        private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
        {
            fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
        }
    }



    public class ShouldMeetFphmFiveYearLookbackRequirementWithLessThan100MOCPointsInReciprocity : RulesForNewFphmIssuanceSpecScenario
    {
        private Credential fphmCredential;
        private IStep fiveYearLookbackStep;
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = Guid.NewGuid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            CertificationService = Container.GetInstance<App.Services.Impl.CertificationService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(credential);

            fphmCredential = CreateFPHMCredentialWithNoIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(fphmCredential);


            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_Completed_Fphm_Initial_Attestation(completedDate: DateTime.Now);

            Condition_RegistrationInterservice_GetUserRegistrations_OnePass_OneFailedLatest(noConsequence: true);

            Condition_InReciprocityProgram(new DateTime(2018, 12, 31));

            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var fphmCert = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(),
                RandomString.Build(), RandomString.Build());

            fphmCert.ExternalId = CertificationId;

            My<ICertificationService>().Setup(c => c.GetByCode(It.IsAny<string>()))
                .Returns(fphmCert);


            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
               .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallFiveYearsLookBackFPHM()
        {
            try
            {
                var processingDate = new DateTime(2018, 12, 31);
                ProgramRulesService.ProcessingDate = processingDate;
                fiveYearLookbackStep = ProgramRulesService.FiveYearsLookBackFPHM(fphmCredential, processingDate);
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

        private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
        {
            fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
        }
    }

    public class ShouldMeetFphmFiveYearLookbackRequirementWithLessThan100MOCPointsNotInReciprocityInSubscpecialty : RulesForNewFphmIssuanceSpecScenario
    {
        private Credential fphmCredential;
        private IStep fiveYearLookbackStep;

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = Guid.NewGuid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            CertificationService = Container.GetInstance<App.Services.Impl.CertificationService>();
            
            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);
            CredentialCollection.Add(credential);

            fphmCredential = CreateFPHMCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year-1, 12, 31),
                                                            certificationId: CertificationId);

            fphmCredential.Certification.Source.Code = RandomString.Build();
            fphmCredential.NewestIssuance.Source.Code = "ABIM";


            CredentialCollection.Add(fphmCredential);


            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_Completed_Fphm_Initial_Attestation(completedDate: DateTime.Now);

            Condition_RegistrationInterservice_GetUserRegistrations_OnePass_OneFailedLatest(noConsequence: true);
            
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            var fphmCert = Certification.Create(null, source, CertificationType.FocusPractice, RandomString.Build(),
                RandomString.Build(), RandomString.Build());

            fphmCert.ExternalId = CertificationId;

            My<ICertificationService>().Setup(c => c.GetByCode(It.IsAny<string>()))
                .Returns(fphmCert);

            var credList = new List<Credential>();
            credList.Add(fphmCredential);

            My<ICredentialService>().Setup(c => c.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(credList);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
               .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
            
        }

        public void WhenICallFiveYearsLookBackFPHM()
        {
            try
            {
                var processingDate = DateTime.Now;
                ProgramRulesService.ProcessingDate = processingDate;
                fiveYearLookbackStep = ProgramRulesService.FiveYearsLookBackFPHM(fphmCredential, processingDate);
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

        private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
        {
            fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
        }
    }

}
