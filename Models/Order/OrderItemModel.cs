namespace SavorAsia.Models.Order
{
    public class OrderItemModel
    {
        public int ProductId { get; set; }
        public int CategoryId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

    }
}
