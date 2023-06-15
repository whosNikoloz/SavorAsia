namespace SavorAsia.Models.Menu
{
    public class DrinkModel
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public byte[]? Data { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }
    }
}
