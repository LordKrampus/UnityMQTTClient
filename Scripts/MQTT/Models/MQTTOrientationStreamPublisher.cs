using System;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

using MRModels;
using OCR.TextureEnhancement;

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

        protected override void Awake()
        {
            base.Awake();

            if (this.player == null)
                Debug.Log("Error raise on Inspector - object(s) reference(s) needed.");
        }

        private void LateUpdate()
        {
            if (base.MQTTCC.IsConnected)
            {
                Transform playerT = player.transform;
                Vector3 position = playerT.position;
                Vector3 rotation = playerT.rotation.eulerAngles;

                Orientation ort = new Orientation(position, rotation);
                this.PublishMessage(this.Topics[0], UnityEngine.JsonUtility.ToJson(ort, true));
            }

        }
    }
}
