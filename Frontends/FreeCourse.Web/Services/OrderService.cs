using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Services;
using FreeCourse.Web.Models.FakePayment;
using FreeCourse.Web.Models.Order;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class OrderService : IOrderService
    {
        private readonly IBasketService _basketService;
        private readonly IPaymentService _paymentService;
        private readonly HttpClient _httpClient;
        private readonly ISharedIdentityService _sharedIdentityService;

        public OrderService(IBasketService basketService, IPaymentService paymentService, 
            HttpClient httpClient, ISharedIdentityService sharedIdentityService)
        {
            _basketService = basketService;
            _paymentService = paymentService;
            _httpClient = httpClient;
            _sharedIdentityService = sharedIdentityService;
        }

        public async Task<OrderCreatedViewModel> CreateOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();

            // Odeme ve siparis islemleri Senkron gerceklestiriliyor

            //-------------------------------
            // 1. Odeme islemi gerceklestirme
            var paymentInfoInput = new PaymentInfoInput() {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice
            };

            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderCreatedViewModel() { Error = "Payment could not be successed", IsSuccessfull = false };
            }

            //---------------------------
            // Odeme tamamlandiktan sonra
            //2. Siparis olusturma
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Line = checkoutInfoInput.Line,
                    Street = checkoutInfoInput.Street,
                    ZipCode = checkoutInfoInput.ZipCode
                }
            };

            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };

                orderCreateInput.OrderItems.Add(orderItem);
            });

            //---------------
            // Call Order API
            var response = await _httpClient.PostAsJsonAsync<OrderCreateInput>("order", orderCreateInput);

            if (!response.IsSuccessStatusCode)
            {
                return new OrderCreatedViewModel() { Error = "Order could not be created", IsSuccessfull=false};
            }

            var orderCreatedViewModel = await response.Content.ReadFromJsonAsync<Response<OrderCreatedViewModel>>();
            
            orderCreatedViewModel.Data.IsSuccessfull = true;

            // Clear Basket
            await _basketService.Delete();

            return orderCreatedViewModel.Data;
        }        

        public async Task<OrderSuspendViewModel> SuspendOrder(CheckoutInfoInput checkoutInfoInput)
        {
            var basket = await _basketService.Get();

            //---------------------
            // 1. Siparis olusturma
            var orderCreateInput = new OrderCreateInput()
            {
                BuyerId = _sharedIdentityService.GetUserId,
                Address = new AddressCreateInput
                {
                    Province = checkoutInfoInput.Province,
                    District = checkoutInfoInput.District,
                    Line = checkoutInfoInput.Line,
                    Street = checkoutInfoInput.Street,
                    ZipCode = checkoutInfoInput.ZipCode
                }
            };

            basket.BasketItems.ForEach(x =>
            {
                var orderItem = new OrderItemCreateInput { ProductId = x.CourseId, Price = x.GetCurrentPrice, PictureUrl = "", ProductName = x.CourseName };

                orderCreateInput.OrderItems.Add(orderItem);
            });

            //-------------------
            // 2. Odeme olusturma
            var paymentInfoInput = new PaymentInfoInput()
            {
                CardName = checkoutInfoInput.CardName,
                CardNumber = checkoutInfoInput.CardNumber,
                Expiration = checkoutInfoInput.Expiration,
                CVV = checkoutInfoInput.CVV,
                TotalPrice = basket.TotalPrice,

                Order = orderCreateInput
            };
            var responsePayment = await _paymentService.ReceivePayment(paymentInfoInput);
            if (!responsePayment)
            {
                return new OrderSuspendViewModel() { Error = "Payment could not be successed", IsSuccessfull = false };
            }

            // Clear Basket
            await _basketService.Delete();

            return new OrderSuspendViewModel() { IsSuccessfull = true};
        }

        public async Task<List<OrderViewModel>> GetOrder()
        {
            var response = await _httpClient.GetFromJsonAsync<Response<List<OrderViewModel>>>("order");
            return response.Data;
        }
    }
}
