using FutevoleiApi.DTOs;
using FutevoleiApi.Entities;
using FutevoleiApi.Enums;
using FutevoleiApi.Services;
using FutevoleiApi.Tests.Fakes;
using Xunit;

namespace FutevoleiApi.Tests.Services;

public class ReservaServiceTests
{
    private const string QuadraId = "6afb95431c506a0d7737babb";
    private static readonly DateTime Dia = DateTime.UtcNow.Date.AddDays(60);

    private static DateTime H(double horas) => Dia.AddHours(horas);

    private static (ReservaService servico, FakeReservaRepository reservas, FakeQuadraRepository quadras)
        CriarServico(bool quadraAtiva = true)
    {
        var reservas = new FakeReservaRepository();
        var quadras = new FakeQuadraRepository();

        quadras.Quadras.Add(new Quadra
        {
            Id = QuadraId,
            Nome = "Quadra Central",
            TipoPiso = TipoPiso.Areia,
            Coberta = true,
            PrecoHora = 85.50m,
            Ativa = quadraAtiva
        });

        return (new ReservaService(reservas, quadras), reservas, quadras);
    }

    private static ReservaRequest Req(DateTime inicio, DateTime fim) => new()
    {
        QuadraId = QuadraId,
        Responsavel = "Joao",
        Telefone = "11999998888",
        Inicio = inicio,
        Fim = fim
    };

    [Fact]
    public async Task Criar_ComDadosValidos_Retorna201EValorDeUmaHora()
    {
        var (servico, _, _) = CriarServico();
        var r = await servico.CriarAsync(Req(H(19), H(20)));

        Assert.True(r.Sucesso);
        Assert.Equal(201, r.StatusCode);
        Assert.Equal(85.50m, r.Dados!.ValorTotal);
    }

    [Fact]
    public async Task Criar_ComUmaHoraEMeia_CalculaValorProporcional()
    {
        var (servico, _, _) = CriarServico();
        var r = await servico.CriarAsync(Req(H(19), H(20.5)));

        Assert.Equal(128.25m, r.Dados!.ValorTotal);
    }

    [Theory]
    [InlineData(20.0, 21.0, true)]
    [InlineData(18.0, 19.0, true)]
    [InlineData(19.5, 20.5, false)]
    [InlineData(18.0, 21.0, false)]
    [InlineData(19.25, 19.75, false)]
    [InlineData(19.0, 20.0, false)]
    public async Task Criar_DetectaConflitoDeHorario(double inicio, double fim, bool devePassar)
    {
        var (servico, _, _) = CriarServico();
        await servico.CriarAsync(Req(H(19), H(20)));

        var r = await servico.CriarAsync(Req(H(inicio), H(fim)));

        if (devePassar)
        {
            Assert.True(r.Sucesso);
            Assert.Equal(201, r.StatusCode);
        }
        else
        {
            Assert.False(r.Sucesso);
            Assert.Equal(409, r.StatusCode);
        }
    }

    [Fact]
    public async Task Criar_ComFimAntesDoInicio_Retorna400()
    {
        var (servico, _, _) = CriarServico();
        var r = await servico.CriarAsync(Req(H(20), H(19)));

        Assert.Equal(400, r.StatusCode);
        Assert.Contains("posterior", r.Erro!);
    }

    [Fact]
    public async Task Criar_ComDataNoPassado_Retorna400()
    {
        var (servico, _, _) = CriarServico();
        var inicio = DateTime.UtcNow.AddDays(-10);

        var r = await servico.CriarAsync(Req(inicio, inicio.AddHours(1)));

        Assert.Equal(400, r.StatusCode);
    }

    [Fact]
    public async Task Criar_ComQuadraInexistente_Retorna404()
    {
        var (servico, _, _) = CriarServico();
        var req = Req(H(19), H(20));
        req.QuadraId = "000000000000000000000000";

        var r = await servico.CriarAsync(req);

        Assert.Equal(404, r.StatusCode);
    }

    [Fact]
    public async Task Criar_ComQuadraInativa_Retorna400()
    {
        var (servico, _, _) = CriarServico(quadraAtiva: false);
        var r = await servico.CriarAsync(Req(H(19), H(20)));

        Assert.Equal(400, r.StatusCode);
        Assert.Contains("inativa", r.Erro!);
    }

    [Fact]
    public async Task Criar_SemQuadraId_Retorna400()
    {
        var (servico, _, _) = CriarServico();
        var req = Req(H(19), H(20));
        req.QuadraId = "";

        var r = await servico.CriarAsync(req);

        Assert.Equal(400, r.StatusCode);
    }

    [Fact]
    public async Task Criar_ConverteDataSemFusoParaUtc()
    {
        var (servico, _, _) = CriarServico();
        var semFuso = DateTime.SpecifyKind(Dia.AddHours(19), DateTimeKind.Unspecified);

        var r = await servico.CriarAsync(Req(semFuso, semFuso.AddHours(1)));

        Assert.Equal(DateTimeKind.Utc, r.Dados!.Inicio.Kind);
        Assert.Equal(DateTime.SpecifyKind(semFuso, DateTimeKind.Local).ToUniversalTime(), r.Dados.Inicio);
    }

    [Fact]
    public async Task Cancelar_LiberaOHorarioParaNovaReserva()
    {
        var (servico, _, _) = CriarServico();
        var criada = await servico.CriarAsync(Req(H(19), H(20)));

        Assert.True(await servico.CancelarAsync(criada.Dados!.Id));

        var nova = await servico.CriarAsync(Req(H(19), H(20)));

        Assert.True(nova.Sucesso);
        Assert.Equal(201, nova.StatusCode);
    }

    [Fact]
    public async Task Cancelar_AlteraStatusParaCancelada()
    {
        var (servico, _, _) = CriarServico();
        var criada = await servico.CriarAsync(Req(H(19), H(20)));

        await servico.CancelarAsync(criada.Dados!.Id);

        var consultada = await servico.ObterPorIdAsync(criada.Dados.Id);

        Assert.Equal(StatusReserva.Cancelada, consultada!.Status);
    }

    [Fact]
    public async Task Criar_RemoveEspacosDeResponsavelETelefone()
    {
        var (servico, _, _) = CriarServico();
        var req = Req(H(19), H(20));
        req.Responsavel = "  Maria  ";
        req.Telefone = "  11999998888  ";

        var r = await servico.CriarAsync(req);

        Assert.Equal("Maria", r.Dados!.Responsavel);
        Assert.Equal("11999998888", r.Dados.Telefone);
    }
}
