﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Order
{
    public class OrderCreatedViewModel
    {
        public int OrderId { get; set; }
        public string Error { get; set; }
        public bool IsSuccessfull { get; set; }
    }
}
