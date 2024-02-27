using API.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace API.Services;

public class PasswordService : IPasswordService
{

    public string GenerateHash(string password)
    {
        var sha256 = SHA256.Create();
        var builder = new StringBuilder();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));

        foreach (var b in bytes)
            builder.Append(b.ToString("x2"));

        return builder.ToString();
    }
    public bool CheckHash(string password, string stringHash)
    {
        var passwordHash = GenerateHash(password);

        return passwordHash == stringHash;
    }
}
