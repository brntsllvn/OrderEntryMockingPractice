using NUnit.Framework;
using OrderEntryMockingPractice.Models;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        [Test]
        public void OrderIsNotValidIdAnyProductsAreOutOfStock()
        {
            // Arrange
            var beerOrder = new Order();
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product { Sku = "1234" },
                Quantity = 2
            });
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product { Sku = "2345" },
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
                Product = new Product {Sku = "1234"},
                Quantity = 2
            });
            beerOrder.OrderItems.Add(new OrderItem
            {
                Product = new Product {Sku = "1234"},
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
        //    var lagunitas = new Product {Inventory = 3};

        //    var russianRiver = new Product {Inventory = 3};

        //    var favoriteIpa = new OrderItem
        //    {
        //        Product = lagunitas,
        //        Quantity = 2
        //    };

        //    var secondFavoriteIpa = new OrderItem
        //    {
        //        Product = russianRiver,
        //        Quantity = 2
        //    };

        //    var beerOrder = new Order();
        //    beerOrder.OrderItems.Add(favoriteIpa);
        //    beerOrder.OrderItems.Add(secondFavoriteIpa);

        //    // Act
        //    var orderValid = beerOrder.ProductsAreInStock();

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