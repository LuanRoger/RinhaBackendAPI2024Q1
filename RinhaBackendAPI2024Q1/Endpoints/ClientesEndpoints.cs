using Microsoft.AspNetCore.Mvc;
using RinhaBackendAPI2024Q1.Controllers;
using RinhaBackendAPI2024Q1.Exceptions;
using RinhaBackendAPI2024Q1.Models.Requests;
using Exception = System.Exception;

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
       [FromServices] IClienteController controller)
   {
       ClienteExtratoResponse response;
       try
       {
           response = await controller.GenerateClienteExtrato(id);
       }
       catch (Exception) { return Results.NotFound(); }
       
        return Results.Ok(response);
    }
   async private static Task<IResult> PostClientesTransacoes(HttpContext context,
        [FromRoute] int id,
        [FromBody] CreateNewTransacaoRequest request,
        [FromServices] IClienteController controller)
    {
        CreateNewTransacaoResponse response;
        try
        {
            response = await controller.CreateTransacaoCliente(id, request);
        }
        catch (Exception e) when (e is InvalidTransacaoRequestException or UnboundClientLimitException)
        {
            return Results.UnprocessableEntity();
        }
        catch (Exception) { return Results.NotFound(); }
        
        return Results.Ok(response);
    }
}