using System.Text.Json.Serialization;

namespace RinhaBackendAPI2024Q1.Models.Requests;

public class ClienteExtratoResponse(ClienteExtratoSaldo saldo, 
    IEnumerable<ClienteUltimaTransacoes> ultimasTransacoes)
{
    public ClienteExtratoSaldo saldo { get; init; } = saldo;

    [JsonPropertyName("ultimas_transacoes")]
    public IEnumerable<ClienteUltimaTransacoes> ultimasTransacoes { get; init; } = ultimasTransacoes;
}

public class ClienteExtratoSaldo
{
    public int total { get; init; }
    [JsonPropertyName("data_extrato")]
    public DateTime dataExtrato { get; init; }
    public int limite { get; init; }
}

public class ClienteUltimaTransacoes
{
    public int valor { get; init; }
    public char tipo { get; init; }
    public string descricao { get; init; } = string.Empty;
    [JsonPropertyName("realizada_em")]
    public DateTime realizadoEm { get; init; }
}