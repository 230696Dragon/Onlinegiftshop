using ClassLibrary2.Dtos;
using Ecommerce.api.Dbcontext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Ecommerce.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly GiftShopDbContext shopDbContext;
        private readonly IConfiguration configuration;

        public TokenController(GiftShopDbContext GiftShopDbContext, IConfiguration configuration)
        {
            this.shopDbContext = GiftShopDbContext;
            this.configuration = configuration;
        }

        [HttpPost]
        public IActionResult AdminLogin([FromBody] LoginDto loginDto)
        {
            var result = shopDbContext.Users.FirstOrDefault(p => p.Email == loginDto.Email && p.Password == loginDto.Password);

            if (result == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
        new Claim(JwtRegisteredClaimNames.Sub, result.Name),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["JwtSettings:Issuer"],
                audience: configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            string tokenKey = new JwtSecurityTokenHandler().WriteToken(token);

            var tokenDto = new TokenDto
            {
                Tokens = tokenKey,
                RoleID = result.RoleID,
                Id = result.Id,
                Name = result.Name
            };

            return Ok(tokenDto);
        }
    }
}
