using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace dbsecrets.api.Models
{
    public class Produto
    {   
        public int Id { get; set; }
        
        public string? Codigo { get; set; }

        public string? Descricao { get; set; }
    }

    public class  Produtos : List<Produto> { }
}