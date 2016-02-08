//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NSubstitute;
//using NUnit.Framework;
//using OrderEntryMockingPractice.Models;
//using OrderEntryMockingPractice.Services;

//namespace OrderEntryMockingPracticeTests
//{
//    [TestFixture]
//    public class InventoryTest
//    {
//        private Inventory _subjectUnderTest;
//        private IProductRepository _productRepo;

//        [SetUp]
//        public void BeforeEach()
//        {
//            _productRepo = Substitute.For<IProductRepository>();
//            _subjectUnderTest = new Inventory(_productRepo);
//        }

//        [Test]
//        public void ProductConstruction()
//        {
//            //Arrange


//            //Act
//            try
//            {
//                new Inventory(null);
//                Assert.Fail("Exception Expected");
//            }
//            catch (Exception e)
//            {
//                Assert.That(e, Is.TypeOf<ArgumentNullException>());
//            }

//            //Assert
//        }

//        [Test]
//        public void ProductIsNotInStock()
//        {
//            //Arrange
//            var product = new Product();
//            _productRepo.IsInStock(product.Sku).Returns(false);
            
//            //Act
//            var result = _subjectUnderTest.IsInStock(product);

//            //Assert
//            Assert.That(result, Is.False);
//        }

//        [Test]
//        public void ProductIsInStock()
//        {
//            //Arrange
//            var product = new Product();
//            _productRepo.IsInStock(product.Sku).Returns(true);

//            //Act
//            var result = _subjectUnderTest.IsInStock(product);

//            //Assert
//            Assert.That(result, Is.True);
//        }
//    }
//}
