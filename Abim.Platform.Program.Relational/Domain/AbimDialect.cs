using NHibernate.Dialect;

namespace Abim.Platform.Program.Relational.Domain
{
    /// <summary>
    /// AbimDialect class
    /// </summary>
    /// <seealso cref="NHibernate.Dialect.MsSql2012Dialect" />
    public class AbimDialect : MsSql2012Dialect
    {     
        /// <summary>
        /// Forces creation of non clustered primary keys (for guids).  
        /// </summary>
        public override string PrimaryKeyString
        {
            get { return "primary key nonclustered"; }
        }
    }
}
