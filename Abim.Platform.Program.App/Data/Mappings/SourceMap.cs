using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    /// <summary>
    /// SourceMap
    /// </summary>
    public sealed class SourceMap : ClassMap<Source>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceMap"/> class.
        /// </summary>
        public SourceMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.SourceTableName);
            Cache.ReadOnly();

            Id(o => o.Id)
                .Column("SourceId")
                .Not.Nullable();

            Map(o => o.ExternalId)
                .Column("SourceGuid")
                .CustomSqlType("uniqueidentifier")
                .Unique().Not.Nullable();
            
            Map(o => o.Name)
                .CustomSqlType("Nvarchar")
                .Length(255);

            Map(o => o.Code)
                .CustomSqlType("Nvarchar")
                .Length(255)
                .Column("Code")
                .Unique();
            
            Component(o => o.AuditData);
        }
    }
}