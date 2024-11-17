using capa_datos.Clases;
using capa_datos.Clases.Modelos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace capa_datos.Controllers
{
    [ApiController]
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private Conexion Conexion;
        public ProductosController(IConfiguration configuration)
        {
            this.Conexion = new Conexion(configuration);
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
        public async Task<ActionResult> Create([FromBody] Producto body)
        {
            int filasAfectadas = 0;
            var idInsertado = 0;
            var datos = body.DevolverDiccionario();
            var categorias = (int[])datos["categorias"];
            datos.Remove("categorias");
            string consulta = "EXEC Procedimientos.REGISTRAR_PRODUCTO ";

            foreach (var valor in datos)
            {
                consulta += $"@{valor.Key}, ";
            }
            consulta = consulta.Remove(consulta.Length - 2);

            using (var _Conexion = new SqlConnection(Conexion.GetCadenaConexion()))
            {
                await _Conexion.OpenAsync();
                var transaccion = _Conexion.BeginTransaction();

                try
                {
                    using (SqlCommand comando = new SqlCommand(consulta, _Conexion, transaccion))
                    {
                        foreach (var valor in datos)
                        {
                            comando.Parameters.AddWithValue($"@{valor.Key}", valor.Value);
                        }

                        //; // Obtener el id del producto insertado                        
                        
                        if ((idInsertado = Convert.ToInt32(await comando.ExecuteScalarAsync())) > 0)
                        {
                            consulta = "INSERT INTO [Entidades].[PRODUCTO_CATEGORIA] (idProducto, idCategoria) VALUES ";

                            for (int i = 0; i < categorias.Length; i++)
                            {
                                consulta += $"(@idInsertado, @categoria{i}), ";
                            }
                            consulta = consulta.Remove(consulta.Length - 2);
                            comando.CommandText = consulta;
                            comando.Parameters.Clear();
                            comando.Parameters.AddWithValue("@idInsertado", idInsertado);

                            for (int i = 0; i < categorias.Length; i++)
                            {
                                comando.Parameters.AddWithValue($"@categoria{i}", categorias[i]);
                            }
                            filasAfectadas = await comando.ExecuteNonQueryAsync();
                        }
                        if (filasAfectadas > 0)
                        {
                            transaccion.Commit();
                            return StatusCode(201);
                        }
                        else
                        {
                            transaccion.Rollback();
                            return Conflict();
                        }
                    }
                }
                catch (ArgumentException ex)
                { transaccion.Rollback(); return BadRequest(ex.Message); }
                catch (SqlException ex)
                { transaccion.Rollback(); return ex.Number == 547 ? Conflict(ex.Message) : StatusCode(500, ex.Message); }
                catch (Exception ex)
                { transaccion.Rollback(); return StatusCode(500, ex.Message); }
            }
        }

        [HttpPatch]
        [Route("cambiarestado/{id}")]
        public async Task<ActionResult> cambiarEstado(int id)
        {
            int filasAfectadas = 0;
            string consulta = "UPDATE [Entidades].[PRODUCTO] SET estadoProducto = ~estadoProducto" +
                " WHERE idProducto = @id";
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas == 1 ? StatusCode(200) : StatusCode(404);
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return ex.Number == 547 ? Conflict(ex.Message) : StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpPatch]
        [Route("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Producto body)
        {
            int filasAfectadas = 0;
            var datos = body.DevolverDiccionario();
            var categorias = (int[])datos["categorias"];
            datos.Remove("categorias");
            string consulta = "UPDATE [Entidades].[PRODUCTO] SET ";

            foreach (var valor in datos)
            {
                consulta += $"{valor.Key} = @{valor.Key}, ";
            }
            consulta = consulta.Remove(consulta.Length - 2) + $" WHERE idProducto = @id";

            using (var _Conexion = new SqlConnection(Conexion.GetCadenaConexion()))
            {
                await _Conexion.OpenAsync();
                var transaccion = _Conexion.BeginTransaction();
                try
                {

                    using (SqlCommand comando = new SqlCommand(consulta, _Conexion, transaccion))
                    {
                        comando.Parameters.AddWithValue("@id", id);
                        foreach (var valor in datos)
                        {
                            comando.Parameters.AddWithValue($"@{valor.Key}", valor.Value);
                        }

                        if (await comando.ExecuteNonQueryAsync() > 0)
                        {
                            comando.Parameters.Clear();
                            comando.CommandText = "DELETE FROM [Entidades].[PRODUCTO_CATEGORIA] WHERE idProducto = @id";
                            comando.Parameters.AddWithValue("@id", id);

                            if (await comando.ExecuteNonQueryAsync() > 0)
                            {
                                comando.Parameters.Clear();
                                consulta = "INSERT INTO [Entidades].[PRODUCTO_CATEGORIA] (idProducto, idCategoria) VALUES ";
                                for (int i = 0; i < categorias.Length; i++)
                                {
                                    consulta += $"(@id, @categoria{i}), ";
                                }
                                consulta = consulta.Remove(consulta.Length - 2);
                                comando.CommandText = consulta;
                                comando.Parameters.AddWithValue("@id", id);

                                for (int i = 0; i < categorias.Length; i++)
                                {
                                    comando.Parameters.AddWithValue($"@categoria{i}", categorias[i]);
                                }
                                filasAfectadas = await comando.ExecuteNonQueryAsync();
                            }
                        }
                        if (filasAfectadas > 0)
                        {
                            transaccion.Commit();
                            return Ok();
                        }
                        else
                        {
                            transaccion.Rollback();
                            return Conflict();
                        }
                    }
                }
                catch (ArgumentException ex)
                { transaccion.Rollback(); return BadRequest(ex.Message); }
                catch (SqlException ex)
                { transaccion.Rollback(); return ex.Number == 547 ? Conflict(ex.Message) : StatusCode(500, ex.Message); }
                catch (Exception ex)
                { transaccion.Rollback(); return StatusCode(500, ex.Message); }
            }
        }

        [HttpGet]
        [Route("filtrar")]
        public async Task<ActionResult> Filtrar([FromQuery] string nombre = "", [FromQuery] int[] categorias = null)
        {
            var resultado = new List<Dictionary<string, object>>();
            string consulta = "SELECT P.* FROM [Entidades].[PRODUCTO] AS P ";

            if (categorias != null)
            {
                consulta += @"JOIN (SELECT idProducto, idCategoria FROM (SELECT idProducto, STRING_AGG(idCategoria, ', ') 
                    as idCategoria FROM [Entidades].[PRODUCTO_CATEGORIA] GROUP BY idProducto) AS C WHERE";

                for (int i = 0; i < categorias.Length; i++)
                {
                    consulta += $" idCategoria LIKE @categorias{i} AND";
                }
                consulta = consulta.Remove(consulta.Length - 3) + ") AS PC ON P.idProducto = PC.idProducto";
            }
            consulta += " WHERE P.nombre LIKE @nombre";
            try
            {
                using (var comando = new SqlCommand(consulta))
                {
                    if (categorias != null)
                    {
                        for (int i = 0; i < categorias.Length; i++)
                        {
                            comando.Parameters.AddWithValue($"categorias{i}", $"%{categorias[i]}%");
                        }
                    }
                    comando.Parameters.AddWithValue("@nombre", $"%{nombre}%");
                    resultado = await Conexion.EjecutarConsulta(comando);
                }
                return resultado.Count > 0 ? Ok(resultado) : NoContent();
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
