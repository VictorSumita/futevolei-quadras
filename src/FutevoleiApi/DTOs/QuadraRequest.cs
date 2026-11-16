using FutevoleiApi.Enums;

namespace FutevoleiApi.DTOs;

public class QuadraRequest
{
    public string Nome { get; set; } = string.Empty;
    public TipoPiso TipoPiso { get; set; }
    public bool Coberta { get; set; }
    public decimal PrecoHora { get; set; }
    public bool Ativa { get; set; } = true;
}
