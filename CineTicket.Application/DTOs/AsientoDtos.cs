namespace CineTicket.Application.DTOs;

public class AsientoDto
{
    public int Id { get; set; }
    public string Fila { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Etiqueta { get; set; } = string.Empty;
    public bool EstaDisponible { get; set; }
}
