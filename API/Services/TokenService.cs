using API.Data;
using API.Interfaces;
using API.Model;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Services;

public class TokenService : ITokenService
{
    private readonly IConfiguration _Configuration;
    private readonly AppDbContext _Db;

    public TokenService(AppDbContext db, IConfiguration configuration)
    {
        _Db = db;
        _Configuration = configuration;
    }
    public string GenerateToken(UsersModel user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_Configuration["JwtConfig:Key"]!);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new Claim[]
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            }),
            Expires = DateTime.UtcNow.AddHours(1),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    public UsersModel GetUserByToken(ClaimsPrincipal token)
    {
        var userId = token.FindFirst(ClaimTypes.NameIdentifier)!.Value;
        return _Db.Users.Find(Guid.Parse(userId))!;
    }
}
