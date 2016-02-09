using System.Collections.Generic;
using System.Linq;
using System.Security;
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
        private IEmailService _emailService;

        private OrderService _subject;
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
            _emailService = Substitute.For<IEmailService>();

            _subject = new OrderService(_orderFulfillmentService,
                _customerRepository,
                _taxRateService,
                _emailService);

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
            _customerRepository.Get(_bestCustomer.CustomerId.Value).Returns(_bestCustomer);
            _taxRateService.GetTaxEntries(_bestCustomer.PostalCode, _bestCustomer.Country).Returns(_listOfTaxEntries);
        }

        [Test]
        public void PlaceOrderDoesNotReturnNull()
        {
            // Arrange

            var order = GetOrderFrom("1234", "4321");
            AllProductsAreInStock();
            _orderFulfillmentService.Fulfill(order).Returns(_orderConfirmation);

            // Act
            var result = _subject.PlaceOrder(order);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        private void AllProductsAreInStock()
        {
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);
        }

        [Test]
        public void PlaceOrderReturnsOrderSummaryIfAllProductsAreInStock()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();
 
            // Act
            var result = _subject.PlaceOrder(order);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PlaceOrderReturnsOrderSummaryIfOrderItemsAreUniqueBySku()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();

            // Act
            var result = _subject.PlaceOrder(order);

            // Assert
            Assert.That(result, Is.Not.Null);
        }


        [Test]
        public void PlaceOrderThrowsIfOrderItemsAreNotUniqueBySku()
        {
            // Arrange
            var order = GetOrderFrom("1234", "1234");
            AllProductsAreInStock();

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _subject.PlaceOrder(order));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(1));

            var reason = e.Reasons.Single();
            Assert.That(reason, Is.EqualTo("Order skus are not unique"));
        }

        [Test]
        public void PlaceOrderThrowsIfProductsAreNotInStock()
        {
            // Arrange
            var order = GetOrderFrom("1234", "4321");
            AllProductsAreNotInStock();

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _subject.PlaceOrder(order));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(1));

            var reason = e.Reasons.Single();
            Assert.That(reason, Is.EqualTo("Some products are not in stock"));
        }

        private void AllProductsAreNotInStock()
        {
            _productRepo.IsInStock(Arg.Any<string>()).Returns(false);
        }

        [TestCase("Some products are not in stock")]
        [TestCase("Order skus are not unique")]
        public void PlaceOrderThrowsIfProductsAreNotInStockAndNotUnique(string reason)
        {
            // Arrange
            var order = GetOrderFrom("1234", "1234");
            AllProductsAreNotInStock();

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _subject.PlaceOrder(order));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(2));
            Assert.That(e.Reasons.Any(r => r.Equals(reason)));
        }

        [Test]
        public void OrderSummaryIsReturnedWithApplicableTaxes()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();
            // Act
            var orderSummary = _subject.PlaceOrder(order);

            // Assert
            Assert.That(orderSummary.Taxes, Is.EquivalentTo(_listOfTaxEntries));
        }

        [Test]
        public void PlaceOrderThrowsListOfReasonsIfOrderIsNull()
        {
            // Arrange

            // Act
            var e = Assert.Throws<InvalidOrderException>(() =>
                _subject.PlaceOrder(null));

            // Assert
            Assert.That(e.Reasons, Has.Count.EqualTo(1));

            var reason = e.Reasons.Single();
            Assert.That(reason, Is.EqualTo("Order is null"));
        }

        [Test]
        public void ValidOrderIsSubmittedToTheOrderFulfillmentService()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();

            // Act
            _subject.PlaceOrder(order);

            // Assert
            _orderFulfillmentService.Received().Fulfill(order);
        }

        [Test]
        public void ValidOrderIsSubmittedWithOrderFulfillmentConfirmNum()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();

            // Act
            var orderSummary = _subject.PlaceOrder(order);

            // Assert
            Assert.That(orderSummary.OrderNumber, Is.EqualTo(_orderConfirmation.OrderNumber));
            Assert.That(orderSummary.OrderId, Is.EqualTo(_orderConfirmation.OrderId));
        }

        [Test]
        public void PlaceOrderCalculatesAndReturnsNetTotal()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();

            // Act
            var result = _subject.PlaceOrder(order);

            // Assert
            Assert.That(result.NetTotal, Is.EqualTo(17*18*order.OrderItems.Count));
        }

        [Test]
        public void PlaceOrderCalculatesAndReturnsOrderTotal()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();

            // Act
            var result = _subject.PlaceOrder(order);

            // Assert
            Assert.That(result.Total, Is.InRange(428.39999,428.40)); //428.4
        }

        [Test]
        public void PlaceOrderSendsConfirmationEmailToCustomer()
        {
            // Arrange
            var order = GetValidOrderWithTwoItems();

            // Act
            var result = _subject.PlaceOrder(order);

            // Assert
            _emailService.Received().SendOrderConfirmationEmail(order.CustomerId, result.OrderId);
        }


        private Order GetValidOrderWithTwoItems()
        {
            var order = GetOrderFrom("1234", "4321");
            AllProductsAreInStock();
            _orderFulfillmentService.Fulfill(order).Returns(_orderConfirmation);
            return order;
        }

        private Order GivenAValidOrderWith(params Product[] products)
        {
            var order = GetOrderFrom(products);
            AllProductsAreInStock();
            _orderFulfillmentService.Fulfill(order).Returns(_orderConfirmation);
            return order;
        }


        private Order GetOrderFrom(params string[] skus)
        {
            var products = skus.Select(row => new Product(_productRepo)
            {
                Sku = row,
                Price = 18
            }).ToArray()
                ;

            return GetOrderFrom(products);

        }

        private Order GetOrderFrom(params Product[] products)
        {
            var order = new Order();

            foreach (var product in products)
            {
                var orderItem = new OrderItem { Product = product, Quantity = 17 };
                order.OrderItems.Add(orderItem);
            }

            order.CustomerId = _bestCustomer.CustomerId.Value;

            return order;
        }
    }
}