# AnimesProtech API

Este projeto é uma API para gerenciar animes e usuários, seguindo os princípios de arquitetura limpa (Clean Architecture) e padrões REST.
Projeto desenvolvido para desafio técnico Protech

## Funcionalidades

- **Animes**: CRUD completo com paginação e filtros.
- **Autenticação**: JWT com controle de acesso baseado em papéis.
- **Logs**: Registro de operações de CRUD realizadas.
- **Testes**: Cobertura com testes unitários usando xUnit e Moq.

## Configuração do Ambiente

### Pré-requisitos

- .NET 8.0 SDK
- PostgreSQL
- Git

### Como Configurar

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/AnimesProtech.git
   cd AnimesProtech
    ```

2. Configure as variáveis de ambiente no arquivo .env:
   ```bash
    SECRET_KEY=uma-chave-secreta
    DB_CONNECTION_STRING=Host=localhost;Database=animes_db;Username=seu_usuario;Password=sua_senha;
    ```

3. Execute as migrations:
   ```bash
    dotnet ef database update -p AnimesProtech.Infrastructure -s AnimesProtech.Web
    ```

4. Iniciando a Aplicação
   ```bash
    dotnet run --project AnimesProtech.Web
    ```

### Branches
* master: Branch principal com a versão final do projeto.
* develop: Branch de desenvolvimento.

