using System.Text.Json.Serialization;
using RinhaBackendAPI2024Q1.Models.Utils;

namespace RinhaBackendAPI2024Q1.Models.Requests.Json;

[JsonSourceGenerationOptions(
    GenerationMode = JsonSourceGenerationMode.Metadata)]
[JsonSerializable(typeof(UnprocessableDebit))]
internal partial class UnprocessableDebitJson : JsonSerializerContext;