-- Used to create a new user which represents and Azure AD Managed Identity. Once the script is run
-- We can update our Database connection string to remove the password, something lke example below
-- Server=dbappserver.database.windows.net;Database=appdatabase;User Id=%ClientId%;Authentication=Active Directory Managed Identity;Connect Timeout=30

-- Create new AD user if they don't exist
IF NOT EXISTS (SELECT 1 FROM Sys.database_principals WHERE Name = '%ManagedIdentityName%')
    BEGIN
        CREATE USER ['%ManagedIdentityName%'] FROM EXTERNAL PROVIDER;
    END

-- Only add the roles if they don't exist (not associated with the managed identity)
IF IS_ROLEMEMBER('db_datareader', '%ManagedIdentityName%') = 0
    BEGIN
        ALTER ROLE db_datareader ADD MEMBER [%ManagedIdentityName%];
    END 
    
IF IS_ROLEMEMBER('db_datawriter', '%ManagedIdentityName%')
    BEGIN
        ALTER ROLE db_datawriter ADD MEMBER [%ManagedIdentityName%];
    END
    
IF IS_ROLEMEMBER('db_ddladmin', '%ManagedIdentityName%')
    BEGIN
        ALTER ROLE db_ddladmin ADD MEMBER [%ManagedIdentityName%];
    END

GO