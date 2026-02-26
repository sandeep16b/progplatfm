using Abim.Platform.Program.App.Data.Mappings.Enumerations;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    /// <summary>
    /// IssuanceMap
    /// </summary>
    /// <returns></returns>
    public sealed class IssuanceMap : ClassMap<Issuance>
    {
        /// <summary>
        /// IssuanceMap
        /// </summary>
        /// <returns></returns>
        public IssuanceMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.IssuanceTableName);
            Cache.ReadWrite();

            Id(o => o.Id).Column("IssuanceId").Not.Nullable();

            Map(o => o.IssuanceDate)
                .Column(Constants.Database.IssuanceTable.IssuanceDate)
                .CustomSqlType("datetime")
                .Not.Nullable();

            Map(o => o.EffectiveDate)
                .Column("EffectiveDate")
                .CustomSqlType("datetime")
                .Not.Nullable();

            Map(o => o.ExpirationDate)
                .Column(Constants.Database.IssuanceTable.ExpirationDate)
                .CustomSqlType("datetime")
                .Nullable();

            Map(o => o.ExpiredDate)
                .Column(Constants.Database.IssuanceTable.ExpiredDate)
                .CustomSqlType("datetime")
                .Nullable();

            Map(o => o.ScheduledUpdate)
                .Column(Constants.Database.IssuanceTable.ScheduledUpdate)
                .CustomSqlType("datetime")
                .Nullable();

            Map(o => o.UnderReview)
                .Column(Constants.Database.IssuanceTable.UnderReview);

            Map(o => o.Duration).Column("Duration")
                .CustomType<DurationTypeMapping>()
                .CustomSqlType("nvarchar").Length(50).Not.Nullable();

            Map(o => o.MaintenanceRequirement)
                .Column("MaintenanceRequirement")
                .CustomType<MaintenanceRequirementTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.MaintenanceStatus)
                .Column("MaintenanceStatus").CustomType<MaintenanceStatusTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.Occurrence)
                .Column("Occurrence").CustomType<OccurrenceTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.IssuanceStatus)
                .Column("IssuanceStatus")
                .CustomType<IssuanceStatusTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.RegistrationGuid)
                .Column("RegistrationGuid")
                .Not.Nullable();

            Map(o => o.DeselectionSubmittedDate)
                .Column(Constants.Database.IssuanceTable.DeselectionSubmittedDate)
                .Nullable();

            Map(o => o.DeselectionEffectiveDate)
                .Column(Constants.Database.IssuanceTable.DeselectionEffectiveDate)
                .Nullable();

            Map(o => o.DeselectionProcessedDate)
                .Column(Constants.Database.IssuanceTable.DeSelectionProcessedDate)
                .Nullable();

            Map(o => o.DeselectionType)
                .Column("DeselectionType").CustomType<DeselectionTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Nullable();

            References(o => o.Source, "SourceId")
                .Cascade.None()
                .Not.Nullable();

            References(o => o.Credential)
                .Column("CredentialId")
                .Cascade.SaveUpdate();

            Component(o => o.AuditData);
        }
    }
}