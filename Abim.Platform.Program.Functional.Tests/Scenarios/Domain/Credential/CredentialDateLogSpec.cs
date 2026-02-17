using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Domain.Credential
{
    [Story(
        AsA = "process creating a domain object credential",
        IWant = "to be ensured that my domain object is valid",
        SoThat = "so that I can safely use this domain object to save to DB"
        )]
    [TestFixture]
    public class CredentialDateLogSpec
    {
        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_AssessmentMetDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenAssessmentMetDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_ExamDueDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenExamDueDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_DisplayExamDueDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenDisplayExamDueDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_MOCExamDueDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenMOCExamDueDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_KCIExamDueDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenKCIExamDueDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_GracePeriodStartDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenGracePeriodStartDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Add_CredentialDateLog_When_GracePeriodEndDate_Changes()
        {
            new ShouldAddCredentialDateLogWhenGracePeriodEndDateChanges().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_AssessmentMetDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenAssessmentMetDateIsSetWithSameValue().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_ExamDueDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenExamDueDateIsSetWithSameValue().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_DisplayExamDueDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenDisplayExamDueDateIsSetWithSameValue().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_MOCExamDueDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenMOCExamDueDateIsSetWithSameValue().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_KCIExamDueDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenKCIExamDueDateIsSetWithSameValue().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_GracePeriodStartDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenGracePeriodStartDateIsSetWithSameValue().BDDfy();
        }

        [Test]
        [WorkItem(136585)]
        public void Should_Not_Add_CredentialDateLog_When_GracePeriodEndDate_Is_Set_With_Same_Value()
        {
            new ShouldNotAddCredentialDateLogWhenGracePeriodEndDateIsSetWithSameValue().BDDfy();
        }

        #region Scenarios

        #region Base Classes
        private abstract class CredentialDateLogScenario
        {
            protected App.Domain.Credential _sut;
            protected DateTime _testDate = new DateTime(2018, 11, 29);
            protected CredentialDateType _dateType;

            protected void GivenIHaveACredential()
            {
                _sut = App.Domain.Credential.Create(
                    null, 
                    Guid.NewGuid(), 
                    CredentialType.General, 
                    PathwayType.MOC,
                    null, null,
                    "Unit Test");
            }

            protected void ThenANewCredentialDateLogObjectShouldHaveBeenCreatedWithTheCorrectValues()
            {
                _sut.DateLogs.Count.Should().Be(1);
                _sut.DateLogs[0].DateType.Should().Be(_dateType);
                _sut.DateLogs[0].ChangedDate.Should().BeAfter(DateTime.Now.AddDays(-1));
                _sut.DateLogs[0].OldValue.HasValue.Should().BeFalse();
                _sut.DateLogs[0].NewValue.Should().Be(_testDate);
                _sut.DateLogs[0].AuditData.CreatedBy.Should().Be("ValueChange");
            }
        }

        #endregion Base Classes


        private class ShouldAddCredentialDateLogWhenAssessmentMetDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeAssessmentMetDate()
            {
                _dateType = CredentialDateType.AssessmentMetDate;
                _sut.AssessmentMetDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenAssessmentMetDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenAssessmentMetDateChanges
        {
            protected void AndWhenISetAssessmentDateAgainUsingTheSameValue()
            {
                _sut.AssessmentMetDate = _testDate;
            }
        }

        private class ShouldAddCredentialDateLogWhenExamDueDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeExamDueDate()
            {
                _dateType = CredentialDateType.ExamDueDate;
                _sut.ExamDueDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenExamDueDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenExamDueDateChanges
        {
            protected void AndWhenISetExamDueDateAgainUsingTheSameValue()
            {
                _sut.ExamDueDate = _testDate;
            }
        }


        private class ShouldAddCredentialDateLogWhenDisplayExamDueDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeDisplayExamDueDate()
            {
                _dateType = CredentialDateType.DisplayExamDueDate;
                _sut.DisplayExamDueDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenDisplayExamDueDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenDisplayExamDueDateChanges
        {
            protected void AndWhenISetDisplayExamDueDateAgainUsingTheSameValue()
            {
                _sut.DisplayExamDueDate = _testDate;
            }
        }

        private class ShouldAddCredentialDateLogWhenMOCExamDueDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeMOCExamDueDate()
            {
                _dateType = CredentialDateType.MOCExamDueDate;
                _sut.MOCExamDueDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenMOCExamDueDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenMOCExamDueDateChanges
        {
            protected void AndWhenISetMOCExamDueDateAgainUsingTheSameValue()
            {
                _sut.MOCExamDueDate = _testDate;
            }
        }

        private class ShouldAddCredentialDateLogWhenKCIExamDueDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeKCIExamDueDate()
            {
                _dateType = CredentialDateType.KCIExamDueDate;
                _sut.KCIExamDueDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenKCIExamDueDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenKCIExamDueDateChanges
        {
            protected void AndWhenISetKCIExamDueDateAgainUsingTheSameValue()
            {
                _sut.KCIExamDueDate = _testDate;
            }
        }

        private class ShouldAddCredentialDateLogWhenGracePeriodStartDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeGracePeriodStartDate()
            {
                _dateType = CredentialDateType.GracePeriodStartDate;
                _sut.GracePeriodStartDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenGracePeriodStartDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenGracePeriodStartDateChanges
        {
            protected void AndWhenISetGracePeriodStartDateAgainUsingTheSameValue()
            {
                _sut.GracePeriodStartDate = _testDate;
            }
        }

        private class ShouldAddCredentialDateLogWhenGracePeriodEndDateChanges : CredentialDateLogScenario
        {
            protected void WhenIChangeGracePeriodEndDate()
            {
                _dateType = CredentialDateType.GracePeriodEndDate;
                _sut.GracePeriodEndDate = _testDate;
            }
        }

        private class ShouldNotAddCredentialDateLogWhenGracePeriodEndDateIsSetWithSameValue : ShouldAddCredentialDateLogWhenGracePeriodEndDateChanges
        {
            protected void AndWhenISetGracePeriodEndDateAgainUsingTheSameValue()
            {
                _sut.GracePeriodEndDate = _testDate;
            }
        }

        #endregion Scenarios
    }
}
