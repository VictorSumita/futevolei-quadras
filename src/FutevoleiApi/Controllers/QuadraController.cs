using FutevoleiApi.DTOs;
using FutevoleiApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace FutevoleiApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuadraController : ControllerBase
{
    private readonly IQuadraService _service;

    public QuadraController(IQuadraService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> ObterTodas()
    {
        var quadras = await _service.ObterTodasAsync();
        return Ok(quadras);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObterPorId(string id)
    {
        var quadra = await _service.ObterPorIdAsync(id);
        if (quadra is null)
            return NotFound(new { mensagem = "Quadra não encontrada." });

        return Ok(quadra);
    }

    [HttpPost]
    public async Task<IActionResult> Criar([FromBody] QuadraRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new { mensagem = "O nome da quadra é obrigatório." });

        if (request.PrecoHora <= 0)
            return BadRequest(new { mensagem = "O preço por hora deve ser maior que zero." });

        var criada = await _service.CriarAsync(request);
        return CreatedAtAction(nameof(ObterPorId), new { id = criada.Id }, criada);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Atualizar(string id, [FromBody] QuadraRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Nome))
            return BadRequest(new { mensagem = "O nome da quadra é obrigatório." });

        if (request.PrecoHora <= 0)
            return BadRequest(new { mensagem = "O preço por hora deve ser maior que zero." });

        var atualizou = await _service.AtualizarAsync(id, request);
        if (!atualizou)
            return NotFound(new { mensagem = "Quadra não encontrada." });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remover(string id)
    {
        var removeu = await _service.RemoverAsync(id);
        if (!removeu)
            return NotFound(new { mensagem = "Quadra não encontrada." });

        return NoContent();
    }
}
