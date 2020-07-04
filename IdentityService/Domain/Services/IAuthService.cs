using IdentityService.Resources;
using System.Threading.Tasks;

namespace IdentityService.Domain.Services
{
    public interface IAuthService
    {
        Task<string> RegisterAsync(RegisterResource registerResource);
        Task<string> LoginAsync(LoginResource loginResource);
    }
}
