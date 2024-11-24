using RestSharp;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace capa_negocio.Clases.Helpers
{
    public static class Imagenes
    {
        private const string ImgBBApiUrl = "https://api.imgbb.com/1/upload";
        private const string ApiKey = "TU_API_KEY"; // Reemplaza con tu API key de ImgBB

        public static async Task<string> SubirImagen(string base64Image)
        {
            try
            {
                var client = new RestClient(ImgBBApiUrl);
                var request = new RestRequest("", Method.Post);

                // Agrega los parámetros necesarios
                request.AddParameter("key", ApiKey);
                request.AddParameter("image", base64Image);

                // Realiza la solicitud
                var response = await client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    // Deserializar la respuesta JSON
                    var jsonResponse = JsonSerializer.Deserialize<ImgBBResponse>(response.Content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return jsonResponse?.Data?.Url ?? throw new Exception("No se pudo obtener la URL de la imagen.");
                }
                else
                {
                    throw new Exception($"Error al subir la imagen: {response.StatusCode} - {response.Content}");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
    public class ImgBBResponse
    {
        [JsonPropertyName("data")]
        public ImgBBData Data { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("success")]
        public bool Success { get; set; }
    }

    public class ImgBBData
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("delete_url")]
        public string DeleteUrl { get; set; }
    }
}
