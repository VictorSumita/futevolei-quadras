using FutevoleiApi.DTOs;
using FutevoleiApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FutevoleiApi.Controllers;

[ApiController]
[Route("api/reservas")]
public class ReservaController : ControllerBase
{
    private readonly IReservaService _service;

    public ReservaController(IReservaService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var reservas = await _service.ObterTodasAsync();
        return Ok(reservas);
    }

    [HttpGet("quadra/{quadraId}")]
    public async Task<IActionResult> ObterPorQuadra(string quadraId)
    {
        var reservas = await _service.ObterPorQuadraAsync(quadraId);
        return Ok(reservas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var reserva = await _service.ObterPorIdAsync(id);
        if (reserva is null)
            return NotFound(new { mensagem = "Reserva não encontrada." });

        return Ok(reserva);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] ReservaRequest request)
    {
        var resultado = await _service.CriarAsync(request);
        if (!resultado.Sucesso)
            return StatusCode(resultado.StatusCode, new { mensagem = resultado.Erro });

        return CreatedAtAction(nameof(ObterPorId), new { id = resultado.Dados!.Id }, resultado.Dados);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] ReservaRequest request)
    {
        var resultado = await _service.AtualizarAsync(id, request);
        if (!resultado.Sucesso)
            return StatusCode(resultado.StatusCode, new { mensagem = resultado.Erro });

        return NoContent();
    }

    [HttpPatch("{id}/cancelar")]
    public async Task<IActionResult> Cancelar(string id)
    {
        var cancelou = await _service.CancelarAsync(id);
        if (!cancelou)
            return NotFound(new { mensagem = "Reserva não encontrada." });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(string id)
    {
        var removeu = await _service.RemoverAsync(id);
        if (!removeu)
            return NotFound(new { mensagem = "Reserva não encontrada." });

        return NoContent();
    }
}
