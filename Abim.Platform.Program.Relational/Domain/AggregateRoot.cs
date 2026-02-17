using System;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Relational.Domain.Types;

namespace Abim.Platform.Program.Relational.Domain
{
    /// <summary>
    /// AggregateRoot base class for domain objects
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Domain.Entity" />
    [Serializable]
    public abstract class AggregateRoot : Entity
    {
        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>
        /// The external identifier.
        /// </value>
        public virtual Guid ExternalId { get; protected internal set; } = Guid.NewGuid();

        /// <summary>
        /// Sets the audit data.
        /// </summary>
        /// <param name="audit">The audit.</param>
        protected internal virtual void SetAuditData(AuditData audit)
        {
            AuditData = audit;
        }
    }

    /// <summary>
    /// Generic AggregateRoot class. Inherits from the non-generic AggregateRoot class
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    /// <seealso cref="Abim.Platform.Program.Relational.Domain.Entity" />
    [Serializable]
    public class AggregateRoot<TAggregateRoot> : AggregateRoot, IAggregateRoot, IDomainValidationHandler<TAggregateRoot>
        where TAggregateRoot : AggregateRoot<TAggregateRoot>
    {
        /// <summary>
        /// Validates the specified factory.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        AbimValidationResult IDomainValidationHandler<TAggregateRoot>.Validate(IValidationFactory factory)
        {
            var val = factory.GetValidatorInstance<TAggregateRoot>();
            return val.Validate(this).ToAbimValidationResult();
        }
    }
}
