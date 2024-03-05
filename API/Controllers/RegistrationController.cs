using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.RegistrationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class RegistrationController : ControllerBase
{
    private readonly AppDbContext _Db;
    private readonly ITokenService _TokenService;
    public RegistrationController(AppDbContext db, ITokenService tokenService) 
    {
        _Db = db;
        _TokenService = tokenService;
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateRegisDto dto)
    {

        if (_Db.Events.Find(dto.EventId) == null)
            return NotFound("This event does not exist");
        
        var user = _TokenService.GetUserByToken(User);

        var register = new RegistrationModel
        {
            Id = Guid.NewGuid(),
            EventId = dto.EventId,
            UserId = user.Id
        };       

        _Db.Registration.Add(register);
        await _Db.SaveChangesAsync();

        return Ok();
    }
    
    [Authorize]
    [HttpDelete("Delete")]
    public IActionResult Delete([FromBody] DeleteRegisDto dto)
    {
        var user = _TokenService.GetUserByToken(User);
        var regis = _Db.Registration.Find(dto.RegistrationId);
        
        if (regis == null || user.Id != regis.UserId) 
            return Unauthorized();

        _Db.Registration.Remove(regis);
        _Db.SaveChanges();

        return Ok("Registration has been deleted.");
    }
    [Authorize]
    [HttpGet("Get")]
    public IActionResult Get() 
    {
        var user = _TokenService.GetUserByToken(User);
        var regis = _Db.Registration
            .Where(r => r.UserId == user.Id)
            .Select(r => new RegistrationDto
            {
                Id = r.Id,
                UserName = r.Users.Name,
                EventName = r.Events.EventName,
                EventDate = r.Events.EventDate
            });

        return Ok(regis);
    }
    [Authorize]
    [HttpGet("GetById")]
    public IActionResult GetById([FromQuery] GetByIdDto dto) 
    {
        var user = _TokenService.GetUserByToken(User);
        var regis = _Db.Registration.Find(dto.Id);

        if (regis == null || user.Id != regis.UserId)
            return Unauthorized();

        var response = new RegistrationDto
        {
            Id = regis.Id,
            UserName = regis.Users.Name,
            EventName = regis.Events.EventName,
            EventDate = regis.Events.EventDate
        };

        return Ok(response);
    }
}