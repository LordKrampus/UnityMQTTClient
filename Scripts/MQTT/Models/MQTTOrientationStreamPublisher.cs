using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using ReferenceModels;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace MQTT.Models
{
    public class MQTTOrientationStreamPublisher : MQTTPublisher
    {
        private static string[] TOPICS = new string[] { "device/holo/orientation" };

        [Header("Source")]
        [SerializeField]
        private GameObject player;

        public override string[] Topics => TOPICS;

        public override void PublishMessage(string topic, string message)
        {
            base.PublishMessage(topic, message);
        }

        public override void OnConnectionEvent()
        {
            base.OnConnectionEvent();

            foreach (string topic in this.Topics)
                base._mqttCC.SubscribeTopic(topic, MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE);
        }

        public override void OnDisconnectionEvent()
        {
            base.OnDisconnectionEvent();
        }

        protected override void Awake()
        {
            base.Awake();

            if (this.player == null)
                Debug.Log("Error raise on Inspector - object(s) reference(s) needed.");
        }

        private void LateUpdate()
        {
            if (base.Process)
            {
                Transform playerT = player.transform;
                Vector3 position = playerT.position;
                Quaternion rotation = playerT.localRotation;

                Orientation ort = new Orientation(position, rotation);
                this.PublishMessage(ort);
            }
        }

        protected virtual void PublishMessage(Orientation ort)
        {
            this.PublishMessage(this.Topics[0], UnityEngine.JsonUtility.ToJson(ort, true));
        }
    }
}
