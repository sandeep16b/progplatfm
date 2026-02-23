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
    public class CorrectiveActionParticipationStatus_MBM_Spec
    {

        [Test]
        public void CorrectiveActionParticipationStatus_MBM_MeetRulesAnyPoints()
        {
            new CorrectiveActionParticipationStatus_MBM_MeetRules_AnyPoints().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_MBM_FutureDates_MeetRulesAnyPoints()
        {
            new CorrectiveActionParticipationStatus_MBM_MeetRules_FutureDates_AnyPoints().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_MBM_MeetRulesReciprocity()
        {
            new CorrectiveActionParticipationStatus_MBM_MeetRules_Reciprocity().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_MBM_MeetRulesRecentlyInitiallyCertified()
        {
            new CorrectiveActionParticipationStatus_MBM_MeetRules_RecentlyInitiallyCertified().BDDfy();
        }

        [Test]
        public void CorrectiveActionParticipationStatus_MBM_MeetRulesNoAttestation()
        {
            new CorrectiveActionParticipationStatus_MBM_MeetRules_NoAttestation().BDDfy();
        }

        //***  negative cases 
        [Test]
        public void CorrectiveActionParticipationStatus_MBM_DontMeetRulesFail2YearLookBack()
        {
            new CorrectiveActionParticipationStatus_MBM_DontMeetRules_Fail2YearLookBack().BDDfy();
        }


        #region Common Spec Scenarios
        private abstract class CorrectiveActionParticipationStatus_MBM_SpecScenario : ProgramRulesIndividualRulesScenario
        {
        }

        #endregion Scenarios

        #region Scenarios

        private class CorrectiveActionParticipationStatus_MBM_MeetRules_AnyPoints : CorrectiveActionParticipationStatus_MBM_SpecScenario
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
                                        TotalMOCPoints: 0.1m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
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

        private class CorrectiveActionParticipationStatus_MBM_MeetRules_FutureDates_AnyPoints : CorrectiveActionParticipationStatus_MBM_SpecScenario
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

                Set_ActivitiesWithPoints(ActivityCompletedDate: FutureActivityDate,
                                        TotalMOCPoints: 0.1m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
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
        }


        private class CorrectiveActionParticipationStatus_MBM_MeetRules_Reciprocity : CorrectiveActionParticipationStatus_MBM_SpecScenario
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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
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

        private class CorrectiveActionParticipationStatus_MBM_MeetRules_RecentlyInitiallyCertified : CorrectiveActionParticipationStatus_MBM_SpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 12, 13);
                ProcessingDate = new DateTime(2019, 01, 13); // --- fixed date !!!!
                FirstIssuanceDate = new DateTime(2017, 11, 01); // Recently initially certified (5 years window)

                //Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                //        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

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

        private class CorrectiveActionParticipationStatus_MBM_MeetRules_NoAttestation : CorrectiveActionParticipationStatus_MBM_SpecScenario
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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
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

            public void ThenResultShouldBeTrue()
            {
                RuleResult.MeetRuleRequirement.Should().Be(true);
            }
        }

        // negative cases 
        private class CorrectiveActionParticipationStatus_MBM_DontMeetRules_Fail2YearLookBack : CorrectiveActionParticipationStatus_MBM_SpecScenario
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

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate.AddYears(-3),
                                        TotalMOCPoints: 0.1m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
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
