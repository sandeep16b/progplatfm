using Abim.Platform.Product.Resources.Constants;
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
    public class CorrectiveActionCertStatusSpec
    {

        [Test]
        public void CorrectiveActionCertStatus_MeetRulesAttestationOk()
        {
            new CorrectiveActionCertStatus_MeetRules_AttestationOk().BDDfy();
        }

        [Test]
        public void CorrectiveActionCertStatus_MeetRules100Points()
        {
            new CorrectiveActionCertStatus_MeetRules_100_Points().BDDfy();
        }
        
        [Test]
        public void CorrectiveActionCertStatus_FutureDates_MeetRules100Points()
        {
            new CorrectiveActionCertStatus_FutureDates_MeetRules_100_Points().BDDfy();
        }
        
        [Test]
        public void CorrectiveActionCertStatus_MeetRulesReciprocity()
        {
            new CorrectiveActionCertStatus_MeetRules_Reciprocity().BDDfy();
        }

        //PBI 320769 : Program Rule 34 (5-Year Lookback Requirements, Reciprocity at the End of Lookback window): applied during Corrective Action processing
        [Test]
        public void CorrectiveActionCertStatus_MeetRulesReciprocityAtTheEndOfLookBackWindowus()
        {
            new CorrectiveActionCertStatus_MeetRules_ReciprocityAtTheEndOfLookBackWindow().BDDfy();
        }

        [Test]
        public void CorrectiveActionCertStatus_MeetRulesNewSubspecialtyInitial()
        {
            new CorrectiveActionCertStatus_MeetRules_NewSubspecialtyInitial().BDDfy();
        }

        [Test]
        public void CorrectiveActionCertStatus_MeetRulesRecentlyInitiallyCertified()
        {
            new CorrectiveActionCertStatus_MeetRules_RecentlyInitiallyCertified().BDDfy();
        }

        //***  negative cases 
        [Test]
        public void CorrectiveActionCertStatus_MeetRulesNoAttestation_FPHMException()
        {
            new CorrectiveActionCertStatus_MeetRules_NoAttestation().BDDfy();
        }

        [Test]
        public void CorrectiveActionCertStatus_DontMeetRulesFail5YearLookBack()
        {
            new CorrectiveActionCertStatus_DontMeetRules_Fail5YearLookBack().BDDfy();
        }


        #region Common Spec Scenarios
        private abstract class CorrectiveActionCertStatus_SpecScenario : ProgramRulesIndividualRulesScenario
        {
        }

        #endregion Scenarios

        #region Scenarios

        private class CorrectiveActionCertStatus_MeetRules_AttestationOk : CorrectiveActionCertStatus_SpecScenario
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
                                        TotalMOCPoints: 100m); // !!!!

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year,12,31),
                                    issuanceStatus:IssuanceStatusType.Expired,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                //--- Attestation
                UserActivities.Add(ActivityResourceDataBuilder
                                                .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                                    ProductCode: ProductResourceConstants.ProductCode.FPHMAttestMOC)
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_MeetRules_100_Points : CorrectiveActionCertStatus_SpecScenario
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
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_FutureDates_MeetRules_100_Points : CorrectiveActionCertStatus_SpecScenario
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
                ProcessingDate = new DateTime(2023, 12, 01);  //pbi 279364:Restore and Correct Program Platform Unit Tests Disabled During 1/6/2024 Deployment 
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 11, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 95.5m);

                // future dates
                Set_ActivitiesWithPoints(ActivityCompletedDate: FutureActivityDate,
                        TotalMOCPoints: 10m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_MeetRules_Reciprocity : CorrectiveActionCertStatus_SpecScenario
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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_MeetRules_ReciprocityAtTheEndOfLookBackWindow : CorrectiveActionCertStatus_SpecScenario
        {
            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();


                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2019, 01, 13); // Evaluation Date
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);
                DateTime ReciprocityCompletedDate = new DateTime(2017, 1, 5); // it is more then 2 years from Processing Date (2019, 1, 13), but valid at the end of lookback window (2018, 12, 31)

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m); // !!!!

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode: ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                //--- ReciprocityAttest
                UserActivities.Add(ActivityResourceDataBuilder.WithReciprocity(ActivityCompletedDate: ReciprocityCompletedDate)
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_MeetRules_NewSubspecialtyInitial : CorrectiveActionCertStatus_SpecScenario
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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_MeetRules_RecentlyInitiallyCertified : CorrectiveActionCertStatus_SpecScenario
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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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
        private class CorrectiveActionCertStatus_DontMeetRules_FailAssessment : CorrectiveActionCertStatus_SpecScenario
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
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
                                    issuanceDate: FirstIssuanceDate,
                                    assessmentMet:false);

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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_DontMeetRules_Fail5YearLookBack : CorrectiveActionCertStatus_SpecScenario
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

                var credential = Set_SuT_Credential(category: CredentialCategoryType.MustBeMaintained,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                // Earned NewSubspecialty Initial Cert but it is CoSponsored (don't count toward this requirements)
                Set_NewSubspecialtyInitialCert(issuanceDate: EventDate.AddYears(-1),
                                                category: CredentialCategoryType.MustBeMaintained,
                                                certificationCode: "GERI",
                                                IsCosponsored: true); // pbi 282145 : (2.51) To prevent meeting 5-year window point requirements with created co-sponsored Subspecialty initial Certificate.

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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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

        private class CorrectiveActionCertStatus_MeetRules_NoAttestation : CorrectiveActionCertStatus_SpecScenario
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
                                    expirationDate: new DateTime(ProcessingDate.AddYears(-1).Year, 12, 31),
                                    issuanceStatus: IssuanceStatusType.Expired,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("CorrectiveActionCertStatus_",
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
        #endregion
    }
}
