using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models.Discount;
using FreeCourse.Web.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services
{
    public class DiscountService : IDiscountService
    {
        private readonly HttpClient _httpClient;

        public DiscountService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<DiscountViewModel> GetDiscount(string discountCode)
        {
            //[controller]/[action]/{code}
            var response = await _httpClient.GetAsync($"discount/GetByCode/{discountCode}");

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            var discountViewModel = await response.Content.ReadFromJsonAsync<Response<DiscountViewModel>>();
            return discountViewModel.Data;
        }
    }
}
