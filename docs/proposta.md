\# Proposta — Trabalho Semestral · Arquitetura de Aplicações Web 2026.2



\- \*\*Aluno:\*\* Victor Sumita 202451084534 · \*\*Curso/Turma:\*\* Arquitetura de Aplicações WEB

\- \*\*Repositório:\*\* https://github.com/VictorSumita/futevolei-quadras



\## 1. Domínio e problema



Aplicação web para gerenciamento de reservas de quadras de futevôlei em arenas esportivas. O problema que resolve é o controle manual de agenda, que hoje é feito por WhatsApp e caderno, gerando conflitos de horário e perda de reservas. O público-alvo é o administrador da arena, que precisa cadastrar suas quadras e organizar a ocupação diária sem sobreposição de horários.



\## 2. Entidades



\*\*Entidade A — Quadra:\*\* identificacao (nome/número), tipoPiso, coberta (booleano), capacidadeJogadores, valorHora (decimal), status (ativa/manutencao).



\*\*Entidade B — Reserva:\*\* quadraId (referência), data, horaInicio, horaFim, nomeResponsavel, valorTotal (decimal), status (confirmada/cancelada).



\*\*Relacionamento:\*\* 1-N — uma Quadra possui várias Reservas; cada Reserva pertence a exatamente uma Quadra.



\*\*Modelagem NoSQL:\*\* referência por `quadraId`, não documento embutido.

\*\*Justificativa:\*\* Coloquei a reserva guardando só o quadraId da quadra, em vez de copiar o documento da quadra inteiro dentro dela. Dois motivos. Primeiro, a quadra existe por conta própria: ela é cadastrada antes de qualquer reserva e continua lá depois que as reservas são canceladas ou apagadas. Se eu embutisse, apagar uma reserva antiga viraria uma edição no documento da quadra — o que não faz sentido nenhum. Segundo, o volume: uma quadra de arena acumula centenas de reservas com o tempo. Embutido, todo GET /api/quadras traria o histórico inteiro junto e o documento ia crescendo sem limite. Referenciando, eu trago um documento pequeno e busco as reservas só quando preciso. Tem ainda um terceiro detalhe: se o valor da hora da quadra mudar, com referência eu atualizo um lugar só; com documento embutido, teria que atualizar todas as reservas já criadas — e aí é questão de tempo até ficar inconsistente.



\## 3. Endpoints previstos



\*\*Quadras\*\*

\- `GET /api/quadras` — lista todas as quadras

\- `GET /api/quadras/{id}` — busca quadra por id

\- `POST /api/quadras` — cria quadra

\- `PUT /api/quadras/{id}` — atualiza quadra

\- `DELETE /api/quadras/{id}` — remove quadra



\*\*Reservas\*\*

\- `GET /api/reservas` — lista reservas (filtro opcional por quadraId e data)

\- `GET /api/reservas/{id}` — busca reserva por id

\- `POST /api/reservas` — cria reserva aplicando a regra de conflito

\- `PUT /api/reservas/{id}` — atualiza reserva

\- `DELETE /api/reservas/{id}` — remove reserva



\## 4. Regra de negócio



A camada de serviço impede a criação de duas reservas com sobreposição de horário na mesma quadra no mesmo dia, e calcula o `valorTotal` com base no `valorHora` da quadra multiplicado pela duração em horas.

A checagem de horário sobreposto e o cálculo do valor total ficam na camada de serviço porque são regra de negócio, e o controller não deveria saber disso. O controller só recebe a requisição HTTP, chama quem sabe resolver e devolve a resposta. Se eu jogasse a regra lá dentro, dois problemas: qualquer outro ponto de entrada no futuro (um endpoint de reserva recorrente, um job, um teste) teria que repetir a mesma lógica; e testar ficaria um porre, porque eu precisaria simular uma requisição HTTP inteira só pra verificar se a sobreposição está sendo barrada. No serviço, eu testo a regra direto, chamando o método com dois objetos e conferindo o retorno. Além disso o controller fica limpo, servindo quase como um índice dos endpoints do sistema.



\## 5. Casos de erro



\- `404 Not Found` — GET, PUT ou DELETE com id inexistente.

\- `409 Conflict` — POST de reserva com horário sobreposto em quadra já ocupada.

\- `400 Bad Request` — `horaFim` anterior ou igual a `horaInicio`, ou corpo da requisição inválido.



\## 6. Stack escolhida



Backend: .NET 10 (C#) com ASP.NET Core Web API. Banco: MongoDB (container Docker). Frontend: HTML + JavaScript com `fetch`.



\## 7. Segurança e testes



JWT emitido no login com expiração configurada, perfis `admin` e `usuario` declarados no payload e validados no servidor. Endpoints de escrita e exclusão restritos ao perfil `admin`. Testes unitários da camada de serviço com xUnit, cobrindo dois cenários de sucesso (reserva criada, valor calculado) e dois de erro (conflito de horário, id inexistente), executáveis com `dotnet test`.

