using Microsoft.AspNetCore.Mvc;
using RestSharp;
using capa_negocio.Clases;
using capa_negocio.Clases.Models;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using static System.Net.Mime.MediaTypeNames;

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
                if ((int)respuesta.StatusCode == 200)
                {
                    var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(respuesta.Content);
                    data.RemoveAll(d => "False".Equals(d["estadoProducto"]?.ToString()));//Eliminar los desactivados
                    return data.Count > 0 ? Ok(data) : NoContent();
                }
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, ex.Message); }
        }
        
        [HttpGet]
        [Route("all")]
        public async Task<ActionResult> ReadAll()
        {
            try
            {
                var solicitud = new RestRequest("productos/read", Method.Get);
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
                // Subir la imagen y obtener la URL

                var solicitud = new RestRequest("productos/create", Method.Post);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpPatch]
        [Route("update/{id}")]
        public async Task<ActionResult> Update([FromBody] Producto body, int id)
        {
            if (body.categorias.Length != body.categorias.Distinct().Count())
                return BadRequest("Categoria de producto duplicada");
            try
            {
                var solicitud = new RestRequest($"productos/update/{id}", Method.Patch);
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
                var solicitud = new RestRequest($"productos/cambiarestado/{id}", Method.Patch);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch(Exception ex) 
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpGet]
        [Route("filtrar")]
        public async Task<ActionResult> Filter()
        {
            try
            {
                var parametros = HttpContext.Request.QueryString.Value;
                var solicitud = new RestRequest($"productos/filtrar{parametros}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);

                if ((int)respuesta.StatusCode != 200)
                    return StatusCode((int)respuesta.StatusCode, respuesta.Content);

                var data = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(respuesta.Content);
                data.RemoveAll(d => "False".Equals(d["estadoProducto"]?.ToString()));//Eliminar los desactivados
                return data.Count > 0 ? Ok(data) : NoContent();
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }
    }
}
