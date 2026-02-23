using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceDataBuilders;
using Abim.Platform.Program.Tests.Setup.Responses;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesServiceTest.Base
{
    public abstract class ProgramRulesServiceScenario : BaseServiceScenario
    {
        //++ Resource Data Builders ++++
        protected ActivityResourceDataBuilder ActivityResourceDataBuilder { get; set; }
        protected ExamResultResourceDataBuilder ExamResultResourceDataBuilder { get; set; }
        protected RegistrationResourceDataBuilder RegistrationResourceDataBuilder { get; set; }
        protected SeatRegistrationResourceDataBuilder SeatRegistrationResourceDataBuilder { get; set; }

        protected CMPRegistrationResourceBuilder CMPRegistrationResourceBuilder { get; set; }
        protected CMPExamSummaryResourceBuilder CMPExamSummaryResourceBuilder { get; set; }

        //++ Domain Data Builders ++++
        protected IssuanceDataBuilder IssuanceDataBuilder { get; set; }
        protected CredentialDataBuilder CredentialDataBuilder { get; set; }
        protected SourceDataBuilder SourceDataBuilder { get; set; }
        protected CertificationDataBuilder CertificationDataBuilder { get; set; }

        //*** SUT ****
        protected IProgramRulesService ProgramRulesService { get; set; }
        protected PrivateObject ProgramRulesServiceObject { get; set; }
        protected Object ResultObject { get; set; }
        protected IList<RuleResults> RuleResults { get; set; }
        protected RuleResults RuleResult { get; set; }

        // ProgramRulesService's properties/fields ++++
        protected IList<ActivityResource> UserActivities { get; set; }

        protected IList<Credential> AllCredentials { get; set; }

        protected DateTime FirstIssuanceDate { get; set; }
        protected IList<RegistrationResource> Registrations { get; set; }
        protected RegistrationResource Registration { get; set; }
        protected UserRegistrationsAndCMPRegistrationsResource AllRegistrations { get; set; }

        protected List<LongitudinalEnrollmentSummaryResource> LongitudinalEnrollments { get; set; }

        protected string AccessToken { get; set; }

        protected Guid FPHMCertificateGuid { get; set; }

        protected ExecutingProcessType ExecutingProcess { get; set; } = ExecutingProcessType.Unkonwn;
        protected bool Result { get; set; }

        //-- special case
        protected ActivityFullCollectionResource ActivityFullCollectionResource { get; set; }
        protected RegistrationFullCollectionResource RegistrationFullCollectionResource { get; set; }

        protected Tuple<DateTime?, DateTime?> LookBackEndDates { get; set; }
        protected LookBackDatesInfo CurrentLookBackDatesInfo { get; set; }

        //-- Insput SUT parameters
        protected List<Credential> InputCredentials { get; set; }
        protected Credential InputCredential { get; set; }
        protected Issuance InputIssuance { get; set; }
        protected Guid MemberId { get; set; }
        protected DateTime EventDate { get; set; }

        protected TriggeringEvent TriggeringEvent { get; set; }

        protected DateTime ProcessingDate { get; set; }
        protected Mock<NLog.ILogger> Log { get; set; }

        public class ExpectedTestResult
        {
            public DateTime? ExamDueDate { get; set; }
            public DateTime? DisplayExamDueDate { get; set; }

            public DateTime? KCIExamDueDate { get; set; }

            public bool ForcedPathway { get; set; }
            public Resources.PathwayType Pathway { get; set; }
            public int ExamFailCount { get; set; }
            public bool AssessmentMet { get; set; }
            public DateTime? AssessmentMetDate { get; set; }
            public DateTime? GracePeriodEndDate { get; set; }
            public DateTime? GracePeriodStartDate { get; set; }
            public bool ExpireActiveIssuances { get; set; }
        }


        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>()
            {
                typeof(ICertificationService),
                typeof(ICredentialService),
                typeof(ISourceService),
                typeof(IProductInterservice),
                typeof(IRegistrationInterservice),
                typeof(IBusControl),
                typeof(IBackgroundJobClient),
                typeof(IValidationFactory),
                typeof(IAccessTokenService),
                typeof(ICorrectiveActionResultService),
                typeof(IProfileInterservice),
                typeof(ILookBackDatesInfoService),
                typeof(ILookbackLogService)
            };
        }


        protected void InitializeBuilders()
        {
            ActivityResourceDataBuilder = new ActivityResourceDataBuilder();
            RegistrationResourceDataBuilder = new RegistrationResourceDataBuilder();
            ExamResultResourceDataBuilder = new ExamResultResourceDataBuilder();
            SeatRegistrationResourceDataBuilder = new SeatRegistrationResourceDataBuilder();

            CMPRegistrationResourceBuilder = new CMPRegistrationResourceBuilder();
            CMPExamSummaryResourceBuilder = new CMPExamSummaryResourceBuilder();

            IssuanceDataBuilder = new IssuanceDataBuilder();
            CredentialDataBuilder = new CredentialDataBuilder();
            CertificationDataBuilder = new CertificationDataBuilder();
            SourceDataBuilder = new SourceDataBuilder();
        }

        protected void InitializeDataProperties()
        {
            InputCredentials = new List<Credential>();
            AllCredentials = new List<Credential>();
            UserActivities = new List<ActivityResource>();

            ActivityFullCollectionResource = new ActivityFullCollectionResource();
            RegistrationFullCollectionResource = new RegistrationFullCollectionResource();

            Registrations = new List<RegistrationResource>();
            AllRegistrations = new UserRegistrationsAndCMPRegistrationsResource()
            {
                Registrations = new List<RegistrationResource>(),
                CMPRegistrations = new List<CMPRegistrationResource>()
            };

            LongitudinalEnrollments = new List<LongitudinalEnrollmentSummaryResource>();

            AccessToken = RandomString.Build();
            FPHMCertificateGuid = Guid.Empty;
            ExecutingProcess = ExecutingProcessType.Unkonwn;
            MemberId = Guid.NewGuid();
        }

        protected void Initialize()
        {
            Log = new Mock<NLog.ILogger>();
        }

        protected void Set_ActivitiesWithPoints(DateTime ActivityCompletedDate, decimal TotalMOCPoints)
        {
            UserActivities.Add(ActivityResourceDataBuilder
                .With(a => a.Product = new ProductResource() { Code = "Any" })
                .With(a => a.CompletedDate = ActivityCompletedDate)
                .With(a => a.ActivityResult = new Product.Extensions.ExternalResponses.EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass))
                .With(a => a.TotalMOCPoints = TotalMOCPoints)
                .With(a => a.ActivityCredits = new List<ActivityCreditResource>()
                    {
                                    //-- MedicalKnowledgePoints
                                    new ActivityCreditResource()
                                            {
                                                CreditDate = ActivityCompletedDate,
                                                CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.MedicalKnowledgePoints },
                                                Claimed=true,
                                                CreditEarned = TotalMOCPoints/2
                                            },
                                    //-- PracticeAssessement
                                    new ActivityCreditResource()
                                        {
                                            CreditDate = ActivityCompletedDate,
                                            CreditType = new CreditTypeResource() { Value = ProductResourceConstants.CreditTypeValue.PracticeAssessement },
                                            Claimed=true,
                                            CreditEarned = TotalMOCPoints/2
                                        }
                    }
                )
                .Build());

            ActivityFullCollectionResource.Data.AddRange(UserActivities);

        }

        protected void Set_Reciprocity (DateTime ActivityCompletedDate)
        {
            ActivityFullCollectionResource.Data.Add(ActivityResourceDataBuilder.WithReciprocity(ActivityCompletedDate: ActivityCompletedDate)
                                            .Build());
        }


        protected Credential Set_SuT_Credential(CredentialCategoryType category,
                                        DateTime issuanceDate,
                                        string certificationCode,
                                        IssuanceStatusType issuanceStatus = IssuanceStatusType.Active,
                                        OccurrenceType occurrenceType = OccurrenceType.Initial,
                                        MaintenanceStatusType maintenanceStatus = MaintenanceStatusType.Maintained,
                                        DateTime? expirationDate = null,
                                        bool assessmentMet = true,
                                        DateTime? assessmentMetDate = null,
                                        List<Action<Credential>> actions = null)
        {
            InputIssuance = IssuanceDataBuilder
                                .StartWithCredentialCategory(category: category,
                                                             issuanceDate: FirstIssuanceDate,
                                                             issuanceStatus: issuanceStatus,
                                                             occurrenceType: occurrenceType,
                                                             maintenanceStatus: maintenanceStatus,
                                                             source: null)
                                .With(a => a.ExpirationDate = expirationDate)
                                .Build();

            InputCredential = CredentialDataBuilder.StartWithCertCode(certCode: certificationCode,
                                                            memberId: MemberId //       credType: CredentialType.General, pathwayType: PathwayType.MOC
                                                   )
                                        .With(a => a.AssessmentMet = assessmentMet)
                                        .With(a => a.AssessmentMetDate = assessmentMetDate ?? FirstIssuanceDate)
                                        .WithList(actions)
                                        .Build();

            InputCredential.AddIssuance(InputIssuance);

            // keep it here for copy/paste
            if (InputCredential.Certification.Code ==  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine)
                FPHMCertificateGuid = InputCredential.ExternalId;

            InputCredentials.Add(InputCredential);
            AllCredentials.Add(InputCredential);

            return InputCredential;
        }

        // Earned NewSubspecialty Initial Cert
        protected Credential Set_NewSubspecialtyInitialCert(CredentialCategoryType category,
                                        DateTime issuanceDate,
                                        string certificationCode,
                                        IssuanceStatusType issuanceStatus = IssuanceStatusType.Active,
                                        OccurrenceType occurrenceType = OccurrenceType.Initial,
                                        MaintenanceStatusType maintenanceStatus = MaintenanceStatusType.Maintained)
        {
            var credentialNewInitialCert = CredentialDataBuilder.StartWithCertCode(certCode: certificationCode,
                                                                                    memberId: MemberId)
                                                                .With(a => a.AssessmentMet = true)
                                                                .With(a => a.AssessmentMetDate = issuanceDate)
                                                                .Build()
                                                                .AddIssuances(new List<Issuance>() {
                                                                                         IssuanceDataBuilder
                                                                                        .StartWithCredentialCategory(category: category,
                                                                                                                     issuanceDate: issuanceDate,
                                                                                                                     issuanceStatus: issuanceStatus,
                                                                                                                     occurrenceType: occurrenceType,
                                                                                                                     maintenanceStatus: maintenanceStatus,
                                                                                                                     source: null)
                                                                                        .Build() });

            InputCredentials.Add(credentialNewInitialCert);
            AllCredentials.Add(credentialNewInitialCert);

            return credentialNewInitialCert;
        }

        protected void Set_Sut_CmpRegistration (Credential credential,
                                                DateTime testDate,
                                                ExamResultType examResult = ExamResultType.Pass)
        {

            AllRegistrations.
                CMPRegistrations.Add(CMPRegistrationResourceBuilder
                                    .WithCMPExam(CMPExamSummaryResourceBuilder.WithCertificationId(credential.Certification.ExternalId).Build())
                                    .WithTestDate(testDate)
                                    .WithExamResult(examResult)
                                    .Build());

        }

        protected void Set_SuT_Registration(Credential credential,
                                                DateTime administrationDate,
                                                DateTime seatDateCert,
                                                DateTime? seatDateMOC = null,
                                                bool withMOC = true,
                                                ExamResultType certExamResult = ExamResultType.Pass,
                                                ExamResultType mocExamResult = ExamResultType.Pass,
                                                List<Action<RegistrationResource>> certActions = null,
                                                List<Action<RegistrationResource>> mocActions = null)
        {
            seatDateMOC = seatDateMOC ?? seatDateCert.AddYears(9);

            // registration initial certification
            AllRegistrations.
                Registrations.Add(RegistrationResourceDataBuilder
                                          .With(a => a.AdministrationYear = administrationDate.Year)
                                          .With(a => a.CertificationId = credential.Certification.ExternalId)
                                          .With(a => a.AdministrationDate = administrationDate)
                                          .With(a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(certExamResult) })
                                          .With(a => a.Result = certExamResult.ToString())
                                          .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Cert))
                                          .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = seatDateCert } })
                                          .With(a => a.Id = Guid.NewGuid())
                                          .With(a => a.AdministrationId = Guid.NewGuid())
                                          .WithList(certActions)
                                          .Build());

            if (withMOC)
                // registration MOC 
                AllRegistrations.
                    Registrations.Add(RegistrationResourceDataBuilder
                                    .With(a => a.AdministrationYear = administrationDate.Year + 9)
                                    .With(a => a.CertificationId = credential.Certification.ExternalId)
                                    .With(a => a.AdministrationDate = new DateTime(administrationDate.Year + 9, 11, 02))
                                    .With(a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(mocExamResult) })
                                    .With(a => a.Result = mocExamResult.ToString())
                                    .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc))
                                    .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = seatDateMOC.Value } })
                                    .With(a => a.Id = Guid.NewGuid())
                                    .With(a => a.AdministrationId = Guid.NewGuid())
                                    .WithList(mocActions)
                                    .Build());

            RegistrationFullCollectionResource.Data.AddRange(Registrations);
        }

        protected RegistrationResource Set_Registration(Credential credential,
                                        DateTime administrationDate,
                                        DateTime? seatDate = null,
                                        ExamResultType examResult = ExamResultType.Pass,
                                        ExamType examType = ExamType.Moc,
                                        List<Action<RegistrationResource>> actions = null)
        {
            seatDate = seatDate ?? administrationDate;

            RegistrationResource registration =
                RegistrationResourceDataBuilder
                            .With(a => a.AdministrationYear = administrationDate.Year)
                            .With(a => a.CertificationId = credential.Certification.ExternalId)
                            .With(a => a.AdministrationDate = administrationDate)
                            .With(a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(examResult) })
                            .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(examType))
                            .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = seatDate.Value } })
                            .With(a => a.Result = examResult.ToString())
                            .With(a=> a.Id = Guid.NewGuid())
                            .With(a=> a.AdministrationId = Guid.NewGuid())
                            .WithList(actions)
                            .Build();

            AllRegistrations.Registrations.Add(registration);

            RegistrationFullCollectionResource.Data.AddRange(AllRegistrations.Registrations);

            return registration;
        }

        protected void Set_CurrentLookBackDatesInfo ()
        {
            //Not Expired Two but Expired Five year look back
            LookBackEndDates = new Tuple<DateTime?, DateTime?>(new DateTime(ProcessingDate.Year + 1, 12, 31), new DateTime(ProcessingDate.Year + 1, 12, 31));
            CurrentLookBackDatesInfo = LookBackDatesInfo.Create(
                                memberId: Guid.NewGuid(),
                                lookback2YearStartDate: new DateTime(LookBackEndDates.Item1.Value.Year - 1, 1, 1),
                                lookback2YearEndDate: LookBackEndDates.Item1,
                                lookback5YearStartDate: new DateTime(LookBackEndDates.Item2.Value.Year - 4, 1, 1),
                                lookback5YearEndDate: LookBackEndDates.Item2,
                                createdBy: "");
        }

    }


}
