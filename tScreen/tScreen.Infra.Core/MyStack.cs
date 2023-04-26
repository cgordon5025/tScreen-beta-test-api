using System.Collections.Immutable;
using System.Threading.Tasks;
using Pulumi;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using tScreen.Infra.Core.Shared;
using tScreen.Infra.Core.Shared;
using SkuName = Pulumi.AzureNative.Storage.SkuName;

namespace tScreen.Infra.Core
{
    class MyStack : Stack
    {
        public MyStack()
        {
            var stackName = Pulumi.Deployment.Instance.StackName;
            var appNamePart = $"tws-core";

            // Create an Azure Resource Group
            var resourceGroup = new ResourceGroup($"tscreen-{stackName}", new ResourceGroupArgs
            {
                Tags = InfrastructureStandard.Tags,
            });

            var dnsZone = new Zone("tws-core-dns-zone", new ZoneArgs
            {
                Location = "global",
                ResourceGroupName = resourceGroup.Name,
                ZoneType = ZoneType.Public,
                ZoneName = "tscreen.health",
                Tags = InfrastructureStandard.Tags
            });

            // Create an Azure resource (Storage Account)
            var storageAccountName = $"{appNamePart.Replace("-", "")}sa";
            var storageAccount = new StorageAccount(storageAccountName, new StorageAccountArgs
            {
                ResourceGroupName = resourceGroup.Name,
                Sku = new SkuArgs
                {
                    Name = SkuName.Standard_LRS
                },
                Kind = Kind.StorageV2
            });

            // https://docs.microsoft.com/en-us/azure/templates/microsoft.web/2018-02-01/serverfarms?tabs=bicep#appserviceplanproperties-object
            var devAppServicePlan = new AppServicePlan($"{appNamePart}-service-plan", new AppServicePlanArgs
            {
                //Kind = "Linux",
                Kind = "linux", // Windows
                // Required for Linux based app plans. See link above for more details. Not available for "Shared"
                Reserved = true,
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Sku = new Pulumi.AzureNative.Web.Inputs.SkuDescriptionArgs
                {
                    Tier = "Basic",
                    Name = "B1",
                },
                Tags = InfrastructureStandard.Tags
            });

            var prdAppServicePlan = new AppServicePlan($"{appNamePart}-prd-service-plan", new AppServicePlanArgs
            {
                Kind = "Linux",
                Reserved = true,
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                Sku = new Pulumi.AzureNative.Web.Inputs.SkuDescriptionArgs
                {
                    Tier = "PremiumV2",
                    Name = "P1v2",
                    Size = "P1v2",
                    Family = "Pv2",
                    Capacity = 1
                },
                Tags = InfrastructureStandard.Tags
            });

            this.CoreResourceGroupName = resourceGroup.Name;

            this.CoreStorageName = storageAccount.Name;

            // Export the primary key of the Storage Account
            this.CoreStorageKey = Output.Tuple(resourceGroup.Name, storageAccount.Name).Apply(names =>
                Output.CreateSecret(GetStorageAccountPrimaryKey(names.Item1, names.Item2)));

            this.CoreAppServicePlanId = devAppServicePlan.Id;
            this.PrdAppServicePlanId = prdAppServicePlan.Id;
        }

        [Output(OutputNames.CoreResourceGroupName)]
        public Output<string> CoreResourceGroupName { get; set; }

        [Output(OutputNames.CoreStorageAccountName)]
        public Output<string> CoreStorageName { get; set; }

        [Output(OutputNames.CoreStorageKey)]
        public Output<string> CoreStorageKey { get; set; }

        [Output(OutputNames.CoreAppServicePlanId)]
        public Output<string> CoreAppServicePlanId { get; set; }

        [Output(OutputNames.PrdAppServicePlan)]
        public Output<string> PrdAppServicePlanId { get; set; }

        private static async Task<string> GetStorageAccountPrimaryKey(string resourceGroupName, string accountName)
        {
            var accountKeys = await ListStorageAccountKeys.InvokeAsync(new ListStorageAccountKeysArgs
            {
                ResourceGroupName = resourceGroupName,
                AccountName = accountName
            });
            return accountKeys.Keys[0].Value;
        }
    }
}
