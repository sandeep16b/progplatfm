using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.HangFireJobs;
using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    public class EarlyYearEndLookbackJobSpec
    {
        [Test]
        public void Should_Find_NoExpired_LookBackDatesInfo()
        {
            new ShouldFindNoExpiredLookBackDatesInfo().BDDfy();
        }

        [Test]
        public void Should_Enqueue_ChildJob_ForEachExpiringLookBack()
        {
            new ShouldEnqueueChildJobForEachExpiringLookBack().BDDfy();
        }

        [Test]
        public void Should_Throw_Exception_When_Cancellation_Token_Says_Cancel()
        {
            new ShouldThrowExceptionWhenCancellationTokenSaysCancel().BDDfy();
        }

        #region Scenarios

        private abstract class EarlyYearEndLookbackJobSpecBase : BaseServiceScenario
        {
            protected EarlyYearEndLookbackJob _sut;
            protected Mock<IHangfireWrapper> _hangfireWrapper;
            protected PerformContextMock _contextMock;
            protected Mock<IJobCancellationToken> _tokenMock;
            protected Mock<ILookBackDatesInfoService> _lookBackDatesInfoServiceMock;
            IEnumerable<LookBackDatesInfo> lookbackDates;
            protected Exception _caughtException;

            protected virtual void SetupCommonTestsMocks()
            {
                SetupBackgroundJobClientMock();
                SetupPerformContextMock();
                SetupJobCancellationTokenMock();
            }

            protected virtual void SetupBackgroundJobClientMock()
            {
                _hangfireWrapper = new Mock<IHangfireWrapper>(MockBehavior.Strict);

                _hangfireWrapper.Setup(x => x.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild"),
                    It.IsAny<EnqueuedState>()))
                    .Returns("Unique identifier of a created background job -or- null, if it was not created.");
            }

            protected virtual void SetupPerformContextMock()
            {
                _contextMock = new PerformContextMock();
            }

            protected virtual void SetupJobCancellationTokenMock()
            {
                _tokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _tokenMock.Setup(x => x.ThrowIfCancellationRequested());
            }

            protected virtual void SetupJobCancellationTokenMockWithException()
            {
                _tokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _tokenMock.Setup(x => x.ThrowIfCancellationRequested()).Throws(new Exception());
            }

            protected virtual void SetupLookBackDatesInfoServiceMock(int recordCount = 0)
            {
                _lookBackDatesInfoServiceMock = new Mock<ILookBackDatesInfoService>(MockBehavior.Strict);

                if (recordCount > 0)
                {
                    Mock<LookBackDatesInfo> lookBack = new Mock<LookBackDatesInfo>();

                    lookBack.Object.Lookback2YearStartDate = new DateTime(2014, 01, 01);
                    lookBack.Object.Lookback2YearEndDate = new DateTime(2015, 12, 31);
                    lookBack.Object.Lookback5YearStartDate = new DateTime(2014, 01, 01);
                    lookBack.Object.Lookback5YearEndDate = new DateTime(2018, 12, 31);
                    lookBack.Object.ExternalId = new Guid();
                    lookBack.Object.AuditData = AuditData.Create("test");

                    List<LookBackDatesInfo> listLookbackDates = new List<LookBackDatesInfo>();
                    for (int i = 0; i < recordCount; i++)
                        listLookbackDates.Add(lookBack.Object);

                    lookbackDates = listLookbackDates.AsEnumerable();
                }
                else
                {
                    lookbackDates = new List<LookBackDatesInfo>();
                }

                _lookBackDatesInfoServiceMock
                    .Setup(x => x.GetExpiredLookBackDatesInfo(It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(lookbackDates));
            }
        }

        private class ShouldFindNoExpiredLookBackDatesInfo : EarlyYearEndLookbackJobSpecBase
        {
            protected override void PreSetup()
            {
                SetupCommonTestsMocks();
                SetupLookBackDatesInfoServiceMock();
            }

            protected override void PostSetup()
            {
                _sut = new EarlyYearEndLookbackJob(
                            _lookBackDatesInfoServiceMock.Object,
                            _hangfireWrapper.Object);
            }

            protected virtual void GivenThatICallTheExecuteMethod()
            {
                try
                {
                    _sut.Execute(_contextMock.Object, _tokenMock.Object, null, null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void WhenTheMethodExecutesNoExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().BeNull();
            }

            private void ThenTheEnqueueShouldNeverBeCalled()
            {
                _hangfireWrapper.Verify(x => x.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild"
                        && job.Args[0].GetType() == typeof(WindowsIntervalType)
                        && job.Args[1].GetType() == typeof(DateTime)
                        && job.Args[2] == null
                        && job.Args[3] == null),
                    It.IsAny<EnqueuedState>()), Times.Never);
            }
        }

        private class ShouldEnqueueChildJobForEachExpiringLookBack : EarlyYearEndLookbackJobSpecBase
        {
            int records = 5;

            protected override void PreSetup()
            {
                SetupCommonTestsMocks();
                SetupLookBackDatesInfoServiceMock(records);
            }

            protected override void PostSetup()
            {
                _sut = new EarlyYearEndLookbackJob(
                            _lookBackDatesInfoServiceMock.Object,
                            _hangfireWrapper.Object);
            }

            protected virtual void GivenThatICallTheExecuteMethod()
            {
                try
                {
                    _sut.Execute(_contextMock.Object, _tokenMock.Object, null, null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void WhenTheMethodExecutesNoExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().BeNull();
            }

            private void ThenTheMethoudShouldHaveEnqueuedRunLookbackForEachMemberId()
            {

                _hangfireWrapper.Verify(x => x.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild"
                        && job.Args[0].GetType() == typeof(Guid)
                        && job.Args[2].GetType() == typeof(DateTime)
                        && job.Args[3] == null
                        && job.Args[4] == null),
                    It.IsAny<EnqueuedState>()), Times.Exactly(records));
            }
        }

        private class ShouldThrowExceptionWhenCancellationTokenSaysCancel : EarlyYearEndLookbackJobSpecBase
        {
            protected override void PreSetup()
            {
                SetupCommonTestsMocks();
                SetupLookBackDatesInfoServiceMock();
                SetupJobCancellationTokenMockWithException();
            }

            protected override void PostSetup()
            {
                _sut = new EarlyYearEndLookbackJob(
                            _lookBackDatesInfoServiceMock.Object,
                            _hangfireWrapper.Object);
            }

            protected virtual void GivenThatICallTheExecuteMethod()
            {
                try
                {
                    _sut.Execute(_contextMock.Object, _tokenMock.Object, null, null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void WhenTheMethodExecutesAnExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().NotBeNull();
            }
        }

        #endregion Scenarios
    }
}
