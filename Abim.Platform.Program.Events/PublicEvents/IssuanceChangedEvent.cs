using System;

namespace Abim.Enterprise.Core.ServiceBus.Program
{

    public class IssuanceChangedEvent : IIssuanceChangedEvent
    {
        public Guid MemberId { get; set; }
        public string CertificationCode { get; set; }
        public Guid CertificationGuid { get; set; }
        public DateTime IssuanceDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string IssuanceStatus { get; set; }
        public bool IsNew { get; set; }
        public string Occurrence { get; set; } 
        public DateTime ProcessingDate { get; set; }
    }

    public interface IIssuanceChangedEvent
    {
        Guid MemberId { get; set; }
        string CertificationCode { get; set; }
        Guid CertificationGuid { get; set; }
        DateTime IssuanceDate { get; set; }
        DateTime? ExpirationDate { get; set; }
        string	IssuanceStatus { get; set; }
        bool IsNew { get; set; }
        string Occurrence { get; set; }
        DateTime ProcessingDate { get; set; }
    }


}
