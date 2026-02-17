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
      AsA = "scheduled occurrence of the Year End Lookback Hangfire job",
      IWant = "to be able to enqueue instances of the YearEndLookbackChildJob",
      SoThat = "it can run the year end lookback process for a given member"
      )]
    [TestFixture]
    public class YearEndLookbackChildJobSpec
    {
        [Test]
        public void Should_Throw_OperationCanceledException_When_Cancellation_Token_Dictates()
        {
            new ShouldThrowOperationCanceledExceptionWhenCancellationTokenDictates().BDDfy();
        }

        [Test]
        public void Should_Call_ProgramRulesService_RunYearEndLookback_Method()
        {
            new ShouldCallProgramRulesServiceRunYearEndLookbackMethod().BDDfy();
        }

        #region Scenarios
        private abstract class YearEndLookbackChildScenarioBase
        {
            protected Mock<IProgramRulesService> _programRulesSvcMock;
            protected Exception _caughtException;
            protected YearEndLookbackChildJob _sut;

            protected virtual void Setup()
            {
                _programRulesSvcMock = new Mock<IProgramRulesService>(MockBehavior.Strict);
                _programRulesSvcMock
                    .Setup(x => x.RunYearEndLookback(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(true));

                _sut = new YearEndLookbackChildJob(_programRulesSvcMock.Object);
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

        private class ShouldCallProgramRulesServiceRunYearEndLookbackMethod : YearEndLookbackChildScenarioBase
        {
            private void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheRunYearEndLookbackMethodShouldHaveBeenExecuted()
            {
                _programRulesSvcMock
                    .Verify(x => x.RunYearEndLookback(It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
            }
        }

        private class ShouldThrowOperationCanceledExceptionWhenCancellationTokenDictates : YearEndLookbackChildScenarioBase
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
