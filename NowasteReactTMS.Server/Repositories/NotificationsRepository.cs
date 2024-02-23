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

    public Task<Notification> Create(Notification notification)
    {
        throw new NotImplementedException();
    }

    public Task<List<Notification>> Get()
    {
        throw new NotImplementedException();
    }

    public Task<int> Remove(string id, string userId)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetArchived(Guid transportOrderId, string getUserId)
    {
        throw new NotImplementedException();
    }

    public Task<int> SetCompleted(string id, string userId)
    {
        throw new NotImplementedException();
    }
}