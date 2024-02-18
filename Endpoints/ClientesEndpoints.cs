using Microsoft.AspNetCore.Mvc;
using RinhaBackendAPI2024Q1.Models;
using RinhaBackendAPI2024Q1.Models.Requests;
using RinhaBackendAPI2024Q1.Repositories;
using RinhaBackendAPI2024Q1.Utils.Extensions;

namespace RinhaBackendAPI2024Q1.Endpoints;

public static class ClientesEndpoints
{
    public static RouteGroupBuilder MapClientesEndpoints(this RouteGroupBuilder builder)
    {
        RouteGroupBuilder clientesIdGroup = builder.MapGroup("{id:int}");
        clientesIdGroup.MapGet("extrato", GetClienteExtrato);
        clientesIdGroup.MapPost("transacoes", PostClientesTransacoes);

        return builder;
    }
    private static IResult GetClienteExtrato(HttpContext context)
    {
        return Results.Ok();
    }
    async private static Task<IResult> PostClientesTransacoes(HttpContext context,
        [FromRoute] int id,
        [FromBody] CreateNewTransacaoRequest request,
        [FromServices] ITransacoesRepository transacoesRepository,
        [FromServices] IClienteRepository clienteRepository)
    {
        ClienteModel cliente;
        try
        {
            cliente = await clienteRepository.GetClienteById(id);
        }
        catch(Exception) { return Results.NotFound(); }
        bool isValid = request.tipo is 'c' or 'd';
        if(!isValid) return Results.BadRequest();

        bool isCredit = request.tipo == 'c';
        int futureSaldo = isCredit ? cliente.saldo + request.valor : cliente.saldo - request.valor;
        if(!isCredit)
        {
            if(futureSaldo < -cliente.limite)
            {
                var dumyEntiy = new
                {
                    saldo_futuro = futureSaldo,
                    limite = cliente.limite
                };
                return Results.UnprocessableEntity(dumyEntiy);
            }
        }
        
        TransacaoModel newTransacao = new()
        {
            valor = request.valor,
            tipo = request.tipo.ConvertCharToIntBasedOnTransacaoType(),
            descricao = request.descricao,
            realizadoEm = DateTime.UtcNow,
            clienteId = id
        };
        
        await transacoesRepository.AddTransacao(newTransacao);
        if(isCredit)
            await clienteRepository.CreditarSaldo(id, request.valor);
        else
            await clienteRepository.DebitarSaldo(id, request.valor);

        CreateNewTransacaoResponse response = new()
        {
            limite = cliente.limite,
            saldo = futureSaldo
        };
        
        return Results.Ok(response);
    }
}