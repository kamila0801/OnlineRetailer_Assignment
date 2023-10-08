using System;
using System.ComponentModel;

namespace SharedModels
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime? Date { get; set; }
        public StatusEnums Status { get; set; } = StatusEnums.created;
        public int CustomerId { get; set; }
        public List<OrderLine> OrderLines { get; set; }
    }

    public class OrderLine
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    public enum StatusEnums
    {

        cancelled,
        created,
        shipped,
        paid,
    }

    public class OrderStatusChangedMessage
    {
        public int? CustomerId { get; set; }
        public IList<OrderLine> OrderLines { get; set; }
    }

    public class OrderPaidMessage
    {
        public int CustomerId { get; set; }
        public int Cost { get; set; }
    }
}
