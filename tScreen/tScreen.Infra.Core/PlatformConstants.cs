namespace tScreen.Infra.Core
{
    public static class PlatformConstants
    {
        public static class Jwt
        {
            public const string SigningKeyName = "Jwt:SigningKey";
        }

        public static class ConnectionStringNames
        {
            public const string Mssql = "TwsMssql:ConnectionString";
            public const string Storage = "BlobStorage:ConnectionString";
        }
    }
}