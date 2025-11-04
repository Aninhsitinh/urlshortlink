namespace UrlShortener.Api.Models
{
    public class AuthResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;

        // Token data
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? ExpireAt { get; set; }
    }
}
