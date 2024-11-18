using capa_negocio.Clases;
using Microsoft.AspNetCore.Mvc;
using capa_negocio.Clases.Models;
using RestSharp;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Text.Json;

namespace capa_negocio.Controllers
{
    [ApiController]
    [Route("usuarios")]
    public class UsuariosController : ControllerBase
    {
        private readonly Solicitudes Solicitudes;

        public UsuariosController(IConfiguration configuracion)
        {
            this.Solicitudes = new Solicitudes(configuracion);
        }

        [HttpGet]
        [Route("read/{id?}")]
        public async Task<ActionResult> Read(int? id)
        {
            string url = "usuarios/read";

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
        public async Task<ActionResult> create([FromBody] UsuarioDireccion body)
        {
            try
            {
                body.Contrasenia = BCrypt.Net.BCrypt.HashPassword(body.Contrasenia);
                var solicitud = new RestRequest("usuarios/create", Method.Post);
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
                var solicitud = new RestRequest($"usuarios/cambiarestado/{id}", Method.Patch);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpPatch]
        [Route("cambiarimagen/{id}")]
        public async Task<ActionResult> CambiarImagen(int id, [FromBody] Dictionary<string, string> body)
        {
            if (!body.ContainsKey("imagen"))
                return BadRequest("Se necesita el campo imagen");
            try
            {
                var solicitud = new RestRequest($"usuarios/cambiarimagen/{id}", Method.Patch);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpGet]
        [Route("filtrar")]
        public async Task<ActionResult> Filtrar()
        {
            try
            {
                var parametros = HttpContext.Request.QueryString.Value;
                var solicutud = new RestRequest($"usuarios/filtrar{parametros}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicutud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch (Exception ex)
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }

        [HttpPatch]
        [Route("update/{id}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateUsuario body)
        {
            try
            {
                var solicitud = new RestRequest($"usuarios/update/{id}", Method.Patch);
                solicitud.AddJsonBody(body);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                return StatusCode((int)respuesta.StatusCode, respuesta.Content);
            }
            catch
            { return StatusCode(500, "Ocurrio un error en el servidor"); }
        }
    }
}
