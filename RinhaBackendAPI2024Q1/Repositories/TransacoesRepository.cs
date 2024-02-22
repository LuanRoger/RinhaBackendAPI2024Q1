using Npgsql;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public class TransacoesRepository(NpgsqlConnection connection) 
{
    private NpgsqlConnection connection { get; } = connection;
    
    private const string GET_TRANSACTIONS_WITH_CLIENT = 
        "SELECT * FROM transacoes WHERE clienteId = $1 ORDER BY realizadoEm DESC LIMIT 10";
    
    public async Task<IEnumerable<TransacaoModel>> GetTransacoesByClienteId(int clienteId)
    {
        await using NpgsqlCommand getTransactionCommand = new(GET_TRANSACTIONS_WITH_CLIENT, connection);
        getTransactionCommand.Parameters.Add(new() { Value = clienteId });
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
}