namespace MQTT
{
    public interface IMQTTEntity
    {
        public MQTTClientController MQTTCC { get; }
        public string[] Topics { get; }

        public void OnClientSetup();
    }
}
