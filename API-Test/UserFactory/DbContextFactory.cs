using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API_Test.UserFactory;

public class DbContextFactory
{
    public static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "InMemoryUserDatabase")
            .Options;

        return new AppDbContext(options);
    }
}
