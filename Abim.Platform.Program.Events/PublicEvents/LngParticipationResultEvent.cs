using System;

namespace Abim.Platform.Program.Events
{
    /// <summary>
    /// This event only used as subsequent event after ParticipationEvent is consumeed by Registration platform
    /// </summary>
    public class LngParticipationResultEvent : ILngParticipationResultEvent
    {
        /// <summary>
        /// CredentialGuid
        /// </summary>
        public Guid CredentialGuid { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// IsSummativeDecisionYear
        /// </summary>
        public bool IsSummativeDecisionYear { get; set; }

        /// <summary>
        /// MetParticipationStatus
        /// </summary>
        public bool? MetParticipationStatus { get; set; }

        /// <summary>
        /// PassSummativeDecision
        /// </summary>
        public bool? PassSummativeDecision { get; set; }

        /// <summary>
        /// ProcessingDate
        /// </summary>
        public DateTime ProcessingDate { get; set; }
    }

    /// <summary>
    /// ILngParticipationResultRelayEvent.
    /// </summary>
    public interface ILngParticipationResultEvent
    {
        /// <summary>
        /// CredentialGuid
        /// </summary>
        Guid CredentialGuid { get; set; }

        /// <summary>
        /// Year
        /// </summary>
        int Year { get; set; }

        /// <summary>
        /// IsSummativeDecisionYear
        /// </summary>
        bool IsSummativeDecisionYear { get; set; }

        /// <summary>
        /// MetParticipationStatus
        /// </summary>
        bool? MetParticipationStatus { get; set; }

        /// <summary>
        /// PassSummativeDecision
        /// </summary>
        bool? PassSummativeDecision { get; set; }

        /// <summary>
        /// ProcessingDate
        /// </summary>
        DateTime ProcessingDate { get; set; }
    }
}
