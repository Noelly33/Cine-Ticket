using CineTicket.Domain.Models;

namespace CineTicket.Application.Interfaces;

public interface ICineRepository
{
    Task<IEnumerable<Theater>> ObtenerTodosAsync();
    Task<Theater?> ObtenerPorIdAsync(int id);
}
