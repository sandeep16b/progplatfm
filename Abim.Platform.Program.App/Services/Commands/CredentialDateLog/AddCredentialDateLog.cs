using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;
using System;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// AddCredentialDateLog
    /// </summary>
    public class AddCredentialDateLog : ICommand
    {
        /// <summary>
        /// CredentialId
        /// </summary>
        public Guid CredentialId { get; set; }
        /// <summary>
        /// DateType
        /// </summary>
        public CredentialDateType DateType { get; set; }
        /// <summary>
        /// OldValue
        /// </summary>
        public DateTime? OldValue { get; set; }
        /// <summary>
        /// NewValue
        /// </summary>
        public DateTime? NewValue { get; set; }
        /// <summary>
        /// ChangedDate
        /// </summary>
        public DateTime ChangedDate { get; set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
    }
}
