using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderEntryMockingPractice.Models;

namespace OrderEntryMockingPractice.Services
{
    public class OrderService
    {
        public OrderSummary PlaceOrder(Order order)
        {
            var reasons = new List<string>();
            if (order == null)
            {
                reasons.Add("Order is null");
                throw new InvalidOrderException(reasons);
            }

            if (!order.OrderItemsAreUnique())
            {
                reasons.Add("Order skus are not unique");

                if (!order.AllProductsAreInStock())
                {
                    reasons.Add("Some products are not in stock");
                }
                throw new InvalidOrderException(reasons);
            }

            if (!order.AllProductsAreInStock())
            {
                reasons.Add("Some products are not in stock");
                throw new InvalidOrderException(reasons);
            }
            return new OrderSummary();
        }
    }

    public class InvalidOrderException : Exception
    {
        public IList<string> Reasons { get; private set; }

        public InvalidOrderException(IEnumerable<string> reasons)
        {
            Reasons = reasons.ToList();
        }
    }
}
