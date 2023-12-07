using MQTT;
using MQTT.Models;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT {
    public class MQTTStarter : MonoBehaviour
    {
        private static string[] MQTT_TOPICS = new string[] { "info/#" };

        [SerializeReference] 
        private List<MQTTEntity> queueOnClientSetup = new List<MQTTEntity>();
        [SerializeField]
        protected MQTTClientController _mqttCC;

        public void Start()
        {
            this._mqttCC.OnConnection = OnConnection;
        }

        public void OnConnection()
        {
            this._mqttCC.SubscribeTopic("info/#", MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
        }

        /*
        public override void OnClientSetup()
        {
            base.MQTTCC.AddListenner(this);

            foreach (string topic in MQTT_TOPICS)
                base.MQTTCC.SubscribeTopic(topic, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
        }
        */

        /*
        public void QueueOnClientSetup(MQTTEntity entity)
        {
            this.queueOnClientSetup.Add(entity);
        }
        */
        /*
        protected override void Awake()
        {
            base.Awake();

            base.MQTTCC.SetBrokerAddress(this.brokerAdress);
            base.MQTTCC.SetBrokerPort(this.brokerPort);

            this.StartCoroutine(this.SetupConnection());
        }
        */

        /*
        // espera o cliente ativo para cadastrar seu listenner
        private IEnumerator SetupConnection()
        {
            //base.MQTTCC.Connect();
            while (!base.MQTTCC.IsConnected)
                yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            foreach (MQTTEntity entity in this.queueOnClientSetup)
            {
                entity.OnClientSetup();
            }
            //this.queueOnClientSetup.Clear();
        }
        */

        /*
        private void LateUpdate()
        {
            if(this.status != null)
                this.status.text = base.MQTTCC.IsConnected? "connected" : "not-connected";
        }
        */

    }
}