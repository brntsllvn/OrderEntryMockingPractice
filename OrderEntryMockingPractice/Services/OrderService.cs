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
        private IEmailService _emailService;

        public OrderService(IOrderFulfillmentService orderFulfillmentService,
            ICustomerRepository customerRepository, ITaxRateService taxRateService,
            IEmailService emailService)
        {
            _orderFulfillmentService = orderFulfillmentService;
            _customerRepository = customerRepository;
            _taxRateService = taxRateService;
            _emailService = emailService;
        }

        public OrderSummary PlaceOrder(Order order)
        {
            ValidateOrder(order);

            var fulfillment = _orderFulfillmentService.Fulfill(order);
            var customer = _customerRepository.Get(order.CustomerId);

            var netTotal = CalculateNetTotal(order);

            List<TaxEntry> listOfTaxEntries = (List<TaxEntry>) _taxRateService.GetTaxEntries(
                customer.PostalCode,
                customer.Country);

            var orderTotal = CalculateOrderTotal(listOfTaxEntries, netTotal);

            var orderSummary = new OrderSummary()
            {
                OrderNumber = fulfillment.OrderNumber,
                OrderId = fulfillment.OrderId,
                Taxes = listOfTaxEntries,
                NetTotal = netTotal,
                Total = orderTotal
            };

            _emailService.SendOrderConfirmationEmail(order.CustomerId, orderSummary.OrderId);

            return orderSummary;
        }

        private static decimal CalculateOrderTotal(List<TaxEntry> listOfTaxEntries, decimal netTotal)
        {
            Decimal orderTotal = 0;
            foreach (var taxEntry in listOfTaxEntries)
            {
                orderTotal += netTotal*taxEntry.Rate;
            }
            return orderTotal;
        }

        private static decimal CalculateNetTotal(Order order)
        {
            Decimal netTotal = 0;
            foreach (var orderItem in order.OrderItems)
            {
                netTotal += orderItem.Quantity*orderItem.Product.Price;
            }
            return netTotal;
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
