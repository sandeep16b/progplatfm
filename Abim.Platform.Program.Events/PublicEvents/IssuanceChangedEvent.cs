using System;

namespace ServiceBus.Events
{
    public class IssuanceChanged : IIssuanceChanged
    {
        public Guid MemberId { get; set; }
        public Guid CredentialGuid { get; set; }
        public string Code { get; set; }
        public string Status { get; set; }
        public string Occurrence { get; set; }
        public DateTime IssuanceDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public bool New { get; set; }
        public bool Cosponsored { get; set; }
        public DateTime ProcessingDate { get; set; }
    }

    public interface IIssuanceChanged
    {
        Guid MemberId { get; set; }
        Guid CredentialGuid { get; set; }
        string Code { get; set; }
        string Status { get; set; }
        string Occurrence { get; set; }
        DateTime IssuanceDate { get; set; }
        DateTime? ExpirationDate { get; set; }
        bool New { get; set; }
        bool Cosponsored { get; set; }
        DateTime ProcessingDate { get; set; }
    }


}
