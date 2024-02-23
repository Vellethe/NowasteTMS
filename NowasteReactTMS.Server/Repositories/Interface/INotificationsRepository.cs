using NowasteTms.Model;

public interface INotificationsRepository
{
    Task<List<Notification>> Get();
    Task<Notification> Create(Notification notification);
    Task<int> SetCompleted(string id, string userId);
    Task<int> Remove(string id, string userId);
    Task<int> SetArchived(Guid transportOrderId, string getUserId);
}