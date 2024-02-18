using Dapper;
using Npgsql;
using RinhaBackendAPI2024Q1.Endpoints;

namespace RinhaBackendAPI2024Q1.Repositories;

public class ClienteRepository(NpgsqlDataSource dataSource) : IClienteRepository
{
    public async Task<ClienteModel> GetClienteById(int id)
    {
        await using NpgsqlConnection connection = dataSource.CreateConnection();
        const string command = "SELECT * FROM clientes WHERE id = @id";
        
        ClienteModel clienteModel = await connection
            .QueryFirstAsync<ClienteModel>(command, new { id });
        return clienteModel;
    }
    
    public async Task CreditarSaldo(int id, int valor)
    {
        await using NpgsqlConnection connection = dataSource.CreateConnection();
        const string command = "UPDATE clientes SET saldo = saldo + @valor WHERE id = @id";
        const string getNewSaldoCommand = "SELECT saldo FROM clientes WHERE id = @id";
        
        await connection.ExecuteAsync(command, new { id, valor });
    }
    
    public async Task DebitarSaldo(int id, int valor)
    {
        await using NpgsqlConnection connection = dataSource.CreateConnection();
        const string command = "UPDATE clientes SET saldo = saldo - @valor WHERE id = @id";
        const string getNewSaldoCommand = "SELECT saldo FROM clientes WHERE id = @id";
        
        await connection.ExecuteAsync(command, new { id, valor });
    }
}