using RinhaBackendAPI2024Q1.Models.Requests;

namespace RinhaBackendAPI2024Q1.Controllers;

public interface IClienteController
{
    public Task<CreateNewTransacaoResponse> CreateTransacaoCliente(int clienteId, CreateNewTransacaoRequest request);
    public Task<ClienteExtratoResponse> GenerateClienteExtrato(int clienteId);
}