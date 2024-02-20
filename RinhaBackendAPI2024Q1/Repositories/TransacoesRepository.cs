using Npgsql;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public class TransacoesRepository(NpgsqlConnection connection) : ITransacoesRepository
{
    public NpgsqlConnection connection { get; } = connection;

    public async Task AddTransacao(TransacaoModel model)
    {
        const string command = "INSERT INTO transacoes (valor, tipo, descricao, realizadoEm, clienteId) VALUES " +
                               "($1, $2, $3, $4, $5)";
        await using NpgsqlCommand insertCommand = new(command, connection);
        insertCommand.Parameters.Add(new() { Value = model.valor });
        insertCommand.Parameters.Add(new() { Value = model.tipo });
        insertCommand.Parameters.Add(new() { Value = model.descricao });
        insertCommand.Parameters.Add(new() { Value = model.realizadoEm });
        insertCommand.Parameters.Add(new() { Value = model.clienteId });
        
        await insertCommand.ExecuteNonQueryAsync();
    }
    
    public async Task<IEnumerable<TransacaoModel>> GetTransacoesByClienteId(int clienteId, int limit = 10)
    {
        const string getTransactionsWithClient = "SELECT * FROM transacoes WHERE clienteId = $1 LIMIT $2";
        
        await using NpgsqlCommand getTransactionCommand = new(getTransactionsWithClient, connection);
        getTransactionCommand.Parameters.Add(new() { Value = clienteId });
        getTransactionCommand.Parameters.Add(new() { Value = limit });
        
        await using NpgsqlDataReader transacoesReader = await getTransactionCommand.ExecuteReaderAsync();
        var transacoes = new List<TransacaoModel>();
        while (await transacoesReader.ReadAsync())
        {
            transacoes.Add(new()
            {
                id = transacoesReader.GetInt32(0),
                valor = transacoesReader.GetInt32(1),
                tipo = transacoesReader.GetInt16(2),
                descricao = transacoesReader.GetString(3),
                realizadoEm = transacoesReader.GetDateTime(4),
                clienteId = transacoesReader.GetInt32(5)
            });
        }
        
        return transacoes;
    }
}