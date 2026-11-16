using FutevoleiApi.DTOs;
using FutevoleiApi.Entities;
using FutevoleiApi.Repositories;

namespace FutevoleiApi.Services;

public class QuadraService : IQuadraService
{
    private readonly IQuadraRepository _repository;

    public QuadraService(IQuadraRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<QuadraResponse>> ObterTodasAsync()
    {
        var quadras = await _repository.ObterTodasAsync();
        return quadras.Select(MapearParaResponse).ToList();
    }

    public async Task<QuadraResponse?> ObterPorIdAsync(string id)
    {
        var quadra = await _repository.ObterPorIdAsync(id);
        return quadra is null ? null : MapearParaResponse(quadra);
    }

    public async Task<QuadraResponse> CriarAsync(QuadraRequest request)
    {
        var quadra = new Quadra
        {
            Nome = request.Nome.Trim(),
            TipoPiso = request.TipoPiso,
            Coberta = request.Coberta,
            PrecoHora = request.PrecoHora,
            Ativa = request.Ativa,
            CriadaEm = DateTime.UtcNow
        };

        var criada = await _repository.CriarAsync(quadra);
        return MapearParaResponse(criada);
    }

    public async Task<bool> AtualizarAsync(string id, QuadraRequest request)
    {
        var existente = await _repository.ObterPorIdAsync(id);
        if (existente is null) return false;

        existente.Nome = request.Nome.Trim();
        existente.TipoPiso = request.TipoPiso;
        existente.Coberta = request.Coberta;
        existente.PrecoHora = request.PrecoHora;
        existente.Ativa = request.Ativa;

        return await _repository.AtualizarAsync(id, existente);
    }

    public async Task<bool> RemoverAsync(string id) =>
        await _repository.RemoverAsync(id);

    private static QuadraResponse MapearParaResponse(Quadra quadra) => new()
    {
        Id = quadra.Id,
        Nome = quadra.Nome,
        TipoPiso = quadra.TipoPiso,
        Coberta = quadra.Coberta,
        PrecoHora = quadra.PrecoHora,
        Ativa = quadra.Ativa,
        CriadaEm = quadra.CriadaEm
    };
}
