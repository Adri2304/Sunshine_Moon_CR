using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace capa_datos.Clases.Models
{
    public class Carrito
    {
        public int? idCarrito { get; set; }
        public int idUsuario { get; set; }
        public int idProducto { get; set; }
        public int cantidad { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idUsuario", idUsuario },
                { "idProducto", idProducto },
                { "cantidad", cantidad }
            };

            if (idCarrito != null)
                diccionario.Add("idCarrito", idCarrito);

            return diccionario;
        }
    }
}
