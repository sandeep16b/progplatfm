using FluentValidation;

namespace Abim.Platform.Program.Relational.Validation.Impl
{
    /// <summary>
    /// ValidationFactory class
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Validation.IValidationFactory" />
    public class ValidationFactory : IValidationFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationFactory"/> class.
        /// </summary>
        public ValidationFactory()
        {
        }

        /// <summary>
        /// </summary>
        /// <typeparam name="T">The type to fetch a validator for.</typeparam>
        /// <returns></returns>
        public IValidator<T> GetValidatorInstance<T>()
        {
            return DependencyResolver.Container.GetInstance<IValidator<T>>();
        }
    }
}
