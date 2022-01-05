using FreeCourse.Services.Basket.Dtos;
using FreeCourse.Services.Basket.Services;
using FreeCourse.Shared.Messages;
using FreeCourse.Shared.Services;
using MassTransit;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Services.Basket.Consumers
{
    public class CourseNameChangedEventConsumer : IConsumer<CourseNameChangedEvent>
    {
        //private readonly IBasketService _basketService;

        //public CourseNameChangedEventConsumer(IBasketService basketService)
        //{
        //    _basketService = basketService;
        //}
    
        private readonly RedisService _redisService;
        public CourseNameChangedEventConsumer(RedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task Consume(ConsumeContext<CourseNameChangedEvent> context)
        {
            //var existBasket = await _basketService.GetBasket("");

            //existBasket.Data.basketItems.ForEach(x =>
            //{
            //    x.UpdateBasketItem(context.Message.UpdatedName);
            //});

            //await _basketService.SaveOrUpdate(existBasket.Data);

            var keys = _redisService.GetKeys();

            if (keys != null)
            {
                foreach (var key in keys)
                {
                    var basket = await _redisService.GetDb().StringGetAsync(key);

                    var basketDto = JsonSerializer.Deserialize<BasketDto>(basket);

                    basketDto.basketItems.ForEach(x =>
                    {
                        if (x.CourseId == context.Message.CourseId)
                        {
                            x.UpdateBasketItem(context.Message.UpdatedName);
                        }
                    });

                    await _redisService.GetDb().StringSetAsync(key, JsonSerializer.Serialize(basketDto));
                }
            }
        }
    }
}
