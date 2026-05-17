using CineTicket.API.Controllers;
using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CineTicket.Tests.Controllers;

public class FuncionesControllerTests
{
    private readonly Mock<IFuncionRepository> _mockFunciones;
    private readonly Mock<IAsientoRepository> _mockAsientos;
    private readonly FuncionesController _controller;

    public FuncionesControllerTests()
    {
        _mockFunciones = new Mock<IFuncionRepository>();
        _mockAsientos = new Mock<IAsientoRepository>();
        _controller = new FuncionesController(_mockFunciones.Object, _mockAsientos.Object);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkConListaVacia()
    {
        // Arrange
        _mockFunciones.Setup(r => r.ObtenerTodosAsync(null)).ReturnsAsync(new List<Showtime>());

        // Act
        var result = await _controller.ObtenerTodos();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task ObtenerTodos_FiltradoPorPelicula_LlamaRepositorioConId()
    {
        // Arrange
        _mockFunciones.Setup(r => r.ObtenerTodosAsync(3)).ReturnsAsync(new List<Showtime>());

        // Act
        var result = await _controller.ObtenerTodos(peliculaId: 3);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        _mockFunciones.Verify(r => r.ObtenerTodosAsync(3), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorId_FuncionNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Showtime?)null);

        // Act
        var result = await _controller.ObtenerPorId(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ObtenerAsientos_FuncionNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Showtime?)null);

        // Act
        var result = await _controller.ObtenerAsientos(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task ObtenerAsientos_FuncionExiste_RetornaListaDeAsientos()
    {
        // Arrange
        var funcion = new Showtime
        {
            Id = 1, SalaId = 1,
            Pelicula = new Movie { Titulo = "Test" },
            Sala = new Room { Nombre = "Sala 1", CineId = 1, Cine = new Theater { Nombre = "Cine" }, Asientos = [] },
            Boletos = []
        };
        var asientos = new List<Seat>
        {
            new() { Id = 1, Fila = "A", Numero = 1, SalaId = 1 },
            new() { Id = 2, Fila = "A", Numero = 2, SalaId = 1 }
        };
        _mockFunciones.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(funcion);
        _mockAsientos.Setup(r => r.ObtenerDisponiblesPorFuncionAsync(1)).ReturnsAsync(asientos);
        _mockAsientos.Setup(r => r.ObtenerPorSalaAsync(1)).ReturnsAsync(asientos);

        // Act
        var result = await _controller.ObtenerAsientos(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(ok.Value);
    }

    [Fact]
    public async Task Desactivar_FuncionExiste_RetornaNoContent()
    {
        // Arrange
        _mockFunciones.Setup(r => r.DesactivarAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Desactivar(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Desactivar_FuncionNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockFunciones.Setup(r => r.DesactivarAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Desactivar(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
