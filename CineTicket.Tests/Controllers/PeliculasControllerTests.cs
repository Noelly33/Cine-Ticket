using CineTicket.API.Controllers;
using CineTicket.Application.DTOs;
using CineTicket.Application.Interfaces;
using CineTicket.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace CineTicket.Tests.Controllers;

public class PeliculasControllerTests
{
    private readonly Mock<IPeliculaRepository> _mockRepo;
    private readonly PeliculasController _controller;

    public PeliculasControllerTests()
    {
        _mockRepo = new Mock<IPeliculaRepository>();
        _controller = new PeliculasController(_mockRepo.Object);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkConListaDePeliculas()
    {
        // Arrange
        var peliculas = new List<Movie>
        {
            new() { Id = 1, Titulo = "Pelicula 1", Genero = "Acción", Clasificacion = "PG", EstaActivo = true },
            new() { Id = 2, Titulo = "Pelicula 2", Genero = "Drama",  Clasificacion = "R",  EstaActivo = true }
        };
        _mockRepo.Setup(r => r.ObtenerTodosAsync(false)).ReturnsAsync(peliculas);

        // Act
        var result = await _controller.ObtenerTodos();

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var lista = Assert.IsAssignableFrom<IEnumerable<PeliculaDto>>(ok.Value);
        Assert.Equal(2, lista.Count());
    }

    [Fact]
    public async Task ObtenerPorId_PeliculaExiste_RetornaOkConDatos()
    {
        // Arrange
        var pelicula = new Movie { Id = 1, Titulo = "Rebelión en Marte", Genero = "Sci-Fi", Clasificacion = "PG-13", EstaActivo = true };
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(1)).ReturnsAsync(pelicula);

        // Act
        var result = await _controller.ObtenerPorId(1);

        // Assert
        var ok = Assert.IsType<OkObjectResult>(result);
        var dto = Assert.IsType<PeliculaDto>(ok.Value);
        Assert.Equal(1, dto.Id);
        Assert.Equal("Rebelión en Marte", dto.Titulo);
    }

    [Fact]
    public async Task ObtenerPorId_PeliculaNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.ObtenerPorIdAsync(99)).ReturnsAsync((Movie?)null);

        // Act
        var result = await _controller.ObtenerPorId(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Crear_DatosValidos_RetornaCreated201()
    {
        // Arrange
        var dto = new CrearPeliculaDto
        {
            Titulo = "Nueva Pelicula",
            Descripcion = "Una descripcion",
            DuracionMinutos = 120,
            Genero = "Acción",
            Clasificacion = "PG-13"
        };
        var peliculaCreada = new Movie { Id = 10, Titulo = dto.Titulo, Genero = dto.Genero, Clasificacion = dto.Clasificacion, EstaActivo = true };
        _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Movie>())).ReturnsAsync(peliculaCreada);

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var created = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, created.StatusCode);
        var dto_resultado = Assert.IsType<PeliculaDto>(created.Value);
        Assert.Equal(10, dto_resultado.Id);
    }

    [Fact]
    public async Task Actualizar_PeliculaNoExiste_RetornaNotFound()
    {
        // Arrange
        var dto = new ActualizarPeliculaDto
        {
            Titulo = "Titulo",
            Descripcion = "Desc",
            DuracionMinutos = 90,
            Genero = "Drama",
            Clasificacion = "PG",
            EstaActivo = true
        };
        _mockRepo.Setup(r => r.ActualizarAsync(99, It.IsAny<Movie>())).ReturnsAsync((Movie?)null);

        // Act
        var result = await _controller.Actualizar(99, dto);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task Desactivar_PeliculaExiste_RetornaNoContent()
    {
        // Arrange
        _mockRepo.Setup(r => r.DesactivarAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Desactivar(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task Desactivar_PeliculaNoExiste_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(r => r.DesactivarAsync(99)).ReturnsAsync(false);

        // Act
        var result = await _controller.Desactivar(99);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }
}
