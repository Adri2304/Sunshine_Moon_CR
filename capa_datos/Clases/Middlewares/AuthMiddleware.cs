namespace capa_datos.Clases.Middlewares
{
    public class AuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string HeaderApiKey;
        private readonly string SecretApiKey;

        public AuthMiddleware(RequestDelegate next, IConfiguration configuracion)
        {
            this._next = next;
            this.HeaderApiKey = "SECRET_API_KEY";
            this.SecretApiKey = configuracion["SECRET_API_KEY"];
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(HeaderApiKey, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                return;
            }
            if (!SecretApiKey.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 403;
                return;
            }
            await _next(context);
        }
    }
}
