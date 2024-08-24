using System.Text.Json;
using Dapr.Client;

namespace FormulaAirline.Services
{
    public class MessageProducer : IMessageProducer
    {
        private readonly string PUBSUB_NAME = "bookings_pubsub";
        private readonly string TOPIC_NAME = "bookings";
        private readonly DaprClient _daprClient;

        public MessageProducer()
        {
            _daprClient = new DaprClientBuilder().Build();
        }

        public async Task PublishNewMessageAsync<T>(T message)
        {
            try
            {
                await _daprClient.PublishEventAsync(PUBSUB_NAME, TOPIC_NAME, message);  // Pass the object directly
                Console.WriteLine($"--> Published message: {JsonSerializer.Serialize(message)}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Error publishing message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"--> Inner exception: {ex.InnerException.Message}");
                }
            }
        }
    }
}
