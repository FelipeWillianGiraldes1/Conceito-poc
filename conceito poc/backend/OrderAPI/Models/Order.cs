
using System;

namespace OrderAPI.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        public string Cliente { get; set; } = string.Empty;
        public string Produto { get; set; } = string.Empty;
        public decimal Valor { get; set; }
        public string Status { get; set; } = "Pendente";
        public DateTime DataCriacao { get; set; } = DateTime.UtcNow;
    }
}
