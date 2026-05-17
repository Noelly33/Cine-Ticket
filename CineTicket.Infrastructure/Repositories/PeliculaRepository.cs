using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using CineTicket.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CineTicket.Infrastructure.Repositories;

public class PeliculaRepository : IPeliculaRepository
{
    private readonly AppDbContext _context;

    public PeliculaRepository(AppDbContext context) => _context = context;

    public async Task<IEnumerable<Movie>> ObtenerTodosAsync(bool incluirInactivos = false)
    {
        var query = _context.Peliculas.AsQueryable();
        if (!incluirInactivos)
            query = query.Where(m => m.EstaActivo);
        return await query.OrderBy(m => m.Titulo).ToListAsync();
    }

    public async Task<Movie?> ObtenerPorIdAsync(int id) =>
        await _context.Peliculas.FindAsync(id);

    public async Task<Movie> AgregarAsync(Movie pelicula)
    {
        _context.Peliculas.Add(pelicula);
        await _context.SaveChangesAsync();
        return pelicula;
    }

    public async Task<Movie?> ActualizarAsync(int id, Movie actualizado)
    {
        var pelicula = await _context.Peliculas.FindAsync(id);
        if (pelicula is null) return null;

        pelicula.Titulo = actualizado.Titulo;
        pelicula.Descripcion = actualizado.Descripcion;
        pelicula.DuracionMinutos = actualizado.DuracionMinutos;
        pelicula.Genero = actualizado.Genero;
        pelicula.Clasificacion = actualizado.Clasificacion;
        pelicula.UrlPoster = actualizado.UrlPoster;
        pelicula.EstaActivo = actualizado.EstaActivo;

        await _context.SaveChangesAsync();
        return pelicula;
    }

    public async Task<bool> DesactivarAsync(int id)
    {
        var pelicula = await _context.Peliculas.FindAsync(id);
        if (pelicula is null) return false;
        pelicula.EstaActivo = false;
        await _context.SaveChangesAsync();
        return true;
    }
}
