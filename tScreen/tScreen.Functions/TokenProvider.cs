using System;
using System.Threading.Tasks;
using Core.Settings.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client;

namespace tScreen.Functions
{
    public class TokenProvider : ITokenProvider
    {
        private readonly AzureAdClientSettings _azureAdClientSettings;
        private readonly ILogger<TokenProvider> _logger;

        public TokenProvider(AzureAdClientSettings clientSettings, ILogger<TokenProvider> logger)
        {
            _azureAdClientSettings = clientSettings;
            _logger = logger;
        }

        public async Task<AuthenticationResult> GetAzureAdToken()
        {
            var clientApp = ConfidentialClientApplication();

            try
            {
                return await clientApp
                    .AcquireTokenForClient(new[] { _azureAdClientSettings.ResourceId })
                    .ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogCritical("Cannot acquire token: {Message}", ex.Message);
                return null;
            }
        }

        private IConfidentialClientApplication ConfidentialClientApplication()
            => ConfidentialClientApplicationBuilder
                .Create(_azureAdClientSettings.ClientId)
                .WithClientSecret(_azureAdClientSettings.ClientSecret)
                .WithAuthority(_azureAdClientSettings.Authority)
                .Build();
    }
}