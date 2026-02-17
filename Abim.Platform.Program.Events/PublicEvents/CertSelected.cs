using System;

namespace ServiceBus.Events
{
    public class CertSelected : ICertSelected
    {
        public Guid MemberId { get; set; }
        public Guid CertificationId { get; set; }
        public Guid CredentialId { get; set; }
        public string CertificationCode { get; set; }
        public bool Cosponsored { get; set; }
        public DateTime InitialCertificationDate { get; set; }
    }

    public interface ICertSelected
    {
        Guid MemberId { get; set; }
        Guid CertificationId { get; set; }
        Guid CredentialId { get; set; }
        string CertificationCode { get; set; }
        bool Cosponsored { get; set; }
        DateTime InitialCertificationDate { get; set; }
    }
}