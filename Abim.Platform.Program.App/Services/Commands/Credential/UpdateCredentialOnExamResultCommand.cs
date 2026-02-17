using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Embarr.WebAPI.AntiXss;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Command to set an Issuance to be 'Maintained'
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.Relational.Services.ICommand" />
    public class UpdateCredentialOnExamResultCommand : ICommand
    {
        /// <summary>
        /// Id of the Credential
        /// </summary>
        public Guid CredentialId { get; set; }

        /// <summary>
        /// The audit username
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string ModifiedBy { get; set; }

        /// <summary>
        /// DateTime when processing is taking place
        /// </summary>
        public DateTime ProcessingDate { get; set; }

        /// <summary>
        /// Gets or sets the pathway.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public PathwayType Pathway { get; protected internal set; }

        /// <summary>
        /// Gets or sets the exam fail count.
        /// </summary>
        /// <value>
        /// The exam fail count.
        /// </value>
        public int ExamFailCount { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Administration Date.
        /// </summary>
        /// <value>
        /// The pathway.
        /// </value>
        public DateTime? AdministrationDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the ForcedPathway bool.
        /// </summary>
        /// <value>
        /// whether it's a forced pathway credential.
        /// </value>
        public bool ForcedPathway { get; protected internal set; }

        /// <summary>
        /// Whether the examType has been met.
        /// </summary>
        /// <value>
        /// true or false.
        /// </value>
        public bool AssessmentMet { get; protected internal set; }

        /// <summary>
        /// The date the examType was met.
        /// </summary>
        /// <value>
        /// A datetime.
        /// </value>
        public DateTime? AssessmentMetDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the grace period start date.
        /// </summary>
        /// <value>
        /// The grace period start date.
        /// </value>
        public DateTime? GracePeriodStartDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the grace period end date.
        /// </summary>
        /// <value>
        /// The grace period end date.
        /// </value>
        public DateTime? GracePeriodEndDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the next exam due date.
        /// </summary>
        /// <value>
        /// The next exam due date.
        /// </value>
        public DateTime? ExamDueDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the display exam due date
        /// </summary>
        /// <value>
        /// The exam due date to display to users
        /// </value>
        public DateTime? DisplayExamDueDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the MOC exam due date
        /// </summary>
        /// <value>
        /// The MOC exam due date
        /// </value>
        public DateTime? MOCExamDueDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the KCI exam due date
        /// </summary>
        /// <value>
        /// The KCI exam due date
        /// </value>
        public DateTime? KCIExamDueDate { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Active Issuances
        /// </summary>
        public bool ExpireActiveIssuances { get; protected internal set; }

        /// <summary>
        /// Gets or sets the Is In CMP flag
        /// </summary>
        public bool IsInCMP { get; protected internal set; }

    }
}
