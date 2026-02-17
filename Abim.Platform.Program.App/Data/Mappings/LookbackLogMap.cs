using Abim.Platform.Program.App.Data.Mappings.Enumerations;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class LookbackLogMap : ClassMap<LookbackLog>
    {
        /// <summary>
        /// Create a new instance of the LookbackLogMap
        /// </summary>
        public LookbackLogMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.LookbackLogTableName);

            Id(o => o.Id).Column("LookbackLogId").Not.Nullable();

            Map(o => o.Reason)
                .Column("LookbackReason")
                .CustomType<LookbackReasonTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.Action)
                .Column("LookbackAction")
                .CustomType<LookbackActionTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.Status)
                .Column("LookbackStatus")
                .CustomType<LookbackStatusTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            References(o => o.Credential)
                .Column("CredentialGuid").PropertyRef(x => x.ExternalId)
                .Cascade.None()
                .Not.Nullable();

            Map(o => o.IsPendingAction)
                .Column("IsPendingAction")
                .Not.Nullable();

            Map(o => o.LogDate)
                .Column("LookbackLogDate")
                .Not.Nullable();

            Component(o => o.AuditData);
        }
    }
}