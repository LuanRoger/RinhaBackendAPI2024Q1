using Npgsql;
using NpgsqlTypes;
using RinhaBackendAPI2024Q1.Endpoints;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public class ClienteRepository(NpgsqlConnection connection) : IClienteRepository
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
        while (await clienteReader.ReadAsync())
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
    
    public async Task CreditarSaldo(int id, int valor, NpgsqlTransaction? transaction = null)
    {
        const string command = "UPDATE clientes SET saldo = saldo + $1 WHERE id = $2";
        await using NpgsqlCommand creditCommand = new(command, connection);
        creditCommand.Parameters.Add(new() { Value = valor, NpgsqlDbType = NpgsqlDbType.Integer });
        creditCommand.Parameters.Add(new() { Value = id, NpgsqlDbType = NpgsqlDbType.Integer });
        
        await creditCommand.ExecuteNonQueryAsync();
    }
    
    public async Task DebitarSaldo(int id, int valor, NpgsqlTransaction? transaction = null)
    {
        const string command = "UPDATE clientes SET saldo = saldo - $1 WHERE id = $2";
        await using NpgsqlCommand debitCommand = new(command, connection);
        debitCommand.Parameters.Add(new() { Value = valor, NpgsqlDbType = NpgsqlDbType.Integer });
        debitCommand.Parameters.Add(new() { Value = id, NpgsqlDbType = NpgsqlDbType.Integer });
        
        await debitCommand.ExecuteNonQueryAsync();
    }
}