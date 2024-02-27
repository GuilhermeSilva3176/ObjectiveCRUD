using API.Interfaces;
using API.Model;
using API.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace API_Test.UserFactory;

public class TokenServiceFactory
{
    private readonly Mock<ITokenService> _tokenService;

    public TokenServiceFactory()
    {
        _tokenService = new Mock<ITokenService>();
    }

    public ITokenService GenerateTokenServiceMock()
    {

        var tokenService = _tokenService.Setup(m => m.GenerateToken(It.IsAny<UsersModel>()))
            .Returns("mocked_token");
        return (ITokenService)tokenService;
    }

    /*public ITokenService GetTokenServiceMock()
    {

    }*/
}
