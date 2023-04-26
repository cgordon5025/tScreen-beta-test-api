using System.Threading.Tasks;
using Pulumi;
using tScreen.Infra.Main;

namespace tScreen.Infra.Main
{
    class Program
    {
        static Task<int> Main() => Deployment.RunAsync<MyStack>();
    }
}
