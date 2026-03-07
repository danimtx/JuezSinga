using System.Net.Http.Json;
using System.Text.Json;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.ExternalServices.Judge0.Modelos;
using Microsoft.Extensions.Options;

namespace Infrastructure.ExternalServices.Judge0;

/// <summary>
/// Implementación del servicio de evaluación utilizando RapidAPI y Judge0.
/// </summary>
public class ServicioJuezRapidApi : IServicioJuez
{
    private readonly HttpClient _clienteHttp;
    private readonly ConfiguracionJudge0 _configuracion;

    public ServicioJuezRapidApi(HttpClient clienteHttp, IOptions<ConfiguracionJudge0> opciones)
    {
        _clienteHttp = clienteHttp;
        _configuracion = opciones.Value;
    }

    public async Task<IEnumerable<Lenguaje>> ObtenerLenguajesAsync()
    {
        var peticion = new HttpRequestMessage(HttpMethod.Get, $"{_configuracion.BaseUrl}/languages");
        AgregarCabecerasRapidApi(peticion);

        var respuesta = await _clienteHttp.SendAsync(peticion);
        respuesta.EnsureSuccessStatusCode();

        var lenguajesJudge0 = await respuesta.Content.ReadFromJsonAsync<IEnumerable<RespuestaLenguajeJudge0>>();
        
        return lenguajesJudge0?.Select(l => new Lenguaje { CodigoJudge0 = l.Id, Nombre = l.Nombre }) ?? Enumerable.Empty<Lenguaje>();
    }

    public async Task<string> CrearEnvioAsync(Envio envio, float? limiteTiempo = null, int? limiteMemoriaKB = null, string? salidaEsperada = null)
    {
        var url = $"{_configuracion.BaseUrl}/submissions?base64_encoded=true&wait=false&fields=*";
        
        // Convertimos a Base64 internamente
        var codigoBase64 = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(envio.CodigoFuente));
        var entradaBase64 = !string.IsNullOrEmpty(envio.EntradaEstandar) 
            ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(envio.EntradaEstandar)) 
            : null;
        var salidaEsperadaBase64 = !string.IsNullOrEmpty(salidaEsperada)
            ? Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(salidaEsperada))
            : null;

        var cuerpo = new
        {
            source_code = codigoBase64,
            language_id = envio.LenguajeId,
            stdin = entradaBase64,
            expected_output = salidaEsperadaBase64,
            cpu_time_limit = limiteTiempo,
            memory_limit = limiteMemoriaKB
        };

        var peticion = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = JsonContent.Create(cuerpo)
        };
        AgregarCabecerasRapidApi(peticion);

        var respuesta = await _clienteHttp.SendAsync(peticion);
        respuesta.EnsureSuccessStatusCode();

        var resultado = await respuesta.Content.ReadFromJsonAsync<RespuestaEnvioCreadoJudge0>();
        return resultado?.Token ?? throw new Exception("No se pudo obtener el token del envío.");
    }

    public async Task<ResultadoEnvio?> ConsultarVeredictoAsync(string token)
    {
        var url = $"{_configuracion.BaseUrl}/submissions/{token}?base64_encoded=true&fields=*";
        var peticion = new HttpRequestMessage(HttpMethod.Get, url);
        AgregarCabecerasRapidApi(peticion);

        var respuesta = await _clienteHttp.SendAsync(peticion);
        respuesta.EnsureSuccessStatusCode();

        var veredicto = await respuesta.Content.ReadFromJsonAsync<RespuestaVeredictoJudge0>();

        if (veredicto == null) return null;

        // Decodificamos la salida de Base64 a texto plano para el Frontend
        var salidaPlana = !string.IsNullOrEmpty(veredicto.SalidaEstandar)
            ? System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(veredicto.SalidaEstandar))
            : null;

        var errorPlano = !string.IsNullOrEmpty(veredicto.SalidaCompilacion ?? veredicto.ErrorEjecucion)
            ? System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(veredicto.SalidaCompilacion ?? veredicto.ErrorEjecucion ?? ""))
            : null;

        return new ResultadoEnvio
        {
            SalidaEstandar = salidaPlana,
            MensajeError = errorPlano,
            Tiempo = float.TryParse(veredicto.Tiempo, out var t) ? t : null,
            Memoria = veredicto.Memoria,
            Estado = (EstadoVeredicto)(veredicto.Estado?.Id ?? (int)EstadoVeredicto.ErrorInterno)
        };
    }

    private void AgregarCabecerasRapidApi(HttpRequestMessage peticion)
    {
        peticion.Headers.Add("x-rapidapi-key", _configuracion.ApiKey);
        peticion.Headers.Add("x-rapidapi-host", _configuracion.Host);
    }
}
