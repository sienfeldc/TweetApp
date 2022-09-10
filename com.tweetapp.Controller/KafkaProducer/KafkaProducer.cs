using System.Configuration;
using System.Net;
using Confluent.Kafka;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace com.tweetapp.Controller.KafkaProducer;

public class KafkaProducer: IKafkaProducer
{
    private readonly IConfiguration _configuration;

    private string bootstrap;
    //private string boostrap = ConfigurationManager.GetSection("Kafka:bootstrap").ToString();
    //private readonly string bootstrap = ConfigurationManager.AppSettings["Kafka:bootstrap"]!;
    //private string bootstrap = "kafka:9092";
    //private string bootstrap = "localhost:9092";
    private string topic = "TweetAppTopic";

    public KafkaProducer(IConfiguration configuration)
    {
        _configuration = configuration;

        bootstrap = _configuration.GetConnectionString("kafka");
        Console.WriteLine("inside constructor",bootstrap);
    }
    public void Publish(string bootstrap, string message)
    {
        // Action<DeliveryReport<Null, string>> handler = r => 
        //     Console.WriteLine(!r.Error.IsError
        //         ? $"Delivered message to {r.TopicPartitionOffset}"
        //         : $"Delivery Error: {r.Error.Reason}");

        using var producer =
            new ProducerBuilder<long, string>(new ProducerConfig { BootstrapServers = bootstrap,
                EnableDeliveryReports = true,
                ClientId = Dns.GetHostName(),
                Debug = "msg",
                Acks = Acks.All,
                // Number of times to retry before giving up
                MessageSendMaxRetries = 2,
                // Duration to retry before next attempt
                RetryBackoffMs = 1000,
                // Set to true if you don't want to reorder messages on retry
                EnableIdempotence = true
            })
                .SetKeySerializer(Serializers.Int64)
                .SetValueSerializer(Serializers.Utf8)
                .SetLogHandler((_, message) =>
                    Console.WriteLine($"Facility: {message.Facility}-{message.Level} Message: {message.Message}"))
                .SetErrorHandler((_, e) => Console.WriteLine($"Error: {e.Reason}. Is Fatal: {e.IsFatal}"))
                .Build();
        try
        {
            Console.WriteLine("\nProducer started...\n\n"+bootstrap);
            var deliveryReport = producer.ProduceAsync(topic, new Message<long, string>
            {               
                Key = DateTime.Now.Ticks,
                Value = "Time: "+DateTime.Now +" message: "+ message
            });
            
            Console.WriteLine($"Message sent (value: '{message}'). Produced to:{deliveryReport.Result.Topic}{deliveryReport.Result.Topic} Delivery status: {deliveryReport.Status}");
            if (deliveryReport.Status != (TaskStatus)PersistenceStatus.Persisted)
            {
                // delivery might have failed after retries. This message requires manual processing.
                Console.WriteLine(
                    $"ERROR: Message not acked by all brokers (value: '{message}'). Delivery status: {deliveryReport.Status}");
            }

            Thread.Sleep(TimeSpan.FromSeconds(5));
            //producer.Flush(TimeSpan.FromSeconds(10));
        }
        catch (Exception e)
        {
            Console.WriteLine($"Oops, something went wrong: {e}");
        }
        
        
        
        // using var producer =
        //     new ProducerBuilder<Null, string>(new ProducerConfig { BootstrapServers = bootstrap }).Build();
        // try
        // {
        //     producer.Produce(topic, new Message<Null, string> { Value = message }, handler);
        //     producer.Flush(TimeSpan.FromSeconds(10));
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine($"Oops, something went wrong: {e}");
        // }

    }
}