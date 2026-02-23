using Abim.Enterprise.Core.Registration.Enums;

namespace Abim.Platform.Program.App.Util
{
#pragma warning disable
    /// <summary>
    /// A collection of constants that will be used in this project and any that reference it.
    /// </summary>
    public static class Constants
    {
        /// <summary>
        /// 
        /// </summary>
        public static class Database
        {
            /// <summary>
            /// The database schema
            /// </summary>
            public const string Schema                              = "dbo";

            public const string LookbackLogTableName                = "LookbackLog";
            public const string CredentialDateLogTableName          = "CredentialDateLog";
            public const string CertificationTableName              = "Certification";
            public const string CredentialTableName                 = "Credential";
            public const string SourceTableName                     = "Source";
            public const string IssuanceTableName                   = "Issuance";
            public const string CorrectiveActionRunTableName = "CorrectiveActionResult";
            public const string LookBackDatesInfoTableName = "LookBackDateInfo";
            public const string LookbackDateLogTableName = "LookbackDateLog";

            public class IssuanceTable                             
            {
                public const string CredentialId                    = "CredentialId";
                public const string DurationType                    = "DurationTypeValue";
                public const string MaintenanceRequirementType      = "MaintenanceRequirementTypeValue";
                public const string MaintenanceStatusType           = "MaintenanceStatusTypeValue";
                public const string OccurrenceType                  = "OccurrenceTypeValue";
                public const string IssuanceStatusType              = "IssuanceStatusTypeValue";
                public const string IssuanceDate                    = "IssuanceDate";
                public const string ExpirationDate                  = "ExpirationDate";
                public const string ExpiredDate                     = "ExpiredDate";
                public const string ScheduledUpdate                 = "ScheduledUpdate";
                public const string UnderReview                     = "UnderReview";
                public const string DeselectionSubmittedDate        = "DeselectionSubmittedDate";
                public const string DeselectionEffectiveDate        = "DeselectionEffectiveDate";
                public const string DeSelectionProcessedDate        = "DeSelectionProcessedDate";
            }
        }

        #region Hangfire Queue Names

        /// <summary>
        /// Hangfire constants
        /// </summary>
        public static class HangfireInfo
        {
            public const string ProgramQueueName = "program";

        }

        #endregion

        public static class RulesStepName
        {
            public const string FiveYearLookBackStep = "FiveYearLookBackStep";
            public const string FiveYearLookBackFPHMStep = "FiveYearLookBackFPHMStep";
            public const string FiveYearLookBackGFPrintingStep = "FiveYearLookBackGFPrintingStep";

            public const string DetermineMaintenanceStatusStep = "[P031][P032]DetermineMaintenanceStatusStep";

            public const string ExamRequirementStep = "ExamRequirementStep";
            public const string ExamRequirementFPHMStep = "ExamRequirementFPHMStep";

        }

        public struct TriggeredCommunication
        {
            public const string Empty = "Empty";
            public const string EarnedMBMCertLetter = "EarnedMBMCertLetter";
        }

        public static class TriggeredCommunicationTemplateExternalKey
        {
            public const string EarnedMBMCertLetter = "ts_earned_MBM_letter";
            public const string DeactivateCertification = "43169";
            public const string Reactivate_Certification = "43168";

        }

        public const string StopFurtherExecutionMessage = "Stop further execution";

        public static class ExamResultConstants
        {
            public static ExamResultType[] ExamResultFailIndIncUtt = new ExamResultType[] {
                                                                        ExamResultType.Fail,
                                                                        ExamResultType.Indeterminate,
                                                                        ExamResultType.Incomplete,
                                                                        ExamResultType.UnableToTest};

            public static ExamResultType[] ExamResultPassFailIndIncUtt = new ExamResultType[] {
                                                                        ExamResultType.Pass,
                                                                        ExamResultType.Fail,
                                                                        ExamResultType.Indeterminate,
                                                                        ExamResultType.Incomplete,
                                                                        ExamResultType.UnableToTest};

            public static ExamResultType[] ExamResultPassFailIndInvIncUtt = new ExamResultType[] {
                                                                        ExamResultType.Pass,
                                                                        ExamResultType.Fail,
                                                                        ExamResultType.Indeterminate,
                                                                        ExamResultType.Invalidated,
                                                                        ExamResultType.Incomplete,
                                                                        ExamResultType.UnableToTest};

            public static ExamResultType[] ExamResultIndIncUtt = new ExamResultType[] {
                                                                        ExamResultType.Indeterminate,
                                                                        ExamResultType.Incomplete,
                                                                        ExamResultType.UnableToTest};
        }
    }
#pragma warning restore
}
