using Abim.Platform.Program.Relational.Domain.Types;

namespace Abim.Platform.Program.Relational.Domain
{
    /// <summary>
    /// IEntity interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEntity<out T>
    {
        /// <summary>
        /// A unique identifier based on our generic type.
        /// </summary>
        T Id { get; }

        /// <summary>
        /// Auditing metadata for the given model object.
        /// </summary>
        AuditData AuditData { get; }
    }
}
