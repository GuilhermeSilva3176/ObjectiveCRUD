using API.Model;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Site.Models.Dtos.Users;

namespace Site.Interfaces;

public interface IUserAuthInterfaces
{
    [Put("/api/User/Update")]
    Task<ApiResponse<string>> ChangePasswordAsync(string token, ChangePasswordDto dto);
}
