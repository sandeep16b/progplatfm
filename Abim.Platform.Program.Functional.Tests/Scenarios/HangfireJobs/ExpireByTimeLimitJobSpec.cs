using System;
using System.Collections.Generic;
using TestStack.BDDfy;
using NUnit.Framework;
using Moq;
using Hangfire;
using Abim.Platform.Program.App.Services;
using Hangfire.Common;
using Hangfire.States;
using Abim.Platform.Program.App.HangFireJobs.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abim.Platform.Program.App.HangFireJobs;
using Shouldly;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    [Story(
      AsA = "scheduled Hangfire job",
      IWant = "to be able to invoke the ExpireByTimeLimitJob",
      SoThat = "it can create new issuances when applicable"
      )]
    [TestFixture]
    public class ExpireByTimeLimitJobSpec
    {
        [Test]
        [WorkItem(185606)]
        public void Should_Only_Process_ABIM_Issued_Credentials()
        {
            new ShouldOnlyProcessABIMIssuedCredentialsScenario().BDDfy();
        }

        #region Scenario Base Class
        private abstract class ExpireByTimeLimitJobScenarioBase
        {
            protected ExpireByTimeLimitJob _sut;
            protected Mock<IHangfireWrapper> _hangfireWrapper;
            protected PerformContextMock _contextMock;
            protected Mock<IJobCancellationToken> _tokenMock;
            protected Mock<IProgramRulesService> _programRulesServiceMock;
            protected Mock<ICredentialService> _credentialServiceMock;
            protected int? _limitedBatchSize;
            protected Exception _caughtException;

            protected virtual void SetupMocks()
            {
                SetupBackgroundJobClientMock();
                SetupPerformContextMock();
                SetupJobCancellationTokenMock();
                SetupProgramRulesServiceMock();
                SetupCredentialServiceMock();
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

            protected virtual void SetupProgramRulesServiceMock()
            {
                //The job requires this as a dependency but doesn't seem to actually use it
                _programRulesServiceMock = new Mock<IProgramRulesService>(MockBehavior.Strict);
            }

            protected virtual void SetupCredentialServiceMock()
            {
                const int numberOfFakeResults = 5;
                var fakeResults = new List<Tuple<Guid, int>>(numberOfFakeResults);
                for (int x = 0; x < numberOfFakeResults; x++)
                {
                    fakeResults.Add(new Tuple<Guid, int>(Guid.NewGuid(), x));
                }

                _credentialServiceMock = new Mock<ICredentialService>(MockBehavior.Strict);

                _credentialServiceMock
                    .Setup(mock => mock.GetExpiredCredentials(It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<bool>()))
                    .Returns(fakeResults);
            }

            protected virtual void Setup()
            {
                SetupMocks();
                _sut = new ExpireByTimeLimitJob(
                            _hangfireWrapper.Object,
                            _programRulesServiceMock.Object,
                            _credentialServiceMock.Object);
            }

            protected virtual void GivenThatIWantToProcessAllExpiringCredentials()
            {
                _limitedBatchSize = null;
            }

            protected virtual void WhenICallTheExecuteMethod()
            {
                try
                {
                    _sut.Execute(_contextMock.Object, _tokenMock.Object, _limitedBatchSize);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected virtual void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBeNull();
            }
        }
        #endregion Scenario Base Class


        #region Scenarios
        private class ShouldOnlyProcessABIMIssuedCredentialsScenario : ExpireByTimeLimitJobScenarioBase
        {
            public void AndOnlyABIMIssuedCredentialsShouldHaveBeenRetrieved()
            {
                _credentialServiceMock
                    .Verify(mock => mock.GetExpiredCredentials(It.IsAny<DateTime>(), It.IsAny<DateTime>(), true), 
                    Times.Once);
            }
        }
        #endregion Scenarios
    }
}
