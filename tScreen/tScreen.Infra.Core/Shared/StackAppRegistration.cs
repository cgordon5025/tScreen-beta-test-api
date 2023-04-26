using Pulumi;
using Pulumi.AzureAD;
using Pulumi.AzureNative.ManagedIdentity;
using static System.Net.Mime.MediaTypeNames;
using System.Text.RegularExpressions;

namespace tScreen.Infra.Core.Shared;

public class StackAppRegistration
{
    public Application Application { get; init; } = null!;
    public ApplicationPassword ApplicationPassword { get; init; } = null!;
    public Application Worker { get; init; } = null!;
    public ApplicationPassword WorkerPassword { get; init; } = null!;
    public Group Group { get; init; } = null!;
    public ServicePrincipal ServicePrincipal { get; init; } = null!;
    public UserAssignedIdentity UserAssignedIdentity { get; init; } = null!;
}