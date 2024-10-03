namespace MessagesManagerDev.Services
{
    public interface IKafkaProducer
    {
        Task ProduceMessage(string topic, string message);
    }
}
