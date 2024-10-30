using capa_datos.Clases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace capa_datos.Controllers
{
    [ApiController]
    [Route("autenticar")]
    public class AutenticarController : Controller
    {
        private Conexion Conexion;
        public AutenticarController(IConfiguration configuracion)
        {
            this.Conexion = new Conexion(configuracion);
        }

        [HttpGet]
        [Route("consultar")]
        public async Task<ActionResult> ConsultarCuenta([FromBody] Dictionary<string, string> body)
        {
            var respuesta = new List<Dictionary<string, object>>();
            string consulta = "SELECT idUsuario, idRol, correo, contrasenia FROM " +
                "[Entidades].[USUARIO] WHERE correo = @correo";
            
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@correo", body["correo"]);
                    respuesta = await Conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? Ok(respuesta) : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
