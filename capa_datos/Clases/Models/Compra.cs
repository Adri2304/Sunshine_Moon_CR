namespace capa_datos.Clases.Models
{
    public class Compra
    {
        public int IdUsuario { set; get; }
        public string TipoEntrega { set; get; }
        public double  CostoEnvio{ set; get; }
        public string ImagenFactura { set; get; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idUsuario", IdUsuario },
                { "tipoEntrega", TipoEntrega },
                { "costoEnvio", CostoEnvio },
                { "imagenFactura", ImagenFactura }
            };
            return diccionario;
        }
    }
}
