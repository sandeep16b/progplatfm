using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
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
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
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
using static Abim.Platform.Program.Resources.ProgramResourceConstants;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can handle to run Time Limited Credentials rules"
      )]
    [TestFixture]
    public class RulesForMustBeMaintainedCertificateSpec
    {
        /* to-do: fix the test ----*/

        //[TestCase]
        //[WorkItem(73194)]
        ////
        //public void MustBeMaintained_EnoughMoc_EnoughMedicalKnowledge_InReciprocity()
        //{
        //    new MustBeMaintained_EnoughMoc_EnoughMedicalKnowledge_InReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_EnoughMoc_NotEnoughMedicalKnowledge_InReciprocity()
        //{
        //    new MustBeMaintained_EnoughMoc_NotEnoughMedicalKnowledge_InReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_NotEnoughMoc_EnoughMedicalKnowledge_InReciprocity()
        //{
        //    new MustBeMaintained_NotEnoughMoc_EnoughMedicalKnowledge_InReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_NotEnoughMoc_NotEnoughMedicalKnowledge_InReciprocity()
        //{
        //    new MustBeMaintained_NotEnoughMoc_NotEnoughMedicalKnowledge_InReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity()
        //{
        //    new MustBeMaintained_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_EnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity()
        //{
        //    new MustBeMaintained_EnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_NotEnoughMoc_EnoughMedicalKnowledge_NotInReciprocity()
        //{
        //    new MustBeMaintained_NotEnoughMoc_EnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_NotEnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity()
        //{
        //    new MustBeMaintained_NotEnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity().BDDfy();
        //}

        //[TestCase]
        //[WorkItem(73194)]
        //public void MustBeMaintained_DidNotPassExam()
        //{
        //    new MustBeMaintained_DidNotPassExam().BDDfy();
        //}
        //*/

        [TestCase]
        [WorkItem(137190)]
        public void Should_Not_Reissue_When_Assessment_Not_Met()
        {
            new ShouldNotReissueWhenAssessmentNotMet().BDDfy();
        }

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

        // to-do: 2 tests below need to redone and use ProgramRulesServiceScenario.cs as base
        [TestCase]
        [WorkItem(159993)]
        public void Should_Not_Issue_New_MBM_Issuance_For_Active_GF_Credential()
        {
            new ShouldNotIssueNewMBMIssuanceForActiveGFCredentialScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(159993)]
        public void Should_Issue_New_MBM_Issuance_When_Credential_Is_Not_Active_GF()
        {
            new ShouldIssueNewMBMIssuanceForCredentialWhichIsNotActiveGFScenario().BDDfy();
        }
    }

    /*
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
            var credential = Credential.Create(certification, Guid.NewGuid(), EnumAttributes.RandomEntry<CredentialType>(), EnumAttributes.RandomEntry<PathwayType>(),
                RandomString.Build());
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

            var cred = CredentialBuilder.Build(PathwayType.MOC);
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

            My<IAccessTokenService>().Setup(x => x.GetAccessToken()).Returns(Task.FromResult("SomeToken"));
            My<IAccessTokenService>().Setup(x => x.GetAccessToken()).Returns(Task.FromResult("SomeToken"));

            SetupRegistrations();

            My<IRegistrationInterservice>()
                .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(_registrations));

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
            My<IAccessTokenService>().Verify(x => x.GetAccessToken(), Times.Once);
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
    */
    #region Scenarios

    /// <summary>
    /// The all requirements met scenario
    /// </summary>
    public class MustBeMaintained_EnoughMoc_EnoughMedicalKnowledge_InReciprocity
        : MustBeMaintainedCertificateSpecScenario
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
            FirstIssuanceDate = new DateTime(Year - 10, 11, 16);
            TriggeringEvent = TriggeringEvent.ActivityCompleted;

            Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                    TotalMOCPoints: 100);

            var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                issuanceDate: FirstIssuanceDate,
                                                certificationCode: CertificationCode.InternalMedicine,
                                                issuanceStatus: IssuanceStatusType.Expired,
                                                occurrenceType: OccurrenceType.Initial,
                                                maintenanceStatus: MaintenanceStatusType.Maintained,
                                                expirationDate: new DateTime(2018, 10, 10),
                                                assessmentMet: true,
                                                assessmentMetDate: FirstIssuanceDate,
                                                actions: null);

            Set_SuT_Registration(credential: credential,
                                administrationDate: FirstIssuanceDate,
                                seatDateCert: FirstIssuanceDate);

            Set_CurrentLookBackDatesInfo();
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

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), 
                                                            issuanceDate: new DateTime(Year - 3, 01, 11),
                                                            DurationType: DurationType.Timelimited); // need to set up Expired and MaintenanceRequirement
            credential.Issuances[0].IssuanceStatus = IssuanceStatusType.Expired;
            credential.Issuances[0].MaintenanceRequirement = MaintenanceRequirementType.NotRequired;
            CredentialId = credential.ExternalId;
            credential.AssessmentMet = true;
            credential.AssessmentMetDate = DateTime.Now.AddYears(-2);
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));

            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            //Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            //Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);
            //Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
            //    medicalKnowledgePoints: 20);

            // ++++++++++++ RegistrationInterservice ++++++++++++
            My<IRegistrationInterservice>().Setup(p => p.GetUserRegistrations(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(RegistrationFullCollectionResource));

            My<IRegistrationInterservice>().Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(AllRegistrations));

            // ++++++++++++ Credential Service ++++++++++++
            My<ICredentialService>().Setup(p => p.Load(It.IsAny<Guid>()))
                .Returns(InputCredentials[0]);

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            // ++++++++++++ Log ++++++++++++
            ProgramRulesService.Log = Log.Object;
            LogTest.Watch(Log);
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                ProcessingDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.AtLeastOnce());
        }
    }
    
    /// <summary>
    /// The scenario in which there aren't enough MOC points, but there are enough medical knowledge points, and in reciprocity
    /// </summary>
    public class MustBeMaintained_NotEnoughMoc_EnoughMedicalKnowledge_InReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);            //90 (not enough points)
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 20);
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05), cancelledDate: new DateTime(Year-10, 11, 11));
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci, certificationId: Guid.Empty);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.AtLeastOnce());
        }
    }
    
    /// <summary>
    /// The scenario in which there are enough MOC points but not medical knowledge points, and in reciprocity
    /// </summary>
    public class MustBeMaintained_EnoughMoc_NotEnoughMedicalKnowledge_InReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 10);                                                                        //10 (not enough points)
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));
            //Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05), cancelledDate: new DateTime(Year, 11, 11));
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci, certificationId: Guid.Empty);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.AtLeastOnce());
        }
    }
    
    /// <summary>
    /// The scenario in which there aren't enough MOC points or medical knowledge points, but in reciprocity
    /// </summary>
    public class MustBeMaintained_NotEnoughMoc_NotEnoughMedicalKnowledge_InReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);            //90 (not enough points)
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 10);                                                                            //10 (not enough points)
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05));
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05), cancelledDate: new DateTime(Year, 11, 11));
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci, certificationId: Guid.Empty);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.AtLeastOnce());
        }
    }
    
    /// <summary>
    /// The scenario in which there are enough MoC points and medical knowledge points, but the user is not in reciprocity
    /// </summary>
    public class MustBeMaintained_EnoughMoc_EnoughMedicalKnowledge_NotInReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 20);
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 3, 05, 05));                              //not in valid range 1/1/Year - 2-09/01/Year
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.AtLeastOnce());
        }
    }
    
    /// <summary>
    /// The scenario in which there are enough MOC points but not medical knowledge points, and not in reciprocity
    /// </summary>
    public class MustBeMaintained_EnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 100);
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 10);                                                                            //10 (not enough points)
            //Condition_InReciprocityProgram(completedDate: new DateTime(Year - 13, 05, 05));                              //not in valid range 1/1/Year - 2-09/01/Year
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }
    
    /// <summary>
    /// The scenario in which there aren't enough MOC points but there are enough medical knowledge points, and not in reciprocity
    /// </summary>
    public class MustBeMaintained_NotEnoughMoc_EnoughMedicalKnowledge_NotInReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);            //90 (not enough points)
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 20);
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 13, 05, 05));                              //not in valid range 1/1/Year - 2-09/01/Year
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId:Guid.Empty);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }
    
    /// <summary>
    /// The scenario in which there aren't enough MOC points or medical knowledge points, and also not in reciprocity
    /// </summary>
    public class MustBeMaintained_NotEnoughMoc_NotEnoughMedicalKnowledge_NotInReciprocity
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);            //90 (not enough points)
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 10);                                                                            //10 (not enough points)
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 13, 05, 05));                              //not in valid range 1/1/Year - 2-09/01/Year
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 2, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.Kci);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Accepted, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }
    
    /// <summary>
    /// The scenario in which user didn't pass their exam
    /// </summary>
    public class MustBeMaintained_DidNotPassExam
        : MustBeMaintainedCertificateSpecScenario
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

            // ***  AccessTokenServiceMock ---
            My<IAccessTokenService>()
               .Setup(o => o.GetAccessToken())
               .Returns("--token--");

            var credential = CreateCredentialWithIssuance(issuanceExpirationDate: new DateTime(DateTime.Now.Year, 12, 31), issuanceDate: new DateTime(Year - 3, 01, 11));
            CredentialId = credential.ExternalId;
            My<ICredentialService>().Setup(p => p.Load(CredentialId))
                .Returns(credential);
            
            My<ICredentialService>().Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                .Returns(new DateTime(Year - 10, 11, 16));
            My<IProductInterservice>().Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));
            
            Condition_ICARDAttest_Activity(icardAttestation_completedDate: new DateTime(Year - 3, 05, 05));
            Condition_MOCPoints_Activity(completedDate: new DateTime(Year - 1, 01, 01), totalMOCPoints: 90);            //90 (not enough points)
            Condition_MedicalKnowledgePoints_ActivityCredit(creditDate: new DateTime(Year, 1, 1), expirationDate: new DateTime(Year + 10, 1, 1),
                medicalKnowledgePoints: 10);                                                                            //10 (not enough points)
            Condition_InReciprocityProgram(completedDate: new DateTime(Year - 1, 05, 05), cancelledDate: new DateTime(Year, 11, 11));
            
            var otherIssuance = CreateActiveExpiringIssuance(issuanceDate: new DateTime(Year, 1, 1));
            My<ICredentialService>().Setup(p => p.GetIssuancesForMemberId(It.IsAny<Guid>()))
                .Returns(new List<Issuance>(){ otherIssuance });
            
            Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 11, 01, 11), examType: ExamType.Moc, certificationId: Guid.Empty);
            //Condition_RegistrationInterservice_GetUserRegistrations(resultDate: new DateTime(Year - 1, 11, 11), examType: ExamType.TwoYear);
            
            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult(CommandStatus.Rejected, null, null));
        }
        
        public void GivenInputAValidCommand()
        {
            Command = new RunRulesForMustBeMaintainedCertificateCommand()
            {
                CredentialId = CredentialId,
                IssuanceId = IssuanceId,
                EventDate = new DateTime(DateTime.Now.Year, 9, 1),
                CreatedBy = "Test" // per PBI
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

        public void AndThenReissueCommandShouldNotBeCalled()
        {
            My<ICredentialService>().Verify(p => p.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never());
        }
    }

    public class ShouldNotReissueWhenAssessmentNotMet : MBMNotReissuedScenario
    {
        //Default setup and methods in base class MBMNotReissuedScenario handle this scenario as-is
    }

    /*
    public class ShouldNotReissueWhenAssessmentMetAfterEvalDate : MBMNotReissuedScenario
    {
        protected override void SetupCredentials()
        {
            base.SetupCredentials();
            _credentials[0].AssessmentMet = true;
            _credentials[0].AssessmentMetDate = BASE_DATE.AddDays(15);
        }
    }
    */
    /*
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
    */
    /*
    public class ShouldReissueWhenNoMOCPassButNoBadKCIorCMPNoConsequencesExam : MBMReissuedScenario
    {
        //Default setup and methods in base class MBMReissuedScenario handle this scenario as-is
    }
    */
    /*
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
    */

    /*
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
    */

    /*
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
    */

    /*
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
    */

    /*
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
    */

    /*
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
    */

    /*
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
    */

    /*
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
    */

    /// <summary>
    /// Test that verifies that we aren't creating new MBM issuances for active Grandfathers
    /// </summary>
    public class ShouldNotIssueNewMBMIssuanceForActiveGFCredentialScenario : ProgramRulesServiceSimplifiedScenario
    {
        private Credential _cred;
        private RegistrationResource _reg;
        DateTime _eventDate;
        DateTime _processingDate;
        RunRulesForMustBeMaintainedCertificateCommand _command;

        public ShouldNotIssueNewMBMIssuanceForActiveGFCredentialScenario()
        {
            SetupCredential();
            SetupRegistration();
        }

        protected override void SetupCredentialServiceMock()
        {
            base.SetupCredentialServiceMock();
            _credSvcMock
                .Setup(mock => mock.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult());

            _credSvcMock
                .Setup(x => x.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(new List<Credential>(0));

            _credSvcMock
                .Setup(mock => mock.Load(It.IsAny<Guid>()))
                .Returns(_cred);
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

            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource()
            {
                Registrations = new List<RegistrationResource>(),
                CMPRegistrations = new List<CMPRegistrationResource>()
            };

            registrationsMock.Registrations.Add(_reg);

            _regInterSvcMock
                .Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));
        }

        protected  void SetupCredential()
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
                    DateTime.Now.AddYears(-10),
                    DurationType.Lifetime,
                    MaintenanceRequirementType.NotRequired,
                    MaintenanceStatusType.NotMaintained,
                    OccurrenceType.Initial));

            var timeLimitedIssuance =
                IssuanceBuilder.BuildWithoutRandoms(
                    source,
                    IssuanceStatusType.Active,
                    DateTime.Now.AddYears(-3),
                    DurationType.Timelimited,
                    MaintenanceRequirementType.Required,
                    MaintenanceStatusType.Maintained,
                    OccurrenceType.Recertification);

            timeLimitedIssuance.ExpirationDate = DateTime.Now.AddYears(-1);

            _cred.AddIssuance(timeLimitedIssuance);

            _cred.LookbackDate = new DateTime(2019, 12, 31);
            _cred.ExamDueDate = new DateTime(2019, 12, 31);
        }

        protected void SetupRegistration()
        {
            var builder = new RegistrationResourceBuilder();
            _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .Build();
        }

        protected void GivenIHaveAValidCommand()
        {
            _eventDate = new DateTime(2019, 11, 30);
            _processingDate = new DateTime(2019, 12, 1);
            _command = 
                new RunRulesForMustBeMaintainedCertificateCommand
                {
                    CreatedBy = "UnitTest",
                    CredentialId = _cred.ExternalId,
                    EventDate = _eventDate,
                    IssuanceId = 0,
                    ProcessingDate = _processingDate,
                    UserInfo = null
                };

        }

        protected async void WhenICallHandleJob()
        {
            try
            {
                _sut.HandleJob(_command);
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

        protected void AndNoNewMBMIssuanceShouldHaveBeenCreated()
        {
            _credSvcMock.Verify(mock => mock.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Never);
        }
    }

    /// <summary>
    /// Test that verifies that we are creating new MBM issuance for when credential is not active Grandfather
    /// </summary>
    public class ShouldIssueNewMBMIssuanceForCredentialWhichIsNotActiveGFScenario : ProgramRulesServiceSimplifiedScenario
    {
        private Credential _cred;
        private RegistrationResource _reg;
        DateTime _eventDate;
        DateTime _processingDate;
        RunRulesForMustBeMaintainedCertificateCommand _command;

        public ShouldIssueNewMBMIssuanceForCredentialWhichIsNotActiveGFScenario()
        {
            SetupCredential();
            SetupRegistration();
        }

        protected override void SetupCredentialServiceMock()
        {
            base.SetupCredentialServiceMock();
            _credSvcMock
                .Setup(mock => mock.Handle(It.IsAny<ExpireAndReissueCommand>()))
                .Returns(new ExpireAndReissueCommandResult());

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

            _credSvcMock
                .Setup(mock => mock.Load(It.IsAny<Guid>()))
                .Returns(_cred);
        }

        protected override void SetupProductInterserviceMock()
        {
            base.SetupProductInterserviceMock();
            var activities = new ActivityFullCollectionResource();
            var builder = new ActivityResourceBuilder();
            var activity = 
                builder
                    .WithTotalMOCPoints(100)
                    .WithCompletedDate(DateTime.Now.AddDays(-10))
                    .WithActivityResult(ActivityResultType.Pass)
                    .Build();

            activities.Data = new List<ActivityResource>(1) { activity };
            _prodInterSvcMock
                .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(activities));
        }

        protected override void SetupRegistrationInterserviceMock()
        {
            base.SetupRegistrationInterserviceMock();
            //var registrations = new RegistrationFullCollectionResource();
            //registrations.Data = new List<RegistrationResource>(1) { _reg };

            //_regInterSvcMock
            //    .Setup(x => x.GetUserRegistrations(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<Guid>()))
            //    .Returns(Task.FromResult(registrations));

            var registrationsMock = new UserRegistrationsAndCMPRegistrationsResource()
            {
                Registrations = new List<RegistrationResource>(),
                CMPRegistrations = new List<CMPRegistrationResource>()
            };

            registrationsMock.Registrations.Add(_reg);

            _regInterSvcMock
                .Setup(p => p.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                .Returns(Task.FromResult(registrationsMock));
        }

        protected void SetupCredential()
        {
            var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "Unit Test");
            _cred =
                CredentialBuilder
                    .BuildWithoutRandoms(
                        source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, Resources.PathwayType.MOC);
            _cred.AssessmentMet = true;
            _cred.AssessmentMetDate = DateTime.Now.AddDays(-10);

            _cred.AddIssuance(
                IssuanceBuilder.BuildWithoutRandoms(
                    source,
                    IssuanceStatusType.Active,
                    DateTime.Now.AddYears(-10),
                    DurationType.Timelimited,
                    MaintenanceRequirementType.NotRequired,
                    MaintenanceStatusType.NotMaintained,
                    OccurrenceType.Initial));

            var timeLimitedIssuance =
                IssuanceBuilder.BuildWithoutRandoms(
                    source,
                    IssuanceStatusType.Active,
                    DateTime.Now.AddYears(-3),
                    DurationType.Timelimited,
                    MaintenanceRequirementType.NotRequired,
                    MaintenanceStatusType.Maintained,
                    OccurrenceType.Recertification);

            timeLimitedIssuance.ExpirationDate = DateTime.Now.AddYears(-1);

            _cred.AddIssuance(timeLimitedIssuance);

            _cred.LookbackDate = new DateTime(2019, 12, 31);
            _cred.ExamDueDate = new DateTime(2019, 12, 31);
        }

        protected void SetupRegistration()
        {
            var builder = new RegistrationResourceBuilder();
            _reg = builder
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Moc)
                    .WithSeat(DateTime.Now.AddDays(-10))
                    .WithAdministrationYear(DateTime.Now.AddDays(-10).Year)
                    .Build();
        }

        protected void GivenIHaveAValidCommand()
        {
            _eventDate = new DateTime(2019, 11, 30);
            _processingDate = new DateTime(2019, 12, 1);
            _command =
                new RunRulesForMustBeMaintainedCertificateCommand
                {
                    CreatedBy = "UnitTest",
                    CredentialId = _cred.ExternalId,
                    EventDate = _eventDate,
                    IssuanceId = 0,
                    ProcessingDate = _processingDate,
                    UserInfo = null
                };

        }

        protected async void WhenICallHandleJob()
        {
            try
            {
                _sut.HandleJob(_command);
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

        protected void AndNoNewMBMIssuanceShouldHaveBeenCreated()
        {
            _credSvcMock.Verify(mock => mock.Handle(It.IsAny<ExpireAndReissueCommand>()), Times.Once);
        }
    }

    #endregion Scenarios

}

