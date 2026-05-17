namespace CineTicket.Application.DTOs;

public class PeliculaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public string? UrlPoster { get; set; }
    public bool EstaActivo { get; set; }
}

public class CrearPeliculaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public string? UrlPoster { get; set; }
}

public class ActualizarPeliculaDto
{
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public int DuracionMinutos { get; set; }
    public string Genero { get; set; } = string.Empty;
    public string Clasificacion { get; set; } = string.Empty;
    public string? UrlPoster { get; set; }
    public bool EstaActivo { get; set; }
}
