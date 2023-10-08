using SharedModels;

namespace CustomerApi.Data;

public class DbInitializer : IDbInitializer
{
    // This method will create and seed the database.
    public void Initialize(CustomerDbContext context)
    {
        context.Database.EnsureDeleted();
        context.Database.EnsureCreated();

        // Look for any Customers
        if (context.Customers.Any())
        {
            return;   // DB has been seeded
        }
        
        List<Customer> customers = new List<Customer>
        {
            new Customer { Name = "Susan Frog", Email = "susan@gmail.com", Phone = 12345678, 
                BillingAddress = "Sun Valley 5a, London", ShippingAddress = "Sun Valley 5a, London",
                CreditStanding = 500
            },
            new Customer { Name = "Jakob Donalds", Email = "jk@gmail.com", Phone = 22345878, 
                BillingAddress = "New Valley 66c, London", ShippingAddress = "New Valley 66c, London",
                CreditStanding = 1200
            }
        };

        context.Customers.AddRange(customers);
        context.SaveChanges();
    }
}