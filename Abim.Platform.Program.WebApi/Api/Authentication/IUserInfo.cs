using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Abim.Platform.Program.WebApi.Authentication
{
    /// <summary>
    /// IUserInfo interface, for injection
    /// </summary>
    public interface IUserInfo
    {
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        string Name { get; set; }

        /// <summary>
        /// Username is the user's login username
        /// </summary>
        string Username { get; set; }

        /// <summary>
        /// The user's current token
        /// </summary>
        string Token { get; set; }

        /// <summary>
        /// The user's ProfileId guid
        /// </summary>
        Guid ProfileId { get; set; }

        /// <summary>
        /// The user's AbimId string (typically a six-digit integer)
        /// </summary>
        string AbimId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user is an admin.
        /// </summary>
        bool IsAdmin { get; set; }
        
        /// <summary>
        /// The user's claims
        /// </summary>
        /// <value>
        /// The claims.
        /// </value>
        List<Claim> Claims { get; set; }

        /// <summary>
        /// The user's current token, without the "Bearer " prefix
        /// </summary>
        string TokenWithoutBearer { get; }
    }
}
