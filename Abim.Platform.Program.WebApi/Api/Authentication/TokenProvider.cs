using Microsoft.Owin.Security.OAuth;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Abim.Platform.Program.WebApi.Objects.Security
{
    /// <summary>
    /// A class to add additional claims to the Identity.
    /// </summary>
    public class TokenProvider : OAuthBearerAuthenticationProvider
    {
        /// <summary>
        /// Handles validating the identity produced from an OAuth bearer token.
        /// </summary>
        /// <param name="context">The identity context.</param>
        /// <returns>The task for asynchronous operations.</returns>
        public override Task ValidateIdentity(OAuthValidateIdentityContext context)
        {
            string token = null;
            if(context != null && context.Request != null && context.Request.Headers != null)
            {
                var authHeaders = context.Request.Headers.Where(hdr => hdr.Key == "Authorization").ToList();
                if(authHeaders.Any()) token = authHeaders.First().Value.First();
            }
            context.Ticket.Identity.AddClaim(new Claim("token", token));
            
            // Get Claims to find out if user actually is permitted to actual api
            // Then you can add an attribute over the api method to define what role
            // the user needs to that funtionality
            return base.ValidateIdentity(context);
        }
    }
}
