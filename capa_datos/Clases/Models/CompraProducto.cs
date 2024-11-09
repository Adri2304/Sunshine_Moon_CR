namespace capa_datos.Clases.Models
{
    public class CompraProducto
    {
        public int? IdProductoComprado { get; set; }
        public int IdCompra { get; set; }
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Precio { get; set; }
        public decimal TotalPrecio { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idCompra", IdCompra },
                { "idProducto", IdProducto },
                { "cantidad", Cantidad },
                { "precio", Precio },
                { "totalPrecio", TotalPrecio }
            };

            if (IdProductoComprado.HasValue)
                diccionario.Add("idProductoComprado", IdProductoComprado);

            return diccionario;
        }
    }
}
