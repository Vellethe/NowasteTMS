using NowasteTms.Model;

public interface ICurrencyRepository
{
    Task<List<Currency>> GetAll();
    Task<Currency> Get(Guid id);
}