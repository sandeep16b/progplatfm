using Abim.Platform.Program.App.HangFireJobs;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using Hangfire;
using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    public class EarlyYearEndLookbackChildJobSpec
    {
        [Test]
        public void Should_Call_ProgramRule_Service_Method()
        {
            new ShouldCallProgramRuleServiceMethod().BDDfy();
        }

        [Test]
        public void Should_Throw_Exception_When_Cancellation_Token_Says_Cancel()
        {
            new ShouldThrowExceptionWhenCancellationTokenSaysCancel().BDDfy();
        }

        #region Scenarios

        private abstract class EarlyYearEndLookbackChildJobSpecBase : BaseServiceScenario
        {
            protected EarlyYearEndLookbackChildJob _sut;
            protected PerformContextMock _contextMock;
            protected Mock<IJobCancellationToken> _tokenMock;
            protected Mock<IProgramRulesService> _programRulesSvcMock;
            protected Exception _caughtException;
        }

        private class ShouldCallProgramRuleServiceMethod : EarlyYearEndLookbackChildJobSpecBase
        {
            protected override void PreSetup()
            {
                //-- PerformContextMock
                _contextMock = new PerformContextMock();

                //-- jobCancellationTokenMock
                _tokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _tokenMock.Setup(x => x.ThrowIfCancellationRequested());

                // -- ProgramRulesSvcMock
                _programRulesSvcMock = new Mock<IProgramRulesService>();

                _programRulesSvcMock
                    .Setup(x => x.HandleEarlyYearEndLookbackChildJob(It.IsAny<Guid>(), It.IsAny<WindowsIntervalType>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(true));
            }

            protected override void PostSetup()
            {
                _sut = new EarlyYearEndLookbackChildJob(_programRulesSvcMock.Object);
            }

            protected virtual async Task GivenThatICallTheExecuteMethod()
            {
                try
                {
                   await _sut.ExecuteChild(Guid.NewGuid(), null,DateTime.Now,_contextMock.Object,_tokenMock.Object);
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

            private void ThenTheProgramServiceShouldBeCalled()
            {
                _programRulesSvcMock
                   .Verify(x => x.HandleEarlyYearEndLookbackChildJob(It.IsAny<Guid>(),It.IsAny<WindowsIntervalType?>(),It.IsAny<DateTime>()), Times.Once);
            }
        }

        private class ShouldThrowExceptionWhenCancellationTokenSaysCancel : EarlyYearEndLookbackChildJobSpecBase
        {
            protected override void PreSetup()
            {
                //-- PerformContextMock
                _contextMock = new PerformContextMock();

                //-- jobCancellationTokenMock
                _tokenMock = new Mock<IJobCancellationToken>(MockBehavior.Strict);
                _tokenMock.Setup(x => x.ThrowIfCancellationRequested()).Throws(new Exception());

                // -- ProgramRulesSvcMock
                _programRulesSvcMock = new Mock<IProgramRulesService>();
            }

            protected override void PostSetup()
            {
                _sut = new EarlyYearEndLookbackChildJob( _programRulesSvcMock.Object);
            }

            protected virtual async Task GivenThatICallTheExecuteMethod()
            {
                try
                {
                    await _sut.ExecuteChild(Guid.NewGuid(), null, DateTime.Now, _contextMock.Object, _tokenMock.Object);
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
