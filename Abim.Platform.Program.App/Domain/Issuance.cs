using System;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using FluentValidation;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// a credential issuance
    /// </summary>
    public class Issuance :
        Entity
    {
        #region Properties

        /// <summary>
        /// Gets the credential.
        /// </summary>
        /// <value>
        /// The credential.
        /// </value>
        public virtual Credential Credential { get; protected internal set; }
        
        /// <summary>
        /// Gets or sets the duration.
        /// </summary>
        /// <value>
        /// The duration.
        /// </value>
        public virtual DurationType Duration { get; protected internal set; }

        /// <summary>
        /// Gets the requirement.
        /// </summary>
        /// <value>
        /// The requirement.
        /// </value>
        public virtual MaintenanceRequirementType MaintenanceRequirement { get; protected internal set; }

        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public virtual MaintenanceStatusType MaintenanceStatus { get; protected internal set; }
        
        /// <summary>
        /// Gets or sets the occurrence.
        /// </summary>
        /// <value>
        /// The occurrence.
        /// </value>
        public virtual OccurrenceType Occurrence { get; protected internal set; }

        /// <summary>
        /// Gets the issuance status.
        /// </summary>
        /// <value>
        /// The issuance status.
        /// </value>
        public virtual IssuanceStatusType IssuanceStatus { get; protected internal set; }

        /// <summary>
        /// Gets or sets the issuance date.
        /// </summary>
        /// <value>
        /// The issuance date.
        /// </value>
        public virtual DateTime IssuanceDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the expiration date.
        /// </summary>
        /// <value>
        /// The expiration date.
        /// </value>
        public virtual DateTime? ExpirationDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the date the issuance was expired.
        /// </summary>
        /// <value>
        /// The date the issuance was expired.
        /// </value>
        /// <remarks>See PBI 142264.</remarks>
        public virtual DateTime? ExpiredDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the effective date.
        /// </summary>
        /// <value>
        /// The effective date.
        /// </value>
        public virtual DateTime? EffectiveDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the scheduled update.
        /// </summary>
        /// <value>
        /// The scheduled update.
        /// </value>
        public virtual DateTime? ScheduledUpdate { get; protected internal set; }

        /// <summary>
        /// Gets a value indicating whether the credential issuance is under review.
        /// </summary>
        /// <value>
        ///   <c>true</c> if the credential issuance is under review; otherwise, <c>false</c>.
        /// </value>
        public virtual bool UnderReview { get; protected internal set; }

        /// <summary>
        /// The id of the Registration.
        /// </summary>
        public virtual Guid RegistrationGuid { get; protected internal set; }

        /// <summary>
        /// Gets or sets the source certification data.
        /// </summary>
        /// <value>
        /// The source.
        /// </value>
        public virtual Source Source { get; protected internal set; }

        /// <summary>
        /// The date/time de-selection was submitted
        /// </summary>
        public virtual DateTime? DeselectionSubmittedDate { get; protected internal set; }

        /// <summary>
        /// The date/time the deselection should take effect
        /// </summary>
        public virtual DateTime? DeselectionEffectiveDate { get; protected internal set; }

        /// <summary>
        /// The date/time the deselection was actually processed
        /// </summary>
        public virtual DateTime? DeselectionProcessedDate { get; protected internal set; }

        /// <summary>
        /// Get property check if an active issuance expiring in current year
        /// </summary>
        public virtual bool ExpiringThisYear
        {
            get
            {
                if(!CanExpire) return false;
                return (ExpirationDate.Value.Year == DateTime.Now.Year &&
                    IssuanceStatus == IssuanceStatusType.Active);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this issuance can expire.
        /// </summary>
        /// <value>
        /// <c>true</c> if this issuance can expire; otherwise, <c>false</c>.
        /// </value>
        public virtual bool CanExpire
        {
            get
            {
                if(Duration != DurationType.Timelimited) return false;
                if(ExpirationDate == null) return false;
                return true;
            }
        }

        /// <summary>
        /// Flag if issuance has been changed by current process
        /// </summary>
        public virtual bool HasChanged { get; protected internal set; }

        /// <summary>
        /// Flag if issuance has been added by current process
        /// </summary>
        public virtual bool HasAdded { get; protected internal set; }
        #endregion

        #region Factory

        /// <summary>
        /// Initializes a new instance of the <see cref="Issuance"/> class.
        /// </summary>
        protected Issuance()
        {
            
        }

        /// <summary>
        /// Creates the issuance.
        /// </summary>
        /// <param name="createdBy">The CreatedBy audit field</param>
        /// <returns></returns>
        protected internal static Issuance Create(string createdBy)
        {
            var issuance = new Issuance()
            {
                AuditData = AuditData.Create(createdBy)
            };
            return issuance;
        }

        /// <summary>
        /// Creates the issuance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="maintenanceRequirement">The requirement.</param>
        /// <param name="maintenanceStatus">The status.</param>
        /// <param name="occurrence">The occurrence.</param>
        /// <param name="issuanceStatus">The issuance status.</param>
        /// <param name="issuanceDate">The issuance date.</param>
        /// <param name="effectiveDate">The issuance date.</param>
        /// <param name="createdBy">The CreatedBy audit field</param>
        /// <returns></returns>
        public static Issuance Create(Source source, DurationType duration, MaintenanceRequirementType maintenanceRequirement, 
            MaintenanceStatusType maintenanceStatus, OccurrenceType occurrence, IssuanceStatusType issuanceStatus, DateTime issuanceDate,
            DateTime effectiveDate, string createdBy)
        {
            var issuance = new Issuance()
            {
                Source                 = source,
                Duration               = duration,
                MaintenanceRequirement = maintenanceRequirement,
                MaintenanceStatus      = maintenanceStatus,
                Occurrence             = occurrence,
                IssuanceStatus         = issuanceStatus,
                IssuanceDate           = issuanceDate,
                EffectiveDate          = effectiveDate,
                AuditData              = AuditData.Create(createdBy)
            };

            issuance.HasAdded = true;

            return issuance;
        }

        /// <summary>
        /// Creates the issuance.
        /// </summary>
        /// <param name="source">The source.</param>
        /// <param name="duration">The duration.</param>
        /// <param name="maintenanceRequirement">The requirement.</param>
        /// <param name="maintenanceStatus">The status.</param>
        /// <param name="occurrence">The occurrence.</param>
        /// <param name="issuanceStatus">The issuance status.</param>
        /// <param name="issuanceDate">The issuance date.</param>
        /// <param name="createdBy">The CreatedBy audit field</param>
        /// <returns></returns>
        public static Issuance Create(Source source, DurationType duration, MaintenanceRequirementType maintenanceRequirement,
            MaintenanceStatusType maintenanceStatus, OccurrenceType occurrence, IssuanceStatusType issuanceStatus, DateTime issuanceDate,
            string createdBy)
        {
            var issuance = new Issuance()
            {
                Source = source,
                Duration = duration,
                MaintenanceRequirement = maintenanceRequirement,
                MaintenanceStatus = maintenanceStatus,
                Occurrence = occurrence,
                IssuanceStatus = issuanceStatus,
                IssuanceDate = issuanceDate,
                AuditData = AuditData.Create(createdBy)
            };

            issuance.HasAdded = true;

            return issuance;
        }

        /// <summary>
        /// ApplyUpdateIssuanceEvent
        /// </summary>
        /// <param name="source"></param>
        /// <param name="duration"></param>
        /// <param name="maintenanceRequirement"></param>
        /// <param name="maintenanceStatus"></param>
        /// <param name="occurrence"></param>
        /// <param name="issuanceStatus"></param>
        /// <param name="issuanceDate"></param>
        /// <param name="effectiveDate"></param>
        /// <param name="expirationDate"></param>
        /// <param name="modifiedBy"></param>
        public virtual void ApplyUpdateIssuanceEvent(Source source, DurationType duration, MaintenanceRequirementType maintenanceRequirement,
            MaintenanceStatusType maintenanceStatus, OccurrenceType occurrence, IssuanceStatusType issuanceStatus, DateTime issuanceDate, 
            DateTime effectiveDate, DateTime? expirationDate, string modifiedBy)
        {
            Source = source;
            Duration = duration;
            MaintenanceRequirement = maintenanceRequirement;
            MaintenanceStatus = maintenanceStatus;
            Occurrence = occurrence;
            IssuanceStatus = issuanceStatus;
            IssuanceDate = issuanceDate;
            EffectiveDate = effectiveDate;
            ExpirationDate = expirationDate;

            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;

            HasChanged = true;

            Credential.UpdateIsActive();
        }

        #endregion

        /// <summary>
        /// Expires the issuance, setting its status and maintenanceStatus to prescribed values
        /// </summary>
        /// <param name="status">The status.</param>
        /// <param name="maintenanceStatus">The maintenance status.</param>
        public virtual void Expire(IssuanceStatusType status, MaintenanceStatusType maintenanceStatus)
        {
            if(!CanExpire)
                throw new Exception(string.Format($"Issuance {Id} cannot expire and so cannot apply an ExpireIssuanceEvent"));
            IssuanceStatus = status;
            MaintenanceStatus = maintenanceStatus;   //MaintenanceStatusType.NotMaintained;
            AuditData.Modified = DateTime.Now;
        }

        /// <summary>
        /// Marks an issuance for deselection.
        /// </summary>
        /// <param name="submittedDate">The date/time the deselect was submitted</param>
        /// <param name="effectiveDate">The date/time the deselection should become effective.</param>
        /// <param name="modifiedBy">The user who modified the Issuance</param>
        public virtual void MarkForDeselection(DateTime submittedDate, DateTime effectiveDate, string modifiedBy)
        {
            DeselectionSubmittedDate = DateTime.Now;
            DeselectionEffectiveDate = effectiveDate;

            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;

            HasChanged = true;
        }

        /// <summary>
        /// Marks an issuance for selection.
        /// </summary>
        /// <param name="modifiedBy">The user who modified the Issuance</param>
        public virtual void MarkForSelection(string modifiedBy)
        {
            DeselectionSubmittedDate = null;
            DeselectionEffectiveDate = null;

            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;

            HasChanged = true;
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
            var other = obj as Issuance;
            if (other == null)
                return false;
            
            return (Id == other.Id);
        }
        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <returns>
        ///   <c>HashCode</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }

    #region Validator Classes

    /// <summary>
    /// Validates an issuance
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Issuance}" />
    public class IssuanceValidator : AbstractValidator<Issuance>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IssuanceValidator"/> class.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public IssuanceValidator(IValidationFactory factory)
        {
            RuleFor(x => x.Source).NotNull()
                .WithMessage("Source is required");
            RuleFor(x => x.Source).SetValidator(factory.GetValidatorInstance<Source>());
            
            RuleFor(x => x.Credential).NotNull()
                .WithMessage("CredentialId is required");
            
            RuleFor(x => x.IssuanceDate).NotEqual(DateTime.MinValue)
                .WithMessage("IssuanceDate is required");
                
            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }

    #endregion
}
