using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using RestSharp;
using BCrypt;
using System.Text.Json;
using capa_negocio.Clases;

namespace capa_negocio.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly Solicitudes Solicitudes;
        private readonly string JwtSecret;
        private readonly string JwtIssuer;
        private readonly string JwtAudience;
        public AuthController(IConfiguration configuracion)
        {
            this.Solicitudes = new Solicitudes(configuracion);
            this.JwtSecret = configuracion["JWT_SECRET"];
            this.JwtIssuer = configuracion["JWT_ISSUER"];
            this.JwtAudience = configuracion["JWT_AUDIENCE"];
        }

        [HttpPost]
        [Route("autenticar")]
        public async Task<ActionResult> Autenticar([FromBody] Dictionary<string, string> body)
        {
            try
            {
                if (!body.ContainsKey("correo") || !body.ContainsKey("contrasenia"))
                    return BadRequest("Los campos 'correo' y 'contrasenia' son requeridos.");

                var solicitud = new RestRequest("autenticar/consultar", Method.Get);
                solicitud.AddParameter("correo", body["correo"], ParameterType.QueryString);
                var response = await Solicitudes.EjecutarSolicitud(solicitud);

                // Comparar las credenciales
                if ((int)response.StatusCode == 200)
                {
                    var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(response.Content);

                    if (BCrypt.Net.BCrypt.Verify(body["contrasenia"], data[0]["contrasenia"].ToString()))
                    {
                        if ("False".Equals(data[0]["estadoCuenta"].ToString()))
                            return Conflict("Esta cuenta esta desactivada");

                        // Generar tokens
                        var tokens = GenerarTokens(data[0]["idUsuario"].ToString(), data[0]["idRol"].ToString(), body["correo"]);
                        // Guardar en la BD
                        solicitud = new RestRequest($"autenticar/settoken/{data[0]["idUsuario"].ToString()}", Method.Patch);
                        response = await Solicitudes.EjecutarSolicitud(solicitud, new Dictionary<string, object>
                        {{ "refreshToken", tokens["refreshToken"] }});

                        if ((int)response.StatusCode == 200)
                        {
                            tokens.Add("idUsuario", data[0]["idUsuario"].ToString());
                            return Ok(tokens);
                        }
                    }
                    else
                        return Conflict("La contraseña no es correcta");
                }
                return StatusCode((int)response.StatusCode);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpPost]
        [Route("refreshToken/{id}")]
        public async Task<ActionResult> RefrescarToken(int id, [FromBody] Dictionary<string, string> body)
        {
            if (!body.ContainsKey("refreshToken"))
                return BadRequest("El campo refreshToken es necesario");

            var refreshToken = body["refreshToken"];

            try
            {
                // Verificar refresh token si es valido
                if (!EsTokenValido(refreshToken))
                    return Conflict("Refresh Token invalido");

                // Verificar que el refresh token no ha expirado
                if (HaExpiradoToken(refreshToken))
                    return Conflict("El token esta expirado");

                // Comparar con refresh token con el de la BD
                var solicitud = new RestRequest($"autenticar/gettoken/{id}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);

                if ((int)respuesta.StatusCode != 200)
                    return StatusCode((int)respuesta.StatusCode, respuesta.Content);
                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(respuesta.Content);

                if (!data[0]["tokenSesion"].ToString().Equals(refreshToken))
                    return Conflict("Refresh token no asignado al usuario");

                // consultar la informacion de la cuenta
                solicitud = new RestRequest($"autenticar/consultar/{id}", Method.Get);
                respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(respuesta.Content);

                //Generar nuevos tokens
                var tokens = GenerarTokens(data[0]["idUsuario"].ToString(), data[0]["idRol"].ToString(), data[0]["correo"].ToString());

                // Guardar refresh token en la BD
                solicitud = new RestRequest($"autenticar/settoken/{data[0]["idUsuario"].ToString()}", Method.Patch);
                respuesta = await Solicitudes.EjecutarSolicitud(solicitud, new Dictionary<string, object>
                        {{ "refreshToken", tokens["refreshToken"] }});

                if ((int)respuesta.StatusCode != 200)
                    return StatusCode((int)respuesta.StatusCode, respuesta.Content);
                return Ok(tokens);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        private Dictionary<string, string> GenerarTokens(string id, string rol, string correo)
        {
            Dictionary<string, string> respuesta;
            var tokens = new Dictionary<string, string>();

            var claims = new List<Claim> {
                new Claim(ClaimTypes.NameIdentifier, id),
                new Claim(ClaimTypes.Role, rol),
                new Claim(ClaimTypes.Email, correo),
            };

            var jwtToken = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(3),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(JwtSecret)
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );

            var jwtRefreshToken = new JwtSecurityToken(
                issuer: JwtIssuer,
                audience: JwtAudience,
                claims: new List<Claim> { new Claim(ClaimTypes.NameIdentifier, id) },
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddDays(2),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(
                       Encoding.UTF8.GetBytes(JwtSecret)
                        ),
                    SecurityAlgorithms.HmacSha256Signature)
                );
            tokens.Add("accessToken", new JwtSecurityTokenHandler().WriteToken(jwtToken));
            tokens.Add("refreshToken", new JwtSecurityTokenHandler().WriteToken(jwtRefreshToken));
            return tokens;
        }

        private bool HaExpiradoToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);
            var expirationClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp);

            if (expirationClaim != null)
            {
                var expirationDate = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationClaim.Value)).UtcDateTime;
                return expirationDate < DateTime.UtcNow;
            }
            return true; 
        }

        private bool EsTokenValido(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(JwtSecret);

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = JwtIssuer,
                    ValidateAudience = true,
                    ValidAudience = JwtAudience,
                    ClockSkew = TimeSpan.Zero
                };
                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true; // El token es válido
            }
            catch
            {
                return false; // El token no es válido
            }
        }
    }
}