namespace CineTicket.Domain.Models;

public class Room
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int CineId { get; set; }
    public Theater Cine { get; set; } = null!;

    public ICollection<Seat> Asientos { get; set; } = [];
    public ICollection<Showtime> Funciones { get; set; } = [];
}
