namespace Abim.Platform.Program.App.Classes
{
    /// <summary>
    /// A collection of constants that will be used in this project and any that reference it. 
    /// </summary>
    public static class Scopes
    {
        /// <summary>
        /// readScope
        /// </summary>
        public static string readScope = $"c.r";
        /// <summary>
        /// writeScope
        /// </summary>
        public static string writeScope = $"c.w";
        /// <summary>
        /// readwriteScope
        /// </summary>
        public static string readwriteScope = $"c.r c.w";
        /// <summary>
        /// readScopes
        /// </summary>
        public static string[] readScopes = new string[] { readScope, readwriteScope };
        /// <summary>
        /// writeScopes
        /// </summary>
        public static string[] writeScopes = new string[] { writeScope, readwriteScope };

    }
}