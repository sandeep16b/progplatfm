using IdentityModel.Client;
using System.Threading.Tasks;

namespace Abim.Platform.Program.Core.Identity
{
    public interface ITokenClientWraper
    {
        Task<TokenResponse> RequestClientCredentialsAsync(string scope);
    }
}