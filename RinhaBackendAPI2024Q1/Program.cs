using RinhaBackendAPI2024Q1.Endpoints;
using RinhaBackendAPI2024Q1.Models.Requests.Json;
using RinhaBackendAPI2024Q1.Utils.Environment;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.WebHost.ConfigureKestrel(options =>
{
    options.AddServerHeader = false;
});
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CreateNewTransacaoRequestJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(1, CreateNewTransacaoResponseJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(2, UnprocessableDebitJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(3, ClienteExtratoResponseJson.Default);
});

builder.Services.AddNpgsqlSlimDataSource(EnvVars.GetConnectionString());

WebApplication app = builder.Build();

RouteGroupBuilder clientesGroup = app.MapGroup("clientes");
clientesGroup.MapClientesEndpoints();

app.Run();