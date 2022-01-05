using FreeCourse.Services.FakePayment.Models;
using FreeCourse.Shared.ControllerBases;
using FreeCourse.Shared.Dtos;
using FreeCourse.Shared.Messages;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FakePayment.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FakePaymentController : CustomBaseController
    {
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public FakePaymentController(ISendEndpointProvider sendEndpointProvider)
        {
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> ReceivePayment(PaymentDto paymentDto)
        {
            // paymentDto ile odeme islemi gerceklestir

            // Order islemi Suspend Order ile RabbitMQya gonderilmis ise
            if (paymentDto.Order != null)
            {
                // Order islemi Suspend Order ile RabbitMQya gonderilmis ise

                var sendEndpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("queue:create-order-service"));

                var createOderMessageCommand = new CreateOrderMessageCommand();
                createOderMessageCommand.BuyerId = paymentDto.Order.BuyerId;
                createOderMessageCommand.Province = paymentDto.Order.Address.Province;
                createOderMessageCommand.District = paymentDto.Order.Address.District;
                createOderMessageCommand.Street = paymentDto.Order.Address.Street;
                createOderMessageCommand.Line = paymentDto.Order.Address.Line;
                createOderMessageCommand.ZipCode = paymentDto.Order.Address.ZipCode;

                paymentDto.Order.OrderItems.ForEach(x =>
               {
                   createOderMessageCommand.OrderItems.Add(new OrderItem
                   {
                       PictureUrl = x.PictureUrl,
                       Price = x.Price,
                       ProductId = x.ProductId,
                       ProductName = x.ProductName
                   });
               });

                // Send RabbitMQ
                await sendEndpoint.Send<CreateOrderMessageCommand>(createOderMessageCommand);
            }

            return CreateActionResultInstance(FreeCourse.Shared.Dtos.Response<NoContent>.Success(200));
        }
    }
}
