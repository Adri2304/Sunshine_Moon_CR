namespace capa_datos.Clases.Models
{
    public class Compra
    {
        public int? IdCompra { get; set; }
        public int IdUsuario { get; set; }
        public int IdEstadoCompra { get; set; }
        public string TipoEntrega { get; set; }
        public decimal SubTotal { get; set; }
        public decimal CostoEnvio { get; set; }
        public decimal Total { get; set; }
        public DateTime FechaCompra { get; set; }
        public string ImagenFactura { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idUsuario", IdUsuario },
                { "idEstadoCompra", IdEstadoCompra },
                { "tipoEntrega", TipoEntrega },
                { "subTotal", SubTotal },
                { "costoEnvio", CostoEnvio },
                { "total", Total },
                { "fechaCompra", FechaCompra },
                { "imagenFactura", ImagenFactura }
            };

            if (IdCompra.HasValue)
                diccionario.Add("idCompra", IdCompra);

            return diccionario;
        }
    }
}
