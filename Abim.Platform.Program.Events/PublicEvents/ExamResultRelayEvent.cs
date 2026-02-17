//using Abim.Enterprise.Core.ServiceBus.Registration.Enums;
using Abim.Platform.Program.Enums;
using System;

namespace Abim.Platform.Program.Events
{
    /// <summary>
    /// This event only used as subsequent event after ExamResultEvent is consumeed.
    /// It is used in this scenario : Product (producer) and Program (consumer) to pass event after Product complete processing original ExamResultEvent 
    /// and Program needs to process only after Product finished it.
    /// </summary>
    public class ExamResultRelayEvent : IExamResultRelayEvent
    {
        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        public Guid RegistrationId { get; set; }

        /// <summary>
        /// Gets or sets the date that the Registration platform consuming the original event from Scoring, and sent out this event.
        /// </summary>
        /// <value>
        /// The processing date.
        /// </value>
        public DateTime ProcessingDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the registration.
        /// </summary>
        /// <value>
        /// The type of the registration.
        /// </value>
        public ExamRegistrationType ExamRegistration { get; set; }
    }

    /// <summary>
    /// IExamResultRelayEvent.
    /// </summary>
    public interface IExamResultRelayEvent
    {

        /// <summary>
        /// Gets or sets the registration identifier.
        /// </summary>
        /// <value>
        /// The registration identifier.
        /// </value>
        Guid RegistrationId { get; set; }

        /// <summary>
        /// Same as in original event IExamResultEvent
        /// Gets or sets the date that the Registration platform consuming the original event from Scoring, and sent out this event.
        /// </summary>
        /// <value>
        /// The processing date.
        /// </value>
        DateTime ProcessingDate { get; set; }

        /// <summary>
        /// Gets or sets the type of the registration.
        /// </summary>
        /// <value>
        /// The type of the registration.
        /// </value>
        ExamRegistrationType ExamRegistration { get; set; }
    }
}
