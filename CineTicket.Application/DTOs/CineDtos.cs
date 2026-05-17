namespace CineTicket.Application.DTOs;

public class SalaDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public int TotalAsientos { get; set; }
}

public class CineDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public List<SalaDto> Salas { get; set; } = [];
}
