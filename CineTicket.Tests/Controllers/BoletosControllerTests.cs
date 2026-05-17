using CineTicket.API.Controllers;
using CineTicket.Application.DTOs;
using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CineTicket.Tests.Controllers;

public class BoletosControllerTests
{
    private readonly Mock<IBoletoRepository> _mockBoletos;
    private readonly Mock<IFuncionRepository> _mockFunciones;
    private readonly Mock<IAsientoRepository> _mockAsientos;
    private readonly BoletosController _controller;

    public BoletosControllerTests()
    {
        _mockBoletos = new Mock<IBoletoRepository>();
        _mockFunciones = new Mock<IFuncionRepository>();
        _mockAsientos = new Mock<IAsientoRepository>();
        _controller = new BoletosController(_mockBoletos.Object, _mockFunciones.Object, _mockAsientos.Object);
    }

    [Fact]
    public async Task ObtenerPorId_BoletoExiste_RetornaOk()
    {
        // Arrange
        var boleto = new Ticket
        {
            Id = 1,
            NombreCliente = "Juan Pérez",
            EmailCliente = "juan@test.com",
            FechaCompra = DateTime.UtcNow,
            Estado = EstadoBoleto.Confirmado,
            PrecioTotal = 9.50m,
            Funcion = new Showtime
            {
                Id = 1,
                HoraInicio = DateTime.UtcNow.AddDays(1),
                Pelicula = new Movie { Titulo = "Test" },
                Sala = new Room { Nombre = "Sala 1", Cine = new Theater { Nombre = "MetroCine" } }
            },
            Asiento = new Seat { Id = 1, Fila = "A", Numero = 1 }
        };
        _mockBoletos.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(boleto);

        // Act
        var result = await _controller.ObtenerPorId(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<BoletoDto>(ok.Value);
        Assert.Equal("Juan Pérez", dto.NombreCliente);
    }

    [Fact]
    public async Task ObtenerPorId_BoletoNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockBoletos.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Ticket?)null);

        // Act
        var result = await _controller.ObtenerPorId(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Comprar_FuncionNoExiste_RetornaBadRequest()
    {
        // Arrange
        var dto = new CrearBoletoDto { FuncionId = 99, AsientoId = 1, NombreCliente = "Test", EmailCliente = "test@test.com" };
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Showtime?)null);

        // Act
        var result = await _controller.Comprar(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("La función no existe.", bad.Value);
    }

    [Fact]
    public async Task Comprar_FuncionInactiva_RetornaBadRequest()
    {
        // Arrange
        var dto = new CrearBoletoDto { FuncionId = 1, AsientoId = 1, NombreCliente = "Test", EmailCliente = "test@test.com" };
        var funcion = new Showtime { Id = 1, SalaId = 1, EstaActivo = false };
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(funcion);

        // Act
        var result = await _controller.Comprar(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("La función no está disponible.", bad.Value);
    }

    [Fact]
    public async Task Comprar_AsientoNoExiste_RetornaBadRequest()
    {
        // Arrange
        var dto = new CrearBoletoDto { FuncionId = 1, AsientoId = 99, NombreCliente = "Test", EmailCliente = "test@test.com" };
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Showtime { Id = 1, SalaId = 1, EstaActivo = true });
        _mockAsientos.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Seat?)null);

        // Act
        var result = await _controller.Comprar(dto);

        // Assert
        Assert.IsType<BadRequestObjectResult>(result);
    }

    [Fact]
    public async Task Comprar_AsientoDeOtraSala_RetornaBadRequest()
    {
        // Arrange
        var dto = new CrearBoletoDto { FuncionId = 1, AsientoId = 1, NombreCliente = "Test", EmailCliente = "test@test.com" };
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Showtime { Id = 1, SalaId = 1, EstaActivo = true });
        _mockAsientos.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Seat { Id = 1, SalaId = 2 });

        // Act
        var result = await _controller.Comprar(dto);

        // Assert
        var bad = Assert.IsType<BadRequestObjectResult>(result);
        Assert.Equal("El asiento no pertenece a la sala de esta función.", bad.Value);
    }

    [Fact]
    public async Task Comprar_AsientoOcupado_RetornaConflict()
    {
        // Arrange
        var dto = new CrearBoletoDto { FuncionId = 1, AsientoId = 1, NombreCliente = "Test", EmailCliente = "test@test.com" };
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Showtime { Id = 1, SalaId = 1, Precio = 9.50m, EstaActivo = true });
        _mockAsientos.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(new Seat { Id = 1, SalaId = 1 });
        _mockBoletos.Setup(r => r.AsientoOcupadoAsync(1, 1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Comprar(dto);

        // Assert
        var conflict = Assert.IsType<ConflictObjectResult>(result);
        Assert.Equal("El asiento ya está reservado para esta función.", conflict.Value);
    }

    [Fact]
    public async Task Cancelar_BoletoExiste_RetornaNoContent()
    {
        // Arrange
        _mockBoletos.Setup(r => r.CancelarAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Cancelar(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Cancelar_BoletoNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockBoletos.Setup(r => r.CancelarAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Cancelar(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
