using System.ComponentModel.DataAnnotations;

public class AuthenticationRequestDto
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}