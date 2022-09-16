using Foody.Services.OrderApi.Models;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.OrderApi.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {

        }

        public DbSet<Order> Orders { get; set; }
        
        public DbSet<OrderDetail> OrderDetails { get; set; }

    }
}
