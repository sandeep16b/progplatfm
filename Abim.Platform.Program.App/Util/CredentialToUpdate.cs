using Abim.Platform.Program.Resources;
using System;

namespace Abim.Platform.Program.App.Util
{
#pragma warning disable
    public class CredentialToUpdate
    {
        public Guid CredentialId { get; protected set; }

        public MaintenanceStatusType MaintenanceStatus { get; set; }

        public DateTime IssuanceDate { get; set; }

        public CredentialCategoryType CredentialCategory { get; set; }

        public DateTime ProcessingDate { get; set; }

        public CredentialToUpdate(Guid credentialId,
                                        MaintenanceStatusType maintenanceStatus,
                                        DateTime issuanceDate,
                                        CredentialCategoryType credentialCategory,
                                        DateTime processingDate)
        {
            CredentialId = credentialId;
            MaintenanceStatus = maintenanceStatus;
            IssuanceDate = issuanceDate;
            CredentialCategory = credentialCategory;
            ProcessingDate = processingDate;
        }

        public CredentialToUpdate(Guid credentialId,
                        MaintenanceStatusType? maintenanceStatus,
                        DateTime? issuanceDate,
                        CredentialCategoryType? credentialCategory,
                        DateTime? processingDate = null)
        {
            CredentialId = credentialId;
            MaintenanceStatus = maintenanceStatus.Value;
            IssuanceDate = issuanceDate.HasValue ? issuanceDate.Value : DateTime.Now;
            CredentialCategory = credentialCategory.HasValue ? credentialCategory.Value : CredentialCategoryType.Unknown;
            ProcessingDate = processingDate.HasValue ? processingDate.Value : DateTime.Now;
        }
    }
#pragma warning restore
}
