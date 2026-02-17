using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can retrieve the latest lookback date for a member"
    )]
    [TestFixture]
    public class GetLatestLookbackDateSpec
    {
        [Test]
        public void Should_Return_Latest_LookbackDate_For_Member()
        {
            new ShouldReturnLatestLookbackDateForMember().BDDfy();
        }

        #region Scenarios

        private class ShouldReturnLatestLookbackDateForMember : CredentialServiceSimplifiedScenario
        {
            private DateTime? _expectedResult = new DateTime(2018, 12, 31);
            private DateTime? _actualResult;
            private Guid _memberId;

            protected override void SetupCredentialRepositoryMock()
            {
                base.SetupCredentialRepositoryMock();
                _credRepoMock
                    .Setup(x => x.GetLatestLookbackDate(It.IsAny<Guid>()))
                    .Returns(_expectedResult);
            }

            private void GivenIHaveAMemberId()
            {
                _memberId = Guid.NewGuid();
            }

            private void WhenICallGetLatestLookbackDate()
            {
                try
                {
                    _actualResult = _sut.GetLatestLookbackDate(_memberId);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.Should().BeNull();
            }

            private void AndTheRepositoryShouldHaveBeenCalled()
            {
                _credRepoMock.Verify(x => x.GetLatestLookbackDate(_memberId), Times.Once);
            }

            private void AndResultShouldBeAsExpected()
            {
                _actualResult.Should().Be(_expectedResult);
            }
        }

        #endregion Scenarios
    }
}
