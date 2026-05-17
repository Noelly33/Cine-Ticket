namespace CineTicket.Domain.Models;

public class Theater
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;

    public ICollection<Room> Salas { get; set; } = [];
}
