namespace UserService.Core.Settings
{
    public class JWTSettings
    {
        public const string SectionName = "JWTSettings";

        public string SecretKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int AccessTokenExpiryMinutes { get; set; } = 15;
        public int RefreshTokenExpiryDays { get; set; } = 7;
    }
}
