using Abim.Enterprise.Core.Registration.Enums;
using Abim.Enterprise.Core.ServiceBus.Program;
using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.DTOs;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using ServiceBus.Events;
using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.Relational.Services.Impl;
using Abim.Platform.Program.Relational.Validation;
using Abim.Platform.Program.Relational.Validation.Impl;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi;
using Hangfire;
using MassTransit;
using NLog;
using ServiceStack.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Abim.Platform.Program.App.Util.Constants;

namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// class CredentialService
    /// </summary>
    public class CredentialService : ServiceBase<Credential, ICredentialRepository>, ICredentialService
    {
        #region Fields

        /// <summary>
        /// The programRulesService service backing field
        /// </summary>
        private IProgramRulesService _programRulesService;

        #endregion

        #region Properties

        /// <summary>
        /// The Certification service
        /// </summary>
        protected ICertificationService CertificationService { get; set; }

        /// <summary>
        /// The Source service
        /// </summary>
        protected ISourceService SourceService { get; set; }

        /// <summary>
        /// The Helper service
        /// </summary>
        protected IHelperService HelperService { get; set; }

        /// <summary>
        /// Gets or sets the ProgramRules service.
        /// </summary>
        /// <value>
        /// The ProgramRules service.
        /// </value>
        private IProgramRulesService ProgramRulesService
        {

            get
            {
                if (_programRulesService == null)
                {
                    _programRulesService = DependencyResolver.Container.GetInstance<IProgramRulesService>();
                }
                return _programRulesService;
            }
            set
            {
                _programRulesService = value;
            }
        }

        /// <summary>
        /// The logger is inherited from the base class. We can expose this property internally for testing
        /// </summary>
        protected internal ILogger Log { get { return Logger; } set { Logger = value; } }


        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="CredentialService"/> class.
        /// </summary>
        /// <param name="credentialRepository"></param>
        /// <param name="certificationService"></param>
        /// <param name="sourceService"></param>
        /// <param name="helperService"></param>
        /// <param name="bus"></param>
        /// <param name="jobClient"></param>
        /// <param name="validationFactory"></param>
        public CredentialService(ICredentialRepository credentialRepository,
                                 ICertificationService certificationService,
                                 ISourceService sourceService,
                                 IHelperService helperService,
                                 IBusControl bus,
                                 IBackgroundJobClient jobClient,
                                 IValidationFactory validationFactory)
            : base(bus, credentialRepository, jobClient, validationFactory)
        {
            CertificationService = certificationService;
            SourceService = sourceService;
            HelperService = helperService;
        }

        #endregion

        #region Custom Get Methods

        /// <summary>
        /// Searches Credentials by member Id
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <param name="paging">The paging</param>
        /// <param name="totalCount">Thetotal Count</param>
        /// <returns></returns>
        public IEnumerable<Credential> SearchByMemberId(Guid memberId, PageDefinition paging, out int totalCount)
        {
            return Repository.SearchByMemberId(memberId, paging, out totalCount);
        }

        /// <summary>
        /// Searches Credentials by member Id
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <returns></returns>
        public IEnumerable<Credential> SearchByMemberId(Guid memberId)
        {
            return Repository.SearchByMemberId(memberId);
        }

        /// <summary>
        /// Searches Credentials by member Id
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<Credential>> SearchByMemberIdAsync(Guid memberId)
        {
            return await Repository.SearchByMemberIdAsync(memberId).ConfigureAwait(false);
        }

        /// <summary>
        /// Searches Certifications by member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="paging"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        public IEnumerable<Certification> SearchCertsByMemberId(Guid memberId, PageDefinition paging, out int totalCount)
        {
            return Repository.SearchCertsByMemberId(memberId, paging, out totalCount);
        }

        /// <summary>
        /// Searches Certifications by member Id
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <returns></returns>
        public IEnumerable<Certification> SearchCertsByMemberId(Guid memberId)
        {
            return Repository.SearchCertsByMemberId(memberId);
        }

        /// <summary>
        /// Get IM Credential By Member
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <returns></returns>
        public Credential GetIMCredentialByMember(Guid memberId)
        {
            return Repository.GetIMCredentialByMember(memberId);
        }

        /// <summary>
        /// Get Credential By Member And Product Code
        /// </summary>
        /// <param name="memberId">The member Id</param>
        /// <param name="Code">The Code</param>
        /// <returns></returns>
        public Credential GetCredentialByMemberAndCode(Guid memberId, string Code)
        {
            return Repository.GetCredentialByMemberAndCode(memberId, Code);
        }

        /// <summary>
        /// Loads Issuances for a Credential
        /// </summary>
        /// <param name="id">The id of the credential</param>
        /// <returns></returns>
        public IEnumerable<Issuance> LoadIssuancesForCredential(Guid id)
        {
            var cred = Repository.Load(id);
            if (cred == null)
                throw new Exception(string.Format("{0} {1} not found", typeof(Credential).Name, id));
            return cred.Issuances;
        }

        /// <summary>
        /// Gets the Guids of all the expired credentials for a all users as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        public IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime checkDate)
        {
            return Repository.GetExpiredCredentials(checkDate);
        }

        /// <summary>
        /// Gets the Guids of all the expired credentials for a all users as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="abimCredentialsOnly">Optional parameter to specify ABIM-issued credentials only (default is false).</param>
        /// <returns></returns>
        public IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime startDate, DateTime endDate, bool abimCredentialsOnly = false)
        {
            return Repository.GetExpiredCredentials(startDate, endDate, abimCredentialsOnly);
        }

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <param name="memberId">The check date.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        public IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime checkDate, Guid memberId)
        {
            return Repository.GetExpiredCredentials(checkDate, memberId);
        }

        /// <summary>
        /// Gets the issuances for member identifier.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public IEnumerable<Issuance> GetIssuancesForMemberId(Guid memberId)
        {
            return Repository.GetIssuancesForMemberId(memberId);
        }

        /// <summary>
        /// Firsts the issuance date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public DateTime? GetFirstIssuanceDate(Guid memberId)
        {
            return Repository.GetFirstIssuanceDate(memberId);
        }

        /// <summary>
        /// Gets the latest lookback date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        public DateTime? GetLatestLookbackDate(Guid memberId)
        {
            return Repository.GetLatestLookbackDate(memberId);
        }

        /// <summary>
        /// Get Non Abim Issuance Count
        /// </summary>
        /// <returns></returns>
        public int GetNonAbimIssuanceCount()
        {
            return Repository.GetNonAbimIssuanceCount();
        }

        /// <summary>
        /// GetCurrentIssuance
        /// </summary>
        /// <param name="credential"></param> 
        /// <returns></returns>
        public Issuance GetCurrentIssuance(Credential credential)
        {
            return credential.Issuances.OrderByDescending(i => i.IssuanceDate).First();
        }

        /// <summary>
        /// Gets the IDs of Credentials marked for deselection
        /// </summary>
        /// <param name="deselectionEffectiveDate">The DeSelectionEffectiveDate to query by</param>
        /// <returns>An IEnumerable of Credential IDs</returns>
        public IEnumerable<IGrouping<Guid, CredentialInfoDTO>> GetInfoOfCredentialsMarkedForDeselection(DateTime deselectionEffectiveDate)
        {
            var credentialInfo = Repository.GetInfoOfCredentialsMarkedForDeselection(deselectionEffectiveDate);

            var credentialInfoGroupedByMemberId =
                credentialInfo.GroupBy(
                    key => key.Item1,
                    values => new CredentialInfoDTO
                    {
                        ExternalId = values.Item2,
                        CertificateName = values.Item3,
                        IsActive = values.Item4
                    });

            return credentialInfoGroupedByMemberId;
        }

        #endregion

        #region LNG methods

        #region UpdateLngAssessmentDueDateCommand

        /// <summary>
        /// Handles an UpdateLngAssessmentDueDateCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<UpdateLngAssessmentDueDateCommandResult> Handle(UpdateLngAssessmentDueDateCommand command)
        {
            Log.Info($"Start Handle UpdateLngAssessmentDueDateCommand for Credential:'{command.CredentialId}' with Cmd Args:'{command.Dump()}'");
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateLngAssessmentDueDateCommand>)this).Validate(command);
                var credentialWasUpdated = false;

                if (!cmdValidation.Succeeded) return Warning<UpdateLngAssessmentDueDateCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                // fetch the credential
                var credential = Load(command.CredentialId);

                if (credential == null) return Warning<UpdateLngAssessmentDueDateCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

                /* PBI 181022 : Program Rule 63 - Longitudinal Assessment Due Date 
                 -----------------------------------------------------------------------------------------------------------------------------------------------------------------
                 	The Longitudinal Assessment due date is 12/31/<longitudinal particiaption status year> + 1 for a non-cosponsored certificate IF:  
                            The certificate in not lapsed AND the certificate is meeting the annual LNG participation requirement AND it is not a summative decision year OR
                            The certificate in not lapsed AND the certificate is meeting the annual LNG participation requirement AND the result of the summative assessment is Pass in a summative decision year.
                            The Longitudinal Assessment due date is 12/31/<summative decision year> + 1 when the diplomate is using Longitudinal as their assessment IF
                            The certificate is lapsed due to not meeting the assessment requirement AND the certificate is meeting the annual LNG participation requirement AND the result of the summative assessment in the year of the summative decision is Pass.
                            Note: Longitudinal enrollment year is the LKA equivalent of the long form administration year.
                 ------------------------------------------------------------------------------------------------------------------------------------------------------------------
                 	The Longitudinal Assessment due date is 12/31/<longitudinal particiaption status year> + 1 for a cosponsored certificate IF: 
                        The assesment due year is equal to the LKA enrollment year AND the certificate is meeting the annual LNG participation requirement AND it is not a summative decision year OR
                        The assesment due year is equal to the LKA enrollment year AND the certificate is meeting the annual LNG participation requirement AND the result of the summative assessment is Pass in a summative decision year.
                        The Longitudinal Assessment due date is 12/31/<summative decision year> + 1 for a cosponsored certificate IF
                        The *assessment due year is less than the enrollment year* AND the result of the summative assessment in the year of the summative decision is Pass.
                            Note: Longitudinal enrollment year is the LKA equivalent of the long form administration year.
                            *Since ABIM does not determine or identify certificate status for cosponsored certificates, the 'assessment due year is less than the enrollment year' is the convention that the rules have used to indicate the  cosponsored equivalent of the ABIM lapsed cert.
               */

                if (command.MetParticipationStatus.HasValue && command.MetParticipationStatus.Value &&
                   // Rule 63.1 don't have to be on LKA path, can be any. For example, they register for MOC exam before we get participation results for the year.
                   !command.IsSummativeDecisionYear &&
                   ((credential.IsCosponsored) || // updated rule 63.2 , don't have to be in due year for co-sponsored diplomates
                   (!credential.IsCosponsored && credential.IsNotLapsedDueToAssessment))) // NoneCosponsored:  is not lapsed due to Assessment requirment : PBI 287070 : (3.1) Update Program Rule 63 – LKA Assessment Due Date
                {

                    credential.ExamDueDate = new DateTime(command.Year + 1, 12, 31);
                    //we would need to delete below obsolete values but we can do it later
                    credential.KCIExamDueDate = credential.MOCExamDueDate = credential.DisplayExamDueDate = credential.ExamDueDate;

                    // Rule 64: Longitudinal Assessment Requirement Met : The certificate is not lapsed and meets their annual longitudinal participation requirement and is not in summative decision year  OR
                    if (!credential.AssessmentMet)
                    {
                        credential.AssessmentMet = true;
                        credential.AssessmentMetDate = new DateTime(command.Year, 12, 31); // bug 246471 : New Issuance not being created after KickOffLkaParticipationMet Job
                    }

                    credential.SetModified($"{command.UserName}:MetParticipationIn{command.Year}");

                    credentialWasUpdated = true;

                    Log.Info($"Credential='{credential.Id}' meets  participation requirements to increment ExamDueDate to {credential.ExamDueDate.Value.Year} in Handle UpdateLngAssessmentDueDateCommand");

                }
                // if pass summative decision then incremenent by one for both CoSponsored and NoneCosponsored regardless of cred status (Lapsed or not)
                else if (command.PassSummativeDecision.HasValue && command.PassSummativeDecision.Value &&
                         // Rule 63.1 don't have to be on LKA path, can be any. For example, they register for MOC exam before we get Summative Decision results for the year.
                         command.IsSummativeDecisionYear)
                {
                    credential.ExamDueDate = new DateTime(command.Year + 1, 12, 31);
                    //we would need to delete below obsolete values but we can do it later
                    credential.KCIExamDueDate = credential.MOCExamDueDate = credential.DisplayExamDueDate = credential.ExamDueDate;

                    if (!credential.AssessmentMet)
                    {
                        credential.AssessmentMet = true;
                        credential.AssessmentMetDate = new DateTime(command.Year, 12, 31);
                    }

                    // Bug 231779 : Rule 65 - Expire TL not getting new MBM after Summative Decision
                    // We should NOT change issuance status and issuance date HERE
                    // It should be done by Corrective Action ONLY (if meet requirements), which should be processed after this operation

                    credential.SetModified($"{command.UserName}:PassSummativeIn{command.Year}");

                    credentialWasUpdated = true;

                    Log.Info($"Credential='{credential.Id}' pass summative decision and ExamDueDate was incremented to {credential.ExamDueDate.Value.Year} in Handle UpdateLngAssessmentDueDateCommand");
                }
                else
                {
                    Log.Info($"Credential='{credential.Id}' didn't meet participation or summative decision requirement or was updated before, no change to credential in in Handle UpdateLngAssessmentDueDateCommand"); // nothing here 

                }


                if (credentialWasUpdated) // if anything get updated then save it
                {

                    //save to the database
                    var errorResult = TryUpdate<UpdateLngAssessmentDueDateCommandResult>(credential, cmdValidation, command.UserName);

                    if (errorResult != null) return errorResult;

                    //Run Corrective Action for the diplomate which would evaluate current participation/certification status for all active certificates.
                    await ProgramRulesService.RunCorrectiveActionForMember(
                                                    memberId: credential.MemberId,
                                                    eventDate: DateTime.Now,
                                                    processingDate: DateTime.Now,
                                                    triggeringEvent: TriggeringEvent.Others);
                }

                return cmdValidation.ToCommandResult<UpdateLngAssessmentDueDateCommandResult, Credential>(credential);

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
            finally
            {
                Log.Trace($"Returning from Handle UpdateLngAssessmentDueDateCommand for Credential:'{command.CredentialId}'");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<UpdateLngAssessmentDueDateCommand>.Validate(UpdateLngAssessmentDueDateCommand command)
        {
            return base.Validate<UpdateLngAssessmentDueDateCommand>(command);
        }

        #endregion

        #endregion

        #region Command Handlers

        #region CreateCredentialCommand

        /// <summary>
        /// Handles an CreateCredentialCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public ICommandResult Handle(CreateCredentialCommand command)
        {
            Log.Trace("Started Handle for CreateCredentialCommand");
            Log.Debug("Command Args for CreateCredentialCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<CreateCredentialCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<CreateCredentialCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                //fetch the certification
                var certification = CertificationService.Load(command.CertificationId);

                //create the model
                var credential = Credential.Create(
                    certification,
                    command.MemberId,
                    command.Type,
                    command.Pathway,
                    null, //PBI 215404 -- These values will be null for non-cosponsored, and for cosponsored the stubbed FPHM credential would get created via data import
                    null,
                    command.UserInfo.Username);

                //apply field settings
                credential.ApplyChangesAfterCreatingCredential(
                    command.IsActive,
                    command.GracePeriodStartDate,
                    command.GracePeriodEndDate,
                    command.ExamDueDate,
                    command.MOCExamDueDate,
                    command.KCIExamDueDate,
                    command.DisplayExamDueDate,
                    command.ConsecutiveKCIPassRequired,
                    command.ReAttestationDueDate);

                //add to the database
                return Add<CreateCredentialCommandResult>(credential, cmdValidation, command.UserInfo.Username);

            }
            finally
            {
                Log.Trace("Returning from Handle for CreateCredentialCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<CreateCredentialCommand>.Validate(CreateCredentialCommand command)
        {
            return base.Validate<CreateCredentialCommand>(command);
        }

        #endregion

        #region CreateCredentialIssuanceCommand

        /// <summary>
        /// Handles an CreateCredentialIssuanceCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public ICommandResult Handle(CreateCredentialIssuanceCommand command)
        {
            Log.Trace("Started Handle for CreateCredentialIssuanceCommand");
            Log.Debug("Command Args for CreateCredentialIssuanceCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<CreateCredentialIssuanceCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<CreateCredentialIssuanceCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                // Fetch the certification
                var certification = CertificationService.Load(command.CertificationId);

                // Create the model
                var credential =
                    Credential.Create(
                        certification,
                        command.MemberId,
                        command.Type,
                        command.Pathway,
                        command.OnBehalfBoardCode,
                        command.OnBehalfBoardName,
                        command.CredentialCreatedBy);

                // Get Abim Source
                var source = SourceService.GetAbimSource();

                credential.ApplyChangesForCreateCredential(
                        command.AssessmentMetDate,
                        command.ExamDueDate,
                        command.ReAttestationDueDate,
                        source,
                        command.MaintenanceStatus,
                        command.ScheduledUpdate,
                        command.DisplayExamDueDate,
                        command.KCIExamDueDate,
                        command.MOCExamDueDate,
                        command.ConsecutiveKCIPassRequired,
                        command.IssuanceCreatedBy);

                //add to the database
                var errorResult = TryAdd<CreateCredentialIssuanceCommandResult>(credential, cmdValidation, command.CredentialCreatedBy);


                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential));

                //return
                return cmdValidation.ToCommandResult<CreateCredentialIssuanceCommandResult, Credential>(credential);

            }
            finally
            {
                Log.Trace("Returning from Handle for CreateCredentialCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<CreateCredentialIssuanceCommand>.Validate(CreateCredentialIssuanceCommand command)
        {
            return base.Validate<CreateCredentialIssuanceCommand>(command);
        }

        #endregion

        #region DeselectCertificateCommand
        /// <summary>
        /// Handles a DeselectCertificateCommand
        /// </summary>
        /// <param name="command">The command to handle</param>
        /// <returns>An ICommandResult indiciating success or failure</returns>
        public ICommandResult Handle(DeselectCertificateCommand command)
        {
            //For PBI 187664
            Log.Trace("Started Handle for DeselectCertificateCommand");
            Log.Debug("Command Args for DeselectCertificateCommand: {0}", command.Dump());
            AbimValidationResult validationResult = null;
            bool publishCMPUnEnrolledEvent = false;

            try
            {
                validationResult = ((ICommandValidationHandler<DeselectCertificateCommand>)this).Validate(command);

                if (!validationResult.Succeeded)
                    return Warning<DeselectCertificateCommandResult>($"Validation Failed: '{validationResult.Results.Dump()}'", validationResult);

                var credential = Load(command.CredentialId);
                if (credential == null)
                    throw new ApplicationException("Credential not found.");

                if (!credential.HasIssuances)
                    throw new ApplicationException($"Credential {command.CredentialId} is not eligible for deselection because it has no issuances.");

                //If we were told to deselect this credential, but the latest issuance is not flagged for deselection, throw an exception
                if (!credential.DeselectionElected)
                    throw new ApplicationException($"Credential {command.CredentialId} has a latest issuance not marked for deselection.");

                //If already deselected, throw Warning
                if (credential.DeselectionProcessed)
                    return Warning<DeselectCertificateCommandResult>($"Credential {command.CredentialId} has already been processed for deselection.", validationResult);

                /*
                If Credential is IM or FPHM, we'll know which one was the 
                "selected" of the two because its "SelectedToMaintain" value 
                will be set to true.
                */

                //Update credential
                credential.Deselect(command.Username, command.ExpiredDate);

                // Pbi 322411 : Unenroll from CMP during the deactivation process
                if (credential.IsInCMP)
                {
                    credential.SetUnEnrollInCMP(modifiedBy: command.Username + "_KickOutFromCmp",
                                                priviousPathway: Resources.PathwayType.MOC, // always MOC since no more KCI
                                                UnEnrollmentDate: DateTime.Now);

                    publishCMPUnEnrolledEvent = true;
                }

                //Save to the database
                var repoResult = TryUpdate<DeselectCertificateCommandResult>(credential, validationResult, command.Username);
                
                if (repoResult != null) return repoResult;

                // successfully updated the credential and if we unelled then we need to publish the event
                if (publishCMPUnEnrolledEvent)
                {
                    Task.Run(async () =>
                    {
                        // It would voluntary (voluntary = true) since diplomate is making the choice to no longer maintain certification.  
                        await Task.Run(() => PublishCMPUnEnrolledEvent(credential: credential,
                                                                        UnEnrollmentDate: DateTime.Now,
                                                                        Voluntary: true));
                    });
                }

                return validationResult.ToCommandResult<DeselectCertificateCommandResult, Credential>(credential);
            }
            catch (Exception ex)
            {
                return Error<DeselectCertificateCommandResult>($"Failed to update latest issuance for Credential {command.CredentialId} for deselection: '{ex}'", validationResult);
            }
            finally
            {
                Log.Trace("Returning from Handle for DeselectCertificateCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<DeselectCertificateCommand>.Validate(DeselectCertificateCommand command)
        {
            return base.Validate<DeselectCertificateCommand>(command);
        }

        #endregion DeselectCertificateCommand

        #region MarkCertificateForDeselectCommand
        /// <summary>
        /// Handles a MarkCertificateForDeselectCommand
        /// </summary>
        /// <param name="command">The command to handle</param>
        /// <returns>An ICommandResult indiciating success or failure</returns>
        public ICommandResult Handle(MarkCertificateForDeselectCommand command)
        {
            //For PBI 187664
            Log.Trace("Started Handle for MarkCertificateForDeselectCommand");
            Log.Debug("Command Args for MarkCertificateForDeselectCommand: {0}", command.Dump());
            AbimValidationResult validationResult = null;

            try
            {
                validationResult = ((ICommandValidationHandler<MarkCertificateForDeselectCommand>)this).Validate(command);

                if (!validationResult.Succeeded)
                    return Warning<MarkCertificateForDeselectCommandResult>($"Validation Failed: '{validationResult.Results.Dump()}'", validationResult);

                //Load the credential and get the latest issuance
                var credential = Load(command.CredentialId);

                if (!credential.HasIssuances)
                    throw new ApplicationException($"Credential {command.CredentialId} is not eligible for to be marked for deselection because it has no issuances.");

                //If already marked for deselection, throw Warning
                if (credential.DeselectionElected)
                {
                    if (credential.DeselectionProcessed)
                        return Warning<MarkCertificateForDeselectCommandResult>($"Credential {credential.ExternalId} was already marked for deselection, and processed on {credential.DeselectionProcessedDate}.", validationResult);
                    else
                        return Warning<MarkCertificateForDeselectCommandResult>($"Credential {credential.ExternalId} has already marked for deselection.", validationResult);
                }

                var newestIssuance = credential.NewestIssuance;

                if (newestIssuance == null)
                    throw new ApplicationException($"No issuances found for credential {command.CredentialId}.");

                DateTime effectiveDate = GetEffectiveDateForCertDeSelect(command.SubmittedDate);

                //Mark the issuance for deselection. This doesn't expire it -- that will happen later
                //when the Hangfire job processes the credentials which have issuances flagged
                //for deselection.
                string username = command?.UserInfo?.Username;
                newestIssuance.MarkForDeselection(command.SubmittedDate, effectiveDate, username);

                //Save to the database
                var repoResult = Repository.Update(credential, username);
                if (!repoResult.Succeeded)
                    return Warning<MarkCertificateForDeselectCommandResult>($"Domain validation failed for CredentialId:'{command.CredentialId}' with message: '{repoResult.Message}'", repoResult);

                //Create a command result this way so we can include the 
                //Credential in the response data
                var commandResult =
                    new MarkCertificateForDeselectCommandResult(
                        CommandStatus.Accepted, repoResult, credential);

                return commandResult;
            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Could not mark credential {command.CredentialId} for deselection!");
                return Error<MarkCertificateForDeselectCommandResult>($"Failed to update latest issuance for Credential {command.CredentialId} for deselection: '{ex}'", validationResult);
            }
            finally
            {
                Log.Trace("Returning from Handle for MarkCertificateForDeselectCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<MarkCertificateForDeselectCommand>.Validate(MarkCertificateForDeselectCommand command)
        {
            return base.Validate<MarkCertificateForDeselectCommand>(command);
        }

        #endregion MarkCertificateForDeSelectCommand

        #region MarkCertificateForSelectCommand
        /// <summary>
        /// Handles a MarkCertificateForSelectCommand
        /// </summary>
        /// <param name="command">The command to handle</param>
        /// <returns>An ICommandResult indiciating success or failure</returns>
        public ICommandResult Handle(MarkCertificateForSelectCommand command)
        {
            //For PBI 187666
            Log.Trace("Started Handle for MarkCertificateForSelectCommand");
            Log.Debug("Command Args for MarkCertificateForSelectCommand: {0}", command.Dump());
            AbimValidationResult validationResult = null;
            AbimValidationResult repoResult = null;

            try
            {
                validationResult = ((ICommandValidationHandler<MarkCertificateForSelectCommand>)this).Validate(command);

                if (!validationResult.Succeeded)
                    return Warning<MarkCertificateForSelectCommandResult>($"Validation Failed: '{validationResult.Results.Dump()}'", validationResult);

                //Load the credential
                var credential = Load(command.CredentialId);

                // IsInitialFPHM does not have any isuances
                if (credential.NewestIssuance == null)
                    throw new ApplicationException($"No issuances found for credential {command.CredentialId}.");

                string username = command?.UserInfo?.Username ?? "markCertificateForSelectCommand";

                //If already marked for deselection and it is not initial FPHM (no issuances)
                if (credential.DeselectionElected)
                {
                    credential.NewestIssuance.MarkForSelection(username);

                    //save to the database
                    repoResult = Repository.Update(credential, username);

                    if (!repoResult.Succeeded)
                        return Warning<MarkCertificateForSelectCommandResult>($"Domain validation failed for CredentialId:'{command.CredentialId}' with message: '{repoResult.Message}'", repoResult);

                    return new MarkCertificateForSelectCommandResult(CommandStatus.Accepted, repoResult, credential);
                }

                // don't addd credential data if nothing was updated  (last parameter is null)
                return new MarkCertificateForSelectCommandResult(CommandStatus.Accepted, repoResult, null);

            }
            catch (Exception ex)
            {
                Log.Error(ex, $"Could not mark credential {command.CredentialId} for selection!");
                return Error<MarkCertificateForSelectCommandResult>($"Failed to update latest issuance for Credential {command.CredentialId} for selection: '{ex}'", validationResult);
            }
            finally
            {
                Log.Trace("Returning from Handle for MarkCertificateForSelectCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<MarkCertificateForSelectCommand>.Validate(MarkCertificateForSelectCommand command)
        {
            return base.Validate<MarkCertificateForSelectCommand>(command);
        }

        #endregion MarkCertificateForSelectCommand

        #region MarkCertificatesForSelectOrDeselectCommand
        /// <summary>
        /// Handles a MarkCertificateForDeselectCommand
        /// </summary>
        /// <param name="command">The command to handle</param>
        /// <returns>An ICommandResult indiciating success or failure</returns>
        public ICommandResult Handle(MarkCertificatesForSelectOrDeselectCommand command)
        {
            Log.Trace("Started Handle for MarkCertificatesForSelectOrDeselectCommand");
            Log.Debug("Command Args for MarkCertificatesForSelectOrDeselectCommand: {0}", command.Dump());
            AbimValidationResult validationResult = null;
            bool transactionBegun = false;
            List<MarkCertificateForDeselectCommandResult> deselectCommandResults = null;
            List<MarkCertificateForSelectCommandResult> selectCommandResults = null;

            try
            {
                validationResult = ((ICommandValidationHandler<MarkCertificatesForSelectOrDeselectCommand>)this).Validate(command);

                if (!validationResult.Succeeded)
                    return Warning<MarkCertificatesForSelectOrDeselectCommandResult>($"Validation Failed: '{validationResult.Results.Dump()}'", validationResult);

                deselectCommandResults =
                    new List<MarkCertificateForDeselectCommandResult>(); //remove initializing size of the list since command.CredentialIdsForDeselect can be null

                selectCommandResults =
                    new List<MarkCertificateForSelectCommandResult>();

                Repository.BeginTransaction();
                transactionBegun = true;

                if (command.CredentialIdsForDeselect != null)
                {
                    foreach (var id in command.CredentialIdsForDeselect)
                    {
                        var markCertForDeselectCommand =
                            new MarkCertificateForDeselectCommand
                            {
                                CredentialId = id,
                                SubmittedDate = DateTime.Now,
                                UserInfo = command.UserInfo
                            };

                        var result =
                            (MarkCertificateForDeselectCommandResult)Handle(markCertForDeselectCommand);

                        if (result.Status != CommandStatus.Accepted)
                            throw new ApplicationException($"Failed to mark credential {id} for deselection: {result.Message}.");

                        deselectCommandResults.Add(result);
                    }
                }

                if (command.CredentialIdsForSelect != null)
                {
                    foreach (var id in command.CredentialIdsForSelect)
                    {
                        var markCertForSelectCommand =
                                new MarkCertificateForSelectCommand
                                {
                                    CredentialId = id,
                                    UserInfo = command.UserInfo
                                };

                        var result =
                            (MarkCertificateForSelectCommandResult)Handle(markCertForSelectCommand);

                        if (result.Status != CommandStatus.Accepted)
                            throw new ApplicationException($"Failed to mark credential {id} for deselection: {result.Message}.");

                        // only add to command results when we updated that credential, result.Data has credential (see markCertForSelectCommand handler) )
                        if (result.Data != null)
                            selectCommandResults.Add(result);

                    }
                }

                Repository.CommitTransaction();

                //run the Corrective Action process to establish the certificate’s true certification and participation statuses, 
                //  along with any MOC deficiencies.
                if (selectCommandResults.Any())
                {
                    var updatedCredentialIds = selectCommandResults.Where(a => a.Succeeded).Select(b => b.Data.ExternalId);

                    //Run Corrective Action for the diplomate which would evaluate current participation/certification status for all active certificates.
                    Task.Run(() =>
                            ProgramRulesService.RunCorrectiveActionForMember(
                                                    memberId: command.MemberId,
                                                    eventDate: DateTime.Now,
                                                    processingDate: DateTime.Now,
                                                    triggeringEvent: TriggeringEvent.ReinstateCredential)).Wait();

                    //PBI 180989 : LNG Fees: ABIM Systems Notification of Diplomate upon Reactivation of Certificate
                    // publish the notification event and the event would be consumed by the notification platform and the marketing cloud would handle the rest. 
                    var certNames = GetCertNamesByCredentialExternalId(command.MemberId, updatedCredentialIds);

                    if (certNames.Count == 0)
                        throw new ApplicationException($"Failed to find certification Names for selected Credentials Ids: {string.Join(", ", command.CredentialIdsForSelect)}.");

                    Task.Run(() =>
                             HelperService.TriggeredCommunication_Reactivate_Certifications(certNames, command.MemberId)).Wait();
                }

                //PBI 187663
                //Publish CertificateDeselectedEvents for each deselected credential
                foreach (var result in deselectCommandResults)
                {
                    Task.Run(() => PublishCertDeselected(result.Data)).Wait();
                }

                //PBI 187665
                //Publish CertSelected for each selected credential
                foreach (var result in selectCommandResults)
                {
                    Task.Run(() => PublishCertSelected(result.Data)).Wait();
                }

                //If we've made it this far, everything was successful. :)
                return validationResult.ToCommandResult<MarkCertificatesForSelectOrDeselectCommandResult>();
            }
            catch (Exception ex)
            {
                //Rollback transaction. If rollback throws an exception, catch it and continue.
                try
                {
                    if (transactionBegun)
                        Repository.RollbackTransaction();
                }
                catch
                {
                    //Nothing to do here
                }

                Log.Error(ex, "Failed to mark credentials for selection or deselection!");
                return Error<MarkCertificatesForSelectOrDeselectCommandResult>($"Failed to mark credentials for deselection: '{ex}'", validationResult);
            }
            finally
            {
                Log.Trace("Returning from Handle for MarkCertificatesForSelectOrDeselectCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<MarkCertificatesForSelectOrDeselectCommand>.Validate(MarkCertificatesForSelectOrDeselectCommand command)
        {
            return base.Validate<MarkCertificatesForSelectOrDeselectCommand>(command);
        }
        #endregion MarkCertificatesForDeselectCommand

        #region ExpireIssuanceCommand

        /// <summary>
        /// Handles an ExpireIssuanceCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public ICommandResult Handle(ExpireIssuanceCommand command)
        {
            Log.Trace("Started Handle for ExpireIssuanceCommand");
            Log.Debug("Command Args for ExpireIssuanceCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<ExpireIssuanceCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<ExpireIssuanceCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                //fetch the existing domain object
                var credential = Repository.Load(command.CredentialId);

                if (credential == null) return Warning<ExpireIssuanceCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

                //check that the issuance exists
                var issuance = credential.Issuances.FirstOrDefault(i => i.Id == command.IssuanceId);

                if (issuance == null) return Warning<ExpireIssuanceCommandResult>(ErrorMessages.NotFound("Issuance", command.IssuanceId), cmdValidation);

                if (!issuance.CanExpire) return Warning<ExpireIssuanceCommandResult>($"Cannot expire issuance {issuance.Id} because it is not time-limited or has no expiration date", cmdValidation);

                //set UserName who made modifications
                issuance.AuditData.ModifiedBy = command.UserInfo.Username;

                //expire the issuance
                issuance.Expire(command.Status, command.MaintenanceStatus);

                //save to the database
                var errorResult = TryUpdate<ExpireAndReissueCommandResult>(credential, cmdValidation, command.UserInfo.Username);
                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential));

                //return
                return cmdValidation.ToCommandResult<ExpireIssuanceCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for ExpireIssuanceCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        AbimValidationResult ICommandValidationHandler<ExpireIssuanceCommand>.Validate(ExpireIssuanceCommand command)
        {
            return base.Validate<ExpireIssuanceCommand>(command);
        }

        #endregion

        #region ExpireAndReissueCommand

        /// <summary>
        /// Handles a ExpireAndReissueCommand command.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(ExpireAndReissueCommand command)
        {
            Log.Trace("Started Handle for ExpireAndReissueCommand");
            Log.Debug("Command Args for ExpireAndReissueCommand: {0}", command.Dump());
            Log.Info($"ExpireAndReissueCommand Handler with CredentialId:{command.CredentialId} IssuanceDate:{command.IssuanceDate.ToShortDateString()} on Thread:{Thread.CurrentThread.ManagedThreadId}");
            try
            {
                var cmdValidation = ((ICommandValidationHandler<ExpireAndReissueCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<ExpireAndReissueCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                //apply it
                credential.ExpireAndReissue(credential.NewestIssuance.Id, SourceService.GetAbimSource(), command.MaintenanceStatus,
                    command.IssuanceDate, command.ScheduledUpdate, command.CreatedBy);

                //save to the database
                //older code: var errorResult = TryUpdate<ExpireAndReissueCommandResult>(credential, cmdValidation, command.CreatedBy);
                var errorResult = Repository.UpdateCredential(credential, command.CreatedBy);

                Log.Info($"Updated credentials with AbimValidationResult: {errorResult?.Dump()}");

                if (errorResult.Succeeded)
                {
                    Log.Info($"Calling PublishIssueChangedEvent for memberId: {credential?.MemberId}");
                    // PBI 95348: MOCP - Earned a new must-be-maintained MOC certificate
                    Task.Run(() => PublishIssueChangedEvent(credential, TriggeredCommunication.EarnedMBMCertLetter, credential.MemberId));
                }
                else
                {
                    // just warning for UNIQUE KEY constraint 'NK_Issuance' 
                    if (errorResult.Message.Contains("'NK_Issuance'"))
                    {
                        Log.Warn($"'NK_Issuance' was Caught in ExpireAndReissueCommand Handler with CredentialId:{command.CredentialId} IssuanceDate:{command.IssuanceDate.ToShortDateString()} on Thread:{Thread.CurrentThread.ManagedThreadId}");

                        throw new ApplicationException(StopFurtherExecutionMessage);

                    }
                    else
                        return Warning<ExpireAndReissueCommandResult>($"Failed Repository.UpdateCredential with message: '{errorResult.Message}'", errorResult);
                }

                return cmdValidation.ToCommandResult<ExpireAndReissueCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for ExpireAndReissueCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(ExpireAndReissueCommand command)
        {
            return base.Validate<ExpireAndReissueCommand>(command);
        }

        #endregion

        #region ReissueCommand

        /// <summary>
        /// Handles a ReissueCommand command. Loads the credential and the source whose code is "ABIM", creates the event from that
        /// data and from the command, applies the event, then updates the credential in the database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(ReissueCommand command)
        {
            Log.Trace("Started Handle for ReissueCommand");
            Log.Debug("Command Args for ReissueCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<ReissueCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<ReissueCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                var source = SourceService.GetAbimSource();
                if (source == null) return Warning<ReissueCommandResult>($"Default {typeof(Source).Name} of code 'ABIM' not found", cmdValidation);

                //apply the changes
                credential.Reissue(source, command.IssuanceDate, command.ScheduledUpdate, command.CreatedBy, command.MaintenanceStatus);

                if (command.ReAttestationDueDate.HasValue)
                    credential.ReAttestationDueDate = command.ReAttestationDueDate.Value;

                //save to the database
                var errorResult = TryUpdate<ReissueCommandResult>(credential, cmdValidation, command.CreatedBy);

                if (errorResult != null) return errorResult;

                Log.Info($"Calling PublishIssueChangedEvent for memberId: {credential?.MemberId}");
                Task.Run(() => PublishIssueChangedEvent(credential));

                return cmdValidation.ToCommandResult<ReissueCommandResult, Credential>(credential);

            }
            finally
            {
                Log.Trace("Returning from Handle for ReissueCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(ReissueCommand command)
        {
            return base.Validate<ReissueCommand>(command);
        }

        #endregion

        #region ReInstateTLCommand

        /// <summary>
        /// Handles a ReinstateTLCommand command. Loads the credential, creates the event from that
        /// data and from the command, applies the event, then updates the credential in the database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(ReinstateTLCommand command)
        {
            Log.Trace("Started Handle for ReinstateTLCommand");
            Log.Debug("Command Args for ReinstateTLCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<ReinstateTLCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<ReinstateTLCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                //apply the changes
                credential.ReinstateTL(command.ModifiedBy, command.IssuanceStatus, command.MaintenanceStatus);

                //save to the database
                var errorResult = TryUpdate<ReinstateTLCommandResult>(credential, cmdValidation, command.ModifiedBy);
                if (errorResult != null) return errorResult;

                return cmdValidation.ToCommandResult<ReinstateTLCommandResult, Credential>(credential);

            }
            finally
            {
                Log.Trace("Returning from Handle for ReinstateTLCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(ReinstateTLCommand command)
        {
            return base.Validate<ReinstateTLCommand>(command);
        }

        #endregion


        #region SetIssuanceToMaintainedCommand

        /// <summary>
        /// Handles a SetIssuanceToMaintainedCommand command. Loads the credential and the source whose code is "ABIM", creates the event from that
        /// data and from the command, applies the event, then updates the credential in the database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(SetIssuanceToMaintainedCommand command)
        {
            Log.Trace("Started Handle for SetIssuanceToMaintainedCommand");
            Log.Debug("Command Args for SetIssuanceToMaintainedCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<SetIssuanceToMaintainedCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<SetIssuanceToMaintainedCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                //apply the changes
                credential.SetMaintained(command.ModifiedBy);

                //save to the database
                var errorResult = TryUpdate<SetIssuanceToMaintainedCommandResult>(credential, cmdValidation, command.ModifiedBy);
                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential));

                return cmdValidation.ToCommandResult<SetIssuanceToMaintainedCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for SetIssuanceToMaintainedCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(SetIssuanceToMaintainedCommand command)
        {
            return base.Validate<SetIssuanceToMaintainedCommand>(command);
        }

        #endregion

        #endregion

        #region UpdateCredentialGracePeriodForGF

        /// <summary>
        /// Handles a SetIssuanceToMaintainedCommand command. Loads the credential and the source whose code is "ABIM", creates the event from that
        /// data and from the command, applies the event, then updates the credential in the database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(UpdateCredentialGracePeriodGFCommand command)
        {
            Log.Trace("Started Handle for UpdateCredentialGracePeriodForGF");
            Log.Debug("Command Args for UpdateCredentialGracePeriodForGF: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateCredentialGracePeriodGFCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UpdateCredentialGracePeriodGFCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                //apply the changes
                credential.SetMaintainedPreviousYearForGF(command.ModifiedBy, command.MaintenanceStatus, command.ProcessingDate);

                //save to the database
                var errorResult = TryUpdate<UpdateCredentialGracePeriodGFCommandResult>(credential, cmdValidation, command.ModifiedBy);
                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential));

                return cmdValidation.ToCommandResult<UpdateCredentialGracePeriodGFCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for UpdateCredentialGracePeriodForGF");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(UpdateCredentialGracePeriodGFCommand command)
        {
            return base.Validate<UpdateCredentialGracePeriodGFCommand>(command);
        }

        #endregion

        #region UpdateCredentialGracePeriodForTLandMBM

        /// <summary>
        /// Handles a SetIssuanceToMaintainedCommand command. Loads the credential and the source whose code is "ABIM", creates the event from that
        /// data and from the command, applies the event, then updates the credential in the database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(UpdateCredentialGracePeriodTLandMBMCommand command)
        {
            Log.Trace("Started Handle for UpdateCredentialGracePeriodForTLandMBM");
            Log.Debug("Command Args for UpdateCredentialGracePeriodForTLandMBM: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateCredentialGracePeriodTLandMBMCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UpdateCredentialGracePeriodTLandMBMCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                //apply changes: Set Maintainance Status for Time Limited (TL) and Must-Be-Maintained (MBM) ONLY
                credential.SetMaintainedForTLandMBM(command.ModifiedBy, command.MaintenanceStatus, command.ProcessingDate);

                //save to the database
                var errorResult = TryUpdate<UpdateCredentialGracePeriodTLandMBMCommandResult>(credential, cmdValidation, command.ModifiedBy);
                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential));

                return cmdValidation.ToCommandResult<UpdateCredentialGracePeriodTLandMBMCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for UpdateCredentialGracePeriodForTLandMBM");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(UpdateCredentialGracePeriodTLandMBMCommand command)
        {
            return base.Validate<UpdateCredentialGracePeriodTLandMBMCommand>(command);
        }

        #endregion

        #region UpdateCredentialOnExamResultCommand

        /// <summary>
        /// Handles a UpdateCredentialOnExamResultCommand command. Loads the credential , creates the event from that
        /// data and from the command, applies the event, then updates the credential in the database.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <returns></returns>
        public ICommandResult Handle(UpdateCredentialOnExamResultCommand command)
        {
            Log.Trace("Started Handle for UpdateCredentialOnExamResultCommand");
            Log.Debug("Command Args for UpdateCredentialOnExamResultCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateCredentialOnExamResultCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UpdateCredentialOnExamResultCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.CredentialId);

                DateTime? expirationDate = null;

                expirationDate = new DateTime(command.AdministrationDate.Value.Year, 12, 31);

                // apply the changes
                credential.SetCredentialOnExamResult(expireActiveIssuances: command.ExpireActiveIssuances,
                                                    examDueDate: command.ExamDueDate,
                                                    displayExamDueDate: command.DisplayExamDueDate,
                                                    mocExamDueDate: command.MOCExamDueDate,
                                                    kciExamDueDate: command.KCIExamDueDate,
                                                    forcedPathway: command.ForcedPathway,
                                                    pathway: command.Pathway,
                                                    examFailCount: command.ExamFailCount,
                                                    assessmentMet: command.AssessmentMet,
                                                    assessmentMetDate: command.AssessmentMetDate,
                                                    gracePeriodEndDate: command.GracePeriodEndDate,
                                                    gracePeriodStartDate: command.GracePeriodStartDate,
                                                    modifiedBy: command.ModifiedBy,
                                                    expirationDate: expirationDate,
                                                    isInCMP: command.IsInCMP,
                                                    processingDate: command.ProcessingDate
                                                    );

                //save to the database
                var errorResult = TryUpdate<UpdateCredentialOnExamResultCommandResult>(credential, cmdValidation, command.ModifiedBy);
                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential));

                return cmdValidation.ToCommandResult<UpdateCredentialOnExamResultCommandResult, Credential>(credential);

            }
            finally
            {
                Log.Trace("Returning from Handle for UpdateCredentialOnExamResultCommand");
            }
        }

        /// <summary>
        /// Validate command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UpdateCredentialOnExamResultCommand command)
        {
            return base.Validate<UpdateCredentialOnExamResultCommand>(command);
        }

        #endregion

        #region UpdatePathwayCommand

        /// <summary>
        /// Handles an UpdatePathwayCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<UpdatePathwayCommandResult> Handle(UpdatePathwayCommand command)
        {
            Log.Trace("Started Handle for UpdatePathwayCommand");
            Log.Info("Updating pathway for credential. Command object: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdatePathwayCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UpdatePathwayCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                Credential credential = Load(command.CredentialId);

                if (credential == null) return Warning<UpdatePathwayCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

                Log.Info($"Updating credential {credential.ExternalId} from pathway {credential.Pathway} to pathway {command.Pathway}");

                // set the value of the flag to the requested argument
                credential.SetPathway(command.Pathway, command.Username);

                //save to the database
                var errorResult = TryUpdate<UpdatePathwayCommandResult>(credential, cmdValidation, command.Username);
                if (errorResult != null) return errorResult;

                return cmdValidation.ToCommandResult<UpdatePathwayCommandResult, Credential>(credential);
            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }
            finally
            {
                Log.Trace("Returning from Handle for UpdatePathwayCommand");
            }

        }

        /// <summary>
        /// Validate a UpdatePathwayCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UpdatePathwayCommand command)
        {
            return Validate<UpdatePathwayCommand>(command);
        }

        #endregion UpdatePathwayCommand

        #region UpdateSelectedToMaintainCommand

        /// <summary>
        /// Handles an UpdateSelectedToMaintainCommand command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<UpdateSelectedToMaintainCommandResult> Handle(UpdateSelectedToMaintainCommand command)
        {
            Log.Trace("Started Handle for UpdateSelectedToMaintainCommand");
            Log.Debug("Command Args for UpdateSelectedToMaintainCommand: {0}", command.Dump());
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateSelectedToMaintainCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UpdateSelectedToMaintainCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                Credential credential = Load(command.CredentialId);

                if (credential == null) return Warning<UpdateSelectedToMaintainCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

                // set the value of the flag to the requested argument
                credential.SetSelectedToMaintain(command.SelectedToMaintain, command.Username);

                //save to the database
                var errorResult = TryUpdate<UpdateSelectedToMaintainCommandResult>(credential, cmdValidation, command.Username);
                if (errorResult != null) return errorResult;

                //- If they cancel their iCard certificate to maintain
                if (credential.Certification.IsICARD() && !command.SelectedToMaintain)
                    await PublishICardCertCancelToMaintainEvent(credential.MemberId);

                return cmdValidation.ToCommandResult<UpdateSelectedToMaintainCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for UpdateSelectedToMaintainCommand");
            }

        }

        /// <summary>
        /// Validate a UpdateSelectedToMaintainCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UpdateSelectedToMaintainCommand command)
        {
            return Validate<UpdateSelectedToMaintainCommand>(command);
        }

        #endregion


        #region Publish (Issue, ICardCertCancelToMaintain) ChangedEvent
        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <returns></returns>
        internal async Task PublishIssueChangedEvent(Credential credential)
        {
            Log.Info($"Called PublishIssueChangedEvent with Issuances {credential.Issuances?.Dump()}");
            var updatedIssuances = credential.Issuances.Where(a => a.HasChanged || a.HasAdded);

            foreach (var issuance in updatedIssuances)
            {
                Log.Info($"Event Publishing with IssuanceDate: {issuance.IssuanceDate} and ExpirationDate: {issuance.ExpirationDate}");

                await Bus.Publish(new IssuanceChanged()
                {
                    MemberId = issuance.Credential.MemberId,
                    CredentialGuid = credential.ExternalId,
                    Code = issuance.Credential.Certification.Code,
                    Status = issuance.IssuanceStatus.ToString(),
                    Occurrence = issuance.Occurrence.ToString(),
                    IssuanceDate = issuance.IssuanceDate,
                    ExpirationDate = issuance.ExpirationDate,
                    New = issuance.HasAdded ? true : false,
                    Cosponsored = credential.IsCosponsored,
                    ProcessingDate = DateTime.Now
                });

                Log.Info($"Event Published with IssuanceDate: {issuance.IssuanceDate} and ExpirationDate: {issuance.ExpirationDate}");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="triggerComunication"></param>
        /// <param name="memberId">Member Id.</param>
        /// <returns></returns>
        internal async Task PublishIssueChangedEvent(Credential credential, string triggerComunication, Guid memberId)
        {
            await PublishIssueChangedEvent(credential);
            // PBI 95348: MOCP - Earned a new must-be-maintained MOC certificate
            await HelperService.TriggeredCommunication(credential, triggerComunication, memberId, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="MemberId"></param>
        /// <returns></returns>
        internal async Task PublishICardCertCancelToMaintainEvent(Guid MemberId)
        {
            var busEvent = new ICardCertCancelToMaintainEvent()
            {
                MemberId = MemberId,
                ProcessingDate = DateTime.Now
            };

            await Bus.Publish(busEvent);
        }

        /// <summary>
        /// Publishes a CertDeselected
        /// </summary>
        /// <param name="credential">The Credential the Certificate Deselection affects</param>
        /// <returns></returns>
        internal async Task PublishCertDeselected(Credential credential)
        {
            var busEvent = new CertDeselected
            {
                MemberId = credential.MemberId,
                CredentialId = credential.ExternalId,
                CertificationId = credential.Certification.ExternalId,
                CertificationCode = credential.Certification.Code,
                Cosponsored = credential.IsCosponsored
            };

            await Bus.Publish<CertDeselected>(busEvent);
        }

        /// <summary>
        /// Publishes a CertSelected
        /// </summary>
        /// <param name="credential">The Credential the Certificate Selection affects</param>
        /// <returns></returns>
        internal async Task PublishCertSelected(Credential credential)
        {
            var busEvent = new CertSelected
            {
                MemberId = credential.MemberId,
                CredentialId = credential.ExternalId,
                CertificationId = credential.Certification.ExternalId,
                CertificationCode = credential.Certification.Code,
                Cosponsored = credential.IsCosponsored,
                InitialCertificationDate = credential.OldestIssuance.IssuanceDate
            };

            await Bus.Publish<CertSelected>(busEvent);
        }

        /// <summary>
        /// Publishes a CMP Enrolled Event
        /// </summary>
        /// <param name="credential">The Credential the Certificate Selection affects</param>
        /// <param name="EnrollmentDate">The Enrollment Date </param>
        /// <returns></returns>
        internal async Task PublishCMPEnrolled(Credential credential, DateTime EnrollmentDate)
        {
            var busEvent = new CMPEnrolled
            {
                MemberId = credential.MemberId,
                CredentialGuid = credential.ExternalId,
                EnrollmentDate = EnrollmentDate,
                ProcessingDate = DateTime.Now
            };

            await Bus.Publish<CMPEnrolled>(busEvent);
        }

        /// <summary>
        /// Publish a CMP Un Enrolled Event
        /// </summary>
        /// <param name="credential"></param>
        /// <param name="UnEnrollmentDate"></param>
        /// <param name="Voluntary"></param>
        /// <returns></returns>
        internal async Task PublishCMPUnEnrolledEvent(Credential credential, DateTime UnEnrollmentDate, bool Voluntary)
        {
            var busEvent = new CMPUnEnrolled
            {
                MemberId = credential.MemberId,
                CredentialGuid = credential.ExternalId,
                UnEnrollmentDate = UnEnrollmentDate,
                Voluntary = Voluntary,
                ProcessingDate = DateTime.Now
            };

            await Bus.Publish<CMPUnEnrolled>(busEvent);
        }

        #endregion

        #region WithdrawCredential

        /// <summary>
        /// Handles an WithdrawCredentialCommandResult command
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<WithdrawCredentialCommandResult> Handle(WithdrawCredentialCommand command)
        {
            Log.Debug("Command Args for WithdrawCredentialCommand: {0}", command.Dump());

            var cmdValidation = ((ICommandValidationHandler<WithdrawCredentialCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded)
                return Warning<WithdrawCredentialCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // load Credential
            Credential credential = Load(command.CredentialId);

            if (credential == null)
                return Warning<WithdrawCredentialCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

            // make sure it is issued by ABIM
            if (credential.NewestIssuance.Source.Code != "ABIM")
                return Warning<WithdrawCredentialCommandResult>($"CredentialId '{credential.ExternalId}' is NOT issued by ABIM", cmdValidation);

            credential.SetWithdrawn(command.WithdrawnDate, command.WithdrawnStatus, command.Username);

            //save to the database
            var errorResult = TryUpdate<WithdrawCredentialCommandResult>(credential, cmdValidation, command.Username);
            if (errorResult != null) return errorResult;

            // notify of Issuance change
            await Task.Run(() => PublishIssueChangedEvent(credential));

            return cmdValidation.ToCommandResult<WithdrawCredentialCommandResult, Credential>(credential);
        }

        /// <summary>
        /// Validate a WithdrawCredentialCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(WithdrawCredentialCommand command)
        {
            return Validate<WithdrawCredentialCommand>(command);
        }

        #endregion

        #region ReinstateCredential

        /// <summary>
        /// Handles an ReinstateCredentialCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<ReinstateCredentialCommandResult> Handle(ReinstateCredentialCommand command)
        {
            Log.Debug("Command Args for ReinstateCredentialCommand: {0}", command.Dump());

            var cmdValidation = ((ICommandValidationHandler<ReinstateCredentialCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded)
                return Warning<ReinstateCredentialCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // load Credential
            Credential credential = Load(command.CredentialId);

            if (credential == null)
                return Warning<ReinstateCredentialCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

            // make sure most recent issuance is Inactive or Revoked or Surrendered or Suspended
            if (!(credential.NewestIssuance.IssuanceStatus == IssuanceStatusType.Inactive ||
                    credential.NewestIssuance.IssuanceStatus == IssuanceStatusType.Revoked ||
                    credential.NewestIssuance.IssuanceStatus == IssuanceStatusType.Surrendered ||
                    credential.NewestIssuance.IssuanceStatus == IssuanceStatusType.Suspended))
            {
                return Warning<ReinstateCredentialCommandResult>($"CredentialId '{credential.ExternalId}' is NOT in Inactive, Revoked, Surrendered or Suspended status.", cmdValidation);
            }

            credential.SetReinstate(command.Username);

            //save to the database
            var errorResult = TryUpdate<ReinstateCredentialCommandResult>(credential, cmdValidation, command.Username);
            if (errorResult != null) return errorResult;

            // run Corrective Actions for memberId with ReinstateDate as EventDate
            await ProgramRulesService.RunCorrectiveActionForMember(credential.MemberId, command.ReinstateDate, DateTime.Now, TriggeringEvent.ReinstateCredential);

            // notify of Issuance change
            await Task.Run(() => PublishIssueChangedEvent(credential));

            return cmdValidation.ToCommandResult<ReinstateCredentialCommandResult, Credential>(credential);
        }

        /// <summary>
        /// Validate a ReinstateCredentialCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(ReinstateCredentialCommand command)
        {
            return Validate<ReinstateCredentialCommand>(command);
        }

        #endregion

        #region ETL processing endpoints

        #region AddCredential

        /// <summary>
        /// Handles an AddCredentialCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<AddCredentialCommandResult> Handle(AddCredentialCommand command)
        {
            Log.Debug("Command Args for AddCredentialCommand: {0}", command.Dump());

            var cmdValidation = ((ICommandValidationHandler<AddCredentialCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded)
                return Warning<AddCredentialCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // Fetch the certification
            var certification = CertificationService.Load(command.CertificationId);

            if (certification == null)
                return Warning<AddCredentialCommandResult>(ErrorMessages.NotFound("Certification", command.CertificationId), cmdValidation);

            // Create the model
            var credential = Credential.Create(certification, command.MemberId, command.Type, command.Pathway, null, null, command.UserInfo.Username); //PBI 215404 - The null params for OnBehalfBoardCode and OnBehalfBoardName are because cosponsored certs get created either via the data import or by earning a new cert, neither of which is happening in this method

            //add to the database
            return Add<AddCredentialCommandResult>(credential, cmdValidation, command.UserInfo.Username);

        }

        /// <summary>
        /// Validate a AddCredentialCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(AddCredentialCommand command)
        {
            return Validate<AddCredentialCommand>(command);
        }

        #endregion

        #region UpdateCredential

        /// <summary>
        /// Handles an UpdateCredentialCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<UpdateCredentialCommandResult> Handle(UpdateCredentialCommand command)
        {
            Log.Debug("Command Args for UpdateCredentialCommand: {0}", command.Dump());

            var cmdValidation = ((ICommandValidationHandler<UpdateCredentialCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded) return Warning<UpdateCredentialCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // Fetch the credential
            var credential = Repository.GetCredentialByMemberAndCert(command.MemberId, command.CertificationId);

            if (credential == null) return Warning<UpdateCredentialCommandResult>($"Credential is not found for MemberId:'{command.MemberId}' and CertificationId:'{command.CertificationId}'", cmdValidation);

            credential.ApplyUpdateCredentialEvent(command.Pathway, command.Type, command.UserInfo.Username);

            //save to the database
            return Update<UpdateCredentialCommandResult>(credential, cmdValidation, command.UserInfo.Username);

        }

        /// <summary>
        /// Validate a UpdateCredentialCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UpdateCredentialCommand command)
        {
            return Validate<UpdateCredentialCommand>(command);
        }

        #endregion

        #region AddIssuance

        /// <summary>
        /// Handles an AddIssuanceCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<AddIssuanceCommandResult> Handle(AddIssuanceCommand command)
        {
            Log.Debug("Command Args for AddIssuanceCommand: {0}", command.Dump());

            AbimValidationResult objValidation = null;

            var cmdValidation = ((ICommandValidationHandler<AddIssuanceCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded)
                return Warning<AddIssuanceCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // Fetch the credential
            var credential = Load(command.CredentialId);

            if (credential == null)
                return Warning<AddIssuanceCommandResult>(ErrorMessages.NotFound("Credential", command.CredentialId), cmdValidation);

            var source = SourceService.Load(command.SourceId);

            if (source == null)
                return Warning<AddIssuanceCommandResult>(ErrorMessages.NotFound("Source", command.SourceId), cmdValidation);

            // Create the model
            var issuance = Issuance.Create(source,
                                        command.Duration,
                                        command.MaintenanceRequirement,
                                        command.MaintenanceStatus,
                                        command.Occurrence,
                                        command.IssuanceStatus,
                                        command.IssuanceDate,
                                        command.EffectiveDate,
                                        command.UserInfo.Username);

            issuance.ExpirationDate = command.ExpirationDate;

            credential.AddIssuance(issuance);

            credential.UpdateIsActive();

            //add to the database
            Repository.BeginTransaction();
            objValidation = await Task.Run(() => Repository.Add(credential));
            if (objValidation.Succeeded) Repository.CommitTransaction();
            else Repository.RollbackTransaction();

            await Task.Run(() => PublishIssueChangedEvent(credential));

            return cmdValidation.ToCommandResult<AddIssuanceCommandResult, Issuance>(issuance);
        }

        /// <summary>
        /// Validate a AddIssuanceCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(AddIssuanceCommand command)
        {
            return Validate<AddIssuanceCommand>(command);
        }

        #endregion

        #region UpdateIssuance

        /// <summary>
        /// Handles an UpdateIssuanceCommand
        /// </summary>
        /// <param name="command">The command</param>
        /// <returns>ICommandResult</returns>
        public async Task<UpdateIssuanceCommandResult> Handle(UpdateIssuanceCommand command)
        {
            Log.Debug("Command Args for UpdateIssuanceCommand: {0}", command.Dump());

            AbimValidationResult objValidation = null;

            var cmdValidation = ((ICommandValidationHandler<UpdateIssuanceCommand>)this).Validate(command);

            if (!cmdValidation.Succeeded) return Warning<UpdateIssuanceCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

            // Fetch the credential
            var issuance = Repository.GetIssuanceByCredentialIdIssuanceDate(command.CredentialId, command.IssuanceDate);

            if (issuance == null) return Warning<UpdateIssuanceCommandResult>($"Issuance is not found for CredentialId:'{command.CredentialId}' and IssuanceDate:'{command.IssuanceDate.ToShortDateString()}'", cmdValidation);

            var source = SourceService.Load(command.SourceId);

            if (source == null)
                return Warning<UpdateIssuanceCommandResult>(ErrorMessages.NotFound("Source", command.SourceId), cmdValidation);

            issuance.ApplyUpdateIssuanceEvent(source,
                                        command.Duration,
                                        command.MaintenanceRequirement,
                                        command.MaintenanceStatus,
                                        command.Occurrence,
                                        command.IssuanceStatus,
                                        command.IssuanceDate,
                                        command.EffectiveDate,
                                        command.ExpirationDate,
                                        command.UserInfo.Username);

            try
            {

                Repository.BeginTransaction();
                objValidation = Repository.Update(issuance.Credential, command.UserInfo.Username);
                if (objValidation.Succeeded) Repository.CommitTransaction();
                else Repository.RollbackTransaction();

            }
            catch (Exception ex)
            {
                return Error<UpdateIssuanceCommandResult>($"Failed to Update the Issuance for CredentialId:'{command.CredentialId}' and IssuanceDate:'{command.IssuanceDate.ToShortDateString()}' in the database: '{ex}'", cmdValidation);
            }

            if (!objValidation.Succeeded)
                return Warning<UpdateIssuanceCommandResult>($"Domain validation failed for CredentialId:'{command.CredentialId}' and IssuanceDate:'{command.IssuanceDate.ToShortDateString()}' with message: '{objValidation.Message}'", cmdValidation);

            await Task.Run(() => PublishIssueChangedEvent(issuance.Credential));

            return cmdValidation.ToCommandResult<UpdateIssuanceCommandResult, Issuance>(issuance);
        }

        /// <summary>
        /// Validate a UpdateCredentialCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UpdateIssuanceCommand command)
        {
            return Validate<UpdateIssuanceCommand>(command);
        }

        #endregion

        #endregion

        #region UpdateCredentialFromLookbackCommand

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommandResult Handle(UpdateCredentialFromLookbackCommand command)
        {
            Log.Trace("Started Handle for UpdateCredentialFromLookbackCommand");
            Log.Debug($"Command Args for UpdateCredentialFromLookbackCommand CredentialId:'{command.Credential.Id}', ModifiedBy:'{command.ModifiedBy}'");
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateCredentialFromLookbackCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded)
                    return Warning<UpdateCredentialFromLookbackCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                //The command contains the credential which has already been updated during the lookback process.
                //No need to reload it. Just attempt to persist.
                var errorResult =
                    TryUpdate<UpdateCredentialFromLookbackCommandResult>(
                        command.Credential, cmdValidation, command.ModifiedBy);

                if (errorResult != null) return errorResult;

                //Because Corrective Action could run after this Handle() method is invoked, 
                //and could modify the issuances, let's wait until this finishes until we 
                //return. During testing, exceptions were occurring because the issuances 
                //enumeration was modified elsewhere while still being iterated through in 
                //the method below.
                var task = PublishIssueChangedEvent(command.Credential);
                task.Wait();
                if (task.IsFaulted)
                    throw task.Exception;

                return cmdValidation.ToCommandResult<UpdateCredentialFromLookbackCommandResult, Credential>(command.Credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for UpdateCredentialFromLookbackCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(UpdateCredentialFromLookbackCommand command)
        {
            return base.Validate<UpdateCredentialFromLookbackCommand>(command);
        }

        #endregion UpdateCredentialFromLookbackCommand

        #region UpdateCredentialFromObjectCommand

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public ICommandResult Handle(UpdateCredentialFromObjectCommand command)
        {
            Log.Trace("Started Handle for UpdateCredentialFromObjectCommand");
            Log.Debug($"Command Args for UpdateCredentialFromObjectCommand CredentialId:'{command.Credential.Id}', SetGracePeriod:{command.SetGracePeriod}," +
                    $"ReistateCertificate:{command.ReistateCertificate}, SetConsecutiveKCIPassRequired:{command.SetConsecutiveKCIPassRequired}, SetCertStatus:{command.SetCertStatus}, SetParticipationStatus:{command.SetParticipationStatus}");
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UpdateCredentialFromObjectCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UpdateCredentialFromObjectCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                var credential = Repository.Load(command.Credential.ExternalId);

                if (credential == null) throw new ApplicationException($"Couldn't find credential {command.Credential.ExternalId}.");

                //*** apply the changes ***

                if (command.SetGracePeriod)
                {
                    credential.GracePeriodStartDate = command.Credential.GracePeriodStartDate;
                    credential.GracePeriodEndDate = command.Credential.GracePeriodEndDate;
                    credential.SetModified(command.Credential.AuditData.ModifiedBy);
                }

                if (command.ReistateCertificate)
                {
                    credential.NewestIssuance.IssuanceStatus = command.Credential.NewestIssuance.IssuanceStatus;
                    credential.NewestIssuance.ExpiredDate = command.Credential.NewestIssuance.ExpiredDate; // pbi 142310 : reinstate certificate - set expired to null
                    credential.NewestIssuance.MaintenanceStatus = command.Credential.NewestIssuance.MaintenanceStatus;
                    credential.NewestIssuance.SetModified(command.Credential.NewestIssuance.AuditData.ModifiedBy);
                    credential.IsActive = command.Credential.IsActive;
                    credential.SetModified(command.Credential.AuditData.ModifiedBy);
                }

                if (command.SetConsecutiveKCIPassRequired)
                {
                    credential.ConsecutiveKCIPassRequired = command.Credential.ConsecutiveKCIPassRequired;
                    credential.SetModified(command.Credential.AuditData.ModifiedBy);
                }

                if (command.SetCertStatus)
                {
                    foreach (var issuance in command.Credential.Issuances.Where(a => a.HasChanged))
                    {
                        var issuanceToUpdate = credential.Issuances.Where(i => i.Id == issuance.Id).First();
                        issuanceToUpdate.IssuanceStatus = issuance.IssuanceStatus;
                        issuanceToUpdate.MaintenanceStatus = issuance.MaintenanceStatus;
                        if (issuance.AuditData.ModifiedBy != null)
                            issuanceToUpdate.SetModified(issuance.AuditData.ModifiedBy);
                    }
                    credential.IsActive = command.Credential.IsActive;
                    if (command.Credential.AuditData.ModifiedBy != null)
                        credential.SetModified(command.Credential.AuditData.ModifiedBy);
                }

                if (command.SetParticipationStatus)
                {
                    foreach (var issuance in command.Credential.Issuances.Where(a => a.HasChanged))
                    {
                        var issuanceToUpdate = credential.Issuances.Where(i => i.Id == issuance.Id).First();
                        issuanceToUpdate.MaintenanceStatus = issuance.MaintenanceStatus;
                        issuanceToUpdate.SetModified(issuance.AuditData.ModifiedBy);
                    }
                }

                //*** save to the database
                var errorResult = TryUpdate<UpdateCredentialFromObjectCommandResult>(credential, cmdValidation, command.Credential.AuditData.ModifiedBy);
                if (errorResult != null) return errorResult;

                Task.Run(() => PublishIssueChangedEvent(credential))
                    .ContinueWith((task) =>
                    {
                        if (task.IsFaulted) throw task.Exception;
                    }
                    );

                return cmdValidation.ToCommandResult<UpdateCredentialFromObjectCommandResult, Credential>(credential);

            }
            finally
            {
                Log.Trace("Returning from Handle for UpdateCredentialFromObjectCommand");
            }
        }

        /// <summary>
        /// Validates the command
        /// </summary>
        public AbimValidationResult Validate(UpdateCredentialFromObjectCommand command)
        {
            return base.Validate<UpdateCredentialFromObjectCommand>(command);
        }

        #endregion

        #region ACC project 

        #region EnrollInCMPCommand

        /// <summary>
        /// Handles an EnrollInCMPCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<EnrollInCMPCommandResult> Handle(EnrollInCMPCommand command)
        {
            try
            {
                var cmdValidation = ((ICommandValidationHandler<EnrollInCMPCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<EnrollInCMPCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                if (command.MemberId == null || command.MemberId == Guid.Empty)
                    command.MemberId = await HelperService.GetMemberIdByAbimId(command.AbimId);

                // if it is still empty (not found then return warning message)
                if (command.MemberId == Guid.Empty)
                    return Warning<EnrollInCMPCommandResult>($"The user '{command.MemberId}' does not exists.", cmdValidation);

                var credential = GetCredentialByMemberAndCode(command.MemberId, command.SubspecialtyCertCode);

                if (credential == null) return Warning<EnrollInCMPCommandResult>($"The user '{command.MemberId}' does not have credential with Subspecialty Code '{command.SubspecialtyCertCode}'", cmdValidation);

                if (credential.IsInCMP) return Warning<EnrollInCMPCommandResult>($"The user '{command.MemberId}' is already enrolled in the CMP pathway.", cmdValidation);

                // set apply method
                credential.SetEnrollInCMP(command.RequestingUserName, command.EnrollmentDate);

                //save to the database
                var errorResult = TryUpdate<EnrollInCMPCommandResult>(credential, cmdValidation, command.RequestingUserName);
                if (errorResult != null) return errorResult;

                // pbi 278052 : (HF after 2.50)Cert Fees for pre-1990 CMP enrollees - team 4 work
                // *** Enrollment in CMP from the data feed ACC.
                await Task.Run(() => PublishCMPEnrolled(credential, command.EnrollmentDate));

                return cmdValidation.ToCommandResult<EnrollInCMPCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for EnrollInCMPCommand");
            }
        }

        /// <summary>
        /// Validate a EnrollInCMPCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(EnrollInCMPCommand command)
        {
            return Validate<EnrollInCMPCommand>(command);
        }

        #endregion

        #region UnEnrollInCMPCommand

        /// <summary>
        /// Handles an UnEnrollInCMPCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public async Task<UnEnrollInCMPCommandResult> Handle(UnEnrollInCMPCommand command)
        {
            try
            {
                var cmdValidation = ((ICommandValidationHandler<UnEnrollInCMPCommand>)this).Validate(command);

                if (!cmdValidation.Succeeded) return Warning<UnEnrollInCMPCommandResult>($"Validation Failed: '{cmdValidation.Results.Dump()}'", cmdValidation);

                if (command.MemberId == null || command.MemberId == Guid.Empty)
                    command.MemberId = await HelperService.GetMemberIdByAbimId(command.AbimId);

                // if it is still empty (not found then return warning message)
                if (command.MemberId == Guid.Empty)
                    return Warning<UnEnrollInCMPCommandResult>($"The user '{command.AbimId}' does not exists.", cmdValidation);

                var credential = GetCredentialByMemberAndCode(command.MemberId, command.SubspecialtyCertCode);

                if (credential == null) return Warning<UnEnrollInCMPCommandResult>($"The user '{command.MemberId}' does not have credential with Subspecialty Code '{command.SubspecialtyCertCode}'", cmdValidation);

                if (!credential.IsInCMP) return Warning<UnEnrollInCMPCommandResult>($"The user '{command.MemberId}' IS NOT enrolled in the CMP pathway.", cmdValidation);

                // *** find most recent Exam Type (default MOC)
                var mostRecentExamType = await HelperService.GetMostRecentExamTypeByCode(command.MemberId, credential.Certification.ExternalId);

                // set by default to MOC but if we find later it is KCI then we would change it
                Resources.PathwayType priviousPathway = Resources.PathwayType.MOC;

                if (mostRecentExamType == ExamType.Kci)
                    priviousPathway = Resources.PathwayType.KCI;

                // set apply method
                credential.SetUnEnrollInCMP(command.RequestingUserName, priviousPathway, command.UnEnrollmentDate);

                //save to the database
                var errorResult = TryUpdate<UnEnrollInCMPCommandResult>(credential, cmdValidation, command.RequestingUserName);
                if (errorResult != null) return errorResult;

                // *** scenario 1 : Unenrollment in CMP should be considered voluntary when the status change is due to the data feed from ACC.
                // *** scenario 2 : It would involuntary (voluntary = false) if the status change is made by ABIM, such as being removed from CMP due to Year End Look Back.
                bool voluntary = command.RequestingUserName == "KickCMP" ? false : true; // set in KickOutOfCMPIfApplicable() {ProgramRulesServiceYearEndLookback.cs}

                // pbi 278052 : (HF after 2.50)Cert Fees for pre-1990 CMP enrollees - team 4 work
                await Task.Run(() => PublishCMPUnEnrolledEvent(credential, UnEnrollmentDate: command.UnEnrollmentDate, Voluntary: voluntary));

                return cmdValidation.ToCommandResult<UnEnrollInCMPCommandResult, Credential>(credential);
            }
            finally
            {
                Log.Trace("Returning from Handle for UnEnrollInCMPCommand");
            }
        }

        /// <summary>
        /// Validate a UnEnrollInCMPCommand command
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public AbimValidationResult Validate(UnEnrollInCMPCommand command)
        {
            return Validate<UnEnrollInCMPCommand>(command);
        }

        #endregion

        #endregion

        #region Private Methods

        private DateTime GetEffectiveDateForCertDeSelect(DateTime submittedDate)
        {
            /*
            From PBI 187664 - obsolete
            All deselect requests made between February 1st and January 31st shall 
            be changed on February 1st of the new year.

            From PBI 286279 - Oct 24 2024 
            All deselect requests made between April 1st and March 31st shall 
            be changed on April 1st of the new year. 

            Example 1 (request made any time during the year): The deselect request 
            is made on May 10, 2024. The certificate status shall be changed on 
            April 1, 2025.

            Example 2 (IM with FPHM): The deselect request is made on 
            November 10, 2025. The certificate status of both IM and FPHM shall be 
            changed on April 1, 2026.

            Example 3 (request made any time during the year): The deselect request 
            is made on March 10, 2025. The certificate status shall be changed on 
            April 1, 2025.
            */
            if (submittedDate.Month == 1 || submittedDate.Month == 2 || submittedDate.Month == 3) //If submitted in January, Feb, Or March effective date is 4/1 of the same year
                return new DateTime(submittedDate.Year, 4, 1);
            else //If submitted anytime between April 1 and December 31, effective date is 4/1 of the following year
                return new DateTime(submittedDate.Year + 1, 4, 1);
        }

        private IList<string> GetCertNamesByCredentialExternalId(Guid memberId, IEnumerable<Guid> credentialIds)
        {

            var creds = SearchByMemberId(memberId).ToList();
            var certNames = new List<string>();

            foreach (var credGuid in credentialIds)
            {
                var certName = creds.Where(cred => cred.ExternalId == credGuid)?.Select(cert => cert.Certification.Name).FirstOrDefault();
                if (!string.IsNullOrEmpty(certName))
                    certNames.Add(certName);
            }

            return certNames;
        }
        #endregion Private Methods

        #region Disposal

        /// <summary>
        /// Disposes the child services.
        /// </summary>
        protected override void DisposeChildServices()
        {
            CertificationService.Dispose();
            SourceService.Dispose();
            HelperService.Dispose();
        }

        #endregion
    }
}
