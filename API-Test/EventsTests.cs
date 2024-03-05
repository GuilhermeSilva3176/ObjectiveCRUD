using API.Controllers;
using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.EventsDtos;
using API_Test.UserFactory;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace API_Test;

public class RegistrationTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ITokenService> _tokenService;
    private readonly EventsController _eventsController;
    private readonly UsersModel _user;
    private readonly Guid _id = Guid.NewGuid();

    public RegistrationTests()
    {
        _context = DbContextFactory.CreateInMemoryDbContext();
        _tokenService = new Mock<ITokenService>();
        _eventsController = new(_context, _tokenService.Object);
        _user = new()
        {
            Id = _id,
            Name = "test_Event",
            Email = "test_Event",
            Password = "test_Event",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _context.Users.Add(_user);
        _context.SaveChanges();
    }

    [Fact]
    public async void Create_Return_OkResult_When_Created()
    {
        // Arrange
        CreateEventsDto eventCreatorDto = new()
        {
            EventName = "TestEventCreate",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
        };
        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_id)!);

        // Act 
        IActionResult successfully = await _eventsController.Create(eventCreatorDto);
        EventsModel eventM = _context.Events.FirstOrDefault(m => m.EventName == eventCreatorDto.EventName)!;

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.NotNull(eventM);
        Assert.Equal(eventCreatorDto.EventDescription, eventM.EventDescription);
        Assert.Equal(eventCreatorDto.EventDate, eventM.EventDate);
        Assert.Equal(eventM.UserId, _id);
    }

    [Fact]
    public async void Create_Return_UnauthorizedResult_When_Created()
    {
        // Arrange
        bool test = false;

        CreateEventsDto eventCreatorDto = new()
        {
            EventName = "TestEvent",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
        };
        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(Guid.NewGuid())!);

        // Act 
        try
        {
            IActionResult Unauthorized = await _eventsController.Create(eventCreatorDto);
        }
        catch (Exception ex)
        {
            test = true;
        }

        // Assert
        Assert.True(test);
    }

    [Fact]
    public async void Get_Return_OkResult_When_Getting_All()
    {
        // Arrange
        CreateEventsDto eventCreatorDto = new()
        {
            EventName = "TestEvent",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
        };
        CreateEventsDto eventCreatorDto2 = new()
        {
            EventName = "TestEvent2",
            EventDescription = "TestEvent2",
            EventDate = DateTime.UtcNow
        };

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_id)!);

        await _eventsController.Create(eventCreatorDto);
        await _eventsController.Create(eventCreatorDto2);

        // Act
        IActionResult successfully = _eventsController.Get();
        var okResult = Assert.IsType<OkObjectResult>(successfully);
        var events = Assert.IsAssignableFrom<IEnumerable<EventDto>>(okResult.Value);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.Contains(events, e => e.EventName == eventCreatorDto.EventName);
        Assert.Contains(events, e => e.EventName == eventCreatorDto2.EventName);
    }

    [Fact]
    public async void GetById_Return_OkResult_When_Getting_By_Id()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        EventsModel eventCreator = new()
        {
            Id = id,
            UserId = _id,
            EventName = "TestEvent",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        GetByIdDto eventCreatorDto = new()
        {
            Id = id
        };

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_id)!);

        _context.Events.Add(eventCreator);
        await _context.SaveChangesAsync();

        // Act
        IActionResult successfully = _eventsController.GetById(eventCreatorDto);
        var okResult = Assert.IsType<OkObjectResult>(successfully);
        var events = Assert.IsAssignableFrom<EventDto>(okResult.Value);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.Equal(eventCreator.EventName, events.EventName);
    }

    [Fact]
    public async void Update_Returns_OkResult_When_Updated()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        EventsModel registerEvent = new()
        {
            Id = id,
            UserId = _id,
            EventName = "TestEventForUpdate",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
           .Returns(_context.Users.Find(_id)!);

        _context.Events.Add(registerEvent);
        await _context.SaveChangesAsync();

        UpdateEventDto eventUpdated = new()
        {
            Id = id,
            EventName = "NameChanged",
            EventDescription = "DescriptionChanged",
            EventDate = DateTime.UtcNow,
        };

        // Act
        IActionResult successfully = _eventsController.Update(eventUpdated);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.Equal(registerEvent.EventName, eventUpdated.EventName);
    }

    [Fact]
    public async void Delete_Return_OkResult_when_Deleted()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        EventsModel registerEvent = new()
        {
            Id = id,
            UserId = _id,
            EventName = "TestEventForUpdate",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
           .Returns(_context.Users.Find(_id)!);

        _context.Events.Add(registerEvent);
        await _context.SaveChangesAsync();

        DeleteEventsDto eventDeleted = new()
        {
            EventId = id,
        };

        // Act
        IActionResult successfully = _eventsController.Delete(eventDeleted);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.Null(_context.Events.Find(id));
    }
}