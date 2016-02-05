using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public bool IsValid()
        {
            List<string> skuList = new List<string>();

            foreach (OrderItem orderItem in this.OrderItems)
            {
                skuList.Add(orderItem.Product.Sku);
            }

            if (skuList.Distinct().Count() == skuList.Count())
                return true;

            return false;
        }
    }
}
