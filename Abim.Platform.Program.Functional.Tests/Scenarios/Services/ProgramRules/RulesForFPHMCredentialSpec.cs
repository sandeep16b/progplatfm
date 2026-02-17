using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using Moq;
using Abim.Enterprise.Core.Interservice;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using Abim.Enterprise.Core.Resource.Program;
using Abim.Enterprise.Core.WebApi.Testing.Setup;
using Abim.Platform.Program.App.Services;
using Abim.Enterprise.Core.Relational.Validation;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Enterprise.Core.WebApi.Testing.Setup.Builders;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Domain;
using Abim.Enterprise.Core.Resource.Product;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Abim.Enterprise.Core.Resource.Registration;
using Abim.Enterprise.Core.WebApi.Response;
using IdentityModel.Client;
using Abim.Enterprise.Core.Relational.Classes;
using Abim.Platform.Program.App.Services.CommandResults;
using CancellationReasonType = Abim.Enterprise.Core.Resource.Product.CancellationReasonType;
using Abim.Enterprise.Core.Interservice.Identity;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to run FPHM rules"
      )]
    [TestFixture]
    public class RulesForFPHMCredentialSpec
    {
        [TestCase]
        [WorkItem(73615)]
        public void FPHM_EnoughMoc_EnoughMedicalKnowledge_PassExam()
        {
            new FPHM_EnoughMoc_EnoughMedicalKnowledge_PassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_EnoughMoc_NotEnoughMedicalKnowledge_PassExam()
        {
            new FPHM_EnoughMoc_NotEnoughMedicalKnowledge_PassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_NotEnoughMoc_EnoughMedicalKnowledge_PassExam()
        {
            new FPHM_NotEnoughMoc_EnoughMedicalKnowledge_PassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_NotEnoughMoc_NotEnoughMedicalKnowledge_PassExam()
        {
            new FPHM_NotEnoughMoc_NotEnoughMedicalKnowledge_PassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_EnoughMoc_EnoughMedicalKnowledge_DidNotPassExam()
        {
            new FPHM_EnoughMoc_EnoughMedicalKnowledge_DidNotPassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_EnoughMoc_NotEnoughMedicalKnowledge_DidNotPassExam()
        {
            new FPHM_EnoughMoc_NotEnoughMedicalKnowledge_DidNotPassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_NotEnoughMoc_EnoughMedicalKnowledge_DidNotPassExam()
        {
            new FPHM_NotEnoughMoc_EnoughMedicalKnowledge_DidNotPassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_NotEnoughMoc_NotEnoughMedicalKnowledge_DidNotPassExam()
        {
            new FPHM_NotEnoughMoc_NotEnoughMedicalKnowledge_DidNotPassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(73615)]
        public void FPHM_DidNotPassExam()
        {
            new FPHM_DidNotPassExam().BDDfy();
        }

    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class FPHMCertificateSpecScenario : ProgramRulesServiceScenario
    {
        protected Guid MemberId { get; set; }
        protected DateTime EventDate { get; set; }
        protected ProgramRulesService ProgramRulesService { get; set; }
        protected Exception ExceptionCaught { get; set; }
        protected ActivityFullCollectionResource ActivitiesFullCollectionResource { get; set; }
        protected FPHMCredentialsCommand Command { get; set; }

        protected int Year = DateTime.Now.Year;

        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            return types;
        }

        protected Credential CreateIMCertificationWithIssuances(IssuanceStatusType issuanceStatusType)
        {
            var source = Source.Create(RandomString.Build());
            source.Code = "ABIM";
            var certification = Certification.Create(null, source, CertificationType.Primary, RandomString.Build());
            certification.Code = "IM";
            var credential = Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), EnumAttributes.RandomEntry<PathwayType>(),
                RandomString.Build());
            var issuanceDate = DateTimeBuilder.Random().Build();
            var issuance1 = Issuance.Create(source, EnumAttributes.RandomEntry<DurationType>(), EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                EnumAttributes.RandomEntry<MaintenanceStatusType>(), EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Expired,
                issuanceDate, RandomString.Build());
            credential.AddIssuance(issuance1);
            // add most recent issuance with input param status
            var issuance2   = Issuance.Create(source, EnumAttributes.RandomEntry<DurationType>(), EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                EnumAttributes.RandomEntry<MaintenanceStatusType>(), EnumAttributes.RandomEntry<OccurrenceType>(), issuanceStatusType,
                issuanceDate.AddYears(5), RandomString.Build());
            credential.AddIssuance(issuance2);

            return credential;
        }

        protected Issuance CreateActiveExpiringIssuance(DateTime issuanceDate)
        {
            var source = Source.Create(RandomString.Build());
            var issuance = Issuance.Create(source, EnumAttributes.RandomEntry<DurationType>(), EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                EnumAttributes.RandomEntry<MaintenanceStatusType>(), EnumAttributes.RandomEntry<OccurrenceType>(), IssuanceStatusType.Active,
                issuanceDate, RandomString.Build());
            issuance.ExpirationDate = issuance.IssuanceDate.AddYears(5);    //just to make sure it would expire very late
            return issuance;
        }

        protected void AddToUserActivities(ActivityResource activityResource)
        {
            ActivitiesFullCollectionResource.Data.Add(activityResource);
        }

        #region Conditions

        // ICard attestation (activity record)
        protected void Condition_FPHMAttest_Activity(DateTime fphmAttestation_completedDate)
        {
            ActivityResource activityResourceFPHMAttest = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.FPHMAttestInitial },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = fphmAttestation_completedDate,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = 0,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource() { CreditType = new CreditTypeResource() { Value = "Unknown" }, CreditDate = DateTime.Now }
                }
            };

            AddToUserActivities(activityResourceFPHMAttest);
        }

        // MOC Points (activity record)
        protected void Condition_MOCPoints_Activity(DateTime completedDate, int totalMOCPoints)
        {
            ActivityResource activityResourceReciprocity = new ActivityResource()
            {
                Product = new ProductResource() { Code = "Any" },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = completedDate,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = totalMOCPoints,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource() { CreditType= new CreditTypeResource() { Value = "Unknown" }, CreditDate = DateTime.Now }
                }
            };

            AddToUserActivities(activityResourceReciprocity);
        }

        protected void Condition_BlendedMedicalKnowledgePoints_ActivityCredit(DateTime creditDate, DateTime expirationDate, int medicalKnowledgePoints)
        {
            ActivityResource activity = new ActivityResource()
            {
                Product = new ProductResource() { Code = "Any" },
                ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = DateTime.Now,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = 0,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource()
                    {
                        CreditDate = creditDate,
                        CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.BlendedMedicalKnowledgeActivity },
                        ExpirationDate = expirationDate,
                        CreditEarned = medicalKnowledgePoints
                    }
                }
            };

            AddToUserActivities(activity);
        }

        protected Certification Condition_CertificationService_GetByCode ()
        {
            var source = Source.Create(RandomString.Build());
            var certification = Certification.Create(null, source, CertificationType.Primary, RandomString.Build());
            certification.Code = ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine;
            My<ICertificationService>().Setup(p => p.GetByCode(It.IsAny<string>())).Returns(certification);
            return certification;
        }

        //exam pass/fail checking 
        protected DateTime? Condition_RegistrationInterservice_GetUserRegistrations(DateTime resultDate, ExamType examType, Guid CertificationGuid)
        {
            var registrationsMock = new RegistrationFullCollectionResource();

            registrationsMock.Data.Add(new RegistrationResource()
            {
                Result = ExamResultType.Pass.ToString(),
                ExamType = new EnumValueResponseResource<ExamType>(examType),
                Exam = new ExamSummaryResource() { CertificationId=CertificationGuid},
                ResultDate = resultDate,
                MemberId = new Guid()
            });

            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));

            return registrationsMock.Data.FindLast(p => p.ResultDate == resultDate).ResultDate;
        }

        #endregion
    }

    #region Scenarios

    /// <summary>
    /// The all requirements met scenario
    /// </summary>
    public class FPHM_EnoughMoc_EnoughMedicalKnowledge_PassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 20);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc,CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test" 
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

        public void AndThenIssueFPHMCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.AtLeastOnce());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points, but there are enough medical knowledge points, and in reciprocity
    /// </summary>
    public class FPHM_NotEnoughMoc_EnoughMedicalKnowledge_PassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 20);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there are enough MOC points but not medical knowledge points, and in reciprocity
    /// </summary>
    public class FPHM_EnoughMoc_NotEnoughMedicalKnowledge_PassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 10);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points or medical knowledge points, but in reciprocity
    /// </summary>
    public class FPHM_NotEnoughMoc_NotEnoughMedicalKnowledge_PassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 95);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 19);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there are enough MoC points and medical knowledge points, but the user is not in reciprocity
    /// </summary>
    public class FPHM_EnoughMoc_EnoughMedicalKnowledge_DidNotPassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 20);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            // resultDate is out of desired range
            var examPassDate10 = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 10, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            var examPassDate2 = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 4, 01, 11), examType: ExamType.Kci, CertificationGuid: certification.ExternalId);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there are enough MOC points but not medical knowledge points, and not in reciprocity
    /// </summary>
    public class FPHM_EnoughMoc_NotEnoughMedicalKnowledge_DidNotPassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
            
            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 10);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            //var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points but there are enough medical knowledge points, and not in reciprocity
    /// </summary>
    public class FPHM_NotEnoughMoc_EnoughMedicalKnowledge_DidNotPassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 99);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 20);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            //var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points or medical knowledge points, and also not in reciprocity
    /// </summary>
    public class FPHM_NotEnoughMoc_NotEnoughMedicalKnowledge_DidNotPassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 10);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which user didn't pass their exam
    /// </summary>
    public class FPHM_DidNotPassExam
        : FPHMCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            My<IAccessTokenService>().Setup(p => p.GetClientAccessTokenAsync())
                   .Returns(Task.FromResult<string>("any"));

            var credential = CreateIMCertificationWithIssuances(issuanceStatusType: IssuanceStatusType.Active);
            My<ICredentialService>().Setup(p => p.GetIMCredentialByMember(MemberId))
                .Returns(credential);

            //Requirement: FPHM Initial attestation
            Condition_FPHMAttest_Activity(fphmAttestation_completedDate: new DateTime(Year - 3, 05, 05));

            //Requirement: 100 Total points within 5 year
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);

            //Requirement: 20 Medical knowledge within 5 year
            Condition_BlendedMedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                    medicalKnowledgePoints: 10);

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            var certification = Condition_CertificationService_GetByCode();

            //Requirement: Pass the exam (2 or 10 year)
            //var examPassDate = Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, CertificationGuid: certification.ExternalId);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);

            //CHECKS TO DETERMINE THE MAINTENANCE STATUS

            My<ICredentialService>().Setup(p => p.FirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 1, 11, 16));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<IssueFPHMCommand>()))
                .Returns(new IssueFPHMCommandResult(CommandStatus.Accepted, null, null));
        }

        public void GivenInputAValidCommand()
        {
            Command = new FPHMCredentialsCommand()
            {
                MemberId = MemberId,
                EventDate = DateTime.Now,
                CreatedBy = "Test"
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

        public void AndThenIssueFPHMCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<IssueFPHMCommand>()), Times.Never());
        }
    }

    #endregion
}
