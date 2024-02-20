using Dapper;
using FluentValidation;
using Npgsql;
using RinhaBackendAPI2024Q1.Controllers;
using RinhaBackendAPI2024Q1.Endpoints;
using RinhaBackendAPI2024Q1.Exceptions;
using RinhaBackendAPI2024Q1.Models.Requests;
using RinhaBackendAPI2024Q1.Models.Requests.Json;
using RinhaBackendAPI2024Q1.Repositories;
using RinhaBackendAPI2024Q1.Utils.Environment;
using RinhaBackendAPI2024Q1.Validators;
[module: DapperAot]

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.TypeInfoResolverChain.Insert(0, CreateNewTransacaoRequestJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(1, CreateNewTransacaoResponseJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(2, UnprocessableDebitJson.Default);
    options.SerializerOptions.TypeInfoResolverChain.Insert(3, ClienteExtratoResponseJson.Default);
});

builder.Services.AddScoped<NpgsqlDataSource>(_ =>
{
    string connString = EnvVars.GetConnectionString();

    NpgsqlDataSource connection = new NpgsqlSlimDataSourceBuilder(connString).Build();
    return connection;
});

builder.Services.AddScoped<IValidator<CreateNewTransacaoRequest>, CreateNewTransacaoRequestValidator>();

builder.Services.AddScoped<ITransacoesRepository, TransacoesRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();

builder.Services.AddScoped<IClienteController, ClienteController>();

WebApplication app = builder.Build();

RouteGroupBuilder clientesGroup = app.MapGroup("clientes");
clientesGroup.MapClientesEndpoints();

app.Run();