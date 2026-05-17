using CineTicket.Application.DTOs;
using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FuncionesController : ControllerBase
{
    private readonly IFuncionRepository _funciones;
    private readonly IAsientoRepository _asientos;

    public FuncionesController(IFuncionRepository funciones, IAsientoRepository asientos)
    {
        _funciones = funciones;
        _asientos = asientos;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos([FromQuery] int? peliculaId = null)
    {
        var funciones = await _funciones.ObtenerTodosAsync(peliculaId);
        return Ok(funciones.Select(ToDto));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var funcion = await _funciones.ObtenerPorIdAsync(id);
        return funcion is null ? NotFound() : Ok(ToDto(funcion));
    }

    [HttpGet("{id}/asientos")]
    public async Task<IActionResult> ObtenerAsientos(int id)
    {
        var funcion = await _funciones.ObtenerPorIdAsync(id);
        if (funcion is null) return NotFound();

        var disponibles = await _asientos.ObtenerDisponiblesPorFuncionAsync(id);
        var todos = await _asientos.ObtenerPorSalaAsync(funcion.SalaId);
        var idsDisponibles = disponibles.Select(a => a.Id).ToHashSet();

        return Ok(todos.Select(a => new AsientoDto
        {
            Id = a.Id,
            Fila = a.Fila,
            Numero = a.Numero,
            Etiqueta = $"{a.Fila}{a.Numero}",
            EstaDisponible = idsDisponibles.Contains(a.Id)
        }));
    }

    [HttpPost]
    public async Task<IActionResult> Crear(CrearFuncionDto dto)
    {
        var funcion = new Showtime
        {
            PeliculaId = dto.PeliculaId,
            SalaId = dto.SalaId,
            HoraInicio = dto.HoraInicio,
            Precio = dto.Precio
        };
        var creada = await _funciones.AgregarAsync(funcion);
        var cargada = await _funciones.ObtenerPorIdAsync(creada.Id);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creada.Id }, ToDto(cargada!));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Desactivar(int id)
    {
        var resultado = await _funciones.DesactivarAsync(id);
        return resultado ? NoContent() : NotFound();
    }

    private static FuncionDto ToDto(Showtime f)
    {
        var totalAsientos = f.Sala.Asientos.Count;
        var ocupados = f.Boletos.Count(b => b.Estado == EstadoBoleto.Confirmado);
        return new FuncionDto
        {
            Id = f.Id,
            PeliculaId = f.PeliculaId,
            TituloPelicula = f.Pelicula.Titulo,
            SalaId = f.SalaId,
            NombreSala = f.Sala.Nombre,
            CineId = f.Sala.CineId,
            NombreCine = f.Sala.Cine.Nombre,
            HoraInicio = f.HoraInicio,
            Precio = f.Precio,
            EstaActivo = f.EstaActivo,
            AsientosDisponibles = totalAsientos - ocupados
        };
    }
}
