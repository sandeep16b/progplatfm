using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
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

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the UnEnrollInCMPCommand"
    )]
    public class UnEnrollInCMPCommandSpec
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
        public void Should_Return_Warning_When_Credential_NOT_In_CMP()
        {
            new ShouldReturnWarningWhenCredentialNotInCMP().BDDfy();
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

        [Test]
        public void Should_Process_Command_Successfully_When_Valid_MostRecentExamType_KCI()
        {
            new ShouldProcessCommandSuccessfullyWhenValidMostRecentExamTypeKCI().BDDfy();
        }

        #region Scenarios
        private abstract class UnEnrollInCMPCommandScenario : CredentialServiceSimplifiedScenario
        {
            protected UnEnrollInCMPCommand _cmd;
            protected UnEnrollInCMPCommandResult _result;

            protected override void SetupValidationFactoryMock()
            {
                base.SetupValidationFactoryMock();

                var validatorMock = new Mock<IValidator<UnEnrollInCMPCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<UnEnrollInCMPCommand>()))
                    .Returns(new ValidationResult());

                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<UnEnrollInCMPCommand>())
                    .Returns(validatorMock.Object);
            }

        }

        #region Setup
        private static class UnEnrollInCMPCommandBuilder
        {
            public static Random Random = new Random();
            public static DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();
            public static UnEnrollInCMPCommand BuildValid()
            {
                return CommandBuilder<UnEnrollInCMPCommand>
                    .Valid()
                    .With(cmd => cmd.AbimId = Random.Next(1000, 100000).ToString())
                    .With(cmd => cmd.MemberId = Guid.NewGuid())
                    .With(cmd => cmd.UnEnrollmentDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build())
                    .With(cmd => cmd.SubspecialtyCertCode = RandomString.BuildWithLength(4, 5))
                    .With(cmd => cmd.UserInfo = new UserInfo { Username = "UnitTest" })
                    .Build();
            }
        }

        #endregion Setup

        private class ShouldProcessCommandSuccessfullyWhenValid : UnEnrollInCMPCommandScenario
        {
            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential.IsInCMP=true;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);

                _credRepoMock
                    .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                    .Returns(new AbimValidationResult { Succeeded = true });

                _credRepoMock
                    .SetupGet(x => x.CommitEachCallInItsOwnTransaction)
                    .Returns(true);

                _credRepoMock
                    .Setup(x => x.CommitTransaction());

            }

            private void GivenIHaveACommand()
            {
               _cmd = UnEnrollInCMPCommandBuilder.BuildValid();
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

            private void AndIsInCMPShouldBeSetToFalse()
            {
                _result.Data.IsInCMP.Should().BeFalse();
            }

            private void AndPathwayShouldBeSetToMOC()
            {
                _result.Data.Pathway.Should().Equals(Resources.PathwayType.MOC);
            }
        }

        private class ShouldNotProcessSuccessfullyWhenCommandIsInvalid : UnEnrollInCMPCommandScenario
        {
            protected override void SetupValidationFactoryMock()
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("someProperty", "someError"));

                var validatorMock = new Mock<IValidator<UnEnrollInCMPCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<UnEnrollInCMPCommand>()))
                    .Returns(result);

                _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<UnEnrollInCMPCommand>())
                    .Returns(validatorMock.Object);
            }

            private void GivenIHaveACommandThatShouldFailValidation()
            {
                //NOTE: Validation failure occurs in the mocked validator
                //for this test. The input values don't really matter here.
                _cmd = new UnEnrollInCMPCommand();
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

        private class ShouldLoadCredentialByByMemberAndCode : UnEnrollInCMPCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                Random Random = new Random();
                DateTimeBuilder DateTimeBuilder = new DateTimeBuilder();

                _cmd = new UnEnrollInCMPCommand
                {
                    SubspecialtyCertCode = "ICARD",
                    MemberId = Guid.NewGuid(),
                    UnEnrollmentDate = DateTimeBuilder.Random().WithYear(Random.Next(1937, DateTime.Now.Year - 1)).Build(),
                    RequestingUserName = RandomString.Build()
                };
            }

            private void WhenICallTheHandleMethod()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheProcessShouldLoadTheCredentialBasedOnTheCredentialId()
            {
                _credRepoMock.Verify(x => x.GetCredentialByMemberAndCode(_cmd.MemberId, ProgramResourceConstants.CertificationCode.InterventionalCardiology), Times.Once);
            }
        }

        private class ShouldReturnWarningWhenCredentialNotFound : UnEnrollInCMPCommandScenario
        {
            UnEnrollInCMPCommandResult result;

            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential = null;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = UnEnrollInCMPCommandBuilder.BuildValid();       
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

        private class ShouldReturnWarningWhenCredentialNotInCMP : UnEnrollInCMPCommandScenario
        {
            UnEnrollInCMPCommandResult result;

            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential.IsInCMP = false;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);

                _credRepoMock
                    .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                    .Returns(new AbimValidationResult { Succeeded = true });

                _credRepoMock
                    .SetupGet(x => x.CommitEachCallInItsOwnTransaction)
                    .Returns(true);

                _credRepoMock
                    .Setup(x => x.CommitTransaction());
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = UnEnrollInCMPCommandBuilder.BuildValid();
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
                result.Message.Should().Contain("IS NOT enrolled in the CMP pathway.");
            }
        }

        private class ShouldRetrieveMemberIdWhenOnlyAbimIdIsAvailable
            : UnEnrollInCMPCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = UnEnrollInCMPCommandBuilder.BuildValid();
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
            : UnEnrollInCMPCommandScenario
        {

            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential.IsInCMP = true;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);

                _credRepoMock
                    .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                    .Returns(new AbimValidationResult { Succeeded = true });

                _credRepoMock
                    .SetupGet(x => x.CommitEachCallInItsOwnTransaction)
                    .Returns(true);

                _credRepoMock
                    .Setup(x => x.CommitTransaction());
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = UnEnrollInCMPCommandBuilder.BuildValid();
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
            : UnEnrollInCMPCommandScenario
        {
            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential.IsInCMP = true;

                _credRepoMock
                 .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                 .Returns(credential);

                _credRepoMock
                    .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                    .Throws(new ApplicationException("BAM!"));

                _credRepoMock
                     .SetupGet(x => x.CommitEachCallInItsOwnTransaction)
                     .Returns(true);

                _credRepoMock
                    .Setup(x => x.CommitTransaction());

            }

            private void GivenIHaveACommand()
            {
                _cmd = UnEnrollInCMPCommandBuilder.BuildValid();
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

        private class ShouldProcessCommandSuccessfullyWhenValidMostRecentExamTypeKCI : UnEnrollInCMPCommandScenario
        {
            protected override void SetupCredentialRepositoryMock()
            {
                _credRepoMock = new Mock<ICredentialRepository>(MockBehavior.Strict);

                var credential = CredentialBuilder.Build();
                credential.IsInCMP = true;
                //------ most Recent ExamType and credential is KCI
                mostRecentExamType = ExamType.Kci;
                credential.Pathway = Resources.PathwayType.KCI;

                _credRepoMock
                     .Setup(x => x.GetCredentialByMemberAndCode(It.IsAny<Guid>(), It.IsAny<string>()))
                     .Returns(credential);

                _credRepoMock
                    .Setup(x => x.Update(It.IsAny<App.Domain.Credential>(), It.IsAny<string>()))
                    .Returns(new AbimValidationResult { Succeeded = true });

                _credRepoMock
                    .SetupGet(x => x.CommitEachCallInItsOwnTransaction)
                    .Returns(true);

                _credRepoMock
                    .Setup(x => x.CommitTransaction());

            }

            private void GivenIHaveACommand()
            {
                _cmd = UnEnrollInCMPCommandBuilder.BuildValid();
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

            private void AndIsInCMPShouldBeSetToFalse()
            {
                _result.Data.IsInCMP.Should().BeFalse();
            }

            private void AndPathwayShouldBeSetToKCI()
            {
                _result.Data.Pathway.Should().Equals(Resources.PathwayType.KCI);
            }
        }

        #endregion Scenarios
    }
}
