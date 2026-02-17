using Abim.Platform.Program.WebApi.Extensions;
using Metrics;
using NLog;
using System;
using System.Diagnostics;
using System.Reflection;

namespace Abim.Platform.Program.WebApi.Objects.Logging
{
    /// <summary>
    /// MethodTimeLogger static class for handling Fody MethodTimer performance.
    /// </summary>
    /// <remarks>
    /// Methods with the [Time] attribute will call this when the request is complete 
    /// with the System.Reflection.MethodBase object and the number of milliseconds it took to complete.
    /// </remarks>
    public static class MethodTimeLogger
    {
        /// <summary>
        /// Logs the specified method base.
        /// </summary>
        /// <param name="methodBase">The MethodBase object.</param>
        /// <param name="milliseconds">The number of milliseconds recorded.</param>
        public static void Log(MethodBase methodBase, long milliseconds, string appDisplayName, string appApiVersion)
        {
            var logger = LogManager.GetCurrentClassLogger();

            try
            {
                logger.Trace("MethodTimeLogger: Begin {0}", methodBase.Name);

                // Get the name of the API to which the method belongs
                var reflectedType = new StackTrace().GetFrame(1).GetMethod().ReflectedType;
                var apiName = (reflectedType != null)
                    ? reflectedType.Name
                    : "";

                // Add a new timer to Metrics .NET for this method
                var timer = Metric.Timer(string.Format("{0}/{1}", apiName, methodBase.Name), Unit.Requests, SamplingType.FavourRecent,
                    TimeUnit.Milliseconds);

                // Manually record the time provided by Fody MethodTimer to the Metrics .NET timer
                timer.Record(milliseconds, TimeUnit.Milliseconds);
                logger.Trace("MethodTimeLogger: End {0}", methodBase.Name);
            }
            catch (Exception ex)
            {
                var msg = string.Format("MethodTimeLogger: Log method failed for {0} with exception: {1}", methodBase.Name, ex.Message);
                logger.ErrorWithMetadata(msg, ex);
            }
        }
    }
}
