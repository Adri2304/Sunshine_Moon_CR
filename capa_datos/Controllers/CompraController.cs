using Microsoft.AspNetCore.Mvc;
using capa_datos.Clases;
using capa_datos.Clases.Models;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;

namespace capa_datos.Controllers
{
    [ApiController]
    [Route("compras")]
    public class CompraController : ControllerBase
    {
        private readonly Conexion Conexion;

        public CompraController(IConfiguration configuracion)
        {
            this.Conexion = new Conexion(configuracion);
        }

        [HttpGet]
        [Route("read/{id?}")]
        public async Task<ActionResult> read(int? id)
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = @"SELECT C.idCompra, U.nombre + ' ' + U.apellidoUno + ' ' + U.apellidoDos AS cliente,
                    U.correo, U.Telefono, EC.nombre AS estado, C.costoEnvio, C.subTotal, C.total, C.fechaCompra 
                    FROM [Compras].[COMPRA] C JOIN [Entidades].[USUARIO] U ON C.idUsuario = U.idUsuario 
                    JOIN [Entidades].[ESTADO_COMPRA] EC ON C.idEstadoCompra = EC.idEstadoCompra";

                if (id.HasValue)
                    consulta += " WHERE C.idUsuario = @id";
                consulta += " ORDER BY C.fechaCompra DESC";

                using (var comando = new SqlCommand(consulta))
                {
                    if (id.HasValue)
                        comando.Parameters.AddWithValue("@id", id);
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

        [HttpGet]
        [Route("detallecompra/{id}")]
        public async Task<ActionResult> DetalleCompra(int id)
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = @"SELECT P.nombre,P.imagen,P.precio,CP.cantidad,
	                CP.totalPrecio FROM [Compras].[COMPRA_PRODUCTO] CP JOIN [Entidades].[PRODUCTO] P
                    ON CP.idProducto = P.idProducto WHERE CP.idCompra = @id";

                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    resultado = await Conexion.EjecutarConsulta(comando);
                }
                return resultado.Count > 0 ? Ok(resultado) : NotFound("No se encontro ningun detalle de la compra");
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
        public async Task<ActionResult> Create([FromBody] Compra body)
        {
            try
            {
                string consulta = "EXEC Procedimientos.REGISTRAR_COMPRA @id, @tipoEntrega, @costoEnvio, @imagenFactura";
                var data = body.DevolverDiccionario();
                int filasAfectadas = 0;

                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", data["idUsuario"]);
                    comando.Parameters.AddWithValue("@tipoEntrega", data["tipoEntrega"]);
                    comando.Parameters.AddWithValue("@costoEnvio", data["costoEnvio"]);
                    comando.Parameters.AddWithValue("@imagenFactura", data["imagenFactura"]);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }

                return filasAfectadas >= 4 ? StatusCode(201, "La compra fue exitosa" + filasAfectadas) : Conflict("No se pudo realizar la compra" + filasAfectadas);
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpPatch]
        [Route("cambiarestado")]
        public async Task<ActionResult> CambiarEstado([FromBody] Dictionary<string, int> body)
        {
            try
            {
                string consulta = "UPDATE [Compras].[COMPRA] set idEstadoCompra = @idEstadoCompra WHERE idCompra = @idCompra";
                int filasAfectadas = 0;

                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@idEstadoCompra", body["idEstadoCompra"]);
                    comando.Parameters.AddWithValue("@idCompra", body["idCompra"]);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas == 1 ? Ok("Se cambio el estado correctamente") : Conflict("No se pudo cambiar el estado");
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return ex.Number == 547 ? Conflict("Verifique los datos enviados") : StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("costoenvio/{id}")]
        public async Task<ActionResult> CostoEnvio(int id)
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = "EXEC Procedimientos.SP_COSTOENVIO_CLIENTE @id";

                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    resultado = await Conexion.EjecutarConsulta(comando);
                }
                return resultado.Count > 0 ? Ok(resultado) : Conflict("No se pudo consultar el costo de envio");
            }
            catch (ArgumentException ex)
            { return BadRequest(ex.Message); }
            catch (SqlException ex)
            { return StatusCode(500, ex.Message); }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }

        [HttpGet]
        [Route("filtrarestado")]
        public async Task<ActionResult> FiltroEstado([FromQuery] int? estado = null)
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = @"SELECT C.idCompra, U.nombre + ' ' + U.apellidoUno + ' ' + U.apellidoDos AS cliente,
                    U.correo, U.Telefono, EC.nombre AS estado, C.costoEnvio, C.subTotal, C.total, C.fechaCompra 
                    FROM [Compras].[COMPRA] C JOIN [Entidades].[USUARIO] U ON C.idUsuario = U.idUsuario 
                    JOIN [Entidades].[ESTADO_COMPRA] EC ON C.idEstadoCompra = EC.idEstadoCompra";

                if (estado.HasValue)
                    consulta += " WHERE C.idEstadoCompra = @estado";
                consulta += " ORDER BY C.fechaCompra DESC"; 

                using (var comando = new SqlCommand(consulta))
                {
                    if (estado.HasValue)
                        comando.Parameters.AddWithValue("@estado", estado);
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

        [HttpGet]
        [Route("filtrarantiguedad")]
        public async Task<ActionResult> FiltroAntiguedad()
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = @"SELECT C.idCompra, U.nombre + ' ' + U.apellidoUno + ' ' + U.apellidoDos AS cliente,
                    U.correo, U.Telefono, EC.nombre AS estado, C.costoEnvio, C.subTotal, C.total, C.fechaCompra 
                    FROM [Compras].[COMPRA] C JOIN [Entidades].[USUARIO] U ON C.idUsuario = U.idUsuario 
                    JOIN [Entidades].[ESTADO_COMPRA] EC ON C.idEstadoCompra = EC.idEstadoCompra
                    ORDER BY C.fechaCompra ASC";

                using (var comando = new SqlCommand(consulta))
                {
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

        [HttpGet]
        [Route("consultarimagen/{id}")]
        public async Task<ActionResult> ConsultarImagen(int id)
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = "SELECT imagenFactura FROM [Compras].[COMPRA] WHERE idCompra = @id";

                using (var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@id", id);
                    resultado = await Conexion.EjecutarConsulta(comando);
                }
                return resultado.Count > 0 ? Ok(resultado) : NotFound("No se encontro la compra");
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
