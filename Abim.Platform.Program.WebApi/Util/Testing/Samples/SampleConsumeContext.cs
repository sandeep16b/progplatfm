using GreenPipes;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Util.Testing
{
    /// <summary>
    /// A Sample ConsumeContext, for testing purposes
    /// </summary>
    /// <typeparam name="TEvent">The type of the event.</typeparam>
    /// <seealso cref="MassTransit.ConsumeContext{TEvent}" />
    public class SampleConsumeContext<TEvent> : ConsumeContext<TEvent>
        where TEvent : class
    {
        #region Properties

        /// <summary>
        /// Gets or sets the event.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        TEvent Data { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SampleConsumeContext{TEvent}"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public SampleConsumeContext(TEvent data)
        {
            Data = data;
        }

        #endregion

        #region Actual Implementation
        
        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        public TEvent Message
        {
            get
            {
                return Data;
            }
        }
        
        #endregion
        
        #region Default Implementation
        
        /// <summary>
        /// Used to cancel the execution of the context
        /// </summary>
        public CancellationToken CancellationToken
        {
            get
            {
                return default(CancellationToken);
            }
        }

        /// <summary>
        /// An awaitable task that is completed once the consume context is completed
        /// </summary>
        public Task CompleteTask
        {
            get
            {
                return Task.FromResult(false);
            }
        }

        /// <summary>
        /// The conversationId of the message, which is copied and carried throughout the message
        /// flow by the infrastructure.
        /// </summary>
        public Guid? ConversationId
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// If the message implements the CorrelatedBy(Guid) interface, this field should be
        /// populated by default to match that value. It can, of course, be overwritten with
        /// something else.
        /// </summary>
        public Guid? CorrelationId
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The destination address of the message
        /// </summary>
        public Uri DestinationAddress
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The expiration time of the message if it is not intended to last forever.
        /// </summary>
        public DateTime? ExpirationTime
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The fault addres to which fault events should be sent if the message consumer faults
        /// </summary>
        public Uri FaultAddress
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Additional application-specific headers that are added to the message by the application
        /// or by features within MassTransit, such as when a message is moved to an error queue.
        /// </summary>
        public Headers Headers
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The host information of the message producer. This may not be present if the message was sent
        /// from an earlier version of MassTransit.
        /// </summary>
        public HostInfo Host
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// If this message was produced within the context of a previous message, the CorrelationId
        /// of the message is contained in this property. If the message was produced from a saga
        /// instance, the CorrelationId of the saga is used.
        /// </summary>
        public Guid? InitiatorId
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The messageId assigned to the message when it was initially Sent. This is different
        /// than the transport MessageId, which is only for the Transport.
        /// </summary>
        public Guid? MessageId
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The original receive context
        /// </summary>
        public ReceiveContext ReceiveContext
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// If the message is a request, or related to a request, such as a response or a fault,
        /// this contains the requestId.
        /// </summary>
        public Guid? RequestId
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The response address to which responses to the request should be sent
        /// </summary>
        public Uri ResponseAddress
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// The address of the message producer that sent the message
        /// </summary>
        public Uri SourceAddress
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the supported message types from the message
        /// </summary>
        public IEnumerable<string> SupportedMessageTypes
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Connects the publish observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>
        public ConnectHandle ConnectPublishObserver(IPublishObserver observer)
        {
                return null;
        }

        /// <summary>
        /// Connects the send observer.
        /// </summary>
        /// <param name="observer">The observer.</param>
        /// <returns></returns>
        public ConnectHandle ConnectSendObserver(ISendObserver observer)
        {
                return null;
        }

        /// <summary>
        /// Returns an existing payload or creates the payload using the factory method provided
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="payloadFactory">The payload factory is the payload is not present</param>
        /// <returns>
        /// The payload
        /// </returns>
        public TPayload GetOrAddPayload<TPayload>(PayloadFactory<TPayload> payloadFactory)
            where TPayload : class
        {
                return null;
        }

        /// <summary>
        /// Return the send endpoint for the specified address
        /// </summary>
        /// <param name="address">The endpoint address</param>
        /// <returns>
        /// The send endpoint
        /// </returns>
        public Task<ISendEndpoint> GetSendEndpoint(Uri address)
        {
                return null;
        }

        /// <summary>
        /// Returns true if the specified message type is contained in the serialized message
        /// </summary>
        /// <param name="messageType"></param>
        /// <returns></returns>
        public bool HasMessageType(Type messageType)
        {
                return false;
        }

        /// <summary>
        /// Checks if a payload is present in the context
        /// </summary>
        /// <param name="payloadType"></param>
        /// <returns></returns>
        public bool HasPayloadType(Type payloadType)
        {
                return false;
        }

        /// <summary>
        /// Notify that the message has been consumed -- note that this is internal, and should not be called by a consumer
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="consumerType">The consumer type</param>
        /// <returns></returns>
        public Task NotifyConsumed(TimeSpan duration, string consumerType)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Notify that the message has been consumed -- note that this is internal, and should not be called by a consumer.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType">The consumer type</param>
        /// <returns></returns>
        public Task NotifyConsumed<T>(ConsumeContext<T> context,TimeSpan duration, string consumerType)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Notify that a fault occurred during message consumption -- note that this is internal, and should not be called by a consumer
        /// </summary>
        /// <param name="duration"></param>
        /// <param name="consumerType"></param>
        /// <param name="exception"></param>
        /// <returns></returns>
        public Task NotifyFaulted(TimeSpan duration, string consumerType, Exception exception)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Notify that a message consumer has faulted -- note that this is internal, and should not be called by a consumer
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <param name="duration"></param>
        /// <param name="consumerType">The message consumer type</param>
        /// <param name="exception">The exception that occurred</param>
        /// <returns></returns>
        public Task NotifyFaulted<T>(ConsumeContext<T> context,TimeSpan duration, string consumerType, Exception exception)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish(object message, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish(object message,Type messageType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish(object message, IPipe<PublishContext> publishPipe,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Publishes an object as a message, using the message type specified. If the object cannot be cast
        /// to the specified message type, an exception will be thrown.
        /// </summary>
        /// <param name="message">The message object</param>
        /// <param name="messageType">The type of the message (use message.GetType() if desired)</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish(object message,Type messageType,IPipe<PublishContext> publishPipe,
                CancellationToken cancellationToken = default(CancellationToken))
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <see cref="!:IBus.Publish&lt;T&gt;(T,CancellationToken)" />: this is a "dynamically"
        /// typed overload - give it an interface as its type parameter,
        /// and a loosely typed dictionary of values and the MassTransit
        /// underlying infrastructure will populate an object instance
        /// with the passed values. It actually does this with DynamicProxy
        /// in the background.
        /// </summary>
        /// <typeparam name="T">The type of the interface or
        /// non-sealed class with all-virtual members.</typeparam>
        /// <param name="values">The dictionary of values to place in the
        /// object instance to implement the interface.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish<T>(object values, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <para>Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.</para>
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish<T>(T message, CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <see cref="!:IBus.Publish&lt;T&gt;(T,CancellationToken)" />: this
        /// overload further takes an action; it allows you to set <see cref="T:MassTransit.PublishContext" />
        /// meta-data. Also <see cref="!:IBus.Publish&lt;T&gt;(T,CancellationToken)" />.
        /// </summary>
        /// <typeparam name="T">The type of the message to publish</typeparam>
        /// <param name="values">The dictionary of values to become hydrated and
        /// published under the type of the interface.</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish<T>(object values,IPipe<PublishContext<T>> publishPipe,
                CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <see cref="!:IBus.Publish&lt;T&gt;(T,CancellationToken)" />: this
        /// overload further takes an action; it allows you to set <see cref="T:MassTransit.PublishContext" />
        /// meta-data. Also <see cref="!:IBus.Publish&lt;T&gt;(T,CancellationToken)" />.
        /// </summary>
        /// <typeparam name="T">The type of the message to publish</typeparam>
        /// <param name="values">The dictionary of values to become hydrated and
        /// published under the type of the interface.</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish<T>(object values,IPipe<PublishContext> publishPipe,
                CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <para>Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.</para>
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish<T>(T message, IPipe<PublishContext> publishPipe,
                CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// <para>Publishes a message to all subscribed consumers for the message type as specified
        /// by the generic parameter. The second parameter allows the caller to customize the
        /// outgoing publish context and set things like headers on the message.</para>
        /// <para>
        /// Read up on publishing: http://readthedocs.org/docs/masstransit/en/latest/overview/publishing.html
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the message</typeparam>
        /// <param name="message">The messages to be published</param>
        /// <param name="publishPipe"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public Task Publish<T>(T message, IPipe<PublishContext<T>> publishPipe,
                CancellationToken cancellationToken = default(CancellationToken))
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Adds a response to the message being consumed, which will be sent once the consumer
        /// has completed. The message is not acknowledged until the response is acknowledged.
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        public void Respond<T>(T message) where T : class
        {
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <returns></returns>
        public Task RespondAsync(object message)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="sendPipe"></param>
        /// <returns></returns>
        public Task RespondAsync(object message, IPipe<SendContext> sendPipe)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="messageType">The message type to send</param>
        /// <returns></returns>
        public Task RespondAsync(object message,Type messageType)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="messageType">The message type to send</param>
        /// <param name="sendPipe"></param>
        /// <returns></returns>
        public Task RespondAsync(object message,Type messageType,IPipe<SendContext> sendPipe)
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="values">The values for the message properties</param>
        /// <returns></returns>
        public Task RespondAsync<T>(object values)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        /// <returns></returns>
        public Task RespondAsync<T>(T message)
            where T : class 
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="values">The values for the message properties</param>
        /// <param name="sendPipe"></param>
        /// <returns></returns>
        public Task RespondAsync<T>(object values,IPipe<SendContext> sendPipe)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        /// <param name="sendPipe">The pipe used to customize the response send context</param>
        /// <returns></returns>
        public Task RespondAsync<T>(T message, IPipe<SendContext> sendPipe)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="values">The values for the message properties</param>
        /// <param name="sendPipe"></param>
        /// <returns></returns>
        public Task RespondAsync<T>(object values,IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Responds to the current message immediately, returning the Task for the
        /// sending message. The caller may choose to await the response to ensure it was sent, or
        /// allow the framework to wait for it (which will happen automatically before the message is acked)
        /// </summary>
        /// <typeparam name="T">The type of the message to respond with.</typeparam>
        /// <param name="message">The message to send in response</param>
        /// <param name="sendPipe">The pipe used to customize the response send context</param>
        /// <returns></returns>
        public Task RespondAsync<T>(T message, IPipe<SendContext<T>> sendPipe)
            where T : class
        {
            return Task.FromResult(false);
        }

        /// <summary>
        /// Returns the specified message type if available, otherwise returns false
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="consumeContext"></param>
        /// <returns></returns>
        public bool TryGetMessage<T>(out ConsumeContext<T> consumeContext)
            where T : class
        {
            consumeContext = null;
            return false;
        }

        /// <summary>
        /// Retrieves a payload from the pipe context
        /// </summary>
        /// <typeparam name="TPayload">The payload type</typeparam>
        /// <param name="payload">The payload</param>
        /// <returns></returns>
        public bool TryGetPayload<TPayload>(out TPayload payload)
            where TPayload : class
        {
            payload = null;
            return false;
        }

        #endregion
    }
}
