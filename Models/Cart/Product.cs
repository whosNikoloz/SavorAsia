namespace SavorAsia.Models.Cart
{
    public class Product
    {
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
        
    }
}
