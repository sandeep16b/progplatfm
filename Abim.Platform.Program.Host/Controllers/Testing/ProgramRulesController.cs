using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.HangFireJobs;
using Abim.Platform.Program.App.Services;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.Enums;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Abim.Platform.Program.WebApi.Api.Attributes;
using Abim.Platform.Program.WebApi.Attributes;
using Hangfire;
using MassTransit;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace Abim.Platform.Program.Host.Api.Controllers
{
    /// <summary>
    /// ProgramRulesController
    /// </summary>
    [RoutePrefix(ProgramResourceConstants.Routes.Prefix.ProgramRules)]
    public class ProgramRulesController : ControllerBase
    {
        #region Properties
        
        /// <summary>
        /// The ProgramRules service
        /// </summary>
        protected IProgramRulesService ProgramRulesService { get; set; }
        
        /// <summary>
        /// The Credential service
        /// </summary>
        protected ICredentialService CredentialService { get; set; }

        /// <summary>
        /// The Credential service
        /// </summary>
        protected IHelperService HelperService { get; set; }

        /// <summary>
        /// Bus
        /// </summary>
        protected IBusControl Bus { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramRulesController"/> class.
        /// </summary>
        /// <param name="programRulesService">The source service.</param>
        /// <param name="credentialService">The credential service.</param>
        /// <param name="helperService">The helper Service.</param>
        /// <param name="bus">The bus.</param>
        public ProgramRulesController(  IProgramRulesService programRulesService, 
                                        ICredentialService credentialService,
                                        IHelperService helperService,
                                        IBusControl bus
            )
        {
            ProgramRulesService = programRulesService;
            CredentialService = credentialService;
            HelperService = helperService;
            Bus = bus;
        }

        #endregion

        #region Disposal

        /// <summary>
        /// Disposes the services.
        /// </summary>
        protected void DisposeServices(HttpRequestMessage message)
        {
            message.RegisterForDispose(CredentialService);
        }

        #endregion

        #region Endpoints 

        /// <summary>
        /// Times the limited credentials.
        /// </summary>
        /// <param name="recordsToProcess">The records to process.</param>
        /// <returns></returns>
        [HttpPost]
        #if !DEBUG
            [HideFromSwagger]
            [Authorize]
        #endif
        [Route("PostTimeLimitedCredentials", Name = "Schedule TimeLimitedCredentials")]
        public IHttpActionResult TimeLimitedCredentials(int? recordsToProcess)
        {
            string recurringJobId = string.Format("Scheduled@{0}_{1} records", DateTime.Now.ToShortDateString(), recordsToProcess.Value);
            RecurringJob.AddOrUpdate<ExpireByTimeLimitJob>(recurringJobId, x => x.Execute(null, null, recordsToProcess), "0 0 29 2/12000 WED");
            return Ok();
        }

        /// <summary>
        /// Posts the time limited credential check consumer.
        /// </summary>
        /// <param name="MemberId">The member identifier.</param>
        /// <param name="EventDate">The event date.</param>
        /// <param name="ProcessingDate">The processing date.</param>
        /// <returns></returns>
        [HttpPost]
        #if !DEBUG
                    [HideFromSwagger]
                    [Authorize]
        #endif
        [Route("PostTimeLimitedCredentialCheckConsumer", Name = "Trigger TimeLimitedCredentials Check Consumer")]
        public IHttpActionResult PostTimeLimitedCredentialCheckConsumer(Guid MemberId, DateTime EventDate, DateTime? ProcessingDate)
        {
            var ConsumerUsername = "MBMforTL";

            var memberId = MemberId;

            DateTime startDate ;

            //check if any value is pass to use, otherwise used current date
            DateTime processingDate = ProcessingDate.HasValue ? ProcessingDate.Value : DateTime.Now;

            // compare to bulk processing date. If it is after then we need to set it to the end of that window
            // It is requirement in pbi 73194
            if (DateTime.Compare(processingDate.Date, new DateTime(processingDate.Year, 9, 1).Date) > 0)
                startDate = new DateTime(processingDate.Year, 12, 31);
            else
                startDate = processingDate;

            // find all certificate that a person holds that are currently time limited (Duration = 'Timelimited') with active status (IssuanceStatus = 'Active' )
            // and Expiration is less of equal to startDate ( CredentialId, IssuanceId)
            IEnumerable<Tuple<Guid, int>> expiringCredentials = CredentialService.GetExpiredCredentials(startDate, memberId);

            foreach (var credential in expiringCredentials)
            {
                var command = new RunRulesForMustBeMaintainedCertificateCommand
                {
                    CredentialId = credential.Item1,
                    IssuanceId = credential.Item2,
                    EventDate = EventDate,
                    CreatedBy = ConsumerUsername, // per PBI
                    ProcessingDate=DateTime.Now
                };

                ProgramRulesService.HandleJob(command);
                //BackgroundJob.Enqueue<ExpireTLChildJob>(x => x.ExecuteChild(command.CredentialId, command, null, null));
            }

            return Ok();
        }

        #endregion

        #region Corrective Action

        /// <summary>
        /// Runs the corrective action for single credential
        /// </summary>
        /// <param name="credentialId">The credential identifier.</param>
        /// <param name="eventDate">The event date.</param>
        /// <param name="processingDate">The processing date.</param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
        [HideFromSwagger]
        [Authorize]
#endif
        [Route("RunCorrectiveActionForCredential", Name = "RunCorrectiveActionForCredential")]
        public async Task<IHttpActionResult> RunCorrectiveAction(Guid credentialId,
                                                            DateTime? eventDate=null,
                                                            DateTime? processingDate=null)
        {
            try
            {
                eventDate = eventDate.HasValue ? eventDate.Value : new DateTime(DateTime.Now.Year - 1, 12, 31);
                processingDate = processingDate.HasValue ? processingDate.Value : DateTime.Now;

                var credential = CredentialService.Load(credentialId);

                if (credential==null)
                    return Content(HttpStatusCode.OK, string.Format("No Credential exists for the following credential: '{0}'", credentialId));

                eventDate = eventDate.HasValue ? eventDate.Value : DateTime.Now;
                processingDate = processingDate.HasValue ? processingDate.Value : DateTime.Now;

                // just pass one credential to exclude other credentials from testing.
                var list = new List<Credential>(){ credential };

                await ProgramRulesService.RunCorrectiveAction(list, 
                                credential.MemberId,
                                eventDate.Value,
                                processingDate.Value);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing corrective action: '{0}'", ex.Message));
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Runs the corrective action for member
        /// </summary>
        /// <param name="memberId">The credential identifier.</param>
        /// <param name="eventDate">The event date.</param>
        /// <param name="processingDate">The processing date.</param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
            [HideFromSwagger]
            [Authorize]
#endif
        [Route("RunCorrectiveActionForMember", Name = "RunCorrectiveActionForMember")]
        public async Task<IHttpActionResult> RunCorrectiveActionForMember(Guid memberId,
                                                            DateTime? eventDate=null,
                                                            DateTime? processingDate=null)
        {
            try
            {

                //eventDate = eventDate.HasValue ? eventDate.Value : DateTime.Now;
                eventDate = eventDate.HasValue ? eventDate.Value : new DateTime( DateTime.Now.Year-1,12,31);
                processingDate = processingDate.HasValue ? processingDate.Value : DateTime.Now;

                await ProgramRulesService.RunCorrectiveActionForMember(memberId, eventDate.Value, processingDate.Value);

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing corrective action: '{0}'", ex.Message));
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        /// <summary>
        /// Run Triggered Communication for EarnedMBMCertLetter
        /// </summary>
        /// <param name="credentialId">The credential identifier.</param>
        /// <param name="communicationType">The communicationType</param>
        /// <returns></returns>
        [HttpGet]
        [HideFromSwagger]
        [Authorize]
        [Route("TriggeredCommunicationEarnedMBMCertLetter", Name = "TriggeredCommunicationEarnedMBMCertLetter")]
        public async Task<IHttpActionResult> TriggeredCommunicationEarnedMBMCertLetter(Guid credentialId,
                                                                    string communicationType)
        {
            try
            {
                
                var credential = CredentialService.Load(credentialId);

                if (credential == null)
                    return Content(HttpStatusCode.OK, string.Format("No Credential exists for the following credential: '{0}'", credentialId));

                //communicationType
                if (communicationType.ToLower()== App.Util.Constants.TriggeredCommunication.EarnedMBMCertLetter.ToLower())
                    await HelperService.TriggeredCommunication(credential, App.Util.Constants.TriggeredCommunication.EarnedMBMCertLetter, ProfileId, CredentialService);

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing TriggeredCommunication action: '{0}'", ex.Message));
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion

        #region Year End Look Back 
        
        /// <summary>
        /// Runs the Year End Look Back  for member id
        /// </summary>
        /// <param name="memberId">The credential identifier.</param>
        /// <param name="lookBackDate">The event date.</param>
        /// <param name="processingDate">The processing date.</param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
        [HideFromSwagger]
        [Authorize]
#endif
        [Route("RunYearEndLookback", Name = "RunYearEndLookback")]
        public async Task<IHttpActionResult> RunYearEndLookback(Guid memberId,
                                                            DateTime? lookBackDate = null,
                                                            DateTime? processingDate=null)
        {
            try
            {

                lookBackDate = lookBackDate?? new DateTime(2018, 12, 31);
                processingDate = processingDate ?? DateTime.Now;

                await ProgramRulesService.RunYearEndLookback(memberId,
                                lookBackDate.Value,
                                processingDate.Value);

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing corrective action: '{0}'", ex.Message));
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion

        #region OnExamResultEvent

        /// <summary>
        /// OnExamResultEvent
        /// </summary>
        /// <param name="registrationGuid"></param>
        /// <param name="examRegistration"></param>
        /// <param name="processingDate"></param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
        [HideFromSwagger]
        [Authorize]
#endif
        [Route("OnExamResultEvent", Name = "OnExamResultEvent")]
        public async Task<IHttpActionResult> OnExamResultEvent(
            Guid registrationGuid,
            ExamRegistrationType examRegistration, 
            DateTime? processingDate = null)
        {
            try
            {

                processingDate = processingDate ?? DateTime.Now;

                await ProgramRulesService.RunProcessesOnExamResultEvent(registrationGuid,
                                processingDate.Value, examRegistration);
                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, $"An error has occurred while testing '{ex.Message}'");
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion

        #region Year End Look Back 

        /// <summary>
        /// Runs the Year End Look Back  for member id
        /// </summary>
        /// <param name="memberId">The credential identifier.</param>
        /// <param name="lookBackDate">The event date.</param>
        /// <param name="processingDate">The processing date.</param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
        [HideFromSwagger]
        [RequiresHttps, Authorize]
#endif
        [Route("RunEarlyYearEndLookbackChildJob", Name = "RunEarlyYearEndLookbackChildJob")]
        public async Task<IHttpActionResult> RunEarlyYearEndLookbackChildJob(Guid memberId,
                                                            DateTime? lookBackDate = null,
                                                            DateTime? processingDate = null)
        {
            try
            {
                processingDate = processingDate ?? DateTime.Now;

                await ProgramRulesService.HandleEarlyYearEndLookbackChildJob(memberId,
                                null,
                                processingDate.Value);

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing corrective action: '{0}'", ex.Message));
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion


        #region Lock Out CoSponsored period

        /// <summary>
        /// Runs the CoSponsored Lock Out Child Job for Credential Id
        /// </summary>
        /// <param name="credentialId">The credential identifier.</param>
        /// <param name="lockOutDate">The event date.</param>
        /// <param name="processingDate">The processing date.</param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
        [HideFromSwagger]
        [RequiresHttps, Authorize]
#endif
        [Route("RunCoSponsoredLockOutChildJob", Name = "CoSponsoredLockOutChildJob")]
        public async Task<IHttpActionResult> CoSponsoredLockOutChildJob(Guid credentialId,
                                                            DateTime? lockOutDate = null,
                                                            DateTime? processingDate = null)
        {
            try
            {

                lockOutDate = lockOutDate ?? new DateTime(DateTime.Now.Year-1, 12, 31);
                processingDate = processingDate ?? DateTime.Now;

                await ProgramRulesService.RunCoSponsoredLockOut(credentialId,
                                lockOutDate.Value,
                                processingDate.Value);

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing lock out period : '{0}'", ex.Message));
            }
            finally
            {
                DisposeServices(Request);
            }
        }

        #endregion


        /* for local use only 
        #region Update LNG Assessment Due Date
        /// <summary>
        /// UpdateAssessmentDueDate
        /// </summary>
        /// <param name="credentialGuid"></param>
        /// <param name="year"></param>
        /// <param name="metParticipationStatus"></param>
        /// <param name="passSummativeDecision"></param>
        /// <returns></returns>
        [HttpGet]
#if !DEBUG
        [HideFromSwagger]
        [RequiresHttps, Authorize]
#endif
        [Route("UpdateAssessmentDueDate", Name = "UpdateAssessmentDueDate")]
        public async Task<IHttpActionResult> UpdateAssessmentDueDate(Guid credentialGuid,
                                                            int year,
                                                            bool? metParticipationStatus,
                                                            bool? passSummativeDecision)
        {
            try
            {

                await CredentialService.Handle(new UpdateLngAssessmentDueDateCommand()
                {
                    CredentialId = credentialGuid,
                    Year = year,
                    MetParticipationStatus = metParticipationStatus,
                    PassSummativeDecision = passSummativeDecision,
                    UserName = "LngParticipationResultConsumer"
                });

                return Ok();
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                return Content(HttpStatusCode.InternalServerError, string.Format("An error has occurred while testing UpdateLngAssessmentDueDate: '{0}'", ex.Message));
            }
        }

        #endregion
        */
    }
}
