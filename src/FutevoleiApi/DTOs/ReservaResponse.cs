using FutevoleiApi.Enums;

namespace FutevoleiApi.DTOs;

public class ReservaResponse
{
    public string Id { get; set; } = string.Empty;
    public string QuadraId { get; set; } = string.Empty;
    public string Responsavel { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
    public decimal ValorTotal { get; set; }
    public StatusReserva Status { get; set; }
    public DateTime CriadaEm { get; set; }
}
