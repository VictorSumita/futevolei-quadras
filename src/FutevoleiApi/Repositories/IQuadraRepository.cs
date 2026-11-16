using FutevoleiApi.Entities;

namespace FutevoleiApi.Repositories;

public interface IQuadraRepository
{
    Task<List<Quadra>> ObterTodasAsync();
    Task<Quadra?> ObterPorIdAsync(string id);
    Task<Quadra> CriarAsync(Quadra quadra);
    Task<bool> AtualizarAsync(string id, Quadra quadra);
    Task<bool> RemoverAsync(string id);
}
