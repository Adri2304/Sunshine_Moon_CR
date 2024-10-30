using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace capa_datos.Clases.Modelos
{
    public class Usuario
    {
        public int? idUsuario { get; set; }
        public int idRol { get; set; }
        public string nombre { get; set; }
        public string apellidoUno { get; set; }
        public string apellidoDos { get; set; }
        public string correo { get; set; }
        public string contrasenia { get; set; }
        public DateTime fechaRegistro { get; set; }
        public string token_sesion { get; set; }
        public byte[] imagen { get; set; }

        public Dictionary<string, object> DevolverDiccionario()
        {
            var diccionario = new Dictionary<string, object>
            {
                { "idRol", idRol },
                { "nombre", nombre },
                { "apellidoUno", apellidoUno },
                { "apellidoDos", apellidoDos },
                { "correo", correo },
                { "contrasenia", contrasenia },
                { "fechaRegistro", fechaRegistro },
                { "token_sesion", token_sesion },
                { "imagen", imagen },
            };

            if (idUsuario != null)
            {
                diccionario.Add("idUsuario", idUsuario);
            }
            return diccionario;
        }
    }
}
