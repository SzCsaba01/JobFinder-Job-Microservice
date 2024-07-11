using Job.Data.Contracts.Helpers.DTO.Message;
using Job.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Job.Services.Business;
public class QueueMessageSenderService : IQueueMessageSenderService, IDisposable
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly string _exchangeName;
    private readonly string _routingKey;

    public QueueMessageSenderService(IConfiguration configuration)
    {
        _configuration = configuration;
        _queueName = _configuration["JobQueueCredentials:QueueName"];
        _exchangeName = _configuration["JobQueueCredentials:ExchangeName"];
        _routingKey = _configuration["JobQueueCredentials:RoutingKey"];

        var hostname = Environment.GetEnvironmentVariable("JOB_QUEUE_HOSTNAME") ?? _configuration["JobQueueCredentials:Hostname"];
        var port = Environment.GetEnvironmentVariable("JOB_QUEUE_PORT") ?? _configuration["JobQueueCredentials:Port"];

        var factory = new ConnectionFactory() 
        {
            HostName = hostname,
            Port = int.Parse(port),
            UserName = _configuration["JobQueueCredentials:Username"],
            Password = _configuration["JobQueueCredentials:Password"]
        };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.QueueDeclare
            (
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );
        
        _channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
    }

    public async Task SendUserFeedbackEmailsAsync(JobEmailMessageDto message)
    {
        await Task.Run(() =>
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            _channel.BasicPublish
                (
                    exchange: _exchangeName,
                    routingKey: _routingKey,
                    basicProperties: null,
                    body: body
                );
        });
    }

    public void Dispose()
    {
        _channel?.Dispose();
        _connection?.Dispose();
    }
}
