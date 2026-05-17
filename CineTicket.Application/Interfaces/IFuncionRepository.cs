using CineTicket.Domain.Models;

namespace CineTicket.Application.Interfaces;

public interface IFuncionRepository
{
    Task<IEnumerable<Showtime>> ObtenerTodosAsync(int? peliculaId = null);
    Task<Showtime?> ObtenerPorIdAsync(int id);
    Task<Showtime> AgregarAsync(Showtime funcion);
    Task<bool> DesactivarAsync(int id);
}
