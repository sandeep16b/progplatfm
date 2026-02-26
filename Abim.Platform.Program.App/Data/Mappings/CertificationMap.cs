using Abim.Platform.Program.App.Data.Mappings.Enumerations;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Util;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.App.Data.Mappings
{
    internal sealed class CertificationMap : ClassMap<Certification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CertificationMap"/> class.
        /// </summary>
        public CertificationMap()
        {
            Schema(Constants.Database.Schema);
            Table(Constants.Database.CertificationTableName);
            
            Id(o => o.Id)
                .Column("CertificationId")
                .Not.Nullable();

            Map(o => o.ExternalId)
                .Column("CertificationGuid")
                .CustomSqlType("uniqueidentifier")
                .Unique()
                .Not.Nullable();
            
            Map(o => o.Type)
                .CustomType<CertificationTypeMapping>()
                .Column("Type")
                .CustomSqlType("nvarchar")
                .Length(50);

            Map(o => o.Name)
                .CustomSqlType("nvarchar")
                .Length(255);

            Map(o => o.Code)
                .CustomSqlType("nvarchar")
                .Length(50);

            Map(o => o.ConsecutiveAttempt)
                .CustomSqlType("int").Nullable();

            Map(o => o.AddedQualification)
                .Not.Nullable();

            Map(o => o.IsCertificateRetired)
                .Column("IsCertificateRetired")
                .CustomSqlType("bit")
                .Not.Nullable();

            Map(o => o.CertificateRetiredDate)
                .Column("CertificateRetiredDate")
                .CustomSqlType("datetime")
                .Nullable();

            References(o => o.Source, "SourceId")
                .Cascade.None();
            //ar@10/25/2015: Temporary removed until solution is found: .Fetch.Join();

            References(o => o.BaseCertification, "BaseCertificationId")
                .Cascade.None();
            
            Component(o => o.AuditData);
        }
    }
}