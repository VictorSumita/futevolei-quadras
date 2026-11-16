namespace FutevoleiApi.DTOs;

public class ReservaRequest
{
    public string QuadraId { get; set; } = string.Empty;
    public string Responsavel { get; set; } = string.Empty;
    public string Telefone { get; set; } = string.Empty;
    public DateTime Inicio { get; set; }
    public DateTime Fim { get; set; }
}
