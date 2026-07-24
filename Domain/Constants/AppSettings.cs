namespace Domain.Constants
{
    public static class AppSettings
    {
        public const string DbConnection = "DB_CONNECTION";

        public const string IdGeneratorSection = "IdGenerator";
        public const string IdGeneratorId = "IdGenerator:Id";

        public const string JwtSection = "JwtSettings";
        public const string JwtIssuer = "JwtSettings:Issuer";
        public const string JwtAudience = "JwtSettings:Audience";
        public const string JwtSecretKey = "JwtSettings:SecretKey";
        public const string AllowedOrigin = "Cors:AllowedOrigin";
    }
}
