using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace FrontBlazor.Services
{
    public class ServicioEntidad
    {
        private readonly HttpClient _clienteHttp;
        private readonly JsonSerializerOptions _opcionesJson;

        public ServicioEntidad(HttpClient clienteHttp)
        {
            _clienteHttp = clienteHttp;
            _opcionesJson = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
        }

        public async Task<List<Dictionary<string, object>>?> ObtenerTodosAsync(string nombreProyecto, string nombreTabla)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}";
                return await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener datos: {ex.Message}");
                return new List<Dictionary<string, object>>();
            }
        }

        public async Task<Dictionary<string, object>?> ObtenerPorClaveAsync(string nombreProyecto, string nombreTabla, string nombreClave, string valorClave)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                var resultado = await _clienteHttp.GetFromJsonAsync<List<Dictionary<string, object>>>(url);
                return resultado?.FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener entidad: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> CrearAsync(string nombreProyecto, string nombreTabla, Dictionary<string, object> entidad)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}";
                var contenido = new StringContent(
                    JsonSerializer.Serialize(entidad),
                    Encoding.UTF8,
                    "application/json");

                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear entidad: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> ActualizarAsync(string nombreProyecto, string nombreTabla, string nombreClave, string valorClave, Dictionary<string, object> entidad)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                var contenido = new StringContent(
                    JsonSerializer.Serialize(entidad),
                    Encoding.UTF8,
                    "application/json");

                var respuesta = await _clienteHttp.PutAsync(url, contenido);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar entidad: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> EliminarAsync(string nombreProyecto, string nombreTabla, string nombreClave, string valorClave)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreTabla}/{nombreClave}/{valorClave}";
                var respuesta = await _clienteHttp.DeleteAsync(url);
                return respuesta.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar entidad: {ex.Message}");
                return false;
            }
        }

        public Task<bool> InsertarAsync(string nombreProyecto, string nombreTabla, Dictionary<string, object> entidad)
        {
            return CrearAsync(nombreProyecto, nombreTabla, entidad);
        }

        /// <summary>
        /// Ejecuta un procedimiento almacenado genérico.
        /// </summary>
        /// <param name="nombreProyecto">Nombre del proyecto en la API.</param>
        /// <param name="nombreProcedimiento">Nombre del procedimiento almacenado (nombre del endpoint).</param>
        /// <param name="parametros">Parámetros del procedimiento como diccionario.</param>
        public async Task<List<Dictionary<string, object>>?> EjecutarProcedimientoAsync(
            string nombreProyecto,
            string nombreProcedimiento,
            Dictionary<string, object> parametros)
        {
            try
            {
                var url = $"{nombreProyecto}/{nombreProcedimiento}";
                var contenido = new StringContent(
                    JsonSerializer.Serialize(parametros),
                    Encoding.UTF8,
                    "application/json");

                var respuesta = await _clienteHttp.PostAsync(url, contenido);
                if (!respuesta.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Error al ejecutar procedimiento: {respuesta.StatusCode}");
                    return null;
                }

                var contenidoJson = await respuesta.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Dictionary<string, object>>>(contenidoJson, _opcionesJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Excepción al ejecutar procedimiento almacenado: {ex.Message}");
                return null;
            }
        }
    }
}
