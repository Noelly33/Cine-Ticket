namespace CineTicket.Domain.Models;

public enum EstadoBoleto { Confirmado, Cancelado }

public class Ticket
{
    public int Id { get; set; }
    public int FuncionId { get; set; }
    public Showtime Funcion { get; set; } = null!;
    public int AsientoId { get; set; }
    public Seat Asiento { get; set; } = null!;
    public string NombreCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public DateTime FechaCompra { get; set; } = DateTime.UtcNow;
    public EstadoBoleto Estado { get; set; } = EstadoBoleto.Confirmado;
    public decimal PrecioTotal { get; set; }
}
