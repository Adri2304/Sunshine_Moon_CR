using capa_datos.Clases;
using capa_datos.Clases.Modelos;
using capa_datos.Clases.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace capa_datos.Controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly Conexion Conexion;

        public UsuariosController(IConfiguration configuracion)
        {
            this.Conexion = new Conexion(configuracion);
        }

        [HttpGet]
        [Route("read/{id?}")]
        public async Task<ActionResult> Read(int id = 0)
        {
            List<Dictionary<string, object>> respuesta = new List<Dictionary<string, object>>();
            string consulta = "Procedimientos.CONSULTAR_USUARIOS @id";

            try
            {
                using (SqlCommand comando = new SqlCommand(consulta))
                {
                        comando.Parameters.AddWithValue("@id", id);                    
                    respuesta = await Conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? StatusCode(200, respuesta) : NoContent();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] UsuarioDireccion body)
        {
            int filasAfectadas;
            var datos = body.DevolverDiccionario();

            string consulta = "EXEC Procedimientos.REGISTRAR_USUARIO ";

            foreach (var valor in datos)
            {
                consulta += $"@{valor.Key}, ";
            }
            consulta = consulta.Remove(consulta.Length - 2);

            try
            {
                using (SqlCommand comando = new SqlCommand(consulta))
                {
                    foreach (var valor in datos)
                    {
                        comando.Parameters.AddWithValue($"@{valor.Key}", valor.Value);
                    }
                    //return Ok(consulta);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas == 2 ? StatusCode(201) : Conflict();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpPatch]
        [Route("cambiarestado/{id}")]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            int filasAfectadas = 0;
            string consulta = "UPDATE [Entidades].[USUARIO] SET estadoCuenta = ~estadoCuenta " +
                "WHERE idUsuario = @id";

            try
            {
                using (var comando = new SqlCommand(consulta))
                {
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

        [HttpPatch]
        [Route("cambiarimagen/{id}")]
        public async Task<ActionResult> CambiarImagen(int id, [FromBody] Dictionary<string, string> body)
        {
            int filasAfectadas = 0;
            string consulta = "UPDATE [Entidades].[USUARIO] SET imagen = @imagen WHERE idUsuario = @id";

            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@imagen", body["imagen"].ToString());
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
    }
}

