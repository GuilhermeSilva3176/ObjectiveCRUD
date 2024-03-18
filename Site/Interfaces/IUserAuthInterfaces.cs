using API.Model;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Site.Models.Dtos.Users;

namespace Site.Interfaces;

public interface IUserAuthInterfaces
{
    [Post("/api/User/Create")]
    Task<ApiResponse<string>> RegisterAsync(UserRegisterDto dto);

    [Get("/api/User/Login")]
    Task<ApiResponse<string>> LoginAsync(UserLoginDto dto);
    
    [Put("/api/User/Update")]
    Task<ApiResponse<string>> ChangePasswordAsync(ChangePasswordDto dto, [Header("Authorization")] string token);

    [Delete("/api/User/Delete")]
    Task<ApiResponse<string>> DeleteUserAsync(DeleteUserDto dto);
}
