using System;
using System.Collections;
using UnityEngine;

using MQTT;

namespace MQTT.Models
{
    public abstract class MQTTEntity : MonoBehaviour, IMQTTEntity
    {
        private MQTTClientController _mqttCC;
        [SerializeField]
        private MQTTStarter mqttStartet;

        public MQTTClientController MQTTCC => this._mqttCC;
        public abstract string[] Topics { get; }

        protected virtual void Awake()
        {
            this._mqttCC = MQTTClientController.Instance;
            mqttStartet.QueueOnClientSetup(this);

            //#if UNITY_EDITOR
            //            Application.runInBackground = true;
            //#endif
        }

        public virtual void OnClientSetup()
        {
        }
    }
}
