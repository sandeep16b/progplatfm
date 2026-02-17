using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using System;

namespace Abim.Platform.Program.Tests.Setup.CommandBuilders.Credential
{
    public class AddIssuanceCommandBuilder
    {
        private AddIssuanceCommand command;

        public AddIssuanceCommandBuilder()
        {
            Reset();
        }

        public void Reset()
        {
            command = new AddIssuanceCommand();
        }

        public AddIssuanceCommandBuilder WithCredential(Guid credentialId)
        {
            command.CredentialId = credentialId;
            return this;
        }

        public AddIssuanceCommandBuilder WithIssuanceDate(DateTime issuanceDate)
        {
            command.IssuanceDate = issuanceDate;
            return this;
        }

        public AddIssuanceCommandBuilder WithEffectiveDate(DateTime effectiveDate)
        {
            command.EffectiveDate = effectiveDate;
            return this;
        }

        public AddIssuanceCommandBuilder WithDuration(DurationType duration)
        {
            command.Duration = duration;
            return this;
        }

        public AddIssuanceCommandBuilder WithExpirationDate(DateTime? expirationDate)
        {
            command.ExpirationDate = expirationDate;
            return this;
        }

        public AddIssuanceCommandBuilder WithMaintenanceRequirement(MaintenanceRequirementType maintenanceReq)
        {
            command.MaintenanceRequirement = maintenanceReq;
            return this;
        }

        public AddIssuanceCommandBuilder WithMaintenanceStatus(MaintenanceStatusType maintenanceStatus)
        {
            command.MaintenanceStatus = maintenanceStatus;
            return this;
        }

        public AddIssuanceCommandBuilder WithIssuanceStatus(IssuanceStatusType issuanceStatus)
        {
            command.IssuanceStatus = issuanceStatus;
            return this;
        }

        public AddIssuanceCommandBuilder WithOccurrence(OccurrenceType occurrence)
        {
            command.Occurrence = occurrence;
            return this;
        }

        public AddIssuanceCommandBuilder WithSourceId(Guid sourceId)
        {
            command.SourceId = sourceId;
            return this;
        }

        public AddIssuanceCommandBuilder WithUserInfo(UserInfo userInfo)
        {
            command.UserInfo = userInfo;
            return this;
        }

        public AddIssuanceCommand Build()
        {
            return command;
        }
    }
}
