using Dapper;
using Npgsql;
using RinhaBackendAPI2024Q1.Endpoints;
using RinhaBackendAPI2024Q1.Exceptions;
using RinhaBackendAPI2024Q1.Models.Requests.Json;
using RinhaBackendAPI2024Q1.Repositories;
[module: DapperAot]

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CreateNewTransacaoRequestJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(1, CreateNewTransacaoResponseJson.Default);
});

builder.Services.AddScoped<NpgsqlDataSource>(_ =>
{
    string? connString = builder.Configuration.GetConnectionString("PostgresConnection");
    if(connString is null)
        throw new ConnectionStringNotProvided();

    NpgsqlDataSource connection = NpgsqlDataSource.Create(connString);
    return connection;
});

builder.Services.AddScoped<ITransacoesRepository, TransacoesRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

WebApplication app = builder.Build();

RouteGroupBuilder clientesGroup = app.MapGroup("clientes");
clientesGroup.MapClientesEndpoints();

app.Run();