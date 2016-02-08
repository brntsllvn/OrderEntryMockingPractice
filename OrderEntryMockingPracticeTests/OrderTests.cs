using NSubstitute;
using NUnit.Framework;
using OrderEntryMockingPractice.Models;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderTests
    {
        private IProductRepository _productRepo;

        [SetUp]
        public void BeforeEach()
        {
            _productRepo = Substitute.For<IProductRepository>();
        }

        [Test]
        public void OrderIsNotValidIdAnyProductsAreOutOfStock()
        {
            // Arrange
            var beerOrder = new Order();
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product(_productRepo) { Sku = "1234" },
                Quantity = 2
            });
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product(_productRepo) { Sku = "2345" },
                Quantity = 5
            });

            // Act
            var orderValid = beerOrder.OrderItemsAreUnique();

            // Assert
            Assert.That(orderValid, Is.True);
        }

        [Test]
        public void OrderIsNotValidIfOrdersAreNotUniqueByProductSku()
        {
            // Arrange
            var beerOrder = new Order();
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product(_productRepo) { Sku = "1234"},
                Quantity = 2
            });
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product(_productRepo) { Sku = "1234"},
                Quantity = 5
            });

            // Act
            var orderValid = beerOrder.OrderItemsAreUnique();

            // Assert
            Assert.That(orderValid, Is.False);
        }

        //[Test]
        //public void OrderIsValidIfAllProductsAreInStock()
        //{
        //    // Arrange
        //    var beerOrder = new Order();
        //    beerOrder.OrderItems.Add(new OrderItem
        //    {
        //        Product = new Product(),
        //        Quantity = 2
        //    });
        //    beerOrder.OrderItems.Add(new OrderItem
        //    {
        //        Product = new Product(),
        //        Quantity = 5
        //    });

        //    // Act
        //    var orderValid = beerOrder.AllProductsAreInStock();

        //    // Assert
        //    Assert.That(orderValid, Is.True);
        //}

        //[Test]
        //public void OrderIsValidIfOrdersAreUniqueByProductSku()
        //{
        //    // Arrange
        //    var lagunitas = new Product {Sku = "1234"};

        //    var russianRiver = new Product {Sku = "4321"};

        //    var favoriteIpa = new OrderItem
        //    {
        //        Product = lagunitas,
        //        Quantity = 2
        //    };

        //    var secondFavoriteIpa = new OrderItem
        //    {
        //        Product = russianRiver,
        //        Quantity = 5
        //    };

        //    var beerOrder = new Order();

        //    beerOrder.OrderItems.Add(favoriteIpa);
        //    beerOrder.OrderItems.Add(secondFavoriteIpa);

        //    // Act
        //    var orderValid = beerOrder.OrderItemsAreUnique();

        //    // Assert
        //    Assert.That(orderValid, Is.True);
        //}
    }
}