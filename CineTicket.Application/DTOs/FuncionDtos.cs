namespace CineTicket.Application.DTOs;

public class FuncionDto
{
    public int Id { get; set; }
    public int PeliculaId { get; set; }
    public string TituloPelicula { get; set; } = string.Empty;
    public int SalaId { get; set; }
    public string NombreSala { get; set; } = string.Empty;
    public int CineId { get; set; }
    public string NombreCine { get; set; } = string.Empty;
    public DateTime HoraInicio { get; set; }
    public decimal Precio { get; set; }
    public bool EstaActivo { get; set; }
    public int AsientosDisponibles { get; set; }
}

public class CrearFuncionDto
{
    public int PeliculaId { get; set; }
    public int SalaId { get; set; }
    public DateTime HoraInicio { get; set; }
    public decimal Precio { get; set; }
}
