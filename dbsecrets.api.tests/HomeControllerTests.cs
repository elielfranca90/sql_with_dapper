using System.Net.Http.Json;
using dbsecrets.api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dapper;
using Npgsql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace dbsecrets.api.tests
{
    [TestClass]
    public class HomeControllerTests
    {
        private static WebApplicationFactory<Program> _factory = default!;
        private static HttpClient _client = default!;
        private static string _connectionString = default!;
        private List<int> _insertedIds = new List<int>();

        [ClassInitialize]
        public static void ClassInit(TestContext context)
        {
            _factory = new WebApplicationFactory<Program>();
            _client = _factory.CreateClient();

            using (var scope = _factory.Services.CreateScope())
            {
                var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
                _connectionString = config.GetConnectionString("DefaultConnection");
            }
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _client.Dispose();
            _factory.Dispose();
        }

        [TestCleanup]
        public async Task Cleanup()
        {
            if (_insertedIds.Count > 0)
            {
                using var connection = new NpgsqlConnection(_connectionString);
                await connection.OpenAsync();
                await connection.ExecuteAsync("DELETE FROM produto WHERE id = ANY(@ids)", new { ids = _insertedIds.ToArray() });
                _insertedIds.Clear();
            }
        }

        [TestMethod]
        public async Task Get_ReturnsWelcomeMessage()
        {
            var response = await _client.GetAsync("/Home");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            Assert.AreEqual("Welcome to the dbsecrets.api!", content);
        }

        [TestMethod]
        public async Task GetProdutos_ReturnsProductList()
        {
            var response = await _client.GetAsync("/Home/produtos");
            response.EnsureSuccessStatusCode();
            var produtos = await response.Content.ReadFromJsonAsync<List<Produto>>();
            Assert.IsNotNull(produtos);
            Assert.IsTrue(produtos.Count >= 0);
        }

        [TestMethod]
        public async Task PostProduto_CreatesAndReturnsProduto()
        {
            var newProduto = new Produto { Codigo = "TEST001", Descricao = "Produto de Teste" };
            var response = await _client.PostAsJsonAsync("/Home/produtos", newProduto);

            response.EnsureSuccessStatusCode();
            var createdProduto = await response.Content.ReadFromJsonAsync<Produto>();

            Assert.IsNotNull(createdProduto);
            Assert.AreEqual(newProduto.Codigo, createdProduto.Codigo);
            Assert.AreNotEqual(0, createdProduto.Id);

            _insertedIds.Add(createdProduto.Id);
        }

        [TestMethod]
        public async Task PostListaProdutos_CreatesMultipleProdutos()
        {
            var list = new List<Produto>
            {
                new Produto { Codigo = "LIST001", Descricao = "Desc 1" },
                new Produto { Codigo = "LIST002", Descricao = "Desc 2" }
            };

            var response = await _client.PostAsJsonAsync("/Home/lista-produtos", list);
            response.EnsureSuccessStatusCode();

            var createdProdutos = await response.Content.ReadFromJsonAsync<List<Produto>>();
            Assert.IsNotNull(createdProdutos);
            Assert.AreEqual(2, createdProdutos.Count);

            foreach (var p in createdProdutos)
            {
                _insertedIds.Add(p.Id);
            }
        }

        [TestMethod]
        public async Task PutProduto_UpdatesExistingProduto()
        {
            // First create one
            var newProduto = new Produto { Codigo = "PUT001", Descricao = "Original" };
            var createResponse = await _client.PostAsJsonAsync("/Home/produtos", newProduto);
            var created = await createResponse.Content.ReadFromJsonAsync<Produto>();
            Assert.IsNotNull(created);
            _insertedIds.Add(created.Id);

            // Update it
            created.Descricao = "Updated";
            var updateResponse = await _client.PutAsJsonAsync("/Home/produtos", created);
            updateResponse.EnsureSuccessStatusCode();

            // Verify
            var verifyResponse = await _client.GetAsync("/Home/produtos");
            var produtos = await verifyResponse.Content.ReadFromJsonAsync<List<Produto>>();
            Assert.IsNotNull(produtos);
            var updated = produtos.FirstOrDefault(p => p.Id == created.Id);

            Assert.IsNotNull(updated);
            Assert.AreEqual("Updated", updated.Descricao);
        }

        [TestMethod]
        public async Task DeleteProduto_RemovesProduto()
        {
            // First create one
            var newProduto = new Produto { Codigo = "DEL001", Descricao = "To Delete" };
            var createResponse = await _client.PostAsJsonAsync("/Home/produtos", newProduto);
            var created = await createResponse.Content.ReadFromJsonAsync<Produto>();
            Assert.IsNotNull(created);
            // Don't add to _insertedIds because we expect it to be deleted

            // Delete it
            var deleteResponse = await _client.SendAsync(new HttpRequestMessage(HttpMethod.Delete, "/Home/produtos")
            {
                Content = JsonContent.Create(created)
            });
            deleteResponse.EnsureSuccessStatusCode();

            // Verify
            var verifyResponse = await _client.GetAsync("/Home/produtos");
            var produtos = await verifyResponse.Content.ReadFromJsonAsync<List<Produto>>();
            Assert.IsNotNull(produtos);
            Assert.IsFalse(produtos.Any(p => p.Id == (created?.Id ?? 0)));
        }
    }
}
