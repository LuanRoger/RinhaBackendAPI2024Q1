namespace RinhaBackendAPI2024Q1.Models;

public class TransacaoModel
{
    public int id { get; set; }
    public int valor { get; set; }
    public int tipo { get; set; }
    public string descricao { get; set; } = string.Empty;
    public DateTime realizadoEm { get; set; }
    public int clienteId { get; set; }
}