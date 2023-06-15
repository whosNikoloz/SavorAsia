namespace SavorAsia.Models.Menu
{
    public class NoodlesModel 
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string? Name { get; set; }

        public string?   Description { get; set; }

        public byte[]? Data { get; set; }

        public decimal Price { get; set; }

    }
}
