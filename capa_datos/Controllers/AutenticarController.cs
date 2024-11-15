using capa_datos.Clases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Globalization;

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
        public async Task<ActionResult> ConsultarCuenta([FromQuery] string correo)
        {
            var respuesta = new List<Dictionary<string, object>>();
            string consulta = "SELECT idUsuario, idRol, correo, contrasenia FROM " +
                "[Entidades].[USUARIO] WHERE correo = @correo";
            
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@correo", correo);
                    respuesta = await Conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? Ok(respuesta) : NotFound();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }
        
        [HttpGet]
        [Route("consultar/{id}")]
        public async Task<ActionResult> ConsultarCuenta(int id)
        {
            var respuesta = new List<Dictionary<string, object>>();
            string consulta = "SELECT idUsuario, idRol, correo, contrasenia FROM " +
                "[Entidades].[USUARIO] WHERE idUsuario = @id";
            
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    respuesta = await Conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? Ok(respuesta) : NotFound();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpPatch]
        [Route("settoken/{id}")]
        public async Task<ActionResult> ActualizarRefreshToken(int id, [FromBody] Dictionary<string,string> body)
        {
            int filasAfectadas = 0;
            string consulta = "UPDATE [Entidades].[USUARIO] SET tokenSesion = @token WHERE idUsuario = @id";
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@token", body["refreshToken"]);
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas == 1 ? Ok() : NotFound();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("gettoken/{id}")]
        public async Task<ActionResult> ObtenerToken(int id)
        {
            var resultado = new List<Dictionary<string, object>>();
            string consulta = "SELECT tokenSesion FROM [Entidades].[USUARIO] WHERE idUsuario = @id";
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    resultado = await Conexion.EjecutarConsulta(comando);
                }
                return resultado.Count > 0 ? Ok(resultado) : NotFound();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }
    }
}
