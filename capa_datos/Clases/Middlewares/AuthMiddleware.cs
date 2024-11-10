namespace capa_datos.Clases.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private const string HEADER_API_KEY = "SECRET_API_KEY";
        private string SECRET_API_KEY;

        public AuthMiddleware(RequestDelegate next, IConfiguration configuracion)
        {
            _next = next;
            SECRET_API_KEY = configuracion["SecretApiKey:key"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HEADER_API_KEY, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                return;
            }
            if (!SECRET_API_KEY.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403;
                return;
            }
            await _next(context);
        }
    }
}
