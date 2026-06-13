using Microsoft.EntityFrameworkCore;
using EnviosRapidosGT.API.Models;

namespace EnviosRapidosGT.API.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Envio> Envios => Set<Envio>();
    public DbSet<HistorialEstado> HistorialEstados => Set<HistorialEstado>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Envio>()
            .HasMany(e => e.Historial)
            .WithOne(h => h.Envio)
            .HasForeignKey(h => h.EnvioId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Envio>()
            .Property(e => e.Tarifa)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Envio>()
            .Property(e => e.TarifaFinal)
            .HasColumnType("decimal(10,2)");

        modelBuilder.Entity<Envio>()
            .Property(e => e.PesoKg)
            .HasColumnType("decimal(10,2)");
    }
}
