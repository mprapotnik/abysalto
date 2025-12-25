using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AbySalto.CartApi.Controllers;

[ApiController]
[Route("dev/token")]
public class DevTokenController : ControllerBase
{
    private readonly IConfiguration _cfg;
    private readonly IWebHostEnvironment _env;

    public DevTokenController(IConfiguration cfg, IWebHostEnvironment env)
    {
        _cfg = cfg;
        _env = env;
    }

    public record TokenRequest(string UserId);

    [HttpPost]
    public IActionResult Create([FromBody] TokenRequest req)
    {
        // - Sigurnosna napomena: ovo je samo za lokalni development
        if (!_env.IsDevelopment())
            return NotFound();

        var issuer = _cfg["Jwt:Issuer"]!;
        var audience = _cfg["Jwt:Audience"]!;
        var key = _cfg["Jwt:SigningKey"]!;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var creds = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, string.IsNullOrWhiteSpace(req.UserId) ? "dev-user" : req.UserId),
            new("scope", "cart:read cart:write")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        return Ok(new { access_token = jwt });
    }
}
