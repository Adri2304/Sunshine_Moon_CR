namespace capa_datos.Clases.Models
{
    public class CostoUbicacion
    {
        public int? IdUbicacion { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public decimal CostoEnvio { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "provincia", Provincia },
                { "canton", Canton },
                { "distrito", Distrito },
                { "costoEnvio", CostoEnvio }
            };

            if (IdUbicacion.HasValue)
                diccionario.Add("idUbicacion", IdUbicacion);

            return diccionario;
        }
    }
}
