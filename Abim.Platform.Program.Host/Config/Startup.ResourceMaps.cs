using Abim.Platform.Program.Host.ResourceMaps;
using AutoMapper;

namespace Abim.Platform.Program.Host.Config
{
    public partial class Startup
    {
        /// <summary>
        /// Uses the mappings.
        /// </summary>
        public static void UseMappings()
        {
            Mapper.Initialize(cfg =>
            {
                AddMappings(cfg);
            });
            
            #if DEBUG
            Mapper.AssertConfigurationIsValid();
            #endif
        }

        /// <summary>
        /// Adds the mappings.
        /// </summary>
        /// <param name="cfg">The CFG.</param>
        public static void AddMappings(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile<CertificationMapping>();
            cfg.AddProfile<CredentialMapping>();
            cfg.AddProfile<IssuanceMapping>();
            cfg.AddProfile<SourceMapping>();
            cfg.AddProfile<PhysicianCredentialsMapping>();
        }
    }
}