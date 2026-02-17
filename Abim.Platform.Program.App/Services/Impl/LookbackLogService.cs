using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Relational.Services.Impl;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Hangfire;
using MassTransit;
using NLog;
using ServiceStack.Text;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// LookbackLogService
    /// </summary>
    public class LookbackLogService : 
        ServiceBase<LookbackLog, ILookbackLogRepository>, 
        ILookbackLogService
    {
        /// <summary>
        /// 
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }
        /// <summary>
        /// 
        /// </summary>
        protected ICredentialService CredentialService { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="repository"></param>
        /// <param name="credentialService"></param>
        /// <param name="bus"></param>
        /// <param name="jobClient"></param>
        /// <param name="validationFactory"></param>
        public LookbackLogService(ILookbackLogRepository repository,
                                    ICredentialService credentialService,
                                    IBusControl bus,
                                    IBackgroundJobClient jobClient,
                                    IValidationFactory validationFactory)
            : base(bus, repository, jobClient, validationFactory)
        {
            CredentialService = credentialService;
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void DisposeChildServices()
        {
            if (CredentialService != null)
                CredentialService.Dispose();
        }

        #region Impl methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommandResult Handle(AddCertificationLookbackLog command)
        {
            Log.Trace("Started Handle for AddCertificationLookbackLog");
            Log.Debug("Command Args for AddCertificationLookbackLog: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<AddCertificationLookbackLog>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<AddLookbackLogCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = CredentialService.Load(command.CredentialId);

                if (credential == null) return Warning<AddLookbackLogCommandResult>($"Default {typeof(Credential).Name} of id {command.CredentialId} not found", cmdValidation);

                var log = LookbackLog.Create(credential, command.Reason, command.Action, LookbackStatusType.CredentialStatus, command.UserName, command.IsPendingAction, command.LookbackLogDate);

                return Add<AddLookbackLogCommandResult>(log, cmdValidation, command.UserName);

            }
            finally
            {
                Log.Trace("Returning from Handle for AddCertificationLookbackLog");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        AbimValidationResult ICommandValidationHandler<AddCertificationLookbackLog>.Validate(AddCertificationLookbackLog command)
        {
            return base.Validate<AddCertificationLookbackLog>(command);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommandResult Handle(AddParticipationLookbackLog command)
        {
            Log.Trace("Started Handle for AddParticipationLookbackLog");
            Log.Debug("Command Args for AddParticipationLookbackLog: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<AddParticipationLookbackLog>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<AddLookbackLogCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = CredentialService.Load(command.CredentialId);

                if (credential == null) return Warning<AddLookbackLogCommandResult>($"Default {typeof(Credential).Name} of id {command.CredentialId} not found", cmdValidation);

                var log = LookbackLog.Create(credential, command.Reason, command.Action, LookbackStatusType.ParticipationStatus, command.UserName, command.IsPendingAction, command.LookbackLogDate);

                return Add<AddLookbackLogCommandResult>(log, cmdValidation, command.UserName);

            }
            finally
            {
                Log.Trace("Returning from Handle for AddParticipationLookbackLog");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        AbimValidationResult ICommandValidationHandler<AddParticipationLookbackLog>.Validate(AddParticipationLookbackLog command)
        {
            return base.Validate<AddParticipationLookbackLog>(command);
        }

        #endregion
    }
}
