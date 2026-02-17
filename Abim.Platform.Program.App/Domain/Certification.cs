using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using FluentValidation;
using System;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// Certification Class.
    /// </summary>
    /// <remarks>
    /// Represents a certification is a given discipline that has been awarded to a diplomate.
    /// A certification can have many issuances over the course of its lifetime.
    /// </remarks>
    public class Certification :
        AggregateRoot<Certification> 
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

        /// <summary>
        /// Gets or sets the Certification type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public virtual CertificationType Type { get; protected internal set; }
        
        /// <summary>
        /// Gets the consecutive attempt number.
        /// </summary>
        /// <value>
        /// The consecutive attempt number.
        /// </value>
        public virtual int? ConsecutiveAttempt { get; protected internal set; }
        
        /// <summary>
        /// Gets or sets the base certification.
        /// </summary>
        /// <value>
        /// The base certification.
        /// </value>
        public virtual Certification BaseCertification { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether [added qualification].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [added qualification]; otherwise, <c>false</c>.
        /// </value>
        public virtual bool AddedQualification { get; protected internal set; }

        /// <summary>
        /// Gets or sets the source certification data.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public virtual Source Source { get; protected internal set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is certificate retired.
        /// </summary>
        public virtual bool IsCertificateRetired { get; set; }

        /// <summary>
        /// Gets or sets the certificate retired date.
        /// </summary>
        public virtual DateTime? CertificateRetiredDate { get; set; }
        #endregion

        #region Computed Properties

        /// <summary>
        /// Check if internal medicine 
        /// </summary>
        public virtual bool IsInternalMedicine
        {
            get
            {
              return (Code == "IM");
            }
        }

        /// <summary>
        /// Check if subspecialty certificate
        /// </summary>
        public virtual bool IsSubspecialty
        {
            get
            {
                return Type.Equals(CertificationType.Subspecialty);
            }
        }

        /// <summary>
        /// Check if subspecialty ACHD certificate
        /// </summary>
        public virtual bool IsSubspecialtyACHD
        {
            get
            {
                return (Code == "ACHD");
            }
        }

        /// <summary>
        /// Check if subspecialty in specific area
        /// </summary>
        public virtual bool IsSubspecialtySpecificArea
        {
            get
            {
                return (BaseCertification != null);
            }
        }

        #endregion

        #region Factory

        /// <summary>
        /// Creates the certification.
        /// </summary>
        /// <param name="baseCertification">The base certification.</param>
        /// <param name="source">The source.</param>
        /// <param name="certType">Type of the cert.</param>
        /// <param name="name">The name.</param>
        /// <param name="code">The code.</param>
        /// <param name="createdBy">The created by.</param>
        /// <returns></returns>
        public static Certification Create(Certification baseCertification, Source source, CertificationType certType, string name,
            string code, string createdBy)
        {
            var cert = new Certification()
            {
                BaseCertification = baseCertification,
                Type = certType,
                Name = name,
                Code = code,
                Source = source,
                AuditData = AuditData.Create(createdBy)
            };
            return cert;
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
            var other = obj as Certification;
            if (other == null)
                return false;
            
            return (ExternalId == other.ExternalId);
        }
        /// <summary>
         /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
         /// </summary>
         /// <returns>
         ///   <c>HashCode</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
         /// </returns>
        public override int GetHashCode()
        {
            return ExternalId.GetHashCode();
        }

        #region Apply Methods

        /// <summary>
        /// Sets the consecutive attempt.
        /// </summary>
        /// <param name="consecutiveAttempt">The consecutive attempt.</param>
        public virtual void SetConsecutiveAttempt(int? consecutiveAttempt)
        {
            ConsecutiveAttempt = consecutiveAttempt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="consecutiveAttempt"></param>
        public virtual void ApplyUpdateCertification(string name, CertificationType type, int? consecutiveAttempt)
        {
            Name = name;
            Type = type;
            ConsecutiveAttempt = consecutiveAttempt;
        }

        #endregion
    }

    #region Validator Classes

    /// <summary>
    /// Validates a certification.
    /// </summary>
    public class CertificationValidator : AbstractValidator<Certification>
    {
        /// <summary>
        /// Creates a new instance of the CertificationValidator object.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public CertificationValidator(IValidationFactory factory)
        {
            RuleFor(x => x.Name).NotEmpty()
                .WithMessage("Name cannot be empty");

            RuleFor(a => a.Name).Length(1, 255).When(a => a.Name != null)
                .WithMessage("Name length must be from 1 to 255");

            RuleFor(x => x.Code).NotEmpty()
                .WithMessage("Code cannot be empty");

            RuleFor(a => a.Code).Length(1, 50).When(a => a.Code != null)
                .WithMessage("Code length must be from 1 to 50");

            RuleFor(x => x.Source).NotNull()
                .WithMessage("Source is required");
            RuleFor(x => x.Source).SetValidator(factory.GetValidatorInstance<Source>());

            //We don't need to validate Subspecialty to have BaseCertification, but below would work correctly if we decided to 
			// inforce this requirement
            //var baseCertificateValidator = this; //factory.GetValidatorInstance<Certification>();
            //RuleFor(x => x.BaseCertification).NotNull().When(o => o.Type == CertificationType.Subspecialty)
            //    .WithMessage("BaseCertification is required for all non-primary {0} objects");
            //RuleFor(x => x.BaseCertification).SetValidator(baseCertificateValidator);

            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }

    #endregion
}
