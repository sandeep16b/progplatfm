using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Setup.Responses;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesIndividualRule
{
    [Story(
       AsA = "controller, background job, or bus consumer",
       IWant = "to be able to utilize the Program Rules Individual Rules",
       SoThat = "it can run the corrective action process"
   )]
    [TestFixture]
    public class CorrectiveActionGFPrintingSpec
    {
        [Test]
        public void CorrectiveActionGFPrintingMeetRules100Points()
        {
            new CorrectiveActionGFPrinting_MeetRules_100_Points().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingFutureDatesMeetRules100Points()
        {
            new CorrectiveActionGFPrinting_FutureDates_MeetRules_100_Points().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingMeetRulesReciprocity()
        {
            new CorrectiveActionGFPrinting_MeetRules_Reciprocity().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingMeetRulesNewSubspecialtyInitial()
        {
            new CorrectiveActionGFPrinting_MeetRules_NewSubspecialtyInitial().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingMeetRulesPassKciExam()
        {
            new CorrectiveActionGFPrinting_MeetRules_PassKCIExam().BDDfy();
        }

        //***  negative cases 
        [Test]
        public void CorrectiveActionGFPrintingDontMeetRulesFailExamRequirement()
        {
            new CorrectiveActionGFPrinting_DontMeetRules_FailExamRequirement().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingDontMeetRulesFail5YearLookBack()
        {
            new CorrectiveActionGFPrinting_DontMeetRules_Fail5YearLookBack().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingDontMeetRulesPendingKCIExam()
        {
            new CorrectiveActionGFPrinting_MeetRules_PendingKCIExam().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrintingDontMeetRulesFailMocExam()
        {
            new CorrectiveActionGFPrinting_MeetRules_FailMocExam().BDDfy();
        }

        //PBI 180686 : Update Program Rule 45 Print Pre-1990 Certs to Include LNG
        [Test]
        public void CorrectiveActionGFPrinting_MeetLkaParticipationInFirstYearAndStatusIsActive_MeetStep()
        {
            new CorrectiveActionGFPrinting_MeetParticipationInFirstYearAndStatusIsActive_MeetStep().BDDfy();
        }

        [Test]
        public void CorrectiveActionGFPrinting_MeetLkaParticipationInFirstYearAndStatusNotActive_DontMeetStep()
        {
            new CorrectiveActionGFPrinting_MeetParticipationInFirstYearAndStatusNotActive_DontMeetStep().BDDfy();
        }

        #region Common Spec Scenarios
        private abstract class CorrectiveActionGFPrintingSpecScenario : ProgramRulesIndividualRulesScenario
        {
        }

        #endregion Scenarios

        #region Scenarios

        private class CorrectiveActionGFPrinting_MeetRules_100_Points : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }
        }

        private class CorrectiveActionGFPrinting_FutureDates_MeetRules_100_Points : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = DateTime.Now;
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 95m);
                // future dates
                Set_ActivitiesWithPoints(ActivityCompletedDate: FutureActivityDate,
                        TotalMOCPoints: 10m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }

            public void ThenIssuanceDateShouldBeInFutureToo()
            {
                RuleResult.CorrectiveActionResult.IssuanceDate.Should().Equals(FutureActivityDate);
            }
        }

        private class CorrectiveActionGFPrinting_MeetRules_Reciprocity : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m); // !!!!

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                //--- ReciprocityAttest
                UserActivities.Add(ActivityResourceDataBuilder.WithReciprocity(ActivityCompletedDate: ActivityCompletedDate)
                                                .Build());
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }
        }

        private class CorrectiveActionGFPrinting_MeetRules_NewSubspecialtyInitial : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                // Earned NewSubspecialty Initial Cert
                Set_NewSubspecialtyInitialCert(issuanceDate: EventDate.AddYears(-1),
                                                category: CredentialCategoryType.MustBeMaintained,
                                                certificationCode: "GERI");
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }
        }

        private class CorrectiveActionGFPrinting_MeetRules_PassKCIExam : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: true,
                                    // set Pass KCI exam
                                    mocActions: new List<Action<RegistrationResource>>()
                                        {
                                            (a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci)),
                                            (a => a.ExamResult = new ExamResultResource() { Result = new RegistrationEnumValueResponseResource<ExamResultType>(ExamResultType.Pass) })
                                        }
                                    );
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }
        }

        // negative cases 
        private class CorrectiveActionGFPrinting_DontMeetRules_FailExamRequirement : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate,
                                    assessmentMet: false);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: false);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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

            public void ThenResultShouldBeFalse()
            {
                RuleResult.MeetRuleRequirement.Should().Be(false);
            }
        }

        private class CorrectiveActionGFPrinting_DontMeetRules_Fail5YearLookBack : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate,
                                    assessmentMet: true);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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

            public void ThenResultShouldBeFalse()
            {
                RuleResult.MeetRuleRequirement.Should().Be(false);
            }
        }

        private class CorrectiveActionGFPrinting_MeetRules_PendingKCIExam : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: true,
                                    mocExamResult: ExamResultType.Pending,
                                    // set KCI exam
                                    mocActions: new List<Action<RegistrationResource>>()
                                        {
                                            (a => a.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci))
                                        }
                                    );
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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

            public void ThenResultShouldBeFaill()
            {
                RuleResult.MeetRuleRequirement.Should().Be(false);
            }
        }

        private class CorrectiveActionGFPrinting_MeetRules_FailMocExam : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: true,
                                    // set Fail MOC exam
                                    mocExamResult: ExamResultType.Fail);
            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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

            public void ThenResultShouldBeFaill()
            {
                RuleResult.MeetRuleRequirement.Should().Be(false);
            }
        }

        private class CorrectiveActionGFPrinting_MeetParticipationInFirstYearAndStatusIsActive_MeetStep : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate,
                                    assessmentMet: false);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: false);


                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = credential.Certification.ExternalId;
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Active); // !!!
                lngSummaryResource.IsActive = true;
                lngSummaryResource.LongitudinalParticipations = new List<LongitudinalParticipationSummaryResource>();
                lngSummaryResource.LongitudinalParticipations.Add(new LongitudinalParticipationSummaryResource() {
                                    Cycle = 1,
                                    PathwayYear = 1,
                                    Status = new RegistrationEnumValueResponseResource<ParticipationStatusType>(ParticipationStatusType.Met)});

                LongitudinalEnrollments.Add(lngSummaryResource);

            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }
        }

        private class CorrectiveActionGFPrinting_MeetParticipationInFirstYearAndStatusNotActive_DontMeetStep : CorrectiveActionGFPrintingSpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate,
                                    assessmentMet: false);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: false);


                LongitudinalEnrollmentSummaryResource lngSummaryResource = new LongitudinalEnrollmentSummaryResource();

                lngSummaryResource.Assessment = new LongitudinalAssessmentSummaryResource();
                lngSummaryResource.Assessment.CertificationId = credential.Certification.ExternalId;
                lngSummaryResource.EnrollmentStatus = new RegistrationEnumValueResponseResource<EnrollmentStatusType>(EnrollmentStatusType.Unenrolled); //!!!
                lngSummaryResource.IsActive = true;
                lngSummaryResource.LongitudinalParticipations = new List<LongitudinalParticipationSummaryResource>();
                lngSummaryResource.LongitudinalParticipations.Add(new LongitudinalParticipationSummaryResource()
                {
                    Cycle = 1,
                    PathwayYear = 1,
                    Status = new RegistrationEnumValueResponseResource<ParticipationStatusType>(ParticipationStatusType.Met)
                });

                LongitudinalEnrollments.Add(lngSummaryResource);

            }

            /// <summary>
            /// Secondary setup (requiring the Container)
            /// </summary>
            protected override void PostSetup()
            {
                base.PostSetup();
            }

            public void WhenICallProgramRulesServiceMethod()
            {
                try
                {
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionGFPrinting_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResults = ResultObject as IList<RuleResults>;
                    RuleResult = RuleResults[0];

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

            public void ThenResultShouldBeFalse()
            {
                RuleResult.MeetRuleRequirement.Should().Be(false);
            }
        }
        #endregion
    }
}
