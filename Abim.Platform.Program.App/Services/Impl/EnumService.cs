using Abim.Platform.Program.WebApi.Objects;


namespace Abim.Platform.Program.App.Services.Impl
{
    /// <summary>
    /// The EnumService for this platform
    /// </summary>
    public class EnumService : EnumProvider
    {
    }
    
    /// <summary>
    /// static class EnumLinkSettings
    /// </summary>
    public static class EnumLinkSettings
    {
        /// <summary>
        /// Whether to include nested enum types in links
        /// </summary>
        public const bool IncludeNestedEnumTypesInLinks = true;
    }
}
