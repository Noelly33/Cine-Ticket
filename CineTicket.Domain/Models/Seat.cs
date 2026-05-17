namespace CineTicket.Domain.Models;

public class Seat
{
    public int Id { get; set; }
    public string Fila { get; set; } = string.Empty;
    public int Numero { get; set; }
    public int SalaId { get; set; }
    public Room Sala { get; set; } = null!;

    public ICollection<Ticket> Boletos { get; set; } = [];
}
