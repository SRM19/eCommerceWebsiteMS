using Foody.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.Email.DbContexts
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options): base(options)
        {

        }

        public DbSet<EmailLog> EmailLogs { get; set; }
    }
}
