using CineTicket.Application.DTOs;
using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CineTicket.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BoletosController : ControllerBase
{
    private readonly IBoletoRepository _boletos;
    private readonly IFuncionRepository _funciones;
    private readonly IAsientoRepository _asientos;

    public BoletosController(IBoletoRepository boletos, IFuncionRepository funciones, IAsientoRepository asientos)
    {
        _boletos = boletos;
        _funciones = funciones;
        _asientos = asientos;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(int id)
    {
        var boleto = await _boletos.ObtenerPorIdAsync(id);
        return boleto is null ? NotFound() : Ok(ToDto(boleto));
    }

    [HttpGet("cliente/{email}")]
    public async Task<IActionResult> ObtenerPorCliente(string email)
    {
        var boletos = await _boletos.ObtenerPorEmailClienteAsync(email);
        return Ok(boletos.Select(ToDto));
    }

    [HttpPost]
    public async Task<IActionResult> Comprar(CrearBoletoDto dto)
    {
        var funcion = await _funciones.ObtenerPorIdAsync(dto.FuncionId);
        if (funcion is null) return BadRequest("La función no existe.");
        if (!funcion.EstaActivo) return BadRequest("La función no está disponible.");

        var asiento = await _asientos.ObtenerPorIdAsync(dto.AsientoId);
        if (asiento is null) return BadRequest("El asiento no existe.");
        if (asiento.SalaId != funcion.SalaId)
            return BadRequest("El asiento no pertenece a la sala de esta función.");

        if (await _boletos.AsientoOcupadoAsync(dto.FuncionId, dto.AsientoId))
            return Conflict("El asiento ya está reservado para esta función.");

        var boleto = new Ticket
        {
            FuncionId = dto.FuncionId,
            AsientoId = dto.AsientoId,
            NombreCliente = dto.NombreCliente,
            EmailCliente = dto.EmailCliente,
            FechaCompra = DateTime.UtcNow,
            PrecioTotal = funcion.Precio,
            Estado = EstadoBoleto.Confirmado
        };

        var creado = await _boletos.AgregarAsync(boleto);
        var cargado = await _boletos.ObtenerPorIdAsync(creado.Id);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, ToDto(cargado!));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Cancelar(int id)
    {
        var resultado = await _boletos.CancelarAsync(id);
        return resultado ? NoContent() : NotFound();
    }

    private static BoletoDto ToDto(Ticket b) => new()
    {
        Id = b.Id,
        FuncionId = b.FuncionId,
        TituloPelicula = b.Funcion.Pelicula.Titulo,
        HoraFuncion = b.Funcion.HoraInicio,
        NombreCine = b.Funcion.Sala.Cine.Nombre,
        NombreSala = b.Funcion.Sala.Nombre,
        AsientoId = b.AsientoId,
        Asiento = $"{b.Asiento.Fila}{b.Asiento.Numero}",
        NombreCliente = b.NombreCliente,
        EmailCliente = b.EmailCliente,
        FechaCompra = b.FechaCompra,
        Estado = b.Estado.ToString(),
        PrecioTotal = b.PrecioTotal
    };
}
