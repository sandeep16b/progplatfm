using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    /// <summary>
    /// Corrective Action Result Map
    /// </summary>
    /// <returns></returns>
    public class CorrectiveActionResultMap : ClassMap<CorrectiveActionResult>
    {
        /// <summary>
        /// Corrective Action Result Map
        /// </summary>
        /// <returns></returns>
        public CorrectiveActionResultMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.CorrectiveActionRunTableName);

            Id(o => o.Id).Column("CorrectiveActionResultId")
               .Not.Nullable();

            Map(o => o.CredentialId)
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable();

            Map(o => o.MemberId)
                .CustomSqlType("uniqueidentifier")
                .Not.Nullable();

            Map(o => o.EventDate)
                .CustomSqlType("datetime")
                .Not.Nullable();

            Map(o => o.MeetRule)
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.PBI)
                .CustomSqlType("nvarchar")
                .Length(255)
                .Nullable();

            Map(o => o.AdditionalResults)
                .CustomSqlType("varchar")
                .Length(8000)
                .Nullable();

            Map(o => o.ValidationResults)
                .CustomSqlType("nvarchar")
                .Length(500)
                .Nullable();

            //----------- a c t u a l    r e s u l t s ------------------------------
            Map(o => o.IssuanceDate)
                .CustomSqlType("datetime")
                .Nullable();

            Map(o => o.MaintenanceStatus)
                //.Column("MaintenanceStatus").CustomType<MaintenanceStatusTypeMapping>()
                .CustomSqlType("nvarchar")
                .Length(50)
                .Nullable();

            Map(o => o.CredentialCategory)
                .CustomSqlType("varchar")
                .Length(50)
                .Nullable();

            //--------------------- Attestation  ------------------------------
            Map(o => o.Attestation)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.ReattestationDueDate)
                .CustomSqlType("datetime")
                .Nullable();
            //--------------------- Exam Requirement ------------------------------
            Map(o => o.ExamRequirement)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.ExamAssessmentMet)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.PassMOCExam)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.PassKCIExam)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.PassCMPExam)
               .CustomSqlType("bit")
               .Nullable();

            Map(o => o.MOCExamTimeRange)
                .CustomSqlType("nvarchar")
                .Length(255)
                .Nullable();

            Map(o => o.KCIExamTimeRange)
                .CustomSqlType("nvarchar")
                .Length(255)
                .Nullable();
            //--------------------- Five Year LookBack ------------------------------------------
            Map(o => o.FiveYearLookBack)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.FiveYearLookBackRange)
                .CustomSqlType("nvarchar")
                .Length(255)
                .Nullable();

            Map(a => a.TotalMOCPoints)
                .CustomSqlType("decimal")
                .Precision(18)
                .Scale(4)
                .Nullable();

            Map(a => a.MedicalKnowledgePoints)
                .CustomSqlType("decimal")
                .Precision(18)
                .Scale(4)
                .Nullable();

            Map(o => o.Reciprocity)
                .CustomSqlType("bit")
                .Nullable();

            Map(o => o.NewSubspecialtyInitialCert)
                .CustomSqlType("bit")
                .Nullable();

            //--------------------- Maintenance Status  ------------------------------------------
            Map(a => a.MaintenanceAnyMOCPoints)
                .CustomSqlType("decimal")
                .Precision(18)
                .Scale(4)
                .Nullable();

            Map(o => o.MaintenanceTwoYearLookBackRange)
                .CustomSqlType("nvarchar")
                .Length(255)
                .Nullable();
            //---------------------- Audit ----------------------
            Map(o => o.AuditData.Created)
                .Column("Created")
                .CustomSqlType("datetime")
                .Not.Nullable();

            Map(o => o.AuditData.CreatedBy)
                .Column("CreatedBy")
                .CustomSqlType("nvarchar")
                .Length(50)
                .Not.Nullable();
            //--------------------------------------------
        }
    }
}