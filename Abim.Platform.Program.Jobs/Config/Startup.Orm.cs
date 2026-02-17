using Abim.Platform.Program.App.Data;
using Abim.Platform.Program.App.Domain;
using Abim.Platform.Program.Relational.Domain.Types;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using System.Diagnostics;

namespace Abim.Platform.Program.Jobs.Config
{
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

            var connStr = ConfigurationManager.ConnectionStrings["Certification"].ConnectionString;
            Debug.Assert(connStr != null);

            SessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012
                    .ConnectionString(connStr)
                    .ShowSql())
                .ExposeConfiguration(c => c.SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true"))

                //TODO: Add caching

                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Certification>())
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<AuditData>())
                .Mappings(m => m.HbmMappings.AddFromAssemblyOf<ICertificationRepository>())
                .BuildSessionFactory();

            Logger.Trace("ConfigureOrm: Finished initializing NHibernate.");
            return SessionFactory;
        }
    }
}
