using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;

namespace capa_negocio.Controllers
{
    [ApiController]
    [Route("autenticacion")]
    public class AuthController : ControllerBase
    {
        private IConfiguration Configuracion;
        public AuthController(IConfiguration configuracion)
        {
            this.Configuracion = configuracion;
        }

        [HttpGet]
        [Route("autenticar")]
        public async Task<ActionResult> Autenticar([FromBody] Dictionary<string, string> body)
        {
            var tokens = GenerateJWTToken("2", "2", "Adrian@gmail.com");
            return Ok(tokens);
        }
        
     

        private Dictionary<string, string> GenerateJWTToken(string id, string rol, string correo)
        {
            var tokens = new Dictionary<string, string>();
            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.Email, correo),
            };
            var jwtToken = new JwtSecurityToken(
                issuer: Configuracion["JwtSettings:JwtIssuer"],
                audience: Configuracion["JwtSettings:JwtAudience"],
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(3),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(Configuracion["JwtSettings:JwtSecret"])
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );

            var jwtRefreshToken = new JwtSecurityToken(
                issuer: Configuracion["JwtSettings:JwtIssuer"],
                audience: Configuracion["JwtSettings:JwtAudience"],
                claims: new List<Claim> { new Claim(ClaimTypes.NameIdentifier, id) },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(Configuracion["JwtSettings:JwtSecret"])
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );

            tokens.Add("accessToken", new JwtSecurityTokenHandler().WriteToken(jwtToken));
            tokens.Add("refreshToken", new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken));
            return tokens;
        }
    }
}
