namespace API.Interfaces;

public interface IPasswordService
{
    string GenerateHash(string password);

    bool CheckHash(string password, string stringHash);
}
