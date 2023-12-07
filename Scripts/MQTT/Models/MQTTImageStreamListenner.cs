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

        private Action<ReferenceModels.Image> _processors;

        // deprecated
        [Header("Stream")]
        [SerializeField]
        private RawImage display;
        private Texture2D _texture;

        private bool _isListenning = true;
        private Coroutine _curCoroutine;

        public override string[] Topics => TOPICS;
        public Action<ReferenceModels.Image> Processors
        {
            set => this._processors += value;
        }

        public delegate void Action(ReferenceModels.Image img);

        public override void ProcessMessage(string topic, string message)
        {
            Debug.Log("$MQTT image listened.");
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
            ReferenceModels.Image img = null;
            try
            {
                img = UnityEngine.JsonUtility.FromJson<ReferenceModels.Image>(message);
            }
            catch (Exception e)
            {
                Debug.LogError("MQTT ${topic} ERROR:\tnão é possível converter uma mensagem recebida.");
                this.StopListenning();
            }
            Debug.Log($"MQTT ${topic} RECEIVE:\n{img.ToString()}.");

            this._processors?.Invoke(img);
            this._isListenning = true;
            yield return new WaitForEndOfFrame();
        }

        public override void OnConnectionEvent()
        {
            base.OnConnectionEvent();
            base._mqttCC.AddListenner(this);
            this._isListenning = true;
        }

        public override void OnDisconnectionEvent()
        {
            base.OnDisconnectionEvent();
            base._mqttCC.RemoveListenner(this);
            this._isListenning = false;
        }

        protected override void Awake()
        {
            base.Awake();

            this._texture = ImageCores.Utils.TextureUtil.ReinitializeTexture(this._texture, 1, 1);
        }

    }
}
