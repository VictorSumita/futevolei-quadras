using FutevoleiApi.Entities;

namespace FutevoleiApi.Repositories;

public interface IReservaRepository
{
    Task<List<Reserva>> ObterTodasAsync();
    Task<List<Reserva>> ObterPorQuadraAsync(string quadraId);
    Task<Reserva?> ObterPorIdAsync(string id);
    Task<Reserva> CriarAsync(Reserva reserva);
    Task<bool> AtualizarAsync(string id, Reserva reserva);
    Task<bool> RemoverAsync(string id);
    Task<bool> ExisteConflitoAsync(string quadraId, DateTime inicio, DateTime fim, string? ignorarReservaId = null);
}
