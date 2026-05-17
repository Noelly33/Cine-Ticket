using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using CineTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Infrastructure.Repositories;

public class FuncionRepository : IFuncionRepository
{
    private readonly AppDbContext _context;

    public FuncionRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Showtime>> ObtenerTodosAsync(int? peliculaId = null)
    {
        var query = _context.Funciones
            .Include(f => f.Pelicula)
            .Include(f => f.Sala).ThenInclude(s => s.Cine)
            .Include(f => f.Sala).ThenInclude(s => s.Asientos)
            .Include(f => f.Boletos)
            .Where(f => f.EstaActivo)
            .AsQueryable();

        if (peliculaId.HasValue)
            query = query.Where(f => f.PeliculaId == peliculaId.Value);

        return await query.OrderBy(f => f.HoraInicio).ToListAsync();
    }

    public async Task<Showtime?> ObtenerPorIdAsync(int id) =>
        await _context.Funciones
            .Include(f => f.Pelicula)
            .Include(f => f.Sala).ThenInclude(s => s.Cine)
            .Include(f => f.Sala).ThenInclude(s => s.Asientos)
            .Include(f => f.Boletos)
            .FirstOrDefaultAsync(f => f.Id == id);

    public async Task<Showtime> AgregarAsync(Showtime funcion)
    {
        _context.Funciones.Add(funcion);
        await _context.SaveChangesAsync();
        return funcion;
    }

    public async Task<bool> DesactivarAsync(int id)
    {
        var funcion = await _context.Funciones.FindAsync(id);
        if (funcion is null) return false;
        funcion.EstaActivo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
