namespace EnviosRapidosGT.API.DTOs;

public record CrearEnvioRequest(
    decimal PesoKg,
    string RemitenteNombre,
    string DestinatarioNombre,
    string OficinaOrigen,
    string OficinaDestino,
    bool TieneNit = false
);

public record ActualizarEstadoRequest(
    string NuevoEstado,
    string Ubicacion,
    string? Notas = null
);

public record EnvioResponse(
    int Id,
    string CodigoRastreo,
    string Estado,
    decimal PesoKg,
    decimal Tarifa,
    decimal TarifaFinal,
    bool TieneNit,
    int IntentosEntrega,
    string RemitenteNombre,
    string DestinatarioNombre,
    string OficinaOrigen,
    string OficinaDestino,
    DateTime FechaRegistro,
    List<HistorialResponse> Historial
);

public record HistorialResponse(
    int Id,
    string Estado,
    string Ubicacion,
    string? Notas,
    DateTime Timestamp
);

public record ReporteEficienciaResponse(
    int TotalEnvios,
    int Entregados,
    int Devueltos,
    int EnProceso,
    decimal PorcentajeEntrega,
    decimal TotalRecaudado
);
