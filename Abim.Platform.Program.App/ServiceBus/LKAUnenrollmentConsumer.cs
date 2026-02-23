using Abim.Enterprise.Core.ServiceBus.Registration;
using Abim.Platform.Program.App.Services;
using MassTransit;
using NLog;
using ServiceStack.Text;
using System;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.ServiceBus
{
    /// <summary>
    /// A consumer for IMemberUnEnrolledEvent objects
    /// </summary>
    public class LKAUnenrollmentConsumer : IConsumer<IMemberUnEnrolledEvent>
    {
        #region Properties

        /// <summary>
        /// The logger
        /// </summary>
        protected static readonly ILogger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Gets or sets the credential service.
        /// </summary>
        /// <value>
        /// The credential service.
        /// </value>
        protected ICredentialService CredentialService { get; set; }

        #endregion Properties

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="credentialService">An instances of ICredentialService to use</param>
        public LKAUnenrollmentConsumer(ICredentialService credentialService)
        {
            CredentialService = credentialService ?? throw new ArgumentNullException(nameof(credentialService));
        }

        /// <summary>
        /// The method for consuming IMemberUnEnrolledEvent objects
        /// </summary>
        /// <param name="context">A MassTransit ConsumeContext</param>
        /// <returns></returns>
        public async Task Consume(ConsumeContext<IMemberUnEnrolledEvent> context)
        {
            try
            {
                Log.Info($"LKAUnenrollmentConsumer.Consume() called with the following parameters: {context.Message.Dump()}");
                var @event = context.Message;

                if (@event.CredentialId == Guid.Empty)
                {
                    Log.Error("No CredentialId is provided.");
                    return;
                }

                //*** below code was commented per Bug 218848 : (Release 2.35) Don't Change Pathway When Unenrolling from LKA
                //** we keep this consumer "The consumer should remain in case we need it in the future, but we should prevent it from changing the pathway."
                //var command = GetUpdatePathwayCommand(@event.CredentialId);
                // await CredentialService.Handle(command);

                //BTW we have an UpdatePathway() method in the Credential Controller

                // just notification if we forget about above the code changes, but checking the log would reveal it
                Log.Info($"LKAUnenrollmentConsumer.Consume() was consumed but DID NOT change anything. Previously updated pathway to MOC for credential ");
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        //private UpdatePathwayCommand GetUpdatePathwayCommand(Guid credentialId)
        //{
        //    var command = new UpdatePathwayCommand();
        //    command.CredentialId = credentialId;
        //    command.Pathway = Resources.PathwayType.MOC;
        //    command.UserInfo = new WebApi.Authentication.UserInfo { Username = "LKAUnenrollmentConsumer" };
        //    return command;
        //}
    }
}
