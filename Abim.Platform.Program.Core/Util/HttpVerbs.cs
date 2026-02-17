namespace Abim.Platform.Program.Util
{
    /// <summary>
    /// The HTTP Verbs to use when creating a <see cref="Link"/>.
    /// </summary>
    public enum HttpVerbs
    {
        /// <summary>
        /// A GET request
        /// </summary>
        Get,

        /// <summary>
        /// A POST request
        /// </summary>
        Post,

        /// <summary>
        /// A PUT request
        /// </summary>
        Put,

        /// <summary>
        /// A DELETE request
        /// </summary>
        Delete,

        /// <summary>
        /// An OPTIONS request
        /// </summary>
        Options,

        /// <summary>
        /// A PATCH request
        /// </summary>
        Patch
    }
}
