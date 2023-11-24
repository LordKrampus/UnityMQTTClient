using System;
using System.Linq;

using UnityEngine.UI;
using UnityEngine;

using uPLibrary.Networking.M2Mqtt.Messages;

using OCR.TextureEnhancement;
using MRModels;

namespace MQTT.Models
{
    public class MQTTImageStreamListenner : MQTTListenner
    {
        private static string[] TOPICS = new string[] { "device/holo/image" };

        [Header("Stream")]
        [SerializeField]
        private RawImage display;

        public override string[] Topics => TOPICS;

        public override void ProcessMessage(string topic, string message)
        {
            MRModels.Image img;
            try
            {
                img = UnityEngine.JsonUtility.FromJson<MRModels.Image>(message);
            }catch(Exception e)
            {
                Debug.Log("MQTT ${topic} ERROR:\tnão é possível converter uma mensagem recebida.");
                return;
            }
            Debug.Log($"MQTT ${topic} RECEIVE:\n{img.ToString()}.");
            //Debug.Log(img.GetBytes());

            Color32[] colors;
            Utilities.TextureUtilities.BytesToColors(img.Width, img.Height, img.GetBytes(), out colors);
            Texture2D imgTexture = new Texture2D(img.Width, img.Height);
            imgTexture.SetPixels32(colors);

            //imgTexture.LoadImage(img.ImgBytes);
            imgTexture.Apply();

            display.texture = imgTexture;
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
