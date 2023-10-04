using System.Text;
using System.Text.Json;
using RabbitMQ.Client;

namespace FormulaAirline.Services
{
    public class MessageProducer : IMessageProducer
    {
        string EXCHANGE_NAME = "DemoExchange";
        string ROUTING_KEY = "demo-routing-key";
        string QUEUE_NAME = "bookings";
        private readonly IConfiguration _configuration;
        private IConnection _connection;
        private IModel _channel;

        public MessageProducer(IConfiguration configuration)
        {
            _configuration = configuration;


            var factory = new ConnectionFactory()//Add Hostname from configuration (appsettings)
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
                _channel.QueueDeclare(QUEUE_NAME, durable: false, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueBind(queue: QUEUE_NAME, exchange: EXCHANGE_NAME, routingKey: ROUTING_KEY, arguments: null);
                _connection.ConnectionShutdown += RabbitMQ_Connection_ConnectionShutdown;

                Console.WriteLine("--> Connected to MessageBus");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not connect to the Message Bus: {ex} ");
            }
        }

        public void PublichNewMessage<T>(T message)
        {
            var strMessage = JsonSerializer.Serialize(message);

            if (_connection.IsOpen)
            {
                Console.WriteLine("--> RabbitMQ Connection is open, sending message...");
                SendMessage(strMessage);
            }
            else
            {
                Console.WriteLine("--> RabbitMQ Connection is closed, not sending...");
            }
        }

        private void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(exchange: EXCHANGE_NAME, routingKey: ROUTING_KEY, basicProperties: null, body: body);

            Console.WriteLine($"--> We have sent {message}");
        }

        public void Dispose()
        {
            if (_channel.IsOpen)
            {
                Console.WriteLine("MessageBus disposed");
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_Connection_ConnectionShutdown(object sender, ShutdownEventArgs e)
        {
            Console.WriteLine("--> RabbitMQ Connection Shutdown");
        }
    }
}