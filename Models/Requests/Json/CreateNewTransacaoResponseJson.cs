using System.Text.Json.Serialization;

namespace RinhaBackendAPI2024Q1.Models.Requests.Json;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(CreateNewTransacaoResponse))]
internal partial class CreateNewTransacaoResponseJson : JsonSerializerContext
{
    
}