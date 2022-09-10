namespace com.tweetapp.Controller.KafkaProducer;

public interface IKafkaProducer
{
    void Publish(string bootstrap, string message);
}