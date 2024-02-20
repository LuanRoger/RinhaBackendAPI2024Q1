namespace RinhaBackendAPI2024Q1.Models.Requests;

public class CreateNewTransacaoRequest
{
    public int valor { get; init; }
    public char tipo { get; init; }
    public string descricao { get; init; } = null!;
    
    public bool isCredit => tipo == 'c';
}