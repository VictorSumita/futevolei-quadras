using FutevoleiApi.DTOs;

namespace FutevoleiApi.Services;

public interface IQuadraService
{
    Task<List<QuadraResponse>> ObterTodasAsync();
    Task<QuadraResponse?> ObterPorIdAsync(string id);
    Task<QuadraResponse> CriarAsync(QuadraRequest request);
    Task<bool> AtualizarAsync(string id, QuadraRequest request);
    Task<bool> RemoverAsync(string id);
}
