using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using RestSharp;
using System.Text.Json;

namespace capa_negocio.Clases.Helpers
{
    public class Correos
    {
        private readonly string Servidor;
        private readonly int Puerto;
        private readonly string Remitente;
        private readonly string Contrasenia;
        private readonly string Nombre;
        private readonly Solicitudes Solicitudes;

        public Correos(IConfiguration configuracion)
        {
            this.Servidor = configuracion["EMAIL_SERVER"];
            this.Puerto = int.Parse(configuracion["EMAIL_PORT"]);
            this.Remitente = configuracion["EMAIL_SENDER"];
            this.Contrasenia = configuracion["EMAIL_PASSWORD"];
            this.Nombre = configuracion["EMAIL_SENDER_NAME"];
            this.Solicitudes = new Solicitudes(configuracion);
        }

        public async Task<bool> EnviarCorreo(string destinatario, string nombre, string encabezado, int tipoCorreo, int id, string imagen = "")
        {
            try
            {
                var mail = new MimeMessage();
                mail.From.Add(new MailboxAddress(Nombre, Remitente));
                mail.To.Add(new MailboxAddress(nombre, destinatario));
                mail.Subject = encabezado;

                string mensaje = await GenerarMensajes(tipoCorreo, id);
                var body = new BodyBuilder();

                if (!string.IsNullOrEmpty(imagen))
                {
                    // Se construye el adjunto
                    byte[] imageBytes = Convert.FromBase64String(imagen);
                    var adjunto = new MimePart("image", "jpeg")
                    {
                        Content = new MimeContent(new MemoryStream(imageBytes)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        FileName = "imagen.jpg"
                    };
                    body.Attachments.Add(adjunto);
                }

                body.HtmlBody = mensaje;
                mail.Body = body.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(Servidor, Puerto, SecureSocketOptions.StartTls);
                    await client.AuthenticateAsync(Remitente, Contrasenia);
                    await client.SendAsync(mail);
                    await client.DisconnectAsync(true);
                }
                return true;
            }
            catch (Exception)
            { return false; }
        }

        private async Task<string> GenerarMensajes(int tipoMensaje, int id)
        {
            string resultado = tipoMensaje switch
            {
                1 => await ConfirmacionCompra(id),
                2 => await CambioEstadoCompra(id),
                _ => throw new Exception("No se pudo generar el mensaje")
            };
            return resultado;
        }

        private async Task<string> ConfirmacionCompra(int id)
        {
            try
            {
                string tabla = "";
                string ruta = Path.Combine(Directory.GetCurrentDirectory(), "Clases", "Plantillas", "ConfirmacionCompra.html");
                string mensaje = File.ReadAllText(ruta);

                var solicitud = new RestRequest($"compras/readcompra/{id}", Method.Get);
                var respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                var infoCompra = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(respuesta.Content);
                //return "";
                solicitud = new RestRequest($"compras/detallecompra/{id}", Method.Get);
                respuesta = await Solicitudes.EjecutarSolicitud(solicitud);
                var detalleCompra = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(respuesta.Content);

                // Generar tabla con los detalles de la compra
                foreach (var producto in detalleCompra)
                {
                    tabla += $"<tr>";
                    foreach (var item in producto)
                    {
                        tabla += $"<td>{item.Value.ToString()}</td>";
                    }
                    tabla += $"</tr>";
                }
                mensaje = mensaje.Replace("{{cliente}}", infoCompra[0]["cliente"].ToString());
                mensaje = mensaje.Replace("{{idCompra}}", infoCompra[0]["idCompra"].ToString());
                mensaje = mensaje.Replace("{{detallesCompra}}", tabla);
                mensaje = mensaje.Replace("{{subTotal}}", infoCompra[0]["subTotal"].ToString());
                mensaje = mensaje.Replace("{{costoEnvio}}", infoCompra[0]["costoEnvio"].ToString());
                mensaje = mensaje.Replace("{{total}}", infoCompra[0]["total"].ToString());
                return mensaje;
            }
            catch(Exception)
            {
                throw new Exception("Fallo el envio del correo");
            }
        }

        private async Task<string> CambioEstadoCompra(int id)
        {
            try
            {
                string ruta = Path.Combine(Directory.GetCurrentDirectory(), "Clases", "Plantillas", "CambioEstadoPedido.html");
                string mensaje = File.ReadAllText(ruta);

                var _solicitud = new RestRequest($"compras/readcompra/{id}", Method.Get);
                var _respuesta = await Solicitudes.EjecutarSolicitud(_solicitud);
                var infoCompra = JsonSerializer.Deserialize<List<Dictionary<string, object>>>(_respuesta.Content);

                var fechaCompra = DateTime.Parse(infoCompra[0]["fechaCompra"].ToString());

                mensaje = mensaje.Replace("{{cliente}}", infoCompra[0]["cliente"].ToString());
                mensaje = mensaje.Replace("{{idCompra}}", infoCompra[0]["idCompra"].ToString());
                mensaje = mensaje.Replace("{{fecha}}", fechaCompra.ToShortDateString());
                mensaje = mensaje.Replace("{{hora}}", fechaCompra.ToShortTimeString());
                mensaje = mensaje.Replace("{{estadoPedido}}", infoCompra[0]["estado"].ToString());

                return mensaje;
            }
            catch (Exception)
            {
                throw new Exception("Fallo en el envio del correo");
            }
        }
    }
}
