using Foody.Services.OrderApi.Models.DataTransferObjs;

namespace Foody.Services.OrderApi.Repository
{
    public interface IOrderRepository
    {
        Task<bool> AddOrdertoDatabase(OrderDto order);

        Task UpdatePaymentStatus(int orderHeaderId, bool paid);

        //can also add get all order for user
        //get order by id
    }
}
