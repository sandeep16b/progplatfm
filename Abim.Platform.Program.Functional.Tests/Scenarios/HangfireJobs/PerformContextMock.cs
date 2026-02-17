using System;
using Hangfire;
using Hangfire.Server;
using Hangfire.Storage;
using Moq;

namespace Abim.Platform.Program.Tests.Scenarios.HangfireJobs
{
    //https://github.com/HangfireIO/Hangfire/blob/master/tests/Hangfire.Core.Tests/Mocks/PerformContextMock.cs

    class PerformContextMock
    {
        private readonly Lazy<PerformContext> _context;

        public PerformContextMock()
        {
            Connection = new Mock<IStorageConnection>();
            BackgroundJob = new BackgroundJobMock();
            CancellationToken = new Mock<IJobCancellationToken>();

            _context = new Lazy<PerformContext>(
                () => new PerformContext(Connection.Object, BackgroundJob.Object, CancellationToken.Object));
        }

        public Mock<IStorageConnection> Connection { get; set; }
        public BackgroundJobMock BackgroundJob { get; set; }
        public Mock<IJobCancellationToken> CancellationToken { get; set; }

        public PerformContext Object => _context.Value;

        public static void SomeMethod()
        {
        }
    }
}
