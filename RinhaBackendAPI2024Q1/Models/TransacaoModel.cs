namespace RinhaBackendAPI2024Q1.Models;

public class TransacaoModel
{
    public int id { get; set; }
    public int valor { get; init; }
    public short tipo { get; init; }
    public string descricao { get; init; } = string.Empty;
    public DateTime realizadoEm { get; init; }
    public int clienteId { get; init; }
}