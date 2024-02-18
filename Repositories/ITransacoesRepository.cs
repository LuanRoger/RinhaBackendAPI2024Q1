using RinhaBackendAPI2024Q1.Models;

namespace RinhaBackendAPI2024Q1.Repositories;

public interface ITransacoesRepository
{
    public Task AddTransacao(TransacaoModel model);
}