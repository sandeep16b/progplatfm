using Abim.Platform.Program.App.ServiceBus;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Events;
using Abim.Platform.Program.Tests.Scenarios.Consumers.Base;
using FluentAssertions;
using MassTransit;
using MassTransit.Testing;
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
        IWant = "to be able to utilize the LngParticipationResult Consumer",
        SoThat = "to consume ILngParticipationResultEvent messages"
    )]
    [TestFixture]
    public class LngParticipationResultConsumerSpec
    {
        [Test]
        public void Should_Call_CredentialService_HandleUpdateCredentialAssessmentDueDate_Sucess()
        {
            new Should_Call_CredentialService_HandleUpdateCredentialAssessmentDueDate_Sucess_Scenario().BDDfy();
        }

        [Test]
        public void Should_Not_Call_CredentialService_HandleUpdateCredentialAssessmentDueDate_Failure()
        {
            new Should_Not_Call_CredentialService_HandleUpdateCredentialAssessmentDueDate_Failure_Scenario().BDDfy();
        }

        #region Scenarios

        private abstract class LngPaticipationResultConsumerScenario : ConsumerScenario
        {
            protected InMemoryTestHarness Harness;
            protected ConsumerTestHarness<LngParticipationResultConsumer> Consumer;
            protected HandlerTestHarness<ILngParticipationResultEvent> Handler;

            protected Mock<ILogger> Log { get; set; }
            protected Mock<ICredentialService> CredentialService { get; set; }
            protected Mock<IBusControl> BusControl { get; set; }

            protected LngParticipationResultEvent EventToSend { get; set; }

            protected Func<LngParticipationResultConsumer> ConsumerFactoryMethod;
        }

        private class Should_Call_CredentialService_HandleUpdateCredentialAssessmentDueDate_Sucess_Scenario : LngPaticipationResultConsumerScenario
        {
            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>();
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new LngParticipationResultConsumer(CredentialService.Object);

                Consumer = Harness.Consumer<LngParticipationResultConsumer>(ConsumerFactoryMethod);

                Handler = Harness.Handler<ILngParticipationResultEvent>();
            }

            public new async Task Teardown()
            {
                await Harness.Stop();
            }

            /// <summary>
            /// Secondary setup
            /// </summary>
            protected override void PostSetup()
            {
                CredentialService
                    .Setup(x => x.Handle(It.IsAny<UpdateLngAssessmentDueDateCommand>()))
                    .Returns(Task.FromResult(new UpdateLngAssessmentDueDateCommandResult()));

                LogTest.Watch(Log);
            }

            public void GivenIInputEventToPublish()
            {
                EventToSend = new LngParticipationResultEvent()
                {
                    CredentialGuid = Guid.NewGuid(),
                    Year=2022,
                    MetParticipationStatus=true,
                    PassSummativeDecision=null
                };
            }

            public async Task WhenISendEventToEndPoint()
            {
                await Harness.Start();
                try
                {
                    await Harness.InputQueueSendEndpoint.Send(EventToSend, context => context.ResponseAddress = Harness.BusAddress);
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
            public void AndThenShouldHaveNoExceptionInConsumer()
            {
                Consumer.Consumed.Select<ILngParticipationResultEvent>().FirstOrDefault().Exception.Should().BeNull();
            }

            public void AndThenSendTheMessageToTheConsumer()
            {
                Harness.Sent.Select<ILngParticipationResultEvent>().Any().Should().BeTrue();
            }

            public void AndThenMessageShouldReceiveTheMessageTypeOfIUpdateLngAssessmentDueDateEvent()
            {
                Harness.Consumed.Select<ILngParticipationResultEvent>().Any().Should().BeTrue();
            }

            public void AndThenShouldHaveCalledTheConsumerMethod()
            {
                Consumer.Consumed.Select<ILngParticipationResultEvent>().Any().Should().BeTrue();
            }

            public void AndThenHandlerShouldConsumed()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            public void AndThenCredentialServiceShouldCallHandleUpdateLngAssessmentDueDateCommandOnce()
            {
                CredentialService
                    .Verify(o => o.Handle(It.IsAny<UpdateLngAssessmentDueDateCommand>()), Times.Once);

            }
        }
       
        private class Should_Not_Call_CredentialService_HandleUpdateCredentialAssessmentDueDate_Failure_Scenario : LngPaticipationResultConsumerScenario
        {
            protected override void PreSetup()
            {
                Harness = new InMemoryTestHarness();

                Log = new Mock<ILogger>();
                CredentialService = new Mock<ICredentialService>(MockBehavior.Strict);
                BusControl = new Mock<IBusControl>();

                ConsumerFactoryMethod = () => new LngParticipationResultConsumer(CredentialService.Object);

                Consumer = Harness.Consumer<LngParticipationResultConsumer>(ConsumerFactoryMethod);

                Handler = Harness.Handler<ILngParticipationResultEvent>();
            }

            public new async Task Teardown()
            {
                await Harness.Stop();
            }

            /// <summary>
            /// Secondary setup
            /// </summary>
            protected override void PostSetup()
            {
                CredentialService
                    .Setup(x => x.Handle(It.IsAny<UpdateLngAssessmentDueDateCommand>()))
                    .Returns(Task.FromResult(new UpdateLngAssessmentDueDateCommandResult()));

                LogTest.Watch(Log);
            }

            public void GivenIInputEventToPublish()
            {
                EventToSend = new LngParticipationResultEvent()
                {
                    CredentialGuid = Guid.Empty,
                    Year = 2022,
                    MetParticipationStatus = true,
                    PassSummativeDecision = null
                };
            }

            public async Task WhenISendEventToEndPoint()
            {
                await Harness.Start();
                try
                {
                    await Harness.InputQueueSendEndpoint.Send(EventToSend, context => context.ResponseAddress = Harness.BusAddress);
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
            public void AndThenShouldHaveNoExceptionInConsumer()
            {
                Consumer.Consumed.Select<ILngParticipationResultEvent>().FirstOrDefault().Exception.Should().BeNull();
            }

            public void AndThenSendTheMessageToTheConsumer()
            {
                Harness.Sent.Select<ILngParticipationResultEvent>().Any().Should().BeTrue();
            }

            public void AndThenMessageShouldReceiveTheMessageTypeOfIUpdateLngAssessmentDueDateEvent()
            {
                Harness.Consumed.Select<ILngParticipationResultEvent>().Any().Should().BeTrue();
            }

            public void AndThenShouldHaveCalledTheConsumerMethod()
            {
                Consumer.Consumed.Select<ILngParticipationResultEvent>().Any().Should().BeTrue();
            }

            public void AndThenHandlerShouldConsumed()
            {
                Handler.Consumed.Select().Any().Should().BeTrue();
            }

            public void AndThenCredentialServiceShouldNOTCallHandleUpdateLngAssessmentDueDateEventCommand()
            {
                CredentialService
                    .Verify(o => o.Handle(It.IsAny<UpdateLngAssessmentDueDateCommand>()), Times.Never);

            }
        }
        
        #endregion Scenarios
    }
}
