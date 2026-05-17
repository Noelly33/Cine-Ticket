using CineTicket.Domain.Models;

namespace CineTicket.Application.Interfaces;

public interface IBoletoRepository
{
    Task<Ticket?> ObtenerPorIdAsync(int id);
    Task<IEnumerable<Ticket>> ObtenerPorEmailClienteAsync(string email);
    Task<bool> AsientoOcupadoAsync(int funcionId, int asientoId);
    Task<Ticket> AgregarAsync(Ticket boleto);
    Task<bool> CancelarAsync(int id);
}
