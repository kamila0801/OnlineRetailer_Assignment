using SharedModels;
using Microsoft.EntityFrameworkCore;

namespace CustomerApi.Data;

public class CustomerRepository : IRepository<Customer>
{
    private readonly CustomerDbContext _db;
    
    public CustomerRepository(CustomerDbContext context)
    {
        _db = context;
    }
    
    public Customer Get(int id)
    {
        return _db.Customers.Where(c => c.Id == id).FirstOrDefault();
    }

    public Customer Add(Customer customer)
    {
        var newCustomer = _db.Customers.Add(customer).Entity;
        _db.SaveChanges();
        return newCustomer;
    }

    public Customer Update(Customer customer)
    {
        _db.Attach(customer).State = EntityState.Modified;
        _db.SaveChanges();
        return customer;
    }

    public bool Delete(int id)
    {
        var deleted = _db.Customers.FirstOrDefault(d => d.Id == id);
        _db.Customers.Remove(deleted);
        _db.SaveChanges();
        return true;
    }
}