using System;
using System.Collections;
using UnityEngine;

using MQTT;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT.Models
{
    public abstract class MQTTEntity : MonoBehaviour, IMQTTEntity
    {
        [SerializeField]
        protected MQTTClientController _mqttCC;
        private bool _process = false;

        public abstract string[] Topics { get; }
        public bool Process => this._process;

        public virtual void OnConnectionEvent()
        {
            foreach(string topic in this.Topics)
                this._mqttCC.SubscribeTopic(topic, MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE);

            this._process = true;
        }

        public virtual void OnDisconnectionEvent()
        {
            this._process = false;
        }

        protected virtual void Awake()
        {
            this._mqttCC.OnConnection = this.OnConnectionEvent;
            this._mqttCC.OnDisconnectoin = this.OnDisconnectionEvent;

        }

        public virtual void OnClientSetup()
        { }
    }
}
