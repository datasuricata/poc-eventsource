using Domain.Entities;
using Domain.Events;
using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using OrderService.Arguments;
using System;
using System.Threading.Tasks;

namespace OrderService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration config;
        private readonly IMongoCollection<Order> context;

        public OrderController(IConfiguration config)
        {
            this.config = config;

            var mongo = new MongoClient(this.config["MongoSettings:Connection"]);

            context = mongo.GetDatabase(this.config["MongoSettings:DatabaseName"]).GetCollection<Order>("Orders");
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] OrderRequest req)
        {
            var subscription = new Subscription
            {
                Name = req.Name,
                Address = req.Address,
                Domain = req.Domain,
            };

            subscription.Validate();

            var order = new Order { Subscription = subscription };

            await context.InsertOneAsync(order);

            using (var bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITCONNECTION") ?? config.GetSection("RabbitSettings").GetSection("Connection").Value))
            {
                bus.Publish(new OrderCreatedEvent(order.Id, order.Subscription));
            }

            return Ok();
        }
    }
}
