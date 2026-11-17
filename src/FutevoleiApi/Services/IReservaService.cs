using FutevoleiApi.DTOs;

namespace FutevoleiApi.Services;

public record ResultadoReserva(bool Sucesso, int StatusCode, string? Erro, ReservaResponse? Dados);

public interface IReservaService
{
    Task<List<ReservaResponse>> ObterTodasAsync();
    Task<List<ReservaResponse>> ObterPorQuadraAsync(string quadraId);
    Task<ReservaResponse?> ObterPorIdAsync(string id);
    Task<ResultadoReserva> CriarAsync(ReservaRequest request);
    Task<ResultadoReserva> AtualizarAsync(string id, ReservaRequest request);
    Task<bool> CancelarAsync(string id);
    Task<bool> RemoverAsync(string id);
}
