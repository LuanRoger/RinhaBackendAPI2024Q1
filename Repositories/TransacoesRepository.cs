﻿using Dapper;
using Npgsql;
using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public class TransacoesRepository(NpgsqlDataSource dataSource) : ITransacoesRepository
{
    public async Task AddTransacao(TransacaoModel model)
    {
        await using NpgsqlConnection connection = dataSource.CreateConnection();
        
        const string command = "INSERT INTO transacoes (valor, tipo, descricao, realizadoEm, clienteId) VALUES " +
                               "(@valor, @tipo, @descricao, @realizadoEm, @clienteId)";
        await connection.ExecuteAsync(command, model);
    }
}