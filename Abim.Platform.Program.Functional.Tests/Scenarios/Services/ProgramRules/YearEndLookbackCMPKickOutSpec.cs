using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Platform.Product.Interservices.Interfaces;
using Abim.Platform.Product.Resources;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules.Base;
using Abim.Platform.Program.Tests.Setup.DomainBuilders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the Program Rules service",
        SoThat = "it can run the year end lookback process and remove a user from CMP when applicable"
    )]
    public class YearEndLookbackCMPKickOutSpec
    {
        [Test]
        [WorkItem(159160)]
        public void Should_Kick_Diplomate_Out_Of_CMP_When_ExamDueDate_Is_Prior_To_Lookback_Date_And_Diplomate_Is_In_CMP()
        {
            new KickOutOfCMPScenario().BDDfy();
        }

        [Test]
        [WorkItem(159160)]
        public void Should_Not_Kick_Diplomate_Out_Of_CMP_When_ExamDueDate_Is_2020_And_Lookback_Date_Is_2021_And_Diplomate_Is_In_CMP()
        {
            new DoNotKickOutOfCMPInWhenExamDueDateIs2020ScenarioWhenDiplomateIsInCMP().BDDfy();
        }

        [Test]
        [WorkItem(209119)]
        public void Should_Not_Kick_Diplomate_Out_Of_CMP_When_ExamDueDate_Is_2021_And_Lookback_Date_Is_2021_And_Diplomate_Is_In_CMP()
        {
            new DoNotKickOutOfCMPInWhenExamDueDateIs2021ScenarioWhenDiplomateIsInCMP().BDDfy();
        }

        [Test]
        [WorkItem(159160)]
        [WorkItem(209119)]
        public void Should_Kick_Diplomate_Out_Of_CMP_In2022_When_ExamDueDate_Is_Prior_To_Lookback_Date_And_Diplomate_Is_In_CMP()
        {
            new DoKickOutOfCMPInWhenExamDueDateIs2021AndLookBackIs2022ScenarioWhenDiplomateIsInCMP().BDDfy();
        }

        [Test]
        [WorkItem(159160)]
        public void Should_Not_Kick_Diplomate_Out_Of_CMP_When_ExamDueDate_Is_Prior_To_Lookback_Date_And_Diplomate_Is_Not_On_OneYear_Pathway()
        {
            new DoNotKickOutOfCMPScenarioWhenDiplomateIsNotOnOneYearPathway().BDDfy();
        }

        [Test]
        [WorkItem(159160)]
        public void Should_Not_Kick_Diplomate_Out_Of_CMP_When_ExamDueDate_Is_Greater_Than_Lookback_Date()
        {
            new DoNotKickOutOfCMPScenarioWhenExamDueDateIsGreaterThanLookbackDate().BDDfy();
        }

        [Test]
        [WorkItem(159160)]
        public void Should_Throw_Exception_When_Call_To_Unenroll_From_CMP_Is_Not_Successful()
        {
            new ThrowExceptionWhenCallToUnenrollFromCMPIsNotSuccessful().BDDfy();
        }

        #region Scenarios
        private abstract class CMPKickOutScenario : ProgramRulesServiceSimplifiedScenario
        {
            protected List<Credential> _creds;
            protected Guid _memberId = Guid.NewGuid();
            // need to check if lookback is in 2020 then we need to go back to 2019 since we don't kick out anyone in year 2020 per Covid exception rules
            protected DateTime _lookbackDate = DateTime.Now.Year == 2021 || DateTime.Now.Year == 2022 ? new DateTime(2019, 12, 31) :  new DateTime(DateTime.Now.Year - 1, 12, 31);
            protected DateTime _processingDate;

            protected virtual void SetupCredentials()
            {
                var abimSource = SourceBuilder.Build("American Board of Internal Medicine", "ABIM", "UnitTest");
                _creds = new List<Credential>(1);
                _creds.Add(CredentialBuilder.Build(abimSource));
                DateTime issuanceDate = _lookbackDate.AddYears(-10);
                _creds[0].AddIssuance(IssuanceBuilder.Build(abimSource, IssuanceStatusType.Active, DurationType.Continuous, issuanceDate: issuanceDate));
                _creds[0].ApplyChangesAfterCreatingCredential(true, DateTime.Now.AddYears(-3), DateTime.Now.AddYears(-2), null, null, null, null, false, null);
                _creds[0].MemberId = _memberId;
                SetCredentialPropertiesRelatedToCMPKickOut();
            }

            protected virtual void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                _creds[0].IsInCMP = true;
                _creds[0].Pathway = PathwayType.OneYear;
                _creds[0].ExamDueDate = _lookbackDate.AddMonths(-1);
            }

            protected override void SetupCredentialServiceMock()
            {
                SetupCredentials();

                _credSvcMock = new Mock<App.Services.ICredentialService>(MockBehavior.Strict);

                _credSvcMock.Setup(x => x.SearchByMemberId(It.IsAny<Guid>())).Returns(_creds);

                _credSvcMock.Setup(x => x.Handle(It.IsAny<UpdateCredentialFromLookbackCommand>()))
                    .Returns(new UpdateCredentialFromLookbackCommandResult());

                //_credSvcMock.Setup(x => x.Handle(It.IsAny<ReissueCommand>()))
                   // .Returns(new ReissueCommandResult());

                _credSvcMock.Setup(x => x.GetFirstIssuanceDate(It.IsAny<Guid>()))
                    .Returns(new DateTime(DateTime.Now.Year - 20, 2, 21));

                MockCredentialServiceUnEnrollInCMPHandleMethod();
            }

            protected virtual void MockCredentialServiceUnEnrollInCMPHandleMethod()
            {
                _credSvcMock.Setup(x => x.Handle(It.IsAny<UnEnrollInCMPCommand>()))
                    .Returns(Task.FromResult(new UnEnrollInCMPCommandResult { Status = CommandStatus.Accepted }));
            }

            protected override void SetupRegistrationInterserviceMock()
            {
                var registrations = new UserRegistrationsAndCMPRegistrationsResource();
                registrations.Registrations = new List<RegistrationResource>(0);
                registrations.CMPRegistrations = new List<CMPRegistrationResource>(0);

                _regInterSvcMock = new Mock<IRegistrationInterservice>(MockBehavior.Strict);
                _regInterSvcMock
                    .Setup(x => x.GetAllRegistrationsAndCMPRegistrationsForUser(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(registrations));

                _regInterSvcMock
                   .Setup(x => x.GetLongitudinalEnrollmentsByMemberId(It.IsAny<string>(), It.IsAny<Guid>()))
                   .Returns(Task.FromResult(new LongitudinalEnrollmentCollectionResource()));
            }

            protected override void SetupProductInterserviceMock()
            {
                var activities = new ActivityFullCollectionResource();
                activities.Data = new List<ActivityResource>(0);
                _prodInterSvcMock = new Mock<IProductInterservice>(MockBehavior.Strict);
                _prodInterSvcMock
                    .Setup(x => x.GetUserActivities(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<DateTime>(), It.IsAny<DateTime>()))
                    .Returns(Task.FromResult(activities));
            }

            protected void GivenThatIHaveParameters()
            {
                _processingDate = DateTime.Now;
            }

            protected async void WhenICallTheRunLookbackMethod()
            {
                try
                {
                    await _sut.RunYearEndLookback(_memberId, _lookbackDate, _processingDate);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }
        }

        private abstract class CMPKickOutScenarioWithoutException : CMPKickOutScenario
        {
            protected void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().BeNull();
            }
        }

        private class KickOutOfCMPScenario : CMPKickOutScenarioWithoutException
        {
            protected void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                    mock => mock.Handle(
                        It.Is<UnEnrollInCMPCommand>(
                            command => 
                                command.MemberId == _memberId 
                                && command.RequestingUserName == "KickCMP" 
                                && command.SubspecialtyCertCode == _creds[0].Certification.Code 
                                && command.UnEnrollmentDate.Date == DateTime.Now.Date
                                )),
                        Times.Once);
            }
        }

        private class DoNotKickOutOfCMPScenarioWhenDiplomateIsNotInCMP : CMPKickOutScenarioWithoutException
        {
            protected override void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                base.SetCredentialPropertiesRelatedToCMPKickOut();
                _creds[0].IsInCMP = false;
            }

            protected void AndTheCredentialServiceHandleMethodShouldNOTHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                    mock => mock.Handle(
                        It.IsAny<UnEnrollInCMPCommand>()),
                        Times.Never);
            }
        }

        private class DoNotKickOutOfCMPInWhenExamDueDateIs2021ScenarioWhenDiplomateIsInCMP : CMPKickOutScenarioWithoutException
        {
            protected override void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                 _lookbackDate = new DateTime(2021, 12, 31);
                _processingDate = _lookbackDate.AddMonths(1);

                base.SetCredentialPropertiesRelatedToCMPKickOut();
                _creds[0].IsInCMP = true;
                _creds[0].ExamDueDate = new DateTime(2021,12,31); // !!!
            }

            protected void AndTheCredentialServiceHandleMethodShouldNOTHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                    mock => mock.Handle(
                        It.IsAny<UnEnrollInCMPCommand>()),
                        Times.Never);
            }
        }

        private class DoNotKickOutOfCMPInWhenExamDueDateIs2020ScenarioWhenDiplomateIsInCMP : CMPKickOutScenarioWithoutException
        {
            protected override void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                _lookbackDate = new DateTime(2021, 12, 31);
                _processingDate = _lookbackDate.AddMonths(1);

                base.SetCredentialPropertiesRelatedToCMPKickOut();
                _creds[0].IsInCMP = true;
                _creds[0].ExamDueDate = new DateTime(2020, 12, 31); // !!!
            }

            protected void AndTheCredentialServiceHandleMethodShouldNOTHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                    mock => mock.Handle(
                        It.IsAny<UnEnrollInCMPCommand>()),
                        Times.Never);
            }
        }

        private class DoKickOutOfCMPInWhenExamDueDateIs2021AndLookBackIs2022ScenarioWhenDiplomateIsInCMP : CMPKickOutScenarioWithoutException
        {
            protected override void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                _lookbackDate = new DateTime(2022, 12, 31); //!!!!
                _processingDate = _lookbackDate.AddMonths(1);

                base.SetCredentialPropertiesRelatedToCMPKickOut();
                _creds[0].IsInCMP = true;
                _creds[0].ExamDueDate = new DateTime(2021, 12, 31); // !!!
            }

            protected void AndTheCredentialServiceHandleMethodShouldHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                        mock => mock.Handle(
                            It.Is<UnEnrollInCMPCommand>(
                                command =>
                                    command.MemberId == _memberId
                                    && command.RequestingUserName == "KickCMP"
                                    && command.SubspecialtyCertCode == _creds[0].Certification.Code
                                    && command.UnEnrollmentDate.Date == DateTime.Now.Date
                                    )),
                            Times.Once);
            }
        }

        private class DoNotKickOutOfCMPScenarioWhenDiplomateIsNotOnOneYearPathway : CMPKickOutScenarioWithoutException
        {
            protected override void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                base.SetCredentialPropertiesRelatedToCMPKickOut();
                _creds[0].Pathway = PathwayType.KCI;
            }

            protected void AndTheCredentialServiceHandleMethodShouldNOTHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                    mock => mock.Handle(
                        It.IsAny<UnEnrollInCMPCommand>()),
                        Times.Never);
            }
        }

        private class DoNotKickOutOfCMPScenarioWhenExamDueDateIsGreaterThanLookbackDate : CMPKickOutScenarioWithoutException
        {
            protected override void SetCredentialPropertiesRelatedToCMPKickOut()
            {
                base.SetCredentialPropertiesRelatedToCMPKickOut();
                _creds[0].ExamDueDate = _lookbackDate.AddYears(1);
            }

            protected void AndTheCredentialServiceHandleMethodShouldNOTHaveBeenCalledToUnEnrollFromCMP()
            {
                _credSvcMock.Verify(
                    mock => mock.Handle(
                        It.IsAny<UnEnrollInCMPCommand>()),
                        Times.Never);
            }
        }

        private class ThrowExceptionWhenCallToUnenrollFromCMPIsNotSuccessful : CMPKickOutScenario
        {
            protected override void MockCredentialServiceUnEnrollInCMPHandleMethod()
            {
                _credSvcMock.Setup(x => x.Handle(It.IsAny<UnEnrollInCMPCommand>()))
                    .Returns(Task.FromResult(new UnEnrollInCMPCommandResult { Status = CommandStatus.Rejected }));
            }

            protected void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.Should().NotBeNull();
            }
        }
        #endregion Scenarios
    }
}
