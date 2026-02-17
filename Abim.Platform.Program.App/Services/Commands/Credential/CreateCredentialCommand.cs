using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Newtonsoft.Json;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Creates a credential itself (not an issuance)
    /// </summary>
    /// <seealso cref="ICommand" />
    public class CreateCredentialCommand : ICommand
    {
        #region Properties

        /// <summary>
        /// Gets or sets the certification identifier.
        /// </summary>
        /// <value>
        /// The certification identifier.
        /// </value>
        public Guid CertificationId { get; set; }

        /// <summary>
        /// Gets or sets the member identifier.
        /// </summary>
        /// <value>
        /// The member identifier.
        /// </value>
        public Guid MemberId { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public CredentialType Type { get; set; }

        /// <summary>
        /// Gets or sets the pathway.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public PathwayType Pathway { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the grace period start date.
        /// </summary>
        /// <value>
        /// The grace period start date.
        /// </value>
        public DateTime? GracePeriodStartDate { get; set; }

        /// <summary>
        /// Gets or sets the grace period end date.
        /// </summary>
        /// <value>
        /// The grace period end date.
        /// </value>
        public DateTime? GracePeriodEndDate { get; set; }

        /// <summary>
        /// Gets or sets the exam due date.
        /// </summary>
        /// <value>
        /// The exam due date.
        /// </value>
        public DateTime? ExamDueDate { get; set; }

        /// <summary>
        /// Gets or sets the MOC exam due date.
        /// </summary>
        /// <value>
        /// The MOC exam due date.
        /// </value>
        public DateTime? MOCExamDueDate { get; set; }

        /// <summary>
        /// Gets or sets the KCI exam due date.
        /// </summary>
        /// <value>
        /// The KCI exam due date.
        /// </value>
        public DateTime? KCIExamDueDate { get; set; }

        /// <summary>
        /// Gets or sets the Display exam due date.
        /// </summary>
        /// <value>
        /// The Display exam due date
        /// </value>
        public DateTime? DisplayExamDueDate { get; set; }

        /// <summary>
        /// Gest or sets the Consecutive KCI pass required
        /// </summary>
        /// <value>
        /// true if consecutive KCI exam passes are required; false otherwise
        /// </value>
        public bool ConsecutiveKCIPassRequired { get; set; }

        /// <summary>
        /// Gets or sets the ReAttestation due date.
        /// </summary>
        /// <value>
        /// The ReAttestation due date.
        /// </value>
        public DateTime? ReAttestationDueDate { get; set; }

        /// <summary>
        /// Gets or sets the user information.
        /// </summary>
        /// <value>
        /// The user information.
        /// </value>
        [JsonIgnore]
        public UserInfo UserInfo { get; set; }

        /// <summary>
        /// Gets or sets the re Processing date.
        /// </summary>
        public DateTime ProcessingDate { get; set; }

        #endregion
    }
}
