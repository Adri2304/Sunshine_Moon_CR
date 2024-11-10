namespace capa_datos.Clases.Models
{
    public class Direccion
    {
        public int? IdDireccion { get; set; }
        public int IdUsuario { get; set; }
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Barrio { get; set; }
        public string DireccionExacta { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idUsuario", IdUsuario },
                { "provincia", Provincia },
                { "canton", Canton },
                { "distrito", Distrito },
                { "barrio", Barrio },
                { "direccionExacta", DireccionExacta }
            };

            if (IdDireccion.HasValue)
                diccionario.Add("idDireccion", IdDireccion);

            return diccionario;
        }
    }
}
