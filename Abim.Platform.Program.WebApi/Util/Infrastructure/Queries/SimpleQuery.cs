using Abim.Platform.Program.Relational;

namespace Abim.Platform.Program.WebApi
{
    /// <summary>
    /// A simple query inheriting from ComplexQueryBase
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.ComplexQueryBase" />
    public class SimpleQuery : ComplexQueryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleQuery"/> class.
        /// </summary>
        public SimpleQuery()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleQuery"/> class.
        /// </summary>
        /// <param name="paging">The paging.</param>
        public SimpleQuery(PageDefinition paging) : base(paging)
        {
        }
    }
}
