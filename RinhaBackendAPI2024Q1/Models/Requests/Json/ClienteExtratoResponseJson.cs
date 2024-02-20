using System.Text.Json.Serialization;

namespace RinhaBackendAPI2024Q1.Models.Requests.Json;

[JsonSourceGenerationOptions(GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(ClienteExtratoResponse))]
[JsonSerializable(typeof(ClienteExtratoSaldo))]
[JsonSerializable(typeof(ClienteUltimaTransacoes))]
internal partial class ClienteExtratoResponseJson : JsonSerializerContext;