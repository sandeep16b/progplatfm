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
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Classes;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to run Time Limited Credentials rules"
      )]
    [TestFixture]
    public class RulesForMBMforTLSpecOld
    {
       
        [TestCase]
        [WorkItem(137190)]
        public void Should_Not_Reissue_When_Assessment_Met_After_Eval_Date()
        {
            new ShouldNotReissueWhenAssessmentMetAfterEvalDate().BDDfy();
        }

        [TestCase]
        [WorkItem(137190)]
        public void Should_Reissue_When_Passed_MOC_Exam_With_Applicable_Admin_Year()
        {
            new ShouldReissueWhenPassedMOCExamWithApplicableAdminYear().BDDfy();
        }

        [TestCase]
        [WorkItem(137190)]
        public void Should_Not_Reissue_When_No_MOC_Pass_And_Bad_KCI_NoConsequences_With_Inapplicable_Admin_Date()
        {
            new ShouldNotReissueWhenNoMOCPassAndBadKCINoConsequencesWithInapplicableAdminDate().BDDfy();
        }

        [TestCase]
        [WorkItem(137190)]
        public void Should_Reissue_When_No_MOC_Pass_And_Bad_KCI_NoConsequences_With_Inapplicable_Admin_Date_But_Has_Later_Pass()
        {
            new ShouldReissueWhenNoMOCPassAndBadKCINoConsequencesWithInapplicableAdminDateButHasLaterPass().BDDfy();
        }

        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOC_Pass_But_No_Bad_KCI_or_CMP_NoConsequences_Exam()
        {
            new ShouldReissueWhenNoMOCPassButNoBadKCIorCMPNoConsequencesExam().BDDfy();
        }
        
        [TestCase]
        [WorkItem(151482)]
        public void Should_Not_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceFailExists_NoSubsequentPass()
        {
            new ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceFailExistsNoSubsequentPass().BDDfy();
        }

        [TestCase]
        [WorkItem(151482)]
        public void Should_Not_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceUttExists_NoSubsequentPass()
        {
            new ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceUttExistsNoSubsequentPass().BDDfy();
        }

        [TestCase]
        [WorkItem(151482)]
        public void Should_Not_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceIndeterminateExists_NoSubsequentPass()
        {
            new ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIndeterminateExistsNoSubsequentPass().BDDfy();
        }

        [TestCase]
        [WorkItem(151482)]
        public void Should_Not_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceIncompleteExists_NoSubsequentPass()
        {
            new ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIncompleteExistsNoSubsequentPass().BDDfy();
        }     

        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceFailExists_SubsequentPass_Exists()
        {
            new ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceFailExistsSubsequentPassExists().BDDfy();
        }
        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceUttExists_SubsequentPass_Exists()
        {
            new ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceUttExistsSubsequentPassExists().BDDfy();
        }

        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceIndtExists_SubsequentPass_Exists()
        {
            new ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIndeterminateExistsSubsequentPassExists().BDDfy();
        }

        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceIncompleteExists_SubsequentPass_Exists()
        {
            new ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIncompleteExistsSubsequentPassExists().BDDfy();
        }     

        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceBadStatusExists_No_SubsequentPass_Exists_Prior_Pass_Exists()
        {
            new ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceBadStatusExistsNoSubsequentPassPriorPassExists().BDDfy();
        }
        
        [TestCase]
        [WorkItem(151482)]
        public void Should_Reissue_When_No_MOCPass_But_NoKCINoConsequencesExam_CmpNoConsequenceBadStatusExists_No_SubsequentPass_Subsequent_Non_Pass()
        {
            new ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceBadStatusExistsNoSubsequentPassSubsequentNonPassExists().BDDfy();
        }

    }


    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class MustBeMaintainedCertificateSpecScenario : ProgramRulesServiceScenario
    {
        protected Guid CredentialId                                                          { get; set; }
        protected int IssuanceId                                                             { get; set; }
        protected ProgramRulesService ProgramRulesService                                    { get; set; }
        protected new Exception ExceptionCaught { get; set; }
        protected ActivityFullCollectionResource ActivitiesFullCollectionResource            { get; set; }
        protected RunRulesForMustBeMaintainedCertificateCommand Command                      { get; set; }

        protected int Year = DateTime.Now.Year;

        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();
            return types;
        }

        protected Credential CreateCredentialWithIssuance(DateTime issuanceExpirationDate, DateTime issuanceDate, DurationType DurationType= DurationType.Timelimited)
        {
            var source = Source.Create(RandomString.Build(), RandomString.Build(), RandomString.Build());
            source.Code = "ABIM";
            var certification = Certification.Create(null, source, CertificationType.Primary, RandomString.Build(), RandomString.Build(), RandomString.Build());
            certification.Code = "ICARD";
            var credential = Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), EnumAttributes.RandomEntry<Resources.PathwayType>(),
                null, null, RandomString.Build());
            var issuance = Issuance.Create(source, DurationType, EnumAttributes.RandomEntry<MaintenanceRequirementType>(),
                EnumAttributes.RandomEntry<MaintenanceStatusType>(), EnumAttributes.RandomEntry<OccurrenceType>(), EnumAttributes.RandomEntry<IssuanceStatusType>(),
                issuanceDate, RandomString.Build());
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

        // ICard attestation (activity record)
        protected void Condition_ICARDAttest_Activity(DateTime icardAttestation_completedDate)
        {
            ActivityResource activityResourceICARDAttest = new ActivityResource()
            {
                Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC },
                ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = icardAttestation_completedDate,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = 0,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource() { CreditType = new CreditTypeResource() { Value = "Unknown" }, CreditDate = DateTime.Now }
                }
            };
            
            AddToUserActivities(activityResourceICARDAttest);
        }

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
        protected void Condition_InReciprocityProgram(DateTime completedDate,DateTime cancelledDate)
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
        protected void Condition_MOCPoints_Activity(DateTime completedDate, int totalMOCPoints)
        {
            ActivityResource activityResourceReciprocity = new ActivityResource()
            {
                Product = new ProductResource() { Code = "Any" },
                ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
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
         
        protected void Condition_MedicalKnowledgePoints_ActivityCredit(DateTime creditDate, DateTime expirationDate, int medicalKnowledgePoints)
        {
            ActivityResource activity = new ActivityResource()
            {
                Product = new ProductResource() { Code = "Any" },
                ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass),
                CompletedDate = DateTime.Now,
                CancellationReason = null,
                CancelledDate = null,
                TotalMOCPoints = 0,
                ActivityCredits = new List<ActivityCreditResource>()
                {
                    new ActivityCreditResource()
                    {
                        CreditDate = creditDate,
                        CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.MedicalKnowledgePoints },
                        ExpirationDate = expirationDate,
                        CreditEarned = medicalKnowledgePoints
                    }
                }
            };
            
            AddToUserActivities(activity);
        }

        //exam pass/fail checking 
        protected void Condition_RegistrationInterservice_GetUserRegistrations(DateTime resultDate, ExamType examType, Guid certificationId)
        {
            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource();

            registrationsMock.Registrations = new List<RegistrationResource>(1);
            registrationsMock.CMPRegistrations = new List<CMPRegistrationResource>(0);

            registrationsMock.Registrations.Add(new RegistrationResource()
            {
                Result      = ExamResultType.Pass.ToString(),
                ExamType    = new EnumValueResponseResource<ExamType>(examType),
                ResultDate  = resultDate,
                MemberId    = new Guid(),
                CertificationId = certificationId,
                Seats       = new List<SeatRegistrationSummaryResource>()
                {
                        new SeatRegistrationSummaryResource
                        {
                            SeatDate = resultDate
                        }
                }
            });
            
            My<IRegistrationInterservice>()
                .Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));
        }

        #endregion
    }

    #region Abstract Scenario Classes for 137190
    public abstract class MBMIssuanceScenario : MustBeMaintainedCertificateSpecScenario
    {
        protected List<Credential> _credentials;
        protected Guid _memberId = Guid.NewGuid();
        protected Guid _certificationId = Guid.NewGuid();
        protected DateTime _eventDate;
        protected DateTime _processingDate;
        protected readonly DateTime BASE_DATE = new DateTime(DateTime.Now.Year, 10, 1);
        protected bool _correctiveActionReturnValue;
        protected UserRegistrationsAndCMPRegistrationsResource _registrations;

        protected virtual void SetupCredentials()
        {
            _credentials = new List<Credential>(1);

            var source = Source.Create("American Board of Internal Medicine", "ABIM", "UnitTest");

            var cred = CredentialBuilder.Build(Resources.PathwayType.MOC);
            cred.Certification = CertificationBuilder.Build(source);
            cred.Certification.ExternalId = _certificationId;

            var issuance = 
                IssuanceBuilder.BuildWithoutRandoms(
                    source, 
                    IssuanceStatusType.Active, 
                    BASE_DATE.AddYears(-4), 
                    DurationType.Timelimited, 
                    MaintenanceRequirementType.NotRequired, 
                    MaintenanceStatusType.Maintained, 
                    OccurrenceType.Initial);
            issuance.ExpirationDate = BASE_DATE.AddYears(-1);

            cred.AddIssuance(issuance);
            _credentials.Add(cred);
        }

        protected virtual void SetupActivities()
        {
            var builder = new ActivityResourceBuilder();
            AddToUserActivities(
                builder
                    .WithActivityResult(ActivityResultType.Pass)
                    .WithCompletedDate(BASE_DATE.AddDays(-4))
                    .WithTotalMOCPoints(110)
                    .Build());
        }

        protected virtual void SetupRegistrations()
        {
            _registrations = new UserRegistrationsAndCMPRegistrationsResource();
            _registrations.Registrations = new List<RegistrationResource>(0);
            _registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);
        }

        protected override void PreSetup()
        {
            ActivitiesFullCollectionResource = new ActivityFullCollectionResource();
            LongitudinalEnrollments = new List<LongitudinalEnrollmentSummaryResource>();
        }

        protected override void PostSetup()
        {
            SetupActivities();

            My<IProductInterservice>()
                .Setup(x => 
                    x.GetUserActivities(
                        It.IsAny<string>(), 
                        It.IsAny<Guid>(), 
                        It.IsAny<DateTime>(), 
                        It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            SetupCredentials();

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            SetupRegistrations();

            My<IRegistrationInterservice>()
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(_registrations));

            My<IRegistrationInterservice>().Setup(p => p.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource() { Data = LongitudinalEnrollments }));

            My<ICredentialService>()
                .Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(BASE_DATE.AddYears(-4));

            ProgramRulesService = Container.GetInstance<ProgramRulesService>();
        }

        protected virtual void GivenIHaveValidParameters()
        {
            //Credentials and Member ID already setup
            _eventDate = BASE_DATE;
            _processingDate = BASE_DATE.AddDays(5);
        }

        protected async void WhenIRunCorrectiveAction()
        {
            try
            {
                _correctiveActionReturnValue = 
                    await ProgramRulesService.RunCorrectiveAction(
                        _credentials,
                        _memberId,
                        _eventDate,
                        _processingDate);
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        protected void ThenNoErrorShouldHaveOccurred()
        {
            ExceptionCaught.Should().BeNull();
        }

        protected void AndTheReturnValueFromTheCallToCorrectiveActionShouldBeTrue()
        {
            _correctiveActionReturnValue.Should().BeTrue();
        }

        protected void AndClientAccessTokenShouldHaveBeenRetrieved()
        {
            My<IAccessTokenService>()
                .Verify(x => x.GetAccessToken(), Times.AtLeastOnce);
        }

        protected void AndUserActivitiesShouldHaveBeenRetrieved()
        {
            My<IProductInterservice>()
                .Verify(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), 
                Times.AtLeastOnce);
        }

        protected void AndRegistrationsShouldHaveBeenRetrieved()
        {
            My<IRegistrationInterservice>()
                .Verify(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()), 
                Times.Once);
        }

        protected void AndTheFirstIssuanceDateShouldHaveBeenRetrieved()
        {
            My<ICredentialService>()
                .Verify(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once);
        }
    }

    public abstract class MBMReissuedScenario : MBMIssuanceScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }

        protected override void PostSetup()
        {
            base.PostSetup();

            My<ICredentialService>()
                .Setup(x => x.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult());
        }

        public void AndTheCredentialShouldHaveBeenReissued()
        {
            My<ICredentialService>().Verify(x =>
                x.Handle(It.Is<ExpireAndReissueCommand>(cmd =>
                    cmd.CreatedBy == "MBMforTL"
                    && cmd.CredentialId == _credentials[0].ExternalId
                    && cmd.IssuanceDate == BASE_DATE
                    && cmd.MaintenanceStatus == MaintenanceStatusType.Maintained
                    && cmd.ScheduledUpdate == new DateTime(BASE_DATE.Year + 1, 4, 1)
                    )),
                Times.Once);
        }
    }

    public abstract class MBMNotReissuedScenario : MBMIssuanceScenario
    {
        public void AndTheCredentialShouldNotHaveBeenReissued()
        {
            My<ICredentialService>().Verify(x => 
                x.Handle(It.IsAny<ExpireAndReissueCommand>()), 
                Times.Never);
        }
    }
    #endregion Abstract Scenario Classes for 137190

    #region Scenarios

    public class ShouldNotReissueWhenAssessmentMetAfterEvalDate : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(15);
        }
    }

    public class ShouldReissueWhenPassedMOCExamWithApplicableAdminYear : MBMReissuedScenario
    {
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new RegistrationResourceBuilder();

            _registrations.Registrations = new List<RegistrationResource>(1);
            _registrations.Registrations.Add(
                builder
                    .WithCertificationId(_certificationId)
                    .WithAdministrationYear(BASE_DATE.Year)
                    .WithAdministrationDate(BASE_DATE)
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Moc)
                    .WithSeat(BASE_DATE.AddDays(-5))
                    .Build());
        }
    }

    public class ShouldReissueWhenNoMOCPassButNoBadKCIorCMPNoConsequencesExam : MBMReissuedScenario
    {
        //Default setup and methods in base class MBMReissuedScenario handle this scenario as-is
    }

    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceFailExistsNoSubsequentPass : MBMNotReissuedScenario//MBMReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder         
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .Build());
        }
    }
    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceUttExistsNoSubsequentPass : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.UnableToTest)
                    .Build());
        }
    }
    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIndeterminateExistsNoSubsequentPass : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Indeterminate)
                    .Build());
        }
    }
    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIncompleteExistsNoSubsequentPass : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Incomplete)
                    .Build());
        }
    }


    public class ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceFailExistsSubsequentPassExists : MBMReissuedScenario
    {
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .Build());

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(50))
                    .WithExamResult(ExamResultType.Pass)
                    .Build());
        }
    }

    public class ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceUttExistsSubsequentPassExists : MBMReissuedScenario
    {
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.UnableToTest)
                    .Build());

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(50))
                    .WithExamResult(ExamResultType.Pass)
                    .Build());
        }
    }

    public class ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIndeterminateExistsSubsequentPassExists : MBMReissuedScenario
    {
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Indeterminate)
                    .Build());

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(50))
                    .WithExamResult(ExamResultType.Pass)
                    .Build());
        }
    }

    public class ShouldReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceIncompleteExistsSubsequentPassExists : MBMReissuedScenario
    {
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Incomplete)
                    .Build());

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(50))
                    .WithExamResult(ExamResultType.Pass)
                    .Build());
        }
    }

    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceBadStatusExistsNoSubsequentPassPriorPassExists : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .Build());

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(-50))//Pass is in the past
                    .WithExamResult(ExamResultType.Pass)
                    .Build());
        }
    }

    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceBadStatusExistsNoSubsequentPassSubsequentNonPassExists : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .Build());

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(50))
                    .WithExamResult(ExamResultType.Pending)
                    .Build());
        }
    }
    public class ShouldNotReissueWhenNoMOCPassButNoKCINoConsequencesExamCmpNoConsequenceBadStatusExistsNoSubsequentPassSubsequentNonCmpPassExists : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new CMPRegistrationResourceBuilder();
            var summaryBuilder = new CMPExamSummaryResourceBuilder();

            _registrations.CMPRegistrations.Add(
                builder
                    .WithCMPExam(summaryBuilder.WithCertificationId(_certificationId).WithNoConsequenceYear(BASE_DATE.AddDays(10).Year).Build())
                    .WithTestDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .Build());

            var regBuilder = new RegistrationResourceBuilder();

            _registrations.Registrations.Add(
                regBuilder
                    .WithCertificationId(_certificationId)
                    .WithAdministrationYear(BASE_DATE.AddDays(50).Year)
                    .WithAdministrationDate(BASE_DATE.AddDays(50))
                    .WithExamResult(ExamResultType.Pass)
                    .WithNoConsequence(true)
                    .WithExamType(ExamType.Kci)
                    .WithSeat(BASE_DATE.AddDays(10))
                    .Build());
       }
    }
    public class ShouldNotReissueWhenNoMOCPassAndBadKCINoConsequencesWithInapplicableAdminDate : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(-15);
        }

        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new RegistrationResourceBuilder();

            _registrations.Registrations = new List<RegistrationResource>(1);
            _registrations.Registrations.Add(
                builder
                    .WithCertificationId(_certificationId)
                    .WithAdministrationYear(BASE_DATE.AddDays(10).Year)
                    .WithAdministrationDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .WithNoConsequence(true)
                    .WithExamType(ExamType.Kci)
                    .WithSeat(BASE_DATE.AddDays(10))
                    .Build());
        }
    }

    public class ShouldReissueWhenNoMOCPassAndBadKCINoConsequencesWithInapplicableAdminDateButHasLaterPass : MBMReissuedScenario
    {
        protected override void SetupRegistrations()
        {
            base.SetupRegistrations();

            var builder = new RegistrationResourceBuilder();

            _registrations.Registrations = new List<RegistrationResource>(1);
            _registrations.Registrations.Add(
                builder
                    .WithCertificationId(_certificationId)
                    .WithAdministrationYear(BASE_DATE.AddDays(10).Year)
                    .WithAdministrationDate(BASE_DATE.AddDays(10))
                    .WithExamResult(ExamResultType.Fail)
                    .WithNoConsequence(true)
                    .WithExamType(ExamType.Kci)
                    .WithSeat(BASE_DATE.AddDays(10))
                    .Build());

            _registrations.Registrations.Add(
                builder
                    .WithCertificationId(_certificationId)
                    .WithAdministrationYear(BASE_DATE.AddDays(11).Year)
                    .WithAdministrationDate(BASE_DATE.AddDays(11))
                    .WithExamResult(ExamResultType.Pass)
                    .WithNoConsequence(true)
                    .WithExamType(ExamType.Kci)
                    .WithSeat(BASE_DATE.AddDays(11))
                    .Build());
        }
    }

    #endregion Scenarios

}

