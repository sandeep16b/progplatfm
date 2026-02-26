using ServiceBus.Events;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
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
        SoThat = "it can handle the UpdateCredentialFromLookback"
    )]
    [TestFixture]
    public class UpdateCredentialFromLookbackCommandSpec
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
        public void Should_Raise_Issuance_Changed_Event()
        {
            new ShouldRaiseIssuanceChangedEvent().BDDfy();
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

        private abstract class UpdateCredentialFromLookbackCommandScenario :
            CredentialServiceSimplifiedScenario
        {
            protected UpdateCredentialFromLookbackCommand _cmd;
            protected UpdateCredentialFromLookbackCommandResult _result;

            protected UpdateCredentialFromLookbackCommand GetValidCommand()
            {
                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                var cred = CredentialBuilder.BuildWithoutRandoms(source, "IM", "Internal Medicine", CertificationType.Primary, CredentialType.General, PathwayType.MOC);
                cred.AddIssuance(IssuanceBuilder.BuildWithoutRandoms(source, IssuanceStatusType.Active, DateTime.Now.AddYears(-5), DurationType.Timelimited, MaintenanceRequirementType.Required, MaintenanceStatusType.Maintained, OccurrenceType.Initial));
                cred.Issuances[0].HasAdded = true;

                return new UpdateCredentialFromLookbackCommand { Credential = cred, ModifiedBy = "UnitTest " };
            }
        }

        private class ShouldProcessCommandSuccessfullyWhenValid 
            : UpdateCredentialFromLookbackCommandScenario
        {
            private void GivenIHaveACommand()
            {
                _cmd = new UpdateCredentialFromLookbackCommand();
                _cmd.Credential = _credentials[0];
                _cmd.ModifiedBy = "YearEnd";
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {
                    _result = (UpdateCredentialFromLookbackCommandResult)_sut.Handle(_cmd);
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
            : UpdateCredentialFromLookbackCommandScenario
        {
            protected override void SetupValidationFactoryMock()
            {
                var result = new ValidationResult();
                result.Errors.Add(new ValidationFailure("someProperty", "someError"));

                var validatorMock = new Mock<IValidator<UpdateCredentialFromLookbackCommand>>();
                validatorMock
                    .Setup(x => x.Validate(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(result);

                _validationFactoryMock = new Mock<IValidationFactory>(MockBehavior.Strict);
                _validationFactoryMock
                    .Setup(x => x.GetValidatorInstance<UpdateCredentialFromLookbackCommand>())
                    .Returns(validatorMock.Object);
            }

            private void GivenIHaveACommandThatShouldFailValidation()
            {
                //NOTE: Validation failure occurs in the mocked validator
                //for this test. The input values don't really matter here.
                _cmd = new UpdateCredentialFromLookbackCommand();
                _cmd.Credential = _credentials[0];
                _cmd.ModifiedBy = "YearEnd";
            }

            private void WhenICallTheHandleMethod()
            {
                try
                {
                    _result = (UpdateCredentialFromLookbackCommandResult)_sut.Handle(_cmd);
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

        private class ShouldRaiseIssuanceChangedEvent
            : UpdateCredentialFromLookbackCommandScenario
        {
            private void GivenThatIHaveACredential()
            {
                var source = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _credentials.Add( 
                    CredentialBuilder.BuildWithoutRandoms(
                        source, 
                        "SomeCode", 
                        "Some Name", 
                        CertificationType.Primary, 
                        CredentialType.General, 
                        PathwayType.MOC));

                _credentials[0].AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        source, 
                        IssuanceStatusType.Expired, 
                        DateTime.Now.AddYears(-10), 
                        DurationType.Timelimited, 
                        MaintenanceRequirementType.Required, 
                        MaintenanceStatusType.NotMaintained, 
                        OccurrenceType.Initial));

                _credentials[0].AddIssuance(
                    IssuanceBuilder.BuildWithoutRandoms(
                        source,
                        IssuanceStatusType.Active,
                        DateTime.Now.AddDays(-1),
                        DurationType.Timelimited,
                        MaintenanceRequirementType.Required,
                        MaintenanceStatusType.Maintained,
                        OccurrenceType.Recertification));
            }

            private void AndGivenThatIHaveACommand()
            {
                _cmd = new UpdateCredentialFromLookbackCommand();
                _cmd.Credential = _credentials[0];
                _cmd.ModifiedBy = "YearEnd";
            }

            private void WhenICallHandle()
            {
                _sut.Handle(_cmd);
            }

            private void ThenTheIssuanceChangedEventShouldHaveBeenThrown()
            {
                var cred = _credentials[0];
                var issuance = cred.NewestIssuance;

                _busControlMock.Verify(
                    x => x.Publish(
                        It.Is<IssuanceChanged>(e =>
                            e.MemberId == issuance.Credential.MemberId
                            && e.CredentialGuid == cred.ExternalId
                            && e.Code == issuance.Credential.Certification.Code
                            && e.ExpirationDate == issuance.ExpirationDate
                            && e.IssuanceDate == issuance.IssuanceDate
                            && e.Status == _credentials[0].NewestIssuance.IssuanceStatus.ToString() 
                            && e.New == true
                            && e.Cosponsored == false
                            && e.Occurrence == issuance.Occurrence.ToString()
                            && e.ProcessingDate > DateTime.Now.AddMinutes(-5) //Not a great test, but value is set to DateTime.Now, so...
                            ),
                        It.IsAny<CancellationToken>()),
                        Times.Once);
            }
        }

        private class ShouldUpdateCredentialInRepository
            : UpdateCredentialFromLookbackCommandScenario
        {
            private void GivenThatIHaveACommand()
            {
                _cmd = GetValidCommand();
                _cmd.ModifiedBy = "YearEnd";
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
            : UpdateCredentialFromLookbackCommandScenario
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
                _cmd = new UpdateCredentialFromLookbackCommand();
                _cmd.Credential = _credentials[0];
                _cmd.ModifiedBy = "YearEnd";
            }

            private void WhenICallHandle()
            {
                try
                {
                    _result = (UpdateCredentialFromLookbackCommandResult)_sut.Handle(_cmd);
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
