using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using MoreLinq;

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

        //public Order Add(string productSku, int quantity)
        //{
        //    this.OrderItems.Add(new OrderItem()
        //    {
        //        Product = new Product()
        //        {
        //            Sku = productSku
        //        },
        //        Quantity = quantity,
        //    });

        //    return this;
        //}

        public bool OrderItemsAreUnique()
        {
            var skuList = this.OrderItems
                .Select(orderItem => orderItem.Product.Sku)
                .ToList()
                ;

            return skuList.Distinct().Count() == skuList.Count();
        }
    }
}
