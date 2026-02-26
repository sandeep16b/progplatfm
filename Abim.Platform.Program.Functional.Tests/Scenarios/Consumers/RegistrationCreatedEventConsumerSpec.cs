using Abim.Enterprise.Core.Registration.Interservice;
using Abim.Enterprise.Core.Registration.Resources;
using Abim.Enterprise.Core.ServiceBus.Registration;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.ServiceBus;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Core.Identity;
using Abim.Platform.Program.Tests.Scenarios.Consumers.Base;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Consumers
{
    [Story(
        AsA = "bus consumer",
        IWant = "to be able to utilize the RegistrationCreatedEventConsumer Consumer",
        SoThat = "to consume IRegistrationCreatedEvent messages"
    )]
    [TestFixture]
    public class RegistrationCreatedEventConsumerSpec
    {
        [Test]
        [WorkItem(211538)]
        public void Should_Not_Mark_Certificate_For_Selection_When_Credential_Data_Is_Not_Found()
        {
            new Should_Not_Mark_Certificate_For_Selection_When_Credential_Data_Is_Not_Found_Spec().BDDfy();
        }

        [Test]
        [WorkItem(211538)]
        public void Should_Not_Mark_Certificate_For_Selection_When_Certification_Data_Is_Not_Found()
        {
            new Should_Not_Mark_Certificate_For_Selection_When_Certification_Data_Is_Not_Found_Spec().BDDfy();
        }

        [Test]
        [WorkItem(211538)]
        public void Should_Mark_Certificate_For_Selection_When_Credential_Data_Found()
        {
            new Should_Mark_Certificate_For_Selection_When_Credential_Data_Found_Spec().BDDfy();
        }



        private abstract class RegistrationCreatedEventConsumerScenario : ConsumerScenario
        {
            protected InMemoryTestHarness Harness;
            protected ConsumerTestHarness<RegistrationCreatedEventConsumer> Consumer;
            protected HandlerTestHarness<IRegistrationCreatedEvent> Handler;

            protected Mock<ILogger> Log { get; set; }
            protected Mock<ICredentialService> CredentialService { get; set; }
            protected Mock<IAccessTokenService> AccessTokenService { get; set; }
            protected Mock<IRegistrationInterservice> RegistrationInterservice { get; set; }
            protected Mock<IBusControl> BusControl { get; set; }

            protected IRegistrationCreatedEvent EventToSend { get; set; }

            protected Func<RegistrationCreatedEventConsumer> ConsumerFactoryMethod;


            protected async Task WhenTheMessageIsSentToTheEndPoint()
            {
                await Harness.Start();
                try
                {
                    await Harness.InputQueueSendEndpoint.Send(
                        EventToSend,
                        context => context.ResponseAddress = Harness.BusAddress);
                }
                catch (Exception ex)
                {
                    ExceptionCaught = ex;
                }
            }

            protected void ThenNoExceptionShouldHaveBeenThrown()
            {
                ExceptionCaught.Should().BeNull();
            }

            protected void AndThereShouldBeNoExceptionInTheConsumer()
            {
                Consumer.Consumed.Select<IRegistrationCreatedEvent>()
                    .FirstOrDefault()
                    .Exception
                    .Should().BeNull();
            }

            protected void AndTheMessageShouldHaveBeenSentToTheConsumerFromTheEndpoint()
            {
                Harness.Sent.Select<IRegistrationCreatedEvent>().Any().Should().BeTrue();
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheHarness()
            {
                Harness.Consumed.Select<IRegistrationCreatedEvent>().Any().Should().BeTrue();
            }
        }


        private class Should_Not_Mark_Certificate_For_Selection_When_Credential_Data_Is_Not_Found_Spec : RegistrationCreatedEventConsumerScenario
        {
            private Guid testMemberId = Guid.NewGuid();
            private Guid testCertificationId = Guid.NewGuid();
            private Guid testCredentialId = Guid.NewGuid();
            private Guid testRegistrationId = Guid.NewGuid();


            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>();
                AccessTokenService = new Mock<IAccessTokenService>();
                RegistrationInterservice = new Mock<IRegistrationInterservice>();
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new RegistrationCreatedEventConsumer(CredentialService.Object, RegistrationInterservice.Object, AccessTokenService.Object);

                Consumer = Harness.Consumer(ConsumerFactoryMethod);

                Handler = Harness.Handler<IRegistrationCreatedEvent>();
            }

            protected new async Task Teardown()
            {
                await Harness.Stop();
            }

            protected override void PostSetup()
            {
                // GetAccessToken returns a string, but we're returning a random GUID 
                // as a string because we just want a random string value. Don't ask.
                //AccessTokenService
                AccessTokenService.Setup(x => x.GetAccessToken())
                    .Returns(Guid.NewGuid().ToString()); 

                // return empty list of issuances to cause the data not to be found
                CredentialService
                    .Setup(x => x.GetIssuancesForMemberId(It.IsAny<Guid>()))
                    .Returns(new List<Issuance>());

                CredentialService
                    .Setup(x => x.Handle(It.IsAny<MarkCertificatesForSelectOrDeselectCommand>()))
                    .Returns(new MarkCertificatesForSelectOrDeselectCommandResult());

                RegistrationInterservice
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new RegistrationResource()
                    {
                        CertificationId = testCertificationId
                    }));

                LogTest.Watch(Log);
            }

            void GivenIHaveAValidEvent()
            {
                EventToSend = new RegistrationCreatedEvent()
                {
                    MemberId = testMemberId,
                    RegistrationId = testRegistrationId
                };
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheConsumer()
            {
                //Calling Any() below is what will enumerate the consumer and actually invoke the
                //Consume event! But having this in the base class didn't work for some reason -- it only 
                //works correctly from here!
                Consumer.Consumed.Select<IRegistrationCreatedEvent>().Any().Should().BeTrue();
            }

            protected void AndTheHandlerShouldHaveConsumedTheMessage()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            protected void AndTheCredentialServiceShouldNotHaveBeenUsedToMarkTheCertificateForSelection()
            {
                CredentialService
                    .Verify(o => o.Handle(It.Is<MarkCertificatesForSelectOrDeselectCommand>(command =>
                    command.MemberId == EventToSend.MemberId
                    && command.CredentialIdsForSelect.Any())), Times.Never);
            }
        }

        private class Should_Not_Mark_Certificate_For_Selection_When_Certification_Data_Is_Not_Found_Spec : RegistrationCreatedEventConsumerScenario
        {
            private Guid testMemberId = Guid.NewGuid();
            private Guid testCertificationId = Guid.NewGuid();
            private Guid testCredentialId = Guid.NewGuid();
            private Guid testRegistrationId = Guid.NewGuid();


            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>();
                AccessTokenService = new Mock<IAccessTokenService>();
                RegistrationInterservice = new Mock<IRegistrationInterservice>();
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new RegistrationCreatedEventConsumer(CredentialService.Object, RegistrationInterservice.Object,AccessTokenService.Object);

                Consumer = Harness.Consumer(ConsumerFactoryMethod);

                Handler = Harness.Handler<IRegistrationCreatedEvent>();
            }

            protected new async Task Teardown()
            {
                await Harness.Stop();
            }

            protected override void PostSetup()
            {
                // GetAccessToken returns a string, but we're returning a random GUID 
                // as a string because we just want a random string value. Don't ask.
                //AccessTokenService
                //    .Setup(x => x.GetAccessToken())
                //    .Returns(Task.FromResult(Guid.NewGuid().ToString()));


                var issuance = Issuance.Create(
                            null,
                            Resources.DurationType.Continuous,
                            Resources.MaintenanceRequirementType.Required,
                            Resources.MaintenanceStatusType.Maintained,
                            Resources.OccurrenceType.Initial,
                            Resources.IssuanceStatusType.Active,
                            new DateTime(2009, 2, 1), "unit test");

                // certification is set to null to cause the data not to be found.
                issuance.Credential = Credential.Create(
                            null,
                            testMemberId,
                            Resources.CredentialType.General,
                            Resources.PathwayType.MOC,
                            "ABCDE",
                            "ABCDE",
                            "unit test"
                            );
                issuance.Credential.ExternalId = testCredentialId;

                CredentialService
                    .Setup(x => x.GetIssuancesForMemberId(It.IsAny<Guid>()))
                    .Returns(new List<Issuance>()
                    {
                        issuance
                    });

                CredentialService
                    .Setup(x => x.Handle(It.IsAny<MarkCertificatesForSelectOrDeselectCommand>()))
                    .Returns(new MarkCertificatesForSelectOrDeselectCommandResult());

                RegistrationInterservice
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new RegistrationResource()
                    {
                        CertificationId = testCertificationId
                    }));

                LogTest.Watch(Log);
            }

            void GivenIHaveAValidEvent()
            {
                EventToSend = new RegistrationCreatedEvent()
                {
                    MemberId = testMemberId,
                    RegistrationId = testRegistrationId
                };
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheConsumer()
            {
                //Calling Any() below is what will enumerate the consumer and actually invoke the
                //Consume event! But having this in the base class didn't work for some reason -- it only 
                //works correctly from here!
                Consumer.Consumed.Select<IRegistrationCreatedEvent>().Any().Should().BeTrue();
            }

            protected void AndTheHandlerShouldHaveConsumedTheMessage()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            protected void AndTheCredentialServiceShouldNotHaveBeenUsedToMarkTheCertificateForSelection()
            {
                CredentialService
                    .Verify(o => o.Handle(It.Is<MarkCertificatesForSelectOrDeselectCommand>(command =>
                    command.MemberId == EventToSend.MemberId
                    && command.CredentialIdsForSelect.Any())), Times.Never);
            }
        }

        private class Should_Mark_Certificate_For_Selection_When_Credential_Data_Found_Spec : RegistrationCreatedEventConsumerScenario
        {
            private Guid testMemberId = Guid.NewGuid();
            private Guid testCertificationId = Guid.NewGuid();
            private Guid testCredentialId = Guid.NewGuid();
            private Guid testRegistrationId = Guid.NewGuid();

            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>();
                AccessTokenService = new Mock<IAccessTokenService>();
                RegistrationInterservice = new Mock<IRegistrationInterservice>();
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new RegistrationCreatedEventConsumer(CredentialService.Object, RegistrationInterservice.Object,AccessTokenService.Object);

                Consumer = Harness.Consumer(ConsumerFactoryMethod);

                Handler = Harness.Handler<IRegistrationCreatedEvent>();
            }

            protected new async Task Teardown()
            {
                await Harness.Stop();
            }

            protected override void PostSetup()
            {
                // GetAccessToken returns a string, but we're returning a random GUID 
                // as a string because we just want a random string value. Don't ask.
                //AccessTokenService
                //    .Setup(x => x.GetClientAccessTokenAsync(It.IsAny<string>()))
                //    .Returns(Task.FromResult(Guid.NewGuid().ToString()));

                var issuance = Issuance.Create(
                            null,
                            Resources.DurationType.Continuous,
                            Resources.MaintenanceRequirementType.Required,
                            Resources.MaintenanceStatusType.Maintained,
                            Resources.OccurrenceType.Initial,
                            Resources.IssuanceStatusType.Active,
                            new DateTime(2009, 2, 1), "unit test");


                var certification = Certification.Create(null, 
                    null, 
                    Resources.CertificationType.Primary,
                    "Internal Medicine", 
                    "IM", 
                    "unit test");
                certification.ExternalId = testCertificationId;

                issuance.Credential = Credential.Create(
                            certification, 
                            testMemberId, 
                            Resources.CredentialType.General, 
                            Resources.PathwayType.MOC, 
                            "ABCDE", 
                            "ABCDE", 
                            "unit test"
                            );
                issuance.Credential.ExternalId = testCredentialId;

                CredentialService
                    .Setup(x => x.GetIssuancesForMemberId(It.IsAny<Guid>()))
                    .Returns(new List<Issuance>()
                    {
                        issuance
                    });

                CredentialService
                    .Setup(x => x.Handle(It.IsAny<MarkCertificatesForSelectOrDeselectCommand>()))
                    .Returns(new MarkCertificatesForSelectOrDeselectCommandResult());

                RegistrationInterservice
                    .Setup(x => x.GetRegistrationById(It.IsAny<string>(), It.IsAny<Guid>()))
                    .Returns(Task.FromResult(new RegistrationResource()
                    {
                        CertificationId = testCertificationId
                    }));

                LogTest.Watch(Log);
            }

            void GivenIHaveAValidEvent()
            {
                EventToSend = new RegistrationCreatedEvent()
                {
                    MemberId = testMemberId,
                    RegistrationId = testRegistrationId
                };
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheConsumer()
            {
                //Calling Any() below is what will enumerate the consumer and actually invoke the
                //Consume event! But having this in the base class didn't work for some reason -- it only 
                //works correctly from here!
                Consumer.Consumed.Select<IRegistrationCreatedEvent>().Any().Should().BeTrue();
            }

            protected void AndTheHandlerShouldHaveConsumedTheMessage()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            protected void AndTheCredentialServiceShouldHaveBeenUsedToMarkTheCertificateForSelection()
            {
                CredentialService
                    .Verify(o => o.Handle(It.Is<MarkCertificatesForSelectOrDeselectCommand>(command =>
                    command.MemberId == EventToSend.MemberId
                    && command.CredentialIdsForSelect.Any())), Times.Once);
            }
        }

    }
}
