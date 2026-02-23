using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using FluentAssertions;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRulesIndividualRule
{
    [Story(
       AsA = "controller, background job, or bus consumer",
       IWant = "to be able to utilize the Program Rules Individual Rules",
       SoThat = "it can run the corrective action process"
   )]
    [TestFixture]
    public class IssueNewCredentialForFPHMSpec
    {
        // --- MeetRules ----
        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_100Points()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_100Points().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_FutureDates100Points()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_FutureDates_100Points().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_Reciprocity()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_Reciprocity().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_NewSubspecialtyInitial()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_NewSubspecialtyInitial().BDDfy();
        }

        // ---Don't MeetRules ----

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_NO_100Points()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_NO_100Points().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_NO_Reciprocity()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_NO_Reciprocity().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_NO_NewSubspecialtyInitial()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_NO_NewSubspecialtyInitial().BDDfy();
        }

        [Test]
        public void IssueNewCredentialForFPHMSpecMeetRules_NO_Exam()
        {
            new IssueNewCredentialForFPHMSpec_MeetRules_NO_Exam().BDDfy();
        }

        #region Common Spec Scenarios
        private abstract class IssueNewCredentialForFPHMSpecScenario : ProgramRulesIndividualRulesScenario
        {


        }

        #endregion Scenarios

        #region Scenarios

        private class IssueNewCredentialForFPHMSpec_MeetRules_100Points : IssueNewCredentialForFPHMSpecScenario
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
                                        TotalMOCPoints: 100m);

                UserActivities.Add(ActivityResourceDataBuilder
                            .WithAttestation(   ActivityCompletedDate: ActivityCompletedDate, 
                                                ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                            .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;
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

 
        private class IssueNewCredentialForFPHMSpec_MeetRules_FutureDates_100Points : IssueNewCredentialForFPHMSpecScenario
        {

            /// <summary>
            /// Primary setup
            /// </summary>
            protected override void PreSetup()
            {
                InitializeBuilders();
                InitializeDataProperties();

                EventDate = new DateTime(2020, 12, 13);
                ProcessingDate = DateTime.Now;
                FirstIssuanceDate = new DateTime(2019, 11, 01);

                DateTime ActivityCompletedDate = new DateTime(2020, 12, 01);

                Set_ActivitiesWithPoints(ActivityCompletedDate: ActivityCompletedDate,
                                         TotalMOCPoints: 95m);
                // future dates
                Set_ActivitiesWithPoints(ActivityCompletedDate: FutureActivityDate,
                        TotalMOCPoints: 10m);

                UserActivities.Add(ActivityResourceDataBuilder
                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                            .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode: ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;
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


        private class IssueNewCredentialForFPHMSpec_MeetRules_Reciprocity : IssueNewCredentialForFPHMSpecScenario
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
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                //--- ReciprocityAttest
                UserActivities.Add(ActivityResourceDataBuilder.WithReciprocity(ActivityCompletedDate: ActivityCompletedDate)
                                                .Build());

                //--- Attestation
                UserActivities.Add(ActivityResourceDataBuilder
                                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate, 
                                                            ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;

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

        private class IssueNewCredentialForFPHMSpec_MeetRules_NewSubspecialtyInitial : IssueNewCredentialForFPHMSpecScenario
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

                UserActivities.Add(ActivityResourceDataBuilder
                                    .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                        ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                                    .Build());


                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                      certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
                      issuanceDate: FirstIssuanceDate);

                // Earned NewSubspecialty Initial Cert
                Set_NewSubspecialtyInitialCert(issuanceDate: new DateTime(2017, 11, 01),
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;

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
        private class IssueNewCredentialForFPHMSpec_MeetRules_NO_100Points : IssueNewCredentialForFPHMSpecScenario
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

                UserActivities.Add(ActivityResourceDataBuilder
                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                            .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;
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

        private class IssueNewCredentialForFPHMSpec_MeetRules_NO_Reciprocity : IssueNewCredentialForFPHMSpecScenario
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

                UserActivities.Add(ActivityResourceDataBuilder
                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                            .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);
                
                //--- ReciprocityAttest
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;
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

        private class IssueNewCredentialForFPHMSpec_MeetRules_NO_NewSubspecialtyInitial : IssueNewCredentialForFPHMSpecScenario
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

                UserActivities.Add(ActivityResourceDataBuilder
                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                            .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
                                    issuanceDate: FirstIssuanceDate);

                Set_SuT_Registration(credential: credential,
                                    administrationDate: FirstIssuanceDate,
                                    seatDateCert: FirstIssuanceDate);

                // Earned NewSubspecialty Initial Cert
                Set_NewSubspecialtyInitialCert(issuanceDate: ProcessingDate.AddYears(-6),
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;
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

        private class IssueNewCredentialForFPHMSpec_MeetRules_NO_Exam : IssueNewCredentialForFPHMSpecScenario
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
                                        TotalMOCPoints: 100m);

                UserActivities.Add(ActivityResourceDataBuilder
                            .WithAttestation(ActivityCompletedDate: ActivityCompletedDate,
                                                ProductCode: ProductResourceConstants.ProductCode.FPHMAttestInitial)
                            .Build());

                var credential = Set_SuT_Credential(category: CredentialCategoryType.TimeLimited,
                                    certificationCode:  ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine,
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
                    ResultObject = ProgramRulesServiceObject.Invoke("IssueNewCredentialForFPHM_",
                                                                                    InputCredentials[0],        // credential
                                                                                    MemberId,               // memberId
                                                                                    EventDate,              // eventDate
                                                                                    ProcessingDate);        // processingDate

                    RuleResult = ResultObject as RuleResults;
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
