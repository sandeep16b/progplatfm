using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Host.Api.Controllers;
using Abim.Platform.Program.Tests.Scenarios.Controllers.Base;
using Abim.Platform.Program.WebApi.Objects;
using Moq;
using NUnit.Framework;
using Shouldly;
using System;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Controllers.Credential
{
    [Story(
       AsA = "caller of the API",
       IWant = "to be able to use the Product Controller",
       SoThat = "to get VOC letters"
       )]
    [TestFixture]
    public class GetVocLetterSpec
    {
        //Tests added as part of fix for Bug 155729, but aren't working yet due to need to further flesh out OwinContext mock.
        //No unit tests were creating during the original coding for CredentialController.GetVocLetter method.
        /*
        [TestCase]
        [WorkItem(155729)]
        public void Should_Behave_When_User_AbimId_Is_Null()
        {
        }

        [TestCase]
        [WorkItem(155729)]
        public void Should_Deny_User_Access_When_AbimId_Is_Present_And_Doesnt_Match_abimId_Param()
        {
        }

        [TestCase]
        [WorkItem(155729)]
        public void Should_Allow_User_Access_When_AbimId_Is_Present_And_Matches_abimId_Param()
        {
            new ShouldAllowUserAccessWhenAbimIdIsPresentAndMatchesabimIdParam().BDDfy();
        }
        */

        private class GetVocLetterSpecBase : BaseControllerScenario
        {
            protected CredentialController _sut;
            protected Mock<ICredentialService> _credentialServiceMock;
            protected Mock<IEnumService> _enumServiceMock;
            protected Mock<IHelperService> _helperServiceMock;
            protected Mock<IProfileInterservice> _profileInterServiceMock;
            protected Mock<IAccessTokenService> _accessTokenServiceMock;

            protected override void Setup()
            {
                base.Setup();
                SetupMocks();
                _sut = new CredentialController(_credentialServiceMock.Object, _enumServiceMock.Object, _helperServiceMock.Object, _profileInterServiceMock.Object);
                _sut.Request = _request;
            }

            protected virtual void SetupMocks()
            {
                SetupCredentialServiceMock();
                SetupEnumServiceMock();
                SetupHelperServiceMock();
                SetupProfileInterServiceMock();
                SetupAccessTokenServiceMock();
            }

            protected virtual void SetupCredentialServiceMock()
            {
                _credentialServiceMock = new Mock<ICredentialService>(MockBehavior.Strict);
            }

            protected virtual void SetupEnumServiceMock()
            {
                _enumServiceMock = new Mock<IEnumService>(MockBehavior.Strict);
            }

            protected virtual void SetupHelperServiceMock()
            {
                _helperServiceMock = new Mock<IHelperService>(MockBehavior.Strict);
            }

            protected virtual void SetupProfileInterServiceMock()
            {
                _profileInterServiceMock = new Mock<IProfileInterservice>(MockBehavior.Strict);
            }

            protected virtual void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
            }
        }

        private class ShouldAllowUserAccessWhenAbimIdIsPresentAndMatchesabimIdParam : GetVocLetterSpecBase
        {
            public void WhenICallGetVocLetter()
            {
                try
                {
                    _sut.GetVocLetter("123456");
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBeNull();
            }
        }
    }
}
