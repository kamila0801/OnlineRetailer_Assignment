using SharedModels;

namespace CustomerApi.Data;

public interface IRepository<T>
{
    T Get(int id);
    T Add(T customer);
    T Update(T customer);
    bool Delete(int id);
}