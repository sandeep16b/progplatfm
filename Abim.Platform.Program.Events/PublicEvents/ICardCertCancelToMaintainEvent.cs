using System;

namespace Abim.Enterprise.Core.ServiceBus.Program
{
    public class ICardCertCancelToMaintainEvent : IICardCertCancelToMaintainEvent
    {
        public Guid MemberId { get; set; }
        public DateTime ProcessingDate { get; set; }
    }

    public interface IICardCertCancelToMaintainEvent
    {
        Guid MemberId { get; set; }
        DateTime ProcessingDate { get; set; }
    }

}
