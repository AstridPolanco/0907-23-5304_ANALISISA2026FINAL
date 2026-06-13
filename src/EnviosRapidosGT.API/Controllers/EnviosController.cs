using Microsoft.AspNetCore.Mvc;
using EnviosRapidosGT.API.DTOs;
using EnviosRapidosGT.API.Services;

namespace EnviosRapidosGT.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EnviosController : ControllerBase
{
    private readonly IEnvioService _service;

    public EnviosController(IEnvioService service) => _service = service;

    /// <summary>HU-01: Registrar un nuevo envío</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EnvioResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Crear([FromBody] CrearEnvioRequest request)
    {
        if (request.PesoKg <= 0) return BadRequest("El peso debe ser mayor a 0.");
        if (string.IsNullOrWhiteSpace(request.RemitenteNombre))
            return BadRequest("El nombre del remitente es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.DestinatarioNombre))
            return BadRequest("El nombre del destinatario es obligatorio.");

        var result = await _service.CrearEnvioAsync(request);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = result.Id }, result);
    }

    /// <summary>HU-02: Consultar todos los envíos</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EnvioResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerTodos() =>
        Ok(await _service.ObtenerTodosAsync());

    /// <summary>HU-03: Obtener envío por ID</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(EnvioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var envio = await _service.ObtenerEnvioAsync(id);
        return envio is null ? NotFound($"Envío con ID {id} no encontrado.") : Ok(envio);
    }

    /// <summary>HU-04: Rastrear envío por código</summary>
    [HttpGet("rastreo/{codigo}")]
    [ProducesResponseType(typeof(EnvioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Rastrear(string codigo)
    {
        var envio = await _service.RastrearPorCodigoAsync(codigo);
        return envio is null ? NotFound($"Código de rastreo '{codigo}' no encontrado.") : Ok(envio);
    }

    /// <summary>HU-05: Actualizar estado del envío</summary>
    [HttpPut("{id:int}/estado")]
    [ProducesResponseType(typeof(EnvioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ActualizarEstado(int id, [FromBody] ActualizarEstadoRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.NuevoEstado))
            return BadRequest("El nuevo estado es obligatorio.");
        if (string.IsNullOrWhiteSpace(request.Ubicacion))
            return BadRequest("La ubicación es obligatoria.");

        try
        {
            var result = await _service.ActualizarEstadoAsync(id, request);
            return result is null ? NotFound($"Envío con ID {id} no encontrado.") : Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>HU-10: Generar reporte de eficiencia</summary>
    [HttpGet("reporte/eficiencia")]
    [ProducesResponseType(typeof(ReporteEficienciaResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> ReporteEficiencia() =>
        Ok(await _service.GenerarReporteAsync());
}
