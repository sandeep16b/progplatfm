using Abim.Platform.Program.Relational;
using Abim.Platform.Program.Relational.Services;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.App.Services.Commands;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Abim.Platform.Program.App.DTOs;

namespace Abim.Platform.Program.App.Services
{
    /// <summary>
    /// ICredentialService interface.
    /// </summary>
    public interface ICredentialService :
        IService<Credential>,
        ICommandHandler<CreateCredentialCommand>,
        ICommandValidationHandler<CreateCredentialCommand>,

        ICommandHandler<CreateCredentialIssuanceCommand>,
        ICommandValidationHandler<CreateCredentialIssuanceCommand>,

        ICommandHandler<IssueFPHMCommand>,
        ICommandValidationHandler<IssueFPHMCommand>,
        ICommandHandler<ReissueCommand>,
        ICommandValidationHandler<ReissueCommand>,
        ICommandHandler<ExpireAndReissueCommand>,
        ICommandValidationHandler<ExpireAndReissueCommand>,
        ICommandHandler<ExpireIssuanceCommand>,
        ICommandValidationHandler<ExpireIssuanceCommand>,
        ICommandHandler<SetIssuanceToMaintainedCommand>,
        ICommandValidationHandler<SetIssuanceToMaintainedCommand>,

        ICommandHandler<UpdateCredentialGracePeriodGFCommand>,
        ICommandValidationHandler<UpdateCredentialGracePeriodGFCommand>,
        ICommandHandler<UpdateCredentialGracePeriodTLandMBMCommand>,
        ICommandValidationHandler<UpdateCredentialGracePeriodTLandMBMCommand>,

        ICommandHandler<UpdateCredentialOnExamResultCommand>,
        ICommandValidationHandler<UpdateCredentialOnExamResultCommand>,

        IAsyncCommandHandler<UpdateSelectedToMaintainCommand, UpdateSelectedToMaintainCommandResult>,
        ICommandValidationHandler<UpdateSelectedToMaintainCommand>,

        IAsyncCommandHandler<UpdatePathwayCommand, UpdatePathwayCommandResult>,
        ICommandValidationHandler<UpdatePathwayCommand>,

        ICommandHandler<UpdateGrandfatherMOCPrintDateCommand>,
        ICommandValidationHandler<UpdateGrandfatherMOCPrintDateCommand>,

        IAsyncCommandHandler<WithdrawCredentialCommand, WithdrawCredentialCommandResult>,
        ICommandValidationHandler<WithdrawCredentialCommand>,

        IAsyncCommandHandler<ReinstateCredentialCommand, ReinstateCredentialCommandResult>,
        ICommandValidationHandler<ReinstateCredentialCommand>,

        IAsyncCommandHandler<AddCredentialCommand, AddCredentialCommandResult>,
        ICommandValidationHandler<AddCredentialCommand>,

        IAsyncCommandHandler<UpdateCredentialCommand, UpdateCredentialCommandResult>,
        ICommandValidationHandler<UpdateCredentialCommand>,

        IAsyncCommandHandler<AddIssuanceCommand, AddIssuanceCommandResult>,
        ICommandValidationHandler<AddIssuanceCommand>,

        IAsyncCommandHandler<UpdateIssuanceCommand, UpdateIssuanceCommandResult>,
        ICommandValidationHandler<UpdateIssuanceCommand>,

        ICommandHandler<UpdateCredentialFromLookbackCommand>,
        ICommandValidationHandler<UpdateCredentialFromLookbackCommand>,

        ICommandHandler<UpdateCredentialFromObjectCommand>,
        ICommandValidationHandler<UpdateCredentialFromObjectCommand>,

        ICommandHandler<ReinstateTLCommand>,
        ICommandValidationHandler<ReinstateTLCommand>,

        IAsyncCommandHandler<EnrollInCMPCommand, EnrollInCMPCommandResult>,
        ICommandValidationHandler<EnrollInCMPCommand>,

        IAsyncCommandHandler<UnEnrollInCMPCommand, UnEnrollInCMPCommandResult>,
        ICommandValidationHandler<UnEnrollInCMPCommand>,

        ICommandHandler<MarkCertificateForDeselectCommand>,
        ICommandValidationHandler<MarkCertificateForDeselectCommand>,

        ICommandHandler<MarkCertificateForSelectCommand>,
        ICommandValidationHandler<MarkCertificateForSelectCommand>,

        ICommandHandler<MarkCertificatesForSelectOrDeselectCommand>,
        ICommandValidationHandler<MarkCertificatesForSelectOrDeselectCommand>, 

        ICommandHandler<DeselectCertificateCommand>, 
        ICommandValidationHandler<DeselectCertificateCommand>,


        //public ICommandResult Handle(UpdateLngAssessmentDueDateCommand command)
        IAsyncCommandHandler<UpdateLngAssessmentDueDateCommand, UpdateLngAssessmentDueDateCommandResult>,
        ICommandValidationHandler<UpdateLngAssessmentDueDateCommand>
    {
        /// <summary>
        /// Searches Credentials by a member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="paging"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<Credential> SearchByMemberId(Guid memberId, PageDefinition paging, out int totalCount);

        /// <summary>
        /// Searches Credentials by a member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IEnumerable<Credential> SearchByMemberId(Guid memberId);

        /// <summary>
        /// Searches Credentials by a member Id Async
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Task<IEnumerable<Credential>> SearchByMemberIdAsync(Guid memberId);

        /// <summary>
        /// Searches Certifications by a member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="paging"></param>
        /// <param name="totalCount"></param>
        /// <returns></returns>
        IEnumerable<Certification> SearchCertsByMemberId(Guid memberId, PageDefinition paging, out int totalCount);

        /// <summary>
        /// Searches Certifications by a member Id
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        IEnumerable<Certification> SearchCertsByMemberId(Guid memberId);

        /// <summary>
        /// Get IM Credential By Member
        /// </summary>
        /// <param name="memberId"></param>
        /// <returns></returns>
        Credential GetIMCredentialByMember(Guid memberId);

        /// <summary>
        /// Get Credential By Member And Code
        /// </summary>
        /// <param name="memberId"></param>
        /// <param name="Code"></param>
        /// <returns></returns>
        Credential GetCredentialByMemberAndCode(Guid memberId, string Code);

        /// <summary>
        /// Loads Issuances for a Credential
        /// </summary>
        /// <param name="id">The id of the credential</param>
        /// <returns></returns>
        IEnumerable<Issuance> LoadIssuancesForCredential(Guid id);
        
        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime checkDate);

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="abimCredentialsOnly">Optional parameter to specify ABIM-issued credentials only (default is false).</param>        
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime startDate, DateTime endDate, bool abimCredentialsOnly = false);

        /// <summary>
        /// Gets the Guids of all the expired credentials for a given user as of a given date, and for each one
        /// also the Id of the active about-to-expire credential (in Tuple pairings)
        /// </summary>
        /// <param name="checkDate">The check date.</param>
        /// <param name="memberId">The member Id.</param>
        /// <returns>
        /// Each tuple consists of a CredentialId and its IssuanceId
        /// </returns>
        IEnumerable<Tuple<Guid, int>> GetExpiredCredentials(DateTime checkDate,Guid memberId);

        /// <summary>
        /// Gets the issuances for member identifier.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        IEnumerable<Issuance> GetIssuancesForMemberId(Guid memberId);

        /// <summary>
        /// Firsts the issuance date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        DateTime? GetFirstIssuanceDate(Guid memberId);

        /// <summary>
        /// Gets the latest lookback date.
        /// </summary>
        /// <param name="memberId">The member identifier.</param>
        /// <returns></returns>
        DateTime? GetLatestLookbackDate(Guid memberId);

        /// <summary>
        /// Get NonAbim Issuance Count
        /// </summary>
        /// <returns></returns>
        int GetNonAbimIssuanceCount();

        /// <summary>
        /// Gets the IDs of Credentials marked for deselection
        /// </summary>
        /// <param name="deselectionEffectiveDate">The DeSelectionEffectiveDate to query by</param>
        /// <returns>An IEnumerable of Credential IDs</returns>
        IEnumerable<IGrouping<Guid, CredentialInfoDTO>> GetInfoOfCredentialsMarkedForDeselection(DateTime deselectionEffectiveDate);
    }
}
