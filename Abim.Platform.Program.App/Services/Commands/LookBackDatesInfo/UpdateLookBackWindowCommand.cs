using System;
using Newtonsoft.Json;
using Abim.Platform.Program.Relational.Services;

namespace Abim.Platform.Program.App.Services.Commands
{
    /// <summary>
    /// Class UpdateLookBackDatesInfoCommand.
    /// </summary>
    /// <seealso cref="ICommand" />
    public class UpdateLookBackDatesInfoCommand : ICommand
    {

        /// <summary>
        /// Gets or sets the user identifier.
        /// </summary>
        /// <value>
        /// The user identifier.
        /// </value>
        [JsonIgnore]
        public Guid MemberId { get; set; }

        /// <summary>
        /// Lookback2YearStartDate
        /// </summary>
        public virtual DateTime? Lookback2YearStartDate { get; set; }

        /// <summary>
        /// Lookback2YearEndDate
        /// </summary>
        public virtual DateTime? Lookback2YearEndDate { get; set; }

        /// <summary>
        /// Lookback5YearEndDate
        /// </summary>
        public virtual DateTime? Lookback5YearEndDate { get; set; }

        /// <summary>
        /// Lookback5YearStartDate
        /// </summary>
        public virtual DateTime? Lookback5YearStartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public virtual string UserName { get; set; }
    }
}
