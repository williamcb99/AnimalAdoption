using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly IConfiguration _configuration;

    private Dictionary<string, string> _whitelist = new();

    public AuthenticationController(JwtTokenService jwtTokenService, IConfiguration configuration)
    {
        _jwtTokenService = jwtTokenService;
        _configuration = configuration;
        _whitelist = _configuration.GetSection("Authentication:Whitelist").Get<Dictionary<string, string>>() ?? new Dictionary<string, string>();
    }

    [HttpPost]
    [EndpointDescription("Authenticate a user and receive a JWT token.")]
    public ActionResult<AuthenticationResponseDto> Authenticate([FromBody] AuthenticationRequestDto auth)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!_whitelist.TryGetValue(auth.Username, out var storedPassword) || auth.Password != storedPassword)
            return Unauthorized("Invalid username or password.");

        var token = _jwtTokenService.GenerateToken(auth.Username);
        
        return Ok(new AuthenticationResponseDto { Token = token.Token, IssuedAt = token.IssuedAt, Expiration = token.Expiration });
    }
}