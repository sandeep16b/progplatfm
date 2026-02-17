using Abim.Enterprise.Core.Relational.Validation;
using Abim.Enterprise.Core.Relational.Validation.Impl;
using Abim.Platform.Program.App.Services.CommandResults.Credential;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Services.CommandResults.ProgramRules
{
    /// <summary>
    /// HoldProgramRuleCommandResult Class.
    /// </summary>
    public class HoldProgramRuleCommandResult : CredentialCommandResult 
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public HoldProgramRuleCommandResult(CommandStatus status, AbimValidationResult validation, App.Domain.Credential data)
            : base(status, validation, data)
        {
        }
        
        /// <summary>
        /// Parameterless Public Constructor
        /// </summary>
        public HoldProgramRuleCommandResult() : base ()
        {
        }
    }
}
