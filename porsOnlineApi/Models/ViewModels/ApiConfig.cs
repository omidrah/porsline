namespace porsOnlineApi.Models.ViewModels
{
    public class ApiConfig
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int TimeoutSeconds { get; set; } = 30;
        public int RetryCount { get; set; } = 3;
        public int RetryDelaySeconds { get; set; } = 5;
        public Dictionary<string, string> Headers { get; set; } = new();
        public AuthenticationType AuthType { get; set; } = AuthenticationType.None;
    }
}
