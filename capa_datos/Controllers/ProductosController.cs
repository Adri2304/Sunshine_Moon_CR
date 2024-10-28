using capa_datos.Clases;
using capa_datos.Clases.Modelos;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using Microsoft.Data.SqlClient;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace capa_datos.Controllers
{
    [ApiController]
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private Conexion conexion;
        public ProductosController(IConfiguration configuration)
        {
            this.conexion = new Conexion(configuration);
        }

        //El READ
        [HttpGet]
        [Route("read/{id?}")]
        public async Task<ActionResult> Read(int? id)
        {
            List<Dictionary<string, object>> respuesta = new List<Dictionary<string, object>>();
            try
            {
                using (SqlCommand comando = new SqlCommand())
                {
                    string consulta = "SELECT * FROM [Entidades].[PRODUCTO]";

                    if (id.HasValue)
                    {
                        consulta += " WHERE idProducto = @id";
                        comando.CommandText = consulta;
                        comando.Parameters.AddWithValue("@id", id);
                    }
                    else
                    {
                        comando.CommandText = consulta;
                    }
                    respuesta = await conexion.EjecutarConsulta(comando);
                }
                return respuesta.Count > 0 ? StatusCode(200, respuesta) : StatusCode(404);
            }
            catch(Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] Producto body)
        {
            int filasAfectadas;
            var datos = body.DevolverDiccionario();
            string consulta = "INSERT INTO [Entidades].[PRODUCTO] (";
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
                    filasAfectadas = await conexion.EjecutarCambios(comando);
                }
                return filasAfectadas > 0 ? StatusCode(201) : StatusCode(400);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            int filasAfectadas = 0;
            string consulta = "DELETE FROM [Entidades].[PRODUCTO] WHERE idProducto = @id";
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await conexion.EjecutarCambios(comando);
                }
                return filasAfectadas > 0 ? StatusCode(200) : StatusCode(404);
            } 
            catch (Exception ex)
            {
                return StatusCode(500);
            } 
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Producto body)
        {
            var datos = body.DevolverDiccionario();
            string consulta = "UPDATE [Entidades].[PRODUCTO] SET ";
            int filasAfectadas = 0;

            foreach (var valor in datos)
            {
                consulta += $"{valor.Key} = @{valor.Key}, ";
            }
            consulta = consulta.Remove(consulta.Length - 2) + $" WHERE idProducto = @id";

            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    foreach (var valor in datos)
                    {
                        comando.Parameters.AddWithValue($"@{valor.Key}", valor.Value);
                    }
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await conexion.EjecutarCambios(comando);
                }
                return filasAfectadas > 0 ? StatusCode(200) : StatusCode(404);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }
    }
}
