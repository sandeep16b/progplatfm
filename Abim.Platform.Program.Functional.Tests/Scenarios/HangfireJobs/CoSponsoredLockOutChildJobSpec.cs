using Abim.Platform.Program.App.HangFireJobs.Impl;
using Abim.Platform.Program.App.Services;
using FluentAssertions;
using Hangfire;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    [Story(
      AsA = "scheduled occurrence of the CoSponsored Lock Out Hangfire job",
      IWant = "to be able to enqueue instances of the CoSponsoredLockOutChildJob",
      SoThat = "it can run the CoSponsored Lock Out process for a given member"
      )]
    [TestFixture]
    public class CoSponsoredLockOutChildJobSpec
    {
        [Test]
        public void Should_Throw_OperationCanceledException_When_Cancellation_Token_Dictates()
        {
            new ShouldThrowOperationCanceledExceptionWhenCancellationTokenDictates().BDDfy();
        }

        [Test]
        public void Should_Call_ProgramRulesService_RunCoSponsoredLockOut_Method()
        {
            new ShouldCallProgramRulesServiceRunCoSponsoredLockOutMethod().BDDfy();
        }

        #region Scenarios
        private abstract class CoSponsoredLockOutChildScenarioBase
        {
            protected Mock<IProgramRulesService> _programRulesSvcMock;
            protected Exception _caughtException;
            protected CoSponsoredLockOutChildJob _sut;

            protected virtual void Setup()
            {
                _programRulesSvcMock = new Mock<IProgramRulesService>(MockBehavior.Strict);
                _programRulesSvcMock
                    .Setup(x => x.RunCoSponsoredLockOut(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(true));

                _sut = new CoSponsoredLockOutChildJob(_programRulesSvcMock.Object);
            }

            protected async virtual void GivenThatICallExecuteChild()
            {
                try
                {
                    await _sut.ExecuteChild(Guid.NewGuid(), DateTime.Now, DateTime.Now, null, null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }
        }

        private class ShouldCallProgramRulesServiceRunCoSponsoredLockOutMethod : CoSponsoredLockOutChildScenarioBase
        {
            private void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheRunCoSponsoredLockOutMethodShouldHaveBeenExecuted()
            {
                _programRulesSvcMock
                    .Verify(x => x.RunCoSponsoredLockOut(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            }
        }

        private class ShouldThrowOperationCanceledExceptionWhenCancellationTokenDictates : CoSponsoredLockOutChildScenarioBase
        {
            Mock<IJobCancellationToken> _cancellationToken = new Mock<IJobCancellationToken>(MockBehavior.Strict);

            protected override void Setup()
            {
                _cancellationToken.Setup(x => x.ThrowIfCancellationRequested()).Throws(new OperationCanceledException());
                base.Setup();
            }

            protected async override void GivenThatICallExecuteChild()
            {
                try
                {
                    await _sut.ExecuteChild(Guid.NewGuid(), DateTime.Now, DateTime.Now, null, _cancellationToken.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenExceptionShouldHaveOccurred()
            {
                _caughtException.Should().NotBeNull();
            }

            private void AndTheExceptionShouldBeAnOperationCancelledException()
            {
                _caughtException.Should().BeOfType<OperationCanceledException>();
            }
        }

        #endregion Scenarios
    }
}
