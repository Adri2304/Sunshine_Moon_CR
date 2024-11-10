using Microsoft.AspNetCore.Mvc;
using capa_datos.Clases;

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
            return Ok();
        }
    }
}
