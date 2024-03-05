using API.Controllers;
using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.EventsDtos;
using API.Model.DTOs.RegistrationDtos;
using API_Test.UserFactory;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace API_Test;

public class RegistrationTest
{
    private readonly AppDbContext _context;
    private readonly Mock<ITokenService> _tokenService;
    private readonly RegistrationController _registrationController;
    private readonly EventsModel _event;
    private readonly UsersModel _user;
    private readonly Guid _userId = Guid.NewGuid();
    private readonly Guid _eventId = Guid.NewGuid();

    public RegistrationTest()
    {
        _context = DbContextFactory.CreateInMemoryDbContext();
        _tokenService = new Mock<ITokenService>();
        _registrationController = new(_context, _tokenService.Object);

        _user = new()
        {
            Id = _userId,
            Name = "test_Event",
            Email = "test_Event",
            Password = "test_Event",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _event = new()
        {
            Id = _eventId,
            UserId = _userId,
            EventName = "Event_ForTests",
            EventDescription = "TestEvent",
            EventDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Events.Add(_event);
        _context.Users.Add(_user);
        _context.SaveChanges();
    }

    [Fact]
    public async void Create_Returns_OkResult_When_Created()
    {
        // Arrange
        CreateRegisDto registrationSuccessfully = new()
        {
            EventId = _eventId,
        };
        CreateRegisDto registrationFailed = new()
        {
            EventId = Guid.NewGuid(),
        };

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_userId)!);

        // Act
        IActionResult successfully = await _registrationController.Create(registrationSuccessfully);
        IActionResult failed = await _registrationController.Create(registrationFailed);

        // Assert
        Assert.IsType<OkResult>(successfully);
        Assert.IsType<NotFoundObjectResult>(failed);
    }

    [Fact]
    public async void Delete_Returns_OkResult_When_Deleted()
    {
        // Arrange
        Guid id = Guid.NewGuid();

        RegistrationModel regisModel = new()
        {
            Id = id,
            EventId = _eventId,
            UserId = _userId,
        };

        DeleteRegisDto registrationSuccessfully = new()
        {
            RegistrationId = id,
        };
        DeleteRegisDto registrationFailed = new()
        {
            RegistrationId = Guid.NewGuid(),
        };

        _context.Registration.Add(regisModel);
        await _context.SaveChangesAsync();

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_userId)!);

        // Act
        IActionResult successfully = _registrationController.Delete(registrationSuccessfully);
        IActionResult failed = _registrationController.Delete(registrationFailed);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.IsType<UnauthorizedResult>(failed);
    }

    [Fact]
    public async void Get_Returns_OkResult_When_Getting()
    {
        // Arrange
        Guid idONE = Guid.NewGuid();
        Guid idTWO = Guid.NewGuid();
        RegistrationModel registrationONE = new()
        {
            Id = idONE,
            EventId = _eventId,
            UserId = _userId
        };
        RegistrationModel registrationTWO = new()
        {
            Id = idTWO,
            EventId = _eventId,
            UserId = _userId
        };

        _context.Registration.Add(registrationONE);
        _context.Registration.Add(registrationTWO);
        await _context.SaveChangesAsync();

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(_userId)!);

        // Act
        IActionResult successfully = _registrationController.Get();
        var okResult = Assert.IsType<OkObjectResult>(successfully);
        var events = Assert.IsAssignableFrom<IEnumerable<RegistrationDto>>(okResult.Value);


        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.Contains(events, e => e.Id == registrationONE.Id);
        Assert.Contains(events, e => e.Id == registrationTWO.Id);
    }
    [Fact]
    public async void GetById_Returns_OkResult_When_Getting()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        RegistrationModel regis = new()
        {
            Id = id,
            EventId = _eventId,
            UserId = _userId
        };

        // Act

        // Assert
    }
}
