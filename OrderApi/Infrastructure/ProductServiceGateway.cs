using System;
using SharedModels;
using RestSharp;

namespace OrderApi.Infrastructure
{
	public class ProductServiceGateway : IServiceGateway<Product>
	{
        string productServiceBaseUrl;

        public ProductServiceGateway(string baseUrl)
        {
            productServiceBaseUrl = baseUrl;
        }

        public Product Get(int id)
        {
            RestClient c = new RestClient(productServiceBaseUrl);

            var request = new RestRequest(id.ToString());
            var response = c.GetAsync<Product>(request);
            response.Wait();
            return response.Result;
        }
    }
}

