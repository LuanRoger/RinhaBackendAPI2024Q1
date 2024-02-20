using Npgsql;
using RinhaBackendAPI2024Q1.Endpoints;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public interface IClienteRepository
{
    public NpgsqlConnection connection { get; }
    
    public Task<ClienteModel?> GetClienteById(int id);
    public Task CreditarSaldo(int id, int valor, NpgsqlTransaction? transaction = null);
    public Task DebitarSaldo(int id, int valor, NpgsqlTransaction? transaction = null);
}