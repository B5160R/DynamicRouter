using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

var factory = new ConnectionFactory() { HostName = "localhost" };

using var connection = factory.CreateConnection();
using var channel = connection.CreateModel();

var run = true;
var consumers = new List<Consumer>();

Console.WriteLine("Press [n] to add a new consumer or [q] to exit.");

while (run)
{
  foreach (var consumer in consumers)
  {
    consumer.ConsumeMessages();
  }

  if (Console.ReadKey().Key == ConsoleKey.Q)
  {
    Console.WriteLine("");
    run = false;
  }
  else if (Console.ReadKey().Key == ConsoleKey.N)
  {
    Console.WriteLine("");
    consumers.Add(new Consumer(channel));
  }
}

Console.WriteLine("Goodbye!");
public class Consumer
{
  private IModel _channel;
  private string _queueName;
  public Consumer(IModel channel)
  {
    _channel = channel;

    _channel.ExchangeDeclare(exchange: "logs", type: ExchangeType.Direct);

    _channel.QueueDeclare(queue: "dynamic_routing_queue",
      durable: false,
      exclusive: false,
      autoDelete: false,
      arguments: null);
    
    var messageRoutingKey = GetUserDefinedRoutingKey();

    _queueName = GetUserDefinedQueueName();

    _channel.BasicPublish(exchange: "logs",
      routingKey: "",
      basicProperties: null,
      body: Encoding.UTF8.GetBytes(messageRoutingKey));

    _channel.QueueDeclare(queue: _queueName,
      durable: false,
      exclusive: false,
      autoDelete: false,
      arguments: null);
    
    _channel.QueueBind(queue: _queueName,
      exchange: "logs",
      routingKey: messageRoutingKey);
    Console.WriteLine("");
    Console.WriteLine($" [x] Created consumer with routing key: '{messageRoutingKey}'");
    Console.WriteLine("");
  }

  public void ConsumeMessages()
  {
    var consumer = new EventingBasicConsumer(_channel);
    consumer.Received += (model, ea) =>
    {
      var body = ea.Body.ToArray();
      var message = Encoding.UTF8.GetString(body);
      var routingKey = ea.RoutingKey;
      Console.WriteLine($" [x] Received '{routingKey}':'{message}'");
      Console.WriteLine("");
    };

    _channel.BasicConsume(queue: _queueName,
      autoAck: true,
      consumer: consumer);

  }

  private string GetUserDefinedRoutingKey()
  {
    Console.WriteLine("Enter a routing key:");
    return Console.ReadLine();
  } 

  private string GetUserDefinedQueueName()
  {
    Console.WriteLine("Enter a queue name:");
    return Console.ReadLine();
  }
}