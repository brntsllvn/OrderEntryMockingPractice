using System;
using NUnit.Framework;
using OrderEntryMockingPractice.Models;

namespace OrderEntryMockingPracticeTests
{
    [TestFixture]
    public class OrderServiceTests
    {
        [Test]
        public void OrderIsValidIfOrdersAreUniqueByProductSku()
        {
            // Arrange
            Product lagunitas = new Product();
            lagunitas.Sku = "1234";

            Product russianRiver = new Product();
            russianRiver.Sku = "4321";

            OrderItem favoriteIpa = new OrderItem();
            favoriteIpa.Product = lagunitas;
            favoriteIpa.Quantity = 2;

            OrderItem secondFavoriteIpa = new OrderItem();
            secondFavoriteIpa.Product = russianRiver;
            secondFavoriteIpa.Quantity = 5;

            Order beerOrder = new Order();
            beerOrder.OrderItems.Add(favoriteIpa);
            beerOrder.OrderItems.Add(secondFavoriteIpa);

            // Act
            Boolean orderValid = beerOrder.IsValid();

            // Assert
            Assert.That(orderValid, Is.True);
        }

        [Test]
        public void OrderIsNotValidIfOrdersAreNotUniqueByProductSku()
        {
            // Arrange
            Product lagunitas = new Product();
            lagunitas.Sku = "1234";

            Product russianRiver = new Product();
            russianRiver.Sku = "1234";

            OrderItem favoriteIpa = new OrderItem();
            favoriteIpa.Product = lagunitas;
            favoriteIpa.Quantity = 2;

            OrderItem secondFavoriteIpa = new OrderItem();
            secondFavoriteIpa.Product = russianRiver;
            secondFavoriteIpa.Quantity = 5;

            Order beerOrder = new Order();
            beerOrder.OrderItems.Add(favoriteIpa);
            beerOrder.OrderItems.Add(secondFavoriteIpa);

            // Act
            Boolean orderValid = beerOrder.IsValid();

            // Assert
            Assert.That(orderValid, Is.False);
        }
    }
}
