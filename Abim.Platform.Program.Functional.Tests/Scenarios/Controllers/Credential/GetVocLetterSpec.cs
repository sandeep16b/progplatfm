using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Host.Api.Controllers;
using Abim.Platform.Program.MembershipClient;
using Abim.Platform.Program.Tests.Scenarios.Controllers.Base;
using Abim.Platform.Program.WebApi.Objects;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Shouldly;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Controllers.Credential
{
    [Story(
       AsA = "caller of the API",
       IWant = "to be able to use the Credential Controller",
       SoThat = "to get VOC letters"
       )]
    [TestFixture]
    public class GetVocLetterSpec
    {
        //Tests added as part of fix for Bug 155729, but aren't working yet due to need to further flesh out OwinContext mock.
        //No unit tests were creating during the original coding for CredentialController.GetVocLetter method.

        [TestCase]
        public void Should_Return_Forbidden_When_User_AbimId_Is_Null()
        {
            new ShouldReturnForbiddenWhenAbimIdIsNull().BDDfy();
        }

        [TestCase]
        public void Should_Deny_User_Access_When_AbimId_Is_Present_And_Doesnt_Match_abimId_Param()
        {
            new ShouldDenyUserAccessWhenAbimIdIsPresentAndDoesntMatchabimIdParam().BDDfy();
        }

        [TestCase]
        public void Should_Allow_User_Access_When_AbimId_Is_Present_And_Matches_abimId_Param()
        {
            new ShouldAllowUserAccessWhenAbimIdIsPresentAndMatchesabimIdParam().BDDfy();
        }

        [TestCase]
        public void Should_Return_PDF_When_Access_Is_Valid()
        {
            new ShouldReturnPDFWhenAccessIsValid().BDDfy();
        }

        [TestCase]
        public void Should_Return_Not_Found_When_ABIMId_Is_Invalid()
        {
            new ShouldReturnNotFoundWhenABIMIdIsInvalid().BDDfy();
        }

        [TestCase]
        public void Should_Return_Bad_Request_When_Voc_Letter_Content_Is_Null()
        {
            new ShouldReturnBadRequestWhenVocLetterContentIsNull().BDDfy();
        }

        [TestCase]
        public void Should_Return_APIException_When_AbimId_IsNot_Found()
        {
            new ShouldReturnApiNotFoundExceptionWhenAbimIdIsNotFound().BDDfy();
        }

        private class GetVocLetterSpecBase : BaseControllerScenario
        {
            protected CredentialController _sut;
            protected Mock<ICredentialService> _credentialServiceMock;
            protected Mock<IEnumService> _enumServiceMock;
            protected Mock<IHelperService> _helperServiceMock;
            protected Mock<IMembershipClientService> _membershipClientService;
            protected Mock<IAccessTokenService> _accessTokenServiceMock;
            protected HttpResponseMessage getVocLetterResult;

            protected override void Setup()
            {
                base.Setup();
                SetupMocks();
                _sut = new CredentialController(_credentialServiceMock.Object, _enumServiceMock.Object, _helperServiceMock.Object, _membershipClientService.Object);
                _sut.Request = _request;
            }

            protected virtual void SetupMocks()
            {
                SetupCredentialServiceMock();
                SetupEnumServiceMock();
                SetupHelperServiceMock();
                SetupAccessTokenServiceMock();
                SetupProfileApiClientWraperServiceMock();
            }

            protected virtual void SetupCredentialServiceMock()
            {
                _credentialServiceMock = new Mock<ICredentialService>(MockBehavior.Strict);
                _credentialServiceMock.Setup(o => o.SearchByMemberIdAsync(It.IsAny<Guid>()))
                    .ReturnsAsync(new List<App.Domain.Credential>());
            }

            protected virtual void SetupEnumServiceMock()
            {
                _enumServiceMock = new Mock<IEnumService>(MockBehavior.Strict);
            }

            protected virtual void SetupHelperServiceMock()
            {
                _helperServiceMock = new Mock<IHelperService>(MockBehavior.Strict);
                _helperServiceMock.Setup(o => o.GetVocLetterContent(It.IsAny<ProfileResource>(), It.IsAny<IEnumerable<App.Domain.Credential>>()))
                    .ReturnsAsync(new App.Domain.VocPdfData());
                _helperServiceMock.Setup(o => o.CreateVocLetter(It.IsAny<App.Domain.VocPdfData>()))
                    .Returns(new MemoryStream());
            }

            protected virtual void SetupProfileApiClientWraperServiceMock()
            {
                _membershipClientService = new Mock<IMembershipClientService>(MockBehavior.Strict);
                _membershipClientService.Setup(o => o.GetProfileByAbimIdAsync(It.IsAny<string>()))
                    .ReturnsAsync(new ProfileResource() { Id = Guid.NewGuid() });
            }

            protected virtual void SetupAccessTokenServiceMock()
            {
                _accessTokenServiceMock = new Mock<IAccessTokenService>(MockBehavior.Strict);
            }
        }

        private class ShouldAllowUserAccessWhenAbimIdIsPresentAndMatchesabimIdParam : GetVocLetterSpecBase
        {
            public async void WhenICallGetVocLetter()
            {
                try
                {
                    await _sut.GetVocLetter("123456");
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

        private class ShouldDenyUserAccessWhenAbimIdIsPresentAndDoesntMatchabimIdParam : GetVocLetterSpecBase
        {
            async void WhenICallGetVocLetter()
            {
                try
                {
                    var getVocLetterResponse = await _sut.GetVocLetter("333333");
                    getVocLetterResult = await getVocLetterResponse.ExecuteAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            async void ThenForbiddenShouldBeReturned()
            {
                getVocLetterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            }
        }

        private class ShouldReturnNotFoundWhenABIMIdIsInvalid : GetVocLetterSpecBase
        {
            protected override void SetupProfileApiClientWraperServiceMock()
            {
                _membershipClientService = new Mock<IMembershipClientService>(MockBehavior.Strict);
                _membershipClientService.Setup(o => o.GetProfileByAbimIdAsync("123456"))
                    .ReturnsAsync((ProfileResource)null);
            }

            async Task WhenICallGetVocLetter()
            {
                try
                {
                    var getVocLetterResponse = await _sut.GetVocLetter("123456");
                    getVocLetterResult = await getVocLetterResponse.ExecuteAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            async void ThenNotFoundShouldBeReturned()
            {
                getVocLetterResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }
        }

        private class ShouldReturnApiNotFoundExceptionWhenAbimIdIsNotFound : GetVocLetterSpecBase
        {
            protected override void SetupProfileApiClientWraperServiceMock()
            { 
                _membershipClientService = new Mock<IMembershipClientService>(MockBehavior.Strict);
                _membershipClientService.Setup(o => o.GetProfileByAbimIdAsync("123456"))
                  .ThrowsAsync(new ApiException("Not Found", 404, "", null, null));
            }

            async Task WhenICallGetVocLetter()
            {
                try
                {
                    var getVocLetterResponse = await _sut.GetVocLetter("123456");
                    getVocLetterResult = await getVocLetterResponse.ExecuteAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

             void ThenNotFoundShouldBeReturned()
            {
                getVocLetterResult.StatusCode.Should().Be(HttpStatusCode.NotFound);
            }

            void ThenNoUserExistsMessageShouldBeReturned()
            {
                var errorMessage = JsonConvert.DeserializeObject<string>(getVocLetterResult.Content.ReadAsStringAsync().Result);
                errorMessage.Should().Be($"No user exists with AbimId:'123456'.");
            }
        }

        private class ShouldReturnPDFWhenAccessIsValid : GetVocLetterSpecBase
        {

            async Task WhenICallGetVocLetter()
            {
                try
                {
                    var getVocLetterResponse = await _sut.GetVocLetter("123456");
                    getVocLetterResult = await getVocLetterResponse.ExecuteAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            void ThenPDFShouldBeReturned()
            {
                getVocLetterResult.Content.Headers.ContentDisposition.DispositionType.Should().Be("attachment");
                getVocLetterResult.Content.Headers.ContentDisposition.FileName.Should().Be("VOC.pdf");
                getVocLetterResult.Content.Headers.ContentType.MediaType.Should().Be("application/pdf");
            }
        }

        private class ShouldReturnForbiddenWhenAbimIdIsNull : GetVocLetterSpecBase
        {
            async void WhenICallGetVocLetter()
            {
                try
                {
                    var getVocLetterResponse = await _sut.GetVocLetter(null);
                    getVocLetterResult = getVocLetterResponse.ExecuteAsync(CancellationToken.None).Result;
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public async void ThenForbiddenShouldBeReturned()
            {
                getVocLetterResult.StatusCode.Should().Be(HttpStatusCode.Forbidden);
            }
        }

        private class ShouldReturnBadRequestWhenVocLetterContentIsNull : GetVocLetterSpecBase
        {
            protected override void SetupHelperServiceMock()
            {
                _helperServiceMock = new Mock<IHelperService>(MockBehavior.Strict);
                _helperServiceMock.Setup(o => o.GetVocLetterContent(It.IsAny<ProfileResource>(), It.IsAny<IEnumerable<App.Domain.Credential>>()))
                    .ReturnsAsync((VocPdfData)null);
            }

            async void WhenICallGetVocLetter()
            {
                try
                {
                    var getVocLetterResponse = await _sut.GetVocLetter("123456");
                    getVocLetterResult = await getVocLetterResponse.ExecuteAsync(CancellationToken.None);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            async void ThenBadRequestShouldBeReturned()
            {
                getVocLetterResult.StatusCode.Should().Be(HttpStatusCode.BadRequest);
            }
        }
    }
}
