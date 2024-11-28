using Microsoft.Extensions.Caching.Memory;
using OtpNet;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace capa_negocio.Clases.Helpers
{
    public class AuthDosPasos
    {
        private readonly IMemoryCache MemoriaCache;

        public AuthDosPasos(IMemoryCache memoriaCache)
        {
            this.MemoriaCache = memoriaCache;
        }

        public async Task<string> GenerarCodigo(string correo)
        {
            var claveSecreta = KeyGeneration.GenerateRandomKey(20);
            var totp = new Totp(claveSecreta, totpSize: 6);
            var codigo = totp.ComputeTotp(DateTime.UtcNow);

            var opcionesCache = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };
            MemoriaCache.Set(correo, claveSecreta, opcionesCache);
            return codigo;
        }

        public async Task<bool> VerificarCodigo(string correo, string codigo)
        {
            if (!MemoriaCache.TryGetValue(correo, out byte[] claveSecreta))
            {
                return false;
            }
            var totp = new Totp(claveSecreta);
            var esValido = totp.VerifyTotp(codigo, out _);

            if (esValido)
                MemoriaCache.Remove(correo);
            return esValido;
        }
    }
}
