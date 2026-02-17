using System;

namespace Abim.Platform.Program.Relational.Domain
{
    /// <summary>
    /// IAggregateRoot interface
    /// </summary>
    public interface IAggregateRoot
    {
        /// <summary>
        /// Gets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        Guid ExternalId { get; }
    }
}
