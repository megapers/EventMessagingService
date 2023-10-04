using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiverWebAPI.AsyncDataServices
{
    //Background service
    public class MessageBusSubscriber : BackgroundService
    {
        string EXCHANGE_NAME = "DemoExchange"; //Move to config
        string ROUTING_KEY = "demo-routing-key";
        private readonly IConfiguration _configuration;
        private readonly int _eventProce;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IConfiguration configuration)
        {
            _configuration = configuration;
            InitilizeRabbitMQ();
        }

        private void InitilizeRabbitMQ()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _configuration["RabbitMQHost"],
                Port = int.Parse(s: _configuration["RabbitMQPort"]),
                // UserName = "user",
                // Password = "user",
                // VirtualHost = "/"
                //Uri = new Uri(uriString: "amqp://guest:guest@localhost:5672")
            };

            try
            {
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();
                _channel.ExchangeDeclare(exchange: EXCHANGE_NAME, type: ExchangeType.Direct);
                _queueName = _channel.QueueDeclare().QueueName;
                _channel.QueueBind(queue: _queueName, exchange: EXCHANGE_NAME, routingKey: ROUTING_KEY, arguments: null);

                Console.WriteLine("--> Listening on the Message Bus...");

                _connection.ConnectionShutdown += RabbitMQ_Connection_ConnectionShutdown;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex} ");
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();

            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (sender, args) =>
            {
                Console.WriteLine("--> Event Received");

                var body = args.Body.ToArray();
                var notificationMessage = Encoding.UTF8.GetString(body);

                Console.WriteLine(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            return Task.CompletedTask;
        }

        private void RabbitMQ_Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }

        public override void Dispose()
        {
            if (_channel.IsOpen)
            {
                Console.WriteLine("MessageBus disposed");
                _channel.Close();
                _connection.Close();
            }
        }
    }
}
