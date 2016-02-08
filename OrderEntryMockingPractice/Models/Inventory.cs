using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPractice.Models
{
    public class Inventory
    {
        private readonly IProductRepository _productRepository;

        public Inventory(IProductRepository productRepository)
        {
            if (productRepository == null)
            {
                throw new ArgumentNullException("productRepository");
            }
            _productRepository = productRepository;
        }

        public bool IsInStock(Product product)
        {

            return _productRepository.IsInStock(product.Sku);

        }
    }
}
