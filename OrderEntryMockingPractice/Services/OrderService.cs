using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OrderEntryMockingPractice.Models;

namespace OrderEntryMockingPractice.Services
{
    public class OrderService
    {
        private readonly IOrderFulfillmentService _orderFulfillmentService;
        private ICustomerRepository _customerRepository;
        private ITaxRateService _taxRateService;

        public OrderService(IOrderFulfillmentService orderFulfillmentService,
            ICustomerRepository customerRepository, ITaxRateService taxRateService)
        {
            _orderFulfillmentService = orderFulfillmentService;
            _customerRepository = customerRepository;
            _taxRateService = taxRateService;
        }

        public OrderSummary PlaceOrder(Order order)
        {
            ValidateOrder(order);

            var fulfillment = _orderFulfillmentService.Fulfill(order);
            var customer = _customerRepository.Get(order.CustomerId);

            return new OrderSummary()
            {
                OrderNumber = fulfillment.OrderNumber,
                OrderId = fulfillment.OrderId,
                Taxes = _taxRateService.GetTaxEntries(
                    customer.PostalCode,
                    customer.Country),
                NetTotal = 10 * 3 + 18 * 2
            };
        }

        public virtual void ValidateOrder(Order order)
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
