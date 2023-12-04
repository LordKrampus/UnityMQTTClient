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

        private Action _processors;

        protected Vector3 position = Vector3.zero;
        protected Quaternion rotation = Quaternion.identity;

        [Header("Stream")]
        [SerializeField]
        private GameObject rvAvatar;

        public override string[] Topics => TOPICS;
        public Vector3 Position => this.position;
        public Quaternion Rotation = Quaternion.identity;

        public Action Processors
        {
            set => this._processors += value;
        }

        public delegate void Action(ReferenceModels.Orientation img);


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

            this._processors?.Invoke(ort);
            
            //Debug.Log(img.GetBytes());

            //this.rvAvatar.transform.position = ort.Position;
            //this.rvAvatar.transform.localRotation = ort.Rotation;

            //this.position = ort.Position;
            //this.rotation = ort.Rotation;
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

        protected virtual void Update()
        {
        }
    }
}
