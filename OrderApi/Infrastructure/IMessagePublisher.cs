using System;
using SharedModels;

namespace OrderApi.Infrastructure
{
	public interface IMessagePublisher
	{
        void PublishOrderStatusChangedMessage(int? customerId, IList<OrderLine> orderLines, string topic);
        void PublishOrderPaidMessage(int customerId, int price, string topic);
    }
}

