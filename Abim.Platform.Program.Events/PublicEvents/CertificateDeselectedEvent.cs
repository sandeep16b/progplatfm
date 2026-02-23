using System;

namespace Abim.Enterprise.Core.ServiceBus.Program
{
    public class CertificateDeselectedEvent : ICertificateDeselectedEvent
    {
        public Guid MemberId { get; set; }
        public Guid CertificationGuid { get; set; }
        public Guid CredentialGuid { get; set; }
    }

    public interface ICertificateDeselectedEvent
    {
        Guid MemberId { get; set; }
        Guid CertificationGuid { get; set; }
        Guid CredentialGuid { get; set; }
    }
}
