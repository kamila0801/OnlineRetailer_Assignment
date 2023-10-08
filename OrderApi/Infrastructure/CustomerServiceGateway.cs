using System;
using RestSharp;
using SharedModels;

namespace OrderApi.Infrastructure
{
	public class CustomerServiceGateway : IServiceGateway<Customer>
	{
        string customerServiceBaseUrl;

        public CustomerServiceGateway(string baseUrl)
        {
            customerServiceBaseUrl = baseUrl;
        }

        public Customer Get(int id)
        {
            RestClient c = new RestClient(customerServiceBaseUrl);

            var request = new RestRequest(id.ToString());
            var response = c.GetAsync<Customer>(request);
            response.Wait();
            return response.Result;
        }
    }
}

