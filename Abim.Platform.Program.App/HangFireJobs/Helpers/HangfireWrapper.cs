using Hangfire;

namespace Abim.Platform.Program.App.HangFireJobs.Helpers
{
    /// <summary>
    /// A class that wraps Hangfire functionality, because despite IBackgroundJobClient, 
    /// Hangfire is still loaded with static classes and extension methods that can't be 
    /// mocked.
    /// </summary>
    public class HangfireWrapper : IHangfireWrapper
    {
        /// <summary>
        /// 
        /// </summary>
        public IBackgroundJobClient BackgroundJobClient => new BackgroundJobClient(JobStorage.Current);

      
    }
}
