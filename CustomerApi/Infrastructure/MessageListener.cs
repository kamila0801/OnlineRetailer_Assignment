using System;
using CustomerApi.Data;
using EasyNetQ;
using SharedModels;

namespace CustomerApi.Infrastructure
{
	public class MessageListener
	{

        IServiceProvider provider;
        string connectionString;

        // The service provider is passed as a parameter, because the class needs
        // access to the product repository. With the service provider, we can create
        // a service scope that can provide an instance of the product repository.
        public MessageListener(IServiceProvider provider, string connectionString)
        {
            this.provider = provider;
            this.connectionString = connectionString;
        }

        public void Start()
        {
            using (var bus = RabbitHutch.CreateBus(connectionString))
            {
                bus.PubSub.Subscribe<OrderPaidMessage>("orderPaid",
                    HandleOrderPaid, x => x.WithTopic(StatusEnums.paid.ToString()));

                // Block the thread so that it will not exit and stop subscribing.
                lock (this)
                {
                    Monitor.Wait(this);
                }
            }

        }

        private void HandleOrderPaid(OrderPaidMessage message)
        {

            using (var scope = provider.CreateScope())
            {
                var services = scope.ServiceProvider;
                var repo = services.GetService<IRepository<Customer>>();

                var customer = repo.Get(message.CustomerId);
                customer.CreditStanding -= message.Cost;
                repo.Update(customer);

            }
        }
    }
}

