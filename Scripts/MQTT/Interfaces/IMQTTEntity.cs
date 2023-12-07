namespace MQTT
{
    public interface IMQTTEntity
    {
        //public MQTTClientController MQTTCC { get; }
        public string[] Topics { get; }
        public bool Process { get; }

        public void OnClientSetup();
    }
}
