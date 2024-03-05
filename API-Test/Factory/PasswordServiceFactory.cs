using API.Interfaces;
using Moq;

namespace API_Test.UserFactory;

public class PasswordServiceFactory
{
    public static IPasswordService CreatePasswordServiceMock()
    {
        var passwordServiceMock = new Mock<IPasswordService>();
        passwordServiceMock.Setup(p =>
            p.GenerateHash(It.IsAny<string>()))
            .Returns("hashedPassword");

        passwordServiceMock.Setup(p => 
            p.CheckHash(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string password, string stringHash) => 
            { return password == stringHash; });


        return passwordServiceMock.Object;
    }
}
