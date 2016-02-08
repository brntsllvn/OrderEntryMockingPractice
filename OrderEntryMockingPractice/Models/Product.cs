using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPractice.Models
{
    public class Product
    {
        private readonly IProductRepository _productRepository;

        public Product(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

    public int? ProductId { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }

        public bool IsInStock()
        {
            return _productRepository.IsInStock(this.Sku);
        }
    }
}