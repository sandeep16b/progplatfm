using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// Use to set the source of where the Certification data was derived from
    /// </summary>
    public class Source : 
        AggregateRoot<Source>,
        IDomainValidationHandler<Source>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public virtual string Name { get; protected internal set; }

        /// <summary>
        /// Gets or sets the code.
        /// </summary>
        /// <value>
        /// The code.
        /// </value>
        public virtual string Code { get; protected internal set; }

        #endregion Properties

        #region Factory

        /// <summary>
        /// Creates the Source.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="code">The code.</param>
        /// <param name="createdBy">The created by.</param>
        /// <returns></returns>
        public static Source Create(string name, string code, string createdBy)
        {
            var src = new Source()
            {
                Name = name,
                Code = code,
                AuditData = AuditData.Create(createdBy)
            };
            return src;
        }

        /// <summary>
        /// Because Source does not support commands, this is used to set the properties internally (specifically for unit testing)
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="externalId">The externalId.</param>
        /// <param name="name">The name.</param>
        /// <param name="code">The code.</param>
        protected internal virtual void SetProperties(int id, Guid externalId, string name, string code)
        {
            Id = id;
            ExternalId = Guid.NewGuid();
            Name = name;
            Code = code;
        }

        #endregion

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as Source;
            if (other == null)
                return false;
            
            return (ExternalId == other.ExternalId);
        }
        /// <summary>
        /// GetHashCode <see cref="System.Object" />.
        /// </summary>
        /// <param></param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override int GetHashCode()
        {
            return ExternalId.GetHashCode();
        }
    }
    
    #region Validator Classes

    /// <summary>
    /// Validator for Source
    /// </summary>
    public class SourceValidator : AbstractValidator<Source>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SourceValidator"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public SourceValidator(IValidationFactory factory)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Name cannot be empty");

            RuleFor(a => a.Name).Length(1, 255).When(a => a.Name != null)
                .WithMessage("Name length must be from 1 to 255");

            RuleFor(x => x.Code).NotEmpty()
                .WithMessage("Code cannot be empty");

            RuleFor(a => a.Code).Length(1, 10).When(a => a.Code != null)
                .WithMessage("Code length must be from 1 to 10");

            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }
        
    #endregion
}
