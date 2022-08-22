using HotelListing.Data.Interfaces;
using HotelListing.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace HotelListing.Data.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApiUser> _userManager;

        private SymmetricSecurityKey _securityKey;

        private IConfigurationSection _configurationSection;

        public TokenService(IConfiguration configuration, UserManager<ApiUser> userManager)
        {
            _configurationSection = configuration.GetSection("Jwt");

            string environmentKey = Environment.GetEnvironmentVariable("KEYHotel", EnvironmentVariableTarget.Machine) ?? "";

            _securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(environmentKey));

            _userManager = userManager;
        }

        public async Task<string> CreateTokenAsync(ApiUser user)
        {

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId,user.Id.ToString()),

                new Claim(JwtRegisteredClaimNames.UniqueName,user.Email)
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var signingCredentials = new SigningCredentials(_securityKey, SecurityAlgorithms.HmacSha512Signature);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject=new ClaimsIdentity(claims),

                Expires=DateTime.Now.AddHours(2),

                SigningCredentials=signingCredentials,
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenScreiptor = tokenHandler.CreateToken(securityTokenDescriptor);

            return tokenHandler.WriteToken(tokenScreiptor);
        }
    }
}
