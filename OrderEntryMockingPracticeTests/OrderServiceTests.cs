using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using OrderEntryMockingPractice.Models;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private IProductRepository _productRepo;
        private IOrderFulfillmentService _orderFulfillmentService;
        private ICustomerRepository _customerRepository;
        private ITaxRateService _taxRateService;

        private OrderService _orderService;
        private Customer _bestCustomer;
        private List<TaxEntry> _listOfTaxEntries;
        private OrderConfirmation _orderConfirmation;

        [SetUp]
        public void BeforeEach()
        {
            _productRepo = Substitute.For<IProductRepository>();
            _orderFulfillmentService = Substitute.For<IOrderFulfillmentService>();
            _customerRepository = Substitute.For<ICustomerRepository>();
            _taxRateService = Substitute.For<ITaxRateService>();

            _orderService = new OrderService(_orderFulfillmentService,
                _customerRepository,
                _taxRateService);

            _bestCustomer = new Customer
            {
                CustomerId = 42,
                PostalCode = "12345",
                Country = "Merica"
            };

            _listOfTaxEntries = new List<TaxEntry>
            {
                new TaxEntry {Description = "High Tax", Rate = (decimal) 0.60},
                new TaxEntry {Description = "Low Tax", Rate = (decimal) 0.10}
            };

            _orderConfirmation = new OrderConfirmation
            {
                OrderId = 1234,
                OrderNumber = "hello"
            };
        }

        [Test]
        public void PlaceOrderDoesNotReturnNull()
        {
            // Arrange
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);
            _orderFulfillmentService.Fulfill(Arg.Any<Order>()).Returns(new OrderConfirmation());

            var beerOrder = GetOrderFromSkus("1234", "4321");
            beerOrder.CustomerId = 42;
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
            _orderFulfillmentService.Fulfill(beerOrder).Returns(_orderConfirmation);

            // Act
            var result = _orderService.PlaceOrder(beerOrder);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PlaceOrderReturnsOrderSummaryIfAllProductsAreInStock()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            beerOrder.CustomerId = 42;
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
            _orderFulfillmentService.Fulfill(beerOrder).Returns(new OrderConfirmation());

            // Act
            var result = _orderService.PlaceOrder(beerOrder);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PlaceOrderReturnsOrderSummaryIfOrderItemsAreUniqueBySku()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            beerOrder.CustomerId = 42;
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
            _orderFulfillmentService.Fulfill(beerOrder).Returns(new OrderConfirmation());

            // Act
            var result = _orderService.PlaceOrder(beerOrder);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PlaceOrderThrowsIfOrderItemsAreNotUniqueBySku()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "1234");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _orderService.PlaceOrder(beerOrder));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(1));

            var reason = e.Reasons.Single();
            Assert.That(reason, Is.EqualTo("Order skus are not unique"));
        }

        [Test]
        public void PlaceOrderThrowsIfProductsAreNotInStock()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(false);

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _orderService.PlaceOrder(beerOrder));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(1));

            var reason = e.Reasons.Single();
            Assert.That(reason, Is.EqualTo("Some products are not in stock"));
        }

        [TestCase("Some products are not in stock")]
        [TestCase("Order skus are not unique")]
        public void PlaceOrderThrowsIfProductsAreNotInStockAndNotUnique(string reason)
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "1234");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(false);

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _orderService.PlaceOrder(beerOrder));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(2));
            Assert.That(e.Reasons.Any(r => r.Equals(reason)));
        }



        [Test]
        public void OrderSummaryIsReturnedWithApplicableTaxes()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            beerOrder.CustomerId = 42;
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
            _orderFulfillmentService.Fulfill(beerOrder).Returns(_orderConfirmation);

            // Act
            var orderSummary = _orderService.PlaceOrder(beerOrder);

            // Assert
            Assert.That(orderSummary.Taxes, Is.EquivalentTo(_listOfTaxEntries));
        }

        [Test]
        public void PlaceOrderThrowsListOfReasonsIfOrderIsNull()
        {
            // Arrange

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _orderService.PlaceOrder(null));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(1));

            var reason = e.Reasons.Single();
            Assert.That(reason, Is.EqualTo("Order is null"));
        }

        [Test]
        public void ValidOrderIsSubmittedToTheOrderFulfillmentService()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            beerOrder.CustomerId = 42;
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
            _orderFulfillmentService.Fulfill(beerOrder).Returns(new OrderConfirmation());

            // Act
            _orderService.PlaceOrder(beerOrder);

            // Assert
            _orderFulfillmentService.Received().Fulfill(beerOrder);
        }

        [Test]
        public void ValidOrderIsSubmittedWithOrderFulfillmentConfirmNum()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            beerOrder.CustomerId = 42;
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
            _orderFulfillmentService.Fulfill(beerOrder).Returns(_orderConfirmation);

            // Act
            var orderSummary = _orderService.PlaceOrder(beerOrder);

            // Assert
            Assert.That(orderSummary.OrderNumber, Is.EqualTo(_orderConfirmation.OrderNumber));
            Assert.That(orderSummary.OrderId, Is.EqualTo(_orderConfirmation.OrderId));
        }

        private Order GetOrderFromSkus(params string[] skus)
        {
            var order = new Order();

            foreach (var sku in skus)
            {
                var product = new Product(_productRepo) { Sku = sku };
                var orderItem = new OrderItem { Product = product };
                order.OrderItems.Add(orderItem);
            }

            return order;
        }
    }
}