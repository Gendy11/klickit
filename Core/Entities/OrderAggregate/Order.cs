﻿using System.Collections.Generic;
using System;

namespace Core.Entities.OrderAggregate
{
    public class Order:BaseEntity
    {

        public Order()
        {

        }
        public Order(IReadOnlyList<OrderItem> orderItems,string buyerEmail,Address shipToAddress,
            DeliveryMethod deliveryMethod,decimal subtotal)
        {
            this.BuyerEmail = buyerEmail;
            this.ShipToAddress = shipToAddress;
            this.DeliveryMethod = deliveryMethod;
            this.OrderItems = orderItems;
            this.Subtotal = subtotal;
        }
        public string BuyerEmail { get; set; }
        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public Address ShipToAddress { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public IReadOnlyList<OrderItem> OrderItems { get; set; } 
        public decimal Subtotal { get; set; }
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal GetTotal()
        {
            return Subtotal + DeliveryMethod.Price;
        }

    }
}