using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Reflection.Metadata;
using System.Runtime.InteropServices.Marshalling;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Dapper;
using Npgsql;
using dbsecrets.api.Models;

namespace dbsecrets.api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private string ConnectionString => _configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to the dbsecrets.api!");
        }

        // Rota explicita para evitar ambiguidade
        [HttpGet("produtos")]
        public async Task<IActionResult> GetProdutos()
        {
            var produtos = new Produtos();

            await using var connection = new NpgsqlConnection(ConnectionString);

            // Abrir explicitamente para que erros de handshake/SSL fiquem claros
            await connection.OpenAsync();

            var query = @"select id,
                                 codigo,
                                 descricao
                          from produto;";

            var result = await connection.QueryAsync<Produto>(query);

            foreach (var item in result)
            {
                produtos.Add(new Produto
                {
                    Id = item.Id,
                    Codigo = item.Codigo,
                    Descricao = item.Descricao
                });
            }

            await connection.CloseAsync();
            await connection.DisposeAsync();

            return Ok(produtos);
        }

        [HttpPost("produtos")]
        public async Task<IActionResult> PostProduto(Produto produto)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);

            await connection.OpenAsync();

            var query = @"INSERT INTO produto (codigo, descricao) 
                         VALUES (@Codigo, @Descricao) 
                         RETURNING id, codigo, descricao;";

            var result = await connection.QuerySingleOrDefaultAsync<Produto>(query, new { produto.Codigo, produto.Descricao });

            await connection.CloseAsync();
            await connection.DisposeAsync();

            if (result == null)
            {
                return BadRequest(new { mensagem = "Erro ao criar produto" });
            }

            return Created($"/produtos/{result.Id}", result);
        }

        [HttpPost("lista-produtos")]
        public async Task<IActionResult> PostListaProdutos(List<Produto> produtos)
        {
            if (produtos == null || produtos.Count <= 0)
                return BadRequest(new { mensagem = "Lista de produtos inválida" });

            await using var connection = new NpgsqlConnection(ConnectionString);

            await connection.OpenAsync();

            var query = @"INSERT INTO produto (codigo, descricao) 
                         VALUES (@Codigo, @Descricao) 
                         RETURNING id, codigo, descricao;";

            var result = new List<Produto>();

            foreach (var produto in produtos)
            {
                var item = await connection.QuerySingleOrDefaultAsync<Produto>(query, new { produto.Codigo, produto.Descricao });
                if (item != null) result.Add(item);
            }

            await connection.CloseAsync();
            await connection.DisposeAsync();

            if (!result.Any())
            {
                return BadRequest(new { mensagem = "Nenhum produto foi criado" });
            }

            return Created($"/produtos", result);
        }


        [HttpPut("produtos")]
        public async Task<IActionResult> PutProduto(Produto produto)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);

            await connection.OpenAsync();

            var query = @"UPDATE produto
                         SET codigo = @Codigo, descricao = @Descricao
                         WHERE id = @Id;";

            var result = await connection.ExecuteAsync(query, new { produto.Id, produto.Codigo, produto.Descricao });

            await connection.CloseAsync();
            await connection.DisposeAsync();

            if (result == 0)
                return BadRequest(new { mensagem = "Erro ao atualizar produto" });

            return Ok(new { mensagem = "Produto atualizado com sucesso" });
        }

        [HttpDelete("produtos")]
        public async Task<IActionResult> DeleteProduto(Produto produto)
        {
            await using var connection = new NpgsqlConnection(ConnectionString);

            await connection.OpenAsync();

            var query = @"DELETE FROM produto
                         WHERE id = @Id;";

            var result = await connection.ExecuteAsync(query, new { produto.Id });

            await connection.CloseAsync();
            await connection.DisposeAsync();

            if (result == 0)
                return BadRequest(new { mensagem = "Erro ao excluir produto" });

            return Ok(new { mensagem = "Produto excluído com sucesso" });
        }

    }
}