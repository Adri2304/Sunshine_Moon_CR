namespace capa_negocio.Clases.Models
{
    public class Producto
    {
        public string nombre { get; set; } // Corresponde a nombre en la tabla
        public double precio { get; set; } // Corresponde a precio en la tabla
        public int cantidad { get; set; } // Corresponde a cantidad en la tabla
        public string descripcion { get; set; } // Corresponde a descripcion en la tabla
        public string medidas { get; set; } // Corresponde a medidas en la tabla
        public string materiales { get; set; } // Corresponde a materiales en la tabla
        public DateTime? fechaRegistro { get; set; } // Corresponde a fechaRegistro en la tabla
        public int[] categorias { get; set; }
        public string imagen { get; set; } // Corresponde a imagen en la tabla (varbinary)

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "nombre", nombre },
                { "precio", precio },
                { "cantidad", cantidad },
                { "descripcion", descripcion },
                { "medidas", medidas },
                { "materiales", materiales },
                { "categorias", categorias},
                { "imagen", imagen },
            };

            if (fechaRegistro != null)
                diccionario.Add("fechaRegistro", fechaRegistro);
            return diccionario;
        }
    }
}
