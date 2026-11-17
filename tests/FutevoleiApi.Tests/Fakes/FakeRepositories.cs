using FutevoleiApi.Entities;
using FutevoleiApi.Enums;
using FutevoleiApi.Repositories;

namespace FutevoleiApi.Tests.Fakes;

public class FakeQuadraRepository : IQuadraRepository
{
    public List<Quadra> Quadras { get; } = new();

    public Task<List<Quadra>> ObterTodasAsync() => Task.FromResult(Quadras.ToList());

    public Task<Quadra?> ObterPorIdAsync(string id) =>
        Task.FromResult(Quadras.FirstOrDefault(q => q.Id == id));

    public Task<Quadra> CriarAsync(Quadra quadra)
    {
        if (string.IsNullOrEmpty(quadra.Id))
            quadra.Id = Guid.NewGuid().ToString("N")[..24];

        Quadras.Add(quadra);
        return Task.FromResult(quadra);
    }

    public Task<bool> AtualizarAsync(string id, Quadra quadra)
    {
        var i = Quadras.FindIndex(q => q.Id == id);
        if (i < 0) return Task.FromResult(false);
        Quadras[i] = quadra;
        return Task.FromResult(true);
    }

    public Task<bool> RemoverAsync(string id) =>
        Task.FromResult(Quadras.RemoveAll(q => q.Id == id) > 0);
}

public class FakeReservaRepository : IReservaRepository
{
    public List<Reserva> Reservas { get; } = new();

    public Task<List<Reserva>> ObterTodasAsync() => Task.FromResult(Reservas.ToList());

    public Task<List<Reserva>> ObterPorQuadraAsync(string quadraId) =>
        Task.FromResult(Reservas.Where(r => r.QuadraId == quadraId).OrderBy(r => r.Inicio).ToList());

    public Task<Reserva?> ObterPorIdAsync(string id) =>
        Task.FromResult(Reservas.FirstOrDefault(r => r.Id == id));

    public Task<Reserva> CriarAsync(Reserva reserva)
    {
        if (string.IsNullOrEmpty(reserva.Id))
            reserva.Id = Guid.NewGuid().ToString("N")[..24];

        Reservas.Add(reserva);
        return Task.FromResult(reserva);
    }

    public Task<bool> AtualizarAsync(string id, Reserva reserva)
    {
        var i = Reservas.FindIndex(r => r.Id == id);
        if (i < 0) return Task.FromResult(false);
        Reservas[i] = reserva;
        return Task.FromResult(true);
    }

    public Task<bool> RemoverAsync(string id) =>
        Task.FromResult(Reservas.RemoveAll(r => r.Id == id) > 0);

    public Task<bool> ExisteConflitoAsync(string quadraId, DateTime inicio, DateTime fim, string? ignorarReservaId = null)
    {
        var existe = Reservas.Any(r =>
            r.QuadraId == quadraId &&
            r.Status != StatusReserva.Cancelada &&
            r.Inicio < fim &&
            r.Fim > inicio &&
            (ignorarReservaId is null || r.Id != ignorarReservaId));

        return Task.FromResult(existe);
    }
}
