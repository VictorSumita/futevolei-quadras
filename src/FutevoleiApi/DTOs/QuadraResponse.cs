using FutevoleiApi.Enums;

namespace FutevoleiApi.DTOs;

public class QuadraResponse
{
    public string Id { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public TipoPiso TipoPiso { get; set; }
    public bool Coberta { get; set; }
    public decimal PrecoHora { get; set; }
    public bool Ativa { get; set; }
    public DateTime CriadaEm { get; set; }
}
