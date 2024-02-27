using API.Model;
using System.Security.Claims;

namespace API.Interfaces;

public interface ITokenService
{
    string GenerateToken(UsersModel user);

    UsersModel GetUserByToken(ClaimsPrincipal token);
}
