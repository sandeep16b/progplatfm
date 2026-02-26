using Abim.Platform.Program.App.Data.Mappings.Enumerations;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    /// <summary>
    /// 
    /// </summary>
    internal sealed class CredentialMap : ClassMap<Credential>
    {
        public CredentialMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.CredentialTableName);

            Id(o => o.Id)
                .Column("CredentialId")
                .Not.Nullable();

            Map(o => o.ExternalId)
                .Column("CredentialGuid")
                .CustomSqlType("uniqueidentifier")
                .Unique().Not.Nullable();
            
            Map(o => o.MemberId)
                .CustomSqlType("uniqueidentifier")
                .Column("MemberId")
                .Not.Nullable();

            Map(o => o.Pathway)
                .CustomType<PathwayTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.ExamFailCount)
                .Not.Nullable();

            Map(o => o.Type)
                .CustomType<CredentialTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();

            Map(o => o.IsActive)
                .Column("IsActive")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.ForcedPathway)
                .Column("ForcedPathway")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.AssessmentMet)
                .Column("AssessmentMet")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.AssessmentMetDate)
                .Access
                .CamelCaseField()
                .Column("AssessmentMetDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Map(o => o.LookbackDate)
                .Column("LookbackDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Map(o => o.SkippedExamLookbackDate)
                .Column("SkippedExamLookbackDate")
                .CustomSqlType("datetime2")
                .Nullable();

            Map(o => o.ExamDueDate)
                .Access
                .CamelCaseField()
                .Column("ExamDueDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            Map(o => o.DisplayExamDueDate)
                .Access
                .CamelCaseField()
                .Column("DisplayExamDueDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            Map(o => o.MOCExamDueDate)
                .Access
                .LowerCaseField()
                .Column("MOCExamDueDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            Map(o => o.KCIExamDueDate)
                .Access
                .LowerCaseField()
                .Column("KCIExamDueDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            Map(o => o.GracePeriodStartDate)
                .Access
                .CamelCaseField()
                .Column("GracePeriodStartDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            Map(o => o.GracePeriodEndDate)
                .Access
                .CamelCaseField()
                .Column("GracePeriodEndDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            References(o => o.Certification)
                .Column("CertificationId")
                .Cascade.None()
                .Not.Nullable();
            //ar@10/25/2015: Temporary removed until solution is found: .Fetch.Join();    .Fetch.Join();

            Map(o => o.ReAttestationDueDate)
                .Column("ReAttestationDueDate").CustomSqlType("datetime2").Nullable();

            Map(o => o.SelectedToMaintain)
                .Column("SelectedToMaintain")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.WithdrawnDate)
                .Column("WithdrawnDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            HasMany(o => o.Issuances)
                .Access
                .LowerCaseField()
                .KeyColumn("CredentialId")
                .Inverse()
                .Cascade.All();

            HasMany(o => o.DateLogs) //PBI 136585
                .Access
                .CamelCaseField()
                .KeyColumn("CredentialGuid").PropertyRef("ExternalId")
                .Inverse()
                .Cascade.All();

            Map(o => o.ConsecutiveKCIPassRequired)
                .Column("ConsecutiveKCIPassRequired")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.IsInCMP)
                .Column("IsInCMP")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.CMPEnrollmentDate)
                .Column("CMPEnrollmentDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            Map(o => o.CMPUnenrollmentDate)
                .Column("CMPUnenrollmentDate")
                .CustomSqlType("datetime2(7)")
                .Nullable();

            //pbi 210449 (Proj 1473) Remove unnecessary elements from homepage/menu for Cosponsored physicians
            Map(o => o.IsCosponsored)
                .Column("IsCosponsored")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.OnBehalfBoardCode)
                .CustomSqlType("nvarchar")
                .Length(50)
                .Nullable();

            Map(o => o.OnBehalfBoardName)
                .CustomSqlType("nvarchar")
                .Length(200)
                .Nullable();

            Component(o => o.AuditData);
        }
    }
}