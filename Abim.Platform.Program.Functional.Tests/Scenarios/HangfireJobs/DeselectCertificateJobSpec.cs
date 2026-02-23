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
using Abim.Platform.Program.App.HangFireJobs;
using Shouldly;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Abim.Platform.Program.App.DTOs;
using System.Linq;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    [Story(
     AsA = "scheduled Hangfire job",
     IWant = "to be able to invoke the DeselectCertificateJob",
     SoThat = "it can deselect any credentials flagged for deselection"
    )]
    [TestFixture]
    public class DeselectCertificateJobSpec
    {
        [Test]
        [WorkItem(187664)]
        public void ShouldExecuteSuccessfully()
        {
            new DeselectCertificateJobShouldExecuteSuccessfully().BDDfy();
        }

        [Test]
        [WorkItem(187664)]
        public void ShouldThrowExceptionWhenCancellationTokenSaysCancel()
        {
            new DeselectCertificateJobShouldCancelRequested().BDDfy();
        }


        #region Scenarios

        #region Base Classes
        private abstract class DeselectCertificateJobScenarioBase
        {
            protected DeselectCertificateJob _sut;
            protected Mock<IHangfireWrapper> _hangfireWrapper;
            protected PerformContextMock _contextMock;
            protected IEnumerable<IGrouping<Guid, CredentialInfoDTO>> _credentialsInfo;
            protected Mock<IJobCancellationToken> _cancellationTokenMock;
            protected Mock<ICredentialService> _credentialServiceMock;
            protected Exception _caughtException = null;
            protected readonly Guid IM_GUID = Guid.Parse("1771AD17-9920-E711-8101-005056AB0204");
            protected readonly Guid CARD_GUID = Guid.Parse("0571AD17-9920-E711-8101-005056AB0198");
            protected readonly Guid DIPLOMATE_1_GUID = Guid.NewGuid();
            protected readonly Guid DIPLOMATE_2_GUID = Guid.NewGuid();


            protected virtual void SetupMocks()
            {
                SetupBackgroundJobClientMock();
                SetupPerformContextMock();
                SetupJobCancellationTokenMock();
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
                _cancellationTokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _cancellationTokenMock.Setup(x => x.ThrowIfCancellationRequested());
            }

            protected virtual void SetupCredentialServiceMock()
            {
                List<Tuple<Guid, Guid, string>> credInfo = 
                    new List<Tuple<Guid, Guid, string>>(3)
                    { 
                        new Tuple<Guid, Guid, string>(DIPLOMATE_1_GUID, IM_GUID, "Internal Medicine"), 
                        new Tuple<Guid, Guid, string>(DIPLOMATE_2_GUID, CARD_GUID, "Cardiovascular Disease"), 
                        new Tuple<Guid, Guid, string>(DIPLOMATE_2_GUID, IM_GUID, "Internal Medicine")
                    };

                _credentialsInfo = 
                    credInfo.GroupBy(
                        key => key.Item1, 
                        values => new CredentialInfoDTO { ExternalId = values.Item2, CertificateName = values.Item3 });

                _credentialServiceMock = new Mock<ICredentialService>(MockBehavior.Strict);
                _credentialServiceMock
                    .Setup(x => x.GetInfoOfCredentialsMarkedForDeselection(It.IsAny<DateTime>()))
                    .Returns(_credentialsInfo);
            }

            protected virtual void Setup()
            {
                SetupMocks();
                _sut = 
                    new DeselectCertificateJob(
                        _credentialServiceMock.Object,
                        _hangfireWrapper.Object);
            }

            public void WhenTheJobIsExecuted()
            {
                try
                {
                    _sut.Execute(_contextMock.Object, _cancellationTokenMock.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }
        }
        #endregion Base Classes

        #region Should Query Service for Eligible Credential IDs Scenario
        private class DeselectCertificateJobShouldExecuteSuccessfully : DeselectCertificateJobScenarioBase
        {
            //Setup and execution steps take place in the base class

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.ShouldBeNull();
            }

            public void AndTheCredentialServiceShouldHaveBeenQueriedForCredentialIds()
            {
                _credentialServiceMock.Verify(mock => mock.GetInfoOfCredentialsMarkedForDeselection(new DateTime(DateTime.Now.Year, 2, 1)), Times.Once);
            }

            public void AndAChildJobShouldHaveBeenEnqueuedForEachMemberId()
            {
                _hangfireWrapper.Verify(mock => mock.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild"
                        && job.Args[0].GetType() == typeof(Guid)
                        && job.Args[1].GetType() == typeof(List<CredentialInfoDTO>)
                        && job.Args[2].GetType() == typeof(DateTime)
                        && job.Args[3] == null
                        && job.Args[4] == null),
                    It.IsAny<EnqueuedState>()), Times.Exactly(_credentialsInfo.Count()));
            }
        }
        #endregion Should Query Service for Eligible Credential IDs Scenario

        #region Should Throw Cancellation Error if Requested
        private class DeselectCertificateJobShouldCancelRequested : DeselectCertificateJobScenarioBase
        {
            //Setup and execution steps take place in the base class
            protected override void SetupJobCancellationTokenMock()
            {
                _cancellationTokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _cancellationTokenMock
                    .Setup(x => x.ThrowIfCancellationRequested())
                    .Throws(new OperationCanceledException());
            }

            public void ThenOperationCanceledExceptionShouldHaveBeenThrown()
            {
                _caughtException.ShouldNotBeNull();
                _caughtException.ShouldBeOfType<OperationCanceledException>();
            }

            public void AndTheCredentialServiceShouldNotHaveBeenQueriedForCredentialIds()
            {
                _credentialServiceMock.Verify(
                    mock => 
                        mock.GetInfoOfCredentialsMarkedForDeselection(new DateTime(DateTime.Now.Year, 2, 1)), 
                        Times.Never);
            }

            public void AndNoChildJobShouldHaveBeenEnqueued()
            {
                _hangfireWrapper.Verify(mock => mock.BackgroundJobClient.Create(
                    It.Is<Job>(job => job.Method.Name == "ExecuteChild"
                        && job.Args[0].GetType() == typeof(Guid)
                        && job.Args[1].GetType() == typeof(DateTime)
                        && job.Args[2] == null
                        && job.Args[3] == null),
                    It.IsAny<EnqueuedState>()), Times.Never);
            }
        }
        #endregion Should Throw Cancellation Error if Requested

        #endregion Scenarios
    }
}
