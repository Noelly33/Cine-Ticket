using CineTicket.Domain.Models;

namespace CineTicket.Application.Interfaces;

public interface IPeliculaRepository
{
    Task<IEnumerable<Movie>> ObtenerTodosAsync(bool incluirInactivos = false);
    Task<Movie?> ObtenerPorIdAsync(int id);
    Task<Movie> AgregarAsync(Movie pelicula);
    Task<Movie?> ActualizarAsync(int id, Movie pelicula);
    Task<bool> DesactivarAsync(int id);
}
