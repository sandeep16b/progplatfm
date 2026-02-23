using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Enums;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Testing.Setup.DataBuilders;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.Tests.Setup.ResourceBuilders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
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
      SoThat = "it can run processes when exam results are received"
      )]
    [TestFixture]
    public class RunProcessesOnExamResultEventSpec
    {
        [Test]
        public void Should_Update_Credential_Pathway_To_MOC_When_Applicable()
        {
            new ShouldUpdateCredentialPathwayToMOCWhenApplicable().BDDfy();
        }

        [Test]
        public void Should_Update_Credential_Pathway_To_KCI_When_Applicable()
        {
            new ShouldUpdateCredentialPathwayToKCIWhenApplicable().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Passed_MOC_Exam()
        {
            new ShouldSetDueDatesForPassedMOCExam().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Passed_KCI_Exam_Where_Consecutive_KCI_Pass_Required_First_Pass_AdminYear_GreaterThan_ExamDueDateYear()
        {
            new ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassAdminYearGreaterThanExamDueDateYear().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Passed_KCI_Exam_Where_Consecutive_KCI_Pass_Required_First_Pass_ExamDueDate_Is_Null()
        {
            new ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassExamDueDateIsNull().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Passed_KCI_Exam_Where_Consecutive_KCI_Pass_Required_Not_First_Pass()
        {
            new ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredNotFirstPass().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Passed_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_Due_Date_Is_ExamDueDate()
        {
            new ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsExamDueDate().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Passed_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_Due_Date_Is_End_Of_AdminYear_Plus_4()
        {
            new ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsEndOfAdminYearPlus4().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Failed_KCI_Exam_Where_Consecutive_KCI_Pass_Required()
        {
            new ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired().BDDfy();
        }
        [Test]
        [WorkItem(173312)]
        public void Should_NotChange_DisplayExamDueDate_For_Failed_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required()
        {
            new ShouldNotChangeDisplayExamDueDateForFailedKCIExamWhereConsecutiveKCIPassNotRequired().BDDfy();
        }

        [Test]
        [WorkItem(197251)]
        public void Should_NotChange_ExamDueDate_For_KCI_Pass_WithEarlyTaker_NoConsecutiveKCIPassRequired()
        {
            new ShouldNotChangeExamDueDateForKCIPassWithEarlyTakerNoConsecutiveKCIPassRequired().BDDfy();
        }

        [Test]
        [WorkItem(197251)]
        public void Should_NotChange_ExamDueDate_For_KCI_Pass_WithEarlyTaker_ConsecutiveKCIPassRequired()
        {
            new ShouldNotChangeExamDueDateForKCIPassWithEarlyTakerConsecutiveKCIPassRequired().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_Indeterminate_KCI_Exam_Where_Consecutive_KCI_Pass_Required()
        {
            new ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassRequired().BDDfy();
        }

        [Test]
        public void Should_NotChange_Due_Dates_For_Indeterminate_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required()
        {
            new ShouldNotChangeDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired().BDDfy();
        }

        [Test]
        [WorkItem(151443)]
        public void Should_Set_DisplayExamDueDate_For_Indeterminate_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_ToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo()
        {
            new ShouldSetDisplayExamDueDateForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequiredToExamDueDateWhenGreaterThanAdminYearPlusTwo().BDDfy();
        }
        [Test]
        public void Should_Set_Due_Dates_For_Incomplete_KCI_Exam_Where_Consecutive_KCI_Pass_Required()
        {
            new ShouldSetDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassRequired().BDDfy();
        }

        [Test]
        [WorkItem(151443)]
        public void Should_Set_DisplayExamDueDate_For_Incomplete_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_ToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo()
        {
            new ShouldSetDisplayExamDueDateForIncompleteKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo().BDDfy();
        }

        [Test]
        public void Should_NotChange_Due_Dates_For_Incomplete_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required()
        {
            new ShouldNotChangeDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassNotRequired().BDDfy();
        }

        [Test]
        public void Should_Set_Due_Dates_For_UnableToTest_KCI_Exam_Where_Consecutive_KCI_Pass_Required()
        {
            new ShouldSetDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassRequired().BDDfy();
        }

        [Test]
        public void Should_NotChange_Due_Dates_For_UnableToTest_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required()
        {
            new ShouldNotChangeDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassNotRequired().BDDfy();
        }

        [Test]
        [WorkItem(151443)]
        public void Should_Set_DisplayExamDueDate_For_UnableToTest_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_ToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo()
        {
            new ShouldSetDisplayExamDueDateForUnableToTestKCIExamWhereConsecutiveKCIPassNotRequiredToExamDueDateWhenGreaterThanAdminYearPlusTwo().BDDfy();
        }
        [Test]
        public void Should_Set_Due_Dates_For_Passed_CMP_Exam()
        {
            new Should_Set_Due_Dates_For_Passed_CMP_Exam_Scenario().BDDfy();
        }

        /*
        Two tests were removed here.
        PBI 150806 had logic to update the display due date for a CMP if the result was Fail or Unable to Test (IND/INC 
        aren't supported for CMP), but this was removed per the subsequent PBI 158548. Therefore the corresponding 
        tests were removed here as well.
        */

        [Test]
        public void Should_Update_Credential_When_Changed()
        {
            new ShouldUpdateCredentialWhenChanged().BDDfy();
        }

        [Test]
        public void Should_Treat_Bad_Result_As_Passing_When_No_Consequence_And_AdminYear_Less_Than_Or_Equal_ExamDueDate_Year_And_ConsecutiveKCIPass_Not_Reqd()
        {
            new ShouldTreatBadResultAsPassingWhenNoConsequenceAndAdminYearLessThanOrEqualExamDueDateYearAndConsecutiveKCIPassNotReqd().BDDfy();
        }

        [Test]
        public void Should_Not_Treat_Bad_Result_As_Passing_When_No_Consequence_And_AdminYear_Greater_Than_ExamDueDate_Year_And_ConsecutiveKCIPass_Not_Reqd()
        {
            new ShouldNotTreatBadResultAsPassingWhenNoConsequenceAndAdminYearGreaterThanExamDueDateYearAndConsecutiveKCIPassNotReqd().BDDfy();
        }

        [Test]
        public void Should_Not_Treat_Bad_Result_As_Passing_When_No_Consequence_And_AdminYear_Less_Than_Or_Equal_To_ExamDueDateYear_And_ConsecutiveKCIPassReqd()
        {
            new ShouldNotTreatBadResultAsPassingWhenNoConsequenceAndAdminYearLessThanOrEqualToExamDueDateYearAndConsecutiveKCIPassReqd().BDDfy();
        }

        [Test]
        public void Should_Not_Update_Due_Dates_For_Incomplete_KCI_When_Consecutive_Pass_Required_And_First_Attempt()
        {
            new ShouldNotUpdateDueDatesForIncompleteKCIWhenConsecutivePassRequiredAndFirstAttempt().BDDfy();
        }

        [Test]
        public void Should_Not_Treat_Bad_Result_As_Passing_When_Not_NoConsequence()
        {
            new ShouldNotTreatBadResultAsPassingWhenNotNoConsequence().BDDfy();
        }

        [Test]
        [WorkItem(136811)]
        public void Should_Issue_CreateCredentialIssuanceCommand_When_ExamResult_IsPass_ForAbimPhysician_And_Set_All_DueDates()
        {
            new ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForAbimPhysicianAndSetAllDueDates().BDDfy();
        }

        [Test]
        [WorkItem(215404)]
        public void Should_Issue_CreateCredentialIssuanceCommand_When_ExamResult_IsPass_For_Cosponsored_Exam_And_Set_All_DueDates()
        {
            new ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForCosponsoredExamAndSetAllDueDates().BDDfy();
        }

        [Test]
        [WorkItem(222010)]
        public void Should_Not_Call_GetFirstIssuanceDate_When_ExamResult_For_Cosponsored_Exam()
        {
            new ShouldNotCallGetFirstIssuanceDateWhenExamResultIsPassForCosponsoredExam().BDDfy();
        }

        [Test]
        [WorkItem(222010)]
        public void Should_Call_GetFirstIssuanceDate_When_ExamResult_Is_Pass_For_NOT_For_Cosponsored_Exam_And_ItIsSubscialty()
        {
            new ShouldCallGetFirstIssuanceDateWhenExamResultIsPassForNotCosponsoredExamAndItIsSubspecialty().BDDfy();
        }

        [Test]
        [WorkItem(136811)]
        public void Should_Not_Issue_CreateCredentialIssuanceCommand_When_ExamResult_IsFail()
        {
            new ShouldNotIssueCreateCredentialIssuanceCommandWhenExamResultIsFail().BDDfy();
        }

        [Test]
        [WorkItem(136811)]
        public void Should_ShouldNotIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForNonAbimPhysician()
        {
            new ShouldNotIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForNonAbimPhysician().BDDfy();
        }

        //Test no longer relevant per changes from Don on 10/17/2019
        /*
        [Test]
        [WorkItem(151443)]
        public void Should_Set_DisplayDueDate_For_Passed_KCIExam_To_ExamDueDate_If_Greater_Than_AdministrationYearPlusTwo()
        {
            new ShouldSetDisplayDueDateForPassedKCIExamToExamDueDateIfGreaterThanAdministrationYearPlusTwo().BDDfy();
        }
        */

        [Test]
        public void Should_Update_Credential_Pathway_To_1Year_When_Applicable()
        {
            new ShouldUpdateCredentialPathwayTo1YearWhenApplicableScenario().BDDfy();
        }

        [Test]
        [WorkItem(178847)]
        public void Should_Set_DisplayExamDueDate_For_Pass_NoConsequnce_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_ToDisplayExamDueDateWhenGreaterThanAdminYearPlusFour()
        {
            new ShouldSetDisplayExamDueDateForPassNoConsequnceKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusFour().BDDfy();
        }

        [Test]
        [WorkItem(178847)]
        public void Should_Set_DisplayExamDueDate_For_Pass_Consequnce_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_ToDisplayExamDueDateWhenGreaterThanAdminYearPlusFour()
        {
            new ShouldSetDisplayExamDueDateForPassConsequnceKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusFour().BDDfy();
        }

        [Test]
        [WorkItem(178847)]
        public void Should_Set_DisplayExamDueDate_For_Fail_NoConsequnce_KCI_Exam_Where_Consecutive_KCI_Pass_Not_Required_ToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo()
        {
            new ShouldSetDisplayExamDueDateForFailNoConsequnceKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo().BDDfy();
        }
        #region Scenarios

        #region Base
        private abstract class RunProcessesOnExamResultEventScenario : ProgramRulesServiceSimplifiedScenario
        {
            protected Guid _registrationId = Guid.NewGuid();
            protected DateTime _processingDate;
            protected Exception _caughtException;
            protected Credential _cred;
            protected RegistrationResource _reg;
            protected Certification _cert;

            //protected override void SetupAccessTokenServiceMock()
            //{
            //    base.SetupAccessTokenServiceMock();
            //    _accessTokenSvcMock.Setup(x => x.GetAccessToken()).Returns(Task.FromResult("someToken"));
            //}
            protected Credential SetupSharedCredential(Program.Resources.PathwayType pathwayType)
            {
                var source = (new SourceDataBuilder()).With(a => a.Code = "ABIM").Build();

                var certification = (new CertificationDataBuilder(source)).Build();

                var issuance = (new IssuanceDataBuilder())
                    .With(a => a.Source = source)
                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                    .With(b => b.Duration = DurationType.Timelimited)
                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                    .Build();

                var credential = (new CredentialDataBuilder(certification))
                        .With(i => i.Pathway = pathwayType)
                        .Build();

                credential.AddIssuance(issuance);

                return credential;
            }

            protected virtual void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.MOC);
            }

            protected override void SetupCredentialServiceMock()
            {

                SetupCredential();
                base.SetupCredentialServiceMock();

                _credSvcMock
                    .Setup(x => x.SearchByMemberId(It.IsAny<Guid>()))
                    .Returns(new List<Credential>(1) { _cred });

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                    .Returns(new UpdateCredentialOnExamResultCommandResult());
            }

            protected virtual void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Moc)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(new DateTime())
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .Build();
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                SetupRegistration();

                base.SetupRegistrationInterserviceMock();
                _regInterSvcMock
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), _registrationId))
                    .Returns(Task.FromResult(_reg));
            }

            protected override void SetupProductInterserviceMock()
            {
                base.SetupProductInterserviceMock();

                ActivityFullCollectionResource ActivitiesFullCollectionResource = new ActivityFullCollectionResource();

                _prodInterSvcMock
                    .Setup(p => p.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(ActivitiesFullCollectionResource));

            }


            protected void GivenIHaveARegistrationIdAndProcessingDate()
            {
                //_registrationId is already set
                _processingDate = new DateTime(2018, 11, 16);
            }

            protected async Task WhenICallRunProcessesOnExamResultEvent()
            {
                try
                {
                    await _sut.RunProcessesOnExamResultEvent(_registrationId, _processingDate, ExamRegistrationType.Registration);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBeNull();
            }
        }

        private abstract class RunProcessesOnCMPExamResultEventScenario : ProgramRulesServiceSimplifiedScenario
        {
            protected Guid _registrationId = Guid.NewGuid();
            protected DateTime _processingDate;
            protected Exception _caughtException;
            protected Credential _cred;
            protected CMPRegistrationResource _reg;
            protected Certification _cert;

            //protected override void SetupAccessTokenServiceMock()
            //{
            //    base.SetupAccessTokenServiceMock();
            //    _accessTokenSvcMock.Setup(x => x.GetAccessToken()).Returns(Task.FromResult("someToken"));
            //}
            protected Credential SetupSharedCredential(Program.Resources.PathwayType pathwayType)
            {
                var source = (new SourceDataBuilder()).With(a => a.Code = "ABIM").Build();

                var certification = (new CertificationDataBuilder(source)).Build();

                var issuance = (new IssuanceDataBuilder())
                    .With(a => a.Source = source)
                    .With(b => b.IssuanceStatus = IssuanceStatusType.Active)
                    .With(b => b.Duration = DurationType.Timelimited)
                    .With(b => b.MaintenanceRequirement = MaintenanceRequirementType.NotRequired)
                    .With(b => b.MaintenanceStatus = MaintenanceStatusType.Maintained)
                    .Build();

                var credential = (new CredentialDataBuilder(certification))
                        .With(i => i.Pathway = pathwayType)
                        .Build();

                credential.AddIssuance(issuance);

                return credential;
            }

            protected virtual void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.MOC);
            }

            protected override void SetupCredentialServiceMock()
            {

                SetupCredential();
                base.SetupCredentialServiceMock();

                _credSvcMock
                    .Setup(x => x.SearchByMemberId(It.IsAny<Guid>()))
                    .Returns(new List<Credential>(1) { _cred });

                _credSvcMock
                    .Setup(x => x.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()))
                    .Returns(new UpdateCredentialOnExamResultCommandResult());
            }

            protected virtual void SetupRegistration()
            {
                var builder = new CMPRegistrationResourceBuilder();
                var examBuilder = new CMPExamSummaryResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(examBuilder.WithCertificationId(_cred.Certification.ExternalId).WithNoConsequenceYear(new DateTime().Year).Build())
                    .WithTestDate(new DateTime())
                    .Build();
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                SetupRegistration();

                base.SetupRegistrationInterserviceMock();
                _regInterSvcMock
                    .Setup(x => x.GetCMPRegistrationById(It.IsAny<string>(), _registrationId))
                    .Returns(Task.FromResult(_reg));
            }

            protected void GivenIHaveARegistrationIdAndProcessingDate()
            {
                //_registrationId is already set
                _processingDate = new DateTime(2018, 11, 16);
            }

            protected async Task WhenICallRunProcessesOnExamResultEvent()
            {
                try
                {
                    await _sut.RunProcessesOnExamResultEvent(_registrationId, _processingDate, ExamRegistrationType.CMPRegistration);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBeNull();
            }
        }
        #endregion Base

        #region ShouldUpdateCredentialPathwayToMOCWhenApplicable
        private class ShouldUpdateCredentialPathwayToMOCWhenApplicable : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Moc)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(new DateTime())
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.DisplayExamDueDate = null;
                _cred.MOCExamDueDate = new DateTime(DateTime.Now.Year + 10, 12, 31);
            }

            private void AndGivenTheStartingPathwayIsKCI()
            {
                _cred.Pathway.Should().Be(Resources.PathwayType.KCI);
            }

            private void AndTheCredentialPathwayShouldBeMOC()
            {
                _cred.Pathway.Should().Be(Resources.PathwayType.MOC);
            }
            //PBI 150794 - no longer updating displayExamDueDate during SetCredentialPathwayFromExamResultIfApplicable
            private void AndTheDisplayDueDateShouldNotHaveBeenChanged()
            {
                _cred.DisplayExamDueDate.Should().Be(null);
            }
        }
        #endregion ShouldUpdateCredentialPathwayToMOCWhenApplicable

        #region ShouldUpdateCredentialPathwayToKCIWhenApplicable
        private class ShouldUpdateCredentialPathwayToKCIWhenApplicable : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(new DateTime())
                    .WithAdministrationDate(new DateTime())
                    .Build();
            }

            private void AndGivenTheStartingPathwayIsMOC()
            {
                _cred.Pathway.Should().Be(Resources.PathwayType.MOC);
            }

            private void AndTheCredentialPathwayShouldBeKCI()
            {
                _cred.Pathway.Should().Be(Resources.PathwayType.KCI);
            }
        }
        #endregion ShouldUpdateCredentialPathwayToKCIWhenApplicable

        #region ShouldUpdateCredentialPathwayTo1YearWhenApplicable
        private class ShouldUpdateCredentialPathwayTo1YearWhenApplicableScenario : RunProcessesOnCMPExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new CMPRegistrationResourceBuilder();
                var examBuilder = new CMPExamSummaryResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(examBuilder.WithCertificationId(_cred.Certification.ExternalId).WithNoConsequenceYear(new DateTime().Year).Build())
                    .WithTestDate(new DateTime())
                    .Build();
            }

            private void AndGivenTheStartingPathwayIsMOC()
            {
                _cred.Pathway.Should().Be(Resources.PathwayType.MOC);
            }

            private void AndTheCredentialPathwayShouldBeOneYear()
            {
                _cred.Pathway.Should().Be(Resources.PathwayType.OneYear);
            }

        }
        #endregion ShouldUpdateCredentialPathwayTo1YearWhenApplicable

        #region ShouldSetDueDatesForPassedMOCExam
        private class ShouldSetDueDatesForPassedMOCExam : RunProcessesOnExamResultEventScenario
        {
            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 10, 12, 31);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
                _cred.KCIExamDueDate.Should().Be(expectedDate);
                _cred.MOCExamDueDate.Should().Be(expectedDate);
            }

            private void AndAssessmentPropertiesShouldBeSetCorrectly()
            {
                _cred.ConsecutiveKCIPassRequired.Should().BeFalse();
                _cred.AssessmentMet.Should().BeTrue();
                _cred.AssessmentMetDate.Should().Be(_reg.Seats[0].SeatDate);
                _cred.GracePeriodStartDate.ShouldBeNull();
                _cred.GracePeriodEndDate.ShouldBeNull();
            }
        }
        #endregion ShouldSetDueDatesForPassedMOCExam

        #region ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassAdminYearGreaterThanExamDueDateYear
        private class ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassAdminYearGreaterThanExamDueDateYear : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime().AddYears(1))
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = DateTime.Now;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 2, 12, 31);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
                _cred.MOCExamDueDate.Should().Be(expectedDate);
            }
        }
        #endregion ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassAdminYearGreaterThanExamDueDateYear

        #region ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassExamDueDateIsNull
        private class ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassExamDueDateIsNull : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = null;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 2, 12, 31);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
                _cred.MOCExamDueDate.Should().Be(expectedDate);
            }
        }
        #endregion ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredFirstPassExamDueDateIsNull

        #region ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredNotFirstPass
        private class ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredNotFirstPass : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year - 1)
                    .WithAdministrationDate(new DateTime())
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = DateTime.Now;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 4, 12, 31);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.MOCExamDueDate.Should().Be(expectedDate);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
            }

            private void AndAssessmentPropertiesShouldBeSetCorrectly()
            {
                _cred.ConsecutiveKCIPassRequired.Should().BeFalse();
                _cred.AssessmentMet.Should().BeTrue();
                _cred.AssessmentMetDate.Should().Be(_reg.Seats[0].SeatDate.Date);
                _cred.GracePeriodStartDate.ShouldBeNull();
                _cred.GracePeriodEndDate.ShouldBeNull();
            }
        }
        #endregion ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassRequiredNotFirstPass

        #region ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsExamDueDate
        private class ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsExamDueDate : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ExamDueDate = DateTime.Now.AddYears(7);
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.KCIExamDueDate.Should().Be(new DateTime(DateTime.Now.Year + 5, 12, 31));
                //PBI 150794
                //Revised per Bug 161596 to use ExamDueDate instead of MOCExamDueDate
                //Revised later to use DisplayExamDueDate instead
                _cred.DisplayExamDueDate.Should().Be(new List<DateTime?> { new DateTime(_reg.AdministrationYear + 2, 12, 31), _cred.DisplayExamDueDate }.Max());
            }

            private void AndAssessmentPropertiesShouldBeSetCorrectly()
            {
                _cred.ConsecutiveKCIPassRequired.Should().BeFalse();
                _cred.AssessmentMet.Should().BeTrue();
                _cred.AssessmentMetDate.Should().Be(_reg.Seats[0].SeatDate.Date);
                _cred.GracePeriodStartDate.ShouldBeNull();
                _cred.GracePeriodEndDate.ShouldBeNull();
            }
        }
        #endregion ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsExamDueDate

        #region ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsEndOfAdminYearPlus4
        private class ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsEndOfAdminYearPlus4 : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.MOCExamDueDate = DateTime.Now.AddYears(2);
                _cred.ExamDueDate = _cred.MOCExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 4, 12, 31);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.KCIExamDueDate.Should().Be(expectedDate);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
            }

            private void AndAssessmentPropertiesShouldBeSetCorrectly()
            {
                _cred.ConsecutiveKCIPassRequired.Should().BeFalse();
                _cred.AssessmentMet.Should().BeTrue();
                _cred.AssessmentMetDate.Should().Be(_reg.Seats[0].SeatDate.Date);
                _cred.GracePeriodStartDate.ShouldBeNull();
                _cred.GracePeriodEndDate.ShouldBeNull();
            }
        }
        #endregion ShouldSetDueDatesForPassedKCIExamWhereConsecutiveKCIPassNotRequiredDueDateIsEndOfAdminYearPlus4

        #region ShouldSetDisplayDueDateForPassedKCIExamToExamDueDateIfGreaterThanAdministrationYearPlusTwo
        private class ShouldSetDisplayDueDateForPassedKCIExamToExamDueDateIfGreaterThanAdministrationYearPlusTwo : RunProcessesOnExamResultEventScenario
        {
            private DateTime _examDueDate = DateTime.Now.AddYears(7);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ExamDueDate = _examDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.ExamDueDate.Should().Be(_examDueDate);
                _cred.KCIExamDueDate.Should().Be(new DateTime(2024, 12, 31));
                _cred.DisplayExamDueDate.Should().Be(_examDueDate);
            }

            private void AndAssessmentPropertiesShouldBeSetCorrectly()
            {
                _cred.ConsecutiveKCIPassRequired.Should().BeFalse();
                _cred.AssessmentMet.Should().BeTrue();
                _cred.AssessmentMetDate.Should().Be(_reg.Seats[0].SeatDate.Date);
                _cred.GracePeriodStartDate.ShouldBeNull();
                _cred.GracePeriodEndDate.ShouldBeNull();
            }
        }
        #endregion


        #region ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired
        private class ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.ExamDueDate.ShouldBeNull();
                _cred.MOCExamDueDate.ShouldBeNull();
            }
        }
        #endregion ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldNotChangeDisplayExamDueDateForFailedKCIExamWhereConsecutiveKCIPassNotRequired : RunProcessesOnExamResultEventScenario
        {
            private DateTime _examDueDate = DateTime.Now.AddYears(7);
            private DateTime _adminDate = DateTime.Now;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(_adminDate.Year)
                    .WithAdministrationDate(_adminDate)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.MOCExamDueDate = _examDueDate;
                _cred.ExamDueDate = _examDueDate;
                _cred.DisplayExamDueDate = _adminDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.DisplayExamDueDate.Should().Be(_adminDate);
            }
        }
        #endregion ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldNotChangeExamDueDateForKCIPassWithEarlyTakerNoConsecutiveKCIPassRequired
        private class ShouldNotChangeExamDueDateForKCIPassWithEarlyTakerNoConsecutiveKCIPassRequired : RunProcessesOnExamResultEventScenario
        {
            private DateTime _examDueDate = DateTime.Now.AddYears(10);
            private DateTime _adminDate = DateTime.Now;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(_adminDate.Year)
                    .WithAdministrationDate(_adminDate)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.MOCExamDueDate = _examDueDate;
                _cred.ExamDueDate = _examDueDate;
                _cred.DisplayExamDueDate = _examDueDate;
            }
            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.ExamDueDate.Should().Be(_examDueDate);
                _cred.DisplayExamDueDate.Should().Be(_examDueDate);
                _cred.MOCExamDueDate.Should().Be(_examDueDate);
            }
        }
        #endregion ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldNotChangeExamDueDateForKCIPassWithEarlyTakerConsecutiveKCIPassRequired
        private class ShouldNotChangeExamDueDateForKCIPassWithEarlyTakerConsecutiveKCIPassRequired : RunProcessesOnExamResultEventScenario
        {
            private DateTime _examDueDate = DateTime.Now.AddYears(10);
            private DateTime _adminDate = DateTime.Now;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(_adminDate.Year)
                    .WithAdministrationDate(_adminDate)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.MOCExamDueDate = _examDueDate;
                _cred.ExamDueDate = _examDueDate;
                _cred.DisplayExamDueDate = _examDueDate;
            }
            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.ExamDueDate.Should().Be(_examDueDate);
                _cred.DisplayExamDueDate.Should().Be(_examDueDate);
                _cred.MOCExamDueDate.Should().Be(_examDueDate);
            }
        }
        #endregion ShouldSetDueDatesForFailedKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassRequired
        private class ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassRequired : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Indeterminate)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year - 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = DateTime.Now;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 2, 12, 31);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.KCIExamDueDate.Should().Be(expectedDate);
            }
        }
        #endregion ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldNotChangeDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired : RunProcessesOnExamResultEventScenario
        {
            private DateTime? _startingExamDueDate;
            private DateTime? _startingMOCExamDueDate;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Indeterminate)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _startingExamDueDate = _cred.ExamDueDate;
                _startingMOCExamDueDate = _cred.MOCExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                // PbI 178847 : (Proj 1474) Display Consequential Due Dates for Two Year Assessments
                // *** don't advance for Inc/Ind/Utt
                _cred.DisplayExamDueDate.Should().Be(_startingExamDueDate);
                _cred.ExamDueDate.Should().Be(_startingExamDueDate);
                _cred.MOCExamDueDate.Should().Be(_startingMOCExamDueDate);
            }
        }
        #endregion ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired

        #region ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldSetDisplayExamDueDateForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequiredToExamDueDateWhenGreaterThanAdminYearPlusTwo : RunProcessesOnExamResultEventScenario
        {
            private DateTime _examDueDate = DateTime.Now.AddYears(7);
            private DateTime _displayExamDueDate = new DateTime(2025, 12, 31);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Indeterminate)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(DateTime.Now)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = _examDueDate;
                _cred.MOCExamDueDate = _examDueDate;
                _cred.DisplayExamDueDate = _displayExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.DisplayExamDueDate.Should().Be(_displayExamDueDate);
            }
        }
        #endregion ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired



        #region ShouldSetDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassRequired
        private class ShouldSetDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassRequired : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Incomplete)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year - 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = DateTime.Now;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 2, 12, 31);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.KCIExamDueDate.Should().Be(expectedDate);
            }
        }
        #endregion ShouldSetDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldSetDisplayExamDueDateForIncompleteKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo : RunProcessesOnExamResultEventScenario
        {
            private DateTime? _startingExamDueDate;
            private DateTime? _startingMOCExamDueDate;
            private DateTime _examDueDate = DateTime.Now.AddYears(7);
            private DateTime _displayExamDueDate = new DateTime(2025, 12, 31);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Incomplete)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(DateTime.Now)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = _examDueDate;
                _cred.DisplayExamDueDate = _displayExamDueDate;
                _startingExamDueDate = _cred.ExamDueDate;
                _startingMOCExamDueDate = _cred.MOCExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.DisplayExamDueDate.Should().Be(_displayExamDueDate);
                _cred.ExamDueDate.Should().Be(_startingExamDueDate);
                _cred.MOCExamDueDate.Should().Be(_startingMOCExamDueDate);
            }
        }
        #endregion ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired

        #region ShouldSetDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldNotChangeDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassNotRequired : RunProcessesOnExamResultEventScenario
        {
            private DateTime? _startingExamDueDate;
            private DateTime? _startingMOCExamDueDate;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Incomplete)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;

                _startingExamDueDate = _cred.ExamDueDate;
                _startingMOCExamDueDate = _cred.MOCExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                // PbI 178847 : (Proj 1474) Display Consequential Due Dates for Two Year Assessments
                // *** don't advance for Inc/Ind/Utt
                _cred.DisplayExamDueDate.Should().Be(_startingExamDueDate);

                _cred.ExamDueDate.Should().Be(_startingExamDueDate);
                _cred.MOCExamDueDate.Should().Be(_startingMOCExamDueDate);
            }
        }
        #endregion #region ShouldSetDueDatesForIncompleteKCIExamWhereConsecutiveKCIPassNotRequired

        #region ShouldSetDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassRequired
        private class ShouldSetDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassRequired : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.UnableToTest)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year - 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = DateTime.Now;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.AdministrationYear + 2, 12, 31);
                _cred.DisplayExamDueDate.Should().Be(expectedDate);
                _cred.ExamDueDate.Should().Be(expectedDate);
                _cred.KCIExamDueDate.Should().Be(expectedDate);
            }
        }
        #endregion ShouldSetDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassRequired

        #region ShouldSetDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldNotChangeDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassNotRequired : RunProcessesOnExamResultEventScenario
        {
            private DateTime? _startingExamDueDate;
            private DateTime? _startingMOCExamDueDate;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.UnableToTest)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _startingExamDueDate = _cred.ExamDueDate;
                _startingMOCExamDueDate = _cred.MOCExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.DisplayExamDueDate.Should().Be(_startingExamDueDate);
                _cred.ExamDueDate.Should().Be(_startingExamDueDate);
                _cred.MOCExamDueDate.Should().Be(_startingMOCExamDueDate);
            }
        }
        #endregion ShouldSetDueDatesForUnableToTestKCIExamWhereConsecutiveKCIPassNotRequired

        #region ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired
        private class ShouldSetDisplayExamDueDateForUnableToTestKCIExamWhereConsecutiveKCIPassNotRequiredToExamDueDateWhenGreaterThanAdminYearPlusTwo : RunProcessesOnExamResultEventScenario
        {
            private DateTime _examDueDate = DateTime.Now.AddYears(7);
            private DateTime _displayExamDueDate = new DateTime(2025, 12, 31);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.UnableToTest)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(DateTime.Now)
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = _examDueDate;
                _cred.DisplayExamDueDate = _displayExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                _cred.DisplayExamDueDate.Should().Be(_displayExamDueDate);
            }
        }
        #endregion ShouldSetDueDatesForIndeterminateKCIExamWhereConsecutiveKCIPassNotRequired

        #region ShouldUpdateCredentialWhenChanged
        private class ShouldUpdateCredentialWhenChanged : RunProcessesOnExamResultEventScenario
        {
            private void AndTheCredentialShouldHaveBeenUpdated()
            {
                _credSvcMock.Verify(x =>
                    x.Handle(It.IsAny<UpdateCredentialOnExamResultCommand>()),
                    Times.Once);
            }
        }
        #endregion ShouldUpdateCredentialWhenChanged

        private class ShouldTreatBadResultAsPassingWhenNoConsequenceAndAdminYearLessThanOrEqualExamDueDateYearAndConsecutiveKCIPassNotReqd : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(true)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.MOC);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = DateTime.Now;
                _cred.AssessmentMet = false;
                _cred.ExamDueDate = DateTime.Now;
            }

            protected void AndTheCredentialShouldBeMarkedAsHavingTheAssessmentMet()
            {
                _cred.AssessmentMet.Should().BeTrue();
            }
        }

        private class ShouldNotTreatBadResultAsPassingWhenNoConsequenceAndAdminYearGreaterThanExamDueDateYearAndConsecutiveKCIPassNotReqd : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(true)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = CredentialBuilder.Build(Resources.PathwayType.MOC);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = DateTime.Now;
                _cred.AssessmentMet = false;
            }

            protected void AndTheCredentialShouldBeMarkedAsHavingTheAssessmentMet()
            {
                _cred.AssessmentMet.Should().BeFalse();
            }
        }

        private class ShouldNotTreatBadResultAsPassingWhenNoConsequenceAndAdminYearLessThanOrEqualToExamDueDateYearAndConsecutiveKCIPassReqd : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(true)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = CredentialBuilder.Build(Resources.PathwayType.MOC);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.ExamDueDate = DateTime.Now;
                _cred.AssessmentMet = false;
            }

            protected void AndTheCredentialShouldBeMarkedAsHavingTheAssessmentMet()
            {
                _cred.AssessmentMet.Should().BeFalse();
            }
        }

        private class ShouldNotTreatBadResultAsPassingWhenNotNoConsequence : RunProcessesOnExamResultEventScenario
        {
            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = CredentialBuilder.Build(Resources.PathwayType.MOC);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = DateTime.Now;
                _cred.AssessmentMet = false;
            }

            protected void AndTheCredentialShouldBeMarkedAsHavingTheAssessmentMet()
            {
                _cred.AssessmentMet.Should().BeFalse();
            }
        }


        private class ShouldNotUpdateDueDatesForIncompleteKCIWhenConsecutivePassRequiredAndFirstAttempt : RunProcessesOnExamResultEventScenario
        {
            private DateTime? _startingExamDueDate;
            private DateTime? _startingMOCExamDueDate;
            private DateTime? _startingDisplayExamDueDate;

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Incomplete)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(DateTime.Now)
                    .WithAdministrationYear(DateTime.Now.Year + 1)
                    .WithAdministrationDate(new DateTime())
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.MOC);
                _cred.ConsecutiveKCIPassRequired = true;
                _cred.AssessmentMet = false;
                _cred.ExamDueDate = DateTime.Now;

                _startingExamDueDate = _cred.ExamDueDate;
                _startingMOCExamDueDate = _cred.MOCExamDueDate;
                _startingDisplayExamDueDate = _cred.DisplayExamDueDate;
            }

            private void AndNoneOfTheDueDatesShouldHaveBeenUpdated()
            {
                _cred.ExamDueDate.Should().Be(_startingExamDueDate);
                _cred.MOCExamDueDate.Should().Be(_startingMOCExamDueDate);
                _cred.DisplayExamDueDate.Should().Be(_startingDisplayExamDueDate);
            }
        }





        private class ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForAbimPhysicianAndSetAllDueDates : RunProcessesOnExamResultEventScenario
        {

            protected DateTime SeatDate;
            protected DateTime FirstIssuanceDate;

            protected override void SetupCredentialServiceMock()
            {
                FirstIssuanceDate = new DateTime(DateTime.Today.Year - 2, 8, 1);

                base.SetupCredential();

                _credSvcMock = new Mock<ICredentialService>(MockBehavior.Default);

                _credSvcMock.Setup(x => x.Handle(It.IsAny<CreateCredentialIssuanceCommand>()))
                    .Returns(new CreateCredentialIssuanceCommandResult());

                _credSvcMock.Setup(p => p.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(FirstIssuanceDate);

            }

            protected override void SetupRegistration()
            {
                SeatDate = DateTime.Today;

                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithResult("Pass")
                    .WithExamType(ExamType.Cert)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(SeatDate)
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithPhyisicianIsAbim(true)
                    .Build();
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                SetupRegistration();

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), _registrationId))
                    .Returns(Task.FromResult(_reg));
            }
            protected virtual void SetupCertification()
            {
                _cert = CertificationBuilder.Build(SourceBuilder.Build(), "IM");
            }

            protected override void SetupCertificationServiceMock()
            {
                base.SetupCertificationServiceMock();
                SetupCertification();

                _certSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_cert);
            }

            protected virtual void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledWithTheCorrectValues()
            {
                var expectedDate = new DateTime(SeatDate.Year + 10, 12, 31);

                _credSvcMock
                  .Verify(x =>
                      x.Handle(It.Is<CreateCredentialIssuanceCommand>(cmd =>
                          cmd.ExamDueDate == expectedDate
                          && cmd.MOCExamDueDate == expectedDate
                          && cmd.KCIExamDueDate == expectedDate
                          && cmd.DisplayExamDueDate == expectedDate
                          && cmd.IsCosponsored == false
                          && cmd.OnBehalfBoardCode == null
                          && cmd.OnBehalfBoardName == null
                          && !cmd.ConsecutiveKCIPassRequired)), Times.Once);
            }

            protected virtual void AndTheCheckIfGetFirstIssuanceDateHaveBeenCalled()
            {
            }

            protected void AndConsecutiveKciPassShouldBeSetCorrectly()
            {
                _cred.ConsecutiveKCIPassRequired.Should().BeFalse();
            }
        }

        private class ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForCosponsoredExamAndSetAllDueDates : ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForAbimPhysicianAndSetAllDueDates
        {
            protected override void SetupRegistration()
            {
                SeatDate = DateTime.Today;

                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithResult("Pass")
                    .WithExamType(ExamType.Cert)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(SeatDate)
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithPhyisicianIsAbim(false)
                    .WithOnBehalfOf("ABPN")
                    .Build();
            }

            protected override void SetupSourceServiceMock()
            {
                var sources = new List<Source>(2);
                
                sources.Add(Source.Create("American Board of Plastic Surgery", "ABPS", "UnitTest"));
                sources.Add(Source.Create("American Board of Psychiatry & Neurology", "ABPN", "UnitTest"));
                
                base.SetupSourceServiceMock();
                _sourceSvcMock
                    .Setup(x => x.Search())
                    .Returns(sources.AsEnumerable());
            }

            protected override void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledWithTheCorrectValues()
            {
                var expectedDate = new DateTime(SeatDate.Year + 10, 12, 31);

                _credSvcMock
                  .Verify(x =>
                      x.Handle(It.Is<CreateCredentialIssuanceCommand>(cmd =>
                          cmd.ExamDueDate == expectedDate
                          && cmd.MOCExamDueDate == expectedDate
                          && cmd.KCIExamDueDate == expectedDate
                          && cmd.DisplayExamDueDate == expectedDate
                          && cmd.IsCosponsored == true
                          && cmd.OnBehalfBoardCode == "ABPN"
                          && cmd.OnBehalfBoardName == "American Board of Psychiatry & Neurology"
                          && !cmd.ConsecutiveKCIPassRequired)), Times.Once);
            }
        }

        private class ShouldNotCallGetFirstIssuanceDateWhenExamResultIsPassForCosponsoredExam : ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForAbimPhysicianAndSetAllDueDates
        {
            protected override void SetupRegistration()
            {
                SeatDate = DateTime.Today;

                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithResult("Pass")
                    .WithExamType(ExamType.Cert)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(SeatDate)
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithPhyisicianIsAbim(false)
                    .WithOnBehalfOf("ABPN") // CoSponsored
                    .Build();
            }

            protected override void SetupSourceServiceMock()
            {
                var sources = new List<Source>(2);

                sources.Add(Source.Create("American Board of Plastic Surgery", "ABPS", "UnitTest"));
                sources.Add(Source.Create("American Board of Psychiatry & Neurology", "ABPN", "UnitTest"));

                base.SetupSourceServiceMock();
                _sourceSvcMock
                    .Setup(x => x.Search())
                    .Returns(sources.AsEnumerable());
            }

            protected override void SetupCertification()
            {
                _cert = CertificationBuilder.Build(SourceBuilder.Build(), "HM");
            }

            protected override void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledWithTheCorrectValues()
            {
                var expectedDate = new DateTime(SeatDate.Year + 10, 12, 31);

                _credSvcMock
                  .Verify(x =>
                      x.Handle(It.Is<CreateCredentialIssuanceCommand>(cmd =>
                          cmd.ExamDueDate == expectedDate
                          && cmd.MOCExamDueDate == expectedDate
                          && cmd.KCIExamDueDate == expectedDate
                          && cmd.DisplayExamDueDate == expectedDate
                          && cmd.IsCosponsored == true
                          && cmd.OnBehalfBoardCode == "ABPN"
                          && cmd.OnBehalfBoardName == "American Board of Psychiatry & Neurology" // !!!
                          && !cmd.ConsecutiveKCIPassRequired)), Times.Once);
            }

            protected override void AndTheCheckIfGetFirstIssuanceDateHaveBeenCalled()
            {
                _credSvcMock
                  .Verify(x =>
                      x.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Never);
            }

        }

        private class ShouldCallGetFirstIssuanceDateWhenExamResultIsPassForNotCosponsoredExamAndItIsSubspecialty : ShouldIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForAbimPhysicianAndSetAllDueDates
        {
            protected override void SetupRegistration()
            {
                SeatDate = DateTime.Today;

                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithResult("Pass")
                    .WithExamType(ExamType.Cert)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(SeatDate)
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithPhyisicianIsAbim(true) // !!!!
                    .WithOnBehalfOf(null) // NOT CoSponsored
                    .Build();
            }

            protected override void SetupCertification()
            {
                _cert = CertificationBuilder.Build(SourceBuilder.Build(), "HM");
            }

            protected override void AndTheCheckIfGetFirstIssuanceDateHaveBeenCalled()
            {
                _credSvcMock
                  .Verify(x =>
                      x.GetFirstIssuanceDate(It.IsAny<Guid>()), Times.Once);
            }

        }

        private class ShouldNotIssueCreateCredentialIssuanceCommandWhenExamResultIsFail : RunProcessesOnExamResultEventScenario
        {
            private DateTime SeatDate;
            protected override void SetupCredentialServiceMock()
            {
                base.SetupCredential();

                _credSvcMock = new Mock<ICredentialService>(MockBehavior.Default);

                _credSvcMock.Setup(x => x.Handle(It.IsAny<CreateCredentialIssuanceCommand>()))
                    .Returns(new CreateCredentialIssuanceCommandResult());

            }
            protected override void SetupRegistration()
            {
                SeatDate = DateTime.Today;

                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithResult("Fail")
                    .WithExamType(ExamType.Cert)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(SeatDate)
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithPhyisicianIsAbim(true)
                    .Build();
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                SetupRegistration();

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), _registrationId))
                    .Returns(Task.FromResult(_reg));
            }
            protected virtual void SetupCertification()
            {
                _cert = CertificationBuilder.Build(SourceBuilder.Build(), "IM");
            }

            protected override void SetupCertificationServiceMock()
            {
                base.SetupCertificationServiceMock();
                SetupCertification();

                _certSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_cert);
            }

            private void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledWithTheCorrectValues()
            {
                _credSvcMock.Verify(p => p.Handle(It.IsAny<CreateCredentialIssuanceCommand>()), Times.Never());
            }

        }


        private class ShouldNotIssueCreateCredentialIssuanceCommandWhenExamResultIsPassForNonAbimPhysician : RunProcessesOnExamResultEventScenario
        {
            private DateTime SeatDate;
            protected override void SetupCredentialServiceMock()
            {
                base.SetupCredential();

                _credSvcMock = new Mock<ICredentialService>(MockBehavior.Default);

                _credSvcMock.Setup(x => x.Handle(It.IsAny<CreateCredentialIssuanceCommand>()))
                    .Returns(new CreateCredentialIssuanceCommandResult());

            }
            protected override void SetupRegistration()
            {
                SeatDate = DateTime.Today;

                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithResult("Pass")
                    .WithExamType(ExamType.Cert)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(SeatDate)
                    .WithAdministrationDate(new DateTime())
                    .WithAdministrationYear(new DateTime().Year)
                    .WithPhyisicianIsAbim(false)
                    .Build();
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                SetupRegistration();

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), _registrationId))
                    .Returns(Task.FromResult(_reg));
            }
            protected virtual void SetupCertification()
            {
                _cert = CertificationBuilder.Build(SourceBuilder.Build(), "IM");
            }

            protected override void SetupCertificationServiceMock()
            {
                base.SetupCertificationServiceMock();
                SetupCertification();

                _certSvcMock.Setup(x => x.Load(It.IsAny<Guid>()))
                    .Returns(_cert);
            }

            private void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledWithTheCorrectValues()
            {
                _credSvcMock.Verify(p => p.Handle(It.IsAny<CreateCredentialIssuanceCommand>()), Times.Never());
            }

        }

        private class Should_Set_Due_Dates_For_Passed_CMP_Exam_Scenario : RunProcessesOnCMPExamResultEventScenario
        {
            protected override void SetupCredential()
            {
                base.SetupCredential();
                _cred.MOCExamDueDate = new DateTime(2019, 12, 31);
                _cred.ExamDueDate = _cred.MOCExamDueDate;
            }
            protected override void SetupRegistration()
            {
                var builder = new CMPRegistrationResourceBuilder();
                var examBuilder = new CMPExamSummaryResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithCMPExam(examBuilder.WithCertificationId(_cred.Certification.ExternalId).WithNoConsequenceYear(new DateTime().Year).Build())
                    .WithTestDate(new DateTime(2019, 6, 11))
                    .Build();
            }

            private void AndGivenThatDatesAreNull()
            {
                //To verify these aren't already set to the value we'll be checking for later
                _cred.DisplayExamDueDate.ShouldBeNull();
                _cred.AssessmentMetDate.ShouldBeNull();
            }

            private void AndGivenThatAssessmentMetIsFalse()
            {
                _cred.AssessmentMet.ShouldBeFalse();
            }

            private void AndTheDueDatesShouldBeAsExpected()
            {
                var expectedDate = new DateTime(_reg.TestDate.Year + 1, 12, 31);
                _cred.ExamDueDate.ShouldBeEquivalentTo(expectedDate);
                _cred.DisplayExamDueDate.ShouldBeEquivalentTo(expectedDate);
            }

            private void AndAssessmentShouldBeMet()
            {
                _cred.AssessmentMet.ShouldBeTrue();
                _cred.AssessmentMetDate.ShouldBe(_reg.TestDate);
            }
        }

        private class ShouldSetDisplayExamDueDateForPassNoConsequnceKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusFour : RunProcessesOnExamResultEventScenario
        {
            private DateTime _startingExamDueDate = new DateTime(2022, 12, 31);

            private DateTime _examPassDate = new DateTime(2020, 11, 03);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(_examPassDate)
                    .WithAdministrationYear(_examPassDate.Year)
                    .WithAdministrationDate(_examPassDate.AddDays(-2))
                    .WithNoConsequence(false)
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = _startingExamDueDate;
                _cred.DisplayExamDueDate = _startingExamDueDate;
                _cred.KCIExamDueDate = _startingExamDueDate;
                _cred.DisplayExamDueDate = _startingExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDueDate = new DateTime(_examPassDate.Year + 4, 12, 31);

                _cred.ExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.DisplayExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.KCIExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.MOCExamDueDate.ShouldBeEquivalentTo(expectedDueDate);

            }
        }

        private class ShouldSetDisplayExamDueDateForPassConsequnceKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusFour : RunProcessesOnExamResultEventScenario
        {
            private DateTime _startingExamDueDate = new DateTime(2022, 12, 31);

            private DateTime _examPassDate = new DateTime(2020, 11, 03);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Pass)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(_examPassDate)
                    .WithAdministrationYear(_examPassDate.Year)
                    .WithAdministrationDate(_examPassDate.AddDays(-2))
                    .WithNoConsequence(true) // !!!!
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = _startingExamDueDate;
                _cred.DisplayExamDueDate = _startingExamDueDate;
                _cred.KCIExamDueDate = _startingExamDueDate;
                _cred.DisplayExamDueDate = _startingExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDueDate = new DateTime(_examPassDate.Year + 4, 12, 31);

                _cred.ExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.DisplayExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.KCIExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.MOCExamDueDate.ShouldBeEquivalentTo(expectedDueDate);

            }
        }

        private class ShouldSetDisplayExamDueDateForFailNoConsequnceKCIExamWhereConsecutiveKCIPassNotRequiredToDisplayExamDueDateWhenGreaterThanAdminYearPlusTwo : RunProcessesOnExamResultEventScenario
        {
            private DateTime _startingExamDueDate = new DateTime(2022, 12, 31);
            private DateTime _examPassDate = new DateTime(2020, 11, 03);

            protected override void SetupRegistration()
            {
                var builder = new RegistrationResourceBuilder();
                _reg = builder
                    .WithExamResult(ExamResultType.Fail)
                    .WithExamType(ExamType.Kci)
                    .WithCertificationId(_cred.Certification.ExternalId)
                    .WithSeat(_examPassDate)
                    .WithAdministrationYear(_examPassDate.Year)
                    .WithAdministrationDate(_examPassDate.AddDays(-2))
                    .WithNoConsequence(true) // !!!!
                    .Build();
            }

            protected override void SetupCredential()
            {
                _cred = SetupSharedCredential(Resources.PathwayType.KCI);
                _cred.ConsecutiveKCIPassRequired = false;
                _cred.ExamDueDate = _startingExamDueDate;
                _cred.DisplayExamDueDate = _startingExamDueDate;
                _cred.KCIExamDueDate = _startingExamDueDate;
                _cred.DisplayExamDueDate = _startingExamDueDate;
            }

            private void AndTheDueDateValuesShouldBeAsExpected()
            {
                var expectedDueDate = new DateTime(_examPassDate.Year + 2, 12, 31);

                _cred.ExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.DisplayExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.KCIExamDueDate.ShouldBeEquivalentTo(expectedDueDate);
                _cred.MOCExamDueDate.ShouldBeEquivalentTo(expectedDueDate);

            }
        }
        #endregion Scenarios
    }
}
