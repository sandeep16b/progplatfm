using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.WebApi.Testing.Setup;
using FluentAssertions;
using IdentityModel.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Integration.Scenarios.AccessToken
{

    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to call AccessTokenService class",
        SoThat = "to get the same access token if not expired"
        )]
    [TestFixture]
    public class AccessTokenServiceSpec
    {
        [TestCase]

        public void GetTheSameAccessTokenWithTheSecondCall()
        {
            new GetTheSameAccessTokenWithTheSecondCallSpec().BDDfy();
        }

        [TestCase]

        public void GetNewTokenWhenExistingTokenExpired()
        {
            new GetNewTokenWhenExistingTokenExpiredSpec().BDDfy();
        }

        [TestCase]

        public void InvalidScopeShouldBeHandeledProperly()
        {
            new InvalidScopeShouldBeHandeledProperlySpec().BDDfy();
        }

    }

    public abstract class AccessTokenScenario : BaseServiceScenario
    {
        protected override List<Type> AdditionalDependencies()
        {
            return new List<Type>()
            {
                typeof(ITokenClientWraper)
            };
        }
    }

    #region Scenarios

    public class GetTheSameAccessTokenWithTheSecondCallSpec : AccessTokenScenario
    {
        protected string _accessToken1 { get; set; }
        protected string _accessToken2 { get; set; }

        protected Exception _caughtException;

        protected Mock<ITokenClientWraper> _tokenClientWraperMock = null;

        protected AccessTokenService _accessTokenService { get; set; }
        protected PrivateObject _accessTokenServiceObject { get; set; }

        protected override void PreSetup()
        {
            
        }

        protected override void PostSetup()
        {
            _tokenClientWraperMock = new Mock<ITokenClientWraper>(MockBehavior.Strict);

            _tokenClientWraperMock
                .SetupSequence(o => o.RequestClientCredentialsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new TokenResponse("{\"access_token\":\"FIRST_ACCESS_TOKEN\",\"expires_in\":3600,\"token_type\":\"Bearer\"}")))
                .Returns(Task.FromResult(new TokenResponse("{\"access_token\":\"SECOND_ACCESS_TOKEN\",\"expires_in\":3600,\"token_type\":\"Bearer\"}")));

            Container.Inject(_tokenClientWraperMock.Object);

            _accessTokenService = Container.GetInstance<AccessTokenService>();
            _accessTokenServiceObject = new PrivateObject(_accessTokenService);

            // set local private static fields
            _accessTokenServiceObject.SetFieldOrProperty("_accessToken", BindingFlags.NonPublic | BindingFlags.Static, "");
            _accessTokenServiceObject.SetFieldOrProperty("expirationTokenTime", BindingFlags.NonPublic | BindingFlags.Static, DateTime.Now.AddMinutes(-1)); //
        }

        public async Task WhenICallToGetAccessTokenTwice()
        {
            try
            {
                _accessToken1 = _accessTokenService.GetAccessToken();

                _accessToken2 = _accessTokenService.GetAccessToken();
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        private void ThenNoExceptionShouldHaveOccurred()
        {
            _caughtException.ShouldBeNull();
        }

        public void AndThenIGetTheCorrectAccessToken1()
        {
            _accessToken1.Should().Be("FIRST_ACCESS_TOKEN");
        }

        public void AndThenIGetTheSameSecondAccessToken()
        {
            _accessToken2.ShouldBeSameAs(_accessToken1);
        }
    }
    
    public class GetNewTokenWhenExistingTokenExpiredSpec : AccessTokenScenario
    {
        protected string _expiredAccessToken { get; set; }
        protected string _newAccessToken2 { get; set; }

        protected Exception _caughtException;

        protected Mock<ITokenClientWraper> _tokenClientWraperMock = null;

        protected AccessTokenService _accessTokenService { get; set; }
        protected PrivateObject _accessTokenServiceObject { get; set; }

        protected override void PreSetup()
        {

        }

        protected override void PostSetup()
        {
            _tokenClientWraperMock = new Mock<ITokenClientWraper>(MockBehavior.Strict);

            _tokenClientWraperMock
                .SetupSequence(o => o.RequestClientCredentialsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new TokenResponse("{\"access_token\":\"EXPIRED_ACCESS_TOKEN\",\"expires_in\":-90,\"token_type\":\"Bearer\"}")))
                .Returns(Task.FromResult(new TokenResponse("{\"access_token\":\"NEW_ACCESS_TOKEN\",\"expires_in\":360,\"token_type\":\"Bearer\"}")));

            Container.Inject(_tokenClientWraperMock.Object);

            _accessTokenService = Container.GetInstance<AccessTokenService>();
            _accessTokenServiceObject = new PrivateObject(_accessTokenService);

            // set local private static fields
            _accessTokenServiceObject.SetFieldOrProperty("_accessToken", BindingFlags.NonPublic | BindingFlags.Static, "");
            _accessTokenServiceObject.SetFieldOrProperty("expirationTokenTime", BindingFlags.NonPublic | BindingFlags.Static, DateTime.Now.AddMinutes(-1)); //
        }

        public async Task WhenICallToGetAccessTokenTwice()
        {
            try
            {

                _expiredAccessToken = _accessTokenService.GetAccessToken();

                _newAccessToken2 = _accessTokenService.GetAccessToken();
            }
            catch (Exception ex)
            {
                _caughtException = ex;
            }
        }

        private void ThenNoExceptionShouldHaveOccurred()
        {
            _caughtException.ShouldBeNull();
        }

        public void AndThenIGetTheCorrectAccessToken1()
        {
            _expiredAccessToken.Should().Be("EXPIRED_ACCESS_TOKEN");
        }

        public void AndThenIGetTheCorrectAccessToken2()
        {
            _newAccessToken2.Should().Be("NEW_ACCESS_TOKEN");
        }

        public void ThenIGetDifferentSecondAccessTocken()
        {
            _newAccessToken2.ShouldNotBeSameAs(_expiredAccessToken);
        }
    }

    public class InvalidScopeShouldBeHandeledProperlySpec : AccessTokenScenario
    {
        protected string _accessToken;

        protected string _exceptionMessage;

        protected Mock<ITokenClientWraper> _tokenClientWraperMock = null;

        protected AccessTokenService _accessTokenService { get; set; }
        protected PrivateObject _accessTokenServiceObject { get; set; }

        protected override void PreSetup()
        {
            _tokenClientWraperMock = new Mock<ITokenClientWraper>(MockBehavior.Strict);
        }

        protected override void PostSetup()
        {
            _tokenClientWraperMock
                .Setup(o => o.RequestClientCredentialsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(new TokenResponse("{\"error\":\"invalid_scope\"}")));

            Container.Inject(_tokenClientWraperMock.Object);

            _accessTokenService = Container.GetInstance<AccessTokenService>(); 
            _accessTokenServiceObject = new PrivateObject(_accessTokenService);

            // set local private static fields
            _accessTokenServiceObject.SetFieldOrProperty("_accessToken", BindingFlags.NonPublic | BindingFlags.Static, "");
            _accessTokenServiceObject.SetFieldOrProperty("expirationTokenTime", BindingFlags.NonPublic | BindingFlags.Static, DateTime.Now.AddMinutes(-1)); //
        }

        public async Task WhenICallToGetAccessToken()
        {
            try
            {
                _accessToken = _accessTokenService.GetAccessToken();
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    _exceptionMessage = ex.InnerException.Message;
                else
                    _exceptionMessage = ex.Message;
            }
        }

        private void ThenExceptionShouldHaveOccurred()
        {
            _exceptionMessage.Should().Contain("invalid_scope");
        }

        public void ThenAccessTockenShouldBeNull()
        {
            _accessToken.ShouldBeNull(); //Value never returned due to exception
        }
    }
    
    #endregion
    
}
