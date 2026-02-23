using Abim.Enterprise.Core.ServiceBus.Program;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Tests.Scenarios.Services.CredentialService.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Moq;
using NUnit.Framework;
using System;
using System.Threading;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.CredentialService
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the CredentialService",
        SoThat = "it can handle the UpdateCredentialFromObject"
    )]
    [TestFixture]
    public class UpdateCredentialFromObjectCommandSpec
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
        public void Should_Apply_New_Values_To_Loaded_Credential()
        {
            new ShouldApplyNewValuesToLoadedCredential().BDDfy();
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

        private abstract class UpdateCredentialFromObjectCommandScenario :
            CredentialServiceSimplifiedScenario
        {
            protected UpdateCredentialFromObjectCommand _cmd;
            protected UpdateCredentialFromObjectCommandResult _result;
        }

        private class ShouldProcessCommandSuccessfullyWhenValid 
            : UpdateCredentialFromObjectCommandScenario
        {
            private void GivenIHaveACommand()
            {
                _cmd = new UpdateCredentialFromObjectCommand();
                _cmd.Credential = CredentialBuilder.Build();
                _cmd.Credential.AddIssuance(IssuanceBuilder.Build());
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {
                    _result = (UpdateCredentialFromObjectCommandResult)_sut.Handle(_cmd);
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

        private class ShouldNotProcessSuccessfullyWhenCommandIsInvalid
            : UpdateCredentialFromObjectCommandScenario
        {
            protected override void SetupValidationFactoryMock()
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("someProperty", "someError"));

                var validatorMock = new Mock<IValidator<UpdateCredentialFromObjectCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<UpdateCredentialFromObjectCommand>()))
                    .Returns(result);

                _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<UpdateCredentialFromObjectCommand>())
                    .Returns(validatorMock.Object);
            }

            private void GivenIHaveACommandThatShouldFailValidation()
            {
                //NOTE: Validation failure occurs in the mocked validator
                //for this test. The input values don't really matter here.
                _cmd = new UpdateCredentialFromObjectCommand();

                _cmd.Credential = _credentials[0];
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {
                    _result = (UpdateCredentialFromObjectCommandResult)_sut.Handle(_cmd);
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

        private class ShouldLoadCredentialById
            : UpdateCredentialFromObjectCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdateCredentialFromObjectCommand();
                _cmd.Credential = CredentialBuilder.Build();
                _cmd.Credential.AddIssuance(IssuanceBuilder.Build());
            }

            private void WhenICallTheHandleMethod()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheProcessShouldLoadTheCredentialBasedOnTheCredentialId()
            {
                _credRepoMock.Verify(x => x.Load(_cmd.Credential.ExternalId), Times.Once);
            }
        }

        private class ShouldApplyNewValuesToLoadedCredential
            : UpdateCredentialFromObjectCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdateCredentialFromObjectCommand();
                _cmd.Credential = CredentialBuilder.Build();
                _cmd.Credential.AddIssuance(IssuanceBuilder.Build());

                _cmd.Credential.NewestIssuance.HasChanged = true;
                _cmd.Credential.HasChanged = true;

                _cmd.Credential.NewestIssuance.AuditData.ModifiedBy = "UnitTest";
                _cmd.Credential.AuditData.ModifiedBy = "UnitTest";

                _cmd.SetCertStatus = true;
                _cmd.SetGracePeriod = true;
                _cmd.SetParticipationStatus = true;
                _cmd.SetConsecutiveKCIPassRequired = true;
                _cmd.ReistateCertificate = true;
            }

            private void WhenICallHandle()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheValuesFromMyCommandShouldBeApplied()
            {
                var cred = _credentials[0];
                var issuance = cred.NewestIssuance;

                cred.GracePeriodEndDate.Should().Be(_cmd.Credential.GracePeriodEndDate);
                cred.GracePeriodStartDate.Should().Be(_cmd.Credential.GracePeriodStartDate);
                cred.LookbackDate.Should().Be(_cmd.Credential.LookbackDate);
                cred.ConsecutiveKCIPassRequired.Should().Be(_cmd.Credential.ConsecutiveKCIPassRequired);

                issuance.MaintenanceStatus.Should().Be(_cmd.Credential.NewestIssuance.MaintenanceStatus);
                issuance.IssuanceStatus.Should().Be(_cmd.Credential.NewestIssuance.IssuanceStatus);
                issuance.EffectiveDate.Should().Be(_cmd.Credential.LookbackDate);
                issuance.ExpirationDate.Should().Be(_cmd.Credential.ExpirationDate);
                issuance.AuditData.ModifiedBy.Should().Be(_cmd.Credential.AuditData.ModifiedBy);
                issuance.AuditData.Modified.HasValue.Should().Be(true);

                //If the below ever goes south, it doesn't necessarily mean
                //that the call wasn't made, it could be that the VALUES weren't 
                //all what we expected them to be.
                _busControlMock.Verify(
                    x => x.Publish(
                        It.Is<IssuanceChangedEvent>(e => 
                            e.CertificationCode == issuance.Credential.Certification.Code
                            && e.CertificationGuid == issuance.Credential.Certification.ExternalId
                            && e.ExpirationDate == issuance.ExpirationDate
                            && e.IssuanceDate == issuance.IssuanceDate
                            && e.IssuanceStatus == issuance.IssuanceStatus.ToString()
                            && e.MemberId == issuance.Credential.MemberId
                            && e.Occurrence == issuance.Occurrence.ToString()
                            && e.ProcessingDate > DateTime.Now.AddMinutes(-5) //Not a great test, but value is set to DateTime.Now, so...
                            ),
                        It.IsAny<CancellationToken>()),
                        Times.Once);
            }
        }

        private class ShouldUpdateCredentialInRepository
            : UpdateCredentialFromObjectCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = new UpdateCredentialFromObjectCommand();
                _cmd.Credential = CredentialBuilder.Build();
                _cmd.Credential.AddIssuance(IssuanceBuilder.Build());
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
            : UpdateCredentialFromObjectCommandScenario
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
                _cmd = new UpdateCredentialFromObjectCommand();
                _cmd.Credential = CredentialBuilder.Build();
                _cmd.Credential.AddIssuance(IssuanceBuilder.Build());
            }

            private void WhenICallHandle()
            {
                try
                {
                    _result = (UpdateCredentialFromObjectCommandResult)_sut.Handle(_cmd);
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
