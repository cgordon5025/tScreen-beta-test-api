export ASPNETCORE_ENVIRONMENT='Development'

# Application configuration
export APPINSIGHTS_INSTRUMENTATIONKEY=''
export APPLICATIONINSIGHTS_CONNECTION_STRING=''
export ApplicationInsightsAgent_EXTENSION_VERSION='~2'

export APPLICATION_HOST='Azure'
export APPLICATION_NAME='Tws.Api'

# Application Storage
export AppStorage__AccountName=''
export AppStorage__UserAssignedId=''

# Token configuration
export Jwt__Audience='Any'
export Jwt__Authority='https://'

# Primary key vault configuration
export KeyVault__UserAssignedId=''
export KeyVault__VaultUri='https://'

# Web clients
export KnownWebClients__Domains__0='http://localhost:3000'
export KnownWebClients__Domains__1=''

# DB Configuration
export TwsMssql__ConnectionString=''

export AzureAdClient__TenantId=''
export AzureAdClient__ResourceId='api://<client id here>.default'
export AzureAdClient__ClientId=''
export AzureAdClient__ClientSecret=''

export TweenScreenApi__BaseUrl='https://'

export ConnectionStrings__AzureWebJobsStorage=''

# For managed identities
# export ConnectionStrings__AzureWebJobsStorage__blobServiceUri="https://${AppStorage__AccountName}.blob.core.windows.net"
# export ConnectionStirngs__AzureWebJobsStorage__queueServiceUri="https://${AppStorage__AccountName}.queue.core.windows.net"
# export ConnectionStrings__AzureWebJobsStorage__credential="managedidentity"