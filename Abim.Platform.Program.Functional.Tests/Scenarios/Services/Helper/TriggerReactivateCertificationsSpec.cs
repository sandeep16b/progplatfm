extern alias SharedOldServiceBus; 
using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Impl;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Tests.Scenarios.Services.SourceService.Base;
using Abim.Platform.Program.WebApi.Testing.Setup.Builders;
using FluentAssertions;
using MassTransit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using SharedOldServiceBus::Abim.Enterprise.Core.ServiceBus.Notification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestStack.BDDfy;
using static Abim.Platform.Program.App.Util.Constants;
using Abim.Platform.Program.MembershipClient;

namespace Abim.Platform.Registration.Tests.Scenarios.Services.Helper
{
    ///<summary>
    ///Unit Test main class
    ///</summary>
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the HelperService",
        SoThat = "it can handle the TriggeredCommunication_Reactivate_Certifications"
        )]
    [TestFixture]
    public class TriggerReactivateCertificationsSpec
    {
        [TestCase]
        [WorkItem(180989)]
        public void TriggerReactivatedCertificationsOnSuccessSpec()
        {
            new TriggerReactivatedCertificationsOnSuccess().BDDfy();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class TriggerReactivateCertificationsScenario : HelperServiceScenarioEx
    {
        protected ProfileResource profile;
        protected IList<string> certNames;
        protected NotificationEvent notificationEvent;

        protected override List<Type> AdditionalDependencies()
        {
            var types = base.AdditionalDependencies();

            types.Add(typeof(IBusControl));
            types.Add(typeof(ILogger));
            types.Add(typeof(IRegistrationInterservice));
            types.Add(typeof(IAccessTokenService));
            return types;
        }
    }

    #region Scenarios

    /// <summary>
    /// The Success scenario
    /// </summary>
    public class TriggerReactivatedCertificationsOnSuccess : TriggerReactivateCertificationsScenario
    {
        IHelperService sut { get; set; }
        Mock<ILogger> Log { get; set; }
        new Exception ExceptionCaught { get; set; }

        // copied from HelpService
        private string GetDelimitedCertNames(IList<string> certNames, string delimiter, bool addDelimiterToEnd)
        {
            return string.Join(delimiter, certNames) + (addDelimiterToEnd ? delimiter : "");
        }

        /// <summary>
        /// Primary setup
        /// </summary>
        protected override void PreSetup()
        {
            Log = new Mock<ILogger>();
            EmailBuilder = new EmailBuilder();
            DateTimeBuilder = new DateTimeBuilder();

            // set up certNames
            certNames = new List<string>() { "Critical Care Medicine", "Hospice and Palliative Medicine", };

            // set up Profile
            profile = new ProfileResource();
            profile.AbimId = "123456";
            profile.Name = new ProfileNameResource { LastName = "Smith" };
            profile.EmailAddress = "ASmith@fakemail.com";


            // set up NotificationEvent
            notificationEvent = new NotificationEvent()
            {
                TemplateExternalKey = TriggeredCommunicationTemplateExternalKey.Reactivate_Certification,
                EmailAddress = profile.EmailAddress,
                Parameters = new Dictionary<string, string> {
                    { "LastName", profile.Name.LastName},
                    { "CertificationNames", GetDelimitedCertNames(certNames, "<br />", true) },
                    { "CertificationNames_TV", GetDelimitedCertNames(certNames, ", ", false) },
                    { "EmailAddress", profile.EmailAddress},
                    { "SubscriberKey", profile.AbimId},
                    { "IID", profile.AbimId},
                    { "Env", "DEV"}
                }
            };

        }

        /// <summary>
        /// Secondary setup (requiring the Container)
        /// </summary>
        protected override void PostSetup()
        {
            // *** set up Profile Interservice
            My<IMembershipClientService>()
                .Setup(o => o.GetProfileByMemberIdAsync(It.IsAny<Guid>()))
                .Returns(Task.FromResult(profile));

            // *** Bus ----
            My<IBusControl>()
                .Setup(mock => mock.Publish(
                    It.IsAny<NotificationEvent>(),
                    It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(true));

            // ***  AccessTokenSingletonWraperMock ---
             My<IAccessTokenService>()
                .Setup(o => o.GetAccessToken())
                .Returns("--token--");

            sut = Container.GetInstance<HelperService>();
        }

        public async void WhenICallTriggeredCommunication_Reactivate_Certifications()
        {
            try
            {
                await sut.TriggeredCommunication_Reactivate_Certifications(certNames, Guid.NewGuid());
            }
            catch (Exception ex)
            {
                ExceptionCaught = ex;
            }
        }

        public void ThenNoExceptionShouldHaveBeenThrown()
        {
            ExceptionCaught.Should().BeNull();
        }


        public void AndProfileMembershipShouldBeCalled()
        {
            My<IMembershipClientService>()
                .Verify(mock =>
                    mock.GetProfileByMemberIdAsync(It.IsAny<Guid>()),
                        Times.Once);
        }

        public void AndBusPublishEventShouldBeCalled()
        {
            My<IBusControl>()
                .Verify(mock =>
                    mock.Publish(
                        It.Is<NotificationEvent>(n => n.TemplateExternalKey == notificationEvent.TemplateExternalKey &&
                                                        n.EmailAddress == notificationEvent.EmailAddress &&
                                                        n.Parameters.SequenceEqual(notificationEvent.Parameters)), It.IsAny<CancellationToken>()),
                        Times.Once);
        }

    }

    #endregion
}

