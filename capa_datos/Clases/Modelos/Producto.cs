using System.Reflection.Metadata.Ecma335;

namespace capa_datos.Clases.Modelos
{
    public class Producto
    {
        public int? idProducto { get; set; } // Corresponde a idProducto en la tabla
        public int idEstadoProducto { get; set; } // Corresponde a idEstadoProducto en la tabla
        public string nombre { get; set; } // Corresponde a nombre en la tabla
        public double precio { get; set; } // Corresponde a precio en la tabla
        public int cantidad { get; set; } // Corresponde a cantidad en la tabla
        public string descripcion { get; set; } // Corresponde a descripcion en la tabla
        public string medidas { get; set; } // Corresponde a medidas en la tabla
        public string materiales { get; set; } // Corresponde a materiales en la tabla
        public DateTime fechaRegistro { get; set; } // Corresponde a fechaRegistro en la tabla
        public byte[] imagen { get; set; } // Corresponde a imagen en la tabla (varbinary)

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idEstadoProducto", idEstadoProducto },
                { "nombre", nombre },
                { "precio", precio },
                { "cantidad", cantidad },
                { "descripcion", descripcion },
                { "medidas", medidas },
                { "fechaRegistro", fechaRegistro },
                { "imagen", imagen },
            };

            if (idProducto != null)
            {
                diccionario.Add("idProducto", idProducto);
            }
            return diccionario;
        }
    }
}
