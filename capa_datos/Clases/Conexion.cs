using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.Text.Json;

namespace capa_datos.Clases
{
    public class Conexion
    {
        private readonly String cadena_conexion;

        public Conexion(IConfiguration configuration)
        {
            this.cadena_conexion = configuration.GetConnectionString("database");
        }

        //Este metodo ejecuta las consultas, y devuelve filas de la base de datos (READ, FILTROS)
        public async Task<List<Dictionary<String, Object>>> EjecutarConsulta(SqlCommand comando)
        {
            List<Dictionary<String, Object>> resultado = new List<Dictionary<String, Object>>();
            try
            {
                using (SqlConnection conexion = new SqlConnection(this.cadena_conexion))
                {
                    await conexion.OpenAsync();
                    comando.Connection = conexion;

                    using (SqlDataReader lectura = await comando.ExecuteReaderAsync())
                    {
                        while (await lectura.ReadAsync())
                        {
                            Dictionary<String, Object> columnas = new Dictionary<String, Object>();

                            for (int i = 0; i < lectura.FieldCount; i++)
                            {
                                columnas.Add(lectura.GetName(i), lectura.GetValue(i));
                            }
                            resultado.Add(columnas);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return resultado;
        }

        //Este metodo sola mente ejecuta cambios en la base de datos (CREATE, DELETE, UPDATE)
        public async Task<int> EjecutarCambios(SqlCommand comando)
        {
            int filasAfectadas = 0;
            try
            {
                using (SqlConnection conexion = new SqlConnection(this.cadena_conexion))
                {
                    await conexion.OpenAsync();
                    comando.Connection = conexion;
                    filasAfectadas = await comando.ExecuteNonQueryAsync();
                }
            }
            catch (Exception ex)
            {
            }
            return filasAfectadas;
        }
    }
}
