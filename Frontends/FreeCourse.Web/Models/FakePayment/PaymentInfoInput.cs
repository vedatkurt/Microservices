﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.FakePayment
{
    public class PaymentInfoInput
    {
        public string CardName { get; set; }
        public string CardNumber  { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
        public string TotalPrice { get; set; }
    }
}
