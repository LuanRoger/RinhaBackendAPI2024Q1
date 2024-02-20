using Npgsql;
using NpgsqlTypes;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public class TransacoesRepository(NpgsqlConnection connection) : ITransacoesRepository
{
    public NpgsqlConnection connection { get; } = connection;
    
    private const string GET_SUM_OF_TRANSACOES = 
        "SELECT SUM(valor) FROM transacoes WHERE clienteId = $1";
    private const string GET_TRANSACTIONS_WITH_CLIENT = 
        "SELECT * FROM transacoes WHERE clienteId = $1 ORDER BY realizadoEm DESC LIMIT 10";

    public async Task AddTransacao(TransacaoModel model, NpgsqlTransaction? transaction = null)
    {
        const string command = "INSERT INTO transacoes (valor, tipo, descricao, realizadoEm, clienteId) VALUES " +
                               "($1, $2, $3, $4, $5)";
        await using NpgsqlCommand insertCommand = new(command, connection, transaction);
        insertCommand.Parameters.Add(new() { Value = model.valor, NpgsqlDbType = NpgsqlDbType.Integer });
        insertCommand.Parameters.Add(new() { Value = model.tipo, NpgsqlDbType = NpgsqlDbType.Smallint });
        insertCommand.Parameters.Add(new() { Value = model.descricao, NpgsqlDbType = NpgsqlDbType.Varchar });
        insertCommand.Parameters.Add(new() { Value = model.realizadoEm });
        insertCommand.Parameters.Add(new() { Value = model.clienteId, NpgsqlDbType = NpgsqlDbType.Integer });
        
        await insertCommand.ExecuteNonQueryAsync();
    }
    
    public async Task<IEnumerable<TransacaoModel>> GetTransacoesByClienteId(int clienteId)
    {
        await using NpgsqlCommand getTransactionCommand = new(GET_TRANSACTIONS_WITH_CLIENT, connection);
        getTransactionCommand.Parameters.Add(new() { Value = clienteId, NpgsqlDbType = NpgsqlDbType.Integer });
        await getTransactionCommand.PrepareAsync();
        
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

    public async Task<int?> GetSumOfTransacoesByClienteId(int clienteId)
    {
        await using NpgsqlCommand getSumCommand = new(GET_SUM_OF_TRANSACOES, connection);
        await getSumCommand.PrepareAsync();
        getSumCommand.Parameters.Add(new() { Value = clienteId, NpgsqlDbType = NpgsqlDbType.Integer });
        
        return await getSumCommand.ExecuteScalarAsync() as int?;
    }
    
    public async Task<(IEnumerable<TransacaoModel>, int?)> GetAllTransacoesInformationByClienteId(int clienteId)
    {
        await using NpgsqlBatch transacoesBatch = new(connection)
        {
            BatchCommands =
            {
                new(GET_SUM_OF_TRANSACOES)
                {
                    Parameters =
                    {
                        new() { Value = clienteId }
                    }
                },
                new(GET_TRANSACTIONS_WITH_CLIENT)
                {
                    Parameters =
                    {
                        new() { Value = clienteId }
                    }
                },
            }
        };
        await using NpgsqlDataReader resultsReader = await transacoesBatch.ExecuteReaderAsync();
        if(!resultsReader.HasRows)
            return (new List<TransacaoModel>(), null);
        var transacoes = new List<TransacaoModel>();
        while (await resultsReader.ReadAsync())
        {
            transacoes.Add(new()
            {
                id = resultsReader.GetInt32(0),
                valor = resultsReader.GetInt32(1),
                tipo = resultsReader.GetInt16(2),
                descricao = resultsReader.GetString(3),
                realizadoEm = resultsReader.GetDateTime(4),
                clienteId = resultsReader.GetInt32(5)
            });
        }

        return (transacoes, null);
    }
}