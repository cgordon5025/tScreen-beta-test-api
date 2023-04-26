using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace tScreen.Functions
{
    public interface ITokenProvider
    {
        Task<AuthenticationResult> GetAzureAdToken();
    }
}