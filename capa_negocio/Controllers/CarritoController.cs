using Microsoft.AspNetCore.Mvc;
using capa_negocio.Clases;
using RestSharp;

namespace capa_negocio.Controllers
{
    [ApiController]
    [Route("carrito")]
    public class CarritoController : ControllerBase
    {
        private readonly Solicitudes Solicitudes;

        public CarritoController(IConfiguration configuracion)
        {
            this.Solicitudes = new Solicitudes(configuracion);
        }

        //READ
        [HttpGet]
        [Route("read/{id}")]
        public async Task<ActionResult> Read(int id)
        {
            try
            {
                var solicitud = new RestRequest($"carrito/read/{id}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        //CREATE]
        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] Dictionary<string, int> body)
        {
            try
            {
                if (!body.ContainsKey("idUsuario") && !body.ContainsKey("idProducto") && !body.ContainsKey("cantidad"))
                    return BadRequest("Se necesitan los campos idUsuario, idProducto y cantidad");

                var solicitud = new RestRequest("carrito/create", Method.Post);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        //ELIMINAR
        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var solicitud = new RestRequest($"carrito/delete/{id}", Method.Delete);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        //VACIAR
        [HttpDelete]
        [Route("truncate/{id}")]
        public async Task<ActionResult> Truncate(int id)
        {
            try
            {
                var solicitud = new RestRequest($"carrito/truncate/{id}", Method.Delete);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }
    }
}
