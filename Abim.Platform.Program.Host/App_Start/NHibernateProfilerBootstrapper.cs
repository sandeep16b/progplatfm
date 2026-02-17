using HibernatingRhinos.Profiler.Appender.NHibernate;

// If you're using .NET Core please remove this line and call NHibernateProfiler.Initialize(); on the very beginning of your application.
[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(Abim.Platform.Program.Host.App_Start.NHibernateProfilerBootstrapper), "PreStart")]
namespace Abim.Platform.Program.Host.App_Start
{
    /// <summary>
    /// NHibernateProfilerBootstrapper
    /// </summary>
	public static class NHibernateProfilerBootstrapper
	{
        /// <summary>
        /// PreStart
        /// </summary>
		public static void PreStart()
		{
            #if DEBUG
            // Initialize the profiler
            NHibernateProfiler.Initialize();
            #endif

            // You can also use the profiler in an offline manner.
            // This will generate a file with a snapshot of all the NHibernate activity in the application,
            // which you can use for later analysis by loading the file into the profiler.
            // var filename = @"c:\profiler-log";
            // NHibernateProfiler.InitializeOfflineProfiling(filename);

            // You can use the following for production profiling.
            // NHibernateProfiler.InitializeForProduction(11234, "A strong password like: ze38r/b2ulve2HLQB8NK5AYig");
        }
    }
}

