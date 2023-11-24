namespace MQTT
{
    public interface IMQTTListenner
    {
        public void ProcessMessage(string topic, string message);
    }
}
