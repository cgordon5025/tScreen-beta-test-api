using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using tScreen.Infra.Core.Shared;

namespace tScreen.Infra.Main
{
    public static class Helpers
    {
        public static async Task<string> GetStorageAccountPrimaryKey(string resourceGroupName, string accountName)
        {

            var accountKeys = await ListStorageAccountKeys.InvokeAsync(new ListStorageAccountKeysArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountName = accountName
            });
            return accountKeys.Keys[0].Value;
        }

        public static Output<ImmutableDictionary<string, object>> TransformToImmutableAndProtect(
            Dictionary<string, Output<string>> values)
        {
            var output = ImmutableDictionary.CreateBuilder<string, object>();
            foreach (var item in values)
            {
                output.Add(item.Key, item.Value);
            }

            return Output.CreateSecret(output.ToImmutable());
        }

        public static Output<ImmutableDictionary<string, object>> TransformToImmutableAndProtect(
            Dictionary<string, VmMachineGroup> values)
        {
            var output = ImmutableDictionary.CreateBuilder<string, object>();
            foreach (var item in values)
            {
                output.Add(item.Key, Output.Tuple(values[item.Key].Ip, values[item.Key].Public, values[item.Key].Private)
                    .Apply(x => new Dictionary<string, string>
                    {
                        { "ip", x.Item1 },
                        { "public", x.Item2 },
                        { "private", x.Item3 },
                    }));
            }

            return Output.CreateSecret(output.ToImmutable());
        }

        public static Output<ImmutableDictionary<string, object>> GetSpaEndpoints(
            Dictionary<string, Output<string>> endpoints)
        {
            var output = ImmutableDictionary.CreateBuilder<string, object>();
            foreach (var endpoint in endpoints)
            {
                output.Add(endpoint.Key, endpoint.Value.Apply(x => x));
            }

            return Output.Create(output.ToImmutable());
        }

        public static Output<ImmutableDictionary<string, object>> GetApiEndpoints(
            Dictionary<string, Output<string>> endpoints)
        {
            var output = ImmutableDictionary.CreateBuilder<string, object>();
            foreach (var endpoint in endpoints)
            {
                output.Add(endpoint.Key, endpoint.Value.Apply(x => x));
            }

            return Output.Create(output.ToImmutable());
        }

        public static InputMap<object> GetManagedIdentity(UserAssignedIdentity userAssignedIdentity)
        {
            return userAssignedIdentity.Id
                .Apply(clientId => new Dictionary<string, object>
                {
                    { clientId, new Dictionary<string, object>() }
                });
        }

        public static Output<string> GetStorageAccountConnectionString(Output<string> resourceGroupName,
            Output<string> storageAccountName)
        {
            var storageKey = Output.Tuple(resourceGroupName, storageAccountName)
                .Apply((tuple) => Output.Create(GetStorageAccountPrimaryKey(tuple.Item1, tuple.Item2)));

            var connectionString = Output
                .Tuple(resourceGroupName, storageAccountName, storageKey)
                .Apply(items =>
                {
                    var (rgName, saName, key) = items;
                    return
                        $"DefaultEndpointsProtocol=https;AccountName={saName};AccountKey={key};EndpointSuffix=core.windows.net;";
                });

            return connectionString;
        }

        public static Output<string> GetMssqlConnectionString(string username, Output<string> password,
            Output<string> server, Output<string> database)
        {
            return Output
                .Tuple(server, password, database)
                .Apply(tuple =>
                {
                    var (name, pass, db) = tuple;

                    var connectionString = new[]
                    {
                        $"Server=tcp:{name}.database.windows.net,1433",
                        $"Initial Catalog={db}",
                        "Persist Security Info=False",
                        $"User ID={username}",
                        $"Password={pass}",
                        "MultipleActiveResultSets=False",
                        "Encrypt=True",
                        "TrustServerCertificate=False",
                        "Connection Timeout=30;"
                    };

                    return string.Join(';', connectionString);
                });
        }

        // public static InputMap<string> ToInputMap(this InputList<NameValuePairArgs> inputList)
        // {
        //     var inputMap = inputList
        //         .Apply(list => Output
        //             .All(list.Select(s => Output.Tuple(s.Name!, s.Value!))))
        //         .Apply(list =>
        //         {
        //             var map = new InputMap<string>();
        //             foreach (var (key, value) in list) {
        //             {
        //                 map.Add(key, value);
        //             }}
        //
        //             return map;
        //         });
        //
        //     return inputMap;
        // }
    }
}