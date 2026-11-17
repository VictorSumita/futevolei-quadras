using FutevoleiApi.DTOs;
using FutevoleiApi.Entities;
using FutevoleiApi.Enums;
using FutevoleiApi.Repositories;

namespace FutevoleiApi.Services;

public class ReservaService : IReservaService
{
    private readonly IReservaRepository _reservaRepository;
    private readonly IQuadraRepository _quadraRepository;

    public ReservaService(IReservaRepository reservaRepository, IQuadraRepository quadraRepository)
    {
        _reservaRepository = reservaRepository;
        _quadraRepository = quadraRepository;
    }

    public async Task<List<ReservaResponse>> ObterTodasAsync()
    {
        var reservas = await _reservaRepository.ObterTodasAsync();
        return reservas.Select(MapearParaResponse).ToList();
    }

    public async Task<List<ReservaResponse>> ObterPorQuadraAsync(string quadraId)
    {
        var reservas = await _reservaRepository.ObterPorQuadraAsync(quadraId);
        return reservas.Select(MapearParaResponse).ToList();
    }

    public async Task<ReservaResponse?> ObterPorIdAsync(string id)
    {
        var reserva = await _reservaRepository.ObterPorIdAsync(id);
        return reserva is null ? null : MapearParaResponse(reserva);
    }

    public async Task<ResultadoReserva> CriarAsync(ReservaRequest request)
    {
        var inicio = ParaUtc(request.Inicio);
        var fim = ParaUtc(request.Fim);

        var validacao = await ValidarAsync(request.QuadraId, inicio, fim, null);
        if (validacao is not null)
            return validacao;

        var quadra = await _quadraRepository.ObterPorIdAsync(request.QuadraId);

        var reserva = new Reserva
        {
            QuadraId = request.QuadraId,
            Responsavel = request.Responsavel.Trim(),
            Telefone = request.Telefone.Trim(),
            Inicio = inicio,
            Fim = fim,
            ValorTotal = CalcularValor(quadra!, inicio, fim),
            Status = StatusReserva.Confirmada,
            CriadaEm = DateTime.UtcNow
        };

        var criada = await _reservaRepository.CriarAsync(reserva);
        return new ResultadoReserva(true, 201, null, MapearParaResponse(criada));
    }

    public async Task<ResultadoReserva> AtualizarAsync(string id, ReservaRequest request)
    {
        var existente = await _reservaRepository.ObterPorIdAsync(id);
        if (existente is null)
            return new ResultadoReserva(false, 404, "Reserva não encontrada.", null);

        var inicio = ParaUtc(request.Inicio);
        var fim = ParaUtc(request.Fim);

        var validacao = await ValidarAsync(request.QuadraId, inicio, fim, id);
        if (validacao is not null)
            return validacao;

        var quadra = await _quadraRepository.ObterPorIdAsync(request.QuadraId);

        existente.QuadraId = request.QuadraId;
        existente.Responsavel = request.Responsavel.Trim();
        existente.Telefone = request.Telefone.Trim();
        existente.Inicio = inicio;
        existente.Fim = fim;
        existente.ValorTotal = CalcularValor(quadra!, inicio, fim);

        await _reservaRepository.AtualizarAsync(id, existente);
        return new ResultadoReserva(true, 204, null, MapearParaResponse(existente));
    }

    public async Task<bool> CancelarAsync(string id)
    {
        var reserva = await _reservaRepository.ObterPorIdAsync(id);
        if (reserva is null)
            return false;

        reserva.Status = StatusReserva.Cancelada;
        return await _reservaRepository.AtualizarAsync(id, reserva);
    }

    public async Task<bool> RemoverAsync(string id) =>
        await _reservaRepository.RemoverAsync(id);

    private async Task<ResultadoReserva?> ValidarAsync(string quadraId, DateTime inicio, DateTime fim, string? ignorarReservaId)
    {
        if (string.IsNullOrWhiteSpace(quadraId))
            return new ResultadoReserva(false, 400, "O id da quadra é obrigatório.", null);

        if (fim <= inicio)
            return new ResultadoReserva(false, 400, "O horário de término deve ser posterior ao de início.", null);

        var quadra = await _quadraRepository.ObterPorIdAsync(quadraId);
        if (quadra is null)
            return new ResultadoReserva(false, 404, "Quadra não encontrada.", null);

        if (!quadra.Ativa)
            return new ResultadoReserva(false, 400, "A quadra informada está inativa.", null);

        if (inicio < DateTime.UtcNow.AddMinutes(-1))
            return new ResultadoReserva(false, 400, "Não é possível reservar um horário no passado.", null);

        var existeConflito = await _reservaRepository.ExisteConflitoAsync(quadraId, inicio, fim, ignorarReservaId);
        if (existeConflito)
            return new ResultadoReserva(false, 409, "Já existe uma reserva para esta quadra neste intervalo.", null);

        return null;
    }

    private static decimal CalcularValor(Quadra quadra, DateTime inicio, DateTime fim)
    {
        var horas = (decimal)(fim - inicio).TotalHours;
        return Math.Round(horas * quadra.PrecoHora, 2);
    }

    private static DateTime ParaUtc(DateTime data) => data.Kind switch
    {
        DateTimeKind.Utc => data,
        DateTimeKind.Local => data.ToUniversalTime(),
        _ => DateTime.SpecifyKind(data, DateTimeKind.Local).ToUniversalTime()
    };

    private static ReservaResponse MapearParaResponse(Reserva reserva) => new()
    {
        Id = reserva.Id,
        QuadraId = reserva.QuadraId,
        Responsavel = reserva.Responsavel,
        Telefone = reserva.Telefone,
        Inicio = reserva.Inicio,
        Fim = reserva.Fim,
        ValorTotal = reserva.ValorTotal,
        Status = reserva.Status,
        CriadaEm = reserva.CriadaEm
    };
}
