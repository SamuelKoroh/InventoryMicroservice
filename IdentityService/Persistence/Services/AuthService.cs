
using IdentityService.Domain.Model;
using IdentityService.Domain.Services;
using IdentityService.Resources;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace IdentityService.Persistence.Services
{
    public class AuthService : IIdentityService
    {
        private readonly UserManager<AppUser> _userManager;

        public AuthService(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }
        public Task<string> LoginAsync(LoginResource loginResource)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> RegisterAsync(RegisterResource registerResource)
        {
            var user = new AppUser
            {
                FirstName = registerResource.FirstName,
                LastName = registerResource.LastName,
                Email = registerResource.Email,
                UserName = registerResource.Email
            };

            var result = await _userManager.CreateAsync(user, registerResource.Password);

            if (!result.Succeeded)
                return "register fail";

            return "register successful";
        }
    }
}
