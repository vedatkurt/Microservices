﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Basket
{
    public class BasketItemViewModel
    {
        public int Quantity { get; set; } = 1;
        public string CourseId { get; set; }
        public string CourseName { get; set; }
        public decimal Price { get; set; }
        private decimal? DiscountAppliedPrice { get; set; }

        public decimal GetCurrentPrice { get => DiscountAppliedPrice != null ? DiscountAppliedPrice.Value : Price; }

        public void ApplyDiscount(decimal discountPrice)
        {
            DiscountAppliedPrice = discountPrice;
        }


    }
}