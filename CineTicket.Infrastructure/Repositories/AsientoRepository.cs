using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using CineTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Infrastructure.Repositories;

public class AsientoRepository : IAsientoRepository
{
    private readonly AppDbContext _context;

    public AsientoRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Seat>> ObtenerPorSalaAsync(int salaId) =>
        await _context.Asientos
            .Where(a => a.SalaId == salaId)
            .OrderBy(a => a.Fila).ThenBy(a => a.Numero)
            .ToListAsync();

    public async Task<IEnumerable<Seat>> ObtenerDisponiblesPorFuncionAsync(int funcionId)
    {
        var funcion = await _context.Funciones
            .Include(f => f.Sala).ThenInclude(s => s.Asientos)
            .FirstOrDefaultAsync(f => f.Id == funcionId);

        if (funcion is null) return [];

        var asientosOcupados = await _context.Boletos
            .Where(b => b.FuncionId == funcionId && b.Estado == EstadoBoleto.Confirmado)
            .Select(b => b.AsientoId)
            .ToListAsync();

        return funcion.Sala.Asientos
            .Where(a => !asientosOcupados.Contains(a.Id))
            .OrderBy(a => a.Fila).ThenBy(a => a.Numero);
    }

    public async Task<Seat?> ObtenerPorIdAsync(int id) =>
        await _context.Asientos.FindAsync(id);
}
