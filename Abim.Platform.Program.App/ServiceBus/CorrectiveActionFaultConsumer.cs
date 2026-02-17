using Abim.Enterprise.Core.ServiceBus.Product;
using Abim.Enterprise.Core.ServiceBus.Program;
using MassTransit;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// It might need to place into another platform???
    /// Consumes Corrective Action Fault Events
    /// </summary>
    public class CorrectiveActionFaultConsumer: IConsumer<Fault<IActivityCompletedEvent>>, IConsumer<Fault<IActivityCreatedEvent>>, IConsumer<Fault<IIssuanceCreatedEvent>>
    {
        /// <summary>
        /// After all of the various retry policies have executed, the bus will generate a fault which you can consume here
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<Fault<IActivityCompletedEvent>> context)
        {
            var originalMessage = context.Message.Message;
            var exceptions = context.Message.Exceptions;

            //send email or something interesting to notify of failure
            await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task Consume(ConsumeContext<Fault<IActivityCreatedEvent>> context)
        {
            var originalMessage = context.Message.Message;
            var exceptions = context.Message.Exceptions;

            //send email or something interesting to notify of failure
            await Task.FromResult<object>(null);
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task Consume(ConsumeContext<Fault<IIssuanceCreatedEvent>> context)
        {
            var originalMessage = context.Message.Message;
            var exceptions = context.Message.Exceptions;

            //send email or something interesting to notify of failure
            await Task.FromResult<object>(null);
        }

    }

}
