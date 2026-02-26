using Hangfire.Common;
using Hangfire.Server;
using NLog;
using System;

namespace Abim.Platform.Program.App.HangFireJobs.Filters
{
    //From https://discuss.hangfire.io/t/job-reentrancy-avoidance-proposal/607/8

    /// <summary>
    /// Attribute to skip a job execution if the same job is already running.
    /// Mostly taken from: http://discuss.hangfire.io/t/job-reentrancy-avoidance-proposal/607
    /// </summary>
    public class SkipConcurrentExecutionAttribute : JobFilterAttribute, IServerFilter
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        private readonly int _timeoutInSeconds;

        /// <summary>
        /// SkipConcurrentExecutionAttribute
        /// </summary>
        /// <param name="timeoutInSeconds"></param>
        public SkipConcurrentExecutionAttribute(int timeoutInSeconds)
        {
            if (timeoutInSeconds < 0) throw new ArgumentException("Timeout argument value should be greater that zero.");

            _timeoutInSeconds = timeoutInSeconds;
        }

        /// <summary>
        /// OnPerforming
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnPerforming(PerformingContext filterContext)
        {
            // filterContext.Job is obsolete
            var resource = String.Format(
                                 "{0}.{1}",
                                filterContext.BackgroundJob.Job.Type.FullName,
                                filterContext.BackgroundJob.Job.Method.Name);

            var timeout = TimeSpan.FromSeconds(_timeoutInSeconds);

            try
            {
                var distributedLock = filterContext.Connection.AcquireDistributedLock(resource, timeout);
                filterContext.Items["DistributedLock"] = distributedLock;
            }
            catch (Exception)
            {
                filterContext.Canceled = true;
                logger.Warn("Cancelling run for {0} job, id: {1} ", resource, filterContext.BackgroundJob.Id);
            }
        }

        /// <summary>
        /// OnPerformed
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnPerformed(PerformedContext filterContext)
        {
            if (!filterContext.Items.ContainsKey("DistributedLock"))
            {
                throw new InvalidOperationException("Can not release a distributed lock: it was not acquired.");
            }

            var distributedLock = (IDisposable)filterContext.Items["DistributedLock"];
            distributedLock.Dispose();
        }
    }
}
