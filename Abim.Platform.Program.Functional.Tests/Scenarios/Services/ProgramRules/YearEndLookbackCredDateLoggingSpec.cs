using TestStack.BDDfy;
using NUnit.Framework;

namespace Abim.Platform.Program.Tests.Scenarios.Services.ProgramRules
{
    [Story(
        AsA = "controller, background job, or bus consumer",
        IWant = "to be able to utilize the Program Rules service",
        SoThat = "it can run the year end lookback process"
    )]
    [TestFixture]
    public class YearEndLookbackCredDateLoggingSpec
    {
        public void Should_Log_When_AssessmentMetDate_Changes()
        {
        }

        public void Should_Log_When_ExamDueDate_Changes()
        {
        }

        public void Should_Log_When_DisplayExamDueDate_Changes()
        {
        }

        public void Should_Log_When_KCIExamDueDate_Changes()
        {
        }

        public void Should_Log_When_MOCExamDueDate_Changes()
        {
        }

        public void Should_Log_When_GracePeriodStartDate_Changes()
        {
        }

        public void Should_Log_When_GracePeriodEndDate_Changes()
        {
        }
    }
}
