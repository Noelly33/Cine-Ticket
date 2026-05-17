using CineTicket.Domain.Models;

namespace CineTicket.Infrastructure.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext context)
    {
        if (context.Peliculas.Any()) return;

        var cine = new Theater
        {
            Nombre = "MetroCine Premier",
            Direccion = "Av. Principal 123, Centro Comercial Village, Local 45"
        };
        context.Cines.Add(cine);
        await context.SaveChangesAsync();

        var sala1 = new Room { Nombre = "Sala 1", CineId = cine.Id };
        var sala2 = new Room { Nombre = "Sala 2", CineId = cine.Id };
        context.Salas.AddRange(sala1, sala2);
        await context.SaveChangesAsync();

        var asientosSala1 = GenerarAsientos(sala1.Id, ["A", "B", "C", "D", "E"], 8);
        var asientosSala2 = GenerarAsientos(sala2.Id, ["A", "B", "C", "D", "E", "F"], 10);
        context.Asientos.AddRange(asientosSala1.Concat(asientosSala2));
        await context.SaveChangesAsync();

        var peliculas = new List<Movie>
        {
            new() { Titulo = "Rebelión en Marte", Descripcion = "Los colonos marcianos inician una revolución por la independencia de la Tierra.", DuracionMinutos = 135, Genero = "Acción / Sci-Fi", Clasificacion = "PG-13" },
            new() { Titulo = "El Último Horizonte", Descripcion = "Un explorador descubre que el fin del mundo está más cerca de lo que todos piensan.", DuracionMinutos = 118, Genero = "Thriller", Clasificacion = "R" },
            new() { Titulo = "Amor en París", Descripcion = "Dos extraños se conocen en París y viven una aventura que cambia sus vidas para siempre.", DuracionMinutos = 102, Genero = "Romance", Clasificacion = "PG" },
            new() { Titulo = "Cartas bajo la Lluvia", Descripcion = "Un romance épico contado a través de correspondencia olvidada en un viejo ático.", DuracionMinutos = 122, Genero = "Terror / Misterio", Clasificacion = "R" },
        };
        context.Peliculas.AddRange(peliculas);
        await context.SaveChangesAsync();

        var hoy = DateTime.UtcNow.Date;
        var funciones = new List<Showtime>
        {
            new() { PeliculaId = peliculas[0].Id, SalaId = sala1.Id, HoraInicio = hoy.AddDays(1).AddHours(14), Precio = 9.50m },
            new() { PeliculaId = peliculas[0].Id, SalaId = sala1.Id, HoraInicio = hoy.AddDays(1).AddHours(18), Precio = 11.00m },
            new() { PeliculaId = peliculas[0].Id, SalaId = sala2.Id, HoraInicio = hoy.AddDays(2).AddHours(16), Precio = 14.00m },
            new() { PeliculaId = peliculas[1].Id, SalaId = sala2.Id, HoraInicio = hoy.AddDays(1).AddHours(20), Precio = 14.00m },
            new() { PeliculaId = peliculas[1].Id, SalaId = sala2.Id, HoraInicio = hoy.AddDays(3).AddHours(15), Precio = 14.00m },
            new() { PeliculaId = peliculas[2].Id, SalaId = sala1.Id, HoraInicio = hoy.AddDays(2).AddHours(12), Precio = 9.50m },
            new() { PeliculaId = peliculas[2].Id, SalaId = sala1.Id, HoraInicio = hoy.AddDays(4).AddHours(17), Precio = 9.50m },
            new() { PeliculaId = peliculas[3].Id, SalaId = sala1.Id, HoraInicio = hoy.AddDays(3).AddHours(21), Precio = 11.00m },
            new() { PeliculaId = peliculas[3].Id, SalaId = sala2.Id, HoraInicio = hoy.AddDays(5).AddHours(19), Precio = 14.00m },
        };
        context.Funciones.AddRange(funciones);
        await context.SaveChangesAsync();
    }

    private static List<Seat> GenerarAsientos(int salaId, string[] filas, int asientosPorFila)
    {
        var asientos = new List<Seat>();
        foreach (var fila in filas)
            for (int n = 1; n <= asientosPorFila; n++)
                asientos.Add(new Seat { Fila = fila, Numero = n, SalaId = salaId });
        return asientos;
    }
}
