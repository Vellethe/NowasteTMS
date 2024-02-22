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
}