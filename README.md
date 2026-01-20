# 🚀 Dapper with DB Secrets

![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
![Dapper](https://img.shields.io/badge/Dapper-007ACC?style=for-the-badge&logo=dapper&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)

Este repositório é um projeto de **estudo e reciclagem de conhecimento** focado em acesso a dados utilizando .NET e o micro-ORM **Dapper**.

## 🎯 Objetivo

O foco principal desta aplicação é demonstrar e praticar:
- Configuração de strings de conexão seguras.
- Utilização do **Dapper** para operações CRUD de alta performance.
- Integração com **PostgreSQL** usando `Npgsql`.
- Melhores práticas de injeção de dependência e gerenciamento de configuração em ASP.NET Core.

## 🛠️ Tecnologias Utilizadas

- **C# / .NET 9**
- **Dapper**: Micro-ORM simples e rápido para mapeamento de objetos.
- **Npgsql**: Provedor ADO.NET para PostgreSQL.
- **Swagger/OpenAPI**: Para documentação e testes da API.

## 🚀 Como Executar

1.  **Pré-requisitos**:
    - Possuir o SDK do .NET 9 instalado.
    - Ter uma instância do PostgreSQL rodando localmente (ou via Docker).

2.  **Configuração do Banco**:
    - Ajuste a string de conexão no arquivo `appsettings.json`:
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Port=5432;Database=postgres;User Id=postgres;Password=sua_senha;"
    }
    ```

3.  **Rodar a aplicação**:
    ```bash
    dotnet run --project dbsecrets.api
    ```

4.  **Acessar o Swagger**:
    Abra o navegador em: `http://localhost:<porta>/swagger/index.html`

## 📄 Estrutura do Projeto

- `dbsecrets.api/Controllers/`: Endpoints da API (Ex: `HomeController`).
- `dbsecrets.api/Models/`: Classes de domínio e DTOs.
- `appsettings.json`: Configurações centralizadas da aplicação.

---
> [!NOTE]
> Este projeto possui fins puramente didáticos. Sinta-se à vontade para explorar e sugerir melhorias!

Feito com ❤️ por [Eliel França](https://github.com/elielfranca)
