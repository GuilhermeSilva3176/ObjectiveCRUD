using API.Controllers;
using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.EventsDtos;
using API.Model.DTOs.UserDtos;
using API_Test.UserFactory;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace API_Test;

public class EventsTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ITokenService> _tokenService;
    private readonly EventsController _eventsController;
    private readonly UsersModel _user;
    private readonly Guid _id = Guid.NewGuid();

    public EventsTests()
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
            EventName = "TestEvent",
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
    public async void Get_Return_OkResult_When_Has_Event()
    {
        // Arrange
        CreateEventsDto eventCreatorDto = new()
        {
            EventName = "TestEvent",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
        };
        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_id)!);

        var event1 = await _eventsController.Create(eventCreatorDto);

        // Act
        IActionResult successfully = _eventsController.Get();
        EventsModel eventM = _context.Events.FirstOrDefault(m => m.EventName == eventCreatorDto.EventName)!;

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.Equal(eventCreatorDto.EventName, eventM.EventName);
    }
}
