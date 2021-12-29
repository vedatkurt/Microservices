using FreeCourse.Web.Handler;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services;
using FreeCourse.Web.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Extensions
{
    public static class ServiceExtension
    {
        public static void AddHttpClientServices(this IServiceCollection services,IConfiguration configuration)
        {
            var serviceApiSettings = configuration.GetSection("ServiceApiSettings").Get<ServiceApiSettings>();

            services.AddHttpClient<IIdentityService, IdentityService>();

            // 119 Ders
            services.AddHttpClient<IUserService, UserService>(opt =>
            {
                opt.BaseAddress = new Uri(serviceApiSettings.IdentityBaseUri);
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            // 130. ders
            // Gateway ve Catalog path set ediliyor
            services.AddHttpClient<ICatalogService, CatalogService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Catalog.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            // 141. ders
            // Gateway ve Photostock path set ediliyor
            services.AddHttpClient<IPhotoStockService, PhotoStockService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.PhotoStock.Path}");
            }).AddHttpMessageHandler<ClientCredentialTokenHandler>();

            // 151. ders
            // Gateway ve Basket path set ediliyor
            services.AddHttpClient<IBasketService, BasketService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Basket.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();

            // 160. ders
            // Gateway ve Discount path set ediliyor
            services.AddHttpClient<IDiscountService, DiscountService>(opt =>
            {
                opt.BaseAddress = new Uri($"{serviceApiSettings.GatewayBaseUri}/{serviceApiSettings.Discount.Path}");
            }).AddHttpMessageHandler<ResourceOwnerPasswordTokenHandler>();
        }
    }
}