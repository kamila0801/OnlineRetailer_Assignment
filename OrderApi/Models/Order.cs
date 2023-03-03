using System;
namespace OrderApi.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public StatusEnums Status { get; set; } = StatusEnums.created;
    }
}
