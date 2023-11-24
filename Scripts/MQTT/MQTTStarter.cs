using MQTT;
using MQTT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT {
    public class MQTTStarter : MQTTListenner
    {
        private static string[] MQTT_TOPICS = new string[] { "info/#" };

        [SerializeField]
        private string brokerAdress = "192.168.0.37";
        [SerializeField]
        private string brokerPort = "1883";

        [SerializeReference] 
        private List<MQTTEntity> queueOnClientSetup = new List<MQTTEntity>();

        public override string[] Topics => MQTT_TOPICS;

        public override void ProcessMessage(string topic, string message)
        {
            Debug.Log($"listened for {topic}, message:\n{message}");
        }

        public override void OnClientSetup()
        {
            base.MQTTCC.AddListenner(this);

            foreach (string topic in MQTT_TOPICS)
                base.MQTTCC.SubscribeTopic(topic, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
        }

        public void QueueOnClientSetup(MQTTEntity entity)
        {
            this.queueOnClientSetup.Add(entity);
        }

        protected override void Awake()
        {
            base.Awake();

            base.MQTTCC.SetBrokerAddress(this.brokerAdress);
            base.MQTTCC.SetBrokerPort(this.brokerPort);

            this.StartCoroutine(this.SetupConnection());
        }

        // espera o cliente ativo para cadastrar seu listenner
        private IEnumerator SetupConnection()
        {
            while (!base.MQTTCC.IsConnected)
                yield return new WaitForEndOfFrame();

            foreach (MQTTEntity entity in this.queueOnClientSetup)
            {
                entity.OnClientSetup();
            }
            //this.queueOnClientSetup.Clear();
        }

    }
}