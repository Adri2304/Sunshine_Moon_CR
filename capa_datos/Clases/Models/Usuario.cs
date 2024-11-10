using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace capa_datos.Clases.Modelos
{
    public class Usuario
    {
        public int? IdUsuario { get; set; }
        public int IdRol { get; set; }
        public string Nombre { get; set; }
        public string ApellidoUno { get; set; }
        public string ApellidoDos { get; set; }
        public string Correo { get; set; }
        public string Telefono { get; set; }
        public string Contrasenia { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public string TokenSesion { get; set; }
        public string Imagen { get; set; }
        public bool EstadoCuenta { get; set; }

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
                { "tokenSesion", TokenSesion },
                { "imagen", Imagen },
                { "estadoCuenta", EstadoCuenta }
            };

            if (IdUsuario.HasValue)
                diccionario.Add("idUsuario", IdUsuario);
            if (FechaRegistro.HasValue)
                diccionario.Add("fechaRegistro", FechaRegistro);

            return diccionario;
        }
    }
}
