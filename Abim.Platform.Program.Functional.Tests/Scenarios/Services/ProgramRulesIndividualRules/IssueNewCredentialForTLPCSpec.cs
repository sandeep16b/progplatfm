using Abim.Enterprise.Core.Registration.Enums;
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
    public class IssueNewCredentialForTLPCSpec
    {
        // --- MeetRules ----
        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_100Points()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_100Points().BDDfy();
        }
        
        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_FutureDates_100Points()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_FutureDates_100Points().BDDfy();
        }
        
        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_Reciprocity()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_Reciprocity().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NewSubspecialtyInitial()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NewSubspecialtyInitial().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_RecentlyInitiallyCertified()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_RecentlyInitiallyCertified().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_Attestation()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_Attestation().BDDfy();
        }

        // ---Don't MeetRules ----

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NO_100Points()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NO_100Points().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NO_Reciprocity()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NO_Reciprocity().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NO_NewSubspecialtyInitial()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NO_NewSubspecialtyInitial().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NO_RecentlyInitiallyCertified()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NO_RecentlyInitiallyCertified().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NO_Attestation()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NO_Attestation().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForTLPCSpecMeetRules_NO_Exam()
        {
            new IssueNewCredentialForTLPCSpec_MeetRules_NO_Exam().BDDfy();
        }

        #region Common Spec Scenarios
        private abstract class IssueNewCredentialForTLPCSpecScenario : ProgramRulesIndividualRulesScenario
        {
           public DateTime FutureActivityDate = DateTime.Now.AddDays(1);
          
        }

        #endregion Scenarios

        #region Scenarios

        private class IssueNewCredentialForTLPCSpec_MeetRules_100Points : IssueNewCredentialForTLPCSpecScenario
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
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);


                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 100);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited, 
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_FutureDates_100Points : IssueNewCredentialForTLPCSpecScenario
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
                // we cannot set today's date to ProcessingDate because 5-year Look Back would move, but points would be in older lookback 
                ProcessingDate = new DateTime(2023, 12, 01);  //pbi 279364:Restore and Correct Program Platform Unit Tests Disabled During 1/6/2024 Deployment
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 95);

                // future activity completed dates
                Set_ActivitiesWithPoints(ActivityCompletedDate: FutureActivityDate,
                        TotalMOCPoints: 10);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_Reciprocity : IssueNewCredentialForTLPCSpecScenario
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
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);


                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 99.9m);

                //--- ReciprocityAttest
                UserActivities.Add(ActivityResourceDataBuilder.WithReciprocity(ActivityCompletedDate: new DateTime(2018, 12, 01))
                                                .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_NewSubspecialtyInitial : IssueNewCredentialForTLPCSpecScenario
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
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                // Earned NewSubspecialty Initial Cert
                Set_NewSubspecialtyInitialCert(issuanceDate: new DateTime(2017, 11, 01),
                                                category: CredentialCategoryType.MustBeMaintained,
                                                certificationCode: "GERI");

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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_RecentlyInitiallyCertified : IssueNewCredentialForTLPCSpecScenario
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
                ProcessingDate = new DateTime(2018, 11, 02); // --- fixed date !!!!
                FirstIssuanceDate = new DateTime(2014, 11, 01); // Recently initially certified (5 years window)

                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate,
                                    expirationDate: new DateTime(2018, 12, 31)); // override to make this test to look into 

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    withMOC: true);
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        // ICARD
        private class IssueNewCredentialForTLPCSpec_MeetRules_Attestation : IssueNewCredentialForTLPCSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                // set Main data >>>>>
                EventDate = new DateTime(2018, 11, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 100);

                //--- Attestation
                UserActivities.Add(ActivityResourceDataBuilder
                                                .WithAttestation(   ActivityCompletedDate: ActivityCompletedDate, 
                                                                    ProductCode: ProductResourceConstants.ProductCode.ICARDAttestMOC)
                                                .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InterventionalCardiology,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        // ---Don't MeetRules ----
        private class IssueNewCredentialForTLPCSpec_MeetRules_NO_100Points : IssueNewCredentialForTLPCSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_NO_Reciprocity : IssueNewCredentialForTLPCSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {

                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                //--- ReciprocityAttest (too old)
                UserActivities.Add(ActivityResourceDataBuilder.WithReciprocity(ActivityCompletedDate: ActivityCompletedDate.AddYears(-2))
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_NO_NewSubspecialtyInitial : IssueNewCredentialForTLPCSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {

                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                // Earned NewSubspecialty Initial Cert
                Set_NewSubspecialtyInitialCert(issuanceDate: ProcessingDate.AddYears(-5),
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_NO_RecentlyInitiallyCertified : IssueNewCredentialForTLPCSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        // NO ICARD
        private class IssueNewCredentialForTLPCSpec_MeetRules_NO_Attestation : IssueNewCredentialForTLPCSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2018, 01, 13);
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2018, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                        TotalMOCPoints: 99.9m);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.InterventionalCardiology,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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

        private class IssueNewCredentialForTLPCSpec_MeetRules_NO_Exam : IssueNewCredentialForTLPCSpecScenario
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
                ProcessingDate = new DateTime(2018, 12, 13);
                FirstIssuanceDate = new DateTime(2008, 11, 01);


                Set_ActivitiesWithPoints(ActivityCompletedDate: new DateTime(2018, 12, 01),
                                        TotalMOCPoints: 100);

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                                    certificationCode:  ProgramResourceConstants.CertificationCode.InternalMedicine,
                                                    issuanceDate: FirstIssuanceDate,
                                                    assessmentMet: false);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate,
                                    mocExamResult:ExamResultType.Fail);
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForTLPC_",
                                                                                    InputCredentials,        // credentials
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
