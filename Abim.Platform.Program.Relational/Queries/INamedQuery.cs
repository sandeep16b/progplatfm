namespace Abim.Platform.Program.Relational.Queries
{
    /// <summary>
    /// INamedQuery interface
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Queries.IQuery" />
    public interface INamedQuery : IQuery
    {
        /// <summary>
        /// Gets or sets the name of the query.
        /// </summary>
        /// <value>
        /// The name of the query.
        /// </value>
        string QueryName { get; set; }
    }
}
