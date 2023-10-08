using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Infrastructure;
using SharedModels;
using RestSharp;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> repository;
        private readonly IServiceGateway<Product> productServiceGateway;
        private readonly IServiceGateway<Customer> customerServiceGateway;
        private readonly IMessagePublisher messagePublisher;

        public OrdersController(
            IRepository<Order> repo,
            IServiceGateway<Product> productGateway,
            IServiceGateway<Customer> customerGateway,
            IMessagePublisher publisher)
        {
            repository = repo;
            productServiceGateway = productGateway;
            customerServiceGateway = customerGateway;
            messagePublisher = publisher;
        }

        // GET: all orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET order by id
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST create order
        [HttpPost]
        public IActionResult Post([FromBody]Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            var orderedProducts = GetProductsFromOrderLines(order.OrderLines);

            if (!ProductItemsAvailable(order, orderedProducts))
                return StatusCode(500, "Not enough items in stock.");
            else if (!CustomerHasEnoughCredits(order, orderedProducts))
                return StatusCode(500, "Customer does not have enough credits");
            else
            {
                try
                {
                    // Publish OrderStatusChangedMessage. If this operation
                    // fails, the order will not be created
                    messagePublisher.PublishOrderStatusChangedMessage(
                        order.CustomerId, order.OrderLines, StatusEnums.created.ToString());

                    // Create order
                    order.Status = StatusEnums.created;
                    var newOrder = repository.Add(order);
                    return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
                }
                catch
                {
                    return StatusCode(500, "An error happened. Try again.");
                }
            }
        }

        private bool ProductItemsAvailable(Order order, List<Product> products)
        {
            foreach (var orderLine in order.OrderLines)
            {
                var orderedProduct = products.First(p => p.Id == orderLine.ProductId);
                if (orderLine.Quantity > orderedProduct.ItemsInStock - orderedProduct.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }

        private bool CustomerHasEnoughCredits(Order order, List<Product> products)
        {
            var totalCost = GetTotalOrderPrice(order.OrderLines, products);
            var customer = customerServiceGateway.Get(order.CustomerId);
            if (customer == null) return false;
            if (customer.CreditStanding < totalCost) return false;
            return true;
        }

        private List<Product> GetProductsFromOrderLines(List<OrderLine> orderLines)
        {
            var products = new List<Product>();
            foreach (var orderLine in orderLines)
            {
                products.Add(productServiceGateway.Get(orderLine.ProductId));

            }
            return products;
        }

        private int GetTotalOrderPrice(List<OrderLine> orderLines, List<Product> products)
        {
            var totalCost = 0;
            foreach (var orderLine in orderLines)
            {
                var orderedProduct = products.First(p => p.Id == orderLine.ProductId);
                var cost = orderedProduct.Price * orderLine.Quantity;
                totalCost = (int)(totalCost + cost);
            }
            return totalCost;
        }

        // Edit status of order
        [HttpPut("status/{id}")]
        public IActionResult EditOrderStatus(int id, StatusEnums status)
        {
            var orderToEdit = repository.Get(id);
            if (orderToEdit == null)
            {
                return BadRequest();
            }

            try
            {

                switch (status)
                {
                    //notify product service
                    case StatusEnums.cancelled:
                    case StatusEnums.shipped:
                        messagePublisher.PublishOrderStatusChangedMessage(
                            orderToEdit.CustomerId, orderToEdit.OrderLines, status.ToString());
                        break;

                    //notify customer service
                    case StatusEnums.paid:
                        //count total cost
                        var orderedProducts = GetProductsFromOrderLines(orderToEdit.OrderLines);
                        var totalCost = GetTotalOrderPrice(orderToEdit.OrderLines, orderedProducts);
                        messagePublisher.PublishOrderPaidMessage(
                            orderToEdit.CustomerId, totalCost, status.ToString());
                        break;
                }

                
                // Update order
                orderToEdit.Status = status;
                repository.Edit(orderToEdit);
                return Ok(orderToEdit);
            }
            catch
            {
                return StatusCode(500, "An error happened. Try again.");
            }
        }

    }
}
