using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Direct);

channel.QueueDeclare(queue: "dynamic_routing_queue",
  durable: false,
  exclusive: false,
  autoDelete: false,
  arguments: null);

IList<string> routingKeys = [];

channel.QueueBind(queue: "dynamic_routing_queue",
  exchange: "logs",
  routingKey: "");

var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) =>
{
  var body = ea.Body.ToArray();
  var message = Encoding.UTF8.GetString(body);
  var routingKey = ea.RoutingKey;
  Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
  Console.WriteLine("");
  routingKeys.Add(message);
  Console.WriteLine($" [x] Added routing key: '{message}'");
  Console.WriteLine($" [x] Current routing keys: {string.Join(", ", routingKeys)}");
  Console.WriteLine("");
};

channel.BasicConsume(queue: "dynamic_routing_queue",
  autoAck: true,
  consumer: consumer
);

var message = "Test log message";
var body = Encoding.UTF8.GetBytes(message);

var run = true;

while (run)
{
  foreach (var routingKey in routingKeys)
  {
    channel.BasicPublish(exchange: "logs",
      routingKey: routingKey,
      basicProperties: null,
      body: body);  

    Console.WriteLine($" [x] Sent '{routingKey}':'{message}'");
  }
  Console.WriteLine("Press any key to send another message or [q] to exit.");
  var key = Console.ReadKey();
  Console.WriteLine("");
  if (key.Key == ConsoleKey.Q)
  {
    run = false;
  }
}
Console.WriteLine("Goodbye!");