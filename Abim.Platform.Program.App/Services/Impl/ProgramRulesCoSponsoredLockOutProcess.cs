using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.App.Services.Commands;
using Abim.Platform.Program.App.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Abim.Platform.Program.App.Services.CommandResults;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.WebApi.Authentication;
using Abim.Enterprise.Core.Registration.Enums;
using Abim.Platform.Program.App.Extensions.Registration;

namespace Abim.Platform.Program.App.Services.Impl
{
    public partial class ProgramRulesService : IProgramRulesService, IDisposable
    {
        /// <summary>
        /// Runs the CoSponsored LockOut process
        /// </summary>
        /// <param name="credentialId">The GUID ID of the credential that CoSponsored LockOut is being run for</param>
        /// <param name="lockOutDate">The date for which CoSponsored LockOut is being run (for example, 12/31/2018)</param>
        /// <param name="processingDate">The date of the CoSponsored LockOut run</param>
        /// <returns></returns>
        public async Task<bool> RunCoSponsoredLockOut(Guid credentialId, DateTime lockOutDate, DateTime processingDate)
        {
            try
            {
                var credential = CredentialService.Load(credentialId);

                if (credential == null)
                {
                    Log.Info($"Credential not found with CredentialId {credentialId}.");
                    return await Task.FromResult<bool>(false);
                }

                ProcessingDate = processingDate;
                ExecutingProcess = ExecutingProcessType.CoSponsoredLockOut;
                MemberId = credential.MemberId;

                ClearGracePeriod(credential, lockOutDate);               // PBI 223713 : (Proj 1492) Program Rule 68 - Cosponsored Certificate Lock-out Exit
                SetLockOutPeriod(ref credential, lockOutDate, processingDate); // PBI 223711 (Proj 1492) Program Rule 66 - Cosponsored Certificate Lock-out Period

                // Record Lookback Date (lockOutDate)
                credential.LookbackDate = lockOutDate;

                CredentialService.Handle(new UpdateCredentialFromLookbackCommand
                {
                    Credential = credential,
                    ModifiedBy = $"CoSponsoredLockOut_{lockOutDate.ToString("yyyy-MM-dd")}"
                });

                //should run CA ?

            }
            catch (Exception ex)
            {
                Logger.Error(ex);
                throw;
            }

            return await Task.FromResult<bool>(true);
        }

        /// <summary>
        /// SetLockOutPeriod
        /// </summary>
        /// <param name="cred"></param>
        /// <param name="lockOutDate"></param>
        /// <param name="processingDate"></param>
        private void SetLockOutPeriod( ref Credential cred, DateTime lockOutDate, DateTime processingDate)
        {
            if (ShouldGoIntoLockOutPeriod(cred, lockOutDate))
            {
                cred.GracePeriodStartDate = new DateTime(lockOutDate.Year + 1, 1, 1);
                cred.GracePeriodEndDate = new DateTime(lockOutDate.Year + 1, 12, 31);
                Log.Info($"CoSponsored Credential {cred.ExternalId} for '{lockOutDate.ToString("yyyy-MM-dd")}', Lock Out Period was set.");
            }
        }

        /// <summary>
        /// ShouldGoIntoLockOutPeriod
        /// </summary>
        /// <param name="cred">The credential to set grace period on if applicable</param>
        /// <param name="lockOutDate">The most recent lookback date</param>
        /// <returns>true if the grace period was set, false otherwise</returns>
        private bool ShouldGoIntoLockOutPeriod(Credential cred, DateTime lockOutDate)
        {
            /* Pbi 223711 : (Proj 1492) Program Rule 66 - Cosponsored Certificate Lock-out Period ( https://tfs.abim.org/tfs/Abim/Enterprise/_workitems/edit/223711)
                The certificate is in the cosponsored lock-out period when:
                The diplomate is not already in the cosponsored lock-out period. OR
                The diplomate received a result of Fail, IND, INC, UTT on the long form MOC assessment in their due year.  OR
                The diplomate is enrolled in the Longitudinal Assessment 
                            AND is in their due year 
                            AND did not meet the annual LNG participation requirement 
                                OR the diplomate received a result of FAIL on the summative assessment.
            */

            if (cred.GracePeriodStartDate.HasValue)
            {
                Log.Info($"CoSponsored Credential {cred.ExternalId} for '{lockOutDate.ToString("yyyy-MM-dd")}' Is ALREADY in the cosponsored lock-out period.");
                return false;
            }

            if (!cred.ExamDueDate.HasValue || cred.ExamDueDate.Value.Year != lockOutDate.Year)
            {
                Log.Info($"CoSponsored Credential {cred.ExternalId} for '{lockOutDate.ToString("yyyy-MM-dd")}' Is NOT In Due Year or ExamDueDate is not set.");
                return false;
            }

            // The diplomate received a result of Fail, IND, INC, UTT on the long form MOC assessment in their due year.
            if (Registrations.Any(r => (r.ExamType != null && r.ExamType.ToEnum() == ExamType.Moc)
                && (r.CertificationId == cred.Certification.ExternalId)
                && (       r.Result == ExamResultType.Fail.ToString()
                        || r.Result == ExamResultType.Indeterminate.ToString()
                        || r.Result == ExamResultType.Incomplete.ToString()
                        || r.Result == ExamResultType.UnableToTest.ToString())
                && (r.AdministrationYear == cred.ExamDueDate.Value.Year)))
            {
                Log.Info($"CoSponsored Credential {cred.ExternalId} for '{lockOutDate.ToString("yyyy-MM-dd")}' received MOC exam result of Fail, IND, INC, UTT in due year");
                return true;
            }

            /*
            The diplomate is enrolled in the Longitudinal Assessment
                        AND is in their due year
                        AND did not meet the annual LNG participation requirement
                            OR the diplomate received a result of FAIL on the summative assessment.
            */
            // reuse existing function with Loging already in place
            if (CorrespondingLkaEnrollment(cred))
                return true;

            return false;
        }

       
    }
}
