using FluentValidation;
using FluentValidation.Results;
using Npgsql;
using RinhaBackendAPI2024Q1.Exceptions;
using RinhaBackendAPI2024Q1.Models;
using RinhaBackendAPI2024Q1.Models.Requests;
using RinhaBackendAPI2024Q1.Repositories;
using RinhaBackendAPI2024Q1.Utils.Extensions;

namespace RinhaBackendAPI2024Q1.Controllers;

public class ClienteController(
    IValidator<CreateNewTransacaoRequest> createNewTransacaoRequestValidator,
    NpgsqlDataSource dataSource
    ) : IClienteController
{
    public async Task<CreateNewTransacaoResponse> CreateTransacaoCliente(int clienteId, CreateNewTransacaoRequest request)
    {
        ValidationResult isValid = await createNewTransacaoRequestValidator.ValidateAsync(request);
        if(!isValid.IsValid)
            throw new InvalidTransacaoRequestException();

        ClienteModel? cliente;
        int futureSaldo;
        await using (NpgsqlConnection connection = await dataSource.OpenConnectionAsync())
        {
            ClienteRepository clienteRepository = new(connection);
            cliente = await clienteRepository.GetClienteById(clienteId);
            if(cliente is null)
                throw new ClientDoesNotExistsException(clienteId);
        
            bool isCredit = request.isCredit;
            futureSaldo = isCredit ? cliente.saldo + request.valor : cliente.saldo - request.valor;
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
        
            await clienteRepository.CreateTransacaoForClienteAndUpdateSaldo(clienteId, request.valor, isCredit, newTransacao);
        }
        
        CreateNewTransacaoResponse response = new()
        {
            limite = cliente.limite,
            saldo = futureSaldo
        };

        return response;
    }
    public async Task<ClienteExtratoResponse> GenerateClienteExtrato(int clienteId)
    {
        ClienteModel? cliente;
        IEnumerable<TransacaoModel> transacoesCliente;
        await using (NpgsqlConnection connection = await dataSource.OpenConnectionAsync())
        {
            ClienteRepository clienteRepository = new(connection);
            TransacoesRepository transacoesRepository = new(connection);
            
            try
            {
                cliente = await clienteRepository.GetClienteById(clienteId);
                if(cliente is null)
                    throw new ClientDoesNotExistsException(clienteId);

                transacoesCliente = await transacoesRepository
                    .GetTransacoesByClienteId(clienteId);
            }
            // ReSharper disable once RedundantCatchClause
            catch (ClientDoesNotExistsException) { throw; }
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
        return response;
    }
}