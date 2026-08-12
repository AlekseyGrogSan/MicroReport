using UserService.Core.Models;

namespace UserService.Core.Interfaces
{
    public interface IJWTService
    {
        string GenerateAcsessToken(User user);
        string GenerateRefreshToken();
    }
}