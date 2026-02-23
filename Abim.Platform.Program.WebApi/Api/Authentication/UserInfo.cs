using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using Thinktecture.IdentityModel.Owin.ResourceAuthorization;

namespace Abim.Platform.Program.WebApi.Authentication
{
    /// <summary>
    /// Represents a user's credentials and identification
    /// </summary>
    /// <seealso cref="Abim.Platform.Program.WebApi.Authentication.IUserInfo" />
    public class UserInfo : IUserInfo
    {
        #region Properties
        
        /// <summary>
        /// Name will consist of first name and last name, separated by a space
        /// </summary>
        public virtual string Name { get; set; }
        
        /// <summary>
        /// Username is the user's login username
        /// </summary>
        public virtual string Username { get; set; }

        /// <summary>
        /// The user's current token
        /// </summary>
        /// <remarks>
        /// This will typically already start with the substring "Bearer "
        /// </remarks>
        public virtual string Token { get; set; }

        /// <summary>
        /// The user's ProfileId guid
        /// </summary>
        public virtual Guid ProfileId { get; set; }

        /// <summary>
        /// The user's AbimId string (typically a six-digit integer)
        /// </summary>
        public virtual string AbimId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this user is an admin.
        /// </summary>
        public virtual bool IsAdmin { get; set; }

        /// <summary>
        /// User Claims
        /// </summary>
        public virtual List<Claim> Claims { get; set; } = new List<Claim>();

        /// <summary>
        /// The user's current token, without the "Bearer " prefix
        /// </summary>
        public virtual string TokenWithoutBearer
        {
            get
            {
                if(Token == null) return null;
                return Token.Split(' ').Last();
            }
        }

        #endregion

        /// <summary>
        /// The default username
        /// </summary>
        public const string DefaultUsername = null;

        /// <summary>
        /// The default name
        /// </summary>
        public const string DefaultName = "No Name";


        /// <summary>
        /// Reads the profile identifier from a collection of claims.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public static Guid ReadProfileIdFromClaims(IEnumerable<Claim> claims)
        {
            if(claims == null) return Guid.Empty;
            Claim userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            if(userIdClaim != null && !string.IsNullOrEmpty(userIdClaim.Value))
            {
                Guid userId;
                if(Guid.TryParse(userIdClaim.Value, out userId)) return userId;
            }
            return Guid.Empty;
        }

        /// <summary>
        /// Reads the abim identifier from a collection of claims.
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <returns></returns>
        public static string ReadAbimIdFromClaims(IEnumerable<Claim> claims)
        {
            if(claims == null) return null;
            var claimType = ConfigurationManager.AppSettings["ClaimType.AbimId"]
                ?? "http://schemas.abim.org/2016/identifier/abim";
            var claim = claims.FirstOrDefault(x =>
                x.Type.Equals(claimType, StringComparison.CurrentCultureIgnoreCase));
            
            return claim != null ? claim.Value : null;
        }

        /// <summary>
        /// Creates a UserInfo object from a ResourceAuthorizationContext.
        /// </summary>
        /// <param name="authContext">The authentication context.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        public static UserInfo Create(ResourceAuthorizationContext authContext, string token = null)
        {
            if(!authContext.Principal.Identity.IsAuthenticated) return null;
            return CreateFrom(authContext.Principal.Claims, authContext.Principal.Identity, token);
        }

        /// <summary>
        /// Creates a UserInfo object from an HttpRequestMessage.
        /// </summary>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        public static UserInfo Create(HttpRequestMessage request)
        {
            if(!request.Properties.Any(p => p.Key == "MS_OwinContext")) return null;
            var owinContextProperty = request.Properties.First(p => p.Key == "MS_OwinContext");
            var owinContext = (IOwinContext)(owinContextProperty.Value);
            if(owinContext.Authentication == null
                || owinContext.Authentication.User == null
                || owinContext.Authentication.User.Identity == null) return null;
            var user = owinContext.Authentication.User;
            return CreateFrom(user.Claims, user.Identity, ControllerBase.GetToken(request));
        }

        /// <summary>
        /// The protected CreateFrom method which is called by both Create() overloads
        /// </summary>
        /// <param name="claims">The claims.</param>
        /// <param name="identity">The identity.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        protected static UserInfo CreateFrom(IEnumerable<Claim> claims, IIdentity identity, string token = null)
        {
            UserInfo userInfo = new UserInfo();
            
            //ProfileId
            userInfo.ProfileId = ReadProfileIdFromClaims(claims);
            
            //AbimId
            userInfo.AbimId = ReadAbimIdFromClaims(claims);
            
            //Username
            Claim preferredUsernameClaim = claims.FirstOrDefault(x => x.Type == "preferred_username");
            if(preferredUsernameClaim != null && !string.IsNullOrEmpty(preferredUsernameClaim.Value))
                userInfo.Username = preferredUsernameClaim.Value;
            else userInfo.Username = DefaultUsername;
            
            //Name
            userInfo.Name = string.IsNullOrEmpty(identity.Name) ? DefaultName : identity.Name;
            
            //Token
            if(token != null) userInfo.Token = token;
            else
            {
                var tokenClaim = claims.FirstOrDefault(c => c.Type == "token");
                if(tokenClaim != null) userInfo.Token = tokenClaim.Value;
            }

            //IsAdmin
            var adminRoleNames = (ConfigurationManager.AppSettings["AdminGroup"] ?? "DevAdmin").Split(',');
            var roleClaims = claims.Where(x => x.Type == ClaimTypes.Role);
            var backgroundClientId = ConfigurationManager.AppSettings["backgroundClientId"] ?? "";
            userInfo.IsAdmin = roleClaims.Any(c => adminRoleNames.Contains(c.Value)) 
                || claims.Any(x => x.Type.Contains("client_id") && x.Value == backgroundClientId.Trim());
            
            //Claims
            userInfo.Claims = claims.ToList();
            
            return userInfo;
        }
    }
}
