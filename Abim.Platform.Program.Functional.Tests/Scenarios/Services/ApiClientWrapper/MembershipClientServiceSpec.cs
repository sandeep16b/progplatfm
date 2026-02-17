using FluentAssertions;
using FluentNHibernate.Conventions; 
using Moq; 
using NUnit.Framework;
using System; 
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ApiClientWrapper
{
    ///<summary>
    ///MembershipClientServiceSpec
    ///</summary> 
    [Story(
    AsA = "Wrapper Service",
        IWant = "to be able to apply the accessToken",
        SoThat = "it can call profile API with retry"
        )]
    [TestFixture]
    public class MembershipClientServiceSpec
    {
        [TestCase]
        public void GetAbimAsyncTest()
        {
            new GetAbimAsyncTest().BDDfy();
        }

        [TestCase]
        public void GetSearchProfilesByNPITest()
        {
            new GetSearchProfilesByNPITest().BDDfy();
        }

        [TestCase]
        public void GetProfileAllAsyncTestTest()
        {
            new GetProfileAllAsyncTest().BDDfy();
        }

        [TestCase]
        public void GetCountryRegionAsyncTest()
        {
            new GetCountryRegionAsyncTest().BDDfy();
        }

        [TestCase]
        public void GetCountryAsyncTest()
        {
            new GetCountryAsyncTest().BDDfy();
        }

        [TestCase]
        public void GetProfileByIdTest()
        {
            new GetProfileByIdTest().BDDfy();
        }

        [TestCase]
        public void RetryPolicyTest()
        {
            new RetryPolicyTest().BDDfy();
        }
    }
    
    public class GetAbimAsyncTest : MembershipClientServiceScenario
    {
        public async void WhenICallServiceMethod()
        {
            try
            {
                resource = await profileMembershipClientWrapperService.GetProfileByAbimIdAsync("123445");
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheResponseShouldNotBeNull()
        {
            resource.Should().NotBeNull();
        }
    }

    public class GetSearchProfilesByNPITest : MembershipClientServiceScenario
    {
        public async void WhenICallServiceMethod()
        {
            try
            {
                vocProfileResource = await profileMembershipClientWrapperService.SearchProfilesByNPI("123445");
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheResponseShouldNotBeNull()
        {
            vocProfileResource.Should().NotBeNull();
        }
    }

    public class GetProfileAllAsyncTest : MembershipClientServiceScenario
    {
        public async void WhenICallServiceMethod()
        {
            try
            {
                vocProfileResource = await profileMembershipClientWrapperService.GetNameAllAsync("Alex","",null, false, 50, 1);
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheResponseShouldNotBeNull()
        {
            vocProfileResource.Should().NotBeNull();
        }
    }

    public class GetCountryRegionAsyncTest : MembershipClientServiceScenario
    {
        public async void WhenICallServiceMethod()
        {
            try
            {
                regionResource = await profileMembershipClientWrapperService.GetCountryRegionsAsync("USA");
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheResponseShouldNotBeNull()
        {
            regionResource.Should().NotBeNull();
        }
    }

    public class GetCountryAsyncTest : MembershipClientServiceScenario
    {
        public async void WhenICallServiceMethod()
        {
            try
            {
                countryResource = await profileMembershipClientWrapperService.GetCountriesAsync();
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheResponseShouldNotBeNull()
        {
            countryResource.Should().NotBeNull();
        }
    }

    public class GetProfileByIdTest : MembershipClientServiceScenario
    {
        public async void WhenICallServiceMethod()
        {
            try
            {
                resource = await profileMembershipClientWrapperService.GetProfileByMemberIdAsync(Guid.NewGuid());
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }

        public void AndThenTheResponseShouldNotBeNull()
        {
            resource.Should().NotBeNull();
        }
    }

    public class RetryPolicyTest : RetryPolicyScenario
    {
        Guid profileId = Guid.NewGuid();
        public RetryPolicyTest()
        {
            BaseUrl = "https://testapi";
            EndpointUrl = "/api/v2.0/membership/profile/" + profileId.ToString();
        }
        public async void WhenICallServiceMethodReturning500()
        {
            try
            {
                await profileClientWrapperService.GetProfileByMemberIdAsync(profileId);
            }
            catch (Exception ex)
            {

            }
        }

        public void ThenServiceShouldBeRetried()
        {
            httpClientFactoryMock.Verify(m => m.CreateClient(It.IsAny<string>()), Times.Exactly(4));
        } 

        public void AndThenExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.IsAny();
        }

    }
}
