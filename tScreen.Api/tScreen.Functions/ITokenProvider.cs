using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace TweenScreen.Functions
{
    public interface ITokenProvider
    {
        Task<AuthenticationResult> GetAzureAdToken();
    }
}