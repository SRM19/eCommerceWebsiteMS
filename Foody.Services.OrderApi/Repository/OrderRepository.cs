using AutoMapper;
using Foody.Services.OrderApi.DbContexts;
using Foody.Services.OrderApi.Models;
using Foody.Services.OrderApi.Models.DataTransferObjs;
using Microsoft.EntityFrameworkCore;

namespace Foody.Services.OrderApi.Repository
{
    public class OrderRepository : IOrderRepository
    {
        //not using application dbcontext directly
        //register application db context as singleton object for order repository
        //singleton object is required for calling message bus
        private readonly DbContextOptions<ApplicationDbContext> _dbContextOptions;
        private IMapper _mapper;

        public OrderRepository(DbContextOptions<ApplicationDbContext> dbContextOptions, IMapper mapper)
        {
            _dbContextOptions = dbContextOptions;
            _mapper = mapper;
        }

        public async Task<bool> AddOrdertoDatabase(OrderDto orderDto)
        {
            //create application dbcontext here
            try
            {
                await using var dbContext = new ApplicationDbContext(_dbContextOptions);
                dbContext.Orders.Add(_mapper.Map<Order>(orderDto));
                await dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
            
        }

        public async Task UpdatePaymentStatus(int orderHeaderId, bool paid)
        {
            try
            {
                await using var dbContext = new ApplicationDbContext(_dbContextOptions);
                var orderFromDB = await dbContext.Orders.FirstOrDefaultAsync(o=> o.OrderHeaderId == orderHeaderId);
                if (orderFromDB != null)
                {
                    orderFromDB.PaymentStatus = paid;
                    dbContext.Orders.Update(orderFromDB);
                    await dbContext.SaveChangesAsync();
                }                
            }
            catch (Exception)
            {
            }
        }
    }
}
