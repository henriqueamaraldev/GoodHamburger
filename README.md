# Good Hamburger

Sistema de registro de pedidos para a lanchonete Good Hamburger. API REST em ASP.NET Core com frontend em Blazor Server.

## Início rápido

> Requer [.NET 10 SDK](https://dotnet.microsoft.com/download) e [Docker](https://www.docker.com/products/docker-desktop) instalados.

A partir da pasta `GoodHamburger/`, um único comando sobe o banco, aplica as migrations, semeia o cardápio e abre o navegador:

**PowerShell:**

```powershell
.\start.ps1
```

**Git Bash:**

```bash
bash start.sh
```

| | URL |
| --- | --- |
| Frontend | <http://localhost:5200> |
| Swagger | <http://localhost:5100/swagger> |

---

## Pré-requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/products/docker-desktop) (para o PostgreSQL)
- [Node.js](https://nodejs.org/) (apenas se quiser recompilar o CSS do Blazor)

## Executando (forma rápida)

A partir da pasta `GoodHamburger/`, execute o script correspondente ao seu terminal. Ele sobe o banco via Docker, aguarda o PostgreSQL ficar pronto, inicia a API (que aplica as migrations e o seed automaticamente) e em seguida o Blazor, abrindo o navegador ao final.

**PowerShell:**

```powershell
.\start.ps1
```

**Git Bash:**

```bash
bash start.sh
```

- Frontend: `http://localhost:5200`
- API / Swagger: `http://localhost:5100/swagger`

## Executando manualmente

Todos os comandos a partir da pasta `GoodHamburger/`.

### 1. Banco de dados

```bash
docker-compose up -d
```

### 2. API

```bash
cd Api && dotnet run
```

Na primeira execução em `Development`, as migrations são aplicadas e o banco é populado com o cardápio automaticamente.

### 3. Frontend (Blazor)

Em outro terminal:

```bash
cd Blazor && dotnet run
```

### 4. Testes

```bash
dotnet test Tests/Tests.csproj
```

Para rodar um teste específico:

```bash
dotnet test Tests/Tests.csproj --filter "FullyQualifiedName~SandwichFriesSoda"
```

## Cardápio

| Item | Tipo | Preço |
| ---- | ---- | ----- |
| X Burger | Sanduíche | R$ 5,00 |
| X Egg | Sanduíche | R$ 4,50 |
| X Bacon | Sanduíche | R$ 7,00 |
| Batata frita | Acompanhamento | R$ 2,00 |
| Refrigerante | Acompanhamento | R$ 2,50 |

## Regras de Desconto

| Combo | Desconto |
| ----- | -------- |
| Sanduíche + batata + refrigerante | 20% |
| Sanduíche + refrigerante | 15% |
| Sanduíche + batata | 10% |

Cada pedido aceita no máximo um sanduíche, uma batata e um refrigerante. Itens duplicados retornam erro 400.

## Endpoints

### Menu

| Método | Rota | Descrição |
| ------ | ---- | --------- |
| `GET` | `/api/menu` | Lista todos os itens do cardápio |

### Pedidos

| Método | Rota | Descrição |
| ------ | ---- | --------- |
| `POST` | `/api/orders` | Cria um pedido |
| `GET` | `/api/orders` | Lista todos os pedidos |
| `GET` | `/api/orders/{id}` | Consulta um pedido por ID |
| `PUT` | `/api/orders/{id}` | Atualiza um pedido |
| `DELETE` | `/api/orders/{id}` | Remove um pedido |

**Exemplo de criação de pedido:**

```json
POST /api/orders
{
  "menuItemIds": [
    "id-do-x-burger",
    "id-da-batata-frita",
    "id-do-refrigerante"
  ]
}
```

**Resposta (201 Created):**

```json
{
  "id": "...",
  "status": "Pending",
  "subtotal": 9.50,
  "discount": 1.90,
  "total": 7.60,
  "items": [...]
}
```

## Arquitetura

Clean Architecture com quatro camadas. A dependência segue a direção `Api → Application → Domain`; `Infrastructure` implementa as interfaces definidas em `Application`.

```text
Domain/          Entidades e enums. Sem dependências externas.
Application/     Serviços, DTOs e exceções de domínio. Referencia Infrastructure via interfaces.
Infrastructure/  EF Core, AppDbContext, repositórios e migrations.
Api/             Controllers, middleware de exceção e configuração de DI.
Blazor/          Frontend Blazor Server consumindo a API via HttpClient.
Tests/           xUnit + NSubstitute. Testes de domínio e de serviço com repositórios mockados.
```

### Decisões relevantes

**Repositório genérico + especializado** — `IRepository<T>` cobre o CRUD padrão. `IOrderRepository` estende com `GetWithItemsAsync`, que faz o eager loading de itens e menu items em uma única query.

**Cálculo de desconto no domínio** — `Order.Calculate()` resolve subtotal, desconto e total a partir dos itens já associados. A lógica fica no agregado, não no serviço, tornando os testes de desconto independentes de infraestrutura.

**Middleware de exceção** — `ServiceException(message, HttpStatusCode)` é lançada pelos serviços e convertida em `{ "error": "..." }` com o status correspondente pelo `CustomExceptionMiddleware`. Exceções não tratadas retornam 500.

**Banco populado automaticamente** — `DbInitializer` aplica migrations e insere o cardápio na primeira execução em `Development`. Não é necessário rodar scripts SQL manualmente.

**Enum serializado como string** — `JsonStringEnumConverter` garante que `status` apareça como `"Pending"` nas respostas, não como `1`.

## O que ficou fora

- **Status de pedido além de `Pending`** — o desafio não define um fluxo de status (ex.: em preparo, entregue), então apenas `Pending` foi implementado.
- **Autenticação/autorização** — fora do escopo do desafio.
- **Paginação na listagem** — `GET /api/orders` retorna todos os pedidos sem paginação. Para volumes maiores seria necessário cursor ou offset/limit.
- **Internacionalização das mensagens de erro** — as mensagens de validação estão em inglês.
