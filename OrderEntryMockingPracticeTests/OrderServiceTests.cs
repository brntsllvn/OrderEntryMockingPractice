using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using NUnit.Framework;
using OrderEntryMockingPractice.Models;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        private OrderService _orderService;
        private IProductRepository _productRepo;

        [SetUp]
        public void BeforeEach()
        {
            _productRepo = Substitute.For<IProductRepository>();
            _orderService = new OrderService();
        }

        [Test]
        public void PlaceOrderDoesNotReturnNull()
        {
            // Arrange
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            // Act
            var result = _orderService.PlaceOrder(new Order());

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PlaceOrderThrowsListOfReasonsIfOrderIsNull()
        {
            // Arrange

            // Act
            try
            {
                _orderService.PlaceOrder(null);
                Assert.Fail("Exception Expected");
            }
            catch (InvalidOrderException e)
            {
                // Assert
                Assert.That(e.Reasons, Has.Count.EqualTo(1));

                var reason = e.Reasons.Single();
                Assert.That(reason, Is.EqualTo("Order is null"));

            }
        }

        [Test]
        public void PlaceOrderReturnsOrderSummaryIfOrderItemsAreUniqueBySku()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

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
            try
            {
                _orderService.PlaceOrder(beerOrder);
                Assert.Fail("Exception Expected");
            }
            catch (InvalidOrderException e)
            {
                // Assert
                Assert.That(e.Reasons, Has.Count.EqualTo(1));

                var reason = e.Reasons.Single();
                Assert.That(reason, Is.EqualTo("Order skus are not unique"));

            }
        }

        [Test]
        public void PlacePrderReturnsOrderSummaryIfAllProductsAreInStock()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(true);

            // Act

            var result = _orderService.PlaceOrder(beerOrder);

            // Assert
            Assert.That(result, Is.Not.Null);
        }

        [Test]
        public void PlaceOrderThrowsIfProductsAreNotInStock()
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "4321");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(false);

            // Act
            try
            {
                _orderService.PlaceOrder(beerOrder);
                Assert.Fail("Exception Expected");
            }
            catch (InvalidOrderException e)
            {
                // Assert
                Assert.That(e.Reasons, Has.Count.EqualTo(1));

                var reason = e.Reasons.Single();
                Assert.That(reason, Is.EqualTo("Some products are not in stock"));

            }
        }

        [TestCase("Some products are not in stock")]
        [TestCase("Order skus are not unique")]
        public void PlaceOrderThrowsIfProductsAreNotInStockAndNotUnique(string reason)
        {
            // Arrange
            var beerOrder = GetOrderFromSkus("1234", "1234");
            _productRepo.IsInStock(Arg.Any<string>()).Returns(false);

            // Act
            try
            {
                _orderService.PlaceOrder(beerOrder);
                Assert.Fail("Exception Expected");
            }
            catch (InvalidOrderException e)
            {
                // Assert
                Assert.That(e.Reasons, Has.Count.EqualTo(2));
                Assert.That(e.Reasons.Any(r => r.Equals(reason)));
            }
        }




        private Order GetOrderFromSkus(params string[] skus)
        {
            var order = new Order();

            foreach (var sku in skus)
            {
                var product = new Product(_productRepo) {Sku = sku};
                var orderItem = new OrderItem {Product = product};
                order.OrderItems.Add(orderItem);
            }

            return order;
        }
    }
}
