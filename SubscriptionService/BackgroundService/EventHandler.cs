using Domain.Entities;
using Domain.Events;
using EasyNetQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SubscriptionService.Data;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SubscriptionService
{
    public class EventHandler : BackgroundService, IHostedService
    {
        private readonly IConfiguration config;
        private readonly IServiceScopeFactory factory;
        private IBus bus;

        public EventHandler(IConfiguration config, IServiceScopeFactory factory)
        {
            this.config = config;
            this.factory = factory;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            bus = RabbitHutch.CreateBus(Environment.GetEnvironmentVariable("RABBITCONNECTION") ?? config.GetSection("RabbitSettings").GetSection("Connection").Value);
            bus.Subscribe<OrderCreatedEvent>("SubscriptionGateway", ProccessSubscription);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
            }

            bus.Dispose();
        }

        private void ProccessSubscription(OrderCreatedEvent order)
        {
            using var scope = factory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            using var transaction = context.Database.BeginTransaction();

            try
            {
                context.Client.Add(new Client
                {
                    OrderId = order.Id,
                    Name = order.Subscription.Name,
                    SubscriptionId = order.Subscription.Id,
                    Address = order.Subscription.Address,
                    Domain = order.Subscription.Domain,
                    IntegrationApiKey = new Random().Next(1, 999999)
                });

                context.SaveChanges();

                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
            }
        }
    }
}
