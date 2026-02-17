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
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceDataBuilders;
using Abim.Platform.Program.Tests.Setup.Responses;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;



namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base
{
    public abstract class ProgramRulesIndividualRulesScenario : BaseServiceScenario
    {
        //++ Resource Data Builders ++++
        protected ActivityResourceDataBuilder ActivityResourceDataBuilder { get; set; }
        protected ExamResultResourceDataBuilder ExamResultResourceDataBuilder { get; set; }
        protected RegistrationResourceDataBuilder RegistrationResourceDataBuilder { get; set; }
        protected SeatRegistrationResourceDataBuilder SeatRegistrationResourceDataBuilder { get; set; }

        //++ Domain Data Builders ++++
        protected IssuanceDataBuilder IssuanceDataBuilder { get; set; }
        protected CredentialDataBuilder CredentialDataBuilder { get; set; }
        protected SourceDataBuilder SourceDataBuilder { get; set; }
        protected CertificationDataBuilder CertificationDataBuilder { get; set; }

        //*** SUT ****
        protected ProgramRulesService ProgramRulesService { get; set; }
        protected PrivateObject ProgramRulesServiceObject { get; set; }
        protected Object ResultObject { get; set; }
        protected IList<RuleResults> RuleResults { get; set; }
        protected RuleResults RuleResult { get; set; }

        // ProgramRulesService's properties/fields ++++
        protected IList<ActivityResource> UserActivities { get; set; }

        protected IList<Credential> AllCredentials { get; set; }

        protected DateTime FirstIssuanceDate { get; set; }
        protected IList<RegistrationResource> Registrations { get; set; }
        protected UserRegistrationsAndCMPRegistrationsResource AllRegistrations { get; set; }

        protected IList<LongitudinalEnrollmentSummaryResource> LongitudinalEnrollments { get; set; }

        protected string AccessToken { get; set; }

        protected Guid FPHMCertificateGuid { get; set; }

        protected ExecutingProcessType ExecutingProcess { get; set; } = ExecutingProcessType.Unkonwn;

        //-- Insput SUT parameters
        protected IList<Credential> InputCredentials { get; set; }
        protected Guid MemberId { get; set; }
        protected DateTime EventDate { get; set; }

        protected DateTime ProcessingDate { get; set; }

        public DateTime FutureActivityDate = DateTime.Now.AddDays(5);

        // *** Random value builders
        //StringBuilder stringBuilder = new StringBuilder();

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
                typeof(IMembershipClientService),
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
            Registrations = new List<RegistrationResource>();
            LongitudinalEnrollments = new List<LongitudinalEnrollmentSummaryResource>();
            AllRegistrations = new UserRegistrationsAndCMPRegistrationsResource()
            {
                Registrations = new List<RegistrationResource>(),
                CMPRegistrations = new List<CMPRegistrationResource>()
            };
            AccessToken = RandomString.Build();
            FPHMCertificateGuid = Guid.Empty;
            ExecutingProcess = ExecutingProcessType.Unkonwn;
            MemberId = Guid.NewGuid();
        }

        protected void Set_ActivitiesWithPoints(DateTime ActivityCompletedDate, decimal TotalMOCPoints)
        {
            UserActivities.Add(ActivityResourceDataBuilder
                .With(a => a.Product = new ProductResource() { Code = "Any" })
                .With(a => a.CompletedDate = ActivityCompletedDate)
                .With(a => a.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass))
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
            var issuance = IssuanceDataBuilder
                                .StartWithCredentialCategory(category: category,
                                                             issuanceDate: FirstIssuanceDate,
                                                             issuanceStatus: issuanceStatus,
                                                             occurrenceType: occurrenceType,
                                                             maintenanceStatus: maintenanceStatus,
                                                             source: null)
                                .With(a => a.ExpirationDate = expirationDate)
                                .Build();

            var credential = CredentialDataBuilder.StartWithCertCode(certCode: certificationCode,
                                                            memberId: MemberId //       credType: CredentialType.General, pathwayType: PathwayType.MOC
                                                   )
                                        .With(a => a.AssessmentMet = assessmentMet)
                                        .With(a => a.AssessmentMetDate = assessmentMetDate ?? FirstIssuanceDate)
                                        .WithList(actions)
                                        .Build();

            credential.AddIssuance(issuance);

            // keep it here for copy/paste
            if (credential.Certification.Code == ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine)
                FPHMCertificateGuid = credential.ExternalId;

            InputCredentials.Add(credential);
            AllCredentials.Add(credential);

            return credential;
        }

        // Earned NewSubspecialty Initial Cert
        protected Credential Set_NewSubspecialtyInitialCert(CredentialCategoryType category,
                                        DateTime issuanceDate,
                                        string certificationCode,
                                        IssuanceStatusType issuanceStatus = IssuanceStatusType.Active,
                                        OccurrenceType occurrenceType = OccurrenceType.Initial,
                                        MaintenanceStatusType maintenanceStatus = MaintenanceStatusType.Maintained,
                                        bool IsCosponsored = false)
        {
            var credentialNewInitialCert = CredentialDataBuilder.StartWithCertCode(certCode: certificationCode,
                                                                                    memberId: MemberId)
                                                                .With(a => a.AssessmentMet = true)
                                                                .With(a => a.AssessmentMetDate = issuanceDate)
                                                                .With(a => a.IsCosponsored = IsCosponsored)
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

            AllCredentials.Add(credentialNewInitialCert);

            return credentialNewInitialCert;
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
                                                .With(a => a.Result= certExamResult.ToString())
                                                .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Cert))
                                                .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = seatDateCert } })
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
                                    .WithList(mocActions)
                                    .Build());
        }

        protected void Set_Registration(Credential credential,
                                        DateTime administrationDate,
                                        DateTime? seatDate = null,
                                        ExamResultType examResult = ExamResultType.Pass,
                                        ExamType examType = ExamType.Moc)
        {
            seatDate = seatDate ?? administrationDate;

            AllRegistrations.
                Registrations.Add(RegistrationResourceDataBuilder
                            .With(a => a.AdministrationYear = administrationDate.Year)
                            .With(a => a.CertificationId = credential.Certification.ExternalId)
                            .With(a => a.AdministrationDate = administrationDate)
                            .With(a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(examResult) })
                            .With(a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(examType))
                            .With(a => a.Seats = new List<SeatRegistrationSummaryResource>() { new SeatRegistrationSummaryResource { SeatDate = seatDate.Value } })
                            .With(a => a.Result = examResult.ToString())
                            .Build());
        }

        protected override void PostSetup()
        {
            ProgramRulesService = Container.GetInstance<ProgramRulesService>(); //ProgramRulesService
            ProgramRulesServiceObject = new PrivateObject(ProgramRulesService);

            ProgramRulesServiceObject.SetFieldOrProperty("_userActivities", UserActivities);
            ProgramRulesServiceObject.SetFieldOrProperty("_firstIssuanceDate", FirstIssuanceDate);
            //ProgramRulesServiceObject.SetFieldOrProperty("_registrations", Registrations); // *** 
            // added in 1444 rel. 3
            ProgramRulesServiceObject.SetFieldOrProperty("_allRegistrations", AllRegistrations);

            ProgramRulesServiceObject.SetFieldOrProperty("_longitudinalEnrollments", LongitudinalEnrollments);

            ProgramRulesServiceObject.SetFieldOrProperty("_accessToken", AccessToken);
            ProgramRulesServiceObject.SetFieldOrProperty("_credentials", AllCredentials);
            ProgramRulesServiceObject.SetFieldOrProperty("_fPHMCertificateGuid", FPHMCertificateGuid);
            ProgramRulesServiceObject.SetFieldOrProperty("ExecutingProcess", ExecutingProcess);

            ProgramRulesServiceObject.SetFieldOrProperty("MemberId", MemberId);
            ProgramRulesServiceObject.SetFieldOrProperty("EventDate", EventDate);
            ProgramRulesServiceObject.SetFieldOrProperty("ProcessingDate", ProcessingDate);
        }

    }
}