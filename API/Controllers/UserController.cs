using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
                 
namespace API.Controllers;
                 
[Route("api/[controller]")]
[ApiController]  
public class UserController : ControllerBase
{                
    private readonly AppDbContext _Db;
    private readonly IPasswordService _PasswordService;
    private readonly ITokenService _TokenService;

    public UserController(AppDbContext db, IPasswordService passwordService, ITokenService tokenService) 
    {            
        _Db = db;
        _PasswordService = passwordService;
        _TokenService = tokenService;
    }            
                 
    [HttpPost("Create")]   
    public async Task<IActionResult> Create([FromBody] CreateUserDto dto)
    {
        if (_Db.Users.Any(u => u.Email == dto.Email))
            return BadRequest("Credentials already in use.");

        var user = new UsersModel
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Email = dto.Email,
            Password = _PasswordService.GenerateHash(dto.Password),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _Db.Users.Add(user);
        await _Db.SaveChangesAsync();

        return Ok("Account Created Successfully");
    }
        
    [HttpGet("Login")]
    public IActionResult Login([FromQuery] LoginUserDto dto)
    {
        var user = _Db.Users.FirstOrDefault(u => u.Email == dto.Email);


        if (user == null || !_PasswordService.CheckHash(dto.Password, user.Password))
            return Unauthorized();

        var token = _TokenService.GenerateToken(user);

        return Ok(new { Token = token });
    }

    [Authorize]
    [HttpPut("Update")]
    public IActionResult Update([FromBody] UpdateDto dto)
    {
        var user = _TokenService.GetUserByToken(User);

        if(user == null || !_PasswordService.CheckHash(dto.Password, user.Password))
            return Unauthorized();

        user.Password = _PasswordService.GenerateHash(dto.NewPassword);
        user.UpdatedAt = DateTime.UtcNow;
        _Db.SaveChanges();

        return Ok("Password changed successfully");
    }

    [Authorize]
    [HttpDelete("Delete")]
    public IActionResult Delete([FromBody] DeleteDto dto)
    {
        var user = _TokenService.GetUserByToken(User);

        if(user == null || !_PasswordService.CheckHash(dto.Password, user.Password))
            return Unauthorized();

        _Db.Users.Remove(user);
        _Db.SaveChanges();

        return Ok("User deleted successfully");
    }
}
