using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
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
    public class CorrectiveActionParticipationStatus_GF_Spec
    {
        [Test]
        public void CorrectiveActionParticipationStatus_GF_MeetRules100Points()
        {
            new CorrectiveActionParticipationStatus_GF_MeetRules_100_Points().BDDfy();
        }
        
        [Test]
        public void CorrectiveActionParticipationStatus_GF_FutureDates_MeetRules100Points()
        {
            new CorrectiveActionParticipationStatus_GF_FutureDatesMeetRules_100_Points().BDDfy();
        }
        
        [Test]
        public void CorrectiveActionParticipationStatus_GF_MeetRulesReciprocity()
        {
            new CorrectiveActionParticipationStatus_GF_MeetRules_Reciprocity().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_GF_MeetRulesNewSubspecialtyInitial()
        {
            new CorrectiveActionParticipationStatus_GF_MeetRules_NewSubspecialtyInitial().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_GF_MeetRulesRecentlyInitiallyCertified()
        {
            new CorrectiveActionParticipationStatus_GF_MeetRules_RecentlyInitiallyCertified().BDDfy();
        }

        //***  negative cases 
        [Test]
        public void CorrectiveActionParticipationStatus_GF_DontMeetRulesFailAssessment()
        {
            new CorrectiveActionParticipationStatus_GF_DontMeetRules_FailAssessment().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_GF_DontMeetRulesFail5YearLookBack()
        {
            new CorrectiveActionParticipationStatus_GF_DontMeetRules_Fail5YearLookBack().BDDfy();
        }


        #region Common Spec Scenarios
        private abstract class CorrectiveActionParticipationStatus_GF_SpecScenario : ProgramRulesIndividualRulesScenario
        {
        }

        #endregion Scenarios

        #region Scenarios

        private class CorrectiveActionParticipationStatus_GF_MeetRules_100_Points : CorrectiveActionParticipationStatus_GF_SpecScenario
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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

        private class CorrectiveActionParticipationStatus_GF_FutureDatesMeetRules_100_Points : CorrectiveActionParticipationStatus_GF_SpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                // we cannot set today's date to ProcessingDate because 5-year Look Back would move, but points would be in older lookback 
                ProcessingDate = new DateTime(2023, 12, 01);  //pbi 279364 : Restore and Correct Program Platform Unit Tests Disabled During 1/6/2024 Deployment 
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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

        private class CorrectiveActionParticipationStatus_GF_MeetRules_Reciprocity : CorrectiveActionParticipationStatus_GF_SpecScenario
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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

        private class CorrectiveActionParticipationStatus_GF_MeetRules_NewSubspecialtyInitial : CorrectiveActionParticipationStatus_GF_SpecScenario
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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

        private class CorrectiveActionParticipationStatus_GF_MeetRules_RecentlyInitiallyCertified : CorrectiveActionParticipationStatus_GF_SpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13); // --- fixed date !!!!
                FirstIssuanceDate = new DateTime(2014, 11, 01); // Recently initially certified (5 years window)

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.GrandFather,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate,
                                    expirationDate: new DateTime(2018, 12, 31)); // override to make this test to look into 

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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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
        private class CorrectiveActionParticipationStatus_GF_DontMeetRules_FailAssessment : CorrectiveActionParticipationStatus_GF_SpecScenario
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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

        private class CorrectiveActionParticipationStatus_GF_DontMeetRules_Fail5YearLookBack : CorrectiveActionParticipationStatus_GF_SpecScenario
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionParticipationStatus_",
                                                                                    InputCredentials,       // credentials
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate,         // processingDate
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
