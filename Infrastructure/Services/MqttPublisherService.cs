using System.Text.Json;
using Abstractions;
using HiveMQtt.Client;
using HiveMQtt.Client.Options;
using HiveMQtt.Client.Results;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;
public class MqttPublisherService : IMqttPublisherService
{
    private readonly ILogger<MqttPublisherService> _logger;
    private HiveMQClientOptions options = new HiveMQClientOptions();
    private HiveMQClient client;
    private ConnectResult connectResult = null!;
    public MqttPublisherService(ILogger<MqttPublisherService> logger)
    {
        _logger = logger;
        options.Host = "6eac87322c1746b8907ff6c869f803a1.s1.eu.hivemq.cloud";
        options.Port = 8883;
        options.UserName = "CoderGuy";
        options.Password = "Coder123";
        client = new HiveMQClient(options);
    }

    public async Task PublishTaskUpdateAsync(string taskKeyId, string oldStatus,string newStatus)
    {
        _logger.LogInformation("Connecting to mqtt");
        connectResult = await client.ConnectAsync().ConfigureAwait(false);
        _logger.LogInformation($"Connected to MQTT? : {connectResult.ReasonCode}");
        _logger.LogInformation("Connected!! now creating message");
        var payloadObj = new
        {
            taskKeyId,
            oldStatus,
            newStatus
        };
        await client.PublishAsync(
              "pms/task-update",           // Topic to publish to
              $"{JsonSerializer.Serialize(payloadObj)}"       // Message to publish
        ).ConfigureAwait(false);
        _logger.LogInformation($"published the message");
    }
}