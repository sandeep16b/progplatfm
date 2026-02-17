using Abim.Platform.Program.WebApi.Authentication;
using System.Linq;

namespace Abim.Platform.Program.Host.Util
{
    /// <summary>
    /// 
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// If Username is null then get it from "client_id" and if that empty then get it from defaultNameToSet 
        /// </summary>
        /// <param name="UserProfile"></param>
        /// <param name="defaultNameToSet"></param>
        public static UserInfo SetUserNameInProfile (UserInfo UserProfile, string defaultNameToSet)
        {
            if (string.IsNullOrEmpty(UserProfile.Username))
            {
                var backgroundClient = UserProfile.Claims.Where(c => c.Type == "client_id").Select(v => v.Value).FirstOrDefault();

                UserProfile.Username = backgroundClient == null ? defaultNameToSet : backgroundClient;
            }
            return UserProfile;
        }
    }
}
