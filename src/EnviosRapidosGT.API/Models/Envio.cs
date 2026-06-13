namespace EnviosRapidosGT.API.Models;

public class Envio
{
    public int Id { get; set; }
    public string CodigoRastreo { get; set; } = string.Empty;
    public string Estado { get; set; } = "Registrado";
    public decimal PesoKg { get; set; }
    public decimal Tarifa { get; set; }
    public bool TieneNit { get; set; }
    public decimal TarifaFinal { get; set; }
    public int IntentosEntrega { get; set; } = 0;
    public string RemitenteNombre { get; set; } = string.Empty;
    public string DestinatarioNombre { get; set; } = string.Empty;
    public string OficinaOrigen { get; set; } = string.Empty;
    public string OficinaDestino { get; set; } = string.Empty;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;
    public List<HistorialEstado> Historial { get; set; } = new();
}

public class HistorialEstado
{
    public int Id { get; set; }
    public int EnvioId { get; set; }
    public string Estado { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public string? Notas { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public Envio Envio { get; set; } = null!;
}
