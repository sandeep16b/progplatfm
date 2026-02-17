using System;

namespace ServiceBus.Events
{
    public class CertDeselected : ICertDeselected
    {
        public Guid MemberId { get; set; }
        public Guid CertificationId { get; set; }
        public Guid CredentialId { get; set; }
        public bool Cosponsored { get; set; }
        public string CertificationCode { get; set; }
    }

    public interface ICertDeselected
    {
        Guid MemberId { get; set; }
        Guid CertificationId { get; set; }
        Guid CredentialId { get; set; }
        bool Cosponsored { get; set; }
        string CertificationCode { get; set; }
    }
}
