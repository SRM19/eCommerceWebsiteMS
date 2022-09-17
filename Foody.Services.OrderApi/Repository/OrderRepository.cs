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

        public async Task<OrderDto> AddOrdertoDatabase(OrderDto orderDto)
        {
            //create application dbcontext here
            try
            {
                await using var dbContext = new ApplicationDbContext(_dbContextOptions);
                Order newOrder = _mapper.Map<Order>(orderDto);
                dbContext.Orders.Add(newOrder);
                await dbContext.SaveChangesAsync();
                return _mapper.Map<OrderDto>(newOrder);
            }
            catch (Exception)
            {
                return null;
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
