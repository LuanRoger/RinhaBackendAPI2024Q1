using System.Text.Json.Serialization;

namespace RinhaBackendAPI2024Q1.Models.Utils;

public class UnprocessableDebit
{
    [JsonPropertyName("saldo_futuro")]
    public int saldoFuturo { get; init; }
    public int limite { get; init; }
}