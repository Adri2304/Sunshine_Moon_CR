using capa_datos.Clases;
using capa_datos.Clases.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Runtime.InteropServices;

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
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas == 2 ? StatusCode(201, "El usuario se creo correctamente")
                    : Conflict("El correo ingresado ya existe");
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

        [HttpGet]
        [Route("filtrar")]
        public async Task<ActionResult> Filtrar([FromQuery] string filtro = "")
        {
            try
            {
                var resultado = new List<Dictionary<string, object>>();
                string consulta = @"SELECT U.idUsuario, U.nombre, U.apellidoUno, U.apellidoDos, U.correo, 
                    U.telefono, U.fechaRegistro, U.imagen, U.estadoCuenta, D.provincia, D.canton, D.distrito, 
                    D.barrio, D.direccionExacta FROM [Entidades].[USUARIO] U 
                    JOIN [Entidades].[DIRECCION] D ON U.idUsuario = D.idUsuario 
                    WHERE U.nombre + ' ' + U.apellidoUno + ' ' + U.apellidoDos LIKE @filtro 
                    OR U.correo LIKE @filtro OR U.telefono LIKE @filtro";

                using(var comando = new SqlCommand(consulta))
                {
                    comando.Parameters.AddWithValue("@filtro", $"%{filtro}%");
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

        [HttpPatch]
        [Route("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] Dictionary<string, string> body)
        {
            try
            {
                int filasAfectadas = 0;
                string consulta = "EXEC Procedimientos.ACTUALIZAR_USUARIO @id, ";

                foreach (var item in body)
                {
                    consulta += $"@{item.Key}, ";
                }
                consulta = consulta.Remove(consulta.Length - 2);

                using (var comando = new SqlCommand(consulta))
                {
                    foreach (var item in body)
                        comando.Parameters.AddWithValue($"@{item.Key}", item.Value);
                    comando.Parameters.AddWithValue("@id", id);
                    filasAfectadas = await Conexion.EjecutarCambios(comando);
                }
                return filasAfectadas == 2 ? Ok() : Conflict("El correo ya existe o no se encontro el usuario");
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

