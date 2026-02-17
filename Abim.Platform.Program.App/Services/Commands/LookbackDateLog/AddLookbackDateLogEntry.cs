using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Services;
using System;

namespace Abim.Platform.Program.App.Services.Commands.LookbackDateLog
{
    /// <summary>
    /// AddLookbackDateLogEntry
    /// </summary>
    public class AddLookbackDateLogEntry: ICommand
    {
        /// <summary>
        /// ChangedDate
        /// </summary>
        public DateTime ChangedDate { get; protected internal set; }
        /// <summary>
        /// MemberGuid
        /// </summary>
        public Guid MemberGuid { get; protected internal set; }
        /// <summary>
        /// LookbackDate
        /// </summary>
        public LookbackDateType LookbackDate { get; protected internal set; }
        /// <summary>
        /// NewValue
        /// </summary>
        public DateTime? NewValue { get; protected internal set; }
        /// <summary>
        /// OldValue
        /// </summary>
        public DateTime? OldValue { get; protected internal set; }
        /// <summary>
        /// UserName
        /// </summary>
        public string UserName { get; set; }
    }
}
