using Npgsql;
using NpgsqlTypes;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public class ClienteRepository(NpgsqlConnection connection)
{
    public NpgsqlConnection connection { get; } = connection;

    public async Task<ClienteModel?> GetClienteById(int id)
    {
        const string command = "SELECT * FROM clientes WHERE id = $1 LIMIT 1";
        await using NpgsqlCommand getClienteCommand = new(command, connection);
        getClienteCommand.Parameters.Add(new() { Value = id, NpgsqlDbType = NpgsqlDbType.Integer });
        await getClienteCommand.PrepareAsync();
        
        await using NpgsqlDataReader clienteReader = await getClienteCommand.ExecuteReaderAsync();
        if(!clienteReader.HasRows)
            return null;

        ClienteModel? clienteModel = null;
        if(await clienteReader.ReadAsync())
        {
            clienteModel = new()
            {
                id = clienteReader.GetInt32(0),
                saldo = clienteReader.GetInt32(1),
                limite = clienteReader.GetInt32(2)
            };
        }
        
        return clienteModel;
    }

    public async Task CreateTransacaoForClienteAndUpdateSaldo(int id, int valor, bool isCredit, TransacaoModel model)
    {
        const string creditCommand = "UPDATE clientes SET saldo = saldo + $1 WHERE id = $2";
        const string debitCommand = "UPDATE clientes SET saldo = saldo - $1 WHERE id = $2";
        const string insertTransacaoCommand = "INSERT INTO transacoes (valor, tipo, descricao, realizadoEm, clienteId) VALUES " +
                               "($1, $2, $3, $4, $5)";

        await using NpgsqlTransaction transacaoTransaction = await connection.BeginTransactionAsync();        
        
        await using NpgsqlCommand updateSaldoCommand = 
            new(isCredit ? creditCommand : debitCommand, connection, transacaoTransaction);
        updateSaldoCommand.Parameters.Add(new() { Value = valor });
        updateSaldoCommand.Parameters.Add(new() { Value = id });
        await updateSaldoCommand.PrepareAsync();
        await updateSaldoCommand.ExecuteNonQueryAsync();
        
        await using NpgsqlCommand insertCommand = new(insertTransacaoCommand, connection, transacaoTransaction);
        insertCommand.Parameters.Add(new() { Value = model.valor });
        insertCommand.Parameters.Add(new() { Value = model.tipo });
        insertCommand.Parameters.Add(new() { Value = model.descricao });
        insertCommand.Parameters.Add(new() { Value = model.realizadoEm });
        insertCommand.Parameters.Add(new() { Value = model.clienteId });
        await insertCommand.PrepareAsync();
        await insertCommand.ExecuteNonQueryAsync();
        
        await transacaoTransaction.CommitAsync();
    }
}