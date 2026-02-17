using Abim.Platform.Program.App.Data.Mappings.Enumerations;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    /// <summary>
    /// LookbackDateLogMap
    /// </summary>
    public sealed class LookbackDateLogMap : ClassMap<LookbackDateLog>
    {
        /// <summary>
        /// Create a new instance of the LookbackLogMap
        /// </summary>
        public LookbackDateLogMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.LookbackDateLogTableName);

            Id(o => o.Id).Column("LookbackDateLogId").Not.Nullable();

            Map(o => o.ChangedDate)
               .Column("ChangedDate")
               .Not.Nullable();

            Map(o => o.MemberGuid)
                .CustomSqlType("uniqueidentifier")
                .Column("MemberGuid")
                .Not.Nullable();

            Map(o => o.LookbackDate)
                .Column("LookbackDate")
                .CustomType<LookbackDateTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.NewValue)
               .Column("NewValue")
               .Nullable();

            Map(o => o.OldValue)
               .Column("OldValue")
               .Nullable();

            Component(o => o.AuditData);
        }
    }
}