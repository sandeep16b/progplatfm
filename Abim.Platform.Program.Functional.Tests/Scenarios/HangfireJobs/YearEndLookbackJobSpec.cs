using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Abim.Platform.Program.App.HangFireJobs.Impl;
using Abim.Platform.Program.App.Services;
using FluentAssertions;
using Hangfire;
using Hangfire.Common;
using Hangfire.States;
using Moq;
using NHibernate;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    [Story(
      AsA = "scheduled Hangfire job",
      IWant = "to be able to invoke the YearEndLookbackJob",
      SoThat = "it can run the year end lookback process"
      )]
    [TestFixture]
    public class YearEndLookbackJobSpec
    {
        [Test]
        public void Should_Query_Database_For_Eligible_Member_IDs()
        {
            new ShouldQueryDatabaseForEligibleMemberIDs().BDDfy();
        }

        [Test]
        public void Should_Throw_Exception_When_Cancellation_Token_Says_Cancel()
        {
            new ShouldThrowExceptionWhenCancellationTokenSaysCancel().BDDfy();
        }

        [Test]
        public void Should_Enqueue_Child_Job_For_Each_Member_Id()
        {
            new ShouldEnqueueChildJobForEachMemberId().BDDfy();
        }

        #region Scenarios

        private abstract class YearEndLookbackJobSpecBase
        {
            protected YearEndLookbackJob _sut;
            protected Mock<IHangfireWrapper> _hangfireWrapper;
            protected PerformContextMock _contextMock;
            protected Mock<IJobCancellationToken> _tokenMock;
            protected Mock<IDbCommand> _dbCommandMock;
            protected Mock<IProgramRulesService> _programRulesSvcMock;
            protected List<Guid> _memberIds;
            protected Mock<IDataReader> _dataReaderMock;
            protected Mock<ISession> _sessionMock;
            protected Exception _caughtException;

            protected virtual void SetupMocks()
            {
                SetupBackgroundJobClientMock();
                SetupPerformContextMock();
                SetupJobCancellationTokenMock();
                SetupSessionMock();
                SetupProgramRulesServiceMock();
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

            protected virtual void SetupDbCommandMock()
            {
                _dbCommandMock = new Mock<IDbCommand>(MockBehavior.Strict);
                _dbCommandMock.SetupProperty(x => x.CommandText);
                _dbCommandMock.SetupProperty(x => x.CommandType);

                _dbCommandMock.Setup(x => x.ExecuteReader()).Returns(_dataReaderMock.Object);

                _dbCommandMock.Setup(x => x.Dispose());
            }

            protected virtual void SetupMemberIds()
            {
                _memberIds = new List<Guid>(5);
                for (var x = 0; x < 5; x++)
                    _memberIds.Add(Guid.NewGuid());
            }

            protected virtual void SetupSessionMock()
            {
                SetupDataReaderMock();
                SetupDbCommandMock();

                _sessionMock = new Mock<ISession>(MockBehavior.Strict);
                _sessionMock
                    .Setup(x => x.Connection.CreateCommand())
                    .Returns(_dbCommandMock.Object);
            }

            protected virtual void SetupDataReaderMock()
            {
                SetupMemberIds();

                _dataReaderMock = new Mock<IDataReader>(MockBehavior.Strict);
                _dataReaderMock.SetupSequence(x => x.Read())
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

                _dataReaderMock.SetupSequence(x => x[0])
                    .Returns(_memberIds[0])
                    .Returns(_memberIds[1])
                    .Returns(_memberIds[2])
                    .Returns(_memberIds[3])
                    .Returns(_memberIds[4]);

                _dataReaderMock.Setup(x => x.Dispose());
            }

            protected virtual void SetupProgramRulesServiceMock()
            {
                _programRulesSvcMock = new Mock<IProgramRulesService>(MockBehavior.Strict);
                _programRulesSvcMock
                    .Setup(x => x.RunYearEndLookback(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(true));
            }

            protected virtual void Setup()
            {
                SetupMocks();
                _sut = new YearEndLookbackJob(
                            _hangfireWrapper.Object,  
                            _programRulesSvcMock.Object, 
                            _sessionMock.Object);
            }

            protected virtual void GivenThatICallTheExecuteMethod()
            {
                try
                {
                    _sut.Execute(_contextMock.Object, _tokenMock.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }
        }

        private class ShouldQueryDatabaseForEligibleMemberIDs : YearEndLookbackJobSpecBase
        {
            private void WhenTheMethodExecutesNoExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().BeNull();
            }

            private void ThenTheMethodShouldHaveQueriedTheDatabaseForMemberIds()
            {
                _dbCommandMock.Verify(x => x.ExecuteReader(), Times.Once);
                _dataReaderMock.Verify(x => x.Read(), Times.Exactly(6));
                _dataReaderMock.Verify(x => x[0], Times.Exactly(5));
            }

            private void AndTheDatabaseObjectsShouldBeDisposed()
            {
                _dbCommandMock.Verify(x => x.Dispose(), Times.Once);
                _dataReaderMock.Verify(x => x.Dispose(), Times.Once);
            }
        }

        private class ShouldThrowExceptionWhenCancellationTokenSaysCancel : YearEndLookbackJobSpecBase
        {
            protected override void SetupJobCancellationTokenMock()
            {
                _tokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _tokenMock.Setup(x => x.ThrowIfCancellationRequested()).Throws(new Exception());
            }

            private void WhenTheMethodExecutesAnExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().NotBeNull();
            }
        }

        private class ShouldEnqueueChildJobForEachMemberId : YearEndLookbackJobSpecBase
        {
            private void WhenTheMethodExecutesNoExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheEnqueueMethodShouldHaveBeenCalledForEachMemberId()
            {
                _hangfireWrapper.Verify(x => x.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild" 
                        && job.Args[0].GetType()==typeof(Guid)
                        && job.Args[1].GetType() == typeof(DateTime)
                        && job.Args[2].GetType() == typeof(DateTime)
                        && job.Args[3] == null
                        && job.Args[4] == null),
                    It.IsAny<EnqueuedState>()),Times.Exactly(_memberIds.Count));
            }
        }

        #endregion Scenarios
    }
}
