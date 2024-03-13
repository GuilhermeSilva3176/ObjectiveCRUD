using API.Model;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Site.Models.Dtos.Users;

namespace Site.Interfaces;

public interface IUserNoAuthInterfaces
{
    [Post("/api/User/Create")]
    Task<ApiResponse<string>> RegisterAsync(UserRegisterDto dto);

    [Get("/api/User/Login")]
    Task<ApiResponse<string>> LoginAsync(UserLoginDto dto);
}
