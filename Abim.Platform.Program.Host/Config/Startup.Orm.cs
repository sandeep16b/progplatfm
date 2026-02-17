using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System.Diagnostics;

namespace Abim.Platform.Program.Host.Config
{
    /// <summary>
    /// StartUp Class.
    /// </summary>
    public partial class Startup
    {
        /// <summary>
        /// Gets or sets the session factory.
        /// </summary>
        /// <value>
        /// The session factory.
        /// </value>
        public static ISessionFactory SessionFactory { get; set; }

        /// <summary>
        /// CreateSessionFactory method.
        /// </summary>
        /// <returns></returns>
        public static ISessionFactory ConfigureOrm()
        {
            Logger.Trace("ConfigureOrm: Initializing NHibernate.");
            if (SessionFactory != null) return SessionFactory;
            
            bool useCache = (new Feature_RedisCache()).FeatureEnabled;
            if (useCache) UseOrmCache();
            
            var connStr = ConfigurationManager.ConnectionStrings["Certification"].ConnectionString;
            Debug.Assert(connStr != null);
            
            SessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(connStr)
                    .ShowSql())
                .ExposeConfiguration(c => c.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true"))
                //ar@7/27/2018 Commented out by PBI:129655:Disable NCache in Program Platform (2.7)
                //.Cache(m => m.ProviderClass("Alachisoft.NCache.Integrations.NHibernate.Cache.NCacheProvider , Alachisoft.NCache.Integrations.NHibernate.Cache").UseQueryCache().UseSecondLevelCache())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Certification>())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditData>())
                .Mappings(m => m.HbmMappings.AddFromAssemblyOf<ICertificationRepository>())
                .BuildSessionFactory();
            
            Logger.Trace("ConfigureOrm: Finished initializing NHibernate.");
            return SessionFactory;
        }
    }
}
