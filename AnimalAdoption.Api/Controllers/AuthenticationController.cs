using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly JwtTokenService _jwtTokenService;
    private readonly IUserRepository _userRepository;
    public AuthenticationController(JwtTokenService jwtTokenService, IUserRepository userRepository)
    {
        _jwtTokenService = jwtTokenService;
        _userRepository = userRepository;
    }

    [HttpPost]
    [EndpointDescription("Authenticate a user and receive a JWT token.")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> Authenticate([FromBody] AuthenticationRequestDto auth)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = await _userRepository.GetUserByUsernameAsync(auth.Username);

        var hash = user?.PasswordHash ?? "$2a$12$d1F4sPK4vm8gOjE./Ji8suSH.ytMQlJxHn.jWVTOf.hZPEA4NQogG";

        if (!BCrypt.Net.BCrypt.Verify(auth.Password, hash))
            return Unauthorized(new ProblemDetails
            {
                Title = "Authentication Failed",
                Detail = "Invalid username or password.",
            });

        var token = _jwtTokenService.GenerateToken(auth.Username);
        
        return Ok(new AuthenticationResponseDto 
        { 
            Token = token.Token, 
            IssuedAt = token.IssuedAt, 
            Expiration = token.Expiration 
        });
    }
}