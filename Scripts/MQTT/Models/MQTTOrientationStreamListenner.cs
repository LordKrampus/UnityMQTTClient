using System;
using System.Linq;

using UnityEngine.UI;
using UnityEngine;

using uPLibrary.Networking.M2Mqtt.Messages;

using ReferenceModels;

namespace MQTT.Models
{
    public class MQTTOrientationStreamListenner : MQTTListenner
    {
        private static string[] TOPICS = new string[] { "device/holo/orientation" };

        [Header("Stream")]
        [SerializeField]
        private GameObject rvAvatar;

        public override string[] Topics => TOPICS;

        public override void ProcessMessage(string topic, string message)
        {
            Orientation ort;
            try
            {
                ort = UnityEngine.JsonUtility.FromJson<Orientation>(message);
            }
            catch (Exception e)
            {
                Debug.Log("MQTT ${topic} ERROR:\tnão é possível converter uma mensagem recebida.");
                return;
            }
            Debug.Log($"MQTT ${topic} RECEIVE:\n{ort.ToString()}.");
            //Debug.Log(img.GetBytes());

            this.rvAvatar.transform.position = ort.Position;
            this.rvAvatar.transform.localRotation = ort.Rotation;
        }

        public override void OnClientSetup()
        {
            base.MQTTCC.AddListenner(this);

            foreach (string topic in this.Topics)
                base.MQTTCC.SubscribeTopic(topic, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
        }

        protected override void Awake()
        {
            base.Awake();
        }

    }
}
