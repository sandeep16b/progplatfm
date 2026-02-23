using Abim.Enterprise.Core.ServiceBus.Registration;
using Abim.Platform.Program.App.ServiceBus;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Tests.Scenarios.Consumers.Base;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NLog;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using TestStack.BDDfy;

namespace Abim.Platform.Program.Testing.Scenarios.Consumers
{
    [Story(
        AsA = "bus consumer",
        IWant = "to be able to utilize the LKAEnrollmentConsumer Consumer",
        SoThat = "to consume IMemberEnrolledEvent messages"
    )]
    [TestFixture]
    public class LKAEnrollmentConsumerSpec
    {
        [Test]
        [WorkItem(216125)]
        public void Should_Update_Credential_Pathway_When_Valid_Message_Is_Consumed()
        {
            new Should_Update_Credential_Pathway_When_Valid_Message_Is_Consumed_Scenario().BDDfy();
        }

        [Test]
        [WorkItem(216125)]
        public void Should_Not_Update_Credential_Pathway_When_Message_Does_Not_Specify_CredentialId()
        {
            new Should_Not_Update_Credential_Pathway_When_Message_Does_Not_Specify_CredentialId_Scenario().BDDfy();
        }

        #region Scenarios
        private abstract class LKAEnrollmentConsumerScenario : ConsumerScenario
        {
            protected InMemoryTestHarness Harness;
            protected ConsumerTestHarness<LKAEnrollmentConsumer> Consumer;
            protected HandlerTestHarness<IMemberEnrolledEvent> Handler;

            protected Mock<ILogger> Log { get; set; }
            protected Mock<ICredentialService> CredentialService { get; set; }
            protected Mock<IBusControl> BusControl { get; set; }

            protected IMemberEnrolledEvent EventToSend { get; set; }

            protected Func<LKAEnrollmentConsumer> ConsumerFactoryMethod;

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
                Consumer.Consumed.Select<IMemberEnrolledEvent>()
                    .FirstOrDefault()
                    .Exception
                    .Should().BeNull();
            }

            protected void AndTheMessageShouldHaveBeenSentToTheConsumerFromTheEndpoint()
            {
                Harness.Sent.Select<IMemberEnrolledEvent>().Any().Should().BeTrue();
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheHarness()
            {
                Harness.Consumed.Select<IMemberEnrolledEvent>().Any().Should().BeTrue();
            }

        }

        private class Should_Update_Credential_Pathway_When_Valid_Message_Is_Consumed_Scenario : LKAEnrollmentConsumerScenario
        {
            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>();
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new LKAEnrollmentConsumer(CredentialService.Object);

                Consumer = Harness.Consumer<LKAEnrollmentConsumer>(ConsumerFactoryMethod);

                Handler = Harness.Handler<IMemberEnrolledEvent>();
            }

            protected new async Task Teardown()
            {
                await Harness.Stop();
            }

            /// <summary>
            /// Secondary setup
            /// </summary>
            protected override void PostSetup()
            {
                CredentialService
                    .Setup(x => x.Handle(It.IsAny<UpdatePathwayCommand>()))
                    .Returns(Task.FromResult(new UpdatePathwayCommandResult()));

                CredentialService
                    .Setup(x => x.Handle(It.IsAny<MarkCertificatesForSelectOrDeselectCommand>()))
                    .Returns(new MarkCertificatesForSelectOrDeselectCommandResult());

                LogTest.Watch(Log);
            }

            protected void GivenIHaveAValidEvent()
            {
                EventToSend = new MemberEnrolledEvent()
                {
                    MemberId = Guid.NewGuid(), //Do you ever worry that someday we'll run out of Guids? :O
                    CredentialId = Guid.NewGuid()
                };
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheConsumer()
            {
                //Calling Any() below is what will enumerate the consumer and actually invoke the
                //Consume event! But having this in the base class didn't work for some reason -- it only 
                //works correctly from here!
                Consumer.Consumed.Select<IMemberEnrolledEvent>().Any().Should().BeTrue();
            }

            protected void AndTheHandlerShouldHaveConsumedTheMessage()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            protected void AndTheCredentialServiceShouldHaveBeenUsedToUpdateThePathway()
            {
                CredentialService
                    .Verify(o => o.Handle(It.Is<UpdatePathwayCommand>(command => 
                        command.CredentialId == EventToSend.CredentialId
                        && command.Pathway == Resources.PathwayType.LNG
                        && command.Username == "LKAEnrollmentConsumer")), Times.Once);
            }

            protected void AndTheCredentialServiceShouldHaveBeenUsedToMarkTheCertificateForSelection()
            {
                CredentialService
                    .Verify(o => o.Handle(It.Is<MarkCertificatesForSelectOrDeselectCommand>(command =>
                    command.MemberId == EventToSend.MemberId 
                    && command.CredentialIdsForSelect.Any(y => y == EventToSend.CredentialId))), Times.Once);
            }
        }

        private class Should_Not_Update_Credential_Pathway_When_Message_Does_Not_Specify_CredentialId_Scenario : LKAEnrollmentConsumerScenario
        {
            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>(MockBehavior.Strict);
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new LKAEnrollmentConsumer(CredentialService.Object);

                Consumer = Harness.Consumer<LKAEnrollmentConsumer>(ConsumerFactoryMethod);

                Handler = Harness.Handler<IMemberEnrolledEvent>();
            }

            protected new async Task Teardown()
            {
                await Harness.Stop();
            }

            /// <summary>
            /// Secondary setup
            /// </summary>
            protected override void PostSetup()
            {
                CredentialService
                    .Setup(x => x.Handle(It.IsAny<UpdatePathwayCommand>()))
                    .Returns(Task.FromResult(new UpdatePathwayCommandResult()));

                LogTest.Watch(Log);
            }

            protected void GivenIHaveAnEventWithAnEmptyCredentialId()
            {
                EventToSend = new MemberEnrolledEvent()
                {
                    CredentialId = Guid.Empty,
                    MemberId = Guid.NewGuid()
                };
            }

            //The following 2 methods are duplicated from the test above, but don't work correctly 
            //from the base class!
            protected void AndTheMessageShouldHaveBeenConsumedByTheConsumer()
            {
                //Calling Any() below is what will enumerate the consumer and actually invoke the
                //Consume event! But having this in the base class didn't work for some reason -- it only 
                //works correctly from here!
                Consumer.Consumed.Select<IMemberEnrolledEvent>().Any().Should().BeTrue();
            }

            protected void AndTheHandlerShouldHaveConsumedTheMessage()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            public void AndTheCredentialServiceShouldNOTBeUsedToUpdateTheCredentialPathway()
            {
                CredentialService
                    .Verify(o => o.Handle(It.IsAny<UpdatePathwayCommand>()), Times.Never);
            }
        }

        #endregion Scenarios
    }
}
