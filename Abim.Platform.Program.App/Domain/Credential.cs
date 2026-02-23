using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.Interservice.Shared;
using Abim.Platform.Program.Relational.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Abim.Platform.Program.App.Domain
{
    /// <summary>
    /// The diplomate's Credential - class
    /// </summary>
    public class Credential :
        AggregateRoot<Credential>,
        IDomainValidationHandler<Credential>
    {
        #region Fields

        /// <summary>
        /// The issuances collection
        /// </summary>
        private IList<Issuance> issuances;

        /// <summary>
        /// The credential date logs for this credential
        /// </summary>
        private IList<CredentialDateLog> dateLogs;

        //Backing fields needed to support logging functionality outlined in PBI 136585
        private DateTime? assessmentMetDate;
        private DateTime? examDueDate;
        private DateTime? displayExamDueDate;
        private DateTime? kciexamduedate; //Lowercase due to Fluent NHibernate issue due to Property name
        private DateTime? mocexamduedate; //Lowercase due to Fluent NHibernate issue due to Property name
        private DateTime? gracePeriodStartDate;
        private DateTime? gracePeriodEndDate;

        #endregion Fields

        #region Properties

        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public virtual Guid MemberId { get; protected internal set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public virtual CredentialType Type { get; protected internal set; }

        /// <summary>
        /// Gets or sets the pathway.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public virtual PathwayType Pathway { get; protected internal set; }

        /// <summary>
        /// Gets or sets the exam fail count.
        /// </summary>
        /// <value>
        /// The exam fail count.
        /// </value>
        public virtual int ExamFailCount { get; protected internal set; }

        /// <summary>
        /// Gets or sets the IsActive bool.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public virtual bool IsActive { get; protected internal set; }

        /// <summary>
        /// Gets or sets the ForcedPathway bool.
        /// </summary>
        /// <value>
        /// whether it's a forced pathway credential.
        /// </value>
        public virtual bool ForcedPathway { get; protected internal set; }

        /// <summary>
        /// Whether the examType has been met.
        /// </summary>
        /// <value>
        /// true or false.
        /// </value>
        public virtual bool AssessmentMet { get; protected internal set; }

        /// <summary>
        /// The date the examType was met.
        /// </summary>
        /// <value>
        /// A datetime.
        /// </value>
        public virtual DateTime? AssessmentMetDate
        {
            get { return assessmentMetDate; }

            protected internal set
            {
                if (assessmentMetDate != value)
                    AddCredentialDateLog(CredentialDateType.AssessmentMetDate, assessmentMetDate, value);

                assessmentMetDate = value;
            }
        }

        /// <summary>
        /// The date a lookback occurred on.
        /// </summary>
        /// <value>
        /// A datetime.
        /// </value>
        public virtual DateTime? LookbackDate { get; protected internal set; }

        /// <summary>
        /// The date a Skipped Exam Lookback Date.
        /// </summary>
        /// <value>
        /// A datetime.
        /// </value>
        public virtual DateTime? SkippedExamLookbackDate { get; protected internal set; }

        /// <summary>
        /// Gets the issuances.
        /// </summary>
        /// <value>
        /// The issuances.
        /// </value>
        public virtual IReadOnlyList<Issuance> Issuances
        {
            get { return new ReadOnlyCollection<Issuance>(issuances); }
        }

        /// <summary>
        /// Gets the date logs.
        /// </summary>
        /// <value>
        /// The date logs.
        /// </value>
        public virtual IReadOnlyList<CredentialDateLog> DateLogs
        {
            get { return new ReadOnlyCollection<CredentialDateLog>(dateLogs); }
        }

        /// <summary>
        /// Gets or sets the Certification.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public virtual Certification Certification { get; protected internal set; }

        /// <summary>
        /// Gets or sets the grace period start date.
        /// </summary>
        /// <value>
        /// The grace period start date.
        /// </value>
        public virtual DateTime? GracePeriodStartDate
        {
            get { return gracePeriodStartDate; } 
            protected internal set
            {
                if (gracePeriodStartDate != value)
                    AddCredentialDateLog(CredentialDateType.GracePeriodStartDate, gracePeriodStartDate, value);

                gracePeriodStartDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the grace period end date.
        /// </summary>
        /// <value>
        /// The grace period end date.
        /// </value>
        public virtual DateTime? GracePeriodEndDate
        {
            get { return gracePeriodEndDate; }
            protected internal set
            {
                if (gracePeriodEndDate != value)
                    AddCredentialDateLog(CredentialDateType.GracePeriodEndDate, gracePeriodEndDate, value);

                gracePeriodEndDate = value;
            }
        }

        /// <summary>
        /// Gets or sets the next exam due date.
        /// </summary>
        /// <value>
        /// The next exam due date.
        /// </value>
        public virtual DateTime? ExamDueDate
        {
            get { return examDueDate; }
            protected internal set
            {
                if (examDueDate != value)
                    AddCredentialDateLog(CredentialDateType.ExamDueDate, examDueDate, value);

                examDueDate = value;
            }
        }

        /// <summary>
        /// The exam due date for display purposes
        /// </summary>
        public virtual DateTime? DisplayExamDueDate
        {
            get { return displayExamDueDate; }
            protected internal set
            {
                if (displayExamDueDate != value)
                    AddCredentialDateLog(CredentialDateType.DisplayExamDueDate, displayExamDueDate, value);

                displayExamDueDate = value;
            }
        }

        /// <summary>
        /// The due date for the KCI exam
        /// </summary>
        public virtual DateTime? KCIExamDueDate
        {
            get { return kciexamduedate; }
            protected internal set
            {
                if (kciexamduedate != value)
                    AddCredentialDateLog(CredentialDateType.KCIExamDueDate, kciexamduedate, value);

                kciexamduedate = value;
            }
        }

        /// <summary>
        /// The due date for the MOC exam
        /// </summary>
        public virtual DateTime? MOCExamDueDate
        {
            get { return mocexamduedate; }
            protected internal set
            {
                if (mocexamduedate != value)
                    AddCredentialDateLog(CredentialDateType.MOCExamDueDate, mocexamduedate, value);

                mocexamduedate = value;
            }
        }

        /// <summary>
        /// Gets or sets the re Attestation due date.
        /// </summary>
        /// <value>
        /// The Re Attestation Due Date.
        /// </value>
        public virtual DateTime? ReAttestationDueDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Selected To Maintain bool
        /// This value is NOT just a UI setting it turns out. We use this 
        /// to determine which credential of an IM/FPHM pair is the selected 
        /// one.
        /// </summary>
        public virtual bool SelectedToMaintain { get; protected internal set; } = true;


        /// <summary>
        /// Gets or sets the Grandfather MOC Print Date . PBI 87085
        /// </summary>
        /// <value>
        /// It would be used by ETL to copy date to Certification database table table GrandFatherMOC 
        /// </value>
        public virtual DateTime? GrandfatherMOCPrintDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Withdrawn Date . PBI 112730
        /// </summary>
        /// <value>
        /// It would be used by UI to set Credential to Suspend / Revoke / Surrender 
        /// </value>
        public virtual DateTime? WithdrawnDate { get; protected internal set; }

        /// <summary>
        /// If enroll into CMP pathway
        /// </summary>
        /// <value>
        /// true or false.
        /// </value>
        public virtual bool IsInCMP { get; protected internal set; }

        /// <summary>
        /// Gets or sets the CMP Enrollment Date . PBI 150807
        /// </summary>
        /// <value>
        /// It would be used only by Proj 1444 (API gateway) temporary !!!
        /// It is temporary measure until Release 3 (it would be removed and own table would be created as CMPEnrollment)
        /// </value>
        public virtual DateTime? CMPEnrollmentDate { get; protected internal set; }
  
        /// <summary>
        /// Flag if issuance has been changed by current process
        /// </summary>
        public virtual bool HasChanged { get; protected internal set; } = false;

        /// <summary>
        /// Flag if issuance has been added by current process
        /// </summary>
        public virtual bool HasAdded { get; protected internal set; } = false;

        /// <summary>
        /// Specifies whether or not passing consecutive KCI exams is required
        /// </summary>
        public virtual bool ConsecutiveKCIPassRequired { get; protected internal set; } = false;


        //pbi 210449 (Proj 1473) Remove unnecessary elements from homepage/menu for Cosponsored physicians
        /// <summary>
        /// IsCosponsored flag
        /// </summary>
        public virtual bool IsCosponsored { get; protected internal set; } = false;

        /// <summary>
        /// OnBehalfBoardCode 
        /// </summary>
        public virtual string OnBehalfBoardCode { get; protected internal set; }

        /// <summary>
        /// OnBehalfBoardName
        /// </summary>
        public virtual string OnBehalfBoardName { get; protected internal set; }


        #region DerivedProperties

        /// <summary>
        /// Gets the oldest issuance.
        /// </summary>
        /// <returns></returns>
        public virtual Issuance OldestIssuance
        {
            get
            {
                if (!HasIssuances) return null;
                return Issuances.MinEntry(i => i.IssuanceDate.Ticks);
            }
        }

        /// <summary>
        /// Gets the oldest ABIM issuance.
        /// </summary>
        /// <returns></returns>
        public virtual Issuance OldestABIMIssuance
        {
            get
            {
                return Issuances.Where(s => s.Source.Code == "ABIM").MinEntry(i => i.IssuanceDate.Ticks);
            }
        }

        /// <summary>
        /// Gets the newest issuance.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsNewestIssuanceABIM
        {
            get
            {
                if (NewestIssuance == null) return false;
                return NewestIssuance.Source.Code == "ABIM";
            }
        }

        /// <summary>
        /// Gets the newest issuance.
        /// </summary>
        /// <returns></returns>
        public virtual Issuance NewestIssuance
        {
            get
            {
                if (!HasIssuances) return null;
                return Issuances.MaxEntry(i => i.IssuanceDate.Ticks);
            }
        }

        /// <summary>
        /// Gets the Grand Father issuance.
        /// </summary>
        /// <returns></returns>
        public virtual Issuance GFIssuance
        {
            get
            {
                if (!HasIssuances) return null;

                return Issuances
                            .OrderBy(a => a.IssuanceDate)
                            .FirstOrDefault(i => i.Duration == DurationType.Lifetime && i.IssuanceStatus == IssuanceStatusType.Active);
            }
        }

        /// <summary>
        /// Find current Credential's Proper Issuance (consider GF spesial case)
        /// </summary>
        public virtual Issuance ProperIssuance
        {
            get
            {
                if (IsGrandfather) return GFIssuance;
                else
                    return NewestIssuance;
            }
        }
      
        /// <summary>
        /// Returns true if any Issuance is active and being maintained
        /// </summary>
        public virtual bool IsActiveParticipating
        {
            get
            {
                return HasIssuances &&
                        Issuances.Any(i => i.IssuanceStatus == IssuanceStatusType.Active &&
                                        i.MaintenanceStatus == MaintenanceStatusType.Maintained);
            }
        }

        /// <summary>
        /// Check if credential has any issuances
        /// </summary>
        /// <returns></returns>
        public virtual bool HasIssuances
        {
            get
            {
                return (issuances != null && issuances.Count > 0);
            }
        }

        /// <summary>
        /// Check if Lifetime issuances (Grandfather) has any NotMaintained issuances
        /// (is used for Grace period selection)
        /// </summary>
        /// <returns></returns>
        public virtual bool IfGrandFatherNotMaintained
        {
            get
            {
                return IsGrandfather &&
                    Issuances.Where(p => p.Duration == DurationType.Lifetime &&
                            p.MaintenanceStatus == MaintenanceStatusType.NotMaintained).Any();
            }
        }

        /// <summary>
        /// Determine whether Credential is a Grandfather or not
        /// </summary>
        /// <returns></returns>
        public virtual bool IsGrandfather
        {
            get
            {
                return HasIssuances
                    && Issuances.Any(i => i.Duration == DurationType.Lifetime && i.IssuanceStatus == IssuanceStatusType.Active);
            }
        }

        /// <summary>
        /// Determine whether Credential is a Exired Grandfather ( and weren’t issued MBM or TLs already. ) or not
        /// </summary>
        /// <returns></returns>
        public virtual bool IsExpiredGrandfather
        {
            get
            {
                return HasIssuances
                    && Issuances.Any(i => i.Duration == DurationType.Lifetime && i.IssuanceStatus == IssuanceStatusType.Expired) // exists epired GF
                    && !Issuances.Any(n => n.MaintenanceRequirement == MaintenanceRequirementType.Required   // no MBM issuances
                                        || n.Duration == DurationType.Timelimited ); // no TL issuances

            }
        }

        /// <summary>
        /// Determine whether Credential is a Timelimited or not (don't have any active GF  and no MBM issuances
        /// </summary>
        /// <returns></returns>
        public virtual bool IsTimelimited
        {
            get
            {
                //PBI: 148021 **** If you have no MBM issuances and no ACTIVE LIFETIME issuances and no Expired LIFETIME issuances, consider the person to be time limited.
                return HasIssuances && !IsGrandfather && !IsExpiredGrandfather && !IsMBM;
                // was before 5/1/2019 : return !IsGrandfather && !IsMBM && Issuances.Any(i => i.Duration == DurationType.Timelimited);
            }
        }
        /// <summary>
        /// Determine whether Credential is a Must be Maintained or not
        /// </summary>
        /// <returns></returns>
        public virtual bool IsMBM
        {
            get
            {
                return HasIssuances && Issuances.Any(i => i.MaintenanceRequirement == MaintenanceRequirementType.Required);
            }
        }

        /// <summary>
        /// Determine whether Credential is a InitialFPHM or not
        /// </summary>
        public virtual bool IsInitialFPHM
        {
            get
            {
                return !HasIssuances && Certification.IsFPHM();
            }
        }
        /// <summary>
        /// Determine whether Credential is Revoked or Surrendered
        /// </summary>
        /// <returns></returns>
        public virtual bool IsRevokedOrSurrendered
        {
            get
            {
                return Issuances.Any(i => i.Source.Code == "ABIM" && ((i.IssuanceStatus ==  IssuanceStatusType.Revoked) || (i.IssuanceStatus == IssuanceStatusType.Surrendered)));
            }
        }

        /// <summary>
        /// Specifies whether or not this credential can have a Must Be Maintained for Time Limited issuance
        /// </summary>
        public virtual bool CanHaveMBMForTLIssuance
        {
            get
            {
                return !IsGrandfather && !IsMBM;
            }
        }

        /// <summary>
        /// Specifies whether or not this credential in a grace period
        /// </summary>
        public virtual bool IsInGracePeriod
        {
            get
            {
                return (GracePeriodStartDate.HasValue || GracePeriodEndDate.HasValue);
            }
        }

        /// <summary>
        /// Specifies whether or not this credential is Not lapsed
        /// </summary>
        public virtual bool IsNotLapsedCredential
        {
            get
            {
                return HasIssuances && NewestIssuance.IssuanceStatus == IssuanceStatusType.Active;
            }
        }

        /// <summary>
        /// Specifies whether or not this credential is lapsed
        /// </summary>
        public virtual bool IsLapsedCredential
        {
            get
            {
                return !IsNotLapsedCredential;
            }
        }

        /// <summary>
        /// Determine whether Credential is a Timelimited or not
        /// </summary>
        /// <returns></returns>
        public virtual CredentialCategoryType CredentialCategory
        {
            get
            {
                if (IsGrandfather)
                    return CredentialCategoryType.GrandFather;
                if (IsExpiredGrandfather)
                    return CredentialCategoryType.ExpiredGrandFather;
                else if (IsTimelimited)
                    return CredentialCategoryType.TimeLimited;
                else if (IsMBM)
                    return CredentialCategoryType.MustBeMaintained;
                else if (IsInitialFPHM)
                    return CredentialCategoryType.InitialFPHM;
                else
                    return CredentialCategoryType.Unknown;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual DateTime? ExpirationDate
        {
            get
            {
                if (!HasIssuances) return null;
                return NewestIssuance.ExpirationDate;
            }
        }

        /// <summary>
        /// Specifies whether or not the Credential was elected for deselection
        /// </summary>
        public virtual bool DeselectionElected
        {
            get
            {
                if (NewestIssuance == null)
                    return false;
                else
                    return NewestIssuance.DeselectionSubmittedDate.HasValue;
            }
        }

        /// <summary>
        /// The date the diplomate elected to deselect the credential, if applicable
        /// </summary>
        public virtual DateTime? DeselectionElectedDate
        {
            get
            {
                return NewestIssuance?.DeselectionSubmittedDate;
            }
        }

        /// <summary>
        /// Specifies whether or not the Credential was processed for deselection
        /// </summary>
        public virtual bool DeselectionProcessed
        {
            get
            {
                if (NewestIssuance == null)
                    return false;
                else
                    return NewestIssuance.DeselectionProcessedDate.HasValue;
            }
        }

        /// <summary>
        /// Specifies whether or not the Credential was deselected completely.
        /// </summary>
        public virtual bool IsDeselected
        {
            get
            {
                if (NewestIssuance == null)
                    return false;
                else
                    return NewestIssuance.DeselectionProcessedDate.HasValue
                        && NewestIssuance.DeselectionEffectiveDate.HasValue
                        && NewestIssuance.DeselectionSubmittedDate.HasValue
                        && NewestIssuance.DeselectionEffectiveDate.Value.Date
                            <= NewestIssuance.DeselectionProcessedDate.Value.Date
                        && NewestIssuance.DeselectionSubmittedDate.Value.Date
                            <= NewestIssuance.DeselectionProcessedDate.Value.Date;
            }
        }

        /// <summary>
        /// The date deselection was processed for the credential, if applicable
        /// </summary>
        public virtual DateTime? DeselectionProcessedDate
        {
            get
            {
                return NewestIssuance?.DeselectionProcessedDate;
            }
        }

        /// <summary>
        /// Specifies whether or not this credential is the COVID 4 disciplines (Infectious Disease, Hospital Medicine, Critical Care, Pulmonary Disease)
        /// </summary>
        public virtual bool IsCovid4
        {
            get
            {
                return  Certification.Code == ProgramResourceConstants.CertificationCode.InfectiousDisease ||
                        Certification.Code == ProgramResourceConstants.CertificationCode.FocusedPracticeHospitalMedicine ||                      
                        Certification.Code == ProgramResourceConstants.CertificationCode.CriticalCareMedicine ||
                        Certification.Code == ProgramResourceConstants.CertificationCode.PulmonaryDisease ;
            }
        }

        #endregion DerivedProperties

        #endregion Properties

        #region Factory

        /// <summary>
        /// Initializes a new instance of the <see cref="Credential"/> class.
        /// </summary>
        protected Credential()
        {
            issuances = new List<Issuance>();
            dateLogs = new List<CredentialDateLog>();
        }

        /// <summary>
        /// Creates the credential.
        /// </summary>
        /// <param name="certification">The certification.</param>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="pathway">The pathway.</param>
        /// <param name="createdBy">The created by.</param>
        /// <param name="onBehalfBoardCode">Code of the other board.</param>
        /// <param name="onBehalfBoardName">Name of the other board.</param>
        /// <returns></returns>
        public static Credential Create(Certification certification, Guid memberId, CredentialType type, PathwayType pathway, string onBehalfBoardCode, string onBehalfBoardName, string createdBy)
        {
            var cred = new Credential()
            {
                Certification = certification,
                MemberId = memberId,
                Type = type,
                ExamFailCount = 0,
                Pathway = pathway, 
                OnBehalfBoardCode = onBehalfBoardCode, 
                OnBehalfBoardName = onBehalfBoardName, 
                IsCosponsored = !String.IsNullOrWhiteSpace(onBehalfBoardCode), 
                AuditData = AuditData.Create(createdBy)
            };
            cred.issuances = new List<Issuance>();
            cred.dateLogs = new List<CredentialDateLog>();

            return cred;
        }

        /// <summary>
        /// Creates the FPHM credential.
        /// </summary>
        /// <param name="certification">The certification.</param>
        /// <param name="source">The source.</param>
        /// <param name="memberId">The member identifier.</param>
        /// <param name="type">The type.</param>
        /// <param name="maintenanceStatus">The maintenance status.</param>
        /// <param name="issuanceDate">The issuance date.</param>
        /// <param name="pathway">The pathway.</param>
        /// <param name="examDueDate">The exam due date.</param>
        /// <param name="reattestationDate">The reattestation date.</param>
        /// <param name="createdBy">The created by.</param>
        /// <returns></returns>
        public static Credential CreateFPHM(Certification certification, Source source, Guid memberId, CredentialType type,
            MaintenanceStatusType maintenanceStatus, DateTime issuanceDate, PathwayType pathway, DateTime examDueDate, DateTime reattestationDate,
            string createdBy)
        {
            //create the credential
            var cred = new Credential()
            {
                Certification = certification,
                MemberId = memberId,
                Type = type,
                Pathway = pathway,
                IsActive = true,
                ExamDueDate = examDueDate,
                ReAttestationDueDate = reattestationDate,
                AuditData = AuditData.Create(createdBy)
            };

            //add an issuance
            cred.issuances = new List<Issuance>();
            var issuance = Issuance.Create(source,
                                          DurationType.Continuous,
                                          MaintenanceRequirementType.Required,
                                          maintenanceStatus,
                                          OccurrenceType.Recertification,
                                          IssuanceStatusType.Active,
                                          issuanceDate,
                                          createdBy);
            issuance.EffectiveDate = issuanceDate;
            issuance.ExpirationDate = null;
            issuance.ScheduledUpdate = ProgramRulesHelpers.ComputeScheduleUpdateDate();
            issuance.UnderReview = false;
            issuance.HasAdded = true;

            cred.AddIssuance(issuance);
            cred.UpdateIsActive();

            cred.dateLogs = new List<CredentialDateLog>();

            //return
            return cred;
        }

        #endregion Factory

        /// <summary>
        /// AddIssuance method.
        /// </summary>
        /// <param name="issuance"></param>
        public virtual void AddIssuance(Issuance issuance)
        {
            issuance.Credential = this;
            issuances.Add(issuance);
            UpdateIsActive();
        }

        /// <summary>
        /// AddIssuances method.
        /// </summary>
        /// <param name="issuances"></param>
        public virtual Credential AddIssuances(IList<Issuance> issuances)
        {
            foreach (var issuance in issuances)
                AddIssuance(issuance);

            return this;
        }

        /// <summary>
        /// Removes the issuance.
        /// </summary>
        /// <param name="issuance">The issuance.</param>
        public virtual void RemoveIssuance(Issuance issuance)
        {
            issuances.Remove(issuance);
            UpdateIsActive();
        }

        /// <summary>
        /// Updates the IsActive boolean
        /// </summary>
        public virtual void UpdateIsActive()
        {
            IsActive = Issuances.Any(i => i.IssuanceStatus == IssuanceStatusType.Active);
        }

        /// <summary>
        /// Gets the issuance.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public virtual Issuance GetIssuance(int id)
        {
            if (issuances == null) return null;
            return Issuances.SingleOrDefault(i => i.Id.Equals(id));
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
            var other = obj as Credential;
            if (other == null)
                return false;

            return (ExternalId == other.ExternalId);
        }/// <summary>
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
        /// Applies the changes after creating credential.
        /// </summary>
        /// <param name="isActive">if set to <c>true</c> [is active].</param>
        /// <param name="gracePeriodStartDate">The grace period start date.</param>
        /// <param name="gracePeriodEndDate">The grace period end date.</param>
        /// <param name="examDueDate">The exam due date.</param>
        /// <param name="mocExamDueDate">The MOC exam due date.</param>
        /// <param name="kciExamDueDate">The KCI exam due date.</param>
        /// <param name="displayExamDueDate">The Display exam due date.</param>
        /// <param name="consecutiveKciPassRequired">Specifies whether or not consecutive KCI exam passes are required.</param>
        /// <param name="reAttestationDueDate">The re-attestation due date.</param>
        public virtual void ApplyChangesAfterCreatingCredential(
            bool isActive, 
            DateTime? gracePeriodStartDate, 
            DateTime? gracePeriodEndDate,
            DateTime? examDueDate, 
            DateTime? mocExamDueDate, 
            DateTime? kciExamDueDate, 
            DateTime? displayExamDueDate, 
            bool consecutiveKciPassRequired, 
            DateTime? reAttestationDueDate)
        {
            IsActive = isActive;
            GracePeriodStartDate = gracePeriodStartDate;
            GracePeriodEndDate = gracePeriodEndDate;
            ExamDueDate = examDueDate;
            MOCExamDueDate = mocExamDueDate;
            KCIExamDueDate = kciExamDueDate;
            DisplayExamDueDate = displayExamDueDate;
            ConsecutiveKCIPassRequired = consecutiveKciPassRequired;
            ReAttestationDueDate = reAttestationDueDate;

            UpdateIsActive();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assessmentMetDate"></param>
        /// <param name="examDueDate"></param>
        /// <param name="reAttestationDueDate"></param>
        /// <param name="source"></param>
        /// <param name="maintenanceStatus"></param>
        /// <param name="scheduledUpdate"></param>
        /// <param name="displayExamDueDate"></param>
        /// <param name="kciExamDueDate"></param>
        /// <param name="mocExamDueDate"></param>
        /// <param name="consecutiveKciPassRequired"></param>
        /// <param name="createdBy"></param>
        public virtual void ApplyChangesForCreateCredential(DateTime assessmentMetDate,
                                                            DateTime examDueDate,
                                                            DateTime? reAttestationDueDate,
                                                            Source source,
                                                            MaintenanceStatusType maintenanceStatus,
                                                            DateTime scheduledUpdate,
                                                            DateTime? displayExamDueDate,
                                                            DateTime? kciExamDueDate,
                                                            DateTime? mocExamDueDate,
                                                            bool consecutiveKciPassRequired,
                                                            string createdBy)
        {
            AssessmentMet = true;
            AssessmentMetDate = assessmentMetDate;
            ExamDueDate = examDueDate;
            ForcedPathway = false;
            GracePeriodStartDate = null;
            GracePeriodEndDate = null;
            IsActive = true;
            LookbackDate = null;
            SkippedExamLookbackDate = null;
            ReAttestationDueDate = reAttestationDueDate;

            DisplayExamDueDate = displayExamDueDate;
            KCIExamDueDate = kciExamDueDate;
            MOCExamDueDate = mocExamDueDate;
            ConsecutiveKCIPassRequired = consecutiveKciPassRequired;

            var issuance = Issuance.Create(source,
                                DurationType.Continuous,
                                MaintenanceRequirementType.Required,
                                maintenanceStatus,
                                OccurrenceType.Initial,
                                IssuanceStatusType.Active,
                                assessmentMetDate,
                                createdBy);

            issuance.EffectiveDate = assessmentMetDate;
            issuance.ExpirationDate = null;
            issuance.ScheduledUpdate = scheduledUpdate;
            issuance.UnderReview = false;
            issuance.HasAdded = true;

            AddIssuance(issuance);
        }


        /// <summary>
        /// Expires the old issuance and reissues a new issuance which is set to a given maintenance status type.
        /// </summary>
        /// <param name="existingIssuanceId">The existing issuance identifier.</param>
        /// <param name="source">The source.</param>
        /// <param name="maintenanceStatus">The maintenance status.</param>
        /// <param name="issuanceDate">The issuance date.</param>
        /// <param name="scheduledUpdate">The scheduled update.</param>
        /// <param name="createdBy">The created by.</param>
        public virtual void ExpireAndReissue(int existingIssuanceId, Source source, MaintenanceStatusType maintenanceStatus,
            DateTime issuanceDate, DateTime scheduledUpdate, string createdBy)
        {
            var newIssuance = Issuance.Create(source,
                                        DurationType.Continuous,
                                        MaintenanceRequirementType.Required,
                                        maintenanceStatus, //MaintenanceStatusType
                                        OccurrenceType.Recertification,
                                        IssuanceStatusType.Active,
                                        issuanceDate,
                                        createdBy);

            //// set rest of values for new issuance record
            newIssuance.EffectiveDate = issuanceDate;
            newIssuance.ExpirationDate = null;
            newIssuance.ScheduledUpdate = scheduledUpdate;
            newIssuance.UnderReview = false;
            newIssuance.HasAdded = true;

            var existingIssuance = GetIssuance(existingIssuanceId);
            existingIssuance.IssuanceStatus = IssuanceStatusType.Expired;
            existingIssuance.ExpiredDate = issuanceDate; //pbi 142306 : issue mbm for tl, set expired date of tl being expired
            existingIssuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            existingIssuance.AuditData.ModifiedBy = createdBy;
            existingIssuance.AuditData.Modified = DateTime.Now;
            existingIssuance.HasChanged = true;

            /*
            PBI 183137
            If a new issuance occurs after a cert is deselected, then the 
            deselection will be carried over to the new issuance and the 
            deselection will be removed from the original issuance.
            */
            CarryOverIssuanceDeselectValuesIfApplicable(newIssuance, existingIssuance);

            AddIssuance(newIssuance);

            //AddIssuance already calls this but let's set a habit of having this as the last line of every Apply() so we don't miss one or
            //... confuse future developers as to why one isn't there
            UpdateIsActive();
        }

        /// <summary>
        /// Reissues a new issuance which is set to Active.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="issuanceDate"></param>
        /// <param name="scheduledUpdate"></param>
        /// <param name="createdBy"></param>
        /// <param name="maintenanceStatus"></param>
        public virtual void Reissue(Source source,
                                    DateTime issuanceDate,
                                    DateTime scheduledUpdate,
                                    string createdBy,
                                    MaintenanceStatusType maintenanceStatus = MaintenanceStatusType.Maintained)
        {
            var newIssuance = Issuance.Create(source,
                                        DurationType.Continuous,
                                        MaintenanceRequirementType.Required,
                                        maintenanceStatus,
                                        OccurrenceType.Recertification,
                                        IssuanceStatusType.Active,
                                        issuanceDate,
                                        createdBy);

            newIssuance.EffectiveDate = issuanceDate;
            newIssuance.ExpirationDate = null;
            newIssuance.ScheduledUpdate = scheduledUpdate;
            newIssuance.UnderReview = false;
            newIssuance.HasAdded = true;

            var existingIssuance = NewestIssuance;

            //PBI 183137
            //If a new issuance is created, but the previous one had been slated 
            //for deselection, carry over the deselect values so that the new issuance 
            //will eventually be deselected. Then, clear them from the existing issuance.
            CarryOverIssuanceDeselectValuesIfApplicable(newIssuance, existingIssuance);

            AddIssuance(newIssuance);

            //AddIssuance already calls this but let's set a habit of having this as the last line of every Apply() so we don't miss one or
            //... confuse future developers as to why one isn't there
            UpdateIsActive();
        }

        /// <summary>
        /// ReinstateTL
        /// </summary>
        /// <param name="modifiedBy"></param>
        /// <param name="issuanceStatus"></param>
        /// <param name="maintenanceStatus"></param>
        public virtual void ReinstateTL(string modifiedBy,
                                        IssuanceStatusType issuanceStatus,
                                        MaintenanceStatusType maintenanceStatus)
        {

            NewestIssuance.IssuanceStatus = issuanceStatus;
            NewestIssuance.SetModified(modifiedBy);
            NewestIssuance.MaintenanceStatus = maintenanceStatus;
            NewestIssuance.ExpiredDate = null;
            NewestIssuance.HasChanged = true;

            HasChanged = true;
            UpdateIsActive();
        }


        /// <summary>
        /// Apply method.
        /// </summary>
        /// <param name="modifiedBy"></param>
        public virtual void SetMaintained(string modifiedBy)
        {
          
            var activeIssuances = new List<Issuance>();

            // if GF: update each active grandfather(lifetime) and active time - limited issuance for the credential
            if (IsGrandfather)
            {
                activeIssuances = Issuances.Where(a => a.IssuanceStatus == IssuanceStatusType.Active 
                            && ( a.Duration==DurationType.Lifetime || a.Duration == DurationType.Timelimited )).ToList();
            }
            // if TL or MBM: update each active issuance for the credential
            else
            {
                activeIssuances = Issuances.Where(a => a.IssuanceStatus == IssuanceStatusType.Active).ToList();
            }   

            if (activeIssuances.Count == 0)
                throw new Exception("Cannot apply SetIssuanceToMaintainedEvent because there are no issuances in credential " + ExternalId);

            foreach (var issuance in activeIssuances)
            {
                issuance.MaintenanceStatus = MaintenanceStatusType.Maintained;
                issuance.AuditData.Modified = DateTime.Now;
                issuance.AuditData.ModifiedBy = modifiedBy;
                issuance.HasChanged = true;
            }

            UpdateIsActive();
        }

        /// <summary>
        /// Sets the maintained previous year for tl and MBM.
        /// </summary>
        /// <param name="modifiedBy">The modified by.</param>
        /// <param name="maintenanceStatus">The maintenance status.</param>
        /// <param name="processingDate">The processing Date.</param>
        /// <exception cref="System.Exception">Cannot apply SetMaintainedForTLandMBM because there are no issuances in credential {0}" + ExternalId</exception>
        public virtual void SetMaintainedForTLandMBM(string modifiedBy, MaintenanceStatusType maintenanceStatus, DateTime processingDate)
        {

            DateTime expiredDate = new DateTime(processingDate.Year - 1, 12, 31);

            if (!HasIssuances)
                throw new Exception("Cannot apply SetMaintainedForTLandMBM because there are no issuances in credential {0}" + ExternalId);

            var issuancesToUpdate = issuances.Where(i => i.ExpirationDate.HasValue && i.ExpirationDate.Value.Date== expiredDate.Date );

            foreach (var issuance in issuancesToUpdate)
            {
                issuance.IssuanceStatus = IssuanceStatusType.Active;
                issuance.MaintenanceStatus = maintenanceStatus;
                issuance.AuditData.Modified = DateTime.Now;
                issuance.AuditData.ModifiedBy = modifiedBy;
                issuance.ExpirationDate = null;
                issuance.HasChanged = true;
            }

            GracePeriodStartDate = new DateTime(processingDate.Year, 1, 1);
            GracePeriodEndDate = new DateTime(processingDate.Year, 12, 31);
            AssessmentMet = true;
            IsActive = true;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        /// Reissues a new issuance which is set to Active.
        /// </summary>
        /// <param name="modifiedBy">modifiedBy.</param>
        /// <param name="maintenanceStatusType">The maintenance Status date.</param>
        /// <param name="processingDate">The processing Date.</param>
        public virtual void SetMaintainedPreviousYearForGF(string modifiedBy, MaintenanceStatusType maintenanceStatusType, DateTime processingDate)
        {

            if (!HasIssuances)
                throw new Exception("Cannot apply SetMaintainedPreviousYearForGF because there are no issuances in credential {0}" + ExternalId);

            var issuancesToUpdate = issuances.Where(i => i.Duration == DurationType.Lifetime
                                    && i.IssuanceStatus == IssuanceStatusType.Active
                                    && i.MaintenanceStatus == MaintenanceStatusType.NotMaintained);

            foreach (var issuance in issuancesToUpdate)
            {
                issuance.MaintenanceStatus = maintenanceStatusType;
                issuance.AuditData.Modified = DateTime.Now;
                issuance.AuditData.ModifiedBy = modifiedBy;
                issuance.HasChanged = true;
            }

            GracePeriodStartDate = new DateTime(processingDate.Year, 1, 1);
            GracePeriodEndDate = new DateTime(processingDate.Year, 12, 31);
            AssessmentMet = true;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        /// Set Active Issuances according to rule in pbi: 94842 ( update credential on exam result event)
        /// </summary>
        /// <param name="modifiedBy"></param>
        /// <param name="expirationDate"></param>
        /// <param name="processingDate"></param>
        public virtual void SetExpireActiveIssuances(string modifiedBy, DateTime? expirationDate, DateTime processingDate)
        {
        /*
           expire active issuance(which includes setting maintenance status to false)  
           if certificate if time - limited and past expiration date or must be maintained
                set issuance.issuancestatus = Expired
                set issuance.ExpirationDate to 12 / 31 / year of exam administration if the field is null
                set issuance.maintenancestatus = 0
                set credential.isactive = 0 if there are no other active issuances for this credential
          set maintenance status to false if grandfather
        */

            // if cert is Time Limited and pass expiration date or Must Be Maintained 
                    if ((IsTimelimited
                && ExpirationDate.HasValue
                && processingDate.Date >= ExpirationDate.Value.Date)
                 || IsMBM)
            {
                foreach (var issuance in Issuances.Where(a => a.IssuanceStatus == IssuanceStatusType.Active))
                {
                    issuance.IssuanceStatus = IssuanceStatusType.Expired;
                    issuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
                    issuance.ExpirationDate = issuance.ExpirationDate?? expirationDate?? processingDate;
                    issuance.AuditData.Modified = DateTime.Now;
                    issuance.AuditData.ModifiedBy = modifiedBy;
                    issuance.HasChanged = true;
                }
            }
            // for Grandfather only
            if (IsGrandfather)
                NewestIssuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expireActiveIssuances"></param>
        /// <param name="examDueDate"></param>
        /// <param name="displayExamDueDate"></param>
        /// <param name="mocExamDueDate"></param>
        /// <param name="kciExamDueDate"></param>
        /// <param name="forcedPathway"></param>
        /// <param name="pathway"></param>
        /// <param name="examFailCount"></param>
        /// <param name="assessmentMet"></param>
        /// <param name="assessmentMetDate"></param>
        /// <param name="gracePeriodEndDate"></param>
        /// <param name="gracePeriodStartDate"></param>
        /// <param name="modifiedBy"></param>
        /// <param name="expirationDate"></param>
        /// <param name="isInCMP"></param>
        /// <param name="processingDate"></param>
        public virtual void SetCredentialOnExamResult(bool expireActiveIssuances,
                                    DateTime? examDueDate, 
                                    DateTime? displayExamDueDate, 
                                    DateTime? mocExamDueDate, 
                                    DateTime? kciExamDueDate, 
                                    bool forcedPathway,
                                    PathwayType pathway,
                                    int examFailCount,
                                    bool assessmentMet,
                                    DateTime? assessmentMetDate,
                                    DateTime? gracePeriodEndDate,
                                    DateTime? gracePeriodStartDate,
                                    string modifiedBy,
                                    DateTime? expirationDate,
                                    bool isInCMP,
                                    DateTime processingDate)
        {
            if (expireActiveIssuances)
                SetExpireActiveIssuances(modifiedBy, expirationDate, processingDate);

            ExamDueDate = examDueDate;
            DisplayExamDueDate = displayExamDueDate;
            MOCExamDueDate = mocExamDueDate;
            KCIExamDueDate = kciExamDueDate;
            ForcedPathway = forcedPathway;
            Pathway = pathway;
            ExamFailCount = examFailCount;
            AssessmentMet = assessmentMet;
            AssessmentMetDate = assessmentMetDate;
            GracePeriodEndDate = gracePeriodEndDate;
            GracePeriodStartDate = gracePeriodStartDate;

            IsInCMP = isInCMP;

            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;

            UpdateIsActive();
          
        }

        /// <summary>
        /// updates the SelectedToMaintain flag of the <see cref="Credential"/> domain object
        /// </summary>
        /// <param name="selectedToMaintain"></param>
        /// <param name="modifiedBy"></param>
        public virtual void SetSelectedToMaintain(bool selectedToMaintain, string modifiedBy)
        {
            SelectedToMaintain = selectedToMaintain;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        /// Updates the pathway of the <see cref="Credential"/> domain object. 
        /// This also changes the DisplayExamDueDate if the new pathway is MOC.
        /// </summary>
        /// <param name="pathway">The pathway value</param>
        /// <param name="modifiedBy">The user or procdess modifiying the credential</param>
        public virtual void SetPathway(PathwayType pathway, string modifiedBy)
        {
            //PBI 136142
            Pathway = pathway;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;

            // Pbi 150792 : (Proj 1444 - Rel 3) switching pathways - don't change date << ar@6/5/2019 >>
            ////If pathway is MOC, set DisplayExamDueDate to MOCExamDueDate or keep as is, depending on 
            ////which value is greater
            //if (pathway == PathwayType.MOC)
            //    DisplayExamDueDate = new List<DateTime?>(2) { DisplayExamDueDate, MOCExamDueDate }.Max();

            //Bug 224230 - If enrolling in LKA, clear the IsInCMP flag
            if (pathway != PathwayType.OneYear)
                IsInCMP = false;
        }

        /// <summary>
        /// updates the GrandfatherMOCPrintDate DateTime field of the <see cref="Credential"/> domain object
        /// </summary>
        /// <param name="grandfatherMOCPrintDate"></param>
        /// <param name="modifiedBy"></param>
        public virtual void SetGrandfatherMOCPrintDate(DateTime grandfatherMOCPrintDate, string modifiedBy)
        {
            GrandfatherMOCPrintDate = grandfatherMOCPrintDate;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        ///  updates the Credential's WithdrawnDate and NewestIssuance status of the <see cref="Credential"/> domain object
        /// </summary>
        /// <param name="withdrawnDate"></param>
        /// <param name="withdrawnStatus"></param>
        /// <param name="modifiedBy"></param>
        public virtual void SetWithdrawn(DateTime withdrawnDate, IssuanceStatusType withdrawnStatus,string modifiedBy)
        {
            WithdrawnDate = withdrawnDate;

            NewestIssuance.IssuanceStatus = withdrawnStatus;
            NewestIssuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;

            NewestIssuance.ExpirationDate = withdrawnDate; // per Jeff M. 
            // ar@10/09/2019 : I think it should be here ....
            NewestIssuance.ExpiredDate = withdrawnDate;

            NewestIssuance.AuditData.Modified = DateTime.Now;
            NewestIssuance.AuditData.ModifiedBy = modifiedBy;
            NewestIssuance.HasChanged = true;

            // find any active issuances and set them to proper status
            foreach ( var iss in  Issuances.Where(i => i.IssuanceStatus == IssuanceStatusType.Active))
            {
                iss.IssuanceStatus = withdrawnStatus;
                iss.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
                iss.ExpirationDate = withdrawnDate; // per Jeff M. 
                iss.AuditData.Modified = DateTime.Now;
                iss.AuditData.ModifiedBy = modifiedBy;
                iss.HasChanged = true;
            }

            IsActive = false;

        }

        /// <summary>
        ///  updates the Credential's WithdrawnDate and NewestIssuance status of the <see cref="Credential"/> domain object
        /// </summary>
        /// <param name="modifiedBy"></param>
        public virtual void SetReinstate(string modifiedBy)
        {

            NewestIssuance.IssuanceStatus = IssuanceStatusType.Expired;
            NewestIssuance.AuditData.Modified = DateTime.Now;
            NewestIssuance.AuditData.ModifiedBy = modifiedBy;
            NewestIssuance.HasChanged = true;

            // find any other issuances (Revoked,Surrendered,Suspended) and set them to proper status
            foreach (var iss in Issuances.Where(i => i.IssuanceStatus == IssuanceStatusType.Revoked ||
                                                    i.IssuanceStatus == IssuanceStatusType.Surrendered ||
                                                    i.IssuanceStatus == IssuanceStatusType.Suspended))
            {
                iss.IssuanceStatus = IssuanceStatusType.Expired;
                iss.AuditData.Modified = DateTime.Now;
                iss.AuditData.ModifiedBy = modifiedBy;
                iss.HasChanged = true;
            }

        }

        /// <summary>
        /// Apply Update Credential Event
        /// </summary>
        public virtual void ApplyUpdateCredentialEvent(PathwayType pathway, 
                                                        CredentialType type,
                                                        string modifiedBy)
        {
            Pathway = pathway;
            Type = type;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        /// SetEnrollInCMP
        /// </summary>
        /// <param name="modifiedBy"></param>
        /// <param name="EnrollmentDate"></param>
        public virtual void SetEnrollInCMP(string modifiedBy, DateTime EnrollmentDate)
        {
            //PBI 136142
            IsInCMP = true;
            Pathway = PathwayType.OneYear; 
            CMPEnrollmentDate = EnrollmentDate;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        /// SetEnrollInCMP
        /// </summary>
        /// <param name="modifiedBy"></param>
        /// <param name="priviousPathway"></param>
        /// <param name="UnEnrollmentDate"></param>
        public virtual void SetUnEnrollInCMP(string modifiedBy, PathwayType priviousPathway,  DateTime UnEnrollmentDate)
        {
            //PBI 151637
            IsInCMP = false;
            Pathway = priviousPathway; // set pro privious pathway
            //ar@ 7/15/2019: I assume it is better to keep that Date unchanged .
            //CMPEnrollmentDate = EnrollmentDate;
            AuditData.Modified = DateTime.Now;
            AuditData.ModifiedBy = modifiedBy;
        }

        /// <summary>
        /// Deselects the Credential
        /// </summary>
        /// <param name="modifiedBy">The name of the user performing the deselect operation</param>
        /// <param name="expiredDate">The date the latest issuance should be considered expired</param>
        /// <param name="processedDate">Processed date</param>
        public virtual void Deselect(string modifiedBy, DateTime expiredDate, DateTime? processedDate = null)
        {
            var latestIssuance = NewestIssuance;

            if (ShouldExpireIssuanceOnDeselect())
            {
                latestIssuance.ExpiredDate = expiredDate;
                latestIssuance.IssuanceStatus = IssuanceStatusType.Expired;
                latestIssuance.MaintenanceStatus = MaintenanceStatusType.NotMaintained;
            }

            if (processedDate.HasValue)
            {
                latestIssuance.DeselectionProcessedDate = processedDate.Value;
            }
            else
            {
                latestIssuance.DeselectionProcessedDate = DateTime.Now;
            }

            latestIssuance.AuditData.ModifiedBy = modifiedBy;
            latestIssuance.AuditData.Modified = DateTime.Now;
            latestIssuance.HasChanged = true;

            IsActive = false;

            HasChanged = true;
        }

        #endregion Apply Methods

        #region Private Methods        

        private void AddCredentialDateLog(CredentialDateType dateType, DateTime? oldValue, DateTime? newValue)
        {
            if (dateLogs == null) dateLogs = new List<CredentialDateLog>();
            dateLogs.Add(CredentialDateLog.Create(this, dateType, oldValue, newValue, DateTime.Now, "ValueChange"));
        }

        private bool ShouldExpireIssuanceOnDeselect()
        {
            return NewestIssuance.IssuanceStatus == IssuanceStatusType.Active;
        }

        private bool ShouldCarryOverIssuanceDeselectValues(Issuance latestIssuance)
        {
            //If the latest issuance has been flagged for deselection, but it hasn't 
            //yet been processed for deselection, return true.
            //We pass latestIssuance in as a param rather than using the 
            //NewestIssuance property because in some cases we've already retrieved 
            //this variable and will be setting some of its properties elsewhere.
            return latestIssuance != null
                && latestIssuance.DeselectionEffectiveDate.HasValue
                && latestIssuance.DeselectionProcessedDate == null;
        }

        private void CarryOverIssuanceDeselectValuesIfApplicable(Issuance newIssuance, Issuance latestIssuance)
        {
            if (ShouldCarryOverIssuanceDeselectValues(latestIssuance))
            {
                /*
                PBI 183137
                The existing issuance was marked for deselection, but the deselection has not yet 
                been processed. Set the deselection values on the new issuance to the equal what's 
                on the existing issuance, then clear these values on the existing issuance.
                */
                newIssuance.DeselectionSubmittedDate = latestIssuance.DeselectionSubmittedDate;
                newIssuance.DeselectionEffectiveDate = latestIssuance.DeselectionEffectiveDate;
                latestIssuance.DeselectionSubmittedDate = null;
                latestIssuance.DeselectionEffectiveDate = null;
                latestIssuance.HasChanged = true;
            }
        }

        #endregion Private Methods
    }

    #region Validator Classes

    /// <summary>
    /// Validates a credential
    /// </summary>
    /// <seealso cref="FluentValidation.AbstractValidator{Credential}" />
    public class CredentialValidator : AbstractValidator<Credential>
    {
        /// <summary>
        /// Creates a new instance of the CredentialValidator object.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public CredentialValidator(IValidationFactory factory)
        {
            RuleFor(x => x.MemberId).NotEqual(Guid.Empty)
                .WithMessage("MemberId is required");

            RuleFor(x => x.Certification).NotNull()
                .WithMessage("Certification is required");
            RuleFor(x => x.Certification).SetValidator(factory.GetValidatorInstance<Certification>());

            RuleFor(x => x.Issuances).SetValidator(factory.GetValidatorInstance<IReadOnlyList<Issuance>>());

            RuleFor(x => x.AuditData).NotNull()
                .WithMessage("AuditData cannot be null");
            RuleFor(x => x.AuditData).SetValidator(factory.GetValidatorInstance<AuditData>());
        }
    }
    /// <summary>
    /// 
    /// </summary>
    public class CredentialIssuancesValidator : AbstractValidator<IReadOnlyList<Issuance>>
    {
        /// <summary>
        /// Creates a new instance of the CredentialIssuancesValidator object.
        /// </summary>
        public CredentialIssuancesValidator(IValidationFactory factory)
        {
            RuleFor(x => x).NotNull()
                .WithMessage("Issuances cannot be null");

            RuleForEach<Issuance>(x => x.ToList()).SetValidator(factory.GetValidatorInstance<Issuance>());
        }
    }

    #endregion Validator Classes
}
