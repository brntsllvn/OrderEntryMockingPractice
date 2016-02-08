using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using MoreLinq;
using OrderEntryMockingPractice.Services;

namespace OrderEntryMockingPractice.Models
{
    public class Order
    {
        public Order()
        {
            this.OrderItems = new List<OrderItem>();
        }
        
        public int? CustomerId { get; set; }

        public List<OrderItem> OrderItems { get; set; }

        public bool OrderItemsAreUnique()
        {
            var skuList = this.OrderItems
                .Select(orderItem => orderItem.Product.Sku)
                .ToList()
                ;

            return skuList.Distinct().Count() == skuList.Count();
        }

        public bool AllProductsAreInStock()
        {
            return this.OrderItems.All(orderItem => orderItem.IsInStock());
        }
    }
}
