using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using Core;
using Core.Text;
using Pulumi;
using Pulumi.AzureAD;
using Pulumi.AzureDevOps;
using Pulumi.AzureNative.Compute;
using Pulumi.AzureNative.Compute.Inputs;
using Pulumi.AzureNative.Insights;
using Pulumi.AzureNative.Insights.Inputs;
using Pulumi.AzureNative.KeyVault;
using Pulumi.AzureNative.KeyVault.Inputs;
using Pulumi.AzureNative.ManagedIdentity;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.AzureNative.OperationalInsights;
using Pulumi.AzureNative.Resources;
using Pulumi.AzureNative.Sql;
using Pulumi.AzureNative.Sql.Inputs;
using Pulumi.AzureNative.Storage;
using Pulumi.AzureNative.Storage.Inputs;
using Pulumi.AzureNative.Web;
using Pulumi.AzureNative.Web.Inputs;
using Pulumi.Random;
using Pulumi.Tls;
using Shared;
using Shared.Resources;
using tScreen.Infra.Core;
using tScreen.Infra.Core.Shared;
using TwingateLabs.Twingate.Inputs;
using Config = Pulumi.Config;
using Deployment = Pulumi.Deployment;
using IdentityType = Pulumi.AzureNative.Sql.IdentityType;
using Kind = Pulumi.AzureNative.Storage.Kind;
using ManagedServiceIdentityArgs = Pulumi.AzureNative.Web.Inputs.ManagedServiceIdentityArgs;
using ManagedServiceIdentityType = Pulumi.AzureNative.Web.ManagedServiceIdentityType;
using SecretArgs = Pulumi.AzureNative.KeyVault.SecretArgs;
using SkuArgs = Pulumi.AzureNative.KeyVault.Inputs.SkuArgs;
using SkuName = Pulumi.AzureNative.KeyVault.SkuName;
using SubnetArgs = Pulumi.AzureNative.Network.SubnetArgs;
using AzureAD = Pulumi.AzureAD;
using IPVersion = Pulumi.AzureNative.Network.IPVersion;
using NetworkProfileArgs = Pulumi.AzureNative.Compute.Inputs.NetworkProfileArgs;
using NetworkSecurityGroupArgs = Pulumi.AzureNative.Network.NetworkSecurityGroupArgs;
using ResourceIdentityType = Pulumi.AzureNative.Compute.ResourceIdentityType;
using SecurityRuleArgs = Pulumi.AzureNative.Network.Inputs.SecurityRuleArgs;
using SshPublicKeyArgs = Pulumi.AzureNative.Compute.SshPublicKeyArgs;
using SubResourceArgs = Pulumi.AzureNative.Network.Inputs.SubResourceArgs;

// ReSharper disable UnusedVariable
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace tScreen.Infra.Main
{
    public class MyStack : Stack
    {
        private readonly string _tenantId;
        private readonly string _stackName;
        private readonly Dictionary<string, UserAssignedIdentity> _managedIdentities = new();
        private readonly Dictionary<string, Vault> _keyVaults = new();
        private readonly Dictionary<string, Output<string>> _jwtSigningKeys = new();
        private readonly Dictionary<string, Output<string>> _storageAccountNames = new();
        private readonly Dictionary<string, Output<string>> _apiEndpoints = new();
        private readonly Dictionary<string, Output<string>> _webEndpoints = new();
        private readonly Dictionary<string, Output<string>> _databases = new();
        private readonly Dictionary<string, Component> _applicationInsights = new();
        private readonly List<Output<string>> _knownWebClients = new();
        private readonly Dictionary<string, VmMachineGroup> _vmMachineGroup = new();

        public MyStack()
        {
            Utils.WaitForDebuggerIfNeeded("Pulumi", "PULUMI_DEBUG");

            // General
            _stackName = Deployment.Instance.StackName;
            var appPrefix = "tws";
            var appNamePart = $"{appPrefix}-{_stackName}";

            // Configuration
            var config = new Config();
            _tenantId = config.Require("tenantId");

            var mssqlUsername = config.Require("mssqlUsername");
            var mssqlPassword = config.RequireSecret("mssqlPassword");
            var location = config.Require("location");
            var secondaryEnvironments = config.GetObject<string[]>("secondaryEnvironments")?
                .ToList() ?? new List<string>();

            // Stack references
            var coreReferenceName = $"{config.Require("organization")}/tscreen-core/primary";
            var coreStackReference = new StackReference(coreReferenceName);
            //
            // var coreStorageAccountName =
            //     coreStackReference.RequireOutput(tScreen.Infra.Core.OutputNames.CoreStorageAccountName);

            var coreAppServicePlanId = coreStackReference
                .RequireOutput(Core.OutputNames.CoreAppServicePlanId);

            var prdAppServicePlanId = coreStackReference
                .RequireOutput(Core.OutputNames.PrdAppServicePlan);

            var coreResourceGroupName = coreStackReference
                .RequireOutput(tScreen.Infra.Core.OutputNames.CoreResourceGroupName);

            var coreResourceGroup = GetResourceGroup.Invoke(new GetResourceGroupInvokeArgs
            {
                ResourceGroupName = coreResourceGroupName.Apply(x => (string)x)
            });

            //
            // var coreStorageAccount = GetStorageAccount
            //     .Invoke(new GetStorageAccountInvokeArgs
            //     {
            //         ResourceGroupName = coreResourceGroup.Apply(x => x.Name),
            //         AccountName = coreStorageAccountName.Apply(x => (string) x)
            //     });

            var appServicePLan = _stackName switch
            {
                "dev" => coreAppServicePlanId,
                "prd" => prdAppServicePlanId,
                _ => coreAppServicePlanId
            };

            var stackUser = GetUser.Invoke(new GetUserInvokeArgs
            {
                UserPrincipalName = config.Require("upn")
            });

            var standardTags = InfrastructureStandard.Tags;
            // standardTags["ManagedBy"] = config.Require("upn");

            // Create an Azure Resource Group
            var resourceGroup = new ResourceGroup(
                $"tscreen-app-{_stackName}-{location}-rg",
                new ResourceGroupArgs
                {
                    Location = location,
                    Tags = standardTags
                });

            var environments = new List<string> { _stackName };
            environments.AddRange(secondaryEnvironments);


            var stackAppRegistrations = BuildIdentitiesAndRegistrations(appPrefix, location, stackUser,
                resourceGroup, environments);

            // var appRegistration = new AzureAD.Application(
            //     $"tscreen-main-{_stackName}-appreg", 
            //     new AzureAD.ApplicationArgs
            //     {
            //         DisplayName = $"Tws {_stackName.ToPascalCase()} API",
            //         IdentifierUris =
            //         {
            //             $"api://tws-api-{_stackName}"
            //         },
            //         Owners =
            //         {
            //             stackUser.Apply(x => x.ObjectId)
            //         }
            //     });
            //
            // var stackGroup = new AzureAD.Group(
            //     $"tscreen-main-{_stackName}-{location}-webapp", 
            //     new AzureAD.GroupArgs
            //     {
            //         DisplayName = "tScreen WebApp",
            //         SecurityEnabled = true,
            //     }, new CustomResourceOptions { Parent = appRegistration, DependsOn = appRegistration });
            //
            // var stackGroupMember = new AzureAD.GroupMember(
            //     $"tscreen-main-{_stackName}-{location}-webapp-member-0001",
            //     new AzureAD.GroupMemberArgs
            //     {
            //         GroupObjectId = stackGroup.ObjectId,
            //         MemberObjectId = stackUser.Apply(x => x.ObjectId)
            //     }, new CustomResourceOptions { Parent = stackGroup, DependsOn = stackGroup });
            //
            // // For automation and other concerns
            // var servicePrincipal = new AzureAD.ServicePrincipal(
            //     $"tscreen-main-{_stackName}-appreg-sp",
            //     new ServicePrincipalArgs
            //     {
            //         ApplicationId = appRegistration.ApplicationId
            //     }, new CustomResourceOptions { Parent = appRegistration, DependsOn = appRegistration });
            //
            // var stackManagedIdentity = new UserAssignedIdentity($"{appNamePart}-mi",
            //     new UserAssignedIdentityArgs
            //     {
            //         Location = resourceGroup.Location,
            //         ResourceGroupName = resourceGroup.Name,
            //         Tags = standardTags
            //     }, new CustomResourceOptions { Parent = appRegistration, DependsOn = appRegistration });

            var (virtualNetwork, backendSubnet) = BuildVirtualNetworkWithStandardSubnets(appNamePart,
                resourceGroup, out var frontendSubnet);

            BuildKeyVaults(appPrefix, stackUser, resourceGroup, environments);
            BuildJwtSigningKeys(appPrefix, resourceGroup, environments);
            var (
                logResourceGroups,
                logStorageAccounts,
                workspaces) = BuildLogsEnvironments(appPrefix, resourceGroup, location, environments);

            var sqlServer = new Server($"{appNamePart}-mssql", new ServerArgs
            {
                PublicNetworkAccess = _stackName == "prd" ? "Disabled" : "Enabled",
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                AdministratorLogin = mssqlUsername,
                AdministratorLoginPassword = mssqlPassword,
                Version = "12.0",
                MinimalTlsVersion = "1.2",
                Administrators = new ServerExternalAdministratorArgs
                {
                    AdministratorType = AdministratorType.ActiveDirectory,
                    AzureADOnlyAuthentication = false,
                    Login = stackUser.Apply(x => x.UserPrincipalName),
                    PrincipalType = PrincipalType.User,
                    Sid = stackUser.Apply(x => x.ObjectId),
                    TenantId = _tenantId
                },
                Identity = new ResourceIdentityArgs
                {
                    Type = IdentityType.UserAssigned,
                    UserAssignedIdentities = Helpers.GetManagedIdentity(_managedIdentities[_stackName])
                },
                PrimaryUserAssignedIdentityId = _managedIdentities[_stackName].Id.Apply(x => x),
                Tags = standardTags,
            });

            if (_stackName == "prd")
            {
                // var policy = new Pulumi.AzureNative.Sql.ServerBlobAuditingPolicy()

                var sqlServerSecurityAlertPolicy = new ServerSecurityAlertPolicy(
                    $"{appNamePart}-mssql-ssap",
                    new ServerSecurityAlertPolicyArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        ServerName = sqlServer.Name,
                        State = SecurityAlertsPolicyState.Enabled
                    });

                // var logResourceGroupsReference =
                //     coreStackReference.RequireOutput(tScreen.Infra.Core.OutputNames.LogResourceGroups);

                // var logWorkspaceReference = coreStackReference
                //     .RequireOutput(tScreen.Infra.Core.OutputNames.LogWorkspace);

                // var logResourceGroups = logResourceGroupsReference
                //     .Apply(o => (ImmutableDictionary<string, object>) o);
                // var logWorkspace = logWorkspaceReference
                //     .Apply(o => (ImmutableDictionary<string, object>) o);

                var sqlServerDiagnosticSetting = new DiagnosticSetting(
                    $"{appNamePart}-mssql-diag",
                    new DiagnosticSettingArgs
                    {
                        Logs = new LogSettingsArgs
                        {
                            Category = "SQLSecurityAuditEvents",
                            Enabled = true,
                            RetentionPolicy = new RetentionPolicyArgs
                            {
                                Days = 0,
                                Enabled = true
                            }
                        },
                        ResourceUri = sqlServer.Id.Apply(id => $"{id}/databases/master"),
                        WorkspaceId = workspaces[_stackName].Id
                    },
                    new CustomResourceOptions
                    {
                        DependsOn = { resourceGroup, sqlServer, workspaces[_stackName] }
                    });
            }

            if (_stackName == "dev")
            {
                // Allow all azure service to access firewsqlseall
                var allAzureServicesFirewallRule = new FirewallRule(
                    $"{appNamePart}-mssql-firewall-rule-allowall",
                    new FirewallRuleArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        FirewallRuleName = "AllowAllWindowsAzureIps",
                        ServerName = sqlServer.Name,
                        EndIpAddress = "0.0.0.0",
                        StartIpAddress = "0.0.0.0"
                    }, new CustomResourceOptions { Parent = sqlServer, DependsOn = sqlServer });
            }

            this.AppSqlServerName = sqlServer.Name.Apply(x => x);

            BuildStorageEnvironments(appPrefix, resourceGroup, sqlServer,
            logResourceGroups, logStorageAccounts,
            environments);

            // Both connection strings are accessible through the app service. If the connection strings are needed 
            // outside of the app service, or web job/function then disable this code to put the connection strings
            // into the associated key vault. Disabled because there's no need for redundancy and connection string
            // are securely stored in the app service.
            //
            // var secretDbConString = new Secret($"{appNamePart}-secret-dbconnstring",
            //     new Pulumi.AzureNative.KeyVault.SecretArgs
            //     {
            //         ResourceGroupName = resourceGroup.Name,
            //         VaultName = keyVault.Name,
            //         SecretName = PlatformConstants.ConnectionStringNames.Mssql.Replace(":", "--"), // for valid name
            //         Properties = new SecretPropertiesArgs
            //         {
            //             ContentType = "text/plain",
            //             Value = Helpers.GetMssqlConnectionString(mssqlUsername, mssqlPassword, sqlServer.Name,
            //                 databases[stackName])
            //         },
            //         Tags = InfrastructureStandard.Tags,
            //     });
            //
            // var secretSaConString = new Secret($"{appNamePart}-secret-saconnstring",
            //     new Pulumi.AzureNative.KeyVault.SecretArgs
            //     {
            //         ResourceGroupName = resourceGroup.Name,
            //         VaultName = keyVault.Name,
            //         SecretName = PlatformConstants.ConnectionStringNames.Storage.Replace(":", "--"), // for valid name
            //         Properties = new SecretPropertiesArgs
            //         {
            //             ContentType = "text/plain",
            //             Value = Helpers.GetStorageAccountConnectionString(resourceGroup.Name,
            //                 storageAccountNames[stackName])
            //         },
            //         Tags = InfrastructureStandard.Tags
            //     });

            var appSettings = new InputMap<string>()
            {
                {
                    "APPLICATION_HOST",
                    "Azure"
                },
                {
                    "AzureAd__TenantId",
                    _tenantId
                }
            };

            if (_stackName == "prd")
            {
                var databaseSubnet = new Subnet($"{appNamePart}-vn-database-sn", new SubnetArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    AddressPrefix = "10.0.5.0/24",
                    SubnetName = "tws-database-sn",
                    // Delegations = new DelegationArgs
                    // {
                    //     ServiceName = "Microsoft.Sql/managedInstances",
                    //     Name = "database-delegation"
                    // },
                    VirtualNetworkName = virtualNetwork.Name,
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = virtualNetwork
                });

                BuildSqlServerPrivateLink(appNamePart, resourceGroup, sqlServer, virtualNetwork, databaseSubnet);

                appSettings.AddRange(new InputMap<string>
                {
                    { "WEBSITE_DNS_SERVER", "168.63.129.16" },
                    { "WEBSITE_VNET_ROUTE_ALL", "1" }
                });
            }

            BuildWebPrimaryEnvironment(appPrefix, resourceGroup, coreResourceGroup, _stackName, secondaryEnvironments);

            // Ensure localhost is available in development (dev stack)
            if (_stackName == "dev")
            {
                _knownWebClients.Add(Output.Create("https://localhost:3000"));
            }

            if (_webEndpoints.ContainsKey(_stackName)) { }
            {
                _knownWebClients.Add(Output.Create("https://tscreen.health"));
                _knownWebClients.Add(_webEndpoints[_stackName].Apply(x => $"https://{x}"));
            }

            var apiAppServiceSettings = new InputMap<string>();
            apiAppServiceSettings.AddRange(appSettings);
            apiAppServiceSettings.AddRange(new InputMap<string>
                {
                    {
                        "AppStorage__AccountName",
                        _storageAccountNames[_stackName]
                    },
                    {
                        "AppStorage__AccountKey",
                        Output
                            .Tuple(resourceGroup.Name, _storageAccountNames[_stackName])
                            .Apply(x =>
                                Output.Create(Helpers.GetStorageAccountPrimaryKey(x.Item1, x.Item2)))
                    },
                    {
                        "AppStorage__TenantId",
                        _tenantId
                    },
                    {
                        "AppStorage__ClientId",
                        stackAppRegistrations[_stackName].Application.ApplicationId
                    },
                    {
                        "AppStorage__ClientSecret",
                        stackAppRegistrations[_stackName].ApplicationPassword.Value
                    },
                    {
                        "AppStorage__UserAssignedId",
                        _managedIdentities[_stackName].ClientId
                    },
                    {
                        "AzureAd__ClientId",
                        stackAppRegistrations[_stackName].Application.ApplicationId
                    },
                    {
                        "AzureAd__ResourceId",
                        stackAppRegistrations[_stackName]
                            .Application.ApplicationId.Apply(x => $"api://tws-{_stackName}-app")
                    },
                    {
                        "AzureAdClient__ResourceId",
                        stackAppRegistrations[_stackName]
                            .Application.ApplicationId.Apply(x => $"api://tws-{_stackName}-app/.default")
                    },
                    {
                        "AzureAdClient__ClientId",
                        stackAppRegistrations[_stackName].Worker.ApplicationId
                    },
                    {
                        "AzureAdClient__ClientSecret",
                        stackAppRegistrations[_stackName].WorkerPassword.Value
                    },
                    {
                        "AzureAdClient__TenantId",
                        _tenantId
                    },
                    {
                        "APPINSIGHTS_INSTRUMENTATIONKEY",
                        _applicationInsights[_stackName].InstrumentationKey
                    },
                    {
                        "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        _applicationInsights[_stackName].InstrumentationKey
                            .Apply(key => $"InstrumentationKey={key}")
                    },
                    {
                        "ApplicationInsightsAgent_EXTENSION_VERSION",
                        "~2"
                    },
                    {
                        "KeyVault__UserAssignedId",
                        _managedIdentities[_stackName].ClientId
                    },
                    {
                        "KeyVault__VaultUri",
                        Output.Format($"https://{_keyVaults[_stackName].Name}.vault.azure.net")
                    },
                    {
                        "TwsMssql__ConnectionString",
                        Helpers.GetMssqlConnectionString(mssqlUsername, mssqlPassword,
                            sqlServer.Name, _databases[_stackName])
                    },
                    {
                        "BlobStorage__ConnectionString",
                        Helpers.GetStorageAccountConnectionString(resourceGroup.Name,
                            _storageAccountNames[_stackName])
                    },
                    // Required because Azure Web Jobs configuration is way too opinionated
                    // Used with the VMs that execute webjobs
                    {
                        "ConnectionStrings__AzureWebJobsStorage",
                        Helpers.GetStorageAccountConnectionString(resourceGroup.Name,
                            _storageAccountNames[_stackName])
                    },
                    {
                        "tScreenClient__BaseUrl",
                        _webEndpoints[_stackName].Apply(x => $"https://{x}")
                    }
                });

            var apiAppService = new WebApp($"{appNamePart}-api-{_stackName}", new WebAppArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                ServerFarmId = Output.Format($"{appServicePLan}"),
                HttpsOnly = true,
                SiteConfig = new SiteConfigArgs
                {
                    // 64 Bit not available on "Free" and "Shared" tiers only on "Basic," "Standard" and above.
                    // Change app service plan to enable
                    Use32BitWorkerProcess = false,
                    // az webapp list-runtimes --os-type linux
                    LinuxFxVersion = "DOTNETCORE|6.0",
                    MinTlsVersion = SupportedTlsVersions.SupportedTlsVersions_1_2,
                    FtpsState = FtpsState.FtpsOnly,
                    // Only support for the Basic and Standard plans disabled for now.
                    AlwaysOn = true,
                    VnetRouteAllEnabled = true
                },
                Identity = new ManagedServiceIdentityArgs
                {
                    Type = ManagedServiceIdentityType.UserAssigned,
                    UserAssignedIdentities = Helpers.GetManagedIdentity(_managedIdentities[_stackName])
                },
                Tags = standardTags
            });

            var webAppVNetConnection = new WebAppSwiftVirtualNetworkConnection(
                $"{appPrefix}-api-{_stackName}-vnetconn",
                new WebAppSwiftVirtualNetworkConnectionArgs
                {
                    Name = apiAppService.Name,
                    ResourceGroupName = resourceGroup.Name,
                    SubnetResourceId = frontendSubnet.Id
                },
                new CustomResourceOptions
                {
                    Parent = apiAppService,
                    DependsOn = { apiAppService, virtualNetwork, frontendSubnet }
                });

            var index = 0;
            foreach (var url in _knownWebClients)
                apiAppServiceSettings.Add($"KnownWebClients__Domains__{index++}", url);

            // We need to see the expected authority of the JWT. The authority is the app service 
            // hostname. But we need to build the app service first before we can have the
            // hostname. The reason for the separation from the other app settings
            apiAppServiceSettings.AddRange(new InputMap<string>
            {
                { "Jwt__Audience", "Any" },
                { "Jwt__Authority", apiAppService.DefaultHostName.Apply(x => $"https://{x}") },
                { "tScreenApi__BaseUrl", apiAppService.DefaultHostName.Apply(x => $"https://{x}")}
            });

            var appServiceSetting = new WebAppApplicationSettings(
                $"{appNamePart}-api-{_stackName}-app-settings",
                new WebAppApplicationSettingsArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    Name = apiAppService.Name,
                    Properties = apiAppServiceSettings
                },
                new CustomResourceOptions
                {
                    Parent = apiAppService,
                    DependsOn = apiAppService
                });

            _apiEndpoints.Add(_stackName, apiAppService.DefaultHostName);

            BuildApiSecondaryEnvironments(
                $"{appPrefix}-api", resourceGroup, appServicePLan, apiAppService, frontendSubnet,
                secondaryEnvironments, appSettings, sqlServer.Name, mssqlUsername, mssqlPassword,
                stackAppRegistrations);

            var workerVms = BuildWorkerVMs(appPrefix, resourceGroup, backendSubnet,
                _managedIdentities, environments);

            // BuildAzureDevopsPipelineVmss(appPrefix, resourceGroup, virtualNetwork);

            if (_stackName == "prd")
            {
                var twingateNetworkResources = new List<TwingateNetworkResource>
                {
                    new TwingateNetworkResource
                    {
                        Name = "Azure SQL",
                        Label = sqlServer.Name.Apply(name => $"{name} (private)"),
                        Address = sqlServer.FullyQualifiedDomainName,
                        GroupIds = new[] { "R3JvdXA6NjM1NzU=", "R3JvdXA6NjUxNjg=" },
                        Protocols = new TwingateResourceProtocolsArgs
                        {
                            Tcp = new TwingateResourceProtocolsTcpArgs
                            {
                                Policy = "RESTRICTED",
                                Ports = new[] { "1433" }
                            },
                            Udp = new TwingateResourceProtocolsUdpArgs
                            {
                                Policy = "DENY_ALL"
                            },
                            AllowIcmp = false
                        }
                        // Everyone for now
                    },
                };

                foreach (var environment in environments)
                {
                    twingateNetworkResources.Add(new TwingateNetworkResource
                    {
                        Name = $"TWS {environment.ToPascalCase()} Worker",
                        Label = workerVms[environment].VirtualMachine.Name
                            .Apply(name => $"{name} (private)"),
                        Address = workerVms[environment].NetworkInterface.IpConfigurations
                            .Apply(x => x[0].PrivateIPAddress ?? "0.0.0.0"),
                        GroupIds = new[] { "R3JvdXA6NjM1NzU=", "R3JvdXA6NjUxNjg=" },
                        Protocols = new TwingateResourceProtocolsArgs
                        {
                            Tcp = new TwingateResourceProtocolsTcpArgs
                            {
                                Policy = "RESTRICTED",
                                Ports = new[] { "22", "9001" }
                            },
                            Udp = new TwingateResourceProtocolsUdpArgs
                            {
                                Policy = "DENY_ALL"
                            },
                            AllowIcmp = false
                        }
                    });
                }

                var twingateSubnet = new Subnet(
                    $"{appNamePart}-vn-twingate-sn",
                    new SubnetArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        AddressPrefix = "10.0.4.0/24",
                        SubnetName = "tws-twingate-sn",
                        VirtualNetworkName = virtualNetwork.Name,
                        Delegations = new[]
                        {
                            new DelegationArgs
                            {
                                ServiceName = "Microsoft.ContainerInstance/containerGroups",
                                Name = "twingate-delegations"
                            }
                        },
                    },
                    new CustomResourceOptions
                    {
                        Parent = virtualNetwork,
                        DependsOn = virtualNetwork
                    });

                var twingate = new TwingateNetwork(
                    $"tws-{_stackName}-{location}-twingate",
                    new TwingateNetworkResourceArgs
                    {
                        TenantUrl = "https://futuresthrive.twingate.com",
                        DisplayName = $"tScreen {_stackName.ToPascalCase()} ({location.ToPascalCase()})",
                        ResourceGroup = resourceGroup,
                        VirtualNetwork = virtualNetwork,
                        Subnet = twingateSubnet,
                        NumberOfConnectors = 2,
                        Resources = twingateNetworkResources,
                        Tags = standardTags
                    }, new ComponentResourceOptions()
                    {
                        DependsOn = { sqlServer, virtualNetwork, twingateSubnet, }
                    });
            }

            // Get URL for where static site is hosts
            this.SpaEndpoints = Helpers.GetSpaEndpoints(_webEndpoints);

            // Get URL for where the API service is hosted
            this.ApiEndpoints = Helpers.GetApiEndpoints(_apiEndpoints);

            this.JwtSigningKeys = Helpers.TransformToImmutableAndProtect(_jwtSigningKeys);

            this.VmWorkers = Helpers.TransformToImmutableAndProtect(_vmMachineGroup);
        }

        private (VirtualNetwork virtualNetwork, Subnet backendSubnet)
            BuildVirtualNetworkWithStandardSubnets(string namePart, ResourceGroup resourceGroup,
            out Subnet frontendSubnet)
        {
            // There's a bug with the VNET provider which when a tag is updated, the state of the
            // VNET resource becomes unstable. For now, we're removing the "CommitHash" tag
            // until the resource provider is fixed
            var virtualNetworkTags = new Dictionary<string, string>(InfrastructureStandard.Tags);
            virtualNetworkTags.Remove("CommitHash");

            var virtualNetwork = new VirtualNetwork(
                $"tws-{_stackName}-app-vn",
                new VirtualNetworkArgs
                {
                    // VirtualNetworkName = 
                    Location = resourceGroup.Location,
                    ResourceGroupName = resourceGroup.Name,
                    AddressSpace = new AddressSpaceArgs
                    {
                        AddressPrefixes = new[]
                        {
                            "10.0.0.0/16",
                        },
                    },
                    Tags = virtualNetworkTags
                },
                new CustomResourceOptions
                {
                    // IgnoreChanges = { "subnets" }
                });

            var backendSubnet = new Subnet(
                $"{namePart}-vn-backend-sn",
                new SubnetArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    AddressPrefix = "10.0.1.0/24",
                    SubnetName = "tws-backend-sn",
                    VirtualNetworkName = virtualNetwork.Name,
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = virtualNetwork
                });

            var servicesSubnet = new Subnet(
                $"{namePart}-vn-services-sn",
                new SubnetArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    AddressPrefix = "10.0.2.0/24",
                    SubnetName = "tws-service-sn",
                    VirtualNetworkName = virtualNetwork.Name
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = virtualNetwork
                });

            frontendSubnet = new Subnet(
                $"{namePart}-vn-frontend-sn",
                new SubnetArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    AddressPrefix = "10.0.3.0/24",
                    SubnetName = "tws-frontend-sn",
                    VirtualNetworkName = virtualNetwork.Name,
                    Delegations = new[]
                    {
                        new DelegationArgs
                        {
                            ServiceName = "Microsoft.Web/serverfarms",
                            Name = "frontend-delegations"
                        }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = virtualNetwork
                });

            return (virtualNetwork, backendSubnet);
        }

        private Dictionary<string, StackAppRegistration> BuildIdentitiesAndRegistrations(
            string namePart, string location, Input<GetUserResult> stackUser, ResourceGroup resourceGroup,
            IEnumerable<string> environments)
        {
            var stackAppRegistrations = new Dictionary<string, StackAppRegistration>();

            foreach (var environment in environments)
            {
                var appRegistration = new AzureAD.Application(
                    $"tscreen-main-{environment}-app-appreg",
                    new AzureAD.ApplicationArgs
                    {
                        DisplayName = $"Tws {environment.ToPascalCase()} API",
                        IdentifierUris =
                        {
                            $"api://tws-{environment}-app"
                        },
                        Owners =
                        {
                            stackUser.Apply(x => x.ObjectId)
                        }
                    });

                var appRegistrationPassword = new AzureAD.ApplicationPassword(
                    $"tscreen-main-{environment}-app-apppwd",
                    new ApplicationPasswordArgs
                    {
                        ApplicationObjectId = appRegistration.ObjectId,
                        DisplayName = "Primary",
                        EndDate = "2025-11-12T17:00:00Z",
                        StartDate = "2022-11-12T17:00:00Z"
                    },
                    new CustomResourceOptions
                    {
                        Parent = appRegistration,
                        DependsOn = appRegistration
                    });

                var appRegistrationWorker = new AzureAD.Application(
                    $"tscreen-main-{environment}-worker-appreg",
                    new AzureAD.ApplicationArgs
                    {
                        DisplayName = $"Tws {environment.ToPascalCase()} Worker",
                        IdentifierUris =
                        {
                            $"api://tws-{environment}-app-worker"
                        },
                        Owners =
                        {
                            stackUser.Apply(x => x.ObjectId)
                        }
                    });

                var appRegistrationWorkerPassword = new AzureAD.ApplicationPassword(
                    $"tscreen-main-{environment}-worker-apppwd",
                    new ApplicationPasswordArgs
                    {
                        ApplicationObjectId = appRegistrationWorker.ObjectId,
                        DisplayName = "Worker Pwd",
                        EndDate = "2025-11-12T17:00:00Z",
                        StartDate = "2022-11-12T17:00:00Z"
                    },
                    new CustomResourceOptions
                    {
                        Parent = appRegistrationWorker,
                        DependsOn = appRegistrationWorker
                    });

                var stackGroup = new AzureAD.Group(
                    $"tscreen-main-{environment}-{location}-webapp",
                    new AzureAD.GroupArgs
                    {
                        DisplayName = "tScreen WebApp",
                        SecurityEnabled = true,
                    },
                    new CustomResourceOptions
                    {
                        Parent = appRegistration,
                        DependsOn = appRegistration
                    });

                var stackGroupMember = new AzureAD.GroupMember(
                    $"tscreen-main-{environment}-{location}-webapp-member-0001",
                    new AzureAD.GroupMemberArgs
                    {
                        GroupObjectId = stackGroup.ObjectId,
                        MemberObjectId = stackUser.Apply(x => x.ObjectId)
                    },
                    new CustomResourceOptions
                    {
                        Parent = stackGroup,
                        DependsOn = stackGroup
                    });

                // For automation and other concerns
                var servicePrincipal = new AzureAD.ServicePrincipal(
                    $"tscreen-main-{environment}-appreg-sp",
                    new ServicePrincipalArgs
                    {
                        ApplicationId = appRegistration.ApplicationId
                    },
                    new CustomResourceOptions
                    {
                        Parent = appRegistration,
                        DependsOn = appRegistration
                    });

                var stackManagedIdentity = new UserAssignedIdentity($"{namePart}-{environment}-mi",
                    new UserAssignedIdentityArgs
                    {
                        Location = resourceGroup.Location,
                        ResourceGroupName = resourceGroup.Name,
                        Tags = InfrastructureStandard.Tags
                    }, new CustomResourceOptions
                    {
                        Parent = appRegistration,
                        DependsOn = appRegistration
                    });

                stackAppRegistrations.Add(environment, new StackAppRegistration
                {
                    Application = appRegistration,
                    ApplicationPassword = appRegistrationPassword,
                    Worker = appRegistrationWorker,
                    WorkerPassword = appRegistrationWorkerPassword,
                    Group = stackGroup,
                    ServicePrincipal = servicePrincipal,
                    UserAssignedIdentity = stackManagedIdentity
                });

                _managedIdentities.Add(environment, stackManagedIdentity);
            }

            return stackAppRegistrations;
        }

        public void BuildKeyVaults(string namePart, Input<GetUserResult> stackUser, ResourceGroup resourceGroup,
            IEnumerable<string> environments)
        {
            foreach (var environment in environments)
            {
                var keyVault = new Vault($"{namePart}-{environment}-api-kv", new VaultArgs
                {
                    Location = resourceGroup.Location,
                    ResourceGroupName = resourceGroup.Name,

                    Properties = new VaultPropertiesArgs
                    {
                        EnableSoftDelete = true,
                        EnabledForDeployment = true,
                        EnabledForDiskEncryption = true,
                        EnabledForTemplateDeployment = true,
                        Sku = new SkuArgs
                        {
                            Family = SkuFamily.A,
                            Name = SkuName.Standard
                        },
                        AccessPolicies =
                        {
                            new AccessPolicyEntryArgs
                            {
                                TenantId = _tenantId,
                                ObjectId = stackUser.Apply(x => x.ObjectId),
                                Permissions = new PermissionsArgs
                                {
                                    Keys = { "list", "encrypt", "decrypt", "delete" },
                                    Secrets = { "get", "list", "set", "delete" }
                                }
                            },
                            new AccessPolicyEntryArgs
                            {
                                TenantId = _tenantId,
                                ObjectId = _managedIdentities[environment].PrincipalId.Apply(x => x),
                                Permissions = new PermissionsArgs
                                {
                                    Keys = { "encrypt", "decrypt", "delete" },
                                    Secrets = { "get", "list", "set" }
                                }
                            }
                        },
                        TenantId = _tenantId
                    },
                    Tags = InfrastructureStandard.Tags
                });

                _keyVaults.Add(environment, keyVault);
            }
        }

        private void BuildSqlServerPrivateLink(
            string namePart, ResourceGroup resourceGroup, Server sqlServer, VirtualNetwork virtualNetwork, Subnet subnet)
        {
            var sqlPrivateDns = new PrivateZone(
                    $"{namePart}-sqldns",
                    new PrivateZoneArgs
                    {
                        PrivateZoneName = "privatelink.database.windows.net",
                        ResourceGroupName = resourceGroup.Name,
                        Location = "Global",
                        Tags = InfrastructureStandard.Tags
                    });

            var sqlPrivateDnsVNetLink = new VirtualNetworkLink(
                $"{namePart}-sqlvlnk",
                new VirtualNetworkLinkArgs
                {
                    PrivateZoneName = sqlPrivateDns.Name,
                    ResourceGroupName = resourceGroup.Name,
                    VirtualNetworkLinkName = $"{namePart}-sqlvlnk",
                    VirtualNetwork = new SubResourceArgs
                    {
                        Id = virtualNetwork.Id
                    },
                    RegistrationEnabled = false,
                    Location = "Global",
                    Tags = InfrastructureStandard.Tags
                },
                new CustomResourceOptions
                {
                    DependsOn = { sqlPrivateDns }
                });

            var sqlPrivateEndpoint = new PrivateEndpoint(
                $"{namePart}-sqlpep",
                new PrivateEndpointArgs
                {
                    PrivateEndpointName = $"{namePart}-sqlpep",
                    ResourceGroupName = resourceGroup.Name,
                    Subnet = new Pulumi.AzureNative.Network.Inputs.SubnetArgs
                    {
                        Id = subnet.Id
                    },
                    PrivateLinkServiceConnections = new[]
                    {
                        new PrivateLinkServiceConnectionArgs
                        {
                            Name = "sql",
                            GroupIds = new []{ "sqlServer" },
                            PrivateLinkServiceId = sqlServer.Id,
                            PrivateLinkServiceConnectionState = new Pulumi.AzureNative.Network.Inputs.PrivateLinkServiceConnectionStateArgs
                            {
                                ActionsRequired = "None",
                                Description = "Auto approved",
                                Status = "Approved"
                            }
                        }
                    },
                    Tags = InfrastructureStandard.Tags
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = { sqlServer, virtualNetwork, subnet }
                });

            var privateDnsZoneGroup = new PrivateDnsZoneGroup(
                $"{namePart}-sqldnsgroup",
                new PrivateDnsZoneGroupArgs
                {
                    PrivateDnsZoneGroupName = $"{namePart}-sqldnsgroup",
                    PrivateEndpointName = sqlPrivateEndpoint.Name,
                    ResourceGroupName = resourceGroup.Name,
                    Name = sqlPrivateDns.Name,
                    PrivateDnsZoneConfigs = new[]
                    {
                        new PrivateDnsZoneConfigArgs
                        {
                            Name = sqlPrivateDns.Name,
                            PrivateDnsZoneId = sqlPrivateDns.Id
                        }
                    }
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = { sqlPrivateEndpoint, sqlPrivateDns }
                });
        }

        private void BuildJwtSigningKeys(
            string namePart, ResourceGroup resourceGroup, IEnumerable<string> environments)
        {
            foreach (var environment in environments)
            {
                var jwtSigningKey = new RandomString(
                    $"{namePart}-{environment}-jwt-signingkey",
                    new RandomStringArgs
                    {
                        Length = 128,
                        Special = false
                    }).Result;

                var secretJwtSigningKey = new Secret(
                    $"{namePart}-{environment}-secret-jwt-signingkey",
                    new SecretArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        VaultName = _keyVaults[environment].Name,
                        SecretName = PlatformConstants.Jwt.SigningKeyName.Replace(":", "--"), // for valid name
                        Properties = new SecretPropertiesArgs
                        {
                            ContentType = "text/plain",
                            Value = jwtSigningKey
                        },
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        Parent = _keyVaults[_stackName],
                        DependsOn = _keyVaults[_stackName]
                    });

                _jwtSigningKeys.Add(environment, jwtSigningKey);
            }
        }

        private void BuildWebPrimaryEnvironment(string namePart, ResourceGroup resourceGroup, Output<GetResourceGroupResult> coreResourceGroup, string environment,
            IEnumerable<string> secondaryEnvironments)
        {
            var staticSite = new StaticSite($"{namePart}-{environment}-swa", new StaticSiteArgs
            {
                Location = resourceGroup.Location,
                ResourceGroupName = resourceGroup.Name,
                BuildProperties = new StaticSiteBuildPropertiesArgs
                {
                    ApiLocation = "api",
                    AppLocation = "src",
                    AppArtifactLocation = "build",
                },
                Identity = new ManagedServiceIdentityArgs
                {
                    Type = ManagedServiceIdentityType.UserAssigned,
                    UserAssignedIdentities = Helpers.GetManagedIdentity(_managedIdentities[environment])
                },
                Sku = new SkuDescriptionArgs
                {
                    Name = "Standard",
                    Tier = "Standard",
                },
                Tags = InfrastructureStandard.Tags,
            },
                new CustomResourceOptions
                {
                    DependsOn = { resourceGroup, _managedIdentities[environment] }
                });

            _webEndpoints.Add(environment, staticSite.DefaultHostname);

            // With static websites we don't need to make multiple instances to represent multiple environments
            // but we need to attach each additional environment to the API slot which will represent the
            // secondary API environment. Environments for static websites can be made adhoc, so if the 
            // pulumi configuration is not kept up-to-date, there can be drift here. 
            foreach (var env in secondaryEnvironments)
            {
                _webEndpoints.Add(env, Output.Tuple(staticSite.DefaultHostname, resourceGroup.Location)
                    .Apply(x => x.Item1.ToSlotUrlWithEnvironment(env, x.Item2)));
            }
        }

        private void BuildWebSecondaryEnvironments(
            string namePart, ResourceGroup resourceGroup, IEnumerable<string> environments)
        {
            foreach (var environment in environments)
            {
                if (_stackName != environment)
                    Log.Debug($"Building secondary environment {environment}");

                var staticSite = new StaticSite($"{namePart}-{environment}-swa", new StaticSiteArgs
                {
                    Location = resourceGroup.Location,
                    ResourceGroupName = resourceGroup.Name,
                    BuildProperties = new StaticSiteBuildPropertiesArgs
                    {
                        ApiLocation = "api",
                        AppLocation = "src",
                        AppArtifactLocation = "build",
                    },
                    Identity = new ManagedServiceIdentityArgs
                    {
                        Type = ManagedServiceIdentityType.UserAssigned,
                        UserAssignedIdentities = Helpers.GetManagedIdentity(_managedIdentities[environment])
                    },
                    Sku = new SkuDescriptionArgs
                    {
                        Name = "Standard",
                        Tier = "Standard",
                    },
                    StagingEnvironmentPolicy = StagingEnvironmentPolicy.Enabled,
                    Tags = InfrastructureStandard.Tags,
                });

                _webEndpoints.Add(environment, staticSite.DefaultHostname);
            }
        }

        private void BuildApiSecondaryEnvironments(
            string namePart, ResourceGroup resourceGroup,
            Input<object> appServicePLan, WebApp appService, Subnet subnet,
            IEnumerable<string> environments,
            InputMap<string> appSettings, Input<string> sqlServerName,
            string sqlUsername, Input<string> sqlPassword, Dictionary<string, StackAppRegistration> stackAppRegistrations)
        {
            var slotAppSettings = new InputMap<string>();
            slotAppSettings.AddRange(appSettings);

            foreach (var environment in environments)
            {
                _knownWebClients.Add(_webEndpoints[environment].Apply(x => $"https://{x}"));

                slotAppSettings.AddRange(new InputMap<string>()
                {
                    {
                        "AppStorage__AccountName",
                        _storageAccountNames[environment]
                    },
                    {
                        "AppStorage__AccountKey",
                        Output
                            .Tuple(resourceGroup.Name, _storageAccountNames[environment])
                            .Apply(x =>
                                Output.Create(Helpers.GetStorageAccountPrimaryKey(x.Item1, x.Item2)))
                    },
                    {
                        "AppStorage__TenantId",
                        _tenantId
                    },
                    {
                        "AppStorage__UserAssignedId",
                        _managedIdentities[environment].ClientId
                    },
                    {
                        "AppStorage__ClientId",
                        stackAppRegistrations[environment].Application.ApplicationId
                    },
                    {
                        "AppStorage__ClientSecret",
                        stackAppRegistrations[environment].ApplicationPassword.Value
                    },
                    {
                        "AzureAd__ResourceId",
                        stackAppRegistrations[environment]
                            .Application.ApplicationId.Apply(x => $"api://tws-{environment}-app")
                    },
                    {
                        "AzureAdClient__ResourceId",
                        stackAppRegistrations[environment]
                            .Application.ApplicationId.Apply(x => $"api://tws-{environment}-app/.default")
                    },
                    {
                        "AzureAdClient__ClientId",
                        stackAppRegistrations[environment].Worker.ApplicationId
                    },
                    {
                        "AzureAdClient__ClientSecret",
                        stackAppRegistrations[environment].WorkerPassword.Value
                    },
                    {
                        "AzureAdClient__TenantId",
                        _tenantId
                    },
                    {
                        "APPINSIGHTS_INSTRUMENTATIONKEY",
                        _applicationInsights[environment].InstrumentationKey
                    },
                    {
                        "APPLICATIONINSIGHTS_CONNECTION_STRING",
                        _applicationInsights[environment].InstrumentationKey
                            .Apply(key => $"InstrumentationKey={key}")
                    },
                    {
                        "ApplicationInsightsAgent_EXTENSION_VERSION",
                        "~2"
                    },
                    {
                        "KeyVault__UserAssignedId",
                        _managedIdentities[environment].ClientId
                    },
                    {
                        "KeyVault__VaultUri",
                        Output.Format($"https://{_keyVaults[environment].Name}.vault.azure.net")
                    },
                    {
                        "TwsMssql__ConnectionString",
                        Helpers.GetMssqlConnectionString(sqlUsername, sqlPassword,
                            sqlServerName,
                            _databases[environment])
                    },
                    {
                        "BlobStorage__ConnectionString",
                        Helpers.GetStorageAccountConnectionString(resourceGroup.Name,
                            _storageAccountNames[environment])
                    },
                    // Required because Azure Web Jobs configuration is way too opinionated
                    // Used with the VMs that execute webjobs
                    {
                        "ConnectionStrings__AzureWebJobsStorage",
                        Helpers.GetStorageAccountConnectionString(resourceGroup.Name,
                            _storageAccountNames[environment])
                    },
                    {
                        "tScreenClient__BaseUrl",
                        _webEndpoints[_stackName].Apply(x => $"https://{x}")
                    }
                });

                var environmentSlot = new Pulumi.AzureNative.Web.WebAppSlot(
                    $"{namePart}-{environment}-app-slot",
                    new WebAppSlotArgs
                    {
                        Name = appService.Name,
                        Slot = environment,
                        Location = resourceGroup.Location,
                        ResourceGroupName = resourceGroup.Name,
                        ServerFarmId = Output.Format($"{appServicePLan}"),
                        HttpsOnly = true,
                        SiteConfig = new SiteConfigArgs
                        {
                            // 64 Bit not available on "Free" and "Shared" tiers only on "Basic," "Standard" and above.
                            // Change app service plan to enable
                            Use32BitWorkerProcess = false,
                            // az webapp list-runtimes --os-type linux
                            LinuxFxVersion = "DOTNETCORE|6.0",
                            MinTlsVersion = SupportedTlsVersions.SupportedTlsVersions_1_2,
                            FtpsState = FtpsState.FtpsOnly,
                            // Only support for the Basic and Standard plans disabled for now.
                            AlwaysOn = true,
                        },
                        Identity = new ManagedServiceIdentityArgs
                        {
                            Type = ManagedServiceIdentityType.UserAssigned,
                            UserAssignedIdentities = Helpers.GetManagedIdentity(_managedIdentities[environment])
                        },
                        VirtualNetworkSubnetId = subnet.Id,
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        DependsOn = { appService, subnet }
                    });

                // var slotVNetConnection = new WebAppSwiftVirtualNetworkConnectionSlot(
                //     $"{namePart}-{environment}-slot-vnetconn",
                //     new WebAppSwiftVirtualNetworkConnectionSlotArgs
                //     {
                //         Name = slot.Name,
                //         Slot = slot.Name,
                //         ResourceGroupName = resourceGroup.Name,
                //         SubnetResourceId = subnet.Id,
                //         SwiftSupported = true
                //     }, 
                //     new CustomResourceOptions 
                //     { 
                //         Parent = slot,
                //         DependsOn = { slot, subnet }
                //     });

                var index = 0;
                foreach (var url in _knownWebClients)
                    slotAppSettings.Add($"KnownWebClients__Domains__{index++}", url);

                slotAppSettings.AddRange(new InputMap<string>
                {
                    { "Jwt__Audience", "Any" },
                    { "Jwt__Authority", environmentSlot.DefaultHostName.Apply(x => $"https://{x}") },
                    { "tScreenApi__BaseUrl", environmentSlot.DefaultHostName.Apply(x => $"https://{x}")}
                });

                var environmentSlotSettings = new WebAppApplicationSettingsSlot(
                    $"{namePart}-{environment}-app-slot-settings",
                    new WebAppApplicationSettingsSlotArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        Name = appService.Name,
                        Slot = environment,
                        Properties = slotAppSettings
                    },
                    new CustomResourceOptions
                    {
                        Parent = environmentSlot,
                        DependsOn = { environmentSlot }
                    });

                _apiEndpoints.Add(environment, environmentSlot.DefaultHostName);
            }
        }

        private void BuildStorageEnvironments(
            string namePart, ResourceGroup resourceGroup, Server sqlServer,
            IReadOnlyDictionary<string, ResourceGroup> logResourceGroups, IReadOnlyDictionary<string, StorageAccount> logStorageAccounts,
            IEnumerable<string> environments)
        {
            foreach (var environment in environments)
            {
                var database = new Database($"{namePart}-{environment}-db", new DatabaseArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    Location = resourceGroup.Location,
                    DatabaseName = $"{namePart}-{environment}".ToPascalCase(),
                    ServerName = sqlServer.Name,
                    Sku = new Pulumi.AzureNative.Sql.Inputs.SkuArgs
                    {
                        Name = environment == "prd" ? "P1" : "S0"
                    },
                    Tags = InfrastructureStandard.Tags,
                },
                    new CustomResourceOptions
                    {
                        Parent = sqlServer,
                        DependsOn = sqlServer,
                        Protect = true
                    });

                if (environment == "prd")
                {
                    var storageAccountKeys = ListStorageAccountKeys
                        .Invoke(new ListStorageAccountKeysInvokeArgs
                        {
                            ResourceGroupName = logResourceGroups[environment].Name,
                            AccountName = logStorageAccounts[environment].Name
                        });

                    var databaseSecurityAlertPolicy = new DatabaseSecurityAlertPolicy(
                        $"{namePart}-{environment}-db-dsap",
                        new DatabaseSecurityAlertPolicyArgs()
                        {
                            ResourceGroupName = resourceGroup.Name,
                            ServerName = sqlServer.Name,
                            DatabaseName = database.Name,
                            SecurityAlertPolicyName = "Default",
                            State = SecurityAlertsPolicyState.Enabled,
                            RetentionDays = InfrastructureStandard.LogRetentionInDays,
                            StorageEndpoint = logStorageAccounts[environment].PrimaryEndpoints.Apply(x => x.Blob),
                            StorageAccountAccessKey = storageAccountKeys.Apply(x => x.Keys[0].Value)
                        },
                        new CustomResourceOptions
                        {
                            DependsOn = { sqlServer, database }
                        });
                }

                var newId = new RandomUuid($"{namePart}-{environment}-sa-0001")
                    .ToString().ToSha256().ToHex()[0..8];

                var envStorageAccountName = $"{namePart}-{environment}-sa-0001-{newId}";
                var envStorageAccount = new StorageAccount(
                    envStorageAccountName,
                    new StorageAccountArgs
                    {
                        AccountName = envStorageAccountName.ToAzureStorageName(),
                        ResourceGroupName = resourceGroup.Name,
                        Sku = new Pulumi.AzureNative.Storage.Inputs.SkuArgs
                        {
                            Name = Pulumi.AzureNative.Storage.SkuName.Standard_LRS
                        },
                        Kind = Kind.StorageV2,
                        Identity = new IdentityArgs
                        {
                            Type = Pulumi.AzureNative.Storage.IdentityType.UserAssigned,
                            UserAssignedIdentities = Helpers.GetManagedIdentity(_managedIdentities[environment])
                        },
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        // Protect = true
                    });

                _databases.Add(environment, database.Name);
                _storageAccountNames.Add(environment, envStorageAccount.Name);

                RbacAuthorization
                    .AssignManagedIdentityPermissionsToStorageAccountRoles(
                        $"{namePart}-{environment}-",
                        _tenantId,
                        _managedIdentities[environment],
                        envStorageAccount);
            }
        }

        private (
            Dictionary<string, ResourceGroup> resourceGroups,
            Dictionary<string, StorageAccount> workspaceStorageAccounts,
            Dictionary<string, Workspace> workspaces)
            BuildLogsEnvironments(string namePart, ResourceGroup resourceGroup, string location,
                    IEnumerable<string> environments)
        {
            var workspaceResourceGroups = new Dictionary<string, ResourceGroup>();
            var workspaceStorageAccounts = new Dictionary<string, StorageAccount>();
            var workspaces = new Dictionary<string, Workspace>();

            foreach (var environment in environments)
            {
                var resourceGroupLogs = new ResourceGroup(
                    $"tscreen-logs-{environment}-{location}-rg",
                    new ResourceGroupArgs
                    {
                        Location = location,
                        Tags = InfrastructureStandard.Tags
                    });

                var workspace = new Pulumi.AzureNative.OperationalInsights.Workspace(
                    $"tscreen-{environment}-{location}-law",
                    new WorkspaceArgs
                    {
                        ResourceGroupName = resourceGroupLogs.Name,
                        Location = resourceGroupLogs.Location,
                        RetentionInDays = InfrastructureStandard.LogRetentionInDays,
                        Sku = new Pulumi.AzureNative.OperationalInsights.Inputs.WorkspaceSkuArgs
                        {
                            Name = WorkspaceSkuNameEnum.PerGB2018
                        },
                        Tags = InfrastructureStandard.Tags
                    });

                var storageAccountName = $"{namePart.Replace("-", "")}{environment}logssa";
                var storageAccount = new StorageAccount(
                    storageAccountName,
                    new StorageAccountArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        Sku = new Pulumi.AzureNative.Storage.Inputs.SkuArgs
                        {
                            Name = Pulumi.AzureNative.Storage.SkuName.Standard_LRS
                        },
                        Kind = Pulumi.AzureNative.Storage.Kind.StorageV2,
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        Parent = workspace,
                        DependsOn = { resourceGroupLogs }
                    });

                var applicationInsights = new Component(
                    $"{namePart}-{environment}-{location}-app-ai",
                    new ComponentArgs
                    {
                        Location = resourceGroupLogs.Location,
                        ResourceGroupName = resourceGroupLogs.Name,
                        Kind = "Web",
                        ApplicationType = "Web",
                        RetentionInDays = InfrastructureStandard.LogRetentionInDays,
                        ImmediatePurgeDataOn30Days = InfrastructureStandard.ImmediatelyPurgeAfterRetentionPeriod,
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        Parent = workspace,
                        DependsOn = { resourceGroupLogs, workspace }
                    });

                workspaceResourceGroups.Add(environment, resourceGroup);
                workspaceStorageAccounts.Add(environment, storageAccount);
                workspaces.Add(environment, workspace);
                _applicationInsights.Add(environment, applicationInsights);
            }

            return (workspaceResourceGroups, workspaceStorageAccounts, workspaces);
        }

        private Dictionary<string, VmMachine> BuildWorkerVMs(string namePart, ResourceGroup resourceGroup, Subnet subnet,
            Dictionary<string, UserAssignedIdentity> managedIdentities, IEnumerable<string> environments)
        {
            var vmMachines = new Dictionary<string, VmMachine>();

            foreach (var environment in environments)
            {
                var adminKey = new PrivateKey($"{namePart}-{environment}-ssh-key-admin", new PrivateKeyArgs
                {
                    Algorithm = "RSA",
                    RsaBits = 2048
                });

                var adminPublicSshKey = new SshPublicKey(
                    $"{namePart}-{environment}-sshpk-admin",
                    new()
                    {
                        PublicKey = adminKey.PublicKeyOpenssh.Apply(x => x.Trim()),
                        SshPublicKeyName = $"tws-{environment}-worker-admin-pem",
                        ResourceGroupName = resourceGroup.Name,
                        Tags = InfrastructureStandard.Tags
                    });

                var virtualMachineNsg = new NetworkSecurityGroup(
                    $"{namePart}-{environment}-worker-vm0001",
                    new NetworkSecurityGroupArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        SecurityRules = new[]
                        {
                            new SecurityRuleArgs
                            {
                                Name = "SSH Allow",
                                SourcePortRange = "*",
                                DestinationPortRange = "22",
                                SourceAddressPrefix = "*",
                                DestinationAddressPrefix = "*",
                                Protocol = SecurityRuleProtocol.Tcp,
                                Access = SecurityRuleAccess.Allow,
                                Direction = SecurityRuleDirection.Inbound,
                                Priority = 100
                            },
                            new SecurityRuleArgs
                            {
                                Name = "Supervisor Allow",
                                Description = "Used with the supervisor web interface",
                                SourcePortRange = "*",
                                DestinationPortRange = "9001",
                                SourceAddressPrefix = "*",
                                DestinationAddressPrefix = "*",
                                Protocol = SecurityRuleProtocol.Tcp,
                                Access = SecurityRuleAccess.Allow,
                                Direction = SecurityRuleDirection.Inbound,
                                Priority = 310
                            }
                        },
                        Tags = InfrastructureStandard.Tags
                    });

                var networkInterface = new NetworkInterface(
                    $"{namePart}-{environment}-vm-nic",
                    new NetworkInterfaceArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        IpConfigurations = new NetworkInterfaceIPConfigurationArgs
                        {
                            Name = "ipconfig1",
                            PrivateIPAddressVersion = IPVersion.IPv4,
                            PrivateIPAllocationMethod = IPAllocationMethod.Dynamic,
                            Subnet = new Pulumi.AzureNative.Network.Inputs.SubnetArgs
                            {
                                Id = subnet.Id,
                            }
                        },
                        NetworkInterfaceName = $"vm-{environment}-worker-nic-0001",
                        NetworkSecurityGroup = new Pulumi.AzureNative.Network.Inputs.NetworkSecurityGroupArgs
                        {
                            Id = virtualMachineNsg.Id
                        },
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions()
                    {
                        DependsOn = { subnet }
                    });

                var adminUsername = "tws_worker";

                var virtualMachine = new VirtualMachine($"{namePart}-{environment}-worker-vm0001",
                    new VirtualMachineArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        HardwareProfile = new HardwareProfileArgs
                        {
                            VmSize = "Standard_B1s"
                        },
                        NetworkProfile = new NetworkProfileArgs
                        {
                            NetworkInterfaces = new[]
                            {
                                new NetworkInterfaceReferenceArgs
                                {
                                    Id = networkInterface.Id,
                                    Primary = true
                                }
                            }
                        },
                        OsProfile = new OSProfileArgs
                        {
                            ComputerName = $"tws-{environment}-worker-vm0001",
                            AdminUsername = adminUsername,
                            LinuxConfiguration = new LinuxConfigurationArgs
                            {
                                DisablePasswordAuthentication = true,
                                Ssh = new SshConfigurationArgs
                                {
                                    PublicKeys = new[]
                                    {
                                        new Pulumi.AzureNative.Compute.Inputs.SshPublicKeyArgs
                                        {
                                            KeyData = adminKey.PublicKeyOpenssh,
                                            Path = $"/home/{adminUsername}/.ssh/authorized_keys"
                                        }
                                    }
                                }
                            }
                        },
                        StorageProfile = new StorageProfileArgs
                        {
                            ImageReference = new ImageReferenceArgs
                            {
                                Offer = "0001-com-ubuntu-server-focal",
                                Publisher = "canonical",
                                Sku = "20_04-lts-gen2",
                                Version = "latest",
                            },
                            OsDisk = new OSDiskArgs
                            {
                                OsType = OperatingSystemTypes.Linux,
                                CreateOption = DiskCreateOptionTypes.FromImage,
                                Caching = CachingTypes.ReadWrite,
                                ManagedDisk = new ManagedDiskParametersArgs
                                {
                                    StorageAccountType = StorageAccountTypes.StandardSSD_LRS
                                },
                                DiskSizeGB = 30
                            }
                        },
                        Identity = new VirtualMachineIdentityArgs
                        {
                            Type = ResourceIdentityType.UserAssigned,
                            UserAssignedIdentities = Helpers.GetManagedIdentity(managedIdentities[environment])
                        },
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        DependsOn = { resourceGroup, networkInterface, subnet }
                    });

                var virtualMachineAadExtension = new VirtualMachineExtension(
                    $"{namePart}-{environment}-worker-vm0001-ext-aadssh",
                    new VirtualMachineExtensionArgs
                    {
                        ResourceGroupName = resourceGroup.Name,
                        VmName = virtualMachine.Name,
                        Publisher = "Microsoft.Azure.ActiveDirectory",
                        Type = "AADSSHLoginForLinux",
                        TypeHandlerVersion = "1.0",
                        AutoUpgradeMinorVersion = true,
                        Tags = InfrastructureStandard.Tags
                    },
                    new CustomResourceOptions
                    {
                        DependsOn = { virtualMachine }
                    });

                _vmMachineGroup.Add(environment, new VmMachineGroup
                {
                    Ip = networkInterface.IpConfigurations
                        .Apply(x => x[0].PrivateIPAddress ?? "0.0.0.0"),
                    Public = adminKey.PublicKeyOpenssh,
                    Private = adminKey.PrivateKeyOpenssh
                });

                vmMachines.Add(environment, new VmMachine
                {
                    VirtualMachine = virtualMachine,
                    NetworkInterface = networkInterface
                });

                BuildAzureDevOpsServiceConnection(namePart, environment, adminUsername, adminKey, networkInterface);
            }

            return vmMachines;
        }

        private static void BuildAzureDevOpsServiceConnection(
            string namePart, string environment, string adminUsername, PrivateKey? adminKey, NetworkInterface nic)
        {
            var project = Pulumi.AzureDevOps.GetProject.Invoke(new GetProjectInvokeArgs
            {
                Name = "tscreen",
                ProjectId = null,
            });

            var sshServiceEndpoint = new Pulumi.AzureDevOps.ServiceEndpointSsh(
                $"{namePart}-{environment}-vm-worker-devops-ssh-service-endpoint",
                new ServiceEndpointSshArgs()
                {
                    ProjectId = project.Apply(x => x.Id),
                    ServiceEndpointName = $"{namePart}-{environment}-vm-worker".ToPascalCase(),
                    Host = nic.IpConfigurations
                        .Apply(x => x[0].PrivateIPAddress ?? "0.0.0.0"),
                    Port = 22,
                    Username = adminUsername,
                    Description = $"Generated by Pulumi: ssh service connection used to access the {namePart} virtual machine",
                    PrivateKey = adminKey!.PrivateKeyOpenssh
                });
        }

        /**
         * Build the virtual machine which will be used for running Azure Devops
         * Pipelines. This is important for process that need to be built and deployed
         * into, but are connected to a private network
         * (not available without a tunnel or VPN connection)
         */
        private void BuildAzureDevopsPipelineVmss(string namePart, ResourceGroup resourceGroup,
            VirtualNetwork virtualNetwork)
        {
            var managedIdentity = new UserAssignedIdentity(
                $"{namePart}-devops-mi",
                new UserAssignedIdentityArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    Tags = InfrastructureStandard.Tags
                },
                new CustomResourceOptions
                {
                    DependsOn = { resourceGroup }
                });

            var devopsKey = new Pulumi.Tls.PrivateKey(
                $"{namePart}-devops-pem", new PrivateKeyArgs
                {
                    Algorithm = "RSA",
                    RsaBits = 2048
                });

            var adminPublicSshKey = new Pulumi.AzureNative.Compute.SshPublicKey(
                $"{namePart}--devops-sshpub-admin",
                new SshPublicKeyArgs
                {
                    PublicKey = devopsKey.PublicKeyOpenssh.Apply(x => x.Trim()),
                    SshPublicKeyName = "tws-devops-admin-pub",
                    ResourceGroupName = resourceGroup.Name,
                    Tags = InfrastructureStandard.Tags
                },
                new CustomResourceOptions
                {
                    Parent = devopsKey,
                    DependsOn = { resourceGroup, devopsKey }
                });

            var devopsSubnet = new Subnet(
                $"{namePart}-{_stackName}-vn-devops-sn",
                new SubnetArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    AddressPrefix = "10.0.6.0/24",
                    SubnetName = "tws-devops-sn",
                    VirtualNetworkName = virtualNetwork.Name
                },
                new CustomResourceOptions
                {
                    Parent = virtualNetwork,
                    DependsOn = { virtualNetwork }
                });

            var devopsNsg = new Pulumi.AzureNative.Network.NetworkSecurityGroup(
                $"{namePart}-devops-vm-nic-nsg", new()
                {
                    ResourceGroupName = resourceGroup.Name,
                    Tags = InfrastructureStandard.Tags
                });

            // var devopsVmssTags = new Dictionary<string, string>();
            // devopsVmssTags.Add("__AzureDevOpsElasticPool", "Private Devops");
            // devopsVmssTags.Add("__AzureDevOpsElasticPoolTimeStamp", "11/15/2022 04:00:00 PM");

            var devopsVmss = new Pulumi.AzureNative.Compute.VirtualMachineScaleSet(
                $"{namePart}-devops-vmss", new VirtualMachineScaleSetArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    OrchestrationMode = OrchestrationMode.Uniform,
                    SinglePlacementGroup = false,
                    Sku = new Pulumi.AzureNative.Compute.Inputs.SkuArgs
                    {
                        Capacity = 1,
                        Name = "Standard_B1s",
                        Tier = "Standard",
                    },
                    UpgradePolicy = new UpgradePolicyArgs
                    {
                        Mode = UpgradeMode.Manual
                    },
                    ScaleInPolicy = new ScaleInPolicyArgs
                    {
                        Rules =
                        {
                            VirtualMachineScaleSetScaleInRules.Default
                        }
                    },
                    VirtualMachineProfile = new Pulumi.AzureNative.Compute.Inputs.VirtualMachineScaleSetVMProfileArgs
                    {
                        OsProfile = new Pulumi.AzureNative.Compute.Inputs.VirtualMachineScaleSetOSProfileArgs
                        {
                            ComputerNamePrefix = "tws-devops-vm",
                            AdminUsername = "devops",
                            LinuxConfiguration = new Pulumi.AzureNative.Compute.Inputs.LinuxConfigurationArgs
                            {
                                DisablePasswordAuthentication = true,
                                Ssh = new Pulumi.AzureNative.Compute.Inputs.SshConfigurationArgs
                                {
                                    PublicKeys = new[]
                                    {
                                        new Pulumi.AzureNative.Compute.Inputs.SshPublicKeyArgs
                                        {
                                            KeyData = devopsKey.PublicKeyOpenssh.Apply(x => $"{x} generated-in-pulumi"),
                                            Path = "/home/devops/.ssh/authorized_keys"
                                        }
                                    }
                                },
                                ProvisionVMAgent = true,
                            }
                        },
                        StorageProfile = new VirtualMachineScaleSetStorageProfileArgs
                        {
                            OsDisk = new VirtualMachineScaleSetOSDiskArgs
                            {
                                OsType = OperatingSystemTypes.Linux,
                                CreateOption = DiskCreateOptionTypes.FromImage,
                                Caching = CachingTypes.ReadWrite,
                                ManagedDisk = new VirtualMachineScaleSetManagedDiskParametersArgs
                                {
                                    StorageAccountType = StorageAccountTypes.StandardSSD_LRS
                                },
                                DiskSizeGB = 30
                            },
                            ImageReference = new ImageReferenceArgs
                            {
                                Publisher = "canonical",
                                Offer = "0001-com-ubuntu-server-focal",
                                Sku = "20_04-lts-gen2",
                                Version = "latest",
                            }
                        },
                        NetworkProfile = new VirtualMachineScaleSetNetworkProfileArgs
                        {
                            NetworkInterfaceConfigurations = new[]
                            {

                                new VirtualMachineScaleSetNetworkConfigurationArgs
                                {
                                    Name = $"{namePart}-devops-mv-nic01",
                                    Primary = true,
                                    EnableAcceleratedNetworking = false,
                                    NetworkSecurityGroup = new Pulumi.AzureNative.Compute.Inputs.SubResourceArgs
                                    {
                                        Id = devopsNsg.Id
                                    },
                                    EnableIPForwarding = true,
                                    IpConfigurations = new[]
                                    {
                                        new VirtualMachineScaleSetIPConfigurationArgs
                                        {
                                            Name = $"{namePart}-devops-mv-nic01-default-config",
                                            Primary = true,
                                            Subnet = new ApiEntityReferenceArgs
                                            {
                                                Id = devopsSubnet.Id
                                            },
                                            PrivateIPAddressVersion = Pulumi.AzureNative.Compute.IPVersion.IPv4
                                        }
                                    }
                                }
                            }
                        }
                    },
                    Identity = new VirtualMachineScaleSetIdentityArgs
                    {
                        Type = ResourceIdentityType.UserAssigned,
                        UserAssignedIdentities = Helpers.GetManagedIdentity(managedIdentity)
                    },
                    Tags = InfrastructureStandard.Tags,
                    DoNotRunExtensionsOnOverprovisionedVMs = false,
                    Overprovision = false,
                    PlatformFaultDomainCount = 1
                },
                new CustomResourceOptions
                {
                    DependsOn = { resourceGroup, virtualNetwork, managedIdentity }
                });

            var devopsVmssAadExtension = new VirtualMachineScaleSetExtension(
                $"{namePart}-devops-vmss-ext-aad",
                new Pulumi.AzureNative.Compute.VirtualMachineScaleSetExtensionArgs
                {
                    ResourceGroupName = resourceGroup.Name,
                    VmScaleSetName = devopsVmss.Name,
                    Type = "AADSSHLoginForLinux",
                    Publisher = "Microsoft.Azure.ActiveDirectory",
                    AutoUpgradeMinorVersion = true,
                    TypeHandlerVersion = "1.0",
                },
                new CustomResourceOptions
                {
                    Parent = devopsVmss,
                    DependsOn = { resourceGroup, devopsVmss }
                });

            // var devopsVmssAzdExtension = new VirtualMachineScaleSetExtension(
            //     $"{namePart}-devops-vmss-ext-azd",
            //     new Pulumi.AzureNative.Compute.VirtualMachineScaleSetExtensionArgs
            //     {
            //         ResourceGroupName = resourceGroup.Name,
            //         VmScaleSetName = devopsVmss.Name,
            //         Name = "TeamServicesAgentLinux",
            //         Type = "TeamServicesAgentLinux",
            //         Publisher = "Microsoft.VisualStudio.Services",
            //         AutoUpgradeMinorVersion = true,
            //         TypeHandlerVersion = "1.23",
            //         Settings = new Dictionary<string, string>{
            //             {"isPipelineAgent", "true"},
            //             {"agentFolder", "/agent"},
            //             {"agentDownloadUrl", "https://vstsagentpackage.azureedge.net/agent/2.213.2/vsts-agent-linux-x64-2.213.2.tar.gz"},
            //             {"enableScriptDownloadUrl", "https://vstsagenttools.blob.core.windows.net/tools/ElasticPools/Linux/14/enableagent.sh"},
            //         }
            //     },
            //     new CustomResourceOptions
            //     {
            //         Parent = devopsVmss,
            //         DependsOn = { resourceGroup, devopsVmss }
            //     });
        }

        [Output(OutputNames.AppSqlSeverName)]
        public Output<string> AppSqlServerName { get; set; }

        [Output(OutputNames.ApiEndpoints)]
        public Output<ImmutableDictionary<string, object>> ApiEndpoints { get; set; }

        [Output(OutputNames.SpaEndpoints)]
        public Output<ImmutableDictionary<string, object>> SpaEndpoints { get; set; }

        [Output("jwtSigningKeys")]
        public Output<ImmutableDictionary<string, object>> JwtSigningKeys { get; set; }

        [Output(OutputNames.VmWorkers)]
        public Output<ImmutableDictionary<string, object>> VmWorkers { get; set; }


    }
}
