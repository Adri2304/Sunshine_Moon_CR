using RestSharp;
using System.Text;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;


namespace capa_negocio.Clases.Helpers
{
    public  class Imagenes
    {
        private readonly string Server;
        private readonly string ApiKey; // Reemplaza con tu API key de ImgBB

        public Imagenes(IConfiguration configuration)
        {
            this.Server = configuration["IMG_SERVER"];
            this.ApiKey = configuration["IMG_API_KEY"];
        }

        public async Task<string> SubirImagen(string base64Image)
        {
            try
            {
                var client = new RestClient(Server);

                // Crea una solicitud POST
                var request = new RestRequest("", Method.Post);
                request.AddParameter("key", ApiKey);
                request.AddParameter("image", base64Image);

                var response = await client.ExecuteAsync(request);

                if ((int)response.StatusCode != 200)
                    throw new Exception("No se pudo subir la imagen");

                var jsonDocument = JsonDocument.Parse(response.Content);
                var url = jsonDocument.RootElement.GetProperty("data").GetProperty("url").GetString();
                return url;
            }
            catch (Exception ex)
            {
                throw new Exception("No se pudo subir la imagen: " + ex);
            }
        }
    }
}
