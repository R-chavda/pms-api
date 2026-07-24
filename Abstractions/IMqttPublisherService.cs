namespace Abstractions;

public interface IMqttPublisherService
{
    Task PublishTaskUpdateAsync(string taskKeyId, string oldStatus,string newStatus);
}