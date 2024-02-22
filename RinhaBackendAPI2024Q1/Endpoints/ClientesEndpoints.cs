using Microsoft.AspNetCore.Mvc;
using Npgsql;
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
   async private static Task<IResult> GetClienteExtrato(HttpContext context,
       [FromRoute] int id,
       [FromServices] NpgsqlDataSource dataSource)
   {
       ClienteModel? cliente;
       IEnumerable<TransacaoModel> transacoesCliente;
       await using (NpgsqlConnection connection = await dataSource.OpenConnectionAsync())
       {
           ClienteRepository clienteRepository = new(connection);
           TransacoesRepository transacoesRepository = new(connection);
            
           cliente = await clienteRepository.GetClienteById(id);
           if(cliente is null)
               return Results.NotFound();

           transacoesCliente = await transacoesRepository
               .GetTransacoesByClienteId(id);
       }
       
       var ultimaTransacoes = transacoesCliente.Select(f => 
           new ClienteUltimaTransacoes
           { 
               valor = f.valor,
               tipo = f.tipo.ConvertIntToCharBasedOnTransacaoType(),
               descricao = f.descricao,
               realizadoEm = f.realizadoEm
           });
       ClienteExtratoSaldo clienteExtratoSaldo = new()
       {
           limite = cliente.limite,
           total = cliente.saldo,
           dataExtrato = DateTime.UtcNow
       };
        
       ClienteExtratoResponse response = new(clienteExtratoSaldo, ultimaTransacoes);
       
        return Results.Ok(response);
    }
   async private static Task<IResult> PostClientesTransacoes(HttpContext context,
        [FromRoute] int id,
        [FromBody] CreateNewTransacaoRequest request,
        [FromServices] NpgsqlDataSource dataSource)
    {
        if(!ValidateTransacaoRequest(request))
            return Results.UnprocessableEntity();
        
        ClienteModel? cliente;
        int futureSaldo;
        await using (NpgsqlConnection connection = await dataSource.OpenConnectionAsync())
        {
            ClienteRepository clienteRepository = new(connection);
            cliente = await clienteRepository.GetClienteById(id);
            if(cliente is null)
                return Results.NotFound();
        
            bool isCredit = request.isCredit;
            futureSaldo = isCredit ? cliente.saldo + request.valor : cliente.saldo - request.valor;
            if(!isCredit && futureSaldo < -cliente.limite)
                return Results.UnprocessableEntity();
        
            TransacaoModel newTransacao = new()
            {
                valor = request.valor,
                tipo = request.tipo.ConvertCharToIntBasedOnTransacaoType(),
                descricao = request.descricao,
                realizadoEm = DateTime.UtcNow,
                clienteId = id
            };
        
            await clienteRepository.CreateTransacaoForClienteAndUpdateSaldo(id, request.valor, isCredit, newTransacao);
        }
        
        CreateNewTransacaoResponse response = new()
        {
            limite = cliente.limite,
            saldo = futureSaldo
        };
        
        return Results.Ok(response);
    }
   
   private static bool ValidateTransacaoRequest(CreateNewTransacaoRequest request)
   {
       if(string.IsNullOrEmpty(request.descricao) || request.descricao.Length > 10)
           return false;
       if(request.valor <= 0 || request.valor % 1 != 0)
           return false;
       if(request.tipo != 'c' && request.tipo != 'd')
           return false;
       
       return true;
   }
}