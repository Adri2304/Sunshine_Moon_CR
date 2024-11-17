using capa_datos.Clases;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using capa_datos.Clases.Models;

namespace capa_datos.Controllers
{
    [ApiController]
    [Route("carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly Conexion Conexion;

        public CarritoController(IConfiguration configuracion)
        {
            this.Conexion = new Conexion(configuracion);
        }

        [HttpGet]
        [Route("read/{id}")]
        public async Task<ActionResult> Read(int id)
        {
            List<Dictionary<string, object>> resultado = new List<Dictionary<string, object>>();
            string consulta = @"SELECT C.idCarrito, P.nombre, P.precio, C.cantidad, P.imagen 
                FROM [Compras].[CARRITO] C JOIN [Entidades].[PRODUCTO] P 
                ON C.idProducto = P.idProducto  WHERE C.idUsuario = @id";

            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    resultado = await this.Conexion.EjecutarConsulta(comando);
                    return resultado.Count > 0 ? Ok(resultado) : NoContent();
                }
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
        public async Task<ActionResult> create([FromBody] Carrito body)
        {
            int filasAfectadas = 0;
            var datos = body.DevolverDiccionario();
            string consulta = "EXEC Procedimientos.REGISTRAR_CARRITO @idUsuario, @idProducto, @cantidad";
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@idUsuario", datos["idUsuario"]);
                    comando.Parameters.AddWithValue("@idProducto", datos["idProducto"]);
                    comando.Parameters.AddWithValue("@cantidad", datos["cantidad"]);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                    return filasAfectadas > 0 ? StatusCode(201) : Conflict();
                }
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return ex.Number == 50000 ? Conflict(ex.Message) : StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            int filasAfectadas = 0;
            string consulta = "DELETE FROM [Compras].[CARRITO] WHERE idCarrito = @id";

            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas > 0 ? Ok() : NotFound();
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpDelete]
        [Route("truncate/{id}")]
        public async Task<ActionResult> Truncate(int id)
        {
            int filasAfectadas = 0;
            string consulta = "DELETE FROM [Compras].[CARRITO] WHERE idUsuario = @id";

            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                    return filasAfectadas > 0 ? Ok() : NotFound();
                }
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
