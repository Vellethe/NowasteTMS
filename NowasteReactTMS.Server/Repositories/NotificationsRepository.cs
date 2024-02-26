using Dapper;
using NowasteTms.Model;
using WMan.Data.ConnectionFactory;
public class NotificationsRepository : INotificationsRepository
{
    private readonly IConnectionFactory connectionFactory;

    public NotificationsRepository(IConnectionFactory connectionFactory)
    {
        this.connectionFactory = connectionFactory;
    }

    public async Task<List<Notification>> Get()
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var notifications = await connection.QueryAsync<Notification>(@"
            SELECT n.[NotificationPK]
            	  ,n.[Level]
            	  ,n.[Message]
            	  ,n.[Comment]
            	  ,n.[OrderPK]
            	  ,n.[TransportOrderPK]
            	  ,n.[Created]
            	  ,n.[CompletedByUserPK]
            	  ,sci.[Country] AS FromCountry
            	  ,cci.[Country] AS ToCountry
              FROM [dbo].[Notifications] n
            LEFT JOIN [dbo].[Order] po ON po.[OrderPK] = n.[OrderPK]
            LEFT JOIN [dbo].[Customer] cu ON po.[CustomerPK] = cu.[CustomerPK]
            LEFT JOIN [BusinessUnit] cbu ON cu.[BusinessUnitPK] = cbu.[BusinessUnitPK]
            LEFT JOIN [dbo].[ContactInformation] cci ON cci.[ContactInformationPK] =
            		( -- zero to many contact rows, so join only first contact line
            		SELECT TOP 1 c.[ContactInformationPK]
            		FROM [ContactInformation] c
            		WHERE c.[IsDefault] = 1 AND c.[IsActive] = 1
            		AND c.[BusinessUnitPK] = cbu.[BusinessUnitPK]
            	)
            LEFT JOIN [Supplier] s ON po.[SupplierPK] = s.[SupplierPK]
            LEFT JOIN [BusinessUnit] sbu ON s.[BusinessUnitPK] = sbu.[BusinessUnitPK]
            LEFT JOIN [dbo].[ContactInformation] sci ON sci.[ContactInformationPK] =
            		( -- zero to many contact rows, so join only first contact line
            		SELECT TOP 1 c.[ContactInformationPK]
            		FROM [ContactInformation] c
            		WHERE c.[IsDefault] = 1 AND c.[IsActive] = 1
            		AND c.[BusinessUnitPK] = sbu.[BusinessUnitPK]
            	)
                WHERE n.[Created] > DATEADD(month, -2, GETDATE())
                  AND n.[Level] < 4 ");
            return notifications.ToList();
        }
    }

    private async Task<Notification> Get(Guid pk)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            var notification = await connection.QueryAsync<Notification>(@"
                SELECT [NotificationPK]
                      ,[Level]
                      ,[Message]
                      ,[Comment]
                      ,[OrderPK]
                      ,[TransportOrderPK]
                      ,[Created]
                      ,[CompletedByUserPK]
                  FROM [dbo].[Notifications]
                WHERE [NotificationPK] = @pk",
                param: new
                {
                    pk
                });
            return notification.FirstOrDefault();
        }
    }

    public async Task<Notification> Create(Notification notification)
    {
        using (var connection = connectionFactory.CreateConnection())
        {
            notification.Created = DateTime.Now;

            await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Notifications]
                            ([NotificationPK]
                            ,[Level]
                            ,[Message]
                            ,[Comment]
                            ,[OrderPK]
                            ,[TransportOrderPK]
                            ,[Created])

                VALUES
                            (@NotificationPK
                            ,@Level
                            ,@Message
                            ,@Comment
                            ,@OrderPK
                            ,@TransportOrderPK
                            ,@Created)",
                notification);
        }

        return notification;
    }

    public async Task<int> SetCompleted(string id, string userId)
    {
        var notificationPk = new Guid(id);
        var userPk = new Guid(userId);
        if (await Get(notificationPk) == null)
            throw new Exception($"Notification id {notificationPk} not found");

        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                 UPDATE [dbo].[Notifications]
                    SET [Level] = @Level
                       ,[CompletedByUserPK] = @UserPK
                       ,[Created] = @CurrentTimestamp
                  WHERE [NotificationPK] = @notificationPk
                    AND [Level] = @LevelToDone",
                new
                {
                    notificationPk,
                    UserPK = userPk,
                    Level = (int)Levels.Done,
                    CurrentTimestamp = DateTime.Now,
                    LevelToDone = (int)Levels.Action
                });
        }
    }

    public async Task<int> Remove(string id, string userId)
    {
        var notificationPk = new Guid(id);
        var notification = await Get(notificationPk);
        if (notification == null)
            throw new Exception($"Notification id {notificationPk} not found");

        using (var connection = connectionFactory.CreateConnection())
        {
            notification.Created = DateTime.Now;

            return await connection.ExecuteAsync(@"
                INSERT INTO [dbo].[Notifications]
                            ([NotificationPK]
                            ,[Level]
                            ,[Message]
                            ,[Comment]
                            ,[OrderPK]
                            ,[TransportOrderPK]
                            ,[Created])

                VALUES
                            (@NotificationPK
                            ,@Level
                            ,@Message
                            ,@Comment
                            ,@OrderPK
                            ,@TransportOrderPK
                            ,@Created)",
                notification);
        }
    }

    public async Task<int> SetArchived(Guid transportOrderId, string userId)
    {
        var userPk = new Guid(userId);
        using (var connection = connectionFactory.CreateConnection())
        {
            return await connection.ExecuteAsync(@"
                UPDATE [dbo].[Notifications]
                    SET [Level] = @Level
                       ,[CompletedByUserPK] = @UserPK
                       ,[Created] = @CurrentTimestamp
                WHERE [TransportOrderPK] = @transportOrderId
                    AND [Level] >= @LevelToArchive",
                new
                {
                    transportOrderId,
                    UserPK = userPk,
                    Level = (int)Levels.Archived,
                    CurrentTimestamp = DateTime.Now,
                    LevelToArchive = (int)Levels.Done
                });
        }
    }
}