namespace Abim.Platform.Program.Util
{
    /// <summary>
    /// An object containing the REST information required to create a link on the client.
    /// </summary>
    public class Link
    {
        #region Properties
        
        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        
        /// <summary>
        /// Gets the method.
        /// </summary>
        /// <value>
        /// The method.
        /// </value>
        public string Method { get; set; }
        
        /// <summary>
        /// Whether the link is enabled.
        /// </summary>
        /// <value>
        /// The boolean value.
        /// </value>
        public bool Enabled { get; set; }
        
        /// <summary>
        /// Gets the appropriate URL for the action.
        /// </summary>
        /// <value>
        /// The appropriate URL for the action.
        /// </value>
        public string Href { get; set; }
        
        #endregion
        
        #region Constructors
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// </summary>
        /// <remarks>
        /// This should only be used for deserialization purposes.
        /// </remarks>
        public Link()
        {
            Enabled = true;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// Will assign the HttpVerb to the name.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="href">The appropriate URL for the action.</param>
        public Link(HttpVerbs method, string href) : this(method.ToString(), method, href)
        {
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// Allows for custom naming of the link.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="method">The method.</param>
        /// <param name="href">The appropriate URL for the action.</param>
        /// 
        public Link(string name, HttpVerbs method, string href) : this()
        {
            Name   = name;
            Method = method.ToString().ToUpper();
            Href   = href;
        }
        
        /// <summary>
        /// Initializes a new instance of the <see cref="Link"/> class.
        /// Allows for custom naming of the link.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="method">The method.</param>
        /// <param name="href">The appropriate URL for the action.</param>
        /// 
        public Link(string name, string method, string href) : this()
        {
            Name   = name;
            Method = method;
            Href   = href;
        }
        
        #endregion
    }
}
