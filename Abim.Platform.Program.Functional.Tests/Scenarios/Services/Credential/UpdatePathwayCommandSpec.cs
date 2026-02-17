using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Services.CommandValidators.Credential;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.WebApi.Authentication;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using System;
using TestStack.BDDfy;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the UpdatePathwayCommand"
    )]
    public class UpdatePathwayCommandSpec
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
        public void Should_Load_Credential_By_Id()
        {
            new ShouldLoadCredentialById().BDDfy();
        }

        [Test]
        public void Should_Apply_New_Pathway_To_Loaded_Credential()
        {
            new ShouldApplyNewPathwayToLoadedCredential().BDDfy();
        }

        [Test]
        public void Should_Set_DisplayExamDueDate_When_Switching_To_MOC_Pathway()
        {
            new ShouldSetDisplayExamDueDateWhenSwitchingToMOCPathway().BDDfy();
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
        private abstract class UpdatePathwayCommandScenario : CredentialServiceSimplifiedScenario
        {
            protected UpdatePathwayCommand _cmd;
            protected UpdatePathwayCommandResult _result;
            protected Mock<UpdatePathwayCommandValidator> _validatorMock;
            protected DateTime _displayExamDueDate;

            protected override void SetupValidationFactoryMock()
            {
                base.SetupValidationFactoryMock();

                var validatorMock = new Mock<IValidator<UpdatePathwayCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<UpdatePathwayCommand>()))
                    .Returns(new ValidationResult());

                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<UpdatePathwayCommand>())
                    .Returns(validatorMock.Object);
            }
        }

        private class ShouldProcessCommandSuccessfullyWhenValid : UpdatePathwayCommandScenario
        {
            private void GivenIHaveACommand()
            {
                _cmd = new UpdatePathwayCommand();
                _cmd.Pathway = PathwayType.MOC;
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
        }

        private class ShouldNotProcessSuccessfullyWhenCommandIsInvalid : UpdatePathwayCommandScenario
        {
            protected override void SetupValidationFactoryMock()
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("someProperty", "someError"));

                var validatorMock = new Mock<IValidator<UpdatePathwayCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<UpdatePathwayCommand>()))
                    .Returns(result);

                _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<UpdatePathwayCommand>())
                    .Returns(validatorMock.Object);
            }

            private void GivenIHaveACommandThatShouldFailValidation()
            {
                //NOTE: Validation failure occurs in the mocked validator
                //for this test. The input values don't really matter here.
                _cmd = new UpdatePathwayCommand();
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

        private class ShouldLoadCredentialById : UpdatePathwayCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdatePathwayCommand();
                _cmd.CredentialId = Guid.NewGuid();
            }

            private void WhenICallTheHandleMethod()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheProcessShouldLoadTheCredentialBasedOnTheCredentialId()
            {
                _credRepoMock.Verify(x => x.Load(_cmd.CredentialId), Times.Once);
            }
        }

        private class ShouldApplyNewPathwayToLoadedCredential
            : UpdatePathwayCommandScenario
        {
            protected override void SetupCredentialRepositoryMock()
            {
                base.SetupCredentialRepositoryMock();
                _credentials[0].Pathway = PathwayType.KCI;
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdatePathwayCommand();
                _cmd.CredentialId = Guid.NewGuid();
                _cmd.Pathway = PathwayType.MOC;
            }

            private void WhenICallHandle()
            {
                _sut.Handle(_cmd);
            }

            private void ThenThePathwayFromMyCommandShouldBeApplied()
            {
                var cred = _credentials[0];
                cred.Pathway.Should().Be(_cmd.Pathway);
            }
        }

        private class ShouldSetDisplayExamDueDateWhenSwitchingToMOCPathway :
            UpdatePathwayCommandScenario
        {
            protected override void SetupCredentialRepositoryMock()
            {
                base.SetupCredentialRepositoryMock();
                _credentials[0].Pathway = PathwayType.KCI;

                _displayExamDueDate = new DateTime(2019, 12, 31);
                _credentials[0].DisplayExamDueDate = _displayExamDueDate;
                _credentials[0].MOCExamDueDate = _displayExamDueDate.AddYears(2); // not a real situation, but just to prove that DisplayExamDueDate would be unchanged
            }

            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdatePathwayCommand();
                _cmd.CredentialId = Guid.NewGuid();
                _cmd.Pathway = PathwayType.MOC;
            }

            private void WhenICallHandle()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheDisplayExamDueDateShouldRemainUnchanged()
            {
                var cred = _credentials[0];
                cred.DisplayExamDueDate.Should().Be(_displayExamDueDate);
            }
        }

        private class ShouldUpdateCredentialInRepository
            : UpdatePathwayCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdatePathwayCommand();
                _cmd.UserInfo = new UserInfo { Username = "UnitTest" };
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
            : UpdatePathwayCommandScenario
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
                _cmd = new UpdatePathwayCommand();
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
