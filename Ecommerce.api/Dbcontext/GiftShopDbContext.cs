using ClassLibrary2.Model;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.api.Dbcontext
{
    public class GiftShopDbContext : DbContext
    {
        public GiftShopDbContext(DbContextOptions<GiftShopDbContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Product> Products { get; set; }

        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }



    }
}
