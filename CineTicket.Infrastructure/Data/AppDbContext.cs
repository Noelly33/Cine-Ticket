using CineTicket.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Movie> Peliculas => Set<Movie>();
    public DbSet<Theater> Cines => Set<Theater>();
    public DbSet<Room> Salas => Set<Room>();
    public DbSet<Seat> Asientos => Set<Seat>();
    public DbSet<Showtime> Funciones => Set<Showtime>();
    public DbSet<Ticket> Boletos => Set<Ticket>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Movie>(e =>
        {
            e.ToTable("Pelicula");
            e.Property(m => m.Titulo).HasMaxLength(200).IsRequired();
            e.Property(m => m.Genero).HasMaxLength(100);
            e.Property(m => m.Clasificacion).HasMaxLength(10);
        });

        modelBuilder.Entity<Theater>(e =>
        {
            e.ToTable("Cine");
            e.Property(t => t.Nombre).HasMaxLength(200).IsRequired();
            e.Property(t => t.Direccion).HasMaxLength(500);
            e.HasMany(t => t.Salas)
             .WithOne(r => r.Cine)
             .HasForeignKey(r => r.CineId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Room>(e =>
        {
            e.ToTable("Sala");
            e.Property(r => r.Nombre).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Seat>(e =>
        {
            e.ToTable("Asiento");
            e.Property(s => s.Fila).HasMaxLength(5).IsRequired();
            e.HasOne(s => s.Sala)
             .WithMany(r => r.Asientos)
             .HasForeignKey(s => s.SalaId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(s => new { s.SalaId, s.Fila, s.Numero }).IsUnique();
        });

        modelBuilder.Entity<Showtime>(e =>
        {
            e.ToTable("Funcion");
            e.Property(s => s.Precio).HasColumnType("decimal(10,2)");
            e.HasOne(s => s.Pelicula)
             .WithMany(m => m.Funciones)
             .HasForeignKey(s => s.PeliculaId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(s => s.Sala)
             .WithMany(r => r.Funciones)
             .HasForeignKey(s => s.SalaId)
             .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Ticket>(e =>
        {
            e.ToTable("Boleto");
            e.Property(t => t.NombreCliente).HasMaxLength(200).IsRequired();
            e.Property(t => t.EmailCliente).HasMaxLength(200).IsRequired();
            e.Property(t => t.PrecioTotal).HasColumnType("decimal(10,2)");
            e.Property(t => t.Estado).HasConversion<string>();
            e.HasOne(t => t.Funcion)
             .WithMany(s => s.Boletos)
             .HasForeignKey(t => t.FuncionId)
             .OnDelete(DeleteBehavior.Restrict);
            e.HasOne(t => t.Asiento)
             .WithMany(s => s.Boletos)
             .HasForeignKey(t => t.AsientoId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
