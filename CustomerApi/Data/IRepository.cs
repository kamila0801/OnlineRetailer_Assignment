using CustomerApi.Models;

namespace CustomerApi.Data;

public interface IRepository
{
    Customer Get(int id);
    Customer Add(Customer customer);
    Customer Update(Customer customer);
    bool Delete(int id);
}