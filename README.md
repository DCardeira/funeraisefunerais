# Funerais e Funerais — Projeto de Desenvolvimento Web

Solução em ASP.NET Core 8: Razor Pages (Componente 1) + Controllers de API REST (Componente 2),
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
cd FunerariaWeb
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

## Publicar no Azure

### 1. Base de dados — Azure SQL Database
1. No portal Azure: **Criar recurso → Azure SQL Database** (tier gratuito/estudante se disponível).
2. Criem um *server* novo, anotem o utilizador/palavra-passe de administração.
3. Em **Networking**, ativem "Allow Azure services to access this server" (para o App Service conseguir ligar).
4. Copiem a *connection string* (ADO.NET) fornecida pelo portal.

### 2. Aplicação — Azure App Service
1. No portal: **Criar recurso → Web App**. Stack: **.NET 8**, SO: Linux ou Windows (tanto faz).
2. Depois de criado, vão a **Configuração → Connection strings** e adicionem `DefaultConnection`
   com o valor copiado do Azure SQL (tipo `SQLAzure`). Isto sobrepõe-se automaticamente ao
   `appsettings.json` — não precisam de alterar código nem fazer commit de passwords.
3. Publicar a partir do Visual Studio: botão direito no projeto **FunerariaWeb → Publish → Azure →
   Azure App Service (Windows/Linux)** e escolham a Web App criada.
   Ou via linha de comandos:
   ```bash
   dotnet publish -c Release
   az webapp deploy --resource-group <nome-grupo> --name <nome-app> --src-path ./bin/Release/net8.0/publish.zip --type zip
   ```
4. Depois do primeiro deploy, a app corre `DbInitializer.SeedAsync` automaticamente e aplica as
   migrações à base de dados Azure SQL — não é preciso correr `dotnet ef database update` manualmente
   em produção.
5. Testem o URL público (`https://<nome-app>.azurewebsites.net`) — é essa app que deve estar
   disponível durante a defesa (ver regras: "a aplicação a ser executada será a presente no servidor web").

### Alternativa: MySQL em vez de SQL Server
Se preferirem MySQL, troquem o pacote `Microsoft.EntityFrameworkCore.SqlServer` por
`Pomelo.EntityFrameworkCore.MySql` no `.csproj`, e em `Program.cs` troquem `UseSqlServer` por
`UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))`. **Atenção:** as regras
exigem que forneçam os scripts SQL de criação de BD/tabelas/utilizadores nesse caso — o EF Migrations
não substitui essa entrega.

## O que falta fazer (lista de verificação)

- [ ] Ajustar `Pages/Sobre.cshtml` com os nomes/números reais dos autores e a lista final de bibliotecas.
- [ ] Rever regras de negócio específicas da vossa funerária (ex.: talvez queiram um `Funcionario`
      responsável por cada cerimónia — é só mais uma tabela com m:1, seguindo o padrão de `Cliente`).
- [ ] Página de registo de novos clientes (`/Conta/Registar`), se quiserem permitir self-signup em
      vez de só o Admin criar clientes.
- [ ] Testes de usabilidade (SUS/ISO 9241) — já mencionado como prática comum no vosso outro trabalho.
- [ ] Publicação efetiva no Azure (ver acima) — Componente 3.
- [ ] Referenciar aqui e no código qualquer biblioteca/código de terceiros adicional que venham a usar.
- [ ] Garantir código comentado (o scaffold já tem comentários explicativos — mantenham o padrão).
- [ ] Configurar o repositório GitHub com histórico de commits reais (não um único commit).

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
