using CineTicket.Application.DTOs;
using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PeliculasController : ControllerBase
{
    private readonly IPeliculaRepository _peliculas;

    public PeliculasController(IPeliculaRepository peliculas) => _peliculas = peliculas;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] bool incluirInactivos = false)
    {
        var peliculas = await _peliculas.ObtenerTodosAsync(incluirInactivos);
        return Ok(peliculas.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var pelicula = await _peliculas.ObtenerPorIdAsync(id);
        return pelicula is null ? NotFound() : Ok(ToDto(pelicula));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearPeliculaDto dto)
    {
        var pelicula = new Movie
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            DuracionMinutos = dto.DuracionMinutos,
            Genero = dto.Genero,
            Clasificacion = dto.Clasificacion,
            UrlPoster = dto.UrlPoster
        };
        var creada = await _peliculas.AgregarAsync(pelicula);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, ToDto(creada));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(int id, ActualizarPeliculaDto dto)
    {
        var actualizada = await _peliculas.ActualizarAsync(id, new Movie
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            DuracionMinutos = dto.DuracionMinutos,
            Genero = dto.Genero,
            Clasificacion = dto.Clasificacion,
            UrlPoster = dto.UrlPoster,
            EstaActivo = dto.EstaActivo
        });
        return actualizada is null ? NotFound() : Ok(ToDto(actualizada));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var resultado = await _peliculas.DesactivarAsync(id);
        return resultado ? NoContent() : NotFound();
    }

    private static PeliculaDto ToDto(Movie m) => new()
    {
        Id = m.Id,
        Titulo = m.Titulo,
        Descripcion = m.Descripcion,
        DuracionMinutos = m.DuracionMinutos,
        Genero = m.Genero,
        Clasificacion = m.Clasificacion,
        UrlPoster = m.UrlPoster,
        EstaActivo = m.EstaActivo
    };
}
