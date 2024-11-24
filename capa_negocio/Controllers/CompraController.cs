using capa_negocio.Clases;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using System.Security.Cryptography.Xml;
using System.Text.Json;

namespace capa_negocio.Controllers
{
    [ApiController]
    [Route("compras")]
    public class CompraController : ControllerBase
    {
        private readonly Solicitudes Solicitudes;

        public CompraController(IConfiguration configuracion)
        {
            this.Solicitudes = new Solicitudes(configuracion);
        }

        // LEER TODOS
        [HttpGet]
        [Route("read/")]
        public async Task<ActionResult> Read()
        {
            try
            {
                var solicitud = new RestRequest("compras/read");
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un erro en el servidor"); }
        }

        // LEER HISTORIAL
        [HttpGet]
        [Route("historial/{id}")]
        public async Task<ActionResult> HistorialUsuario(int id)
        {
            try
            {
                var solicitud = new RestRequest($"compras/read/{id}");
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un erro en el servidor"); }
        }

        // CREATE
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] Dictionary<string, object> body)
        {
            try
            {
                // Validar que vengan los campos necesarios
                if (!body.ContainsKey("idUsuario") || !body.ContainsKey("tipoEntrega") || !body.ContainsKey("imagenFactura"))
                    return BadRequest("Se necesitan los campos idUsuario, tipoEntrega, imagenFactura");
                // Validar que el campo tipoEntrega sea estrictamente ENVIO o RETIRO
                if (!"ENVIO".Equals(body["tipoEntrega"].ToString()) && !"RETIRO".Equals(body["tipoEntrega"].ToString()))
                    return BadRequest("El campo tipoEntrega debe ser 'RETIRO' o 'ENVIO'");

                double costoEnvio = 0.0;

                if ("ENVIO".Equals(body["tipoEntrega"].ToString()))
                {
                    var request = new RestRequest($"compras/costoenvio/{body["idUsuario"].ToString()}", Method.Get);
                    var response = await Solicitudes.EjecutarSolicitud(request);

                    if ((int)response.StatusCode != 200)
                        return StatusCode((int)response.StatusCode, response.Content);

                    var data = JsonSerializer.Deserialize<List<Dictionary<string, double>>>(response.Content);
                    costoEnvio = data[0]["costoEnvio"];
                }

                // Subir la imagen de la factura y obtener su URL

                body.Add("costoEnvio", costoEnvio);
                var solicitud = new RestRequest("compras/create", Method.Post);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        // DETALLE COMPRA
        [HttpGet]
        [Route("detallecompra/{id}")]
        public async Task<ActionResult> DetalleCompra(int id)
        {
            try
            {
                var solicitud = new RestRequest($"compras/detallecompra/{id}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un erro en el servidor"); }
        }

        // FILTRAR ESTADO
        [HttpGet]
        [Route("filtrarestado")]
        public async Task<ActionResult> FiltrorEstado()
        {
            try
            {
                var parametros = HttpContext.Request.QueryString.Value;
                var solicitud = new RestRequest($"compras/filtrarestado{parametros}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        // FILTRAR ANTIGUEDAD
        [HttpGet]
        [Route("filtrarantiguedad")]
        public async Task<ActionResult> FiltroAntiguedad()
        {
            try
            {
                var solicitud = new RestRequest($"compras/filtrarantiguedad", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        // CAMBIAR ESTADO
        [HttpPatch]
        [Route("cambiarestado")]
        public async Task<ActionResult> CambiarEstado([FromBody] Dictionary<string, int> body)
        {
            try
            {
                // Validar que vengan los campos necesarios
                if (!body.ContainsKey("idCompra") || !body.ContainsKey("idEstadoCompra"))
                    return BadRequest("Se necesitan los campos idCompra, idEstadoCompra");

                var solicitud = new RestRequest("compras/cambiarestado", Method.Patch);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        // CONSULTAR IMAGEN
        [HttpGet]
        [Route("consultarimagen/{id}")]
        public async Task<ActionResult> ConsultarImagen(int id)
        {
            try
            {
                var solicitud = new RestRequest($"compras/consultarimagen/{id}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex) 
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }
    }
}
