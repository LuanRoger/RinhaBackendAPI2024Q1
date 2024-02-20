using Npgsql;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public interface ITransacoesRepository
{
    public NpgsqlConnection connection { get; }
    public Task AddTransacao(TransacaoModel model, NpgsqlTransaction? transaction = null);
    public Task<IEnumerable<TransacaoModel>> GetTransacoesByClienteId(int clienteId);
}