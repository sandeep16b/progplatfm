extern alias SharedOldServiceBus;
using Abim.Enterprise.Core.Profile.Interservice.Interservices.Interfaces;
using Abim.Enterprise.Core.Profile.Resource;
using Abim.Platform.Program.App.DTOs;
using Abim.Platform.Program.App.HangFireJobs;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Core.Identity;
using Hangfire;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;
using SharedOldServiceBus::Abim.Enterprise.Core.ServiceBus.Notification;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestStack.BDDfy;
using static Abim.Platform.Program.App.Util.Constants;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    [Story(
     AsA = "scheduled Hangfire job",
     IWant = "to be able to invoke the DeselectCertificateChildJob",
     SoThat = "it can deselect any credentials flagged for deselection"
    )]
    [TestFixture]
    public class DeselectCertificateChildJobSpec
    {
        [Test]
        [WorkItem(187664)]
        public void ShouldProcessSuccessfully()
        {
            new ShouldProcessSuccessfullyScenario().BDDfy();
        }

        [Test]
        [WorkItem(187664)]
        public void ShouldCancelWhenRequested()
        {
            new ShouldCancelWhenRequestedScenario().BDDfy();
        }

        [Test]
        [WorkItem(180986)]
        public void ShouldPublishNotificationEventForDeactivatedCredentials()
        {
            new ShouldPublishNotificationEventForDeactivatedCredentialsScenario().BDDfy();
        }

        [Test]
        [WorkItem(180986)]
        public void ShouldNotPublishNotificationEventWhenNoDeactivatedCreds()
        {
            new ShouldNotPublishNotificationEventWhenNoDeactivatedCredentialsScenario().BDDfy();
        }


        #region Scenarios

        #region Scenario Base Classes
        private abstract class DeselectCertificateChildJobScenarioBase
        {
            protected Mock<ICredentialService> _credentialServiceMock;
            protected Mock<IBusControl> _busControlMock;
            protected Mock<IProfileInterservice> _profileInterserviceMock;
            protected Mock<IAccessTokenService> _accessTokenSingletonWraper;
            protected string _profileHostUrl = "https://myFakeProfileUrl/api/";
            protected Exception _caughtException;
            protected DeselectCertificateChildJob _sut;
            protected readonly Guid IM_GUID = Guid.Parse("1771AD17-9920-E711-8101-005056AB0204");
            protected readonly Guid CARD_GUID = Guid.Parse("0571AD17-9920-E711-8101-005056AB0198");
            protected readonly Guid ICARD_GUID = Guid.Parse("1871AD17-9920-E711-8101-005056AB0204");
            protected ProfileNestedResource _profile;
            protected Guid _memberId = Guid.NewGuid();

            protected virtual void Setup()
            {
                SetupCredentialServiceMock();
                SetupBusControlMock();
                SetupProfileInterserviceMock();
                SetupAccessTokenServiceMock();

                _sut =
                    new DeselectCertificateChildJob(
                        _credentialServiceMock.Object,
                        _busControlMock.Object,
                        _profileInterserviceMock.Object,
                        _profileHostUrl,
                        _accessTokenSingletonWraper.Object);
            }

            protected virtual void SetupCredentialServiceMock()
            {
                _credentialServiceMock = new Mock<ICredentialService>(MockBehavior.Strict);
                _credentialServiceMock
                    .Setup(x => x.Handle(It.IsAny<DeselectCertificateCommand>()))
                    .Returns(new DeselectCertificateCommandResult());
            }

            protected virtual void SetupBusControlMock()
            {
                _busControlMock = new Mock<IBusControl>(MockBehavior.Strict);
                _busControlMock
                    .Setup(mock => mock.Publish(It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()))
                    .Returns(Task.FromResult(false));
            }

            protected virtual void SetupProfileInterserviceMock()
            {
                _profile = new ProfileNestedResource();
                _profile.AbimId = "123456";
                _profile.Name = new NameResource { LastName = "Shatner" };
                _profile.EmailAddress = new EmailAddressSummaryResource { EmailAddress = "billshatner@fakemail.com" };
                
                _profileInterserviceMock = new Mock<IProfileInterservice>(MockBehavior.Strict);
                _profileInterserviceMock
                    .Setup(mock => mock.GetProfileById(It.IsAny<string>(), _profileHostUrl, _memberId))
                    .Returns(Task.FromResult(_profile));
            }

            protected virtual void SetupAccessTokenServiceMock()
            {
                _accessTokenSingletonWraper = new Mock<IAccessTokenService>(MockBehavior.Strict);
                _accessTokenSingletonWraper
                    .Setup(mock => mock.GetAccessToken())
                    .Returns("-token-");
            }

            protected virtual void GivenThatICallExecuteChild()
            {
                try
                {
                    _sut.ExecuteChild(
                        _memberId, 
                        new List<CredentialInfoDTO>(2) 
                        { 
                            new CredentialInfoDTO { ExternalId = IM_GUID, CertificateName = "Internal Medicine", IsActive = false }, 
                            new CredentialInfoDTO { ExternalId = CARD_GUID, CertificateName = "Cardiovascular Disease", IsActive = false } 
                        }, 
                        new DateTime(DateTime.Now.Year, 2, 1), 
                        null, 
                        null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }
        }
        #endregion Scenario Base Classes

        #region Should Process Successfully
        private class ShouldProcessSuccessfullyScenario : DeselectCertificateChildJobScenarioBase
        {
            private void ThenNoExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldBeNull();
            }

            private void AndTheCredentialServiceHandleMethodShouldHaveBeenExecuted()
            {
                _credentialServiceMock
                    .Verify(mock => mock.Handle(It.IsAny<DeselectCertificateCommand>()), Times.Exactly(2)); //One for each credential ID
            }
        }
        #endregion Should Process Successfully

        #region Operation Canceled Scenario
        private class ShouldCancelWhenRequestedScenario : DeselectCertificateChildJobScenarioBase
        {
            Mock<IJobCancellationToken> _cancellationToken = new Mock<IJobCancellationToken>(MockBehavior.Strict);

            protected override void Setup()
            {
                _cancellationToken.Setup(x => x.ThrowIfCancellationRequested()).Throws(new OperationCanceledException());
                base.Setup();
            }

            protected override void GivenThatICallExecuteChild()
            {
                try
                {
                    _sut.ExecuteChild(
                        _memberId,
                        new List<CredentialInfoDTO>(2)
                        {
                            new CredentialInfoDTO { ExternalId = IM_GUID, CertificateName = "Internal Medicine" },
                            new CredentialInfoDTO { ExternalId = CARD_GUID, CertificateName = "Cardiovascular Disease" }
                        },
                        new DateTime(DateTime.Now.Year, 2, 1), 
                        null, 
                        _cancellationToken.Object);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            private void ThenExceptionShouldHaveOccurred()
            {
                _caughtException.ShouldNotBeNull();
            }

            private void AndTheExceptionShouldBeAnOperationCancelledException()
            {
                _caughtException.ShouldBeOfType<OperationCanceledException>();
            }
        }
        #endregion Operation Canceled Scenario

        #region Notification Event Scenario

        private class ShouldPublishNotificationEventForDeactivatedCredentialsScenario : DeselectCertificateChildJobScenarioBase
        {
            protected override void GivenThatICallExecuteChild()
            {
                try
                {
                    _sut.ExecuteChild(
                        _memberId,
                        new List<CredentialInfoDTO>(3)
                        {
                            new CredentialInfoDTO { ExternalId = IM_GUID, CertificateName = "Internal Medicine", IsActive = true }, 
                            new CredentialInfoDTO { ExternalId = ICARD_GUID, CertificateName = "Interventional Cardiology", IsActive = false }, 
                            new CredentialInfoDTO { ExternalId = CARD_GUID, CertificateName = "Cardiovascular Disease", IsActive = true }
                        },
                        new DateTime(DateTime.Now.Year, 2, 1),
                        null,
                        null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void AndANotificationEventShouldHaveBeenPublishedForTheCertsWeDeactivated()
            {
                _busControlMock.Verify(mock => 
                    mock.Publish(It.Is<NotificationEvent>(evt => 
                        evt.EmailAddress == _profile.EmailAddress.EmailAddress
                        && evt.TemplateExternalKey == TriggeredCommunicationTemplateExternalKey.DeactivateCertification
                        && evt.Parameters["LastName"] == _profile.Name.LastName
                        && evt.Parameters["CertificationNames"] == "Internal Medicine<br />Cardiovascular Disease<br />"
                        && evt.Parameters["CertificationNames_TV"] == "Internal Medicine, Cardiovascular Disease"
                        && evt.Parameters["SubscriberKey"] == _profile.EmailAddress.EmailAddress
                        && evt.Parameters["IID"] == _profile.AbimId), It.IsAny<CancellationToken>()), Times.Once);
            }
        }

        #endregion Notification Event Scenario

        #region No Notification Event When No Deactivated Creds Scenario

        private class ShouldNotPublishNotificationEventWhenNoDeactivatedCredentialsScenario : DeselectCertificateChildJobScenarioBase
        {
            protected override void GivenThatICallExecuteChild()
            {
                try
                {
                    _sut.ExecuteChild(
                        _memberId,
                        new List<CredentialInfoDTO>(3)
                        {
                            new CredentialInfoDTO { ExternalId = IM_GUID, CertificateName = "Internal Medicine", IsActive = false },
                            new CredentialInfoDTO { ExternalId = ICARD_GUID, CertificateName = "Interventional Cardiology", IsActive = false },
                            new CredentialInfoDTO { ExternalId = CARD_GUID, CertificateName = "Cardiovascular Disease", IsActive = false }
                        },
                        new DateTime(DateTime.Now.Year, 2, 1),
                        null,
                        null);
                }
                catch (Exception ex)
                {
                    _caughtException = ex;
                }
            }

            protected void AndANotificationEventShouldHaveBeenPublishedForTheCertsWeDeactivated()
            {
                _busControlMock.Verify(mock =>
                    mock.Publish(It.IsAny<NotificationEvent>(), It.IsAny<CancellationToken>()), Times.Never);
            }
        }

        #endregion No Notification Event When No Deactivated Certs Scenario

        #endregion Scenarios
    }



}
