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
            try
            {
                using (SqlCommand comando = new SqlCommand())
                {
                    string consulta = "SELECT * FROM [Entidades].[USUARIO]";

                    if (id.HasValue)
                    {
                        consulta += " WHERE idUsuario = @id";
                        comando.CommandText = consulta;
                        comando.Parameters.AddWithValue("@id", id);
                    }
                    else
                    {
                        comando.CommandText = consulta;
                    }
                    respuesta = await Conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? StatusCode(200, respuesta) : NoContent();
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
            string consulta = "INSERT INTO [Entidades].[USUARIO] (";
            string values = ") VALUES (";

            foreach (var valor in datos)
            {
                consulta += $"{valor.Key}, ";
                values += $"@{valor.Key}, ";
            }
            consulta = consulta.Remove(consulta.Length - 2);
            values = values.Remove(values.Length - 2);
            values += ")";
            consulta += values;

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
                return filasAfectadas > 0 ? StatusCode(201) : StatusCode(400);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
