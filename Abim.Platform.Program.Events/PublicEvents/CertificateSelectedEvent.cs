using System;

namespace Abim.Enterprise.Core.ServiceBus.Program
{
    public class CertificateSelectedEvent : ICertificateSelectedEvent
    {
        public Guid MemberId { get; set; }
        public Guid CertificationGuid { get; set; }
        public Guid CredentialGuid { get; set; }
        public DateTime InitialCertDate { get; set; }
    }

    public interface ICertificateSelectedEvent
    {
        Guid MemberId { get; set; }
        Guid CertificationGuid { get; set; }
        Guid CredentialGuid { get; set; }
        DateTime InitialCertDate { get; set; }
    }
}
