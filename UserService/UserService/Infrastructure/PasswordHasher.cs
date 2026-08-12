using UserService.Core.Interfaces;

namespace UserService.Infrastructure
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HasPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hashPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashPassword);
        }
    }
}
