using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Platform.Program.WebApi.Testing.Setup;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using ServiceBus.Events;
using System.Threading;
using Abim.Platform.Program.App.Domain;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the EnrollInCMPCommand"
    )]
    public class EnrollInCMPCommandSpec
    {
        [Test]
        public void Should_Process_Command_Successfully_When_Valid()
        {
            new ShouldProcessCommandSuccessfullyWhenValid().BDDfy();
        }

        [Test]
        public void Should_Not_Process_Successfully_When_Command_Is_Invalid()
        {
            new ShouldNotProcessSuccessfullyWhenCommandIsInvalid().BDDfy();
        }

        [Test]
        public void Should_Load_Credential_By_Member_And_Code()
        {
            new ShouldLoadCredentialByByMemberAndCode().BDDfy();
        }

        [Test]
        public void Should_Return_Warning_When_Credential_Not_Found()
        {
            new ShouldReturnWarningWhenCredentialNotFound().BDDfy();
        }

        [Test]
        public void Should_Return_Warning_When_Credential_Already_In_CMP()
        {
            new ShouldReturnWarningWhenCredentialAlreadyInCMP().BDDfy();
        }

        [Test]
        public void Should_Retrieve_MemberId_when_Only_AbimId_is_Available()
        {
            new ShouldRetrieveMemberIdWhenOnlyAbimIdIsAvailable().BDDfy();
        }

        [Test]
        public void Should_Update_Credential_In_Repository()
        {
            new ShouldUpdateCredentialInRepository().BDDfy();
        }

        [Test]
        public void Should_Return_Error_Result_When_Repo_Update_Fails()
        {
            new ShouldReturnErrorResultWhenRepoUpdateFails().BDDfy();
        }

        #region Scenarios
        private abstract class EnrollInCMPCommandScenario : CredentialServiceSimplifiedScenario
        {
            protected EnrollInCMPCommand _cmd;
            protected EnrollInCMPCommandResult _result;

            protected override void SetupValidationFactoryMock()
            {
                base.SetupValidationFactoryMock();

                var validatorMock = new Mock<IValidator<EnrollInCMPCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<EnrollInCMPCommand>()))
                    .Returns(new ValidationResult());

                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<EnrollInCMPCommand>())
                    .Returns(validatorMock.Object);
            }

        }

        #region Setup
        private static class EnrollInCMPCommandBuilder
        {
            public static Random Random = new Random();
            public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
            public static EnrollInCMPCommand BuildValid()
            {
                return CommandBuilder<EnrollInCMPCommand>
                    .Valid()
                    .With(cmd => cmd.AbimId = Random.Next(1000, 100000).ToString())
                    .With(cmd => cmd.MemberId = Guid.NewGuid())
                    .With(cmd => cmd.EnrollmentDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                    .With(cmd => cmd.SubspecialtyCertCode = RandomString.BuildWithLength(4, 5))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "UnitTest" })
                    .Build();
            }
        }

        #endregion Setup

        private class ShouldProcessCommandSuccessfullyWhenValid : EnrollInCMPCommandScenario
        {

            private void GivenIHaveACommand()
            {
                _cmd = EnrollInCMPCommandBuilder.BuildValid();
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {
                    _result = _sut.Handle(_cmd).Result;
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }

            private void AndResultShouldBeSuccessful()
            {
                _result.Succeeded.Should().BeTrue();
            }

            private void AndCredentialShoulBeSetToInCMP()
            {
                _result.Data.IsInCMP.Should().BeTrue();
            }

            private void AndACMPEnrolledShouldBePublished ()
            {
                _busControlMock
                    .Verify(x => 
                                x.Publish(It.IsAny<CMPEnrolled>(), It.IsAny<CancellationToken>()),
                                Times.Once);
            }
        }

        private class ShouldNotProcessSuccessfullyWhenCommandIsInvalid : EnrollInCMPCommandScenario
        {
            protected override void SetupValidationFactoryMock()
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("someProperty", "someError"));

                var validatorMock = new Mock<IValidator<EnrollInCMPCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<EnrollInCMPCommand>()))
                    .Returns(result);

                _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<EnrollInCMPCommand>())
                    .Returns(validatorMock.Object);
            }

            private void GivenIHaveACommandThatShouldFailValidation()
            {
                //NOTE: Validation failure occurs in the mocked validator
                //for this test. The input values don't really matter here.
                _cmd = new EnrollInCMPCommand();
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {
                    _result = _sut.Handle(_cmd).Result;
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }

            private void AndResultShouldNotBeSuccessful()
            {
                _result.Succeeded.Should().BeFalse();
            }

            private void AndMessageShouldStateThatValidationFailed()
            {
                _result.Message.Should().StartWith("Validation Failed");
            }
        }

        private class ShouldLoadCredentialByByMemberAndCode : EnrollInCMPCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                Random Random = new Random();
                DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

                _cmd = new EnrollInCMPCommand
                {
                    SubspecialtyCertCode = "ICARD",
                    MemberId = Guid.NewGuid(),
                    EnrollmentDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build(),
                    RequestingUserName = RandomString.Build()
                };
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {

                    _sut.Handle(_cmd);
                }
                catch( Exception ex)
                {
                    _caughtException = ex;
                }
            }

            public void ThenNoExceptionShouldHaveBeenThrown()
            {
                _caughtException.Should().BeNull();
            }
            private void ThenTheProcessShouldLoadTheCredentialBasedOnTheCredentialId()
            {
                _credRepoMock.Verify(x => x.GetCredentialByMemberAndCode(_cmd.MemberId, ProgramResourceConstants.CertificationCode.InterventionalCardiology), Times.Once);
            }
        }

        private class ShouldReturnWarningWhenCredentialNotFound : EnrollInCMPCommandScenario
        {
            EnrollInCMPCommandResult result;

            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                Credential credential;
                CredentialBuilder.Build();
                credential = null;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = EnrollInCMPCommandBuilder.BuildValid();       
            }

            private  async void WhenICallTheHandleMethod()
            {
               result = await _sut.Handle(_cmd);
            }

            private void ThenTheProcessShouldNotBeSucceeded()
            {
                result.Succeeded.Should().BeFalse();
            }

            private void ThenTheProcessShouldReturnProperMessage()
            {
                result.Message.Should().Contain("does not have credential with Subspecialty Code");
            }
        }

        private class ShouldReturnWarningWhenCredentialAlreadyInCMP : EnrollInCMPCommandScenario
        {
            EnrollInCMPCommandResult result;

            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential.IsInCMP=true;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = EnrollInCMPCommandBuilder.BuildValid();
            }

            private async void WhenICallTheHandleMethod()
            {
                result = await _sut.Handle(_cmd);
            }

            private void ThenTheProcessShouldNotBeSucceeded()
            {
                result.Succeeded.Should().BeFalse();
            }

            private void ThenTheProcessShouldReturnProperMessage()
            {
                result.Message.Should().Contain("is already enrolled in the CMP pathway");
            }
            private void AndACMPEnrolledShouldNotBePublished()
            {
                _busControlMock
                    .Verify(x =>
                                x.Publish(It.IsAny<CMPEnrolled>(), It.IsAny<CancellationToken>()),
                                Times.Never);
            }
        }

        private class ShouldRetrieveMemberIdWhenOnlyAbimIdIsAvailable
            : EnrollInCMPCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = EnrollInCMPCommandBuilder.BuildValid();
                _cmd.MemberId = Guid.Empty;
            }

            private void WhenICallHandle()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheProcessShouldCallGetMemberIdByAbimId()
            {
                _helperSvcMock.Verify(x => x.GetMemberIdByAbimId(_cmd.AbimId), Times.Once);
            }

            private void ThenTheProcessShouldSetMemberId()
            {
                _cmd.MemberId.Should().NotBe(Guid.Empty);
            }



        }

        private class ShouldUpdateCredentialInRepository
            : EnrollInCMPCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = EnrollInCMPCommandBuilder.BuildValid();
            }

            private void WhenICallHandle()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheRepoShouldUpdateTheCredential()
            {
                _credRepoMock.Verify(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()), Times.Once);
            }
        }

        private class ShouldReturnErrorResultWhenRepoUpdateFails
            : EnrollInCMPCommandScenario
        {
            protected override void SetupCredentialRepositoryMock()
            {
                base.SetupCredentialRepositoryMock();
                _credRepoMock
                    .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                    .Throws(new ApplicationException("BAM!"));
            }

            private void GivenIHaveACommand()
            {
                _cmd = EnrollInCMPCommandBuilder.BuildValid();
            }

            private void WhenICallHandle()
            {
                try
                {
                    _result = _sut.Handle(_cmd).Result;
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenNoExceptionShouldHaveBeenThrownFromHandle()
            {
                _caughtException.Should().BeNull();
            }

            private void AndAnUnsuccessfulResultShouldHaveBeenReturned()
            {
                _result.Succeeded.Should().BeFalse();
            }

            private void AndTheFailureReasonShouldBeInTheResults()
            {
                _result.Message.Should().Be("Failed to update the Credential in the database; BAM!");
            }
        }

        #endregion Scenarios
    }
}
