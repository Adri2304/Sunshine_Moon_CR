using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using System.Net.Http;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;

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

        //[HttpGet]
        //[Route("autenticar")]
        //public async Task<ActionResult> Autenticar([FromBody] Dictionary<string, string> body)
        //{
        //    string url = "http://localhost:5215/autenticar/consultar";
        //    try
        //    {
        //        using (var request = new HttpClient())
        //        {
        //            // Serializar el objeto data a JSON
        //            string jsonData = JsonConvert.SerializeObject(body);

        //            // Crear el contenido de la solicitud con el JSON
        //            StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json"); 
        //            var respuesta = request.GetAsync();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //    //var tokens = GenerateJWTToken("2", "2", "Adrian@gmail.com");
        //    return Ok();
        //}
        
     

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
