using Microsoft.EntityFrameworkCore;
using SavorAsia.Models.Menu;

namespace SavorAsia.Data
{
    public class MenuDbContext : DbContext
    {

       public  DbSet<DrinkModel> Drinks {  get; set; }
       public  DbSet<RiceModel> Rice {  get; set; }
       public  DbSet<RamenModel> Ramen {  get; set; }
       public DbSet<NoodlesModel> Noodles {  get; set; }


        public MenuDbContext(DbContextOptions<MenuDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DrinkModel>()
                .Property(d => d.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<NoodlesModel>()
                .Property(n => n.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RamenModel>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18, 2)");

            modelBuilder.Entity<RiceModel>()
                .Property(r => r.Price)
                .HasColumnType("decimal(18, 2)");


            base.OnModelCreating(modelBuilder);
        }

    }
}
