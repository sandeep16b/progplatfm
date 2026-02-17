using Abim.Platform.Program.Relational.Domain.Types;
using FluentNHibernate.Mapping;

namespace Abim.Platform.Program.Relational.Domain.Mapping
{
    /// <summary>
    /// AuditDataMap clas
    /// </summary>
    /// <seealso cref="FluentNHibernate.Mapping.ComponentMap{Abim.Platform.Program.Relational.Domain.Types.AuditData}" />
    public class AuditDataMap : ComponentMap<AuditData>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AuditDataMap"/> class.
        /// </summary>
        public AuditDataMap()
        {
            Map(x => x.Created)
                .Not.Update()
                .CustomType("DateTime")
                .CustomSqlType("datetime")
                .Not.Nullable();
            
            Map(x => x.CreatedBy)
                .Not.Update()
                .Not.Nullable();

            Map(x => x.Modified)
                .Nullable();

            Map(x => x.ModifiedBy)
                .Nullable();
        }
    }
}
