using Microsoft.IdentityModel.Tokens;
using RestSharp;

namespace capa_negocio.Clases
{
    public class Solicitudes
    {
        private readonly string SERVER;
        private readonly string HeaderName;
        private readonly string HeaderValue;
        private readonly RestClient cliente;

        public Solicitudes(IConfiguration configuracion)
        {
            this.SERVER = configuracion["SERVER"];
            this.HeaderName = "SECRET_API_KEY";
            this.HeaderValue = configuracion["SECRET_API_KEY"];
            this.cliente = new RestClient(SERVER);
        }

        public async Task<RestResponse> EjecutarSolicitud(RestRequest request, Dictionary<string, object>? body = null)
        {
            try
            {
                if (!body.IsNullOrEmpty())
                {
                    request.AddJsonBody(body);
                }
                request.AddHeader(HeaderName, HeaderValue);
                var response = await cliente.ExecuteAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
