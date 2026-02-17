using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Resources;
using Embarr.WebAPI.AntiXss;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Creates a credential itself (not an issuance)
    /// </summary>
    /// <seealso cref="ICommand" />
    public class CreateCredentialIssuanceCommand : ICommand
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
        /// 
        /// </summary>
        public DateTime AssessmentMetDate { get; set; }

        /// <summary>
        /// Gets or sets the exam due date.
        /// </summary>
        public DateTime ExamDueDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? ReAttestationDueDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MaintenanceStatusType MaintenanceStatus { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime ScheduledUpdate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string CredentialCreatedBy { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [AntiXss]  //all public string properties with setters in our command classes get this attribute
        public string IssuanceCreatedBy { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? DisplayExamDueDate { get; set; } 
        /// <summary>
        /// 
        /// </summary>
        public DateTime? KCIExamDueDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? MOCExamDueDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool ConsecutiveKCIPassRequired { get; set; }

        /// <summary>
        /// The code of the board the credential was created on behalf of
        /// </summary>
        public string OnBehalfBoardCode { get; set; }

        /// <summary>
        /// The name of the board the credential was created on behalf of
        /// </summary>
        public string OnBehalfBoardName { get; set; }

        /// <summary>
        /// Indicates whether or not the credential is cosponsored
        /// </summary>
        public bool IsCosponsored { get { return !String.IsNullOrWhiteSpace(OnBehalfBoardCode); } }

        #endregion
    }
}
