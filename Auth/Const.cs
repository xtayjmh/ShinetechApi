namespace API.Auth
{
    public class Const
    {
        public static string AuthHeaderUser { get; internal set; } = "user";
        public static string TokenSecurityKey { get; internal set; } = "TokenSecurityKey";
        public static string AesEncryptKey { get; internal set; } = "AesEncryptKey";
        public static string TokenCanRefreshWithinMinutes { get; internal set; } = "TokenCanRefreshWithinMinutes";
        public static string TokenExpireMinutes { get; internal set; } = "TokenExpireMinutes";
    }
}