namespace CustomerApi.Data;

public interface IDbInitializer
{
    void Initialize(CustomerDbContext context);
}