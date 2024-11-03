using capa_datos.Clases;
using capa_datos.Clases.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

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
        public async Task<ActionResult> Read(int? id)
        {
            List<Dictionary<string, object>> respuesta = new List<Dictionary<string, object>>();
            string consulta = "EXEC consultarUsuarios NULL";
            try
            {
                using (SqlCommand comando = new SqlCommand())
                {
                    comando.CommandText = consulta;
                    if (id.HasValue)
                    {
                        consulta = consulta.Remove(consulta.Length - 4);
                        consulta += "@id";
                        comando.CommandText = consulta;
                        comando.Parameters.AddWithValue("@id", id);
                    }
                    
                    respuesta = await Conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? StatusCode(200, respuesta) : NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest();
            }
            catch (SqlException ex)
            {
                return ex.Number == 547 || ex.Number == 2627 ? BadRequest() : StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] Usuario body)
        {
            int filasAfectadas;
            var datos = body.DevolverDiccionario();
            string consulta = "EXEC crearUsuario ";

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
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas > 1 ? StatusCode(201) : StatusCode(400);
            }
            catch (ArgumentException ex)
            {
                return BadRequest();
            }
            catch (SqlException ex)
            {
                return ex.Number == 547 || ex.Number == 2627 ? BadRequest() : StatusCode(500);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        //[HttpPatch]
        //[Route("disable/{id}")]
        //public async Task<ActionResult> Disable(int id)
        //{
        //    int filasAfectadas = 0;
        //    string consulta = "UPDATE [Entidades].[USUARIO] SET ";
        //}
    }
}
