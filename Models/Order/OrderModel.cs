using SavorAsia.Models.Menu;
using System.ComponentModel.DataAnnotations.Schema;

namespace SavorAsia.Models.Order
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string? UserId { get; set; }
        public string? CustomerName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public decimal OrderTotal { get; set; }
        public DateTime OrderDate { get; set; }
        public string?  OrderedItems { get; set; }

        public OrderModel()
        {
            OrderDate = DateTime.Now; // Initialize with current date and time
        }
    }
}
