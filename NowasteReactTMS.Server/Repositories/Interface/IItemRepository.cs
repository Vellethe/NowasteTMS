using NowasteTms.Model;

public interface IItemRepository
{
    Task<List<Item>> GetItems();
    Task<Item> GetItem(Guid id);
    Task<Item> UpdateItem(Guid id, Item agent);
    Task<Item> CreateItem(Item agent);
    Task<Guid> DeleteItem(Guid id);
}