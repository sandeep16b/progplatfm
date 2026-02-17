namespace Abim.Platform.Program.Host.Api
{
    /// <summary>
    /// A collection of constants that will be used in this project and any that reference it. 
    /// </summary>
    public static class ApiConstants
    {
        /// <summary>
        /// Cache Constants
        /// </summary>
        public static class Cache
        {
            /// <summary>
            /// The cache server time span
            /// </summary>
            public const int CacheServerTimeSpan = 3600;

            /// <summary>
            /// The cache client time span
            /// </summary>
            public const int CacheClientTimeSpan = 3600;
        }

        /// <summary>
        /// Hangfire Level Information Constants
        /// </summary>
        public static class HangfireInfo
        {
            /// <summary>
            /// 
            /// </summary>
            public static string ProgramQueueName = "program";
        }
    }
}