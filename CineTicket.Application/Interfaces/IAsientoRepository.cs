using CineTicket.Domain.Models;

namespace CineTicket.Application.Interfaces;

public interface IAsientoRepository
{
    Task<IEnumerable<Seat>> ObtenerPorSalaAsync(int salaId);
    Task<IEnumerable<Seat>> ObtenerDisponiblesPorFuncionAsync(int funcionId);
    Task<Seat?> ObtenerPorIdAsync(int id);
}
