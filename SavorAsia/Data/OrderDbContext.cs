using Microsoft.EntityFrameworkCore;
using SavorAsia.Models.Order;

namespace SavorAsia.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
        {
        }

        public DbSet<OrderModel> Orders { get; set; }

        
    }
}
