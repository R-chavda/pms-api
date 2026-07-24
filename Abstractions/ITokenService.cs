using Domain.Entities;

namespace Abstractions
{
    public interface ITokenService
    {
        string GenerateAccessToken(AppUser appUser);
    }
}
