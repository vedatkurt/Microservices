using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IBasketService _basketService;
        private readonly IOrderService _orderService;

        public OrderController(IBasketService basketService, IOrderService orderService)
        {
            _basketService = basketService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Checkout()
        {
            // Basket icerisinde Checkout yapilinca
            // basket bilgisi ViewBag ile Checkout View icine aktariliyor.
            var basket = await _basketService.Get();
            ViewBag.basket = basket;

            return View(new CheckoutInfoInput());
        }

        [HttpPost]
        public async Task<IActionResult> Checkout(CheckoutInfoInput checkoutInfoInput)
        {
            // Checkout View icinde iken Checkout post edilir

            if (!checkoutInfoInput.IsSendToQueue)
            {
                // 1. yol Senkron iletisim
                // OrderService/CreateOrder icinde Odeme ve Siparis islemleri yapilir.

                var orderCreatedStatus = await _orderService.CreateOrder(checkoutInfoInput);

                if (!orderCreatedStatus.IsSuccessfull)
                {
                    var basket = await _basketService.Get();
                    ViewBag.basket = basket;

                    ViewBag.error = orderCreatedStatus.Error;

                    return View();
                }

                return RedirectToAction(nameof(SuccessfullCheckout), new { orderId = orderCreatedStatus.OrderId });
            }
            else
            {
                // 2. yol Asenkron iletisim
                var orderSuspendedStatus = await _orderService.SuspendOrder(checkoutInfoInput);

                if (!orderSuspendedStatus.IsSuccessfull)
                {
                    var basket = await _basketService.Get();
                    ViewBag.basket = basket;

                    ViewBag.error = orderSuspendedStatus.Error;

                    return View();
                }

                return RedirectToAction(nameof(SuccessfullCheckout), new { orderId = new Random().Next(1, 1000) });
            }
        }

        public IActionResult SuccessfullCheckout(int orderId)
        {
            ViewBag.orderId = orderId;
            return View();
        }

        public async Task<IActionResult> CheckoutHistory()
        {
           return View(await _orderService.GetOrder());
        }
    }
}
