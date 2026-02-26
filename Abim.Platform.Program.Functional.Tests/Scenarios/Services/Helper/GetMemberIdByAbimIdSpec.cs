using Abim.Platform.Program.MembershipClient;
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
        public async Task GetEmptyGuidWhenProfileMembershipThrowsException()
        {
            
            new GetEmptyGuidWhenProfileMembershipThrowsExceptionScenario().BDDfy();
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
            protected override void SetupProfileMembershipMock()
            {
                base.SetupProfileMembershipMock();
                ProfileResource profile = null;
                //Modify mock to return null profile
                _membershipClientServiceMock
                   .Setup(x => x.GetProfileByAbimIdAsync(It.IsAny<string>()))
                   .ReturnsAsync(profile);
            }

            //Given I have an ABIM ID is in base class
            //When I call GetMemberIdByAbimId() is in base class

            private void ThenIShouldReceiveAnEmptyGuidResult()
            {
                _result.ShouldBe(Guid.Empty);
            }

            //And no exception should have been thrown by GetMemberIdByAbimId() is in base class
        }

        private class GetEmptyGuidWhenProfileMembershipThrowsExceptionScenario : GetMemberByAbimIdScenario
        {
            protected override void SetupProfileMembershipMock()
            {
                base.SetupProfileMembershipMock();

                //Modify mock to return null profile
                _membershipClientServiceMock
                     .Setup(x => x.GetProfileByAbimIdAsync(It.IsAny<string>()))
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
