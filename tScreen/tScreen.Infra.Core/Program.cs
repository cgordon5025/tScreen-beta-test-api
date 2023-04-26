using System.Threading.Tasks;
using Pulumi;
using tScreen.Infra.Core;

namespace tScreen.Infra.Core
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}
