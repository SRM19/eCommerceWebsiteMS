using Foody.Services.Email.DbContexts;
using Foody.Services.Email.Messages;
using Foody.Services.Email.Models;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.Email.Repository
{
    public class EmailRepository : IEmailRepository
    {
        //not using application dbcontext directly
        //register application db context as singleton object for order repository
        //singleton object is required for calling message bus
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;

        public EmailRepository(DbContextOptions<ApplicationDbContext> dbContextOptions)
        {
            _dbContextOptions = dbContextOptions;
        }


        public async Task SendandLogEmail(PaymentStatusDto paymentStatusDto)
        {
            //implement logic to send email using an external library
            EmailLog newEmail = new()
            {
                Email = paymentStatusDto.UserEmail,
                EmailSent = DateTime.Now,
                Message = $"Order - {paymentStatusDto.OrderId} has been created successfully"
            };
            await using var dbContext = new ApplicationDbContext(_dbContextOptions);
            dbContext.EmailLogs.Add(newEmail);
            await dbContext.SaveChangesAsync();
        }

    }
}
