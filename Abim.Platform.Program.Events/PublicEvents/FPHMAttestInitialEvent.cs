using System;

namespace Abim.Enterprise.Core.ServiceBus.Program
{

    // It is ONLY used to send event after CreateActivityForAttestationCompleteCommand
    // ( from Product to Program per PBI: 92099 )
    public class FPHMAttestInitialEvent : IFPHMAttestInitialEvent
    {
        public Guid MemberId { get; set; }
        public DateTime ProcessingDate { get; set; }
    }

    public interface IFPHMAttestInitialEvent
    {
        Guid MemberId { get; set; }
        DateTime ProcessingDate { get; set; }
    }


}
