using API.Controllers;
using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.UserDtos;
using API_Test.UserFactory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace API_Test;

public class EventsTests
{
    private readonly AppDbContext _context;
    private readonly Mock<ITokenService> _tokenService;
    private readonly EventsController _eventsController;

    public EventsTests()
    {
        _context = DbContextFactory.CreateInMemoryDbContext();
        _tokenService = new Mock<ITokenService>();
        _eventsController = new(_context, _tokenService.Object);
    }
}
