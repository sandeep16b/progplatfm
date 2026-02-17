using Abim.Platform.Program.App.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Abim.Platform.Program.App.Util
{
    internal static class RuleResultExtensions
    {
        public static IEnumerable<RuleResults> UpdateResult(this IEnumerable<RuleResults> ruleResults, Action<IEnumerable<CredentialToUpdate>, DateTime> action, DateTime processingDate)
        {
            var credentialsToUpdate = ruleResults.Where(res => res.CorrectiveActionResult.MeetRule)
                                                 .Select(res => new CredentialToUpdate(res.CorrectiveActionResult.CredentialId,
                                                        res.CorrectiveActionResult.MaintenanceStatus.Value,
                                                        res.CorrectiveActionResult.IssuanceDate.Value,
                                                        res.CorrectiveActionResult.CredentialCategory.Value,
                                                        processingDate)).ToList();
            if (credentialsToUpdate.Any())
                action(credentialsToUpdate, processingDate);

            return ruleResults;

        }

        public static IEnumerable<RuleResults> UpdateResult(this IEnumerable<RuleResults> ruleResults, Action<IEnumerable<CredentialToUpdate>> action)
        {
            var credentialsToUpdate = ruleResults.Where(res => res.CorrectiveActionResult.MeetRule)
                                                 .Select(res => new CredentialToUpdate(res.CorrectiveActionResult.CredentialId,
                                                        res.CorrectiveActionResult.MaintenanceStatus,
                                                        res.CorrectiveActionResult.IssuanceDate,
                                                        res.CorrectiveActionResult.CredentialCategory)).ToList();
            if (credentialsToUpdate.Any())
                action(credentialsToUpdate);

            return ruleResults;

        }

        public static IEnumerable<RuleResults> UpdateResult(this IEnumerable<RuleResults> ruleResults, Action<IEnumerable<Guid>> action)
        {
            var credentialsToUpdate = ruleResults.Where(res => res.CorrectiveActionResult.MeetRule)
                                                 .Select(res => res.CorrectiveActionResult.CredentialId).ToList();

            if (credentialsToUpdate.Any())
                action(credentialsToUpdate);

            return ruleResults;
        }

        public static RuleResults UpdateResult(this RuleResults ruleResult, Action<CredentialToUpdate> action)
        {

            if (ruleResult.CorrectiveActionResult.MeetRule)
                action(new CredentialToUpdate(ruleResult.CorrectiveActionResult.CredentialId,
                                              ruleResult.CorrectiveActionResult.MaintenanceStatus,
                                              ruleResult.CorrectiveActionResult.IssuanceDate,
                                              ruleResult.CorrectiveActionResult.CredentialCategory));

            return ruleResult;
        }

        public static Task LogCorrectiveActionResult(this IEnumerable<RuleResults> ruleResults, Func<IEnumerable<CorrectiveActionResult>, Task> action)
        {

            var correctiveActionResults = ruleResults.Select(p => p.CorrectiveActionResult);

            return Task.Run(async () => await action(correctiveActionResults));

        }

        public static Task LogCorrectiveActionResult(this RuleResults ruleResult, Func<CorrectiveActionResult, Task> action)
        {
            return Task.Run(async () => await action(ruleResult.CorrectiveActionResult));
        }

    }
}
