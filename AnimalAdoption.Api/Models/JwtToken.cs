public class JwtToken
{
    public string Token { get; set; } = string.Empty;
    public DateTime Expiration { get; set; }
    public DateTime IssuedAt { get; set; }
}