# Futevolei Quadras

**Aluno:** Victor Hugo Leroy Sumita 202451084534 · **Curso:** Analise e Desenvolvimento de Sistemas · **Turma:** 3001

API REST para gestao de quadras de futevolei e reserva de horarios.
Desenvolvida em .NET 10 com ASP.NET Core e MongoDB.

## Requisitos

- .NET SDK 10
- Docker (para o MongoDB)

## Como executar

### 1. Subir o MongoDB

```bash
docker run -d --name mongo-futevolei -p 27017:27017 -v mongo-data:/data/db mongo:7
```

Se o container ja existir e estiver parado:

```bash
docker start mongo-futevolei
```

### 2. Rodar a API

```bash
dotnet run --project src/FutevoleiApi
```

A documentacao interativa abre em:

```
http://localhost:5244/swagger
```

## Como executar os testes

```bash
dotnet test
```

Sao 17 testes unitarios da camada de servico. Nao exigem MongoDB nem Docker:
os repositorios sao substituidos por implementacoes em memoria.

## Endpoints

### Quadras

| Metodo | Rota | Descricao |
|--------|------|-----------|
| GET | /api/quadras | Lista todas as quadras |
| GET | /api/quadras/{id} | Busca quadra por id |
| POST | /api/quadras | Cria quadra |
| PUT | /api/quadras/{id} | Atualiza quadra |
| DELETE | /api/quadras/{id} | Remove quadra |

### Reservas

| Metodo | Rota | Descricao |
|--------|------|-----------|
| GET | /api/reservas | Lista todas as reservas |
| GET | /api/reservas/quadra/{quadraId} | Reservas de uma quadra |
| GET | /api/reservas/{id} | Busca reserva por id |
| POST | /api/reservas | Cria reserva |
| PUT | /api/reservas/{id} | Atualiza reserva |
| PATCH | /api/reservas/{id}/cancelar | Cancela reserva |
| DELETE | /api/reservas/{id} | Remove reserva |

## Regras de negocio

A criacao de reserva aplica as seguintes validacoes, nesta ordem:

| Situacao | Status |
|----------|--------|
| quadraId vazio | 400 |
| fim anterior ou igual ao inicio | 400 |
| quadra inexistente | 404 |
| quadra inativa | 400 |
| inicio no passado | 400 |
| horario ja ocupado | 409 |

**Deteccao de conflito.** Duas reservas colidem quando os intervalos se
sobrepoem: `inicio < fim_existente && fim > inicio_existente`. Intervalos que
apenas se encostam (uma termina 20:00, a outra comeca 20:00) nao conflitam.
Reservas com status Cancelada sao ignoradas na verificacao, liberando o horario.

**Calculo do valor.** `valorTotal = horas x precoHora`, arredondado em duas
casas decimais. Uma reserva de 1h30 em uma quadra de R$ 85,50 resulta em
R$ 128,25.

**Fuso horario.** Datas recebidas sem timezone sao interpretadas como horario
local e convertidas para UTC antes da persistencia. O MongoDB armazena sempre
em UTC, garantindo que a comparacao de conflito use uma base unica.

## Estrutura

```
src/FutevoleiApi
  Configuration/   Configuracao do MongoDB
  Controllers/     Endpoints HTTP
  DTOs/            Contratos de entrada e saida
  Entities/        Modelos persistidos
  Enums/           TipoPiso, StatusReserva
  Repositories/    Acesso ao MongoDB
  Services/        Regras de negocio

tests/FutevoleiApi.Tests
  Fakes/           Repositorios em memoria
  Services/        Testes das regras de negocio
```

## Decisoes de projeto

**Service devolve resultado estruturado.** O servico retorna `ResultadoReserva`
com status e mensagem, em vez de lancar excecoes para fluxos esperados. O
controller apenas repassa o status, mantendo a regra de negocio fora da camada
HTTP.

**Testes sem infraestrutura.** Os testes usam repositorios em memoria, o que
permite `dotnet test` funcionar em qualquer maquina sem Docker. O filtro de
conflito em memoria espelha a mesma expressao usada no MongoDB.

## Uso de IA

- **Ferramentas usadas:** Adapta ONE (assistente de IA generativa)
- **Onde ajudou:** gerou o esqueleto dos repositorios Mongo, o mapeamento de
  ObjectId e a estrutura inicial do ReservaService e dos testes
- **O que eu revisei/reescrevi:** revisei a conversao de fuso no ParaUtc e a
  ordem das validacoes no ValidarAsync; ajustei o calculo do valor total para
  arredondar em duas casas e o retorno 409 para conflito
