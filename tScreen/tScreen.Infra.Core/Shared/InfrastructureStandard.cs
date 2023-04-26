using System.Collections.Generic;
using Pulumi;

namespace tScreen.Infra.Core.Shared
{
    public static class InfrastructureStandard
    {
        public const int LogRetentionInDays = 30;
        public const bool ImmediatelyPurgeAfterRetentionPeriod = true;

        public static Dictionary<string, string> Tags = new Dictionary<string, string>
        {
            { "App", "tScreen"},
            { "Company", "FuturesThrive" },
            { "Environment", Pulumi.Deployment.Instance.StackName },
            { "Team", "DevOps" },
            { "Department", "IT" },
            { "CostCenter", "IT" },
            { "ManagedBy", "blint@agrinhealth" },
            { "CountryCode", "USA" },
            { "InfraName", "Pulumi" },
            { "InfraType", "Generated"},
            { "InfraProject", Pulumi.Deployment.Instance.ProjectName },
            { "CommitHash", Utility.GetGitHashShort()}
        };
    }
}