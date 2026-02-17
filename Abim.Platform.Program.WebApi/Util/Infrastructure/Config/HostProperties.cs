namespace Abim.Platform.Program.WebApi.Config
{
    /// <summary>
    /// HostProperties stores the status of the Host for a current platform
    /// </summary>
    public static class HostProperties
    {
        /// <summary>
        /// The default status
        /// </summary>
        public const string DefaultStatus = "UNKNOWN";

        /// <summary>
        /// The status backing field
        /// </summary>
        private static string _status = DefaultStatus;

        /// <summary>
        /// Gets or sets the status.
        /// </summary>
        /// <value>
        /// The status.
        /// </value>
        public static string Status
        {
            get
            {
                return _status;
            }
            set
            {
                if(string.IsNullOrEmpty(value)) _status = DefaultStatus;
                else _status = value;
            }
        }
    }
}
