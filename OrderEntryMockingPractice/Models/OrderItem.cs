﻿using System;
using System.Collections.Generic;

namespace OrderEntryMockingPractice.Models
{
    public class OrderItem
    {
        public Product Product { get; set; }
        public decimal Quantity { get; set; }

        public bool IsInStock()
        {
            var inStock = Product.IsInStock();
            return inStock;
        }
    }
}
