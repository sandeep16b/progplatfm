using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestStack.BDDfy;
using NUnit;
using NUnit.Framework;
using Moq;
using FluentAssertions;
using Hangfire;
using Hangfire.Server;
using System.Data;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.HangFireJobs.Impl;
using System.Linq.Expressions;
using Hangfire.Common;
using Hangfire.States;
using Abim.Platform.Program.App.HangFireJobs.Helpers;
using NHibernate;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    [Story(
      AsA = "scheduled Hangfire job",
      IWant = "to be able to invoke the CoSponsoredLockOutTestJob",
      SoThat = "it can perform test runs of the CoSponsored Lock Out Test process"
      )]
    [TestFixture]
    public class CoSponsoredLockOutTestJobSpec
    {
        [Test]
        public void Should_Query_Database_For_Eligible_Credential_IDs()
        {
            new ShouldQueryDatabaseForEligibleCredentialIDs().BDDfy();
        }

        [Test]
        public void Should_Throw_Exception_When_Cancellation_Token_Says_Cancel()
        {
            new ShouldThrowExceptionWhenCancellationTokenSaysCancel().BDDfy();
        }

        [Test]
        public void Should_Enqueue_Child_Job_For_Each_Credential_Id()
        {
            new ShouldEnqueueChildJobForEachCredentialId().BDDfy();
        }

        #region Scenarios

        private abstract class CoSponsoredLockOutTestJobSpecBase
        {
            protected CoSponsoredLockOutTestJob _sut;
            protected Mock<IHangfireWrapper> _hangfireWrapper;
            protected PerformContextMock _contextMock;
            protected Mock<IJobCancellationToken> _tokenMock;
            protected Mock<IDbCommand> _dbCommandMock;
            protected Mock<IProgramRulesService> _programRulesSvcMock;
            protected List<Guid> _credentialIds;
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

            protected virtual void SetupCredentialIds()
            {
                _credentialIds = new List<Guid>(5);
                for (var x = 0; x < 5; x++)
                    _credentialIds.Add(Guid.NewGuid());
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
                SetupCredentialIds();

                _dataReaderMock = new Mock<IDataReader>(MockBehavior.Strict);
                _dataReaderMock.SetupSequence(x => x.Read())
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(true)
                    .Returns(false);

                _dataReaderMock.SetupSequence(x => x[0])
                    .Returns(_credentialIds[0])
                    .Returns(_credentialIds[1])
                    .Returns(_credentialIds[2])
                    .Returns(_credentialIds[3])
                    .Returns(_credentialIds[4]);

                _dataReaderMock.Setup(x => x.Dispose());
            }

            protected virtual void SetupProgramRulesServiceMock()
            {
                _programRulesSvcMock = new Mock<IProgramRulesService>(MockBehavior.Strict);
                _programRulesSvcMock
                    .Setup(x => x.RunCoSponsoredLockOut(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(true));
            }

            protected virtual void Setup()
            {
                SetupMocks();
                _sut = new CoSponsoredLockOutTestJob(
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

        private class ShouldQueryDatabaseForEligibleCredentialIDs : CoSponsoredLockOutTestJobSpecBase
        {
            private void WhenTheMethodExecutesNoExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().BeNull();
            }

            private void ThenTheMethodShouldHaveQueriedTheDatabaseForCredentialIds()
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

        private class ShouldThrowExceptionWhenCancellationTokenSaysCancel : CoSponsoredLockOutTestJobSpecBase
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

        private class ShouldEnqueueChildJobForEachCredentialId : CoSponsoredLockOutTestJobSpecBase
        {
            private void WhenTheMethodExecutesNoExceptionShouldHaveBeenCaught()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheEnqueueMethodShouldHaveBeenCalledForEachCredentialId()
            {
                _hangfireWrapper.Verify(x => x.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild"
                        && job.Args[0].GetType() == typeof(Guid)
                        && job.Args[1].GetType() == typeof(DateTime)
                        && job.Args[2].GetType() == typeof(DateTime)
                        && job.Args[3] == null
                        && job.Args[4] == null),
                    It.IsAny<EnqueuedState>()), Times.Exactly(_credentialIds.Count));
            }
        }

        #endregion Scenarios
    }
}
