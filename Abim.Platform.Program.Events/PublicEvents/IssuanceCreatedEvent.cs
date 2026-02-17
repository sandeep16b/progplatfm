using System;

namespace Abim.Enterprise.Core.ServiceBus.Program
{
    public class IssuanceCreatedEvent: IIssuanceCreatedEvent
    {
        public Guid CredentialId { get; set; }
        public Guid MemberId { get; set; }

        public DateTime IssuanceDate { get; set; }
        public DateTime? ProcessingDate { get; set; }
    }

    public interface IIssuanceCreatedEvent
    {
        Guid CredentialId { get; set; }
        Guid MemberId { get; set; }

        DateTime IssuanceDate { get; set; }
        DateTime? ProcessingDate { get; set; }
    }

}
