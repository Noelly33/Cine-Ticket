using CineTicket.Application.DTOs;
using CineTicket.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CineTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CinesController : ControllerBase
{
    private readonly ICineRepository _cines;

    public CinesController(ICineRepository cines) => _cines = cines;

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var cines = await _cines.ObtenerTodosAsync();
        return Ok(cines.Select(c => new CineDto
        {
            Id = c.Id,
            Nombre = c.Nombre,
            Direccion = c.Direccion,
            Salas = c.Salas.Select(s => new SalaDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                TotalAsientos = s.Asientos.Count
            }).ToList()
        }));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var cine = await _cines.ObtenerPorIdAsync(id);
        if (cine is null) return NotFound();

        return Ok(new CineDto
        {
            Id = cine.Id,
            Nombre = cine.Nombre,
            Direccion = cine.Direccion,
            Salas = cine.Salas.Select(s => new SalaDto
            {
                Id = s.Id,
                Nombre = s.Nombre,
                TotalAsientos = s.Asientos.Count
            }).ToList()
        });
    }
}
