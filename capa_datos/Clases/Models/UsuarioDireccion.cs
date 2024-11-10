using capa_datos.Clases.Modelos;
using System.ComponentModel.DataAnnotations;

namespace capa_datos.Clases.Models
{
    public class UsuarioDireccion
    {
        // Propiedades del Usuario
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public string ApellidoUno { get; set; }
        public string ApellidoDos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Contrasenia { get; set; }
        public bool? EstadoCuenta { get; set; }

        // Propiedades de la Dirección
        public string Provincia { get; set; }
        public string Canton { get; set; }
        public string Distrito { get; set; }
        public string Barrio { get; set; }
        public string DireccionExacta { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
        {
            { "idRol", IdRol },
            { "nombre", Nombre },
            { "apellidoUno", ApellidoUno },
            { "apellidoDos", ApellidoDos },
            { "correo", Correo },
            { "telefono", Telefono },
            { "contrasenia", Contrasenia },
            { "provincia", Provincia },
            { "canton", Canton },
            { "distrito", Distrito },
            { "barrio", Barrio },
            { "direccionExacta", DireccionExacta }
        };
            if (EstadoCuenta.HasValue)
                diccionario.Add("estadoCuenta", EstadoCuenta);

            return diccionario;
        }
    }
}
