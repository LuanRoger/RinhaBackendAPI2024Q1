using FluentValidation;
using FluentValidation.Results;
using RinhaBackendAPI2024Q1.Endpoints;
using RinhaBackendAPI2024Q1.Exceptions;
using RinhaBackendAPI2024Q1.Models;
using RinhaBackendAPI2024Q1.Models.Requests;
using RinhaBackendAPI2024Q1.Repositories;
using RinhaBackendAPI2024Q1.Utils.Extensions;

namespace RinhaBackendAPI2024Q1.Controllers;

public class ClienteController(
    ITransacoesRepository transacoesRepository, 
    IClienteRepository clienteRepository,
    IValidator<CreateNewTransacaoRequest> createNewTransacaoRequestValidator
    ) : IClienteController
{
    public async Task<CreateNewTransacaoResponse> CreateTransacaoCliente(int clienteId, CreateNewTransacaoRequest request)
    {
        ValidationResult isValid = await createNewTransacaoRequestValidator.ValidateAsync(request);
        if(!isValid.IsValid)
            throw new InvalidTransacaoRequestException();
        
        //TODO: Verificar se o cliente existe
        ClienteModel cliente = await clienteRepository.GetClienteById(clienteId);

        bool isCredit = request.isCredit;
        int futureSaldo = isCredit ? cliente.saldo + request.valor : cliente.saldo - request.valor;
        if(!isCredit && futureSaldo < -cliente.limite)
            throw new UnboundClientLimitException();
        
        TransacaoModel newTransacao = new()
        {
            valor = request.valor,
            tipo = request.tipo.ConvertCharToIntBasedOnTransacaoType(),
            descricao = request.descricao,
            realizadoEm = DateTime.UtcNow,
            clienteId = clienteId
        };
        
        await transacoesRepository.AddTransacao(newTransacao);
        if(isCredit)
            await clienteRepository.CreditarSaldo(clienteId, request.valor);
        else
            await clienteRepository.DebitarSaldo(clienteId, request.valor);

        CreateNewTransacaoResponse response = new()
        {
            limite = cliente.limite,
            saldo = futureSaldo
        };

        return response;
    }
    public async Task<ClienteExtratoResponse> GenerateClienteExtrato(int clienteId)
    {
        //TODO: Verificar se o cliente existe
        ClienteModel cliente = await clienteRepository.GetClienteById(clienteId);
        
        var transacoesCliente = await transacoesRepository
            .GetTransacoesByClienteId(clienteId);
       
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
        return response;
    }
}