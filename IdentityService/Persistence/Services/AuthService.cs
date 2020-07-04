
using IdentityService.AppSettings;
using IdentityService.Domain.Model;
using IdentityService.Domain.Services;
using IdentityService.Resources;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityService.Persistence.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly JwtSetting _jwtSetting;
        public AuthService(UserManager<AppUser> userManager, 
            RoleManager<IdentityRole> roleManager, 
            IOptions<JwtSetting> jwtSetting)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _jwtSetting = jwtSetting.Value;
        }
        public async Task<string> LoginAsync(LoginResource loginResource)
        {
            var user = await _userManager.FindByEmailAsync(loginResource.Email);

            if (user == null)
                return "The user account does not exist!";

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginResource.Password);

            if (!isPasswordValid)
                return "The user account does not exist!";

            return await GenerateUserJwtToken(user);
        }

        public async Task<string> RegisterAsync(RegisterResource registerResource)
        {
            var user = await _userManager.FindByEmailAsync(registerResource.Email);

            if (user != null)
                return "The user account already exist!";

            if (!await _roleManager.RoleExistsAsync(registerResource.Role))
                return "The role does not exist!";

            user = new AppUser
            {
                FirstName = registerResource.FirstName,
                LastName = registerResource.LastName,
                Email = registerResource.Email,
                UserName = registerResource.Email
            };

            var result = await _userManager.CreateAsync(user, registerResource.Password);

            if (!result.Succeeded)
                return string.Join(" ", result.Errors.Select(x => x.Description));

            result = await _userManager.AddToRoleAsync(user, registerResource.Role);

            if (!result.Succeeded)
                return string.Join(" ", result.Errors.Select(x => x.Description));

            return "register successful";
        }

        private async Task<string> GenerateUserJwtToken(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var secretKey = Encoding.ASCII.GetBytes(_jwtSetting.SecretKey);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id),
                    new Claim(ClaimTypes.Email, user.Email)
                };

            var roles =  await _userManager.GetRolesAsync(user);

            if (roles != null)
                foreach (var role in roles)
                    claims.Add(new Claim(ClaimTypes.Role, role));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey),
                SecurityAlgorithms.HmacSha256Signature),
                Expires = DateTime.UtcNow.AddHours(4)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}
