using NLog;
using System;

namespace Abim.Platform.Program.Core.Identity
{
    public class AccessTokenService : IAccessTokenService
    {
        private static string _accessToken { get; set; } = "";

        private static DateTime expirationTokenTime { get; set; } = DateTime.UtcNow; // UtcNow is NOT adjusted for seasonal changes like DST and it is faster ...

        private ITokenClientWraper _tokenClientWraper;

        private int timeInSecondsBeforeExpirationToGetNewToken = 60;

        ///private static string scope = "webapi program_read-write product_read-write registration_read-write profile_read-write";

        private static string scope = "a.r a.w ata.r ata.w b.r b.w c.r c.w f.r f.w n.r n.w pd.r pd.w pf.r pf.w r.r r.w s.r s.w t.r t.w u.r u.w";

        private static ILogger Log = LogManager.GetCurrentClassLogger();

        public AccessTokenService(ITokenClientWraper tokenClient)
        {
            _tokenClientWraper = tokenClient ?? throw new ArgumentNullException("tokenClient"); //not sure if need null check here
        }

        //All requests to get tokens come through here. We only expose the token as a return value of this method.
        public string GetAccessToken()
        {
            //If _accessToken is null, or getNew is true, lock _locker and get a new token
            if ((string.IsNullOrEmpty(_accessToken) || (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow >= expirationTokenTime))) // UtcNow is NOT adjusted for seasonal changes like DST and it is faster ...
            {
                lock (_accessToken)
                {
                    //RACE CONDITION HANDLING (by Alan from email on 7/12/2022 8:39 AM)
                    //Make sure we didn�t already just get one from a different request. If we did, just return what we already have
                    if (!string.IsNullOrEmpty(_accessToken) && DateTime.UtcNow < expirationTokenTime)
                    {
                        //If we�re here, it means we were blocked waiting for a lock to release, and another request already got us a new token. Just return THAT.
                        return _accessToken;
                    }
                    else
                    {
                        GetToken();
                    }

                }
            }
            return _accessToken; //We always just return this value, because it�s either up-to-date, or we refreshed it above.
        }

        private void GetToken()
        {
            var taskResponse = _tokenClientWraper.RequestClientCredentialsAsync(scope).Result;

            expirationTokenTime = DateTime.UtcNow.AddSeconds(taskResponse.ExpiresIn - timeInSecondsBeforeExpirationToGetNewToken); // default ExpiresIn 3600 seconds (60 minutes)

            if (taskResponse.IsError) // possible error if we ask for scope that is not assigned to the client <IdentityTestApi> (taskResponse.Error == "invalid_scope")
            {
                _accessToken = ""; //We lock() this object, and it can't be null. Set to empty.
                throw new Exception($"The following error in Program.{nameof(AccessTokenService)}.{nameof(GetToken)} with input param:'{scope}' error:'{taskResponse.Error}'");
            }

            Log.Info($"Got a new access token with experationTokenTime:'{expirationTokenTime.ToString("MM/dd/yyyy HH:mm")}'");

            _accessToken = taskResponse.AccessToken ?? ""; //Stop-gap to prevent null
        }

    }
}
