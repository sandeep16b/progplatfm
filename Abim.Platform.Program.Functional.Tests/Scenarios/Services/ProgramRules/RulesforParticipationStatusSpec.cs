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
      IWant = "to be able to utilize the Corrective Action in Program Rules service",
      SoThat = "it can handle to evaluate Participation Status for TL, MBM and GF"
      )]
    [TestFixture]
    public class RulesforParticipationStatusSpec
    {
        [TestCase]
        [WorkItem(134063)]
        public void ParticipationStatus_TL_Meet2YearLookBack_MeetingScenario()
        {
            new ParticipationStatus_TL_Meet2YearLookBack_MeetingScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void ParticipationStatus_MBM_Meet2YearLookBack_MeetingScenario()
        {
            new ParticipationStatus_MBM_Meet2YearLookBack_MeetingScenario().BDDfy();
        }

        [TestCase]
        [WorkItem(134063)]
        public void ParticipationStatus_GF_Meet2YearLookBack_MeetingScenario()
        {
            new ParticipationStatus_GF_Meet2YearLookBack_MeetingScenario().BDDfy();
        }

    }

    /// <summary>
    /// Base class for this file
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Tests.Scenarios.Controllers.ProgramRulesServiceScenario.Base.CredentialServiceScenario" />
    public abstract class ParticipationStatusSpecScenario : ProgramRulesServiceScenario
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
        // builders
        protected IssuanceDataBuilder IssuanceDataBuilder { get; set; }
        protected SourceDataBuilder SourceDataBuilder { get; set; }
        protected CertificationDataBuilder CertificationDataBuilder { get; set; }
        protected CredentialDataBuilder CredentialDataBuilder { get; set; }

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

    //**** Time Limited ******
    /// <summary>
    /// The ParticipationStatus_TL_Meet2YearLookBack_MeetingScenario
    /// </summary>
    public class ParticipationStatus_TL_Meet2YearLookBack_MeetingScenario : ParticipationStatusSpecScenario
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
            ProcessingDate = DateTime.Now;

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                            .With(a=>a.Code="ABIM")
                            .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();
            
            //Active TL issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a=>a.Source=source)
                                .With(b=>b.IssuanceStatus=IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Timelimited)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder (certification))
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

            My<ICredentialService>().Setup(p => p.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(new List<Credential>() { Credential });

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new SetIssuanceToMaintainedCommandResult(CommandStatus.Accepted, null, null));

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

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

        public void AndThenSetIssuanceToMaintainedCommandShouldBeCalled()
        {
            My<ICredentialService>()
                .Verify(p => p.Handle(It.IsAny<SetIssuanceToMaintainedCommand>()), Times.Once());
        }
    }

    //**** MBM ********
    /// <summary>
    /// The ParticipationStatus_MBM_Meet2YearLookBack_MeetingScenario
    /// </summary>
    public class ParticipationStatus_MBM_Meet2YearLookBack_MeetingScenario : ParticipationStatusSpecScenario
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
            ProcessingDate = DateTime.Now;

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                            .With(a => a.Code = "ABIM")
                            .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            //Active MBM issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Continuous)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.Required)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
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

            My<ICredentialService>().Setup(p => p.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(new List<Credential>() { Credential });

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new SetIssuanceToMaintainedCommandResult(CommandStatus.Accepted, null, null));

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

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

        public void AndThenSetIssuanceToMaintainedCommandShouldBeCalled()
        {
            My<ICredentialService>()
                .Verify(p => p.Handle(It.IsAny<SetIssuanceToMaintainedCommand>()), Times.Once());
        }
    }

    //**** GF ********
    /// <summary>
    /// The ParticipationStatus_GF_Meet2YearLookBack_MeetingScenario
    /// </summary>
    public class ParticipationStatus_GF_Meet2YearLookBack_MeetingScenario : ParticipationStatusSpecScenario
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
            ProcessingDate = DateTime.Now;

            // *********  creating domain data  *********
            Source source = SourceDataBuilder
                            .With(a => a.Code = "ABIM")
                            .Build();

            Certification certification = (new CertificationDataBuilder(source))
                                            .Build();

            //Active GF issuance NotMaintained
            Issuance issuance = IssuanceDataBuilder
                                .With(a => a.Source = source)
                                .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                .With(b => b.Duration = DurationType.Lifetime)
                                .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                .With(c=>c.Occurrence=OccurrenceType.Initial)
                                .With(b => b.MaintenanceStatus = MaintenanceStatusType.NotMaintained)
                                .Build();

            Credential = (new CredentialDataBuilder(certification))
                        .With(a=>a.AssessmentMet=true)
                        .With(a=>a.AssessmentMetDate= new DateTime(EventDate.Year,1,1))
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

            My<ICredentialService>().Setup(p => p.SearchByMemberId(It.IsAny<Guid>()))
                .Returns(new List<Credential>() { Credential });

            My<ICredentialService>().Setup(p => p.Handle(It.IsAny<SetIssuanceToMaintainedCommand>()))
                .Returns(new SetIssuanceToMaintainedCommandResult(CommandStatus.Accepted, null, null));

            // ++++++++++++ ProductInterservice ++++++++++++
            My<IProductInterservice>()
                .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                .Returns(Task.FromResult(ActivitiesFullCollectionResource));

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

        public void AndThenSetIssuanceToMaintainedCommandShouldBeCalled()
        {
            My<ICredentialService>()
                .Verify(p => p.Handle(It.IsAny<SetIssuanceToMaintainedCommand>()), Times.Once());
        }
    }

    #endregion
}
