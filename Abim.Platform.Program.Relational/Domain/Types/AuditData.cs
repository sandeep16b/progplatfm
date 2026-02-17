using FluentValidation;
using FluentValidation.Results;
using System;

namespace Abim.Platform.Program.Relational.Domain.Types
{
    /// <summary>
    /// Contains Audit Data (Created, CreatedBy, Modified, ModifiedBy) for any containing domain object type. This class should never be inherited from,
    /// but left unsealed for NHibernate.
    /// </summary>
    [Serializable]
    public class AuditData
    {
        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        /// <value>
        /// The create date.
        /// </value>
        public virtual DateTime Created { get; protected internal set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>
        /// The created by.
        /// </value>
        public virtual string CreatedBy { get; protected internal set; }

        /// <summary>
        /// Gets or sets the last modified date.
        /// </summary>
        /// <value>
        /// The last modified date.
        /// </value>
        public virtual DateTime? Modified { get; set; }

        /// <summary>
        /// Gets or sets the last updated by.
        /// </summary>
        /// <value>
        /// The last updated by.
        /// </value>
        public virtual string ModifiedBy { get; set; }

        /// <summary>
        /// The seconds forgiveness
        /// </summary>
        protected static TimeSpan secondsForgiveness = new TimeSpan(0, 0, 5);
        

        /// <summary>
        /// Initializes a new instance of the <see cref="AuditData"/> class.
        /// </summary>
        public static AuditData Create(string createdBy)
        {
            return new AuditData
            {
                CreatedBy = createdBy
            };
        }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <returns></returns>
        public ValidationResult Validate()
        {
            return new AuditDataValidator().Validate(this);
        }
        
        /// <summary>
        /// Gets the standard "DateModified" for sorting by date last modified or created.
        /// </summary>
        /// <returns></returns>
        public DateTime DateModified()
        {
            if(Modified != null) return Modified.Value;
            return Created;
        }
        
        /// <summary>
        /// inner class AuditDataValidator
        /// </summary>
        /// <seealso cref="FluentValidation.AbstractValidator{Abim.Platform.Program.Relational.Domain.Types.AuditData}" />
        public class AuditDataValidator : AbstractValidator<AuditData>
        {
            public AuditDataValidator()
            {
                RuleFor(x => x.Created).NotEmpty()
                    .WithMessage("Created date is required", x => x.Created);
                RuleFor(x => x.Created).Must(BeBeforeNow)
                    .WithMessage("Created date {0} is in the past", x => x.Created);
                RuleFor(x => x.CreatedBy).NotEmpty()
                    .WithMessage("CreatedBy is required", x => x.Created);
                RuleFor(x => x.Modified).Must(BeBeforeNow).When(x => x.Modified != null)
                    .WithMessage("Modified date {0} is in the past", x => x.Modified);
            }
        }
    
        /// <summary>
        /// Used in validation.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        /// <remarks>
        /// we have to have this in a separate function, otherwise the realtime value of DateTime.Now won't be used    
        /// </remarks>
        protected static bool BeBeforeNow(DateTime dateTime)
        {
            return dateTime <= (DateTime.Now.Add(secondsForgiveness));
        }

        /// <summary>
        /// Used in validation.
        /// </summary>
        /// <param name="dateTime">The date time.</param>
        /// <returns></returns>
        /// <remarks>
        /// we have to have this in a separate function, otherwise the realtime value of DateTime.Now won't be used    
        /// </remarks>
        protected static bool BeBeforeNow(DateTime? dateTime)
        {
           if(dateTime == null) return true;
            return dateTime.Value <= (DateTime.Now.Add(secondsForgiveness));
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as AuditData;
           if(other == null)
                return false;

            var same = Math.Abs((Created - other.Created).TotalSeconds) < 1;
            same = same && CreatedBy == other.CreatedBy;
            same = same && ModifiedBy == other.ModifiedBy;

            var span = Modified - other.Modified;
            same = (span != null)
                ? same && span.Value.TotalSeconds < 1
                : same && Modified == other.Modified;

            return same;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode(){ return base.GetHashCode(); }
    }
}
