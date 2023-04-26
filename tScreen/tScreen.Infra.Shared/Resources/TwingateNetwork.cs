using System.Collections.Generic;
using System.Linq;
using Core.Text;
using Pulumi;
using Pulumi.AzureNative.ContainerInstance;
using Pulumi.AzureNative.ContainerInstance.Inputs;
using Pulumi.AzureNative.Network;
using Pulumi.AzureNative.Network.Inputs;
using Pulumi.AzureNative.Resources;
using TwingateLabs.Twingate;
using TwingateLabs.Twingate.Inputs;

namespace Shared.Resources;

public record TwingateNetworkResourceArgs : IComponentResourceTag
{
    public string DisplayName { get; init; } = null!;
    public ResourceGroup ResourceGroup { get; init; } = null!;
    public VirtualNetwork VirtualNetwork { get; init; } = null!;
    public string TenantUrl { get; set; } = null!;
    public Subnet Subnet { get; init; } = null!;
    public IEnumerable<TwingateNetworkResource>? Resources { get; init; }
    public int NumberOfConnectors { get; init; } = 0;
    public InputMap<string>? Tags { get; init; }
}

public class TwingateNetwork : ComponentResource
{
    private readonly TwingateRemoteNetwork _network;
    private readonly TwingateNetworkResourceArgs _resourceArgs;
    private static readonly List<TwingateNetworkResource> DefaultEmptyList = new();

    public TwingateNetwork(
        string networkName,
        TwingateNetworkResourceArgs resourceArgs,
        ComponentResourceOptions? options = null)
        : base($"agrinhealth:networking:{nameof(TwingateNetwork)}", networkName)
    {
        _resourceArgs = resourceArgs;

        _network = new TwingateRemoteNetwork(
            networkName,
            new TwingateRemoteNetworkArgs()
            {
                Name = _resourceArgs.DisplayName,
            }, new CustomResourceOptions
            {
                Parent = this
            });

        if (_resourceArgs.NumberOfConnectors > 0)
        {
            AddConnectors(networkName, _resourceArgs.NumberOfConnectors);
        }

        var twingateNetworkResources = _resourceArgs.Resources?.ToList() ?? DefaultEmptyList;
        if (!twingateNetworkResources.Any()) return;

        foreach (var resource in twingateNetworkResources)
        {
            var twingateResource = new TwingateResource(
                $"{networkName}-res-{resource.Name.ToKebabCase()}",
                new TwingateResourceArgs
                {
                    Name = resource.Label,
                    Address = resource.Address,
                    GroupIds = resource.GroupIds,
                    RemoteNetworkId = _network.Id,
                    Protocols = resource.Protocols ?? TwingateResourceProtocolsArgs.Empty
                },
                new CustomResourceOptions
                {
                    Parent = this
                });
        }
    }

    protected void AddConnectors(string name, int totalInstances = 1)
    {
        for (var i = 0; i < totalInstances; i++)
        {
            var increment = (i + 1).ToString().PadLeft(4, '0');
            var connectorName = $"{name}-connector-{increment}";
            var connector = new TwingateConnector(
                connectorName,
                new TwingateConnectorArgs
                {
                    // Name = connectorName,
                    RemoteNetworkId = _network.Id
                },
                new CustomResourceOptions
                {
                    Parent = this
                });

            var connectorTokens = new TwingateConnectorTokens(
                $"{name}-connector-token-{increment}",
                new TwingateConnectorTokensArgs
                {
                    ConnectorId = connector.Id
                },
                new CustomResourceOptions
                {
                    Parent = this
                });

            AddContainerInstance(connectorName, connectorTokens.AccessToken, connectorTokens.RefreshToken);
        }
    }

    private void AddContainerInstance(string name, Input<string> accessToken, Input<string> refreshToken)
    {
        const string imageName = "twingate/connector:1";

        var isolatedTags = new InputMap<string>();
        if (_resourceArgs.Tags is not null)
        {
            isolatedTags.Add(_resourceArgs.Tags);
        }

        isolatedTags.Add("Purpose", "TwingateConnector");

        var networkProfile = new NetworkProfile(
            $"{name}-networkprofile",
            new NetworkProfileArgs
            {
                ContainerNetworkInterfaceConfigurations = new ContainerNetworkInterfaceConfigurationArgs
                {
                    IpConfigurations = new[]
                    {
                        new IPConfigurationProfileArgs
                        {
                            Name = "ipConfig",
                            Subnet = new Pulumi.AzureNative.Network.Inputs.SubnetArgs
                            {
                                Id = _resourceArgs.Subnet.Id
                            }
                        }
                    },
                    Name = "eth1"
                },
                NetworkProfileName = $"{name}-networkprofile",
                ResourceGroupName = _resourceArgs.ResourceGroup.Name
            },
            new CustomResourceOptions
            {
                Parent = this
            });

        var containerGroup = new ContainerGroup(name, new ContainerGroupArgs
        {
            ResourceGroupName = _resourceArgs.ResourceGroup.Name,
            OsType = "Linux",
            Containers =
            {
                new ContainerArgs()
                {
                    Name = name.ToKebabCase(),
                    Image = imageName,
                    Ports = { new ContainerPortArgs() { Port = 80 } },
                    Resources = new ResourceRequirementsArgs
                    {
                        Requests = new ResourceRequestsArgs
                        {
                            Cpu = 1.0,
                            MemoryInGB = 1.0,
                        }
                    },
                    EnvironmentVariables =
                    {
                        new EnvironmentVariableArgs
                        {
                            Name = "TENANT_URL",
                            Value = _resourceArgs.TenantUrl
                        },
                        new EnvironmentVariableArgs
                        {
                            Name = "ACCESS_TOKEN",
                            Value = accessToken
                        },
                        new EnvironmentVariableArgs
                        {
                            Name = "REFRESH_TOKEN",
                            Value = refreshToken
                        },
                        new EnvironmentVariableArgs
                        {
                            Name = "TWINGATE_TIMESTAMP_FORMAT",
                            Value = "2"
                        }
                    }
                }
            },
            NetworkProfile = new ContainerGroupNetworkProfileArgs
            {
                Id = networkProfile.Id
            },
            IpAddress = new IpAddressArgs
            {
                Ports =
                {
                    new PortArgs
                    {
                        Port = 80,
                        Protocol = "Tcp"
                    }
                },
                Type = "Private"
            },
            RestartPolicy = "always",
            Tags = _resourceArgs?.Tags!
        },
        new CustomResourceOptions
        {
            Parent = this,
            DependsOn = _network
        });
    }
}
