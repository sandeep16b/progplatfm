using Abim.Platform.Program.App.Util;
using Abim.Platform.Program.Resources;

namespace Abim.Platform.Program.App.Classes
{
    /// <summary>
    /// A collection of constants that will be used in this project and any that reference it. 
    /// </summary>
    public static class Scopes
    {
        public static string legacyWebApiScope = "webapi";

        public static string readScope = $"{ProgramResourceConstants.AppInfo.BasicName.ToLower()}_read";

        public static string writeScope = $"{ProgramResourceConstants.AppInfo.BasicName.ToLower()}_write";

        public static string readwriteScope = $"{ProgramResourceConstants.AppInfo.BasicName.ToLower()}_read-write";

        public static string[] readScopes = new string[] { readScope, readwriteScope };

        public static string[] writeScopes = new string[] { writeScope, readwriteScope };

        public static string program_product_registration_profile_readwriteScopes = $"{GetPlatformReadWriteScope(PlatformName.Program)} {GetPlatformReadWriteScope(PlatformName.Product)} {GetPlatformReadWriteScope(PlatformName.Registration)} {GetPlatformReadWriteScope(PlatformName.Profile)}";

        public static string profile_readwriteScopes = $"{GetPlatformReadWriteScope(PlatformName.Profile)}";

        public static string profile_registration_readwriteScopes = $"{GetPlatformReadWriteScope(PlatformName.Profile)} {GetPlatformReadWriteScope(PlatformName.Registration)}";

        // below will be removed after removing webapi scope
        public static string webapi_program_product_registration_profile_readwriteScopes = $"{legacyWebApiScope} {GetPlatformReadWriteScope(PlatformName.Program)} {GetPlatformReadWriteScope(PlatformName.Product)} {GetPlatformReadWriteScope(PlatformName.Registration)} {GetPlatformReadWriteScope(PlatformName.Profile)}";

        public static string webapi_profile_readwriteScopes = $"{legacyWebApiScope} {GetPlatformReadWriteScope(PlatformName.Profile)}";

        public static string webapi_profile_registration_readwriteScopes = $"{legacyWebApiScope} {GetPlatformReadWriteScope(PlatformName.Profile)} {GetPlatformReadWriteScope(PlatformName.Registration)}";

        /// <summary>
        /// GetPlatformReadWriteScope
        /// </summary>
        /// <param name="platformName"></param>
        /// <param name="includeLegacyWebApiScope"></param>
        /// <returns></returns>
        public static string GetPlatformReadWriteScope (PlatformName platformName)
        {
          return $"{platformName.ToString().ToLower()}_read-write";
        }

        
        //was advised to hard code this value
        public static string program_product_registration_profile = "webapi program_read-write product_read-write registration_read-write profile_read-write";
    }
}