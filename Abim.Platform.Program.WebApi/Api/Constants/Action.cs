using Abim.Platform.Program.Core.Api.Attributes;

namespace Abim.Platform.Program.WebApi.Attributes
{
    /// <summary>
    /// Actions enumeration
    /// </summary>
    public static class Actions 
    {
        #region View
        
        /// <summary>
        /// View a particular resource
        /// </summary>
        public const string View = "View";
        
        /// <summary>
        /// View a particular resource list
        /// </summary>
        public const string ViewAll = "ViewAll";
        
        /// <summary>
        /// Same as View, however also clarifying that the user must be an admin
        /// </summary>
        public const string ViewAdmin = "ViewAdmin";
        
        /// <summary>
        /// Same as ViewAll, however also clarifying that the user must be an admin
        /// </summary>
        public const string ViewAllAdmin = "ViewAllAdmin";
        
        #endregion
        
        #region Non-View
        
        /// <summary>
        /// Create
        /// </summary>
        public const string Create = "Create";
        
        /// <summary>
        /// Update
        /// </summary>
        public const string Update = "Update";
        
        /// <summary>
        /// Delete
        /// </summary>
        public const string Delete = "Delete";
        
        /// <summary>
        /// Uncache
        /// </summary>
        public const string Uncache = "Uncache";
        
        #endregion
        
        /// <summary>
        /// Determines whether [is view type] [the specified action name].
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>
        ///   <c>true</c> if [is view type] [the specified action name]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[]{ "ViewAdmin" }, ShouldReturn = true)]
        public static bool IsViewType(string actionName)
        {
            return actionName.ToLower().StartsWith("view");
        }

        /// <summary>
        /// Determines whether [is admin type] [the specified action name].
        /// </summary>
        /// <param name="actionName">Name of the action.</param>
        /// <returns>
        ///   <c>true</c> if [is admin type] [the specified action name]; otherwise, <c>false</c>.
        /// </returns>
        [ExpectedReturnExample(Arguments = new object[] { "ViewAdmin" }, ShouldReturn = true)]
        public static bool IsAdminType(string actionName)
        {
            return actionName.ToLower().Contains("admin");
        }
    }
}
