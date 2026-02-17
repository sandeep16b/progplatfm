using FeatureToggle.Toggles;

namespace Abim.Platform.Program.Host
{
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the RestApi
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_RestApi : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_RestApi"/> is disable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the Hangfire dashboard
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_HangfireDashboard : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_HangfireDashboard"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the Error page
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_ErrorPage : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_ErrorPage"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the Welcome page
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_WelcomePage : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_WelcomePage"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the service bus
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_MassTransit : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_MassTransit"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for all metrics
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_Metrics : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_Metrics"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the metrics console
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_MetricsConsole : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_MetricsConsole"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the metrics endpoint
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_MetricsEndpoint : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_MetricsEndpoint"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
    
    /// <summary>
    /// A feature-enabling and disabling class, using SimpleFeatureToggle, for the Redis cache
    /// </summary>
    /// <seealso cref="FeatureToggle.Toggles.SimpleFeatureToggle" />
    public class Feature_RedisCache : SimpleFeatureToggle
    {
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="Feature_RedisCache"/> is disabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if disable; otherwise, <c>false</c>.
        /// </value>
        public static bool Disable { get; set; }
        /// <summary>
        /// Gets a value indicating whether [feature enabled].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [feature enabled]; otherwise, <c>false</c>.
        /// </value>
        public new bool FeatureEnabled
        {
            get
            {
                if (Disable) return false;
                return base.FeatureEnabled;
            }
        }
    }
}
