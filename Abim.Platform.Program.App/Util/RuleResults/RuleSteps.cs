using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Resources;
using Abim.Platform.Program.Util.Extensions;
using System;

namespace Abim.Platform.Program.App.Util
{
#pragma warning disable
    #region Interface 
    public interface IStep
    {
        bool MeetStepRule { get; set; }

        void MapStep(ref CorrectiveActionResult resul);
    }
    #endregion

    #region IStep classes
    public class FiveYearLookBackStep : IStep
    {
        public bool MeetStepRule { get; set; }
        public decimal? TotalMOCPoints { get; set; }
        public decimal? MKPoints { get; set; }

        public Tuple<DateTime, DateTime> FiveYearLookBackDates { get; set; }

        public DateTime? EvaluationDate { get; set; }

        public bool? Reciprocity { get; set; }
        //public string ReciprocityTimeRange { get; set; }

        public bool? RecentlyInitiallyCertified { get; set; }
        //public string RecentlyInitiallyCertifiedTimeRange { get; set; }

        public bool? NewSubspecialtyInitialCert { get; set; }

        public void MapStep(ref CorrectiveActionResult result)
        {
            result.FiveYearLookBack = MeetStepRule;
            result.FiveYearLookBackRange = string.Format($"{FiveYearLookBackDates?.Item1.ToShortDateString()}-{FiveYearLookBackDates?.Item2.ToShortDateString()}");
            result.TotalMOCPoints = TotalMOCPoints;
            result.MedicalKnowledgePoints = MKPoints;
            result.Reciprocity = Reciprocity;
            if (RecentlyInitiallyCertified.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" Points_RecentlyInitiallyCertified:{RecentlyInitiallyCertified.Value.ToYesNo()}";
            if (NewSubspecialtyInitialCert.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" Points_NewSubspecialtyInitialCert:{NewSubspecialtyInitialCert.Value.ToYesNo()}";
            // only if whole rule is met set issuance date to avoid confusion
            if (result.MeetRule)
                result.IssuanceDate = EvaluationDate;
        }
        //public string NewSubspecialtyInitialCertTimeRange { get; set; }
    }

    public class AttestationStep : IStep
    {
        public bool MeetStepRule { get; set; }

        public bool? Attestation { get; set; }
        //public Tuple<DateTime, DateTime> AttestationTimeRange { get; set; }

        public DateTime? ReattestationDueDate { get; set; }

        public void MapStep(ref CorrectiveActionResult result)
        {
            result.Attestation = Attestation;
            result.ReattestationDueDate = ReattestationDueDate;
            //result.AttestationRange = $"{AttestationTimeRange?.Item1.ToShortDateString()}-{AttestationTimeRange?.Item2.ToShortDateString()}";
        }
    }

    public class ExamRequirementStep : IStep
    {
        public bool MeetStepRule { get; set; }

        public bool? ExamAssessmentMet { get; set; }

        public bool? PassMOCExam { get; set; }
        public bool? PassKCIExam { get; set; }
        public bool? PassCMPExam { get; set; }

        public bool? MetParticipationInTheFirstYear { get; set; }

        public bool? PassExamWith2YearsOfFailed { get; set; }

        public bool? ExamPassAfterNoconcequences { get; set; }

        public bool? InGracePeriod { get; set; }

        public bool? PendingExamResultsExist { get; set; }

        public Tuple<DateTime, DateTime> MOCExamTimeRange { get; set; }
        public Tuple<DateTime, DateTime> KCIExamTimeRange { get; set; }
        public Tuple<DateTime, DateTime> CMPExamTimeRange { get; set; }

        public void MapStep(ref CorrectiveActionResult result)
        {
            result.ExamRequirement = this.MeetStepRule;
            result.ExamAssessmentMet = this.ExamAssessmentMet;
            result.PassMOCExam = this.PassMOCExam;
            result.PassKCIExam = this.PassKCIExam;
            result.PassCMPExam = this.PassCMPExam;

            result.MOCExamTimeRange = $"{MOCExamTimeRange?.Item1.ToShortDateString()}-{MOCExamTimeRange?.Item2.ToShortDateString()}";
            result.KCIExamTimeRange = $"{KCIExamTimeRange?.Item1.ToShortDateString()}-{KCIExamTimeRange?.Item2.ToShortDateString()}";
            result.CMPExamTimeRange = $"{CMPExamTimeRange?.Item1.ToShortDateString()}-{CMPExamTimeRange?.Item2.ToShortDateString()}";
            if (PassExamWith2YearsOfFailed.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" Exam_PassExamWith2YearsOfFailed:{PassExamWith2YearsOfFailed.Value.ToYesNo()}";

            if (ExamPassAfterNoconcequences.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" Exam_ExamPassAfterNoconcequences:{ExamPassAfterNoconcequences.Value.ToYesNo()}";

            if (InGracePeriod.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" InGracePeriod:{InGracePeriod.Value.ToYesNo()}";

            if (PendingExamResultsExist.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" PendingExamResultsExist:{this.PendingExamResultsExist.Value.ToYesNo()}";

            if (MetParticipationInTheFirstYear.HasValue) 
                result.AdditionalResults += result.AdditionalResults + $" MetParticipationInTheFirstYear:{this.MetParticipationInTheFirstYear.Value.ToYesNo()}";

        }
    }

    public class FailedOrPendingExamInIssuanceExpiredYearStep : IStep
    {
        public bool MeetStepRule { get; set; }
        public void MapStep(ref CorrectiveActionResult result)
        {
            result.AdditionalResults += result.AdditionalResults + $" FailedOrPendingExamInIssuanceExpiredYearStep:{MeetStepRule}";
        }
    }

    public class MaintenanceStatusStep : IStep
    {
        public bool MeetStepRule { get; set; }
        public bool MeetMaintenanceStatus { get; set; }
        public decimal? MaintenanceAnyMOCPoints { get; set; }
        public bool? Reciprocity { get; set; }
        public bool? RecentlyInitiallyCertified { get; set; }
        public Tuple<DateTime, DateTime> MaintenanceTwoYearLookBackDates { get; set; }
        public CredentialCategoryType? CredentialCategory { get; set; }

        public void MapStep(ref CorrectiveActionResult result)
        {

            result.MaintenanceAnyMOCPoints = MaintenanceAnyMOCPoints;
            result.MaintenanceTwoYearLookBackRange = $"{MaintenanceTwoYearLookBackDates?.Item1.ToShortDateString()}-{MaintenanceTwoYearLookBackDates?.Item2.ToShortDateString()}";
            if (Reciprocity.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" Maintenance_Reciprocity:{Reciprocity.Value.ToYesNo()}";
            if (RecentlyInitiallyCertified.HasValue)
                result.AdditionalResults += result.AdditionalResults + $" Maintenance_RecentlyInitiallyCertified:{RecentlyInitiallyCertified.Value.ToYesNo()}";

            result.MaintenanceStatus = MeetMaintenanceStatus ? MaintenanceStatusType.Maintained : MaintenanceStatusType.NotMaintained;
        }
    }
    #endregion
#pragma warning restore
}

