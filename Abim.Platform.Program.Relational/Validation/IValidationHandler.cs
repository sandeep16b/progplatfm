using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Validation.Impl;

namespace Abim.Platform.Program.Relational.Validation
{
    /// <summary>
    /// IDomainValidationHandler interface
    /// </summary>
    /// <typeparam name="TAggregateRoot">The type of the aggregate root.</typeparam>
    public interface IDomainValidationHandler<in TAggregateRoot> where TAggregateRoot : IAggregateRoot
    {
        /// <summary>
        /// Validates.
        /// </summary>
        /// <param name="factory">The factory.</param>
        /// <returns></returns>
        AbimValidationResult Validate(IValidationFactory factory);
    }
}
