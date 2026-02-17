using Hangfire;
using Hangfire.States;
using System;

namespace Abim.Platform.Program.App.Data.Impl
{
    /// <remarks>
    /// We need this as the default IBackgroundJobClient instance, for IoC
    /// </remarks>
    /// <seealso cref="Hangfire.IBackgroundJobClient" />
    public class BackgroundJobClient : IBackgroundJobClient
    {
        /// <summary>
        /// 
        /// </summary>
        public bool ChangeState(string jobId, IState state, string expectedState)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public string Create(Hangfire.Common.Job job, IState state)
        {
            throw new NotImplementedException();
        }
    }
}
