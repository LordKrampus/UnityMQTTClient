namespace MQTT
{
    public interface IMQTTPublisher
    {
        public void PublishMessage(string topic, string message);
    }
}
