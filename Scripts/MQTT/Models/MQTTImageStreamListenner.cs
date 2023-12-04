using System;
using System.Linq;

using UnityEngine.UI;
using UnityEngine;

using uPLibrary.Networking.M2Mqtt.Messages;

using ReferenceModels;
using System.Collections;

namespace MQTT.Models
{
    public class MQTTImageStreamListenner : MQTTListenner
    {
        private static string[] TOPICS = new string[] { "device/holo/image" };

        private Action _processors;

        // deprecated
        [Header("Stream")]
        [SerializeField]
        private RawImage display;
        private Texture2D _texture;

        private bool _isListenning = true;
        private Coroutine _curCoroutine;

        public override string[] Topics => TOPICS;
        public Action Processors
        {
            set => this._processors += value;
        }

        public delegate void Action(ReferenceModels.Image img);

        public override void ProcessMessage(string topic, string message)
        {
            if (!this._isListenning)
                return;

            this._isListenning = false;
            this._curCoroutine = this.StartCoroutine(this.Listenning(topic, message));
        }

        private void StopListenning()
        {
            this._isListenning = true;
            this.StopCoroutine(this._curCoroutine);
        }

        private IEnumerator Listenning(string topic, string message)
        {
            yield return new WaitForEndOfFrame();

            ReferenceModels.Image img = null;
            try
            {
                img = UnityEngine.JsonUtility.FromJson<ReferenceModels.Image>(message);
            }
            catch (Exception e)
            {
                Debug.Log("MQTT ${topic} ERROR:\tnão é possível converter uma mensagem recebida.");
                this.StopListenning();
            }
            Debug.Log($"MQTT ${topic} RECEIVE:\n{img.ToString()}.");

            this._processors?.Invoke(img);
            this._isListenning = true;

            //if(this._texture.width != img.Width || this._texture.height != img.Height)
            //{
            //    this._texture = ImageCores.Utils.TextureUtil.ReinitializeTexture(this._texture, img.Width, img.Height);
            //}
            //this._texture.LoadImage(img.ImgBytes);
            //this._texture.Apply();
            //
            //display.texture = this._texture;
            //yield return new WaitForEndOfFrame();
            //this._isListenning = true;
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

            this._texture = ImageCores.Utils.TextureUtil.ReinitializeTexture(this._texture, 1, 1);
        }

    }
}
