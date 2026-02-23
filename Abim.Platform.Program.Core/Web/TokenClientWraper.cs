using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Core.Identity
{
    public class TokenClientWraper : IDisposable, ITokenClientWraper
    {
        private TokenClient _tokenClient;

        public TokenClientWraper()
        {
        }

        public TokenClientWraper(string address, string clientId, string clientSecret)
        {
            _tokenClient = new TokenClient(address, clientId, clientSecret);
        }

        public async Task<TokenResponse> RequestClientCredentialsAsync(string scope = null)
        {
            return await _tokenClient.RequestClientCredentialsAsync(scope);
        }

        public void Dispose()
        {
            ((IDisposable)_tokenClient).Dispose();
        }
    }
}
