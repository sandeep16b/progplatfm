using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Extensions.ExternalResponses;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
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

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to run Print Grand Father certificate"
      )]
    [TestFixture]
    public class RulesforPrintGrandFatherSpec
    {
        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity()
        {
            new PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_EnoughMoc_NotEnoughMedicalKnowledge_InReciprocity()
        {
            new PrintGrandFatherCert_EnoughMoc_NotEnoughMedicalKnowledge_InReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_NotEnoughMoc_EnoughMedicalKnowledge_InReciprocity()
        {
            new PrintGrandFatherCert_NotEnoughMoc_EnoughMedicalKnowledge_InReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_NotEnoughMoc_NotEnoughMedicalKnowledge_InReciprocity()
        {
            new PrintGrandFatherCert_NotEnoughMoc_NotEnoughMedicalKnowledge_InReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_InReciprocity()
        {
            new PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_InReciprocity().BDDfy();
        }

        //++++ negative cases ********
        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_EnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity()
        {
            new PrintGrandFatherCert_EnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_NotEnoughMoc_EnoughMedicalKnowledge_NotInReciprocity()
        {
            new PrintGrandFatherCert_NotEnoughMoc_EnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_NotEnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity()
        {
            new PrintGrandFatherCert_NotEnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        public void PrintGrandFatherCert_DidNotPassExam()
        {
            new PrintGrandFatherCert_DidNotPassExam().BDDfy();
        }

        [TestCase]
        [WorkItem(87085)]
        [NUnit.Framework.Ignore("Being corrected in another branch")]
        public void PrintGrandFatherCert_AlreadyHasGrandFatherCertPrinted()
        {
            new PrintGrandFatherCert_AlreadyHasGrandFatherCertPrinted().BDDfy();
        }
       
        [TestCase]
        [WorkItem(87085)]
        public void ShouldMeet_GF_FiveYearLookbackRequirement_With100MOCPoints()
        {
            new ShouldMeetFphmFiveYearLookbackRequirementWith100MOCPoints().BDDfy();
        }
        [TestCase]
        [WorkItem(87085)]
        public void ShouldMeet_GF_FiveYearLookbackRequirement_WithLessThan100MOCPoints_InReciprocity()
        {
            new ShouldMeetGFFiveYearLookbackRequirementWithLessThan100MOCPointsInReciprocity().BDDfy();
        }
        [TestCase]
        [WorkItem(87085)]
        public void ShouldMeet_GF_FiveYearLookbackRequirement_WithLessThan100MOCPoints_NotInReciprocity_InSubscpecialty()
        {
            new ShouldMeetGFFiveYearLookbackRequirementWithLessThan100MOCPointsNotInReciprocityInSubscpecialty().BDDfy();
        }

        [TestCase]
        [WorkItem(150925)]
        public void PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity_Passed_CMP_Exam()
        {
            new PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity_Passed_CMP_Exam().BDDfy();
        }
    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="CredentialServiceScenario" />
    public abstract class PrintGrandFatherCertCertificateSpecScenario : ProgramRulesServiceScenario
    {
        protected Guid CredentialId { get; set; }

        protected Guid CertificationId { get; set; }

        protected int IssuanceId { get; set; }
        protected ProgramRulesService ProgramRulesService { get; set; }
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

        protected Credential CreateCredentialWithIssuance(DateTime issuanceExpirationDate)
        {
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            source.Code = "ABIM";
            var certification = Certification.Create(null, source, CertificationType.Primary, RandomString.Build(), RandomString.Build(), RandomString.Build());
            certification.Code = "ICARD";
            var credential = Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), EnumAttributes.RandomEntry<Resources.PathwayType>(),
                 null, null, RandomString.Build());
            var issuance = Issuance.Create(source, EnumAttributes.RandomEntry<DurationType>(), EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                EnumAttributes.RandomEntry<MaintenanceStatusType>(), EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(),
                DateTimeBuilder.Random().Build(), RandomString.Build());
            IssuanceId = issuance.Id;
            issuance.ExpirationDate = issuanceExpirationDate;
            credential.AddIssuance(issuance);
            return credential;
        }

        protected Issuance CreateActiveExpiringIssuance(DateTime issuanceDate)
        {
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
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

        // CompletedDate
        protected void Condition_InReciprocityProgram(DateTime completedDate)
        {
            ActivityResource activityResourceReciprocity = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ReciprocityAttest },
                ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
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
                ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Cancelled),
                CompletedDate = completedDate,
                CancellationReason = new Product.Extensions.ExternalResponses.EnumValueResponseResource<Product.Resources.Enums.CancellationReasonType>(Product.Resources.Enums.CancellationReasonType.Financial),
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
                ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
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
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();

            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                Result = examResultType.ToString(),
                ExamType = new EnumValueResponseResource<ExamType>(examType),
                MemberId = new Guid(),
                CertificationId = certificationId,
                Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource() { SeatDate = examTestDate } }
            });

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));
        }

        protected void Condition_RegistrationInterservice_GetUserCMPRegistrations(Guid certificationId, 
                                                                                    DateTime examTestDate, 
                                                                                    ExamResultType result)
        {
            var output = new UserRegistrationsAndCMPRegistrationsResource();

            output.Registrations = new List<RegistrationResource>(0);
            output.CMPRegistrations = new List<CMPRegistrationResource>(1);

            var examBuilder = new CMPExamSummaryResourceBuilder();
            var regBuilder = new CMPRegistrationResourceBuilder();

            var reg = 
                regBuilder
                    .WithExamResult(result)
                    .WithTestDate(examTestDate)
                    .WithCMPExam(examBuilder.WithCertificationId(certificationId).Build())
                    .Build();

            output.CMPRegistrations.Add(reg);

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(output));
        }


        #endregion
    }

    #region Scenarios

    /// <summary>
    /// The all requirements met scenario
    /// </summary>
    public class PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 100,
                                                                        medicalKnowledgePoints: 20);

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.AtLeastOnce());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points, but there are enough medical knowledge points, and in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_NotEnoughMoc_EnoughMedicalKnowledge_InReciprocity
        : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 99.9m,
                                                                        medicalKnowledgePoints: 20);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.AtLeastOnce());
        }

    }

    /// <summary>
    /// The scenario in which there are enough MOC points but not medical knowledge points, and in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_EnoughMoc_NotEnoughMedicalKnowledge_InReciprocity
        : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 100,
                                                                        medicalKnowledgePoints: 19.9m);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.AtLeastOnce());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points or medical knowledge points, but in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_NotEnoughMoc_NotEnoughMedicalKnowledge_InReciprocity
        : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 99.9m,
                                                                        medicalKnowledgePoints: 19.9m);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.AtLeastOnce());
        }
    }

    /// <summary>
    /// The scenario in which there are enough MoC points and medical knowledge points, but the user is not in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_InReciprocity
        : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 110,
                                                                        medicalKnowledgePoints: 21.9m);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.AtLeastOnce());
        }
    }

    /// <summary>
    /// The scenario in which there are enough MOC points but not medical knowledge points, and not in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_EnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity
        : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 110,
                                                                        medicalKnowledgePoints: 18.9m);

            //Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.Once());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points but there are enough medical knowledge points, and not in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_NotEnoughMoc_EnoughMedicalKnowledge_NotInReciprocity : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 88.8m,
                                                                        medicalKnowledgePoints: 28.9m);

            //Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points or medical knowledge points, and also not in reciprocity
    /// </summary>
    public class PrintGrandFatherCert_NotEnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 99.8m,
                                                                        medicalKnowledgePoints: 18.9m);

            //Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which user didn't pass their exam
    /// </summary>
    public class PrintGrandFatherCert_DidNotPassExam : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 100.8m,
                                                                        medicalKnowledgePoints: 20.9m);

            //Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Fail);

            My<IRegistrationInterservice>().Setup(x => x.GetUserCMPRegistrations(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(new CMPRegistrationCollectionResource()));

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));


        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.Never());
        }
    }

    /// <summary>
    /// The scenario in which user already has grandfather certificate printed
    /// </summary>
    public class PrintGrandFatherCert_AlreadyHasGrandFatherCertPrinted : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            credential.SetGrandfatherMOCPrintDate(It.IsAny<DateTime>(), "Test");
            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 100.8m,
                                                                        medicalKnowledgePoints: 20.9m);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Fail);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.Never());
        }
    }


    /// <summary>
    /// The scenario in which there aren't enough MOC points, but there are enough medical knowledge points, and in reciprocity
    /// </summary>
    public class ShouldMeetFphmFiveYearLookbackRequirementWith100MOCPoints
        : PrintGrandFatherCertCertificateSpecScenario
    {
        private Credential gfCredential;
        private IStep fiveYearLookbackStep;
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            gfCredential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(gfCredential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(gfCredential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 100,
                                                                        medicalKnowledgePoints: 0);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
        {
            try
            {
                var processingDate = DateTime.Now;
                ProgramRulesService.ProcessingDate = processingDate;
                fiveYearLookbackStep = ProgramRulesService.FiveYearsLookBackGFPrinting(gfCredential, processingDate);
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
            fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
        }
    }


    /// <summary>
    /// The scenario in which there aren't enough MOC points, but there are enough medical knowledge points, and in reciprocity
    /// </summary>
    public class ShouldMeetGFFiveYearLookbackRequirementWithLessThan100MOCPointsInReciprocity
        : PrintGrandFatherCertCertificateSpecScenario
    {
        private Credential gfCredential;
        private IStep fiveYearLookbackStep;
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            gfCredential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(gfCredential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(gfCredential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 10,
                                                                        medicalKnowledgePoints: 0);

            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));

            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
        {
            try
            {
                var processingDate = DateTime.Now;
                ProgramRulesService.ProcessingDate = processingDate;
                fiveYearLookbackStep = ProgramRulesService.FiveYearsLookBackGFPrinting(gfCredential, processingDate);
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
            fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
        }
    }

    /// <summary>
    /// The scenario in which there aren't enough MOC points, but there are enough medical knowledge points, and in reciprocity
    /// </summary>
    public class ShouldMeetGFFiveYearLookbackRequirementWithLessThan100MOCPointsNotInReciprocityInSubscpecialty
        : PrintGrandFatherCertCertificateSpecScenario
    {
        private Credential gfCredential;
        private IStep fiveYearLookbackStep;
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            gfCredential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year -1, 12, 31),
                                                            certificationId: CertificationId);

            gfCredential.Certification.Code = RandomString.Build();

            CredentialCollection.Add(gfCredential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(gfCredential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 10,
                                                                        medicalKnowledgePoints: 0);
            
            Condition_RegistrationInterservice_GetUserRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 2, 01, 11),
                                                                    examType: ExamType.Moc,
                                                                    examResultType: ExamResultType.Pass);

            var credList = new List<Credential>();
            credList.Add(gfCredential);

            My<ICredentialService>().Setup(c => c.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(credList);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
        {
            try
            {
                var processingDate = DateTime.Now;
                ProgramRulesService.ProcessingDate = processingDate;
                fiveYearLookbackStep = ProgramRulesService.FiveYearsLookBackGFPrinting(gfCredential, processingDate);
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
            fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
        }
    }

    public class PrintGrandFatherCert_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity_Passed_CMP_Exam : PrintGrandFatherCertCertificateSpecScenario
    {
        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            CredentialCollection = new List<Credential>();
            CertificationId = new Guid();
        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateGFCredentialWithIssuance(issuanceDate: new DateTime(DateTime.Now.Year, 12, 31),
                                                            certificationId: CertificationId);

            CredentialCollection.Add(credential);

            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
              .Returns(credential);

            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            Condition_MOCPoints_MedicalKnowledgePoints_ActivityCredit(completedDate: DateTime.Now,
                                                                        creditDate: DateTime.Now,
                                                                        totalMOCPoints: 100,
                                                                        medicalKnowledgePoints: 20);

            My<IRegistrationInterservice>().Setup(x => x.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>())).Returns(Task.FromResult(new RegistrationFullCollectionResource()));
            
            Condition_RegistrationInterservice_GetUserCMPRegistrations(certificationId: CertificationId,
                                                                    examTestDate: new DateTime(Year - 1, 01, 11),
                                                                    result: ExamResultType.Pass);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()))
                .Returns(new UpdateGrandfatherMOCPrintDateCommandResult(CommandStatus.Accepted, null, null));
        }

        public void WhenICallHandle()
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<UpdateGrandfatherMOCPrintDateCommand>()), Times.AtLeastOnce());
        }
    }

    #endregion
}
