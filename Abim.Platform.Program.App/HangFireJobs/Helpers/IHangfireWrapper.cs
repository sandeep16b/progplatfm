using Hangfire;

namespace Abim.Platform.Program.App.HangFireJobs.Helpers
{
    /// <summary>
    /// An interface for us in wrapping/mocking Hangfire functionality
    /// </summary>
    public interface IHangfireWrapper
    {
        /// <summary>
        /// BackgroundJobClient
        /// </summary>
        IBackgroundJobClient BackgroundJobClient { get; }


    }
}
