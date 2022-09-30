using Core.Interfaces;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Entities.OrderAggregate;
using Core.Entities;
using System.Linq;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService:IOrderService
    {
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        public OrderService(IUnitOfWork unitOfWork,IBasketRepository basketRepo)
        {
            _unitOfWork=unitOfWork;
            _basketRepo = basketRepo;

        }

        public async Task<Order> CreateOrderAsync(string buyerEmail,int deliveryMethodId,string basketId,Address shippingAddress)
        {
            //get basket from repo
            var basket = await _basketRepo.GetBasketAsync(basketId);
            //get items from product repo
            var items = new List<OrderItem>();
            foreach(var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered,productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            //get deliver method from repo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            //cal subtotal
            var subtotal = items.Sum(item => item.Price * item.Quantity);
            //create order
            var order = new Order(items, buyerEmail, shippingAddress, deliveryMethod, subtotal);
            //save to db

            _unitOfWork.Repository<Order>().Add(order);
            var result=await _unitOfWork.Complete();

            if (result<= 0) return null;

            //delete basket
            await _basketRepo.DeleteBasketAsync(basketId);
            return order;

        }
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }
        public async Task<Order> GetOrderByIdAsync(int id,string buyerEmail)
        {
            var spec=new OrdersWithItemsAndOrderingSpecification(id,buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

        }
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec=new OrdersWithItemsAndOrderingSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
            
        }
        public async Task<IReadOnlyList<Order>> GetAllOrdersAsync()
        {
            var spec=new OrdersWithItemsAndOrderingSpecification(OrderStatus.Pending);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
        public async Task<Order> AcceptOrderAsync(int orderId)
        {
            var order= await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            order.Status=OrderStatus.Accepted;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();
            return order;
        }
         public async Task<Order> RejectOrderAsync(int orderId)
        {
            var order= await _unitOfWork.Repository<Order>().GetByIdAsync(orderId);
            order.Status=OrderStatus.Rejected;
            _unitOfWork.Repository<Order>().Update(order);
            await _unitOfWork.Complete();
            return order;
        }
    }
}