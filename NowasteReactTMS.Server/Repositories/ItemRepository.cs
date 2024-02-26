using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class ItemRepository : IItemRepository
{
    private readonly IConnectionFactory connectionFactory;

    public ItemRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<List<Item>> GetItems()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var items = await connection.QueryAsync<Item>(@"
                SELECT i.[ItemPK]
                      ,i.[ItemID] 
                      ,i.[Name]
                      ,i.[Weight]
                      ,i.[Volume]
                      ,i.[TransportTemp]
                      ,i.[StorageTemp]
                      ,i.[Company]
                      ,i.[isActive]
                      ,i.[EditName]
                  FROM [dbo].[Item] i");

            return items.ToList();
        }
    }

    public async Task<Item> GetItem(Guid id)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var item = await connection.QueryFirstOrDefaultAsync<Item>(@"
                SELECT i.[ItemPK]
                      ,i.[ItemID] 
                      ,i.[Name]
                      ,i.[Weight]
                      ,i.[Volume]
                      ,i.[TransportTemp]
                      ,i.[StorageTemp]
                      ,i.[Company]
                      ,i.[isActive]
                      ,i.[EditName]
                  FROM [dbo].[Item] i
                WHERE i.[ItemPK] = @id",
                new { id });

            return item;
        }
    }

    public async Task<Item> UpdateItem(Guid id, Item item)
    {
        if (await GetItem(id) == null)
            throw new Exception($"Item {id} not found.");

        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Item]
                SET    [Name] = @Name
                      ,[Weight] = @Weight
                      ,[Volume] = @Volume
                      ,[TransportTemp] = @TransportTemp
                      ,[StorageTemp] = @StorageTemp
                      ,[Company] = @Company
                      ,[isActive] = @isActive
                      ,[EditName] = @EditName
                      ,[TimeStamp] = GETDATE()
                WHERE [ItemID] = @ItemID", item);
        }
        return item;
    }

    public async Task<Item> CreateItem(Item item)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Item]
                            ([ItemPK]
                            ,[ItemID]
                            ,[Name]
                            ,[Weight]
                            ,[Volume]
                            ,[TransportTemp]
                            ,[StorageTemp]
                            ,[Company]
                            ,[isActive]
                            ,[EditName]
                            ,[TimeStamp]
                )

                VALUES
                            (@ItemPK
                            ,@ItemID
                            ,@Name
                            ,@Weight
                            ,@Volume
                            ,@TransportTemp
                            ,@StorageTemp
                            ,@Company
                            ,@isActive
                            ,@EditName
                            ,GETDATE())", item);
        }

        return item;
    }

    public async Task<Guid> DeleteItem(Guid id)
    {
        if (await GetItem(id) == null)
            throw new Exception($"Item {id} not found.");

        using (var connection = connectionFactory.CreateConnection())
        {
            await connection.ExecuteAsync(@"
                UPDATE [dbo].[Item]
                SET    [isActive] = 0
                      ,[TimeStamp] = GETDATE()
                WHERE [ItemPK] = @id",
                id);
        }

        return id;
    }
}