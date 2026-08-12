namespace UserService.Core.Interfaces
{
    public interface IPasswordHasher
    {
        string HasPassword(string password);
        bool Verify(string password, string hashPassword);
    }
}