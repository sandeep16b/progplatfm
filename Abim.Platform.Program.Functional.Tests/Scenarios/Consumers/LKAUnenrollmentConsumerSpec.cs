using Abim.Enterprise.Core.ServiceBus.Registration;
using Abim.Platform.Program.App.ServiceBus;
using Abim.Platform.Program.App.Services;
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
        IWant = "to be able to utilize the LKAUnenrollmentConsumer Consumer",
        SoThat = "to consume IMemberUnEnrolledEvent messages"
    )]
    [TestFixture]
    public class LKAUnenrollmentConsumerSpec
    {

        [Test]
        [WorkItem(216125)]
        [WorkItem(218848)]
        // Left this obsolete test below which can be used as a starting point if we decide to add new functionality to this consumer.
        public void Should_Not_Update_Credential_Pathway_When_Message_Does_Not_Specify_CredentialId()
        {
            new Should_Not_Update_Credential_Pathway_When_Message_Does_Not_Specify_CredentialId_Scenario().BDDfy();
        }

        #region Scenarios
        private abstract class LKAUnenrollmentConsumerScenario : ConsumerScenario
        {
            protected InMemoryTestHarness Harness;
            protected ConsumerTestHarness<LKAUnenrollmentConsumer> Consumer;
            protected HandlerTestHarness<IMemberUnEnrolledEvent> Handler;

            protected Mock<ILogger> Log { get; set; }
            protected Mock<ICredentialService> CredentialService { get; set; }
            protected Mock<IBusControl> BusControl { get; set; }

            protected IMemberUnEnrolledEvent EventToSend { get; set; }

            protected Func<LKAUnenrollmentConsumer> ConsumerFactoryMethod;

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
                Consumer.Consumed.Select<IMemberUnEnrolledEvent>()
                    .FirstOrDefault()
                    .Exception
                    .Should().BeNull();
            }

            protected void AndTheMessageShouldHaveBeenSentToTheConsumerFromTheEndpoint()
            {
                Harness.Sent.Select<IMemberUnEnrolledEvent>().Any().Should().BeTrue();
            }

            protected void AndTheMessageShouldHaveBeenConsumedByTheHarness()
            {
                Harness.Consumed.Select<IMemberUnEnrolledEvent>().Any().Should().BeTrue();
            }

        }

        private class Should_Not_Update_Credential_Pathway_When_Message_Does_Not_Specify_CredentialId_Scenario : LKAUnenrollmentConsumerScenario
        {
            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>(MockBehavior.Strict);
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new LKAUnenrollmentConsumer(CredentialService.Object);

                Consumer = Harness.Consumer<LKAUnenrollmentConsumer>(ConsumerFactoryMethod);

                Handler = Harness.Handler<IMemberUnEnrolledEvent>();
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
                //CredentialService
                //    .Setup(x => x.Handle(It.IsAny<UpdatePathwayCommand>()))
                //    .Returns(Task.FromResult(new UpdatePathwayCommandResult()));

                LogTest.Watch(Log);
            }

            protected void GivenIHaveAnEventWithAnEmptyCredentialId()
            {
                EventToSend = new MemberUnEnrolledEvent()
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
                Consumer.Consumed.Select<IMemberUnEnrolledEvent>().Any().Should().BeTrue();
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
