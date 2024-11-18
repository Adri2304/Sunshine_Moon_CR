namespace capa_negocio.Clases.Models
{
    public class UpdateUsuario
    {
        // Propiedades del Usuario
        public string Nombre { get; set; }
        public string ApellidoUno { get; set; }
        public string ApellidoDos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }

        // Propiedades de la Dirección
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Barrio { get; set; }
        public string DireccionExacta { get; set; }
    }
}
