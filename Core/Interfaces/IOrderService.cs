using System.Threading.Tasks;
using Core.Entities.OrderAggregate;
using System.Collections.Generic;

namespace Core.Interfaces
{
    public interface IOrderService
    {
        Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethod, string basketId, Address shippingAddress);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail);
        Task<Order> GetOrderByIdAsync(int id, string buyerEmail);
        Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync();
        Task<IReadOnlyList<Order>> GetAllOrdersAsync();
        Task<Order> AcceptOrderAsync(int id);
        Task<Order> RejectOrderAsync(int id);
    }
}