namespace CineTicket.Domain.Models;

public class Showtime
{
    public int Id { get; set; }
    public int PeliculaId { get; set; }
    public Movie Pelicula { get; set; } = null!;
    public int SalaId { get; set; }
    public Room Sala { get; set; } = null!;
    public DateTime HoraInicio { get; set; }
    public decimal Precio { get; set; }
    public bool EstaActivo { get; set; } = true;

    public ICollection<Ticket> Boletos { get; set; } = [];
}
