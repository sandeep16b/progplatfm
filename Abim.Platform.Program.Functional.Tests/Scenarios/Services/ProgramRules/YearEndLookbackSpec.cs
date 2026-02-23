using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Extensions.ExternalResponses;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Product.Resources.Constants;
using Abim.Platform.Product.Resources.Enums;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using Abim.Platform.Program.Tests.Setup.Responses;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
      AsA = "controller, background job, or bus consumer",
      IWant = "to be able to utilize the Program Rules service",
      SoThat = "it can run the year end lookback process"
      )]
    [TestFixture]
    public class YearEndLookbackSpec
    {
        [Test]
        [WorkItem(134881)]
        public void Should_Get_Eligible_Credentials_During_Lookback_Process()
        {
            new ShouldGetEligibleCredentialsDuringLookbackProcessScenario().BDDfy();
        }

        [Test]
        [WorkItem(134881)]
        public void Should_Submit_Command_With_Correct_Values_To_Update_Credential()
        {
            new ShouldSubmitCommandWithCorrectValuesToUpdateCredential().BDDfy();
        }

        [Test]
        [WorkItem(134885)]
        public void Should_Clear_Grace_Period_When_Lookback_Occurs_After()
        {
            new ShouldClearGracePeriodWhenLookbackOccursAfter().BDDfy();
        }

        [Test]
        [WorkItem(134885)]
        public void Should_Not_Clear_Grace_Period_When_Lookback_Occurs_During()
        {
            new ShouldNotClearGracePeriodWhenLookbackOccursDuring().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Meet_FiveYearLookbackRequirement_When100MOCPoints_AndNoMedicalKnowledgePoints()
        {
            new ShouldMeetFiveYearLookbackRequirementWhen100MOCPointsAndNoMedicalKnowledgePoints().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Meet_FiveYearLookbackRequirement_WhenMOCPointsLessThan100_AndEnrolledInReciprocity()
        {
            new ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100AndEnrolledInReciprocity().BDDfy();
        }
        
        [Test]
        [WorkItem(135348)]
        public void Should_Meet_FiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_EarnedSubspecialtyCert()
        {
            new ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityEarnedSubspecialtyCert().BDDfy();
        }
        
        [Test]
        [WorkItem(135348)]
        public void Should_Meet_FiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_NotEarnedSubspecialtyCert_RecentlyCertified()
        {
            new ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertRecentlyCertified().BDDfy();
        }
        
        [Test]
        [WorkItem(135348)]
        public void Should_Not_MeetFiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_NotEarnedSubspecialtyCert_NotRecentlyCertified()
        {
            new ShouldNotMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertified().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Meet_TLFiveYearLookbackRequirement_When100MOCPoints_AndNoMedicalKnowledgePoints()
        {
            new ShouldMeetTLFiveYearLookbackRequirementWhen100MOCPointsAndNoMedicalKnowledgePoints().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Meet_TLFiveYearLookbackRequirement_WhenMOCPointsLessThan100_AndEnrolledInReciprocity()
        {
            new ShouldMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100AndEnrolledInReciprocity().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Meet_TLFiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_EarnedSubspecialtyCert()
        {
            new ShouldMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityEarnedSubspecialtyCert().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Meet_TLFiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_NotEarnedSubspecialtyCert_RecentlyCertified()
        {
            new ShouldMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertRecentlyCertified().BDDfy();
        }

        [Test]
        [WorkItem(135348)]
        public void Should_Not_MeetTLFiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_NotEarnedSubspecialtyCert_NotRecentlyCertified()
        {
            new ShouldNotMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertified().BDDfy();
        }
        
        [Test]
        [WorkItem(135348)]
        public void ShouldMeet_FiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_NotEarnedSubspecialtyCert_NotRecentlyCertified_CheckDateReqTrueBefore2018()
        {
            new ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertifiedCheckDateReqTrueBeforeEnd2018().BDDfy();
        }
        
        [Test]
        [WorkItem(135348)]
        public void ShouldNotMeet_FiveYearLookbackRequirement_WhenMOCPointsLessThan100_NotEnrolledInReciprocity_NotEarnedSubspecialtyCert_NotRecentlyCertified_CheckDateReqTrueAfter2018()
        {
            new ShouldNotMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertifiedCheckDateReqTrueAfter2018().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]
        public void Should_Meet_AttestationRequirment_WithPassedIcardMOCAttestation_WithinFiveYears()
        {
            new ShouldMeetAttestationRequirmentWithPassedIcardMOCAttestationWithinFiveYears().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]
        public void ShouldNot_MeetAttestationRequirment_WithNoPassedIcardMOCAttestation_WithinFiveYears()
        {
            new ShouldNotMeetAttestationRequirmentWithNoPassedIcardMOCAttestationWithinFiveYears().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]
        public void ShouldMeet_AttestationRequirment_WithOldestIssuanceWithinFiveYears_NoPassedAttestation()
        {
            new ShouldMeetAttestationRequirmentWithOldestIssuanceWithinFiveYearsNoPassedAttestation().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]
        public void ShouldMeet_AttestationRequirment_WithPassedFphmMOCAttestation_WithinFiveYears()
        {
            new ShouldMeetAttestationRequirmentWithPassedFphmMOCAttestationWithinFiveYears().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]
        public void ShouldNot_MeetAttestationRequirment_WithPassedMocAttestationWithinFiveYears_ForADifferentCert_OrPassedInitialAttestation()
        {
            new ShouldNotMeetAttestationRequirmentWithPassedMocAttestationWithinFiveYearsForADifferentCertOrPassedInitialAttestation().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]
        public void ShouldMeet_AttestationRequirement_WhenNotICard_NotFphm_AndNoAttestations()
        {
            new ShouldMeetAttestationRequirementWhenNotICardNotFphmAndNoAttestations().BDDfy();
        }
        
        [Test]
        [WorkItem(135605)]//
        public void ShouldNotMeet_NonExamsRequirement_WhenAttestationRequirementIsNotMet()
        {
            new ShouldNotMeetNonExamsRequirementWhenAttestationRequirementIsNotMet().BDDfy();
        }

        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenCredentialMOCExamDueDateYear_NotEqualToLookbackYear()
        {
            new ShouldNotSetGracePeriodWhenCredentialMOCExamDueDateYearNotEqualToLookbackYear().BDDfy();
        }
        
        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenIssuanceNotActiveAndParticipating()
        {
            new ShouldNotSetGracePeriodWhenIssuanceNotActiveAndParticipating().BDDfy();
        }
        
        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenNoCorrespondingRegistration()
        {
            new ShouldNotSetGracePeriodWhenNoCorrespondingRegistration().BDDfy();
        }

        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenNoCorrespondingRegistrationUtt()
        {
            new ShouldNotSetGracePeriodWhenNoCorrespondingRegistrationUtt().BDDfy();
        }

        [Test]
        [WorkItem(135753)]
        public void ShouldSetGracePeriod_WhenCorrespondingRegistration_ActiveParticipating_NonExamsReq_MocExamDueYearEqualLookbackYear()
        {
            new ShouldSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear().BDDfy();
        }

        /* 
         Pbi 216370 : Certification & Participation Status Changes for 2020 and 2021 MOC Requirements(COVID 4)
            For 2020, 2021, and 2022, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
            *** Diplomates in one of the COVID 4 disciplines (Infectious Disease, Hospital Medicine, Critical Care, Pulmonary Disease) currently in the grace period in 2020 will have their grace period term extended to 12/31/2023.         ============================================================================================================================================================================================
         --------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
            Pbi 208254 : (Release 2.35) Certification & Participation Status Changes for 2020 and 2021 MOC Requirements (Not COVID 4)
              For 2020 and 2021, a diplomate will not experience a negative status change (from certified to not certified or participating to not participating) for any of the following reasons:           
              *** Diplomates  currently in the grace period in 2020 will have their grace period term extended to 12/31/2022.
        ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
        */
        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void ShouldNotSetGracePeriod_in2021_Only_WhenCorrespondingRegistration_ActiveParticipating_NonExamsReq_MocExamDueYearEqualLookbackYear()
        {
            new ShouldNotSetGracePeriod_in2021Only_WhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void ShouldNotSetGracePeriod_in2022_Covid4_WhenCorrespondingRegistration_ActiveParticipating_NonExamsReq_MocExamDueYearEqualLookbackYear()
        {
            new ShouldNotSetGracePeriod_in2022_Covid4_WhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void ShouldSetGracePeriod_in2022_NotCovid4_WhenCorrespondingRegistration_ActiveParticipating_NonExamsReq_MocExamDueYearEqualLookbackYear()
        {
            new ShouldSetGracePeriod_in2022_NotCovid4_WhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear().BDDfy();
        }

        // extending Grace period in 2021 ( 1 year for not covid 4 and 2 years for covid 4)

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void ShouldExtendGracePeriod_in2020_ByTwoYears_NotCovid4_WhenCurrentlyInGracePeriod()
        {
            new ShoulExtendSetGracePeriod_in2020_ByTwoYears_NotCovid4_WhenCurrentlyInGracePeriod().BDDfy();
        }

        [Test]
        [WorkItem(216370)]
        [WorkItem(208254)]
        public void ShouldExtendGracePeriod_in2020_ByThreeYears_Covid4_WhenCurrentlyInGracePeriod()
        {
            new ShoulExtendSetGracePeriod_in2020_ByThreeYears_Covid4_WhenCurrentlyInGracePeriod().BDDfy();
        }

        //-------------------------------------------------------------------------------------------------



        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenCorrespondingRegistrationIsForADifferentCert()
        {
            new ShouldNotSetGracePeriodWhenCorrespondingRegistrationIsForADifferentCert().BDDfy();
        }

        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenCorrespondingRegistrationIsForANonMocExam()
        {
            new ShouldNotSetGracePeriodWhenCorrespondingRegistrationIsForANonMocExam().BDDfy();
        }
        
        [Test]
        [WorkItem(135753)]
        public void ShouldNot_SetGracePeriod_WhenCorrespondingRegistration_IsNotIndt_NotFail_NotUtt()
        {
            new ShouldNotSetGracePeriodWhenCorrespondingRegistrationIsNotIndtNotFailNotUtt().BDDfy();
        }

        [Test]
        [WorkItem(135753)]
        public void ShouldNotSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYearPointsBeyondYELBLessThan100_123()
        {
            new ShouldNotSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYearPointsBeyondYELBLessThan100().BDDfy();
        }

        [Test]
        [WorkItem(135753)]
        public void ShouldSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYearPointsBeyondYELB_asd()
        {
            new ShouldSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYearPointsBeyondYELB().BDDfy();
        }


        //-----  Clear Assessment Met (PBI 134115 ) ----------------------------------------------------------------
        [Test]
        [WorkItem(134115)]
        public void Should_Clear_AssesmentMet()
        {
            new ShouldClearAssesmentMet().BDDfy();
        }

        [Test]
        [WorkItem(134115)]
        public void Should_NOT_Clear_AssesmentMet()
        {
            new ShouldNotClearAssesmentMet().BDDfy();
        }

        [Test]
        [WorkItem(231852)]
        public void Should_NOT_Clear_AssessmentMet_When_Eligible_For_COVID_Extension_And_Not_In_COVID_4()
        {
            new ShouldNotClearAssesmentMet(new DateTime(2021, 12, 31)).BDDfy();
        }

        [Test]
        [WorkItem(231852)]
        public void Should_NOT_Clear_AssessmentMet_When_Eligible_For_COVID_Extension_And_In_COVID_4()
        {
            new ShouldNotClearAssesmentMet(new DateTime(2022, 12, 31), "ID").BDDfy();
        }

        //---------------------------------------------------------------------------------------------

        //-----  Set 2 Year Pass Due ( ConsecutiveKCIPassRequired ) (PBI 134114 )  --------------------------------------------------------
        [Test]
        [WorkItem(134114)]
        public void Should_Set_ConsecutiveKCIPassRequired()
        {
            new ShouldSetConsecutiveKCIPassRequired().BDDfy();
        }

        [Test]
        [WorkItem(134114)]
        public void Should_NOT_Set_ConsecutiveKCIPassRequired()
        {
            new ShouldNOTSetConsecutiveKCIPassRequired().BDDfy();
        }
        
        [Test]
        [WorkItem(137218)]
        public void Should_UseTheCurrentDate_InMeetFiveYearLookbackRequirement_For100MOCPoints_And_MeetRequirement()
        {
            new ShouldUseTheCurrentDateInMeetFiveYearLookbackRequirementFor100MOCPointsAndMeetRequirement().BDDfy();
        }

        
        [Test]
        [WorkItem(137218)]
        public void Should_UseTheCurrentDate_InMeetFiveYearLookbackRequirement_For100MOCPoints_And_Not_MeetRequirement()
        {
            new ShouldUseTheCurrentDateInMeetFiveYearLookbackRequirementFor100MOCPointsAndNotMeetRequirement().BDDfy();
        }
        
        [Test]
        [WorkItem(139327)]
        public void Should_Not_SetGracePeriod_When_ThereAreNo_IssuancesOnACert()
        {
            new ShouldNotSetGracePeriodWhenThereAreNoIssuancesOnACert().BDDfy();
        }

        [Test]
        [WorkItem(210153)]
        public void Should_Not_Process_Cosponsored_Credentials()
        {
            new ShouldNotProcessCosponsoredCredentials().BDDfy();
        }

        //----------------------------------------------------------------------------------------------

        #region Scenarios

        #region ShouldGetEligibleCredentialsDuringLookbackProcessScenario
        private class ShouldGetEligibleCredentialsDuringLookbackProcessScenario : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(4);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                var otherBoardSource = SourceBuilder.Build("Some Other Board", "OTBD", "UnitTest");

                for (var x = 0; x < 4; x++)
                {
                    //Make one of them not an ABIM credential
                    if (x == 2)
                        _creds.Add(CredentialBuilder.Build(otherBoardSource));
                    else
                        _creds.Add(CredentialBuilder.Build(abimSource));

                    _creds[x].AddIssuance(IssuanceBuilder.Build());
                }

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                base.SetupRegistrationInterserviceMock();
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) }));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenIRunTheLookbackProcess()
            {
                await _sut.RunYearEndLookback(_memberId, new DateTime(2018, 12, 31), new DateTime(2019, 1, 31));
            }

            private void ThenTheDiplomatesEligibleCredentialsAreRetrieved()
            {
                _credSvcMock.Verify(x => x.SearchByMemberId(_memberId), Times.AtLeastOnce);

                //This verifies that only the expected ones were returned
                _credSvcMock.Verify(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()), Times.Exactly(3));
            }
        }
        #endregion ShouldGetEligibleCredentialsDuringLookbackProcessScenario

        #region ShouldSubmitCommandWithCorrectValuesToUpdateCredential
        private class ShouldSubmitCommandWithCorrectValuesToUpdateCredential : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                base.SetupRegistrationInterserviceMock();
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) }));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);
            }

            private async void WhenIRunTheLookbackProcess()
            {
                await _sut.RunYearEndLookback(_memberId, _lookbackDate, new DateTime(2019, 1, 31));
            }

            private void ThenACommandWithTheCorrectValuesShouldHaveBeenSentToTheHandleMethod()
            {
                _credSvcMock.Verify(x => x.Handle(
                    It.Is<UpdateCredentialFromLookbackCommand>(
                        cmd =>
                            cmd.Credential == _creds[0]
                            && cmd.ModifiedBy == "YearEnd"
                            )), Times.Once);
            }

        }
        #endregion ShouldSubmitCommandWithCorrectValuesToUpdateCredential

        #region ShouldClearGracePeriodWhenLookbackOccursAfter
        private class ShouldClearGracePeriodWhenLookbackOccursAfter : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            // need to check if lookback is in 2020 then we need to go back to 2019 since we have exception rules in 2020 (run YELB in 2021)
            protected DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);

            protected override void SetupCredentialServiceMock()
            {
                int lastYear = _lookbackDate.Year;
                _creds = new List<Credential>(1);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds.Add(CredentialBuilder.Build(abimSource));

                _creds[0].AddIssuance(IssuanceBuilder.Build());

                //We need to set the grace period for the credential we're testing with, and 
                //this appears to be the only way
                _creds[0].ApplyChangesAfterCreatingCredential(
                    true, new DateTime(lastYear, 1, 1), new DateTime(lastYear, 12, 31), null, null, null, null, false, null);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                base.SetupRegistrationInterserviceMock();
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new UserRegistrationsAndCMPRegistrationsResource() { CMPRegistrations = new List<CMPRegistrationResource>(0), Registrations = new List<RegistrationResource>(0) }));
            }

            private void GivenThatIHaveADiplomateIdWithACredentialWhoseGracePeriodHasExpired()
            {
                _memberId = Guid.NewGuid();
                //Grace period of credential was set when cred svc mock was setup
                _creds[0].GracePeriodStartDate.HasValue.Should().Be(true);
                _creds[0].GracePeriodEndDate.HasValue.Should().Be(true);
            }

            private async void WhenIRunTheLookbackProcess()
            {
                await _sut.RunYearEndLookback(_memberId, _lookbackDate, new DateTime(DateTime.Now.Year, 1, 31));
            }

            private void ThenTheGracePeriodOfTheCredentialShouldBeCleared()
            {
                _creds[0].GracePeriodStartDate.Should().Be(null);
                _creds[0].GracePeriodEndDate.Should().Be(null);
            }
        }
        #endregion ShouldClearGracePeriodWhenLookbackOccursAfter

        #region ShouldNotClearGracePeriodWhenLookbackOccursDuring
        private class ShouldNotClearGracePeriodWhenLookbackOccursDuring : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime? _gracePeriodStart;
            private DateTime? _gracePeriodEnd;

            protected override void SetupCredentialServiceMock()
            {
                int lastYear = DateTime.Now.AddYears(-1).Year;
                _creds = new List<Credential>(1);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds.Add(CredentialBuilder.Build(abimSource));

                _creds[0].AddIssuance(IssuanceBuilder.Build());

                //We need to set the grace period for the credential we're testing with, and 
                //this appears to be the only way
                _gracePeriodStart = new DateTime(lastYear, 1, 1);
                _gracePeriodEnd = new DateTime(DateTime.Now.Year, 12, 31);

                _creds[0].ApplyChangesAfterCreatingCredential(
                    true, _gracePeriodStart, _gracePeriodEnd, null, null, null, null, false, null);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }
            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = Guid.NewGuid();
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = DateTime.Now.Year;


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            private void GivenThatIHaveADiplomateIdWithACredentialWhoseGracePeriodHasNotExpired()
            {
                _memberId = Guid.NewGuid();
                //Grace period of credential was set when cred svc mock was setup
                _creds[0].GracePeriodStartDate.HasValue.Should().Be(true);
                _creds[0].GracePeriodEndDate.HasValue.Should().Be(true);
            }

            private async void WhenIRunTheLookbackProcess()
            {
                await _sut.RunYearEndLookback(_memberId, new DateTime(DateTime.Now.Year -1, 12, 31), new DateTime(DateTime.Now.Year, 1, 31));
            }

            private void ThenTheGracePeriodOfTheCredentialShouldNotBeCleared()
            {
                _creds[0].GracePeriodStartDate.Should().Be(_gracePeriodStart);
                _creds[0].GracePeriodEndDate.Should().Be(_gracePeriodEnd);
            }
        }
        #endregion ShouldNotClearGracePeriodWhenLookbackOccursDuring

        #region ShouldDetermineFiveYearLookBackRequirement

     
        private class ShouldMeetFiveYearLookbackRequirementWhen100MOCPointsAndNoMedicalKnowledgePoints : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);
                
            }

            private void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement:false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldUseTheCurrentDateInMeetFiveYearLookbackRequirementFor100MOCPointsAndMeetRequirement: ProgramRulesServiceSimplifiedScenario
        {
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(DateTime.Today.Year, 1, 1);
            private DateTime mockToday = new DateTime((DateTime.Today.Year + 2), 1, 1);
            private Exception exception;
            private FiveYearLookBackStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(mockToday.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 50;
                activity.CompletedDate = new DateTime(_lookbackDate.Year + 1, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.TotalMOCPoints = 50;
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = RandomString.Build() };
                
                activities.Data.Add(activityTwo);


                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void WhenICallFiveYearsLookBack()
            {
                try
                {
                    //We are testing that "today" is in the future relative to _lookbackDate. The 100 Moc Point total should include
                    //activities which are before "today" instead of before _lookbackDate
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, mockToday, checkCheckDateRequirement: false) as FiveYearLookBackStep;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue100MOCPoints()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
                fiveYearLookbackStep.TotalMOCPoints.ShouldBe(100);
            }
        }


        private class ShouldUseTheCurrentDateInMeetFiveYearLookbackRequirementFor100MOCPointsAndNotMeetRequirement : ProgramRulesServiceSimplifiedScenario
        {
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(DateTime.Today.Year, 1, 1);
            private DateTime mockToday = new DateTime((DateTime.Today.Year + 2), 1, 1);
            private Exception exception;
            private FiveYearLookBackStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(mockToday.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 50;
                activity.CompletedDate = new DateTime(_lookbackDate.Year + 1, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.TotalMOCPoints = 50;
                activityTwo.CompletedDate = new DateTime(mockToday.Year + 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activityTwo);


                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void WhenICallFiveYearsLookBack()
            {
                try
                {
                    //We are testing that "today" is in the future relative to _lookbackDate. The 100 Moc Point total should include
                    //activities which are before "today" instead of before _lookbackDate
                    _sut.ProcessingDate = _lookbackDate;
                    _sut.ExecutingProcess = ExecutingProcessType.YearEndLookBack;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, mockToday, checkCheckDateRequirement: false) as FiveYearLookBackStep;
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleFalse50MOCPoints()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(false);
                fiveYearLookbackStep.TotalMOCPoints.ShouldBe(50);
            }
        }



        private class ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100AndEnrolledInReciprocity : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(DateTime.Now.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ReciprocityAttest" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement: false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }


        private class ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityEarnedSubspecialtyCert : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 1, 3, 20), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);
                

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement: false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertRecentlyCertified : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 1, 3, 20), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement: false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldNotMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertified : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource, "IM"));
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, new DateTime(DateTime.Now.Year - 10, 3, 20)));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement: false);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleFalse()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(false);
            }
        }

        private class ShouldMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertifiedCheckDateReqTrueBeforeEnd2018 : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource, "IM"));
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, new DateTime(DateTime.Now.Year - 10, 3, 20)));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2017, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement: true);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleFalse()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldNotMeetFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertifiedCheckDateReqTrueAfter2018 : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource, "IM"));
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, new DateTime(DateTime.Now.Year - 10, 3, 20)));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBack(_creds[0], _lookbackDate, _lookbackDate, checkCheckDateRequirement: true);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleFalse()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(false);
            }
        }

        private class ShouldMeetTLFiveYearLookbackRequirementWhen100MOCPointsAndNoMedicalKnowledgePoints : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
               
            }

            private void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = DateTime.Now;
                    fiveYearLookbackStep = _sut.FiveYearsLookBackTL(_creds[0], _lookbackDate, _sut.ProcessingDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100AndEnrolledInReciprocity : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(DateTime.Now.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ReciprocityAttest" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBackTL(_creds[0], _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }


        private class ShouldMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityEarnedSubspecialtyCert : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 1, 3, 20), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));


                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);


                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBackTL(_creds[0], _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertRecentlyCertified : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, new DateTime(DateTime.Now.Year - 1, 3, 20), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(DateTime.Now.Year - 1, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBackTL(_creds[0], _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleTrue()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldNotMeetTLFiveYearLookbackRequirementWhenMOCPointsLessThan100NotEnrolledInReciprocityNotEarnedSubspecialtyCertNotRecentlyCertified : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep fiveYearLookbackStep;

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource, "IM"));
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, new DateTime(DateTime.Now.Year - 10, 3, 20)));

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);
            }

            private async void WhenICallFiveYearsLookBack()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    fiveYearLookbackStep = _sut.FiveYearsLookBackTL(_creds[0], _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenFiveYearLookbackStepShouldHaveMeetStepRuleFalse()
            {
                fiveYearLookbackStep.MeetStepRule.ShouldBe(false);
            }
        }

        #endregion

        #region ShouldDetermineAttestationRequirement

        private class ShouldMeetAttestationRequirmentWithPassedIcardMOCAttestationWithinFiveYears : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);

            private Exception exception;
            private IStep attestationLookbackStep;
            private Credential _cred;

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndACredentialWithIssuanceDateBeforeStartYearPeriod()
            {
                _memberId = Guid.NewGuid();

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _cred = CredentialBuilder.Build(abimSource);
                
                _cred.AddIssuance(IssuanceBuilder.Build(abimSource, _lookbackDate.AddYears(((int)WindowsIntervalType.FiveYearLookBack + 3)*-1)));
                _cred.Certification.Code = "ICARD";
            }

            private void WhenICallAttestationYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    attestationLookbackStep = _sut.Attestation(_cred, _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenAttestationLookbackStepShouldHaveMeetStepRuleTrue()
            {
                attestationLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldMeetAttestationRequirmentWithPassedFphmMOCAttestationWithinFiveYears : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            // need to check if lookback is in 2020 then we need to go back to 2019 since we have exception rules in 2020 (run YELB in 2021)
            private DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) : new DateTime(DateTime.Now.Year - 1, 12, 31);
            private Exception exception;
            private IStep attestationLookbackStep;
            private Credential _cred;

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20); 
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.FPHMAttestMOC };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDateAndACredentialWithIssuanceDateBeforeStartYearPeriod()
            {
                _memberId = Guid.NewGuid();

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _cred = CredentialBuilder.Build(abimSource);

                _cred.AddIssuance(IssuanceBuilder.Build(abimSource, _lookbackDate.AddYears(((int)WindowsIntervalType.FiveYearLookBack + 3) * -1)));
                _cred.Certification.Code = "HOSP";
            }

            private void WhenICallAttestationYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    attestationLookbackStep = _sut.Attestation(_cred, _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenAttestationLookbackStepShouldHaveMeetStepRuleTrue()
            {
                attestationLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }


        private class ShouldNotMeetAttestationRequirmentWithNoPassedIcardMOCAttestationWithinFiveYears : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep attestationLookbackStep;
            private Credential _cred;

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Cancelled);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.CancelledForNonPayment);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.DeadlinePassedSysCancelled);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Fail);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.InProgress);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Incomplete);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.PurchasedOutsideProgram);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Revoked);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Suspended);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDateAndACredentialWithIssuanceDateBeforeStartYearPeriod()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _cred = CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC);

                _cred.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, _lookbackDate.AddYears(((int)WindowsIntervalType.FiveYearLookBack + 3) * -1), DurationType.Timelimited, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial));
                _cred.Certification.Code = "ICARD";
            }

            private void WhenICallAttestationYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    attestationLookbackStep = _sut.Attestation(_cred, _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenAttestationLookbackStepShouldHaveMeetStepRuleFalse()
            {
                attestationLookbackStep.MeetStepRule.ShouldBe(false);
            }
        }

        private class ShouldMeetAttestationRequirmentWithOldestIssuanceWithinFiveYearsNoPassedAttestation : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep attestationLookbackStep;
            private Credential _cred;

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();
                
                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDateAndACredentialWithIssuanceDateBeforeStartYearPeriod()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _cred = CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC);
                _cred.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(abimSource, IssuanceStatusType.Active, _lookbackDate.AddYears(1), DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, OccurrenceType.Recertification));


                _cred.Certification.Code = "ICARD";
            }

            private void WhenICallAttestationYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    attestationLookbackStep = _sut.Attestation(_cred, _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenAttestationLookbackStepShouldHaveMeetStepRuleTrue()
            {
                attestationLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        private class ShouldNotMeetAttestationRequirmentWithPassedMocAttestationWithinFiveYearsForADifferentCertOrPassedInitialAttestation : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep attestationLookbackStep;
            private Credential _cred;

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.ICARDAttestMOC };

                activities.Data.Add(activity);

                activity = new ActivityResource();
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = ProductResourceConstants.ProductCode.FPHMAttestInitial };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDateAndACredentialWithIssuanceDateBeforeStartYearPeriod()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _cred = CredentialBuilder.BuildWithoutRandoms(abimSource, "SomeCode", "SomeName", CertificationType.General, CredentialType.General, Resources.PathwayType.MOC);

                _cred.AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        abimSource, 
                        IssuanceStatusType.Active, 
                        _lookbackDate.AddYears(((int)WindowsIntervalType.FiveYearLookBack + 3) * -1), 
                        DurationType.Timelimited, 
                        MaintenanceRequirementType.Required, 
                        MaintenanceStatusType.Maintained, 
                        OccurrenceType.Initial));

                _cred.Certification.Code = "HOSP";
            }

            private void WhenICallAttestationYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    attestationLookbackStep = _sut.Attestation(_cred, _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenAttestationLookbackStepShouldHaveMeetStepRuleFalse()
            {
                attestationLookbackStep.MeetStepRule.ShouldBe(false);
            }
        }

        private class ShouldMeetAttestationRequirementWhenNotICardNotFphmAndNoAttestations : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private DateTime _lookbackDate;
            private Exception exception;
            private IStep attestationLookbackStep;
            private Credential _cred;

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();
                

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDateAndACredentialWithIssuanceDateBeforeStartYearPeriod()
            {
                _memberId = Guid.NewGuid();
                _lookbackDate = new DateTime(2018, 12, 31);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _cred = CredentialBuilder.Build(abimSource);

                _cred.AddIssuance(IssuanceBuilder.Build(abimSource, _lookbackDate.AddYears(((int)WindowsIntervalType.FiveYearLookBack + 3) * -1)));
                _cred.Certification.Code = "IM";
            }

            private void WhenICallAttestationYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    attestationLookbackStep = _sut.Attestation(_cred, _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenAttestationLookbackStepShouldHaveMeetStepRuleTrue()
            {
                attestationLookbackStep.MeetStepRule.ShouldBe(true);
            }
        }

        #endregion

        #region NonExamsRequirement

        private class ShouldNotMeetNonExamsRequirementWhenAttestationRequirementIsNotMet : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "BLAH", "Some Cert Name", CertificationType.Subspecialty, CredentialType.Subspecialty, Resources.PathwayType.MOC));
                //_creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained());
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.NotMaintained, issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].MOCExamDueDate = new DateTime(2018,1,1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }
          
            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = 2018;
                reg.AdministrationDate = _lookbackDate.AddMonths(-2);

                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeenSetSinceNonExamsRequirementIsNotMet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }
        #endregion

        #region SetGracePeriod
        private class ShouldNotSetGracePeriodWhenCredentialMOCExamDueDateYearNotEqualToLookbackYear : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                //_creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained());
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year - 5, 12, 31);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year - 5;


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }


        private class ShouldNotSetGracePeriodWhenIssuanceNotActiveAndParticipating : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                _creds[0].AddIssuance(IssuanceBuilder.Build());

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-3);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        private class ShouldNotSetGracePeriodWhenNoCorrespondingRegistration : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                //_creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained());
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].MOCExamDueDate = new DateTime(_lookbackDate.Year, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(1);

                // -- RegistrationResource

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year-5;

                regResource.Registrations.Add(reg);

                // -- CMPRegistrationResource

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg =  cmpRegBuilder.WithExamResult(ExamResultType.Fail)
                                           .WithTestDate(new DateTime(_lookbackDate.Year - 5, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                    .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeen()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        private class ShouldNotSetGracePeriodWhenNoCorrespondingRegistrationUtt : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                //_creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained());
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.UnableToTest.ToString();
                reg.AdministrationYear = _lookbackDate.Year - 5;

                regResource.Registrations.Add(reg);

                // -- CMPRegistrationResource

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Fail)
                                           .WithTestDate(new DateTime(_lookbackDate.Year - 5, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeen()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        private class ShouldSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _creds[0].GracePeriodStartDate.ShouldBeNull();
                    _creds[0].GracePeriodEndDate.ShouldBeNull();

                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldNotBeNull();
                _creds[0].GracePeriodEndDate.ShouldNotBeNull();

                _creds[0].GracePeriodStartDate.Value.Date.ShouldBeEquivalentTo(new DateTime(_lookbackDate.Year + 1, 1, 1).Date);
                _creds[0].GracePeriodEndDate.Value.Date.ShouldBeEquivalentTo(new DateTime(_lookbackDate.Year + 1, 12, 31).Date);
                //_creds[0].AuditData.ModifiedBy.ShouldBeEquivalentTo("SetGrace"); //This gets overriden to YearEnd
            }
        }

        //**** Diplomates with an assessment due in 2021 who do not meet the assessment requirement in 2021 will not be put in the grace period in 2021.
        private class ShouldNotSetGracePeriod_in2021Only_WhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2021, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _creds[0].GracePeriodStartDate.ShouldBeNull();
                    _creds[0].GracePeriodEndDate.ShouldBeNull();

                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotBeSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }
     
        private class ShouldNotSetGracePeriod_in2022_Covid4_WhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2022, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = ProgramResourceConstants.CertificationCode.InfectiousDisease; // COVID 4 CRED
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _creds[0].GracePeriodStartDate.ShouldBeNull();
                    _creds[0].GracePeriodEndDate.ShouldBeNull();

                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotBeSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        private class ShouldSetGracePeriod_in2022_NotCovid4_WhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYear : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2022, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = ProgramResourceConstants.CertificationCode.GeriatricMedicine; // not COVID 4 CRED
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldBeSet()
            {
                _creds[0].GracePeriodEndDate.ShouldBe(_creds[0].ExamDueDate.Value.AddYears(1));
                _creds[0].GracePeriodStartDate.ShouldBe(new DateTime(_creds[0].ExamDueDate.Value.Year + 1,1,1));
            }
        }

        //***** Diplomates currently in the grace period in 2021 will have their grace period term extended to 12/31/2022.
        private class ShoulExtendSetGracePeriod_in2020_ByTwoYears_NotCovid4_WhenCurrentlyInGracePeriod : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2021, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                // existing grace period in 2020
                _creds[0].GracePeriodStartDate = new DateTime(2020, 1, 1);
                _creds[0].GracePeriodEndDate = new DateTime(2021, 12, 31); // was extended in 2021

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {

                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldBeSet2YearsGracePeriod()
            {
                _creds[0].GracePeriodStartDate.Value.Date.ShouldBeEquivalentTo(new DateTime(2020, 1, 1).Date);
                _creds[0].GracePeriodEndDate.Value.Date.ShouldBeEquivalentTo(new DateTime(2022, 12, 31).Date);
            }
        }

        private class ShoulExtendSetGracePeriod_in2020_ByThreeYears_Covid4_WhenCurrentlyInGracePeriod : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2021, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = ProgramResourceConstants.CertificationCode.InfectiousDisease;
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                // existing grace period in 2020
                _creds[0].GracePeriodStartDate = new DateTime(2020, 1, 1);
                _creds[0].GracePeriodEndDate = new DateTime(2021, 12, 31); // was extenended last year

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldBeSet3YearGracePeriod()
            {
                _creds[0].GracePeriodStartDate.Value.Date.ShouldBeEquivalentTo(new DateTime(2020, 1, 1).Date);
                _creds[0].GracePeriodEndDate.Value.Date.ShouldBeEquivalentTo(new DateTime(2023, 12, 31).Date);
            }
        }

        //-------------------------------------------------------------------------------------------------------

        private class ShouldSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYearPointsBeyondYELB : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddMonths(-4); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained(issuanceDate: issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 80;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 8);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year + 1, 1, 10);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _creds[0].GracePeriodStartDate.ShouldBeNull();
                    _creds[0].GracePeriodEndDate.ShouldBeNull();

                    _sut.ProcessingDate = _lookbackDate.AddMonths(2);
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _sut.ProcessingDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldNotBeNull();
                _creds[0].GracePeriodEndDate.ShouldNotBeNull();

                _creds[0].GracePeriodStartDate.Value.Date.ShouldBeEquivalentTo(new DateTime(_lookbackDate.Year + 1, 1, 1).Date);
                _creds[0].GracePeriodEndDate.Value.Date.ShouldBeEquivalentTo(new DateTime(_lookbackDate.Year + 1, 12, 31).Date);
                //_creds[0].AuditData.ModifiedBy.ShouldBeEquivalentTo("SetGrace"); //This gets overriden to YearEnd
            }
        }

        private class ShouldNotSetGracePeriodWhenCorrespondingRegistrationActiveParticipatingNonExamsReqMocExamDueYearEqualLookbackYearPointsBeyondYELBLessThan100 : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();
            private DateTime _firstIssuanceDate = new DateTime(1998, 2, 21);

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Subspecialty, CredentialType.Subspecialty, Resources.PathwayType.MOC));
                _creds[0].AddIssuance(IssuanceBuilder.BuildWithoutRandoms(
                            abimSource,
                            IssuanceStatusType.Active,
                            _firstIssuanceDate,
                            DurationType.Timelimited,
                            MaintenanceRequirementType.Required,
                            MaintenanceStatusType.Maintained,
                            OccurrenceType.Initial));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 80;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 8);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activity.TotalMOCPoints = 10;
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year + 1, 1, 10);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-3);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                       .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                       .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _creds[0].GracePeriodStartDate.ShouldBeNull();
                    _creds[0].GracePeriodEndDate.ShouldBeNull();

                    _sut.ProcessingDate = _lookbackDate.AddMonths(1);
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldHaveNotBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }



        private class ShouldNotSetGracePeriodWhenThereAreNoIssuancesOnACert : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);
                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _creds[0].GracePeriodStartDate.ShouldBeNull();
                    _creds[0].GracePeriodEndDate.ShouldBeNull();

                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
                
            }
        }



        private class ShouldNotSetGracePeriodWhenCorrespondingRegistrationIsForADifferentCert: ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();
            private DateTime _firstIssuanceDate = new DateTime(1998, 2, 21);

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(
                    CredentialBuilder
                        .BuildWithoutRandoms(abimSource, "ICARD", "Interventional Cardiology", CertificationType.Subspecialty, CredentialType.Subspecialty, Resources.PathwayType.MOC));

                _creds[0].AddIssuance(
                    IssuanceBuilder
                        .BuildWithoutRandoms(
                            abimSource,
                            IssuanceStatusType.Active,
                            _firstIssuanceDate,
                            DurationType.Timelimited,
                            MaintenanceRequirementType.Required,
                            MaintenanceStatusType.Maintained,
                            OccurrenceType.Initial));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);

                _creds[0].GrandfatherMOCPrintDate = new DateTime(2014, 1, 1);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(_firstIssuanceDate);
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = Guid.NewGuid();
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-2);

                regResource.Registrations.Add(reg);

                // -- CMPRegistrationResource
                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Fail)
                                           .WithTestDate(new DateTime(_lookbackDate.Year - 5, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        private class ShouldNotSetGracePeriodWhenCorrespondingRegistrationIsForANonMocExam : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                //_creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained());
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Timelimited, MaintenanceRequirementType.NotRequired, MaintenanceStatusType.Maintained, issuanceDate));


                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Kci);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);

                regResource.Registrations.Add(reg);

                // ----- CMPRegistrationResource ----------

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Fail)
                                           .WithTestDate(new DateTime(_lookbackDate.Year - 5, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        private class ShouldNotSetGracePeriodWhenCorrespondingRegistrationIsNotIndtNotFailNotUtt : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));
                //_creds[0].AddIssuance(IssuanceBuilder.BuildActiveMaintained());
                DateTime issuanceDate = _lookbackDate.AddMonths(-(new Random()).Next(1, 24)); // make sure it is before _lookbackDate
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, issuanceDate));

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year, 12, 31);

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 100;
                activity.CompletedDate = new DateTime(_lookbackDate.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                var activityTwo = new ActivityResource();
                activityTwo.CompletedDate = new DateTime(_lookbackDate.Year - 1, 3, 20);
                activityTwo.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activityTwo.Product = new ProductResource() { Code = "ICARDAttestMOC" };

                activities.Data.Add(activityTwo);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Pass.ToString();
                reg.AdministrationYear = _lookbackDate.Year;
                reg.AdministrationDate = _lookbackDate.AddMonths(-4);


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                // ----- CMPRegistrationResource ----------

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Pass)
                                           .WithTestDate(new DateTime(_lookbackDate.Year, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookbackWithNullGracePeriodStartAndEndDates()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThenGracePeriodShouldNotHaveBeenSet()
            {
                _creds[0].GracePeriodStartDate.ShouldBeNull();
                _creds[0].GracePeriodEndDate.ShouldBeNull();
            }
        }

        #endregion

        #region Clear Assesment Met 
        private class ShouldClearAssesmentMet : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            IssuanceDataBuilder IssuanceDataBuilder = new IssuanceDataBuilder();
            SourceDataBuilder SourceDataBuilder = new SourceDataBuilder();
            //----------------- variable input parameter ---------------
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));

                //-------------------------------------------------- variable input parameter ------------------
                _creds[0].AssessmentMet = true;
                _creds[0].MOCExamDueDate = new DateTime(_lookbackDate.Year - 5, 12, 31);
                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year - 5, 12, 31);
                //----------------------------------------------------------------------------------------------            

                ////Active TL issuance NotMaintained
                _creds[0].AddIssuance(IssuanceDataBuilder
                                    .With(a => a.Source = SourceDataBuilder.With(c => c.Code = "ABIM").Build())
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Timelimited)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .With(b => b.ExpirationDate = new DateTime(_lookbackDate.Year + 1, 12, 31))
                                    .Build());

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = 2018;

                regResource.Registrations.Add(reg);

                // ----- CMPRegistrationResource ----------

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Fail)
                                           .WithTestDate(new DateTime(2018, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThen_AssessmentMet_Should_HaveBeenSet_To_False()
            {
                _creds[0].AssessmentMet.ShouldBeFalse();
            }
        }

        private class ShouldNotClearAssesmentMet : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            IssuanceDataBuilder IssuanceDataBuilder = new IssuanceDataBuilder();
            SourceDataBuilder SourceDataBuilder = new SourceDataBuilder();
            //----------------- variable input parameter ---------------
            private DateTime _lookbackDate;
            private Exception exception;
            private Guid certId = Guid.NewGuid();
            private string certCode;

            public ShouldNotClearAssesmentMet(DateTime? lookbackDate = null, string certCodeForCredential = "ICARD")
            {
                _lookbackDate = lookbackDate.HasValue ? lookbackDate.Value : new DateTime(2018, 12, 31);
                certCode = certCodeForCredential;
            }

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));

                //-------------------------------------------------- variable input parameter ------------------
                _creds[0].AssessmentMet = true;
                _creds[0].MOCExamDueDate = new DateTime(_lookbackDate.Year + 1, 12, 31);
                _creds[0].ExamDueDate = new DateTime(_lookbackDate.Year + 1, 12, 31);
                //----------------------------------------------------------------------------------------------

                ////Active TL issuance NotMaintained
                _creds[0].AddIssuance(IssuanceDataBuilder
                                    .With(a => a.Source = SourceDataBuilder.With(c => c.Code = "ABIM").Build())
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Timelimited)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .With(b => b.ExpirationDate = new DateTime(_lookbackDate.Year + 1, 12, 31))
                                    .Build());

                _creds[0].Certification.Code = certCode;
                _creds[0].Certification.ExternalId = certId;

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = 2018;

                regResource.Registrations.Add(reg);

                // ----- CMPRegistrationResource ----------

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Fail)
                                           .WithTestDate(new DateTime(2018, 12, 31))
                                           .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                           .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThen_AssessmentMet_Should_HaveBeenSet_To_True()
            {
                _creds[0].AssessmentMet.ShouldBeTrue();
            }
        }
        #endregion

        #region Set 2 Year Pass Due (  ConsecutiveKCIPassRequired )
        private class ShouldSetConsecutiveKCIPassRequired : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            IssuanceDataBuilder IssuanceDataBuilder = new IssuanceDataBuilder();
            SourceDataBuilder SourceDataBuilder = new SourceDataBuilder();
            //----------------- variable input parameter ---------------
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            //------------------------------------------------------------
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));

                //----------------- variable input parameter ---------------
                _creds[0].ConsecutiveKCIPassRequired = false;
                _creds[0].MOCExamDueDate = _lookbackDate;
                _creds[0].ExamDueDate = _lookbackDate;
                //----------------------------------------------------------

                ////Active TL issuance NotMaintained
                _creds[0].AddIssuance(IssuanceDataBuilder
                                    .With(a => a.Source = SourceDataBuilder.With(c => c.Code = "ABIM").Build())
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Timelimited)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .With(b => b.ExpirationDate = new DateTime(_lookbackDate.Year + 1, 12, 31))
                                    .Build());

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);

                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                reg.Result = ExamResultType.Fail.ToString();
                reg.AdministrationYear = 2018;


                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThen_ConsecutiveKCIPassRequired_ShouldHaveBeenSet()
            {
                _creds[0].ConsecutiveKCIPassRequired.ShouldBeTrue();
            }
        }

        private class ShouldNOTSetConsecutiveKCIPassRequired : ProgramRulesServiceSimplifiedScenario
        {
            private Guid _memberId;
            private List<Credential> _creds;
            IssuanceDataBuilder IssuanceDataBuilder = new IssuanceDataBuilder();
            SourceDataBuilder SourceDataBuilder = new SourceDataBuilder();
            //----------------- variable input parameter ---------------
            private DateTime _lookbackDate = new DateTime(2018, 12, 31);
            //------------------------------------------------------------
            private Exception exception;
            private Guid certId = Guid.NewGuid();

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(1);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                _creds.Add(CredentialBuilder.Build(abimSource));

                //----------------- variable input parameter ---------------
                _creds[0].ConsecutiveKCIPassRequired = false;
                _creds[0].MOCExamDueDate = _lookbackDate;
                _creds[0].ExamDueDate = _lookbackDate;
                //----------------------------------------------------------

                ////Active TL issuance NotMaintained
                _creds[0].AddIssuance(IssuanceDataBuilder
                                    .With(a => a.Source = SourceDataBuilder.With(c => c.Code = "ABIM").Build())
                                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                                    .With(b => b.Duration = DurationType.Timelimited)
                                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                                    .With(b => b.ExpirationDate = new DateTime(_lookbackDate.Year + 1, 12, 31))
                                    .Build());

                _creds[0].Certification.Code = "ICARD";
                _creds[0].Certification.ExternalId = certId;

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());


                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(_lookbackDate.Year - 20, 2, 21));
            }

            protected override void SetupProductInterserviceMock()
            {
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);

                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>();

                var activity = new ActivityResource();
                activity.TotalMOCPoints = 15;
                activity.CompletedDate = new DateTime(DateTime.Now.Year - 2, 3, 20);
                activity.ActivityResult = new EnumValueResponseResource<ActivityResultType>(ActivityResultType.Pass);
                activity.Product = new ProductResource() { Code = RandomString.Build() };

                activities.Data.Add(activity);

                _prodInterSvcMock.Setup(
                        x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(),
                                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var regResource = new UserRegistrationsAndCMPRegistrationsResource();

                regResource.Registrations = new List<RegistrationResource>(1);
                regResource.CMPRegistrations = new List<CMPRegistrationResource>(0);


                var reg = new RegistrationResource();
                reg.CertificationId = certId;
                reg.ExamType = new RegistrationEnumValueResponseResource<ExamType>(ExamType.Moc);
                //------------------------- Has Pending Exam -------------------------
                reg.Result = ExamResultType.Pending.ToString();
                //--------------------------------------------------------------------
                reg.AdministrationYear = 2018;

                regResource.Registrations.Add(reg);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                // ----- CMPRegistrationResource ----------

                var cmpExamBuilder = new CMPExamSummaryResourceBuilder();
                var cmpRegBuilder = new CMPRegistrationResourceBuilder();

                var cmpReg = cmpRegBuilder.WithExamResult(ExamResultType.Pending)
                                          .WithTestDate(new DateTime(2018, 12, 31))
                                          .WithCMPExam(cmpExamBuilder.WithCertificationId(certId).Build())
                                          .Build();

                regResource.CMPRegistrations.Add(cmpReg);

                // Mock registration Inter Svc Mock

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);

                _regInterSvcMock.Setup(
                        x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(regResource));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));

            }

            protected override void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenServiceMock.Setup(x => x.GetAccessToken())
                    .Returns(RandomString.Build());
            }

            protected override void SetupLookBackDatesInfoServiceMock()
            {
                base.SetupLookBackDatesInfoServiceMock();

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(new List<LookBackDatesInfo>().AsEnumerable()));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.GetLookBackDatesInfo(It.IsAny<Guid>()))
                    .Returns(Task.FromResult(LookBackDatesInfo.Create(Guid.NewGuid(), null, null, null, null, "")));

                _lookBackDatesInfoSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateLookBackDatesInfoCommand>()))
                    .Returns(Task.FromResult(true));
            }

            private void GivenThatIHaveADiplomateIdAndLookbackDate()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    _sut.ProcessingDate = _lookbackDate;
                    await _sut.RunYearEndLookback(Guid.NewGuid(), _lookbackDate, _lookbackDate);
                }
                catch (Exception ex)
                {
                    exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                exception.ShouldBeNull();
            }

            private void AndThen_ConsecutiveKCIPassRequired_Should_NOT_HaveBeenSet()
            {
                _creds[0].ConsecutiveKCIPassRequired.ShouldBeFalse();
            }
        }
        #endregion Set 2 Year Pass Due (  ConsecutiveKCIPassRequired )

        #region Should Not Process Cosponsored Credentials

        private class ShouldNotProcessCosponsoredCredentials : ProgramRulesServiceSimplifiedScenario
        {
            private List<Credential> _creds;
            private Guid _memberId;
            private Exception _exception;
            private Mock<ILogger> _loggerMock;

            protected override void SetupMocks()
            {
                _loggerMock = new Mock<ILogger>(MockBehavior.Strict);
                _loggerMock.Setup(x => x.Info(It.IsAny<string>()));
                base.SetupMocks();
            }

            protected override void Setup()
            {
                base.Setup();
                _sut.Log = _loggerMock.Object; //The service apparently has 2 loggers. Not sure why. But this is the one to use.
            }

            protected override void SetupCredentialServiceMock()
            {
                _creds = new List<Credential>(3);

                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");

                //We'll make 3 just for good measure
                for (int x = 0; x < 3; x++)
                {
                    _creds.Add(CredentialBuilder.Build(abimSource));
                    _creds[x].IsCosponsored = true;
                }

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);
                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());
            }

            private void GivenThatIHaveADiplomateId()
            {
                _memberId = Guid.NewGuid();
            }

            private async void WhenICallRunYearEndLookback()
            {
                try
                {
                    await _sut.RunYearEndLookback(_memberId, DateTime.Now, DateTime.Now);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                _exception.ShouldBeNull();
            }

            private void AndTheLoggerShouldShowThatWeHadNoEligibleCredentials()
            {
                _loggerMock.Verify(x =>
                    x.Info($"Member {_memberId} does not have any credentials eligible for lookback."),
                    Times.Once);
            }
        }

        #endregion Should Not Process Cosponsored Credentials

        #endregion Scenarios
    }
}
