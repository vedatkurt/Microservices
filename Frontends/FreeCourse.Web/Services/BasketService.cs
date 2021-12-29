using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Basket;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class BasketService : IBasketService
    {
        private readonly HttpClient _httpClient;
        private readonly IDiscountService _discountService;


        // DI container done
        public BasketService(HttpClient httpClient, IDiscountService discountService)
        {
            _httpClient = httpClient;
            _discountService = discountService;
        }

        public async Task AddBasketItem(BasketItemViewModel basketItemViewModel)
        {
            var basket = await Get();

            if (basket != null)
            {
                if (!basket.BasketItems.Any(x => x.CourseId == basketItemViewModel.CourseId))
                {
                    basket.BasketItems.Add(basketItemViewModel);
                }
            }
            else
            {
                basket = new BasketViewModel();
                basket.BasketItems.Add(basketItemViewModel);
            }

            await SaveOrUpdate(basket);
        }

        public async Task<bool> ApplyDiscount(string discountCode)
        {
            await CancelApplyDiscount();

            var basket = await Get();

            if (basket == null)
            {
                return false;
            }

            var hasDiscount = await _discountService.GetDiscount(discountCode);

            if (hasDiscount == null)
            {
                return false;
            }

            basket.ApplyDiscount(hasDiscount.Code, hasDiscount.Rate);

            await SaveOrUpdate(basket);

            return true;
        }

        public async Task<bool> CancelApplyDiscount()
        {
            var basket = await Get();

            if (basket == null && basket.DiscountCode==null)
            {
                return false;
            }


            basket.CancelDiscount();

            await SaveOrUpdate(basket);

            return true;
        }

        public async Task<bool> Delete()
        {
            var result = await _httpClient.DeleteAsync("basket");

            return result.IsSuccessStatusCode;
        }

        public async Task<bool> RemoveBasketItem(string courseId)
        {
            // Basket alinir
            var basket = await Get();
            if (basket == null)
            {
                return false;
            }

            // Basket icinde ilgili course alinir
            var deleteBasketItem = basket.BasketItems.FirstOrDefault(x => x.CourseId == courseId);
            if (deleteBasketItem == null)
            {
                return false;
            }
            
            // Basket icindeki course siliinir
            var deleteResult = basket.BasketItems.Remove(deleteBasketItem);

            if (!deleteResult)
            {
                return false;
            }

            // Basketteki tum itemlar bitti ise discount null yapilir
            if (!basket.BasketItems.Any())
            {
                basket.DiscountCode = null;
            }

            return await SaveOrUpdate(basket);
        }

        public async Task<bool> SaveOrUpdate(BasketViewModel basketViewModel)
        {
            var response = await _httpClient.PostAsJsonAsync("basket", basketViewModel);

            return response.IsSuccessStatusCode;
        }

        public async Task<BasketViewModel> Get()
        {
            var response = await _httpClient.GetAsync("basket");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var basketViewModel = await response.Content.ReadFromJsonAsync<Response<BasketViewModel>>();
            return basketViewModel.Data;
        }
    }
}
