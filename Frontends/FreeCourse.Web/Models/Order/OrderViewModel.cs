using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Models.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        //public Address Address { get; set; }
        public string BuyerId { get; set; }
        private List<OrderItemViewModel> OrderItems { get; set; }
    }
}
