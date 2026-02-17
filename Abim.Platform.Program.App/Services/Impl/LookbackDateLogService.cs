using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults.LookbackDateLog;
using Abim.Platform.Program.App.Services.Commands.LookbackDateLog;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Relational.Services.Impl;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using MassTransit;
using NLog;
using ServiceStack.Text;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// LookbackDateLogService
    /// </summary>
    public class LookbackDateLogService:
        ServiceBase<LookbackDateLog, ILookbackDateLogRepository>,
        ILookbackDateLogService
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="bus"></param>
        /// <param name="validationFactory"></param>
        public LookbackDateLogService(ILookbackDateLogRepository repository,
                                    IBusControl bus,
                                    IValidationFactory validationFactory)
            : base(bus, repository, validationFactory)
        {
           
        }

        /// <summary>
        /// DisposeChildServices
        /// </summary>
        protected override void DisposeChildServices()
        {
            //No child services currently
        }


        #region Impl methods

        /// <summary>
        /// Handle AddLookbackDateLogEntry
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommandResult Handle(AddLookbackDateLogEntry command)
        {
            Log.Trace("Started Handle for AddLookbackDateLogEntry");
            Log.Debug("Command Args for AddLookbackDateLogEntry: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<AddLookbackDateLogEntry>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<AddLookbackDateLogCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);
                
                var log = LookbackDateLog.Create(command.ChangedDate, command.MemberGuid, command.LookbackDate, command.NewValue, command.OldValue, command.UserName);
                
                return Add<AddLookbackDateLogCommandResult>(log, cmdValidation, command.UserName);
            }
            finally
            {
                Log.Trace("Returning from Handle for AddLookbackDateLogEntry");
            }
        }

        /// <summary>
        /// Validate AddLookbackDateLogEntry
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        AbimValidationResult ICommandValidationHandler<AddLookbackDateLogEntry>.Validate(AddLookbackDateLogEntry command)
        {
            return base.Validate<AddLookbackDateLogEntry>(command);
        }

        #endregion
    }
}
