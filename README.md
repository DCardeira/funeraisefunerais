# Funerais e Funerais — Projeto de Desenvolvimento Web

Solução em ASP.NET Core 8: Razor Pages + Controllers de API REST,
partilhando o mesmo modelo de dados via Entity Framework Core.

## Modelo de dados

- **Cliente** — família/pessoa de contacto.
- **Servico** — serviços/produtos da funerária (velório, cremação, transporte, flores, documentação).
- **Cerimonia** — o processo de acompanhamento de um funeral. Relação **muitos-para-um** com `Cliente`.
- **CerimoniaServico** — tabela de junção que implementa a relação **muitos-para-muitos** entre
  `Cerimonia` e `Servico` (com quantidade e preço acordado).
- Utilizadores via **ASP.NET Identity**, com 2 papéis: `Administrador` e `Cliente`.

## Como correr localmente

Pré-requisitos: .NET 8 SDK, e SQL Server LocalDB (já vem com o Visual Studio) ou uma instância SQL Server.

```bash
cd funeraisefunerais
dotnet restore
dotnet ef migrations add Inicial
dotnet ef database update
dotnet run
```

Isto cria a base de dados `FunerariaDb`, aplica o *seed* de serviços e cria 2 contas de teste
(ver credenciais na página **/Sobre**, também exigidas pelas regras de avaliação):

| Papel | Email | Palavra-passe |
|---|---|---|
| Administrador | admin@funeraria.pt | Admin123! |
| Cliente | cliente@funeraria.pt | Cliente123! |

Se não tiverem a ferramenta `dotnet ef` instalada:
```bash
dotnet tool install --global dotnet-ef
```

## Estrutura de pastas

```
FunerariaWeb/
  Controllers/Api/     -> Componente 2: endpoints REST (Clientes, Servicos, Cerimonias)
  Pages/                -> Componente 1: Razor Pages (CRUD completo)
  Pages/Conta/          -> Login/Logout com ASP.NET Identity
  Models/               -> Entidades EF Core
  Data/                 -> DbContext + seed de dados/roles/utilizadores
  Hubs/                 -> SignalR (notificações em tempo real no painel admin)
  wwwroot/              -> CSS/JS estáticos
```
