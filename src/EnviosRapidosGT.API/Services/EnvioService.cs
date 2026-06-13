using Microsoft.EntityFrameworkCore;
using EnviosRapidosGT.API.Data;
using EnviosRapidosGT.API.DTOs;
using EnviosRapidosGT.API.Models;

namespace EnviosRapidosGT.API.Services;

public interface IEnvioService
{
    Task<EnvioResponse> CrearEnvioAsync(CrearEnvioRequest request);
    Task<EnvioResponse?> ObtenerEnvioAsync(int id);
    Task<EnvioResponse?> RastrearPorCodigoAsync(string codigo);
    Task<List<EnvioResponse>> ObtenerTodosAsync();
    Task<EnvioResponse?> ActualizarEstadoAsync(int id, ActualizarEstadoRequest request);
    Task<ReporteEficienciaResponse> GenerarReporteAsync();
    string GenerarCodigoRastreo();
    decimal CalcularTarifa(decimal pesoKg);
}

public class EnvioService : IEnvioService
{
    private readonly AppDbContext _db;
    private static readonly string[] EstadosValidos =
        ["Registrado", "EnTransito", "EnReparto", "Entregado", "EnDevolucion", "Devuelto"];

    // Transiciones permitidas
    private static readonly Dictionary<string, string[]> Transiciones = new()
    {
        ["Registrado"]    = ["EnTransito"],
        ["EnTransito"]    = ["EnReparto"],
        ["EnReparto"]     = ["Entregado", "EnDevolucion"],
        ["EnDevolucion"]  = ["Devuelto"],
        ["Entregado"]     = [],
        ["Devuelto"]      = []
    };

    public EnvioService(AppDbContext db) => _db = db;

    public string GenerarCodigoRastreo()
    {
        var fecha = DateTime.UtcNow.ToString("yyyyMMdd");
        var random = new Random().Next(1000, 9999);
        return $"ENV-{fecha}-{random}";
    }

    public decimal CalcularTarifa(decimal pesoKg)
    {
        return pesoKg switch
        {
            <= 1m => 25.00m,
            <= 5m => 45.00m,
            <= 10m => 75.00m,
            _ => 100.00m
        };
    }

    public async Task<EnvioResponse> CrearEnvioAsync(CrearEnvioRequest req)
    {
        var tarifa = CalcularTarifa(req.PesoKg);
        var tarifaFinal = req.TieneNit ? tarifa * 0.95m : tarifa;

        var envio = new Envio
        {
            CodigoRastreo = GenerarCodigoRastreo(),
            PesoKg = req.PesoKg,
            Tarifa = tarifa,
            TarifaFinal = Math.Round(tarifaFinal, 2),
            TieneNit = req.TieneNit,
            RemitenteNombre = req.RemitenteNombre,
            DestinatarioNombre = req.DestinatarioNombre,
            OficinaOrigen = req.OficinaOrigen,
            OficinaDestino = req.OficinaDestino,
            Estado = "Registrado"
        };

        envio.Historial.Add(new HistorialEstado
        {
            Estado = "Registrado",
            Ubicacion = req.OficinaOrigen,
            Notas = "Envío registrado en el sistema",
            Timestamp = DateTime.UtcNow
        });

        _db.Envios.Add(envio);
        await _db.SaveChangesAsync();
        return MapToResponse(envio);
    }

    public async Task<EnvioResponse?> ObtenerEnvioAsync(int id)
    {
        var envio = await _db.Envios.Include(e => e.Historial).FirstOrDefaultAsync(e => e.Id == id);
        return envio is null ? null : MapToResponse(envio);
    }

    public async Task<EnvioResponse?> RastrearPorCodigoAsync(string codigo)
    {
        var envio = await _db.Envios.Include(e => e.Historial)
            .FirstOrDefaultAsync(e => e.CodigoRastreo == codigo);
        return envio is null ? null : MapToResponse(envio);
    }

    public async Task<List<EnvioResponse>> ObtenerTodosAsync()
    {
        var envios = await _db.Envios.Include(e => e.Historial).ToListAsync();
        return envios.Select(MapToResponse).ToList();
    }

    public async Task<EnvioResponse?> ActualizarEstadoAsync(int id, ActualizarEstadoRequest req)
    {
        var envio = await _db.Envios.Include(e => e.Historial).FirstOrDefaultAsync(e => e.Id == id);
        if (envio is null) return null;

        // Validar transición
        if (!Transiciones.TryGetValue(envio.Estado, out var siguientes) ||
            !siguientes.Contains(req.NuevoEstado))
            throw new InvalidOperationException(
                $"No se puede cambiar de '{envio.Estado}' a '{req.NuevoEstado}'.");

        // Manejo de intentos de entrega
        if (req.NuevoEstado == "EnReparto")
        {
            envio.IntentosEntrega++;
            if (envio.IntentosEntrega >= 3)
            {
                envio.Estado = "EnDevolucion";
                envio.Historial.Add(new HistorialEstado
                {
                    Estado = "EnDevolucion",
                    Ubicacion = req.Ubicacion,
                    Notas = "Máximo de intentos alcanzado (3). Enviado a devolución automáticamente.",
                    Timestamp = DateTime.UtcNow
                });
                await _db.SaveChangesAsync();
                return MapToResponse(envio);
            }
        }

        envio.Estado = req.NuevoEstado;
        envio.Historial.Add(new HistorialEstado
        {
            Estado = req.NuevoEstado,
            Ubicacion = req.Ubicacion,
            Notas = req.Notas,
            Timestamp = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
        return MapToResponse(envio);
    }

    public async Task<ReporteEficienciaResponse> GenerarReporteAsync()
    {
        var envios = await _db.Envios.ToListAsync();
        var total = envios.Count;
        var entregados = envios.Count(e => e.Estado == "Entregado");
        var devueltos = envios.Count(e => e.Estado == "Devuelto");
        var enProceso = total - entregados - devueltos;
        var porcentaje = total == 0 ? 0 : Math.Round((decimal)entregados / total * 100, 2);
        var totalRecaudado = envios.Where(e => e.Estado == "Entregado").Sum(e => e.TarifaFinal);

        return new ReporteEficienciaResponse(
            total, entregados, devueltos, enProceso, porcentaje, totalRecaudado);
    }

    private static EnvioResponse MapToResponse(Envio e) => new(
        e.Id,
        e.CodigoRastreo,
        e.Estado,
        e.PesoKg,
        e.Tarifa,
        e.TarifaFinal,
        e.TieneNit,
        e.IntentosEntrega,
        e.RemitenteNombre,
        e.DestinatarioNombre,
        e.OficinaOrigen,
        e.OficinaDestino,
        e.FechaRegistro,
        e.Historial.OrderBy(h => h.Timestamp)
            .Select(h => new HistorialResponse(h.Id, h.Estado, h.Ubicacion, h.Notas, h.Timestamp))
            .ToList()
    );
}
