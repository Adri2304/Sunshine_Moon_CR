using capa_negocio.Clases;
using Microsoft.AspNetCore.Mvc;
using capa_negocio.Clases.Models;
using RestSharp;

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
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
