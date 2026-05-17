using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using CineTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Infrastructure.Repositories;

public class CineRepository : ICineRepository
{
    private readonly AppDbContext _context;

    public CineRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Theater>> ObtenerTodosAsync() =>
        await _context.Cines
            .Include(c => c.Salas).ThenInclude(s => s.Asientos)
            .ToListAsync();

    public async Task<Theater?> ObtenerPorIdAsync(int id) =>
        await _context.Cines
            .Include(c => c.Salas).ThenInclude(s => s.Asientos)
            .FirstOrDefaultAsync(c => c.Id == id);
}
