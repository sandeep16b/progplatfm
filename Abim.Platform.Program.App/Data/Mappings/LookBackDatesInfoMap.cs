using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    internal sealed class LookBackDatesInfoMap : ClassMap<LookBackDatesInfo>
    {
        public LookBackDatesInfoMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.LookBackDatesInfoTableName);

            Id(o => o.Id).Column("LookbackDateInfoId")
               .Not.Nullable();

            Map(o => o.ExternalId)
            .Column("MemberId")
            .CustomSqlType("uniqueidentifier")
            .Unique().Not.Nullable();

            Map(o => o.Lookback2YearStartDate)
                .Column("Lookback2YearStartDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Map(o => o.Lookback2YearEndDate)
                .Column("Lookback2YearEndDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Map(o => o.Lookback5YearStartDate)
                .Column("Lookback5YearStartDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Map(o => o.Lookback5YearEndDate)
                .Column("Lookback5YearEndDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Component(o => o.AuditData);
        }
    }
}