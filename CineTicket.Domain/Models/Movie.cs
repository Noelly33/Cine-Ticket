namespace CineTicket.Domain.Models;

public class Movie
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public string? UrlPoster { get; set; }
    public bool EstaActivo { get; set; } = true;

    public ICollection<Showtime> Funciones { get; set; } = [];
}
