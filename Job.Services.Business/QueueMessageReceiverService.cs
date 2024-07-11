using Job.Data.Contracts.Helpers;
using Job.Data.Contracts.Helpers.DTO.Message;
using Job.Services.Contracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Job.Services.Business;
public class QueueMessageReceiverService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly string _queueName;
    private readonly string _exchangeName;
    private readonly string _routingKey;

    public QueueMessageReceiverService
        (
            IServiceProvider serviceProvider, 
            IConfiguration configuration
        )
    {
        _configuration = configuration;
        _serviceProvider = serviceProvider;

        _queueName = _configuration["UserQueueCredentials:QueueName"];
        _exchangeName = _configuration["UserQueueCredentials:ExchangeName"];
        _routingKey = _configuration["UserQueueCredentials:RoutingKey"];

        var hostname = Environment.GetEnvironmentVariable("USER_QUEUE_HOSTNAME") ?? _configuration["UserQueueCredentials:Hostname"];

        var factory = new ConnectionFactory
        {
            HostName = hostname,
            Port = int.Parse(_configuration["UserQueueCredentials:Port"]),
            UserName = _configuration["UserQueueCredentials:UserName"],
            Password = _configuration["UserQueueCredentials:Password"]
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

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        _channel.QueueBind
            (
                queue: _queueName, 
                exchange: _exchangeName,
                routingKey: _routingKey
            );

        consumer.Received += async (model, eventArgs) =>
        {
            var body = eventArgs.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            await ProcessMessageAsync(message);
        };

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5, stoppingToken);
        }
    }

    private async Task ProcessMessageAsync(string message)
    {
        try
        {
            var userMessage = JsonConvert.DeserializeObject<UserMessageDto>(message);
            var scope = _serviceProvider.CreateScope();

            if (userMessage.MessageType == AppConstants.USER_DELETE_MESSAGE)
            {
                var _queueMessageProcesserService = scope.ServiceProvider.GetRequiredService<IQueueMessageProcesserService>();
                await _queueMessageProcesserService.DeleteUserAsync(userMessage.UserMessageDetails.UserProfileId);
            }
            else if (userMessage.MessageType == AppConstants.RECOMMEND_JOBS_MESSAGE)
            {
                var _queueMessageProcesserService = scope.ServiceProvider.GetRequiredService<IQueueMessageProcesserService>();

                await _queueMessageProcesserService.RecommendJobsAsync(userMessage.UserMessageDetails);
            }
            else
            {
                throw new Exception("Invalid message type");
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error processing message: {ex.Message}");
        }
    }
}
