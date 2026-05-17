using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using CineTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Infrastructure.Repositories;

public class BoletoRepository : IBoletoRepository
{
    private readonly AppDbContext _context;

    public BoletoRepository(AppDbContext context) => _context = context;

    public async Task<Ticket?> ObtenerPorIdAsync(int id) =>
        await _context.Boletos
            .Include(b => b.Funcion).ThenInclude(f => f.Pelicula)
            .Include(b => b.Funcion).ThenInclude(f => f.Sala).ThenInclude(s => s.Cine)
            .Include(b => b.Asiento)
            .FirstOrDefaultAsync(b => b.Id == id);

    public async Task<IEnumerable<Ticket>> ObtenerPorEmailClienteAsync(string email) =>
        await _context.Boletos
            .Include(b => b.Funcion).ThenInclude(f => f.Pelicula)
            .Include(b => b.Funcion).ThenInclude(f => f.Sala).ThenInclude(s => s.Cine)
            .Include(b => b.Asiento)
            .Where(b => b.EmailCliente.ToLower() == email.ToLower())
            .OrderByDescending(b => b.FechaCompra)
            .ToListAsync();

    public async Task<bool> AsientoOcupadoAsync(int funcionId, int asientoId) =>
        await _context.Boletos
            .AnyAsync(b => b.FuncionId == funcionId && b.AsientoId == asientoId && b.Estado == EstadoBoleto.Confirmado);

    public async Task<Ticket> AgregarAsync(Ticket boleto)
    {
        _context.Boletos.Add(boleto);
        await _context.SaveChangesAsync();
        return boleto;
    }

    public async Task<bool> CancelarAsync(int id)
    {
        var boleto = await _context.Boletos.FindAsync(id);
        if (boleto is null) return false;
        boleto.Estado = EstadoBoleto.Cancelado;
        await _context.SaveChangesAsync();
        return true;
    }
}
