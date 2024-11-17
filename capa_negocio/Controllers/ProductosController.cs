using Microsoft.AspNetCore.Mvc;
using RestSharp;
using capa_negocio.Clases;
using capa_negocio.Clases.Models;

namespace capa_negocio.Controllers
{
    [ApiController]
    [Route("productos")]
    public class ProductosController : ControllerBase
    {
        private readonly Solicitudes Solicitudes;
        public ProductosController(IConfiguration configuracion)
        {
            this.Solicitudes = new Solicitudes(configuracion);
        }

        [HttpGet]
        [Route("read/{id?}")]
        public async Task<ActionResult> Read(int? id)
        {
            string url = "productos/read";

            if (id.HasValue)
                url += $"/{id}";
            try
            {
                var solicitud = new RestRequest(url, Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpPost]
        [Route("create")]
        public async Task<ActionResult> Create([FromBody] Producto body)
        {
            if (body.categorias.Length != body.categorias.Distinct().Count())
                return BadRequest("Categoria de producto duplicada");
            try
            {
                var solicitud = new RestRequest("productos/create", Method.Post);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpPatch]
        [Route("cambiarestado/{id}")]
        public async Task<ActionResult> CambiarEstado(int id)
        {
            try
            {
                var solicitud = new RestRequest($"productos/cambiarestado{id}", Method.Patch);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch(Exception ex) 
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }
    }
}
