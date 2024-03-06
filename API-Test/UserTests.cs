using API.Controllers;
using API.Data;
using API.Interfaces;
using API.Model;
using API.Model.DTOs.UserDtos;
using API_Test.UserFactory;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;

namespace API_Test;

public class UserTests
{
    private readonly AppDbContext _context;
    private readonly IPasswordService _passwordService;
    private readonly Mock<ITokenService> _tokenService;
    private readonly UserController _controller;

    public UserTests()
    {
        _context = DbContextFactory.CreateInMemoryDbContext();
        _tokenService = new Mock<ITokenService>();
        _passwordService = PasswordServiceFactory.CreatePasswordServiceMock();
        _controller = new(_context, _passwordService, _tokenService.Object);

        _tokenService.Setup(m => m.GenerateToken(It.IsAny<UsersModel>())).Returns("mocked_token");
    }

    [Fact]
    public async void Create_Returns_OkResult_When_Created()
    {
        // Arrange
        CreateUserDto createUserDto = new()
        {
            Name = "test",
            Email = "test",
            Password = "test"
        };

        // Act
        IActionResult result = await _controller.Create(createUserDto);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.Equal("Account Created Successfully", okResult!.Value);

        // Verify user is added to the database
        var user = _context.Users.FirstOrDefault(u => u.Email == createUserDto.Email);
        Assert.NotNull(user);
        Assert.Equal(createUserDto.Name, user.Name);
        Assert.Equal(createUserDto.Email, user.Email);
        Assert.Equal("hashedPassword", user.Password);
    }

    [Fact]
    public async void Login_Return_OkResult_When_Logged()
    {
        // Arrange

        UsersModel user = new()
        {
            Id = Guid.NewGuid(),
            Name = "test2",
            Email = "test2",
            Password = "test2",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        LoginUserDto logOk = new()
        {
            Email = "test2",
            Password = "test2"
        };

        LoginUserDto logUnauthorized = new()
        {
            Email = "test2",
            Password = "WrongPassword"
        };

        // Act
        IActionResult successfully = _controller.Login(logOk);
        var objectSuccessfully = successfully as OkObjectResult;
        IActionResult failed = _controller.Login(logUnauthorized);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.IsType<UnauthorizedResult>(failed);
        Assert.NotNull(objectSuccessfully!.Value);
    }

    [Fact]
    public async void Update_Return_OkResult_When_Updated()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UsersModel userWithAOlderPassword = new()
        {
            Id = id,
            Name = "test3",
            Email = "test3",
            Password = "test3",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _context.Users.Add(userWithAOlderPassword);
        await _context.SaveChangesAsync();

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(id)!);

        UpdateDto updatePasswordDtoReturnsOk = new()
        {
            Password = "test3",
            NewPassword = "123"
        };

        UpdateDto updateUserDtoReturnsUnauthorized = new()
        {
            Password = "wrongPassword",
            NewPassword = "123"
        };

        // Act
        IActionResult succesfully = _controller.Update(updatePasswordDtoReturnsOk);
        IActionResult unauthorized = _controller.Update(updateUserDtoReturnsUnauthorized);

        // Assert
        Assert.IsType<OkObjectResult>(succesfully);
        Assert.IsType<UnauthorizedResult>(unauthorized);
        Assert.Equal("hashedPassword", userWithAOlderPassword.Password);
    }


    [Fact]
    public async void Delete_Return_OkResult_when_Deleted()
    {
        // Arrange
        Guid id = Guid.NewGuid();
        UsersModel user = new()
        {
            Id = id,
            Name = "test4",
            Email = "test4",
            Password = "test4",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _tokenService.Setup(m => m.GetUserByToken(It.IsAny<ClaimsPrincipal>()))
            .Returns(_context.Users.Find(id)!);
       

        DeleteDto deleteAccountReturnOk = new()
        {
            Password = "test4"
        };
        DeleteDto deleteAccountReturnUnauthorized = new()
        {
            Password = "WrongPassword"
        };


        // Act
        IActionResult successfully = _controller.Delete(deleteAccountReturnOk);
        IActionResult unauthorized = _controller.Delete(deleteAccountReturnUnauthorized);

        // Assert
        Assert.IsType<OkObjectResult>(successfully);
        Assert.IsType<UnauthorizedResult>(unauthorized);
        Assert.Null(_context.Users.Find(id));
    }
}