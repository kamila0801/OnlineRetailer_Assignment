using System;
using EasyNetQ;
using SharedModels;

namespace OrderApi.Infrastructure
{
	public class MessagePublisher : IMessagePublisher, IDisposable
    {
        IBus bus;

        public MessagePublisher(string connectionString)
        {
            bus = RabbitHutch.CreateBus(connectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public void PublishOrderPaidMessage(int customerId, int price, string topic)
        {
            var message = new OrderPaidMessage
            {
                Cost = price,
                CustomerId = customerId
            };

            bus.PubSub.Publish(message, topic);
        }

        public void PublishOrderStatusChangedMessage(int? customerId, IList<OrderLine> orderLines, string topic)
        {
            var message = new OrderStatusChangedMessage
            {
                CustomerId = customerId,
                OrderLines = orderLines
            };

            bus.PubSub.Publish(message, topic);
        }
    }
}

