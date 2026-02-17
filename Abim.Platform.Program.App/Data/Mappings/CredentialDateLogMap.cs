using Abim.Platform.Program.App.Data.Mappings.Enumerations;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    internal sealed class CredentialDateLogMap : ClassMap<CredentialDateLog>
    {
        public CredentialDateLogMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.CredentialDateLogTableName);

            Id(o => o.Id).Column("CredentialDateLogId").Not.Nullable();

            References(o => o.Credential)
                .Column("CredentialGuid").PropertyRef(x => x.ExternalId)
                .Cascade.None()
                .Not.Nullable();

            Map(x => x.ChangedDate);

            Map(o => o.DateType)
                .Column("CredentialDate")
                .CustomType<CredentialDateTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(x => x.NewValue);
            Map(x => x.OldValue);

            Component(o => o.AuditData);
        }
    }
}