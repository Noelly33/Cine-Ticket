namespace CineTicket.Application.DTOs;

public class BoletoDto
{
    public int Id { get; set; }
    public int FuncionId { get; set; }
    public string TituloPelicula { get; set; } = string.Empty;
    public DateTime HoraFuncion { get; set; }
    public string NombreCine { get; set; } = string.Empty;
    public string NombreSala { get; set; } = string.Empty;
    public int AsientoId { get; set; }
    public string Asiento { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
    public DateTime FechaCompra { get; set; }
    public string Estado { get; set; } = string.Empty;
    public decimal PrecioTotal { get; set; }
}

public class CrearBoletoDto
{
    public int FuncionId { get; set; }
    public int AsientoId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string EmailCliente { get; set; } = string.Empty;
}
