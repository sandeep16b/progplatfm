using Abim.Enterprise.Core.Profile.Resource;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.Helper
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the HelperService",
        SoThat = "it can retrieve member IDs by ABIM IDs"
        )]
    [TestFixture]
    public class GetMemberIdByAbimIdSpec
    {
        [Test]
        [WorkItem(201366)]
        public async Task GetMemberIdWhenProfileExistsForSpecifiedABIMId()
        {
            new GetMemberIdWhenProfileExistsForABIMIdScenario().BDDfy();
        }

        [Test]
        [WorkItem(201366)]
        public async Task GetEmptyGuidWhenProfileDoesNotExistForABIMId()
        {
            new GetEmptyGuidWhenProfileDoesNotExistForABIMIdScenario().BDDfy();
        }

        [Test]
        [WorkItem(201366)]
        public async Task GetEmptyGuidWhenProfileInterserviceThrowsException()
        {
            new GetEmptyGuidWhenProfileInterserviceThrowsExceptionScenario().BDDfy();
        }

        [Test]
        [WorkItem(230643)]
        public async Task GetMemberIdByAbimId_WhenFirstAttemptWithExpiredAccessToken()
        {
            new GetMemberIdByAbimId_WhenFirstAttemptWithExpiredAccessTokenScenario().BDDfy();
        }

        [Test]
        [WorkItem(230643)]
        public async Task GetMemberIdByAbimId_WhenAfterThreeUnSuccessfullAttemptToGetTokenShouldStop()
        {
            new GetMemberIdByAbimId_WhenAfterThreeUnSuccessfullAttemptToGetTokenShouldStopScenario().BDDfy();
        }

        private class GetMemberIdWhenProfileExistsForABIMIdScenario : GetMemberByAbimIdScenario
        {
            //Given I have an ABIM ID is in base class
            //When I call GetMemberIdByAbimId() is in base class

            private void ThenIShouldReceiveANonEmptyGuidResult()
            {
                _result.ShouldBe(_profileGuid);
            }

            //And no exception should have been thrown by GetMemberIdByAbimId() is in base class
        }

        private class GetEmptyGuidWhenProfileDoesNotExistForABIMIdScenario : GetMemberByAbimIdScenario
        {
            protected override void SetupProfileInterserviceMock()
            {
                base.SetupProfileInterserviceMock();
                
                //Modify mock to return null profile
                _profileInterserviceMock
                    .Setup(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Returns(Task.FromResult<ProfileNestedResource>(null));
            }

            //Given I have an ABIM ID is in base class
            //When I call GetMemberIdByAbimId() is in base class

            private void ThenIShouldReceiveAnEmptyGuidResult()
            {
                _result.ShouldBe(Guid.Empty);
            }

            //And no exception should have been thrown by GetMemberIdByAbimId() is in base class
        }

        private class GetEmptyGuidWhenProfileInterserviceThrowsExceptionScenario : GetMemberByAbimIdScenario
        {
            protected override void SetupProfileInterserviceMock()
            {
                base.SetupProfileInterserviceMock();

                //Modify mock to return null profile
                _profileInterserviceMock
                    .Setup(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Throws(new Exception("ALL YOUR BASE ARE BELONG TO US!"));
            }

            //Given I have an ABIM ID is in base class
            //When I call GetMemberIdByAbimId() is in base class

            private void ThenIShouldReceiveAnEmptyGuidResult()
            {
                _result.ShouldBe(Guid.Empty);
            }

            //And no exception should have been thrown by GetMemberIdByAbimId() is in base class
        }

        private class GetMemberIdByAbimId_WhenFirstAttemptWithExpiredAccessTokenScenario : GetMemberByAbimIdScenario
        {
            protected override void SetupProfileInterserviceMock()
            {
                base.SetupProfileInterserviceMock();

                MemberId = Guid.NewGuid();

                _profileInterserviceMock
                    .SetupSequence(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Throws(new Exception("Unauthorized was returned"))
                    .Returns(Task.FromResult<ProfileNestedResource>(new ProfileNestedResource() { AbimId = _abimId, Id = MemberId }));
            }

            private void ThenIShouldReceiveCorrectResult()
            {
                _result.ShouldBe(MemberId);
            }

            private void AndThenAccessTokenShouldBeCalledThreeTimes()
            {
                _accessTokenServiceMock.Verify(x => x.GetAccessToken() ,Times.Exactly(3));

            }

            private void AndProfileInterserviceMockShouldBeCalledTwice ()
            {
                _profileInterserviceMock.Verify(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), _abimId), Times.Exactly(2));
            }

            protected void AndNoExceptionShouldHaveBeenThrown()
            {
                _exception.ShouldBeNull();
            }
        }

        private class GetMemberIdByAbimId_WhenAfterThreeUnSuccessfullAttemptToGetTokenShouldStopScenario : GetMemberByAbimIdScenario
        {
            protected override void SetupProfileInterserviceMock()
            {
                base.SetupProfileInterserviceMock();

                MemberId = Guid.NewGuid();

                _profileInterserviceMock
                    .SetupSequence(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                    .Throws(new Exception("Unauthorized was returned"))
                    .Throws(new Exception("Unauthorized was returned"))
                    .Throws(new Exception("Unauthorized was returned"));
            }

            private void ThenIShouldReceiveCorrectResult()
            {
                _result.ShouldBe(Guid.Empty);
            }

            private void AndThenAccessTokenShouldBeCalledSixTimes()
            {
                _accessTokenServiceMock.Verify(x => x.GetAccessToken(), Times.Exactly(6));

            }

            private void AndProfileInterserviceMockShouldBeCalledThreeTimes()
            {
                _profileInterserviceMock.Verify(x => x.GetProfileByABIMId(It.IsAny<string>(), It.IsAny<string>(), _abimId), Times.Exactly(3));
            }

            protected void AndNoExceptionShouldHaveBeenThrown()
            {
                _exception.ShouldBeNull();
            }
        }

        private abstract class GetMemberByAbimIdScenario : HelperServiceScenario
        {
            protected string _abimId;
            protected Guid _result;
            protected Guid MemberId;

            protected void GivenIHaveAnABIMId()
            {
                _abimId = "123456";
            }

            protected async void WhenICallGetMemberIdByAbimId()
            {
                try
                {
                    _result = await _sut.GetMemberIdByAbimId(_abimId);
                }
                catch (Exception ex)
                {
                    _exception = ex;
                }
            }

            protected void AndNoExceptionShouldHaveBeenThrownByGetMemberIdByAbimId()
            {
                _exception.ShouldBeNull();
            }
        }
    }
}
