using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.EventsDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly AppDbContext _Db;
    private readonly ITokenService _TokenService;

    public EventsController(AppDbContext db, ITokenService tokenService)
    {
        _Db = db;
        _TokenService = tokenService;
    }

    [Authorize]
    [HttpPost("Create")]
    public async Task<IActionResult> Create([FromBody] CreateEventsDto dto)
    {
        var user = _TokenService.GetUserByToken(User);

        var registerEvent = new EventsModel
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            EventName = dto.EventName,
            EventDescription = dto.EventDescription,
            EventDate = dto.EventDate,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _Db.Events.Add(registerEvent);
        await _Db.SaveChangesAsync();

        return Ok("Event Created Successfully");
    }

    [Authorize]
    [HttpGet("Get")]
    public IActionResult Get()
    {
        var user = _TokenService.GetUserByToken(User);
        var eventGet = _Db.Events
            .Where(e => e.UserId == user.Id)
            .Select(r => new EventDto
            {
                EventId = r.Id,
                EventName = r.EventName,
                EventDescription = r.EventDescription,
                EventDate = r.EventDate,
            });
        
        return Ok(eventGet);
    }

    [Authorize]
    [HttpGet("GetById")]
    public IActionResult GetById([FromQuery] GetByIdDto dto)
    {
        var getEvent = _Db.Events.Find(dto.Id)!;

        if(getEvent == null)
            return BadRequest("Event doesn't exist");

        var response = new EventDto
        {
            EventId = getEvent.Id,
            EventName = getEvent.EventName,
            EventDescription = getEvent.EventDescription,
            EventDate = getEvent.EventDate,
        };

        return Ok(response);
    }

    [Authorize]
    [HttpPut("Update")]
    public IActionResult Update([FromBody] UpdateEventDto dto)
    {
        var updateEvent = _Db.Events.Find(dto.Id)!;

        if (updateEvent == null)
            return BadRequest("Event doesn't exist");

        updateEvent.EventName = dto.EventName;
        updateEvent.EventDescription = dto.EventDescription;
        updateEvent.EventDate = dto.EventDate;
        updateEvent.UpdatedAt = DateTime.UtcNow;
        _Db.SaveChanges();

        return Ok("Event updated successfully");
    }

    [Authorize]
    [HttpDelete("Delete")]
    public IActionResult Delete([FromBody] DeleteEventsDto dto)
    {
        var user = _TokenService.GetUserByToken(User);
        var deleteEvent = _Db.Events.Find(dto.EventId);

        if (user == null || user.Id != deleteEvent!.UserId)
            return Unauthorized();

        _Db.Events.Remove(deleteEvent);
        _Db.SaveChanges();

        return Ok("Event Deleted Successfully.");
    }
}
