using Core.Entities.OrderAggregate;
using System;
using System.Linq.Expressions;

namespace Core.Specifications
{
	public class OrdersWithItemsAndOrderingSpecification: BaseSpecification<Order>
	{
        public OrdersWithItemsAndOrderingSpecification(OrderStatus status):base(o=>o.Status==status)
        {
            AddInclude(o => o.OrderItems);
            AddInclude(o => o.DeliveryMethod);
        }


        public OrdersWithItemsAndOrderingSpecification(string email):base(o=>o.BuyerEmail==email)
		{
			AddInclude(o => o.OrderItems);
			AddInclude(o => o.DeliveryMethod);
			AddOrderByDescending(o => o.OrderDate);
		}
		public OrdersWithItemsAndOrderingSpecification(int id,string email) : base(o => o.Id == id && o.BuyerEmail==email)
		{
			AddInclude(o => o.OrderItems);
			AddInclude(o => o.DeliveryMethod);
		}

	}
}